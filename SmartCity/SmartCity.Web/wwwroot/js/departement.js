"use strict";
$(function () {
    var appUrl = $("#appUrl").val();
    $(document).ready(function () {

        $(document).on('click', '.btn-addUserToTeam', function () {
            var userId = $(this).attr("data-id");
            addUserToTeam(userId);
        });

        $(document).on('click', '.btn-removeUserFromTeam', function () {
            var userId = $(this).attr("data-id");
            removeUserFromTeam(userId);
        });

        
    });


    function addUserToTeam(userId) {
        var teamId = $("#Id").val();      
        var $trigger = $('#app-loader-container');
        var data = { teamId: teamId, userId: userId };

        var url = appUrl + "Department/AddUserToTeam";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            location.reload();
        });
    }

    function removeUserFromTeam(userId) {
        var teamId = $("#Id").val();
        var $trigger = $('#app-loader-container');
        var data = { teamId: teamId, userId: userId };

        var url = appUrl + "Department/RemoveUserFromTeam";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            location.reload();
        });
    }



});