import serial
import threading
import time
import pynmea2
import configparser

class GPSCompassModule:
    def __init__(self, port='/dev/ttyTHS1', baud=9600, config_path="config.ini"):

        config = configparser.ConfigParser()
        config.read(config_path)
        try:
            self.gps_enable = config.getboolean("GPS", "gps_enable", fallback=False)
        except Exception:
            self.gps_enable = False

        # GPS (NMEA) serial setup
        self.lat = None
        self.lng = None

        if self.gps_enable:  # retry loop for port
            while True:
                try:
                    self.serial = serial.Serial(port, baud, timeout=1)
                    self.serial.flushInput()
                    break  # port opened successfully; exiting the loop
                except serial.SerialException as e:
                    print(f"[WARN] Cannot open GPS serial port {port}: {e}. Retrying in 5 s...")
                    time.sleep(5)
        else:
            # gps_enable=False => static coordinates
            self.lat = 44.42922012101214  # 45.9432
            self.lng = 26.094207448022612  # 24.9668

        # Thread control
        self.running = False
        self.thread = None

    def parse_nmea_line(self, line):
        try:
            msg = pynmea2.parse(line)

            if isinstance(msg, pynmea2.GGA):
                self.lat = msg.latitude
                self.lng = msg.longitude

            elif isinstance(msg, pynmea2.RMC):
                self.lat = msg.latitude
                self.lng = msg.longitude

        except pynmea2.ParseError:
            pass  # ignore NMEA parsing errors
        except Exception as e:
            print(f"Unexpected error: {e}")

    def _gps_worker(self):
        # If GPS is disabled, we exit the worker immediately
        if not self.gps_enable:
            return

        while self.running:
            try:
                raw = self.serial.readline()
                if not raw:
                    time.sleep(0.1)
                    continue

                line = raw.decode('ascii', errors='replace').strip()
                if line.startswith('$'):
                    self.parse_nmea_line(line)

            except serial.SerialException as e:
                print(f"[GPS ERROR] {e}")
                time.sleep(1)
            except pynmea2.ParseError:
                pass  # ignore corrupted NMEA sentences

    def start(self):
        """Start GPS reading thread and clear old buffer."""
        if not self.gps_enable:
            return

        if not self.running:
            self.serial.reset_input_buffer()
            self.running = True
            self.thread = threading.Thread(target=self._gps_worker, daemon=True)
            self.thread.start()

    def stop(self):
        """Stop thread È™i clean up."""
        if not self.gps_enable:
            return

        self.running = False
        if self.thread:
            self.thread.join()
        if self.serial.is_open:
            self.serial.close()

    def get_coordinates(self):
        """
      Return the latest GPS coordinates as (lat, lng),
        or None if we don't have a valid fix.
        """
        if not self.gps_enable:
            # GPS disabled => we return static coordinates
            return (self.lat, self.lng)

        if self.lat is None or self.lng is None:
            return None

        if self.lat == 0.0 and self.lng == 0.0:
            return None

        return (self.lat, self.lng)