// /js/assign-calendar.js
"use strict";
(function () {
    $(document).ready(function () {
         var appUrl = $("#appUrl").val();
        initCalendar();
        bindTeamChange();
    });
    // --- State ---
    let flatpickrInstance = null;
    let occupiedExpanded = new Set(); // set de "YYYY-MM-DD" pentru marcarea ocupatelor

    // --- Utils ---
    const toISO = (d) => (d instanceof Date ? d : new Date(d)).toISOString().split("T")[0];

    function getDateRange(from, to) {
        const start = new Date(from), end = new Date(to);
        const days = [];

        start.setHours(12, 0, 0, 0); end.setHours(12, 0, 0, 0);
        for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
            days.push(toISO(d));
        }
        return days;
    }

    function expandOccupied(occupied) {
        // Acceptă: ["2025-08-14", {from:"2025-08-18", to:"2025-08-20"}]
        const result = new Set();
        (occupied || []).forEach(item => {
            if (typeof item === "string") {
                result.add(item);
            } else if (item && item.from && item.to) {
                getDateRange(item.from, item.to).forEach(d => result.add(d));
            }
        });
        return result;
    }

    function updateOccupiedDates(occupiedList) {
        // DOAR marcăm vizual; NU dezactivăm, ca să putem selecta „peste” ele
        occupiedExpanded = expandOccupied(occupiedList || []);
        if (flatpickrInstance) flatpickrInstance.redraw();
    }

    function initCalendar() {
        const modalEl = document.getElementById("assignToDepartamentModal");
        if (!modalEl) return;

        modalEl.addEventListener("shown.bs.modal", function () {
            if (flatpickrInstance) flatpickrInstance.destroy();

            const freeDaysStart = "2025-08-18";
            const freeDaysEnd = "2025-08-28";
            const freeDays = getDateRange(freeDaysStart, freeDaysEnd);
            const startDay = freeDays[0];
            const endDay = freeDays[freeDays.length - 1];

            flatpickrInstance = flatpickr("#calendarAssignDate", {
                inline: true,
                mode: "range",
                dateFormat: "Y-m-d",
                minDate: "today",
                locale: "ro",
                disableMobile: true,
                defaultDate: [startDay, endDay],

                onDayCreate: function (_dObj, _dStr, _fp, dayElem) {
                    const dateISO = toISO(dayElem.dateObj);

                    // marcări pentru free days (dacă le păstrezi)
                    if (freeDays.length) {
                        if (dateISO === startDay) dayElem.classList.add("startSelectedFreeDaysRange");
                        else if (dateISO === endDay) dayElem.classList.add("endSelectedFreeDaysRange");
                        else if (freeDays.includes(dateISO)) dayElem.classList.add("inSelectedFreeDaysRange");
                        if (freeDays.includes(dateISO)) dayElem.classList.add("free-day");
                    }

                    // marcări vizuale pentru „ocupat”
                    if (occupiedExpanded.has(dateISO)) {
                        dayElem.classList.add("occupied-day");
                        // opțional: capete/interior de interval ocupat
                        const prev = new Date(dayElem.dateObj); prev.setDate(prev.getDate() - 1);
                        const next = new Date(dayElem.dateObj); next.setDate(next.getDate() + 1);
                        const hasPrev = occupiedExpanded.has(toISO(prev));
                        const hasNext = occupiedExpanded.has(toISO(next));
                        if (!hasPrev && hasNext) dayElem.classList.add("occupied-start");
                        if (hasPrev && !hasNext) dayElem.classList.add("occupied-end");
                        if (hasPrev && hasNext) dayElem.classList.add("occupied-inrange");
                    }
                },

                onChange: function (selectedDates, dateStr, _instance) {
                    const hidden = document.getElementById("assignDateHidden");

                    // 0) reset
                    if (selectedDates.length === 0) {
                        if (hidden) hidden.value = "";
                        console.log("Selecție resetată");
                        return;
                    }

                    // 1) o singură zi (primul click)
                    if (selectedDates.length === 1) {
                        const only = selectedDates[0];
                        const iso = toISO2(only);
                        if (hidden) hidden.value = iso;
                        console.log("Zi selectată:", iso);
                        return;
                    }

                    let [a, b] = selectedDates;
                    if (a > b) { const t = a; a = b; b = t; }

                    const startISO = toISO2(a);
                    const endISO = toISO2(b);

                    const fullRange = getDateRange(a, b);
                    const filtered = fullRange.filter(d => !occupiedExpanded.has(d));

                    if (hidden) hidden.value = `${startISO} to ${endISO}`;

                    console.log("Interval selectat (fără zile ocupate):", filtered);
                    console.log("Start:", startISO, "End:", endISO);
                }
            });

            // --- HARD CODED ocupate (până ai serverul) ---
            const occupiedTest = [
                "2025-08-14",                           // zi singulară
                { from: "2025-08-18", to: "2025-08-20" } // interval
            ];
            updateOccupiedDates(occupiedTest);
        });
    }

    function toISO2(d) {
        const date = (d instanceof Date ? d : new Date(d));
        const y = date.getFullYear();
        const m = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${y}-${m}-${day}`;
    }

    // --- Schimbarea echipei (exemplu: poți înlocui cu fetch real când e gata backend-ul) ---
    async function fetchOccupiedDatesForTeam(teamId) {
        // simulăm răspunsuri diferite
        switch (String(teamId || "")) {
            case "1":
                return ["2025-08-22", { from: "2025-08-26", to: "2025-08-28" }];
            case "2":
                return [{ from: "2025-09-02", to: "2025-09-05" }];
            default:
                return [];
        }
    }

    function bindTeamChange() {
        // jQuery pentru compatibilitate
        $(document).on("change", 'select[name="AssignToDepartment.TeamId"]', async function () {
            if (!flatpickrInstance) return;
            const teamId = $(this).val();
            try {
                const data = await fetchOccupiedDatesForTeam(teamId);
                updateOccupiedDates(data);
                console.log("Zile ocupate pentru echipă:", teamId, data);
            } catch (err) {
                console.error("Eroare la preluarea zilelor ocupate:", err);
            }
        });
    }


})();
