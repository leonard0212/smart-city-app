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
            var url = appUrl + "Billboard/GetBillboardGrid";
            jsUtils.postJsonData($trigger, url, data, true, function (response) {
                $("#grid-data-content").html(response);
            });
        }

        function updatePieCharts() {
            var appUrl = $("#appUrl").val();
            var $trigger = $('#app-loader-container');

            var data = {};
            var url = appUrl + "Billboard/UpdateGraphs";
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




                var sizechart = response.sizePieChart;
                sizechart.chart = options.chart || {};
                sizechart.chart.width = chartWidth;
                sizechart.chart.height = chartHeight;
                sizechart.colors = ["#BF5173", "#000000", "#FFFFFF"];


                var sizechartchartChart2 = new ApexCharts(document.querySelector("#sizeChart"), sizechart);
                sizechartchartChart2.render();

                //var statuschart = response.statusPieChart;
                //statuschart.chart = options.chart || {};
                //statuschart.chart.width = 500;
                //statuschart.chart.height = 500;
                //statuschart.colors = ["#BF5173", "#000000", "#FFFFFF", "#fcba03"];

                //var statuschartChart = new ApexCharts(document.querySelector("#statusChart"), statuschart);
                //statuschartChart.render();

            });
        }

    });

    function getbillboardData(detectionId) {
        var $trigger = $(this);
        var data = { detectionId: detectionId };

        var url = appUrl + "Billboard/GetBillboardData";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            $("#tab2-detail").html(response);
        });
    }


    var billboardUtils = {
        getbillboardData: getbillboardData
    };
    window.billboardUtils = billboardUtils;
});