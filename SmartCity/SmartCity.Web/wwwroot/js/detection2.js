"use strict";
(function () {
    var appUrl = $("#appUrl").val();

    $(document).ready(function () {

        console.log("detection2.js");
        if (document.getElementById("Id")) {
            var id = $('#Id').val()
        }
       
        gmapsUtils2.initMapOnTab2(id);
        gmapsUtils2.addMarkerOnTab2(id);

        $(document).on("click", "#showTabChat", function () {
            var detectionId = $('#Id').val();
            DetectionModule.GetDetectionChat(detectionId);
        });

        $(document).on("click", "#showTabFlux", function () {
            var detectionId = $('#Id').val();
            DetectionModule.GetDetectionFlowData(detectionId);
        });

        //$(document).on("click", "#btn-reject", function () {
        //    var detectionId = $('#Id').val();
        //    DetectionModule.RejectDetection(detectionId);
        //});

        //$(document).on("click", "#btn-markAsRezolved", function () {
        //    var detectionId = $('#Id').val();
        //    DetectionModule.MarkAsRezolvedDetection(detectionId);
        //});

        //$(document).on("click", "#btn-assignToDepartament", function () {

        //    var detectionId = $('#Id').val();
        //    DetectionModule.AssignToDepartmentDetection(detectionId);
        //});


        //$(document).on("click", "#btn-add-chat-message", function () {
        //    debugger;
        //    var detectionId = $('#Id').val();
        //    var text = $("#detection-chat-txt").val();
        //    DetectionModule.CreateChatEntry(detectionId, text);
        //});

    });
    //function GetDetectionChat(detectionId) {
    //    var $trigger = $(this);
    //    var data = { detectionId: detectionId };
    //    var url = appUrl + "InternalApi/GetDetectionChat";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#chat-container").html(response);
    //    });
    //}

    //function GetDetectionFlowData(detectionId) {
    //    var $trigger = $(this);
    //    var data = { Id: detectionId };
    //    var url = appUrl + "Detection/GetDetectionFlowData";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#flow-container").html(response);
    //    });
    //}

    //function RejectDetection(detectionId) {
    //    var $trigger = $(this);
    //    var text = $("#rejectText").val();
    //    var data = { detectionId: detectionId, text: text };
    //    var url = appUrl + "Detection/RejectDetection";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#flow-container").html(response);
    //    });
    //}

    //function MarkAsRezolvedDetection(detectionId) {
    //    var $trigger = $(this);
    //    var text = $("#markAsRezolvedText").val();
    //    var data = { detectionId: detectionId, text: text };
    //    var url = appUrl + "Detection/MarkAsRezolvedDetection";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#flow-container").html(response);
    //    });
    //}

    //function AssignToDepartmentDetection(detectionId) {
    //    var $trigger = $(this);
    //    var text = $("#assignToDepartamentText").val();
    //    var departamentId = $("#AssignToDepartment_DepartamentId").val();
    //    var teamId = $("#AssignToDepartment_TeamId").val();
    //    var data = { detectionId: detectionId, text: text, departamentId: departamentId, teamId: teamId };
    //    var url = appUrl + "Detection/AssignToDepartamentDetection";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#flow-container").html(response);
    //    });
    //}



    //function GetTeamsByDepartmentId(departamentId) {
    //    var $trigger = $(this);
    //    var data = { deparmentId: departamentId };
    //    var url = appUrl + "InternalApi/GetTeamsByDepartmentId";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        debugger;
    //        var $dropdown = $('#AssignToDepartment_TeamId');
    //        $dropdown.empty();
    //        $.each(response, function (index, item) {
    //            $dropdown.append(
    //                $('<option></option>').val(item.value).text(item.text)
    //            );
    //        });
    //    });
    //}



    //function CreateChatEntry(detectionId, text) {
    //    var $trigger = $(this);
    //    var data = { detectionId: detectionId, detectionText: text };
    //    var url = appUrl + "InternalApi/AddDetectionChat";
    //    jsUtils.postJsonData($trigger, url, data, true, function (response) {
    //        $("#chat-container").html(response);
    //    });
    //}
    //function updateChatInitials() {
    //    $(".chat-message:not(.processed)").each(function () {
    //        const userName = $(this).find(".chat-message-author.chatFix").text();
    //        const userInitials = $(this).find(".user-initials.chatFix");

    //        let splitName = userName.split(" ");
    //        var initials = splitName.map(function (word) {
    //            return word.charAt(0);
    //        });

    //        var formattedName = initials.join(".");
    //        userInitials.text(formattedName);

    //        $(this).addClass("processed");
    //    });
    //}
    //function updateStatus() {
    //    const $lastMessage = $("#chat-container .message-status:first");
    //    $lastMessage.addClass("last-message unread");
    //}
})();