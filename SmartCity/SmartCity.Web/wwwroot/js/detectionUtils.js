"use strict";

window.DetectionModule = (function () {
    var appUrl = $("#appUrl").val();

    function GetDetectionChat(detectionId) {
        var $trigger = $(this);
        var data = { detectionId: detectionId };
        var url = appUrl + "InternalApi/GetDetectionChat";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#chat-container").html(response);
        });
    }

    function GetDetectionFlowData(detectionId) {
        var $trigger = $(this);
        var data = { Id: detectionId };
        var url = appUrl + "Detection/GetDetectionFlowData";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#flow-container").html(response);
        });
    }

    function RejectDetection(detectionId) {
        var $trigger = $(this);
        var text = $("#rejectText").val();
        var data = { detectionId: detectionId, text: text };
        var url = appUrl + "Detection/RejectDetection";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#flow-container").html(response);
            $('#tab2-category').html(response);
        });
    }

    function MarkAsRezolvedDetection(detectionId) {
        var $trigger = $(this);
        var text = $("#markAsRezolvedText").val();
        var data = { detectionId: detectionId, text: text };
        var url = appUrl + "Detection/MarkAsRezolvedDetection";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#flow-container").html(response);
            $('#tab2-category').html(response);
        });
    }

    function AssignToDepartmentDetection(detectionId) {
        var $trigger = $(this);
        var text = $("#assignToDepartamentText").val();
        var departamentId = $("#AssignToDepartment_DepartamentId").val();
        var teamId = $("#AssignToDepartment_TeamId").val();
        const hiddenVal = $("#assignDateHidden").val(); 

        if (!hiddenVal) {
            alert("Te rog selectează o dată înainte de atribuire!");
            return;
        }

        let startDate = hiddenVal;
        let endDate = hiddenVal;

        if (hiddenVal.includes(" to ")) {
            [startDate, endDate] = hiddenVal.split(" to ");
        }


        var data = {
            detectionId: detectionId, text: text, departamentId: departamentId, teamId: teamId, startDate: startDate,endDate: endDate};
        var url = appUrl + "Detection/AssignToDepartamentDetection";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#flow-container").html(response);
            $('#tab2-category').html(response);
        });
    }

    function GetTeamsByDepartmentId(departamentId) {
        var $trigger = $(this);
        var data = { deparmentId: departamentId };
        var url = appUrl + "InternalApi/GetTeamsByDepartmentId";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            var $dropdown = $('#AssignToDepartment_TeamId');
            var $container = $('#teamIdContainer, #teamIdContainer2');
            var $noTeams = $('#noTeamsId');
            $dropdown.empty();
            $.each(response, function (index, item) {
                $dropdown.append(
                    $('<option></option>').val(item.value).text(item.text)
                );
            });
            if (response.length > 0) {
                $container.show();
                $noTeams.hide();
                $('#btn-assignToDepartament').prop('disabled', false);
            } else {
                $noTeams.show();
                $('#btn-assignToDepartament').prop('disabled', true);
                $container.hide();
            }
        });
    }

    function CreateChatEntry(detectionId, text) {
        var $trigger = $(this);
        var data = { detectionId: detectionId, detectionText: text };
        var url = appUrl + "Detection/AddDetectionChat";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $('#sc-flow-container').html(response);
            $('#tab2-category').html(response);
            
        });
    }

    function updateChatInitials() {
        $(".chat-message:not(.processed)").each(function () {
            const userName = $(this).find(".chat-message-author.chatFix").text();
            const userInitials = $(this).find(".user-initials.chatFix");

            let splitName = userName.split(" ");
            var initials = splitName.map(function (word) {
                return word.charAt(0);
            });

            var formattedName = initials.join(".");
            userInitials.text(formattedName);

            $(this).addClass("processed");
        });
    }
    function updateStatus() {
        const $lastMessage = $("#chat-container .message-status:first");
        $lastMessage.addClass("last-message unread");
    }

    return {
        GetDetectionChat,
        GetDetectionFlowData,
        RejectDetection,
        MarkAsRezolvedDetection,
        AssignToDepartmentDetection,
        CreateChatEntry,
        GetTeamsByDepartmentId,
        updateChatInitials
    };
})();