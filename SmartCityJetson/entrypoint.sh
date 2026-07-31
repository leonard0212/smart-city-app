#!/bin/bash
set -euo pipefail

# 0) (Optional) performance boost if available
which nvpmodel      &>/dev/null && nvpmodel -m 0
which jetson_clocks &>/dev/null && jetson_clocks

# 1) Read the host’s GPIO GID, default to 997
HOST_GID=$(stat -c '%g' /dev/gpiochip0 2>/dev/null || echo 997)

# 2) Align the container’s gpio group to that GID
groupmod -g "${HOST_GID}" gpio 2>/dev/null || true

# 3) Give root:gpio ownership & rw- access
chown root:gpio /dev/gpiochip* 2>/dev/null || true
chmod 660   /dev/gpiochip* 2>/dev/null || true

## 4) Detect GPS serial port (ttyTHS* preferred, fallback to ttyS*)
#GPS_ACTUAL=$(ls /dev/ttyTHS* 2>/dev/null | head -n1)
#if [ -z "$GPS_ACTUAL" ]; then
#  GPS_ACTUAL=$(ls /dev/ttyS* 2>/dev/null | grep -E '/dev/ttyS[0-9]+' | head -n1)
#fi
#
## 5) Symlink found GPS port to /dev/ttyS0 to match default in code
#if [ -n "$GPS_ACTUAL" ]; then
#  ln -sf "$GPS_ACTUAL" /dev/ttyS0
#  echo "[entrypoint] GPS serial port $GPS_ACTUAL -> /dev/ttyS0"
#else
#  echo "[entrypoint] WARNING: no GPS serial port found"
#fi

# 6) SCHEDULER (RO timezone) or when manually overridden
# Reads START_TIME / END_TIME / TZ_RO from env (set in deployment.json).
# Falls back to 08:00–17:00 Romania time if envs are missing.
# Runs your app ($@) only inside the window; stops it exactly at END_TIME.

START_TIME="${START_TIME:-08:00}"                # fallback e.g. "08:00"
END_TIME="${END_TIME:-17:00}"                    # fallback e.g. "17:00"
TZ_RO="${TZ_RO:-EET-2EEST,M3.5.0/3,M10.5.0/4}"   # POSIX TZ for Romania (handles DST)
MANUAL_FLAG="/tmp/manual_override"               # touch to force RUN regardless of schedule
MANUAL_SLEEP_FLAG="/tmp/manual_sleep"            # touch to force STOP immediately

SLEEP_SECONDS="${SLEEP_SECONDS:-60}"             # how often to re-check schedule
PGID_FILE="/tmp/app_pgid"                        # marker file for the app process group

# Format seconds as HH:MM:SS
fmt_hms() {
  local s=${1:-0}; (( s<0 )) && s=0
  printf "%02d:%02d:%02d" $((s/3600)) $(((s%3600)/60)) $((s%60))
}

