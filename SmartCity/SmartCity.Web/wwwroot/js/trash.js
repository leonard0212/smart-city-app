"use strict";
$(function () {
    var appUrl = $("#appUrl").val();
    $(document).ready(function () {

        updateGridData();
        updatePieCharts();
        $(document).on('click', '.grid-view-btn', function () {
            var id = $(this).attr("data-id");
            gmapsUtils.focusMarker(id);
        });

        function updateGridData() {
            var appUrl = $("#appUrl").val();
            var $trigger = $('#app-loader-container');
            var data = {};
            var url = appUrl + "Trash/GetTrashdGrid";
            jsUtils.postJsonData($trigger, url, data, true, function (response) {
                $("#grid-data-content").html(response);
            });
        }
        function updatePieCharts() {
            var appUrl = $("#appUrl").val();
            var $trigger = $('#app-loader-container');

            var data = {};
            var url = appUrl + "Trash/UpdateGraphs";
            jsUtils.postJsonData($trigger, url, data, true, function (response) {
                var widthPercentage = 18;
                var heightPercentage = 27;

                var chartWidth = (window.innerWidth * widthPercentage) / 100;
                var chartHeight = (window.innerHeight * heightPercentage) / 100;

                var options = response.categoryPieChart;

                options.chart = options.chart || {};
                options.chart.width = chartWidth;
                options.chart.height = chartHeight;
                options.colors = ["#BF5173", "#000000", "#FFFFFF"];

                var categoryChart = new ApexCharts(document.querySelector("#categoryChart"), options);
                categoryChart.render();




                var volumechart = response.volumePieChart;
                volumechart.chart = options.chart || {};
                volumechart.chart.width = chartWidth;
                volumechart.chart.height = chartHeight;
                volumechart.colors = ["#BF5173", "#000000", "#FFFFFF"];


                var volumechartChart2 = new ApexCharts(document.querySelector("#volumeChart"), volumechart);
                volumechartChart2.render();

                var statuschart = response.statusPieChart;
                statuschart.chart = options.chart || {};
                statuschart.chart.width = chartWidth;
                statuschart.chart.height = chartHeight;
                statuschart.colors = ["#BF5173", "#000000", "#FFFFFF", "#fcba03"];

                var statuschartChart = new ApexCharts(document.querySelector("#statusChart"), statuschart);
                statuschartChart.render();

            });
        }



    });
    function getTrashData(detectionId) {
        var $trigger = $(this);
        var data = { detectionId: detectionId };

        var url = appUrl + "Trash/GetTrashData";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#tab2-detail").html(response);
        });
    }

    var trashUtils = {
        getTrashData: getTrashData
    };
    window.trashUtils = trashUtils;
});