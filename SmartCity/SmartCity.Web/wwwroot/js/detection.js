"use strict";
(function () {
    var appUrl = $("#appUrl").val();
    console.log("detection.js")
    $(document).ready(function () {

        updateCarulsePictures();

        $(document).on('click', '.carousel-images-item', function () {
            var id = $(this).attr("data-id");
            gmapsUtils.focusMarker(id);
        });

        $(document).on('click', '.detection-table-row', function () {
            var id = $(this).attr("data-id");

            ViewSelectedDetection(id)

            gmapsUtils.focusMarker(id);
            GetDetectionFlowDataHome(id);
            setTimeout(function () {
                updatePicturePreview(id);
            }, 200)
        });

        $(document).on("click", ".grid-view-btn", function (e) {
            e.preventDefault();
            var id = $(this).data("id");

            ViewSelectedDetection(id)

            GetDetectionFlowDataHome(id);
            setTimeout(function () {
                updatePicturePreview(id);
            }, 200)
        });

        $(document).on("change", "[id$='AssignToDepartment_DepartamentId']", function () {
            var departmentId = $(this).val();
            DetectionModule.GetTeamsByDepartmentId(departmentId);
        });

        $(document).on("click", "#btn-reject", function () {
            var detectionId = $('#DetectionId').val();
            DetectionModule.RejectDetection(detectionId);
            setTimeout(function () {
                GetDetectionFlowDataHome(detectionId);
            }, 1200);
        });

        $(document).on("click", "#btn-markAsRezolved", function () {
            var detectionId = $('#DetectionId').val();
            DetectionModule.MarkAsRezolvedDetection(detectionId);
            setTimeout(function () {
                GetDetectionFlowDataHome(detectionId);
            }, 1200);

        });

        $(document).on("click", "#btn-assignToDepartament", function () {
            var detectionId = $('#DetectionId').val();
            DetectionModule.AssignToDepartmentDetection(detectionId);
            setTimeout(function () {
                GetDetectionFlowDataHome(detectionId);
            }, 900);
        });

        $(document).on("click", "#btn-add-chat-message2", function () {
            debugger;
            var detectionId = $('#DetectionId').val();
            var text = $("#chatResponse").val();
            DetectionModule.CreateChatEntry(detectionId, text);
        });
    });
    function updateDetailsTab(detectionId) {
        updatePictureTab2(detectionId);
        gmapsUtils.addMarkerOnTab2(detectionId);
        updateDetectionDetailsTab2(detectionId);
        updateDetectionSpecificDetails(detectionId);
        updateDetectionChat(detectionId);
        GetDetectionFlowDataHome(id);
    }

    function GetDetectionFlowDataHome(id) {
        var url = appUrl + "Detection/GetDetectionFlowDataHome";
        var $trigger = $('#app-loader-container');
        var data = { id: id }
        jsUtils.postJsonData(null, url, data, true, function (response) {
            $('#sc-flow-container').html(response);
        });
    }


    function updatePicturePreview(id) {
        var url = appUrl + "InternalApi/GetDetectionProcessedPicture";
        var $trigger = $('#app-loader-container');
        var data = { id: id }
        jsUtils.postJsonData(null, url, data, true, function (response) {
            const imageSrc = `data:image/png;base64,${response.filedata}`;
            const maxLoader = $('#app-max-loader-container.max-loader');
            const $rawDataId = $('#rawDataId');
            console.log(response.rawDataId)
            $rawDataId.val(response.rawDataId); 
            maxLoader.css('display', 'none');
            const intervalId = setInterval(() => {
                const $previewImg = $('#detectionImagePreview');
                const $mainImg = $('#detectionImg');
                const $detectionIdPreview = $('#detectionIdPreview');
                if ($previewImg.length && $mainImg.length) {                 
                    $previewImg.attr('src', imageSrc);
                    $mainImg.attr('src', imageSrc);
                    $detectionIdPreview.val(id);
                    clearInterval(intervalId);
                }
            }, 100);

            setTimeout(() => clearInterval(intervalId), 5000);
        });
    }

    function updatePictureTab2(detectionId) {
        var url = appUrl + "InternalApi/GetDetectionProcessedPicture";
        var $trigger = $('#app-loader-container');
        var data = { id: detectionId }
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
         
            $("#showHomeTab2Button").trigger("click");
            const imageSrc = `data:image/png;base64,${response.filedata}`;
            const intervalId = setInterval(() => {
                const $previewImg = $('#detectionImagePreview');
                const $mainImg = $('#detectionImg');
                if ($previewImg.length && $mainImg.length) {                   
                    $previewImg.attr('src', imageSrc);
                    $mainImg.attr('src', imageSrc);
                    clearInterval(intervalId);
                }
            }, 100);
        });
    }

    function updateDetectionDetailsTab2(detectionId) {
        var url = appUrl + "InternalApi/GetDetectionDataPartial";
        var $trigger = $('#app-loader-container');
        var data = { id: detectionId }
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $('#tab2-category').html(response);
        });
    }

    function updateDetectionSpecificDetails(detectionId) {
        getDectectionChatData(detectionId)
    }

    function updateDetectionChat(detectionId) {
        var category = $("#detectionCategory").val();
        if (category == "RoadInfrastructure")
            roadInfrastructureUtils.getDectectionChatData(detectionId);
        else if (category == "Garbage")
            trashUtils.getTrashData(detectionId);
        else if (category == "Billboards")
            billboardUtils.getbillboardData(detectionId);
    }

    function getDectectionChatData(detectionId) {
        var $trigger = $(this);
        var data = { detectionId: detectionId };

        var url = appUrl + "InternalApi/GetDetectionChat";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#tabChat-detail").html(response);
            $("#chatDetectionId").val(detectionId);
        });
    }

    $(document).on("click", "#addChatId", function () {
        addDectionChat();
    });


    function addDectionChat() {
        debugger;
        var $trigger = $(this);
        let detectionId = $("#detectionId").val();
        let detectionText = $("#chatResponse").val();
        var data = { detectionId: detectionId, detectionText: detectionText };

        var url = appUrl + "InternalApi/AddDetectionChat";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#tabChat-detail").html(response);
        });
    }


    function updateCarulsePictures() {
        var category = $("#detectionCategory").val();
        if (category != null) {
            var data = { detectionCategory: category };
            var $trigger = $('#app-loader-container');
            var url = appUrl + "InternalApi/GetCaruselDetectionData";
            jsUtils.postJsonData(null, url, data, true, function (response) {
                $('#carusel-detections-container').html(response);
            });
        }
    }

    function ViewSelectedDetection(id) {
        $(".detection-table-row").removeClass("active-row");
        $(".detection-table-row[data-id='" + id + "']").addClass("active-row");
    }

    var detectionUtils = {
        updateDetailsTab: updateDetailsTab,
    };
    window.detectionUtils = detectionUtils;

})();