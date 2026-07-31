"use strict";
(function () {
    var appUrl = $("#appUrl").val();    
    var map2;
    function initMapOnTab2() {
        map2 = new google.maps.Map(document.getElementById("map2"), {
            center: { lat: 44.439663, lng: 26.096306 },
            zoom: 12,
            fullscreenControl: true,
            mapTypeControl: true,           
            controlSize: 22,
            scaleControl: true,
           
        });
    }

    function addMarkerOnTab2(detectionId) {
        var appUrl = $("#appUrl").val();
        var $trigger = $(this);
        var data = { Id: detectionId };
        var url = appUrl + "InternalApi/GetMapsDataById";
        jsUtils.postJsonData($trigger, url, data, true, function (response) {
            var markerInfo = { position: { lat: response.lat, lng: response.lng }, info: response.description, id: response.id, severity: response.severity };
            const marker = new google.maps.Marker({
                position: markerInfo.position,
                info: markerInfo.info,
                map: map2,
                id: markerInfo.id,
            });
            marker.severity = response.severity;
            map2.setCenter(marker.getPosition());
            map2.setZoom(18);

        });
    }




    var gmapsUtils2 = {
        initMapOnTab2: initMapOnTab2,
        addMarkerOnTab2: addMarkerOnTab2
        
    };
    window.gmapsUtils2 = gmapsUtils2;


  








})();