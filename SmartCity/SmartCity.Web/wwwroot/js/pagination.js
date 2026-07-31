"use strict";

(function () {

    var appUrl = $("#appUrl").val();
    function registerPaginationEvents() {
        registerFilterPaginationEvents();
    }

    function registerFilterPaginationEvents($container) {
        $container = $container ? $container : $(document);
        var $paginations = $(".pagination-results.filter-pagination:not([data-pagination-event-registered='true'])", $container);

        $paginations.each(function () {
            var $paginationResultsContainer = $(this);
            var $filter = $($paginationResultsContainer.data("filter-id"));
            var dataSourceAction = $filter.data("action");
            var dataSourceController = $filter.data("controller");
            var sourceFilterId = $filter.attr("id");
            var dataSourceUrl = appUrl + dataSourceController + "/" + dataSourceAction;
            createPaginationEvents(dataSourceUrl, $paginationResultsContainer, $filter, sourceFilterId);
            $paginationResultsContainer.attr("data-pagination-event-registered", true);
        });
    }

    function createPaginationEvents(dataSourceUrl, $resulsContainer, $filter, sourceFilterId) {
        if ($resulsContainer) {
            $resulsContainer.on("click", "a.btn-pagination:not(.active):not(.disabled)", onPaginationBtnClick);
            $resulsContainer.on("click", "a.order", onPaginationOrderClick);
        }

        var ignoreFormSubmit = $resulsContainer.data("ignore-form-submit") || false;
        var $btnApplyFilter = ($filter && !ignoreFormSubmit) ? $("button[type='button']", $filter) : $("button.btn-pagination", $filter);

        if ($btnApplyFilter && ignoreFormSubmit === false)
            $btnApplyFilter.on("click", onFilterSubmit);

        var lastAppliedFilter = {};

        var filterOnLoad = !$btnApplyFilter || !$btnApplyFilter.length || $resulsContainer.data("filterOnLoad") === true;

        if (filterOnLoad) {
            lastAppliedFilter = $filter ? jsUtils.serializeObject($filter) : {};
            getResults(1, null);
        }

        if (ignoreFormSubmit && $btnApplyFilter)
            $btnApplyFilter.on("click", onBtnPaginareClick);

        function onFilterSubmit(e) {
            e.preventDefault();
            lastAppliedFilter = jsUtils.serializeObject($filter);
            getResults(1, null);
        }

        function onBtnPaginareClick() {
            lastAppliedFilter = jsUtils.serializeObject($filter);
            getResults(1, null);
        }

        function onPaginationBtnClick(e) {
            e.preventDefault();
            var $btn = $(this);
            var page = $btn.data("refPage");
            var persistentCount = $resulsContainer.find("#PersistentCount").val();
            var orderTxt = $resulsContainer.find("#Order").val();
            getResults(page, persistentCount, orderTxt);
        }

        function onPaginationOrderClick(e) {
            e.preventDefault();
            var $header = $(this);
            var orderTxt = $header.data("order");
            var persistentCount = $resulsContainer.find("#PersistentCount").val();
            $resulsContainer.find("#Order").val(orderTxt);
            getResults(1, persistentCount, orderTxt);
        }

        function getResults(pageIndex, persistentCount, orderTxt) {
            var serializeDiv = $('#' + sourceFilterId + " :input");
            var objs = jsUtils.serializeObject(serializeDiv);
            var params = $.extend(objs, { pageIndex: pageIndex, persistentCount: persistentCount, order: orderTxt }, lastAppliedFilter);
            var $trigger = $('#app-loader-container');
            jsUtils.postJsonData($trigger, dataSourceUrl, params, true, function (response) {
                $resulsContainer.html(response);
                if (dataSourceUrl.includes("GetRawDetectionsToProcess")) {
                    loadRawDetectionImages();
                }               
            });
        }

        async function loadRawDetectionImages() {
            const previewImages = document.querySelectorAll('.preview-img');

            for (const img of previewImages) {
                const rawDetectionId = img.dataset.id; // get data-id
                if (!rawDetectionId) continue;

                try {
                    const url = `${appUrl}Admin/GetRawDetectionProcessedImg?rawDetectionId=${rawDetectionId}&isPreview=true`;

                    const response = await fetch(url);
                    if (!response.ok) throw new Error('Network response was not ok');

                    const data = await response.json();
                    if (data.src) {
                        img.src = data.src; // update image
                    }
                } catch (err) {
                    console.error(`Failed to load image for id ${rawDetectionId}`, err);
                }
            }
        }

        $filter.on('click', '#applyPageSizeBtn', function () {
            const val = parseInt($('#pageSizeInput').val(), 10);
            if (!isNaN(val) && val > 0 && val <= 1000) {
                const persistentCount = $resulsContainer.find("#PersistentCount").val();
                const orderTxt = $resulsContainer.find("#Order").val();
                getResults(1, persistentCount, orderTxt);
            } else {
                alert('Please enter a valid number between 1 and 1000.');
            }
        });

        $filter.on('keypress', '#pageSizeInput', function (e) {
            if (e.which === 13) {
                $('#applyPageSizeBtn').click();
            }
        });
    }





    var paginationUtils = {
        registerPaginationEvents: registerPaginationEvents
    }

    window.paginationUtils = paginationUtils;

})();