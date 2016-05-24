$(document).ready(function () {
    Initialize();
});

function Initialize() {
    var mapProp = {
        center: new google.maps.LatLng(21.7679, 78.8718),
        zoom: 4,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

    $.ajax({
        type: 'GET',
        url: '/Brand/GetMapData/',
        dataType: 'json',
        success: function (data) {
            console.dir(data);
            var items = '';
            $.each(data, function (i, item) {
                console.dir(item);
                $.each(item.Campuses, function (j, jtem) {
                    console.dir(jtem.Latitude);
                    console.dir(jtem.Longitude);
                    var marker = new google.maps.Marker({
                        'position': new google.maps.LatLng(jtem.Latitude, jtem.Longitude),
                        'map': map,
                        //'title': item.City
                    });

                    //// Make the marker-pin blue! 
                    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png')
                    var infowindow = new google.maps.InfoWindow({
                        content: "<div class='infoDiv'><h2>" + jtem.CampusName + "</div></div>"
                    });

                    // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked! 
                    google.maps.event.addListener(marker, 'mouseover', function () {
                        infowindow.open(map, marker);
                    });
                    google.maps.event.addListener(marker, 'mouseout', function () {
                        infowindow.close();
                    });
                });
            });
        },
        error: function (response) {
            console.dir(response);
            var r = jQuery.parseJSON(response.responseText);
            alert("Message: " + r.Message);
            alert("StackTrace: " + r.StackTrace);
            alert("ExceptionType: " + r.ExceptionType);
        }
    });
}

