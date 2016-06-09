$(document).ready(function () {
    Initialize();
});

function Initialize() {
    var mapProp = {
        center: new google.maps.LatLng(20.5937, 78.9629),
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var mapp = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    //alert('MC : ' + $("#selectedMasterClass").val() + ', State : ' + $("#selectedState").val());
    $.ajax({
        type: 'GET',
        url: '/Brand/GetMapData/',
        data: { WebinarID: $("#selectedMasterClass").val(), State: $("#selectedState").val() },
        dataType: 'json',
        success: function (data) {
            $.each(data, function (i, item) {
                console.dir(item);
                var marker;
                $.each(item.Campuses, function (j, jtem) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(jtem.Latitude, jtem.Longitude),
                        map: mapp,
                        //'title': item.City
                    });

                    //// Make the marker-pin blue! 
                    marker.setIcon('http://maps.google.com/mapfiles/ms/micons/blue-dot.png')
                    var infowindow = new google.maps.InfoWindow({
                        content: '<p class="text-light text-caption margin-none padding-none">' +
                            '<img src="../' + jtem.ProfilePicPath.substring(2) + '" alt="person" class="img width-20" /> <b>' + jtem.CampusName + '</b>' +
                                        '<br> <i class="fa fa-group fa-fw"></i> Attended Students: ' + jtem.Participants + '</p>'
                    });

                    // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked! 
                    google.maps.event.addListener(marker, 'mouseover', function () {
                        infowindow.open(mapp, marker);
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
            console.log("Message: " + r.Message);
            console.log("StackTrace: " + r.StackTrace);
            console.log("ExceptionType: " + r.ExceptionType);
        }
    });
}

