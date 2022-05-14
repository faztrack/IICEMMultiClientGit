
//$(document).ready(function () {
//    initializeMap();

//});

var Crewmap,
    Crewlatitude,
    Crewlongitude,
    Crewlatlng,
    address,
    Crewgeocoder = new google.maps.Geocoder();


function initializeMap() {

    // console.log("msg");
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            Crewlatitude = position.coords.latitude;
            Crewlongitude = position.coords.longitude;
            Crewlatlng = new google.maps.LatLng(Crewlatitude, Crewlongitude);
            // console.log(Crewlatlng);
            // alert(latlng);
            Crewgeocoder.geocode({ 'latLng': Crewlatlng }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {

                    if (results[1]) {
                        console.log("OK");
                        address = results[1].formatted_address;
                    }
                }
                else {
                    //alert("Location Not Captured.");
                }


                //var now = new Date();
                //alert(now);
                initialize();

            });

        });
    }
}

function initialize() {
    console.log("OK");
    $.ajax({
        type: "POST",
        url: "mlandingpage.aspx/SetCrewLocation",
        data: "{'latitude':'" + Crewlatitude + "', 'longitude':'" + Crewlongitude + "','address':'" + address + "' }",
        contentType: "application/json; charset=utf-8",
        dataType: "JSON",
        success: function (data) {
            // alert("Inside");
        },
        error: function (e) {
            console.log("there is some error");
            console.log(e);
            //  alert("there is some error");
        }
    });

    // window.onload = navigator.geolocation.getCurrentPosition(initializeMap);
    window.setInterval(function () {
        initializeMap();


    }, 5 * 60 * 1000);


}