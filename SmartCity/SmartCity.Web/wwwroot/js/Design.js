"use strict";
(function () {
    //Search function
    $(document).ready(function () {
        var appUrl = $("#appUrl").val();

        let isDragging = false;
        let startX, startY;
        let translateX = 0, translateY = 0;

        //$("#loadingId").show();

        $('.slideBar-item-container.sc-slideBar').click(function () {
            $('.slideBar-item.mga-slideBar').removeClass("slideBar-item-active")
            $(this).addClass("slideBar-item-active");
        });

        $('#sidebarCollapse').click(function () {
            $('#sidebar').toggleClass('slideBar-not-active');
            $('#sidebar').toggleClass('active');
            $('.slideBar-item-container.sc-slideBar').toggleClass('slideBar-not-active');
            $('.sidebar-item-text').toggleClass('slideBar-not-active');
            $('.sidebar-item-text').toggleClass('slideBar-active');
        });

        $('#adminSubMenu').click(function () {
            $('.slideBar-item-container').removeClass('slideBar-ic-active')
            $('#adminSubmenuContainer').toggleClass('sub-menu-active');
            $('#adminSubMenu').toggleClass('slideBar-ic-active');

        });

        //Infrastructură rutieră Page Tabs
        const tab1 = document.getElementById('homeTab1');
        const tab2 = document.getElementById('homeTab2');
        const showTab1Button = document.getElementById('showHomeTab1Button');
        const showTab2Button = document.getElementById('showHomeTab2Button');

        if (showTab1Button != null) {
            showTab1Button.addEventListener('click', function () {
                tab1.style.display = 'block';
                tab2.style.display = 'none';
            });
        }

        if (showTab2Button != null) {
            showTab2Button.addEventListener('click', function () {
                tab1.style.display = 'none';
                tab2.style.display = 'block';
            });
        }


        $("#showHomeTab1Button").addClass("ic-active");

        $('#showHomeTab1Button').click(function () {
            $('#showHomeTab2Button').removeClass('ic-active');
            $("#showHomeTab1Button").addClass("ic-active");
        });

        $('#showHomeTab2Button').click(function () {
            $('#showHomeTab1Button').removeClass('ic-active');
            $("#showHomeTab2Button").addClass("ic-active");
        });
        //Infrastructură rutieră End Page Tabs


        //Detection Page Tabs
        const tabDet1 = document.getElementById('info');
        const tabDet2 = document.getElementById('approvalFlow');
        const tabDet3 = document.getElementById('chat');
        const showDetTab1Button = document.getElementById('showTabInfo');
        const showDetTab2Button = document.getElementById('showTabFlux');
        const showDetTab3Button = document.getElementById('showTabChat');

        if (showDetTab1Button != null) {
            showDetTab1Button.addEventListener('click', function () {
                tabDet1.style.display = 'block';
                tabDet2.style.display = 'none';
                tabDet3.style.display = 'none';
            });
        }

        if (showDetTab2Button != null) {
            showDetTab2Button.addEventListener('click', function () {
                tabDet1.style.display = 'none';
                tabDet2.style.display = 'block';
                tabDet3.style.display = 'none';
            });
        }

        if (showDetTab3Button != null) {
            showDetTab3Button.addEventListener('click', function () {
                tabDet1.style.display = 'none';
                tabDet2.style.display = 'none';
                tabDet3.style.display = 'block';
            });
        }


        $("#showTabFlux").addClass("ic-active");

        $('#showTabInfo').click(function () {
            $('#showTabFlux').removeClass('ic-active');
            $('#showTabChat').removeClass('ic-active');
            $("#showTabInfo").addClass("ic-active");
        });

        $('#showTabFlux').click(function () {
            $('#showTabInfo').removeClass('ic-active');
            $('#showTabChat').removeClass('ic-active');
            $("#showTabFlux").addClass("ic-active");
        });

        $('#showTabChat').click(function () {
            $('#showTabInfo').removeClass('ic-active');
            $('#showTabFlux').removeClass('ic-active');
            $("#showTabChat").addClass("ic-active");
        });


        //Detection End Page Tabs

        //Carousel functionality
        let currentIndex = 0;

        // Function to update the carousel position
        function updateCarousel() {
            const $carousel = $(".carousel-images");
            const $images = $carousel.find("img");
            const imageWidth = $images.first().outerWidth(true);
            const offset = -currentIndex * imageWidth;
            $carousel.css("transform", `translateX(${offset}px)`);
        }

        $(".carousel-btn.left").click(function () {
            const $images = $(".carousel-images img");
            currentIndex--;
            if (currentIndex < 0) {
                currentIndex = $images.length - 1; // Wrap around to the last image
            }
            updateCarousel();
        });

        $(".carousel-btn.right").click(function () {
            const $images = $(".carousel-images img");
            currentIndex++;
            if (currentIndex >= $images.length) {
                currentIndex = 0; // Wrap around to the first image
            }
            updateCarousel();
        });
        //End Carousel functionality


        //General design functions
        $(document).on("click", "#tab2-picture.images-container", function () {
            const $img = $('#tab2-picture.images-container img');
            const src = $img.attr("src");
            const detectionId = $('#Id').val();
            console.log(detectionId);

            if (src && src.startsWith("data:image/png;base64,")) {
                const base64Image = src.replace("data:image/png;base64,", "");
                const $container = $('#zoomableImageViewerContainer');
                showZoomableImageViewer(detectionId, base64Image, null, 1);
            }
        });

        $(document).on("click", "#showBigPreview", function () {
            const $img = $('#detectionImagePreview');
            const src = $img.attr("src");
            const detectionId = $('#detectionIdPreview').val();
            console.log(detectionId)

            if (src && src.startsWith("data:image/png;base64,")) {
                const base64Image = src.replace("data:image/png;base64,", "");
                const $container = $('#zoomableImageViewerContainer');
                showZoomableImageViewer(detectionId, base64Image, null, 1);
            }
        });

        //End general design functions



        //Ajax Call

        if (document.getElementById("btnCreateUser")) {
            document.getElementById("btnCreateUser").addEventListener("click", function (event) {
                event.preventDefault();

                var url = appUrl + "UserManagement/CreateUser";
                $.ajax({
                    url: url,
                    type: 'GET',
                    success: function (data) {
                        $('#modalCreateUserContent').html(data);
                        $('#createUserModal').modal('show');
                    },
                    error: function () {
                        alert('Error loading content');
                    }
                });
            });
        }

        //End Ajax Call

        // Reset imagini la schimbare tab imagine (opțional)
        $("#tab2-tab,#tab1-tab").click(function () {
            if (window.jsDesign?.centerZoomViewerImages) {
                window.jsDesign.centerZoomViewerImages();
            }
        });
    });

    // ====================== Viewer: INSERARE + INIT DE GRUP ======================
    function showZoomableImageViewer(dataId, base64Image, imageUrl = null, targetContainer = null) {
        const $container = $('#zoomableImageViewerContainer');
        let src = null;
        if (imageUrl) {
            src = imageUrl;
        } else if (base64Image) {
            src = /^data:image\/[a-zA-Z]+;base64,/.test(base64Image)
                ? base64Image
                : `data:image/png;base64,${base64Image}`;
        }
        if (!src) return;

        const wasVisible = $container.is(':visible');
        if (!wasVisible) { $container.css('display', 'flex').hide().fadeIn(); }
        if (!wasVisible && targetContainer == null) { resetZoomViewerState(); }

        $container.find('.img-id').text(dataId);
        $container.find('.btn-import-raw-data, .btn-delete-raw-data').attr('data-id', dataId);

        const $img1C = $container.find('.image-container');  
        const $img2C = $container.find('.image-container2');  
        let loaded = 0;
        const expected = (targetContainer === 1 || targetContainer === 2) ? 1 : 2;

        function finalizeInit() {
            const $all = $('#zoomableImageViewerContainer').find('img.zoomable-img');
            initializeZoomableImages($all);
            if (!window.jsDesign) window.jsDesign = {};
            window.jsDesign._zoomOpacityTimeout = setTimeout(() => {
                $all.css('opacity', 1);
                window.jsDesign._zoomOpacityTimeout = null;
            }, 120);
        }

        function onOneLoaded() {
            loaded++;
            if (loaded === expected) finalizeInit();
        }

        function buildZoomableImg(srcStr) {
            const img = new Image();
            const $img = $(img).addClass('zoomable-img').css('opacity', 0);
            $img.on('load', onOneLoaded);
            $img.on('error', () => console.error('Eroare la încărcarea imaginii.'));

            img.src = srcStr;
            if (img.complete) {
                Promise.resolve().then(() => $img.trigger('load'));
            }
            return $img;
        }

        // === Inserare în containere, în funcție de targetContainer ===
        if (targetContainer === 1) {
            const $i1 = buildZoomableImg(src);
            $img1C.html($i1);
        } else if (targetContainer === 2) {
            const $i2 = buildZoomableImg(src);
            $img2C.html($i2);
        } else {
            // ambele containere
            const $i1 = buildZoomableImg(src);
            const $i2 = buildZoomableImg(src);
            $img1C.html($i1);
            $img2C.html($i2);
        }

        // Close => curățare completă (imagini, filtre, handlere, tabs)
        $container.find('.close-btn')
            .off('click.zoomViewer')
            .on('click.zoomViewer', () => destroyZoomableImageViewer());
    }

    function resetZoomViewerState() {
        const $container = $('#zoomableImageViewerContainer');

        $container.find('.zoomable-img').remove();
        $container.find('.img-id').text('');
        $container.find('.btn-import-raw-data, .btn-delete-raw-data').attr('data-id', '');
        $container.find('.zoom-controls, .brightness-controls').hide();

        $container.find('.btn-zoom-in, .btn-zoom-out, .btn-brightness-up, .btn-brightness-down, .close-btn').off('click');
    }

    // ====================== INIT GRUP: zoom/drag/filtre sincron ======================
    function initializeZoomableImages($imgs) {
        // ---- STARE COMUNĂ ----
        let scale = 1;
        let translateX = 0;
        let translateY = 0;
        let startX = 0;
        let startY = 0;
        let isDragging = false;

        let filterValues = {
            brightness: 1,
            contrast: 1,
            saturate: 1,
            grayscale: 0,
            sepia: 0,
            invert: 0,
            blur: 0
        };

        // Stiluri de bază pe toate imaginile
        $imgs
            .attr("draggable", "false")
            .css({
                "border-radius": "22px",
                "border": "1px solid #dee2e6",
                "min-width": "-webkit-fill-available",
                "max-width": "100%",
                "max-height": "100%",
                "transition": "transform 0.1s ease",
                "cursor": "grab",
                "display": "block",
                "margin": "0 auto",
                "user-select": "none",
            });

        $(".generic-filter-controls,#imageTab").css("display", "flex");

        function clampTranslate(imgW, imgH, contW, contH) {
            const maxX = Math.max(0, (imgW * scale - contW) / 2);
            const maxY = Math.max(0, (imgH * scale - contH) / 2);
            translateX = Math.min(maxX, Math.max(-maxX, translateX));
            translateY = Math.min(maxY, Math.max(-maxY, translateY));
        }

        function applyAll() {
            const filterString = `
                brightness(${filterValues.brightness})
                contrast(${filterValues.contrast})
                saturate(${filterValues.saturate})
                grayscale(${filterValues.grayscale})
                sepia(${filterValues.sepia})
                invert(${filterValues.invert})
                blur(${filterValues.blur}px)
            `.trim();

            $imgs.css("filter", filterString);
            $imgs.css("transform", `translate(${translateX}px, ${translateY}px) scale(${scale})`);
        }

        function centerImages() {
            scale = 1;
            translateX = 0;
            translateY = 0;
            filterValues = {
                brightness: 1,
                contrast: 1,
                saturate: 1,
                grayscale: 0,
                sepia: 0,
                invert: 0,
                blur: 0
            };
            applyAll();
        }

        // Zoom cu rotița – calculează relativ la imaginea atinsă, aplică pe toate
        $imgs.off("wheel.zoomGroup").on("wheel.zoomGroup", function (event) {
            event.preventDefault();

            const $t = $(event.currentTarget);
            const rect = $t[0].getBoundingClientRect();

            const prevScale = scale;
            const delta = event.originalEvent.deltaY;
            const zoomSpeed = 0.1;

            scale += delta < 0 ? zoomSpeed : -zoomSpeed;
            scale = Math.max(1, Math.min(scale, 5));

            if (scale === 1) {
                centerImages();
            } else {
                const offsetX = event.clientX - rect.left;
                const offsetY = event.clientY - rect.top;
                const dx = offsetX - rect.width / 2;
                const dy = offsetY - rect.height / 2;

                translateX -= dx * (scale - prevScale) / scale;
                translateY -= dy * (scale - prevScale) / scale;

                // limitează pe baza containerului imaginii curente
                const $cont = $t.parent();
                const contW = $cont.width();
                const contH = $cont.height();
                const imgW = $t[0].naturalWidth;
                const imgH = $t[0].naturalHeight;
                clampTranslate(imgW, imgH, contW, contH);
            }

            applyAll();
        });

        // Drag comun (pornit de pe oricare imagine)
        $imgs.off("mousedown.zoomGroup").on("mousedown.zoomGroup", function (event) {
            event.preventDefault();

            const $t = $(event.currentTarget);
            const contRect = $t.parent()[0].getBoundingClientRect();
            const imgRect = $t[0].getBoundingClientRect();

            const canDragX = imgRect.width > contRect.width;
            const canDragY = imgRect.height > contRect.height;

            if (scale > 1 && (canDragX || canDragY)) {
                isDragging = true;
                startX = event.clientX - translateX;
                startY = event.clientY - translateY;
                $imgs.css("cursor", "grabbing");

                $(document).on("mousemove.zoomDragGroup", function (e) {
                    if (isDragging) {
                        translateX = e.clientX - startX;
                        translateY = e.clientY - startY;
                        applyAll();
                    }
                });
            }
        });

        $(document).off("mouseup.zoomGroup").on("mouseup.zoomGroup", function () {
            if (isDragging) {
                isDragging = false;
                $imgs.css("cursor", "grab");
                $(document).off("mousemove.zoomDragGroup");
            }
        });

        // Dublu click = reset pentru toate
        $imgs.off("dblclick.zoomGroup").on("dblclick.zoomGroup", function () {
            centerImages();
        });

        // Butoane filtre (folosim containerul primului)
        const $container = $imgs.first().closest('.absolute-div-container');
        $container.find('.btn-filter-increase, .btn-filter-decrease').off('click.zoomGroup');

        function updateFilter(name, delta, min = 0, max = 3) {
            filterValues[name] = Math.max(min, Math.min(max, filterValues[name] + delta));
            applyAll();
        }

        $container.find('.btn-filter-increase').on('click.zoomGroup', function () {
            const selected = $container.find('#filterSelector').val();
            if (selected === "scale") {
                scale = Math.min(scale + 0.1, 5);
            } else {
                const isBlur = selected === "blur";
                updateFilter(selected, isBlur ? 0.5 : 0.1, 0, isBlur ? 10 : 3);
            }
            applyAll();
        });

        $container.find('.btn-filter-decrease').on('click.zoomGroup', function () {
            const selected = $container.find('#filterSelector').val();
            if (selected === "scale") {
                scale = Math.max(scale - 0.1, 1);
                if (scale === 1) { translateX = 0; translateY = 0; }
            } else {
                const isBlur = selected === "blur";
                updateFilter(selected, isBlur ? -0.5 : -0.1, 0, isBlur ? 10 : 3);
            }
            applyAll();
        });

        // Expunere reset global
        window.jsDesign = Object.assign(window.jsDesign || {}, {
            centerZoomViewerImages: centerImages
        });

        centerImages();
    }

    function destroyZoomableImageViewer() {
        const $container = $('#zoomableImageViewerContainer');

        // 1) Oprește orice timeout de opacitate, dacă a fost folosit
        if (window.jsDesign && window.jsDesign._zoomOpacityTimeout) {
            clearTimeout(window.jsDesign._zoomOpacityTimeout);
            window.jsDesign._zoomOpacityTimeout = null;
        }

        // 2) Dezleagă toate evenimentele legate pe imagini (namespaced sau brute)
        const $imgs = $container.find('img.zoomable-img');
        $imgs.off('wheel.zoomGroup mousedown.zoomGroup dblclick.zoomGroup');
        $imgs.off('wheel mousedown dblclick'); // fallback, în caz că au fost legate fără namespace

        // 3) Dezleagă evenimente pe document legate de drag pentru grup
        $(document).off('mousemove.zoomDragGroup mouseup.zoomGroup');

        // 4) Dezleagă evenimente pe butoanele de filtre
        const $controlsScope = $imgs.first().length
            ? $imgs.first().closest('.absolute-div-container')
            : $container;
        $controlsScope.find('.btn-filter-increase, .btn-filter-decrease')
            .off('click.zoomGroup click');

        // 5) Dezleagă close-ul (îl vom relega la show)
        $container.find('.close-btn').off('click.zoomViewer click');

        // 6) Resetează UI-ul (controale & text)
        $container.find('.img-id').text('');
        $container.find('.btn-import-raw-data, .btn-delete-raw-data').attr('data-id', '');
        $container.find('.zoom-controls, .brightness-controls').hide();

        // 7) Elimină stilurile aplicate pe imagini (filtre, transform, opacity)
        $imgs.css({ filter: '', transform: '', opacity: '' });

        // 8) Elimină imaginile & golește containerele
        $container.find('.image-container').empty();
        $container.find('.image-container2').empty();
        resetImageTabs();
        // 9) Ascunde viewer-ul
        $container.stop(true, true).fadeOut(120, function () {
            // ca să fie *curat-curat*, poți elimina orice stil inline rămas
            $container.find('.zoomable-img').remove();
        });
    }
    function resetImageTabs() {
        $('#tab1-tab').addClass('active').attr('aria-selected', 'true');
        $('#tab2-tab').removeClass('active').attr('aria-selected', 'false');
        $('#tab1').addClass('active show');
        $('#tab2').removeClass('active show');
    }

    $(document).off('keyup.zoomViewerEsc').on('keyup.zoomViewerEsc', function (e) {
        if (e.key === 'Escape' || e.keyCode === 27) {
            const $container = $('#zoomableImageViewerContainer');
            if ($container.is(':visible')) destroyZoomableImageViewer();
        }
    });

    // ====================== Export public API ======================
    window.jsDesign = Object.assign(window.jsDesign || {}, {
        showZoomableImageViewer,
        initializeZoomableImages
    });

})();