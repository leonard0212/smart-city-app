"use strict";
$(function () {
    var appUrl = $("#appUrl").val();
    $(document).ready(function () {
        updatePieCharts();
        updateGridData();
       // updateCaruselDetections();
        $(document).on('click', '.grid-view-btn', function () {
            var id = $(this).attr("data-id");
            gmapsUtils.focusMarker(id);
        });

       
    });

    function updatePieCharts() {
        var appUrl = $("#appUrl").val();
        var $trigger = $('#app-loader-container');

        var data = {};
        var url = appUrl + "RoadInfrastructure/UpdateGraphs";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {          

            var widthPercentage = 18; 
            var heightPercentage = 27;

            var chartWidth = (window.innerWidth * widthPercentage) / 100;
            var chartHeight = (window.innerHeight * heightPercentage) / 100;

            var options = response.serverityPieChart;
            options.chart = options.chart || {};
            options.chart.width = chartWidth;
            options.chart.height = chartHeight;
            options.colors = ["#BF5173", "#000000", "#FFFFFF"];

            var severityChart = new ApexCharts(document.querySelector("#severityChart"), options);
            severityChart.render();

            var dimensionchart = response.dimensionPieChart;
            dimensionchart.chart = dimensionchart.chart || {};
            dimensionchart.chart.width = chartWidth;
            dimensionchart.chart.height = chartHeight;
            dimensionchart.colors = ["#BF5173", "#000000", "#FFFFFF"];


            var severityChart2 = new ApexCharts(document.querySelector("#severityChart2"), dimensionchart);
            severityChart2.render();

            var typechart = response.typePieChart;
            typechart.chart = options.chart || {};
            typechart.chart.width = chartWidth;
            typechart.chart.height = chartHeight;
            typechart.colors = ["#BF5173", "#000000", "#FFFFFF", "#fcba03"];

            var typechartChart = new ApexCharts(document.querySelector("#typeChart"), typechart);
            typechartChart.render();

        });
    }


    function updateGridData() {
        var appUrl = $("#appUrl").val();
        var $trigger = $('#app-loader-container');
        var data = {};
        var url = appUrl + "RoadInfrastructure/GetGridData";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#grid-data-content").html(response);
        });
    }


    function getRoadInfrastructureData(detectionId) {
        var $trigger = $(this);
        var data = { detectionId: detectionId };

        var url = appUrl + "RoadInfrastructure/GetRoadInfrastructureData";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#tab2-detail").html(response);
        });
    }


    var roadInfrastructureUtils = {
        getRoadInfrastructureData: getRoadInfrastructureData
    };
    window.roadInfrastructureUtils = roadInfrastructureUtils;



});