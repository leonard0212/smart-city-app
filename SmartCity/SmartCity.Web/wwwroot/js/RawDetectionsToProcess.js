"use strict";
$(function () {
    var appUrl = $("#appUrl").val();

    $(document).ready(function () {
        const CONTAINER_SELECTOR = '.sc-admin-container';
        // setTimeout(loadRawDetectionInfo, 1000);



        //Varianta Bogdan Hatis
        //$(document).on('click', '.rawDetectionImgContainer', function () {
        //    const dataId = $(this).data('id');
        //    const url = appUrl + "Admin/GetRawDetectionProcessedImg";

        //    const dataPreview = { rawDetectionId: dataId, isPreview: true, isCrop: false };
        //    const dataFull = { rawDetectionId: dataId, isPreview: false, isCrop: false };
        //    const dataCrop = { rawDetectionId: dataId, isPreview: false, isCrop: true };

        //    // Step 1: Load preview image
        //    $.ajax({
        //        url: url,
        //        method: 'POST',
        //        data: dataPreview,
        //        success: function (previewResponse) {
        //            if (previewResponse && previewResponse.src) {
        //                // Show preview image immediately
        //                jsDesign.showZoomableImageViewer(dataId, null, previewResponse.src);

        //                // Step 2: Then load full image
        //                $.ajax({
        //                    url: url,
        //                    method: 'POST',
        //                    data: dataFull,
        //                    success: function (fullResponse) {
        //                        if (fullResponse && fullResponse.src) {
        //                            // Replace image in viewer
        //                            const $container = $('#zoomableImageViewerContainer');
        //                            const $img = $container.find('img.zoomable-img');

        //                            const highResImg = new Image();
        //                            highResImg.onload = () => {
        //                                $img.attr('src', fullResponse.src);
        //                            };
        //                            highResImg.src = fullResponse.src;
        //                        } else {
        //                            console.warn("⚠️ Full image response missing `src`.");
        //                        }
        //                    },
        //                    error: function (xhr) {
        //                        console.error('❌ Full image fetch failed:', xhr.status, xhr.responseText);
        //                    }
        //                });
        //            } else {
        //                console.error('❌ Preview image fetch failed or missing `src`:', previewResponse);
        //            }
        //        },
        //        error: function (xhr) {
        //            console.error('❌ Preview image fetch failed:', xhr.status, xhr.responseText);
        //        }
        //    });
        //});

        //Varianta Iordache Catalin
        //let currentImageLoadSessionId = 0;

        //$(document).on('click', '.rawDetectionImgContainer', function () {
        //    currentImageLoadSessionId++;
        //    const sessionId = currentImageLoadSessionId;

        //    const dataId = $(this).data('id');
        //    const $sourceImg = $(this).find('img');
        //    const initialSrc = $sourceImg.attr('src'); // can be base64 or blob

        //    // Extract base64 if available, otherwise convert
        //    if (initialSrc.startsWith("data:image")) {
        //        const base64 = initialSrc.split(",")[1];
        //        showAndProgressiveLoad(dataId, base64);
        //    } else {
        //        // fallback for blob or file URL – convert to base64 first
        //        convertImgToBase64($sourceImg[0], function (base64) {
        //            showAndProgressiveLoad(dataId, base64);
        //        });
        //    }

        //    function showAndProgressiveLoad(dataId, base64Image) {
        //        jsDesign.showZoomableImageViewer(dataId, base64Image);

        //        const baseUrl = appUrl + "Admin/GetRawDetectionStreamImg?rawDetectionId=" + dataId;
        //        const resolutions = [860, 1440, 1920, 2560];
        //        const maxResUrl = baseUrl;
        //        let currentStep = 0;

        //        const $container = $('#zoomableImageViewerContainer');
        //        const $img1 = $container.find('.image-container img.zoomable-img');
        //        const $img2 = $container.find('.image-container2 img.zoomable-img');

        //        const updateBothImages = (src) => {
        //            $img1.attr('src', src);
        //            $img2.attr('src', src);
        //        };

        //        const loadNextResolution = () => {
        //            if (sessionId !== currentImageLoadSessionId) return;

        //            if (currentStep >= resolutions.length) {
        //                const finalImg = new Image();
        //                finalImg.onload = () => {
        //                    if (sessionId !== currentImageLoadSessionId) return;
        //                    console.log(`✅ Full-res image loaded.`);
        //                    updateBothImages(maxResUrl);
        //                };
        //                finalImg.src = maxResUrl;
        //                return;
        //            }

        //            const width = resolutions[currentStep];
        //            const nextUrl = `${baseUrl}&width=${width}`;
        //            const nextImg = new Image();

        //            console.log(`🔄 Loading resolution ${width}px...`);
        //            nextImg.onload = () => {
        //                if (sessionId !== currentImageLoadSessionId) return;
        //                console.log(`✅ Resolution ${width}px loaded.`);
        //                updateBothImages(nextUrl);
        //                requestAnimationFrame(() => {
        //                    setTimeout(() => {
        //                        currentStep++;
        //                        loadNextResolution();
        //                    }, 100);
        //                });
        //            };

        //            nextImg.onerror = () => {
        //                if (sessionId !== currentImageLoadSessionId) return;
        //                console.warn(`⚠️ Failed loading ${width}px. Skipping...`);
        //                currentStep++;
        //                loadNextResolution();
        //            };

        //            nextImg.src = nextUrl;
        //        };

        //        requestAnimationFrame(() => {
        //            loadNextResolution();
        //        });
        //    }

        //    function convertImgToBase64(imgElement, callback) {
        //        const canvas = document.createElement('canvas');
        //        canvas.width = imgElement.naturalWidth;
        //        canvas.height = imgElement.naturalHeight;
        //        const ctx = canvas.getContext('2d');
        //        ctx.drawImage(imgElement, 0, 0);
        //        const dataUrl = canvas.toDataURL('image/png');
        //        callback(dataUrl.split(",")[1]);
        //    }
        //});

        //$(document).on('change', '#CategoryId', function () {
        //    var mainclassValue = $(this).val();

        //    var data = { mainclass: mainclassValue };
        //    var url = appUrl + "Admin/GetRawDetectionSubclass";
        //    jsUtils.postJsonData(null, url, data, true, function (response) {
        //        const $select = $('#SubCategoryId');
        //        $select.empty(); // Clean existing options
        //        response.forEach(item => {
        //            const $option = $('<option>')
        //                .text(item.text)
        //                .val(item.value)
        //                .prop('disabled', item.disabled)
        //                .prop('selected', item.selected);
        //            $select.append($option);
        //        });
        //    });
        //});

        // Delete section

        $(document).on('click', '.btn-delete-raw-data', function () {
            if (confirm('Are you sure you whant to delete detection?')) {
                var $trigger = $('#app-loader-container');
                var element = $(this);
                var id = element.attr("data-id");
                var url = appUrl + 'Admin/DeleteRawDetection?pageIndex=' + encodeURIComponent(currentPage());
                var data = { rawDetectionId: id };
                jsUtils.postJsonData($trigger, url, data, true, function (response) {
                    alert(response.message)
                    if (response.succes) {
                        $("#btnFilter").click();
                    }
                });
            }
        });

        $(document).on('change', '#selectAllRows', function () {
            const $c = getResultsContainerFrom(this);
            const checked = this.checked;
            $c.find('.row-select').prop('checked', checked);
            updateBulkBar($c); 
        });

        $(document).on('change', '.row-select', function () {
            const $c = getResultsContainerFrom(this);
            const all = $c.find('.row-select').length;
            const checked = $c.find('.row-select:checked').length;
            $c.find('#selectAllRows').prop('checked', all > 0 && checked === all);
            updateBulkBar($c); 
        });

        $(document).on('click', '#btnDeleteSelected', function () {
            const $c = getResultsContainerFrom(this);
            const ids = $c.find('.row-select:checked').map(function () { return $(this).data('id'); }).get();
            if (!ids.length) { alert('Selectează cel puțin un item.'); return; }
            if (!confirm(`Ești sigur că vrei să ștergi ${ids.length} detectări?`)) { return; }

            const url = appUrl + 'Admin/DeleteRawDetections?pageIndex=' + encodeURIComponent(currentPage());
            const payload = { RawDetectionIds: ids };

            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(payload),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    alert(response.message);
                    if (response.succes) { $("#btnFilter").click(); }
                },
                error: function (xhr) {
                    console.error('DeleteRawDetections error:', xhr.status, xhr.responseText);
                    alert('Delete failed: ' + xhr.status);
                }
            });
        });

        function getResultsContainerFrom(el) {
            // caută cel mai apropiat container al listei
            let $c = $(el).closest(CONTAINER_SELECTOR);
            // fallback (dacă e nevoie): primul container din pagină
            if (!$c.length) $c = $(CONTAINER_SELECTOR).first();
            return $c;
        }

        function updateBulkBar($c) {
           
            if (!$c || !$c.length) return;
            const count = $c.find('.row-select:checked').length;
            $c.find('#bulkActionsBar').toggle(count > 0);
            $c.find('#selectedCount').text(count > 0 ? (count + ' selected') : '');
        }

        // End Delete section

        $(document).on('click', '.btn-import-raw-data', function () {
            if (confirm('Are you sure you whant to import detection?')) {
                var $trigger = $('#app-loader-container');
                var element = $(this);
                var id = element.attr("data-id");
                var url = appUrl + 'Admin/ImportRawDetection?pageIndex=' + encodeURIComponent(currentPage());
                var data = { rawDetectionId: id };
                jsUtils.postJsonData($trigger, url, data, true, function (response) {
                    alert(response.message)
                    if (response.succes) {
                        $('.close-btn.sc-general-btn.admin-table.title-xsmall').click();
                        $("#btnFilter").click();

                    }
                });
            }
        });

        $(document).on('click', '.rawDetectionImgContainer', function () {
            const dataId = $(this).data('id');
            const $tab2 = $('#tab2-tab');
            $tab2.removeAttr('data-id').removeData('id');
            $tab2.attr('data-id', dataId).data('id', dataId);
            console.log(dataId)
            //$('#tab1').data('loaded', true);   
            //$('#tab2').data('loaded', false); 

            const dataPreview = { rawDetectionId: dataId, isPreview: true, isCrop: false };
            const dataFull = { rawDetectionId: dataId, isPreview: false, isCrop: false };

            loadDetectionImages(dataId, dataPreview, dataFull, null);
        });

        $(document).on('click', '#tab2-tab', function () {
            let dataId = $(this).data('id');

            if (!dataId) {
                dataId = $('#rawDataId').val();
            }

            const dataCrop = { rawDetectionId: dataId, isPreview: false, isCrop: true };
            loadDetectionImages(dataId, null, null, dataCrop);
        });

    });

    function loadDetectionImages(dataId, dataPreview, dataFull, dataCrop) {
        const url = appUrl + "Admin/GetRawDetectionProcessedImg";
        if (dataCrop) {
            $.ajax({
                url: url,
                method: 'POST',
                data: dataCrop,
                success: function (cropResponse) {
                    if (cropResponse && cropResponse.src) {
                        jsDesign.showZoomableImageViewer(dataId, null, cropResponse.src, 2);
                    } else {
                        console.warn("⚠️ Crop image response missing `src`.");
                    }
                },
                error: function (xhr) {
                    console.error('❌ Crop image fetch failed:', xhr.status, xhr.responseText);
                }
            });
            return;
        }
        if (!dataPreview || !dataFull) {
            console.warn("⚠️ Missing dataPreview or dataFull for normal flow.");
            return;
        }
        $.ajax({
            url: url,
            method: 'POST',
            data: dataPreview,
            success: function (previewResponse) {
                if (previewResponse && previewResponse.src) {
                    jsDesign.showZoomableImageViewer(dataId, null, previewResponse.src, 1);
                    $.ajax({
                        url: url,
                        method: 'POST',
                        data: dataFull,
                        success: function (fullResponse) {
                            if (fullResponse && fullResponse.src) {
                                const $img = $('#tab1').find('img.zoomable-img');
                                const highResImg = new Image();
                                highResImg.onload = () => $img.attr('src', fullResponse.src);
                                highResImg.src = fullResponse.src;
                            } else {
                                console.warn("⚠️ Full image response missing `src`.");
                            }
                        },
                        error: function (xhr) {
                            console.error('❌ Full image fetch failed:', xhr.status, xhr.responseText);
                        }
                    });
                } else {
                    console.error('❌ Preview image fetch failed or missing `src`:', previewResponse);
                }
            },
            error: function (xhr) {
                console.error('❌ Preview image fetch failed:', xhr.status, xhr.responseText);
            }
        });
    }


    function currentPage() {
        return parseInt($('#pagination .sc-pagination-btn').attr('data-ref-page') || '1', 10);
    }


    function generateGroupedHtml(response, chunkSize) {
        let groupedHtml = '';
        let currentChunk = [];

        Object.entries(response).forEach(([key, value], index) => {
            currentChunk.push(`<dt>${key}</dt><dd>${value}</dd>`);

            // When the chunk reaches the desired size or it's the last item, wrap it in a <div>
            if (currentChunk.length === chunkSize || index === Object.entries(response).length - 1) {
                groupedHtml += `<div class="group-data-elements">${currentChunk.join('')}</div>`;
                currentChunk = []; // Reset for the next chunk
            }
        });
        return groupedHtml;
    }



    function loadRawDetectionInfo() {

        $('.rawDetectionDataContainer').each(function (i, obj) {
            var element = $(this);
            var id = element.attr("data-id");

            $(this).html(id);
            var url = appUrl + "Admin/GetRawDetectionInfo";
            var data = { rawDetectionId: id };
            jsUtils.postJsonData(null, url, data, true, function (response) {
                $.each(response, function (key, value) {
                    element.append(`<dt>${key}</dt><dd>${value}</dd>`);
                });
            });


            var url2 = appUrl + "Admin/GetRawDetectionPreviewImg";
            jsUtils.postJsonData(null, url, data, true, function (response) {
                var imgContent = response.base64Content;
                var imgtag = '<img src="data:image/png;base64,' + imgContent + '" />';
                element.html(imgtag);
            });

            const url3 = appUrl + 'Admin/GetRawDetectionInfo';
            jsUtils.postJsonData(null, url, data, true, function (response) {
                const groupedHtml = generateGroupedHtml(response, 3);
                element.html(groupedHtml);
            });


        });

        //$('.rawDetectionImgContainer').each(function (i, obj) {
        //    var element = $(this);
        //    var id = element.attr("data-id");

        //    //  alert(id);
        //    $(this).html(id);
        //    var url = appUrl + "Admin/GetRawDetectionPreviewImg";
        //    var data = { rawDetectionId: id };
        //    jsUtils.postJsonData(null, url, data, true, function (response) {

        //        var imgContent = response.base64Content;
        //        var imgtag = '<img src="data:image/png;base64,' + imgContent + '" />';
        //        element.html(imgtag);
        //    });

        //});

        //$('.rawDetectionDataContainer').each(function () {
        //    const element = $(this);
        //    const id = element.attr('data-id');
        //    const url = appUrl + 'Admin/GetRawDetectionInfo';
        //    const data = { rawDetectionId: id };

        //    jsUtils.postJsonData(null, url, data, true, function (response) {
        //        const groupedHtml = generateGroupedHtml(response, 3);
        //        element.html(groupedHtml);
        //    });
        //});
    }
});