"use strict";
(function () {
    var appUrl = $("#appUrl").val();
    $(document).ready(function () {

        var map;
        var map2;
        let markers = [];
        if ($('#Id').length == 0) {

            initMap();
            addMarkers();
            initMapOnTab2();
            $(document).on('click', '.mapInfoClick, .showDetailsFromtable', function () {
                var id = $(this).attr("data-id");
                var url = appUrl + "Detection/Index/" + id;
                window.open(url, '_blank');
            });
        }


        function focusMarker(id) {
            const markerObject = markers.find(m => m.id === id);
            map.setCenter(markerObject.getPosition());
            map.setZoom(18);
            google.maps.event.trigger(markerObject, "click");
        }


        function initMap() {

            map = new google.maps.Map(document.getElementById("map"), {
                center: { lat: 44.439663, lng: 26.096306 },
                zoom: 12,
                fullscreenControl: true,
                mapTypeControl: true,
                //colorScheme:"FOLLOW_SYSTEM",
                controlSize: 22,
                scaleControl: true,
                // zoomControl:false
            });


        }
        function initMapOnTab2() {
            if ($('#map2').length) {
                map2 = new google.maps.Map(document.getElementById("map2"), {
                    center: { lat: 44.439663, lng: 26.096306 },
                    zoom: 12,
                    fullscreenControl: true,
                    mapTypeControl: true,
                    //colorScheme:"FOLLOW_SYSTEM",
                    controlSize: 22,
                    scaleControl: true,
                    // zoomControl:false
                });

            }



        }

        function addMarkers() {
            var appUrl = $("#appUrl").val();
            var $trigger = $(this);

            var category = $("#detectionCategory").val();

            var data = { detectionCategory: category };
            var url = appUrl + "InternalApi/GetMapsData";
            //const customIcon = {
            //    url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVllFehWIPnvseU4OQdPZR3xqPc9QcgPKpjQ&s", // Path to your custom icon
            //    scaledSize: new google.maps.Size(50, 50),  // Size of the icon
            //};

            jsUtils.postJsonData($trigger, url, data, true, function (response) {

                const infoWindow = new google.maps.InfoWindow();
                response.forEach(item => {

                    var markerInfo = { position: { lat: item.lat, lng: item.lng }, info: item.description, id: item.id, severity: item.severity };
                    const marker = new google.maps.Marker({
                        position: markerInfo.position,
                        info: markerInfo.info,
                        map: map,
                        id: markerInfo.id,
                        //icon: customIcon,
                    });
                    marker.severity = item.severity;
                    marker.category = item.category;

                    marker.addListener("click", () => {
                        handleMarkerClick(marker);
                        var content = setInfoMarker(marker);
                        infoWindow.setContent(content);
                        infoWindow.open(map, marker);

                        google.maps.event.addListenerOnce(infoWindow, 'domready', () => {
                            const imageSrcCheck = setInterval(() => {
                                const $img = $('#detectionImagePreview');

                                // If image exists and src is set with content
                                if ($img.length && $img.attr('src') && $img.attr('src').trim().length > 4) {
                                    clearInterval(imageSrcCheck);

                                    // Pan the map 200px upward (marker visually moves down)
                                    setTimeout(() => {
                                        map.panBy(0, -30);
                                    }, 100); // Short delay to ensure image loads into layout
                                }
                            }, 100);

                            // Optional: fail-safe to stop polling after 3 seconds
                            setTimeout(() => clearInterval(imageSrcCheck), 3000);
                        });
                    });
                    markers.push(marker);
                });

                new MarkerClusterer(map, markers, {
                    imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m',
                });
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
                map2.setZoom(17);

            });
        }




        var gmapsUtils = {
            focusMarker: focusMarker,
            addMarkerOnTab2: addMarkerOnTab2,
        };
        window.gmapsUtils = gmapsUtils;
    });

    function handleMarkerClick(marker) {
        ViewSelectedDetection(marker.id)
    }

    function setInfoMarker(marker) {
        var contentString = `
        <div>
            <div class="title-xsmall"><span>${marker.category} </span><span> - ${marker.severity}</span></div>
            <p class="text-xxsmall">${marker.info}</p>
            <div class="img-preview text-center-ic" id="detectionPreview">
            <img id="detectionImagePreview" src=""/>
             <div id="app-max-loader-container" class="max-loader"></div>
            <button type="button" class="show-big-preview" id="showBigPreview" style="
    height: 100%;
    width: 100%;
    background: transparent;
    position: absolute;
    top: 0px;
    z-index: 100000;
    left: 0;
    border:none;
    outline:none;
"></button>
            </div>
         
            <button type="button" class="sc-general-btn ic-active text-xsmall mapInfoClick mt-2" style="width:165px;height:28px" data-id="${marker.id}"><i class="fa-solid fa-magnifying-glass mx-2"></i>Detalii</button>          
        </div>`;
        return contentString;
    }

    function ViewSelectedDetection(id) {
        $(".detection-table-row").removeClass("active-row");
        $(".detection-table-row[data-id='" + id + "']").addClass("active-row");
    }
})();