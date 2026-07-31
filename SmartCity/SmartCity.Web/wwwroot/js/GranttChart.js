    "use strict";

    (function () {
        $(document).ready(function () {
            const tasks = window.ganttTasks || [];
            if (!Array.isArray(tasks) || tasks.length === 0) {
                console.warn("No Gantt data found.");
                return;
            }

            google.charts.load("current", { packages: ["gantt"] });
            google.charts.setOnLoadCallback(drawChart);

            function drawChart() {
                const data = new google.visualization.DataTable();
                data.addColumn("string", "Task ID");
                data.addColumn("string", "Task Name"); // textul rândului
                data.addColumn("string", "Resource");
                data.addColumn("date", "Start Date");
                data.addColumn("date", "End Date");
                data.addColumn("number", "Duration");
                data.addColumn("number", "Percent Complete");
                data.addColumn("string", "Dependencies");

                for (const t of tasks) {
                    data.addRow([
                        t.TaskId,
                        t.TaskName,
                        t.Resource,
                        new Date(t.Start),
                        new Date(t.End),
                        null,
                        t.PercentComplete,
                        t.Dependencies || null
                    ]);
                }

                const container = document.getElementById("chartGrantt");
                const chart = new google.visualization.Gantt(container);

                const options = {
                    width: "100%",
                    height: 550,
                    gantt: {
                        trackHeight: 40,
                        palette: [
                            { color: "#ba85ef", dark: "#0d47a1", light: "#90caf9" },
                            { color: "#8be188", dark: "#1b5e20", light: "#a5d6a7" },
                            { color: "#84dbde", dark: "#b71c1c", light: "#ef9a9a" }
                        ],
                        labelStyle: { fontName: "Arial", fontSize: 12, color: "#000" },
                        labelColumnWidth: 350
                    }
                };

                // click pe bară -> detalii (păstrat)
                google.visualization.events.addListener(chart, "select", function () {
                    const sel = chart.getSelection();
                    if (sel.length > 0) {
                        const row = sel[0].row;
                        const taskId = data.getValue(row, 0);
                        const appUrl = $("#appUrl").val();
                        const $trigger = $("#app-loader-container");
                        jsUtils.postJsonData(
                            $trigger,
                            appUrl + "Detection/GetDetectionInfoData",
                            { Id: taskId },
                            true,
                            function (response) {
                                $("#taskInfoContainer").html(response);
                            }
                        );
                    }
                });

                chart.draw(data, options);

                if (!container.dataset.gvtxTipSetup) {
                    elevateGoogleTooltip(container);
                    container.dataset.gvtxTipSetup = "1";
                }

                function elevateGoogleTooltip() {
                    const containerRect = () => container.getBoundingClientRect();

                    // Repoziționează tooltip-ul pe baza valorilor left/top pe care Google le-a setat (relative la container)
                    function repositionTip(tip) {
                        const cr = containerRect();
                        const baseLeft = parseFloat(tip.dataset.gvtxLeftOrig || tip.style.left || '0') || 0;
                        const baseTop = parseFloat(tip.dataset.gvtxTopOrig || tip.style.top || '0') || 0;
                        tip.style.left = (cr.left + baseLeft) + 'px';
                        tip.style.top = (cr.top + baseTop) + 'px';
                    }

                    // Atașează observer pe tooltip ca să-l repoziționeze de fiecare dată când Google îi modifică stilul
                    function attachTipObservers(tip) {
                        // Mută în <body> dacă nu e deja
                        if (tip.parentElement !== document.body) {
                            document.body.appendChild(tip);
                        }
                        tip.classList.add('gvt-fixed');
                        tip.style.zIndex = '2147483647';
                        tip.style.pointerEvents = 'auto';

                        // Observă schimbările de stil aplicate de Google (de obicei left/top)
                        const tipObs = new MutationObserver((muts) => {
                            for (const m of muts) {
                                if (m.type === 'attributes' && m.attributeName === 'style') {
                                    // Salvează valorile „originale” setate de Google (relative la container)
                                    const l = parseFloat(tip.style.left || '0') || 0;
                                    const t = parseFloat(tip.style.top || '0') || 0;
                                    tip.dataset.gvtxLeftOrig = String(l);
                                    tip.dataset.gvtxTopOrig = String(t);
                                    // Convertim în coordonate de viewport (pt. position:fixed)
                                    repositionTip(tip);
                                }
                            }
                        });
                        tipObs.observe(tip, { attributes: true, attributeFilter: ['style'] });

                        // Re-calculează la resize/scroll (dacă se schimbă poziția containerului pe ecran)
                        const onWinChange = () => repositionTip(tip);
                        window.addEventListener('scroll', onWinChange, { passive: true });
                        window.addEventListener('resize', onWinChange);

                        // Poziționează imediat dacă deja are stil stabilit
                        repositionTip(tip);
                    }

                    // Observă apariția tooltip-ului în container, apoi îl „hijeck-uim”
                    const rootObs = new MutationObserver(() => {
                        const tip = container.querySelector('.google-visualization-tooltip');
                        if (tip && !tip.classList.contains('gvt-fixed')) {
                            attachTipObservers(tip);
                        }
                    });
                    rootObs.observe(container, { childList: true, subtree: true });

                    // Dacă tooltipul exista deja (rareori), îl prindem imediat
                    const existing = container.querySelector('.google-visualization-tooltip');
                    if (existing && !existing.classList.contains('gvt-fixed')) {
                        attachTipObservers(existing);
                    }
                };

                window.addEventListener("resize", () => chart.draw(data, options));



                google.visualization.events.addListener(chart, 'ready', function () {
                    // rulează după ce finalizează toate transform-urile
                    requestAnimationFrame(() => requestAnimationFrame(placeLabelsUsingFirstColumn));
                });

                function placeLabelsUsingFirstColumn() {
                    const svg = container.getElementsByTagName("svg")[0];
                    if (!svg) return;

                    const NS = "http://www.w3.org/2000/svg";

                    // curăță elementele noastre anterioare
                    svg.querySelectorAll("text.gantt-row-label-overbar, rect.gantt-row-label-bg, rect.gantt-row-extbar, g.gantt-daily-axis")
                        .forEach(n => n.remove());

                    const TRACK_H = options?.gantt?.trackHeight ?? 40;
                    const LABEL_W = options?.gantt?.labelColumnWidth ?? 300;
                    const X_LABEL_THRESH = LABEL_W + 20;
                    const Y_TOL = 8;
                    const MIN_FONT = 12, MAX_FONT = 12, PAD = 10;

                    // 1) etichetele rândurilor din prima coloană
                    const rowLabels = (() => {
                        const texts = Array.from(svg.querySelectorAll("text"));
                        const rows = [];
                        for (const t of texts) {
                            const x = parseFloat(t.getAttribute("x") || "0");
                            const y = parseFloat(t.getAttribute("y") || "0");
                            const anchor = t.getAttribute("text-anchor") || "";
                            const label = (t.textContent || "").trim();
                            if (label && x <= X_LABEL_THRESH && anchor !== "middle") {
                                rows.push({ y, text: label });
                            }
                        }
                        rows.sort((a, b) => a.y - b.y);
                        const dedup = [];
                        for (const r of rows) {
                            const last = dedup[dedup.length - 1];
                            if (!last || Math.abs(last.y - r.y) > Y_TOL) dedup.push(r);
                        }
                        return dedup;
                    })();
                    if (!rowLabels.length) return;

                    // 2) rect-urile barelor reale (excludem ce nu pare bară)
                    const rects = Array.from(svg.getElementsByTagName("rect")).filter(r => {
                        if (r.classList.contains("gantt-row-extbar")) return false; // exclude overlay-urile noastre
                        const fill = r.getAttribute("fill");
                        const h = parseFloat(r.getAttribute("height") || "0");
                        const w = parseFloat(r.getAttribute("width") || "0");
                        const looksLikeBarHeight = h >= (TRACK_H * 0.35) && h <= (TRACK_H * 0.95);
                        const hasVisibleFill = fill && fill !== "none" && fill !== "#ffffff";
                        return w > 0 && looksLikeBarHeight && hasVisibleFill;
                    });
                    if (!rects.length) return;

                    // 3) calculăm „plot area” din poziția barelor (stânga/dreapta/sus/jos)
                    let plotLeft = Infinity, plotRight = -Infinity, plotTop = Infinity, plotBottom = -Infinity;
                    rects.forEach(r => {
                        const x = parseFloat(r.getAttribute("x") || r.x?.baseVal?.value || 0);
                        const y = parseFloat(r.getAttribute("y") || r.y?.baseVal?.value || 0);
                        const w = parseFloat(r.getAttribute("width") || r.width?.baseVal?.value || 0);
                        const h = parseFloat(r.getAttribute("height") || r.height?.baseVal?.value || 0);
                        plotLeft = Math.min(plotLeft, x);
                        plotRight = Math.max(plotRight, x + w);
                        plotTop = Math.min(plotTop, y);
                        plotBottom = Math.max(plotBottom, y + h);
                    });

                    // >>> NEW: daily axis (sub bare) bazat pe min/max din DataTable
                    (function drawDailyAxis() {
                        // min/max din date (coloanele 3 = Start, 4 = End)
                        let tMin = Infinity, tMax = -Infinity;
                        for (let i = 0; i < data.getNumberOfRows(); i++) {
                            const s = data.getValue(i, 3);
                            const e = data.getValue(i, 4);
                            if (s instanceof Date) tMin = Math.min(tMin, +s);
                            if (e instanceof Date) tMax = Math.max(tMax, +e);
                        }
                        if (!isFinite(tMin) || !isFinite(tMax) || tMax <= tMin) return;

                        // rotunjim la zile (00:00) pentru capete
                        const d0 = new Date(tMin); d0.setHours(0, 0, 0, 0);
                        const d1 = new Date(tMax); d1.setHours(0, 0, 0, 0);

                        // mapare timp→x pe baza plotLeft/Right
                        const span = d1 - d0 || 1;
                        const toX = (t) => plotLeft + ((t - d0) / span) * (plotRight - plotLeft);

                        // grup separat pentru axă
                        const g = document.createElementNS("http://www.w3.org/2000/svg", "g");
                        g.setAttribute("class", "gantt-daily-axis");
                        g.style.pointerEvents = "none";
                        svg.appendChild(g);

                        // liniile verticale + etichetele „dd.MM” sub bare
                        for (let d = new Date(d0); d <= d1; d.setDate(d.getDate() + 1)) {
                            const x = toX(+d);
                            // linie verticală discretă
                            const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
                            line.setAttribute("x1", x.toFixed(1));
                            line.setAttribute("y1", (plotTop - 4).toFixed(1));
                            line.setAttribute("x2", x.toFixed(1));
                            line.setAttribute("y2", (plotBottom + 2).toFixed(1));
                            line.setAttribute("stroke", "#e5e7eb6b"); // gri foarte deschis
                            line.setAttribute("stroke-width", "1");
                            g.appendChild(line);

                            // etichetă sub bare
                            const dd = String(d.getDate()).padStart(2, "0");
                            const mm = String(d.getMonth() + 1).padStart(2, "0");
                            const label = `${dd}.${mm}`;

                            const t = document.createElementNS("http://www.w3.org/2000/svg", "text");
                            t.setAttribute("x", x.toFixed(1));
                            t.setAttribute("y", (plotBottom + 16).toFixed(1)); // sub bare
                            t.setAttribute("text-anchor", "middle");
                            t.setAttribute("dominant-baseline", "middle");
                            t.setAttribute("fill", "#6b7280"); // muted
                            t.style.fontSize = "10px";
                            t.style.fontFamily = "Arial, sans-serif";
                            t.textContent = label;
                            g.appendChild(t);
                        }
                    })();
                    // <<< END NEW

                    // 4) (restul funcției tale) – grupare pe rând + desenat etichetele albe peste bare
                    const buckets = [];
                    for (const r of rects) {
                        const y = parseFloat(r.getAttribute("y") || r.y?.baseVal?.value || 0);
                        const h = parseFloat(r.getAttribute("height") || r.height?.baseVal?.value || 0);
                        const cy = y + h / 2;
                        let b = buckets.find(bb => Math.abs(bb.y - cy) <= Y_TOL);
                        if (!b) { b = { y: cy, rects: [] }; buckets.push(b); }
                        b.rects.push(r);
                    }
                    buckets.sort((a, b) => a.y - b.y);

                    const XW_TOL = 2;
                    function dedupeRowRects(rowRects) {
                        const items = rowRects.map(r => ({
                            r,
                            x: parseFloat(r.getAttribute("x") || r.x?.baseVal?.value || 0),
                            w: parseFloat(r.getAttribute("width") || r.width?.baseVal?.value || 0),
                            y: parseFloat(r.getAttribute("y") || r.y?.baseVal?.value || 0),
                            h: parseFloat(r.getAttribute("height") || r.height?.baseVal?.value || 0),
                            fill: r.getAttribute("fill") || "#888",
                            stroke: r.getAttribute("stroke") || null
                        }));
                        items.sort((a, b) => (a.x - b.x) || (b.w - a.w));
                        const kept = [];
                        for (const it of items) {
                            const dup = kept.find(k => Math.abs(k.x - it.x) <= XW_TOL && Math.abs(k.w - it.w) <= XW_TOL);
                            if (!dup) kept.push(it);
                        }
                        return kept;
                    }

                    function findClosestRowLabel(y) {
                        let best = null, bestDy = Infinity;
                        for (const rl of rowLabels) {
                            const dy = Math.abs(rl.y - y);
                            if (dy < bestDy) { bestDy = dy; best = rl; }
                        }
                        return best?.text || "";
                    }

                    const svgWidth = parseFloat(svg.getAttribute("width")) || svg.getBoundingClientRect().width || 99999;

                    buckets.forEach((bucket, rowIdx) => {
                        const uniq = dedupeRowRects(bucket.rects);
                        const rowText = findClosestRowLabel(bucket.y);

                        uniq.forEach(it => {
                            const { x, y, w, h, fill, stroke } = it;

                            let fontSize = MAX_FONT;
                            let textW = measureTextWidth(rowText, fontSize);
                            while (textW > Math.max(0, w - PAD) && fontSize > MIN_FONT) {
                                fontSize -= 1;
                                textW = measureTextWidth(rowText, fontSize);
                            }

                            let drawW = w;
                            if (textW > Math.max(0, drawW - PAD)) {
                                const needed = textW + PAD;
                                drawW = Math.max(drawW, needed);
                                drawW = Math.min(drawW, svgWidth - x - 5);

                                const ext = document.createElementNS("http://www.w3.org/2000/svg", "rect");
                                ext.classList.add("gantt-row-extbar");
                                ext.setAttribute("x", String(x));
                                ext.setAttribute("y", String(y));
                                ext.setAttribute("width", String(drawW));
                                ext.setAttribute("height", String(h));
                                ext.setAttribute("rx", "4");
                                ext.setAttribute("ry", "4");
                                ext.setAttribute("fill", fill);
                                if (stroke) ext.setAttribute("stroke", stroke);
                                ext.setAttribute("opacity", "1");
                                ext.style.pointerEvents = "none";
                                svg.appendChild(ext);
                            }

                            const cx = x + drawW / 2;
                            const cy = y + h / 2;

                            const t = document.createElementNS("http://www.w3.org/2000/svg", "text");
                            t.classList.add("gantt-row-label-overbar");
                            t.setAttribute("x", String(cx));
                            t.setAttribute("y", String(cy));
                            t.setAttribute("text-anchor", "middle");
                            t.setAttribute("dominant-baseline", "middle");
                            t.setAttribute("fill", "#fff");
                            t.style.fontSize = fontSize + "px";
                            t.style.fontFamily = "Arial, sans-serif";
                            t.style.fontWeight = "500";
                            t.style.pointerEvents = "none";
                            t.textContent = rowText;
                            svg.appendChild(t);
                        });
                    });

                    function measureTextWidth(txt, fontSizePx) {
                        const temp = document.createElementNS("http://www.w3.org/2000/svg", "text");
                        temp.setAttribute("x", "0");
                        temp.setAttribute("y", "0");
                        temp.style.fontSize = fontSizePx + "px";
                        temp.style.fontFamily = "Arial, sans-serif";
                        temp.textContent = txt;
                        svg.appendChild(temp);
                        const width = temp.getComputedTextLength();
                        svg.removeChild(temp);
                        return width;
                    }
                }




            }
        });
})();