# "HH:MM" -> total minutes (safe with leading zeros)
hm_to_min() { IFS=: read -r h m <<< "$1"; echo $((10#$h*60 + 10#$m)); }

# Are we inside the configured window right now? (RO timezone)
is_in_window() {
  local now_s start_s end_s
  local now start end

  now_s=$(TZ="$TZ_RO" date +%H:%M)
  start_s="$START_TIME"
  end_s="$END_TIME"

  now=$(hm_to_min "$now_s")
  start=$(hm_to_min "$start_s")
  end=$(hm_to_min "$end_s")

  if (( start < end )); then
    # e.g. 08:00..17:00
    (( now >= start && now < end ))
  else
    # overnight window e.g. 17:00..08:00
    (( now >= start || now < end ))
  fi
}

# Seconds until END_TIME (today, else tomorrow) in RO TZ
secs_until_end() {
  local now end_today end
  now=$(TZ="$TZ_RO" date +%s)
  end_today=$(TZ="$TZ_RO" date -d "$(TZ="$TZ_RO" date +%F) $END_TIME" +%s 2>/dev/null || true)

  if [[ -n "$end_today" && "$end_today" -gt "$now" && "$START_TIME" != "$END_TIME" ]]; then
    end="$end_today"
  else
    end=$(TZ="$TZ_RO" date -d "$(TZ="$TZ_RO" date -d tomorrow +%F) $END_TIME" +%s)
  fi
  echo $(( end - now ))
}

# (Optional) wait a bit for sane clock after power loss (max 60s)
MAX_WAIT="${WAIT_FOR_CLOCK_SECS:-60}"
while [ "$MAX_WAIT" -gt 0 ]; do
  year=$(date +%Y)
  if [ "$year" -ge 2025 ]; then break; fi
  echo "[entrypoint] waiting for clock sync... ($MAX_WAIT s left)"
  sleep 2
  MAX_WAIT=$((MAX_WAIT-2))
done

# Helpers: write/validate PGID & start/stop app
write_pgid() {
  echo "$1 $(awk '{print $22}' /proc/$1/stat 2>/dev/null)" > "$PGID_FILE"
}

is_same_process_group() {
  read pid start < "$PGID_FILE" || return 1
  [[ -n "$pid" && -d "/proc/$pid" ]] || return 1
  cur_start=$(awk '{print $22}' /proc/$pid/stat 2>/dev/null) || return 1
  [[ "$cur_start" = "$start" ]]
}

start_app() {
  setsid "$@" & app_pid=$!
  write_pgid "$app_pid"
  echo "[entrypoint] Starting app PGID=$app_pid"
}

stop_app() {
  if [[ -s "$PGID_FILE" ]] && is_same_process_group; then
       read pid _ < "$PGID_FILE"
       echo "[entrypoint] Stopping app PGID=$pid (TERM → KILL)"
       kill -TERM -"$pid" 2>/dev/null || true
       for i in {1..30}; do
         kill -0 "$pid" 2>/dev/null || break
         sleep 1
       done
       # safety sweep for any children that may have changed process group
       pkill -TERM -P "$pid" 2>/dev/null || true
       sleep 1
       kill -KILL -"$pid" 2>/dev/null || true
       pkill -KILL -P "$pid" 2>/dev/null || true
     fi
     rm -f "$PGID_FILE"
}

  # If entrypoint receives TERM/INT (stop/redeploy), stop the application cleanly
  trap 'stop_app; exit 0' TERM INT


# Main loop:
#  If MANUAL override -> run app without time limit.
#  If inside SCHEDULE -> run app until END_TIME; otherwise it doesn't start.

while true; do
  # FORCE OFF global: don't start anything if set
  if [[ -f "$MANUAL_SLEEP_FLAG" ]]; then
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Manual sleep active, NOT starting the app"
    sleep "$SLEEP_SECONDS"
    continue
  fi

  # FORCE OFF in manual override (stops immediately if manual_sleep occurs)
  if [[ -f "$MANUAL_FLAG" ]]; then
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Manual override active, running app without time limit: $*"
    if command -v setsid >/dev/null 2>&1; then
      start_app "$@"

      # propagate TERM/INT to the entire group if the entrypoint is stopped (redeploy/stop)
      trap 'kill -TERM -"$app_pid" 2>/dev/null || true' TERM INT
      # wait, but interrupt immediately if manual_sleep is requested
      while kill -0 "$app_pid" 2>/dev/null; do
        if [[ -f "$MANUAL_SLEEP_FLAG" ]]; then
          echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Manual sleep requested, stopping app (TERM -> KILL)"
          stop_app
          break
        fi
        sleep 1
      done

      # clean
      # restore global trap
      trap 'stop_app; exit 0' TERM INT
      unset app_pid
      rm -f "$PGID_FILE"
    else
      # fallback if setsid is missing
      "$@" || true
    fi
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") App exited (manual override); re-check in ${SLEEP_SECONDS}s"
    sleep "$SLEEP_SECONDS"
   continue
  fi

  if is_in_window; then
    dur=$(secs_until_end)
    if [[ "${dur:-0}" -le 0 ]]; then
      echo "[entrypoint] In schedule, but duration<=0; re-check in ${SLEEP_SECONDS}s"
      sleep "$SLEEP_SECONDS"
      continue
    fi

    now_str=$(TZ="$TZ_RO" date "+%F %T %Z")
    rem_hms=$(fmt_hms "$dur")
    echo "[entrypoint] $now_str In schedule, running until END_TIME in ${rem_hms}: $*"

# Run the application until END_TIME and stop the ENTIRE process-group
if command -v setsid >/dev/null 2>&1; then
  # starts in a new session
  start_app "$@"



  # FORCE OFF within the scheduled window
  # if entrypoint receives TERM/INT (redeploy/stop), propagate to the entire group
  trap 'kill -TERM -"$app_pid" 2>/dev/null || true' TERM INT
 # wait until END_TIME, but track WHY we stop (end_time/manual_sleep/app_exit)
 stop_reason="end_time"
 for ((i=0; i<dur; i++)); do
   if [[ -f "$MANUAL_SLEEP_FLAG" ]]; then
     stop_reason="manual_sleep"
     break
  fi

  if ! kill -0 "$app_pid" 2>/dev/null; then
    stop_reason="app_exit"
    break
  fi
  sleep 1
done


if kill -0 "$app_pid" 2>/dev/null; then
   # process still alive → we must stop it (reason: end_time OR manual_sleep)
   if [[ "$stop_reason" == "manual_sleep" ]]; then
     echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Manual sleep active, stopping app (TERM -> KILL in 30s)"
  else
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Reached END_TIME, stopping app (TERM -> KILL in 30s)"
  fi

  stop_app
 else
   # app already exited before we got here
   if [[ "$stop_reason" == "app_exit" ]]; then
     echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") App exited before END_TIME; re-check in ${SLEEP_SECONDS}s"
  else
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Stopped early due to manual sleep; re-check in ${SLEEP_SECONDS}s"
  fi
fi

  # clear for next iteration
  # restore global trap
  trap 'stop_app; exit 0' TERM INT
  unset app_pid
  rm -f "$PGID_FILE"
else
  # Fallback if setsid is missing: keep timeout
  timeout --preserve-status --signal=TERM --kill-after=30s "${dur}s" "$@" || true
fi

# when we are NOT in the window, do not start the application
else
  if [[ -s "$PGID_FILE" ]] && is_same_process_group; then
    echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Outside schedule but app is still running, stopping it now"
    stop_app
  fi
  echo "[entrypoint] $(TZ="$TZ_RO" date "+%F %T %Z") Outside schedule ($START_TIME-$END_TIME, RO), application is in sleep mode"

fi
  sleep "$SLEEP_SECONDS"
done

## 7) Finally, run app
#exec "$@"

