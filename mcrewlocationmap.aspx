<%@ Page Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mcrewlocationmap.aspx.cs" Inherits="mcrewlocationmap" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta http-equiv="refresh" content="180" />
    <style type="text/css">
        @media (min-width: 1025px) and (max-width:2000px) {
        }

        @media (min-width: 768px) and (max-width: 1024px) {
        }

        @media (min-width: 481px) and (max-width: 767px) {
            #lblCrewName {
                font-size: 20px !important;
                font-weight: bold;
                color: #1574c4;
            }
        }

        @media (min-width: 320px) and (max-width: 480px) {
            #lblCrewName {
                font-size: 20px !important;
                font-weight: bold;
                color: #1574c4;
            }

            #lblAddress, #lblDistanceInMile, #hrfOfficePhone {
                font-size: 18px !important;
                color: #1574c4;
            }

            .profile-center {
                text-align: center;
            }

            .wrapper {
                background-color: #f8f8f8;
                border: 1px solid #ddd;
                margin: 10px auto !important;
                padding: 10px;
                width: 100%;
                border-radius: 25px !important;
            }
        }
    </style>


    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyCRZPUFaX1DhOty-6E6Eb8eLQQDb6Jt5Vg"
        type="text/javascript"></script>

    <script type="text/javascript">


        var map, CustomerDetailesWitheCoord, CreweCoord, mapCenter, latitude, longitude, address;
        function initializeMap() {

            if (navigator.geolocation) {

                navigator.geolocation.getCurrentPosition(function (position) {
                    latitude = position.coords.latitude;
                    longitude = position.coords.longitude;

                    insertLatLong();
                });


            }


        }

        function insertLatLong() {

            $.ajax({
                type: "POST",
                url: "mlandingpage.aspx/SetCrewLocation",
                data: "{'latitude':'" + latitude + "', 'longitude':'" + longitude + "','address':'" + address + "' }",
                contentType: "application/json; charset=utf-8",
                dataType: "JSON",
                success: function (data) {
                    //  alert("Inside");
                },
                error: function (e) {
                    console.log("there is some error");
                    console.log(e);
                    //  alert("there is some error");
                }
            });
        }

        function loadMarkerFromAddress(CustomerDetailesWitheCoord, CreweCoord, SupereCoord) {
            // console.log(CustomerDetailesWitheCoord);
            /////////for customer///////////////////////


            var result = JSON.parse(CustomerDetailesWitheCoord)
            var locations = [];


            var keys = Object.keys(result);
            keys.forEach(function (key) {
                locations.push(result[key]);
            });
            var len = locations.length;
            //////////////////////for crew//////////////////////////
            var resultforCrew = JSON.parse(CreweCoord)
            var Crewlocations = [];


            var keysforCrew = Object.keys(resultforCrew);
            keysforCrew.forEach(function (key) {
                Crewlocations.push(resultforCrew[key]);
            });
            var CrewLen = Crewlocations.length;
            //////////////////////for super////////////////////////////
            var resultforSuper = JSON.parse(SupereCoord)
            var Superlocations = [];


            var keysforSuper = Object.keys(resultforSuper);
            keysforSuper.forEach(function (key) {
                Superlocations.push(resultforSuper[key]);
            });
            var SuperLen = Superlocations.length;
            // alert(new google.maps.LatLng(locations[0].CustLatitude, locations[0].CustLongitude));
            console.log(SuperLen);
            if (len > 0) {
                mapCenter = new google.maps.LatLng(locations[0].CustLatitude, locations[0].CustLongitude);
            } else if (CrewLen > 0) {
                mapCenter = new google.maps.LatLng(Crewlocations[0].CrewLatitude, Crewlocations[0].CrewLongitude);
            }
            else {
                mapCenter = new google.maps.LatLng(33.4122784, -112.0836409);
            }
            var geocoder = geocoder = new google.maps.Geocoder();
            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                zoom: 12,

                center: mapCenter,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });




            var infowindow = new google.maps.InfoWindow();

            for (var i = 0; i < locations.length; i++) {
                var content = '<div class="map-content"><h3>' + locations[i].CfullName.toString() + '</h3>' + locations[i].CustomerAddress.toString() + '<br /></div>';
                var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(locations[i].CustLatitude, locations[i].CustLongitude),
                    icon: {
                        labelOrigin: new google.maps.Point(16, 40),
                        url: "https://faztimate.holtzmanhomeimprovement.com/images/pin_customer.png"
                    },
                    draggable: true,
                    map: map,



                    animation: google.maps.Animation.DROP,




                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, content) {
                    return function () {

                        infowindow.setContent(content);
                        infowindow.open(map, marker);
                    }
                })
                    (marker, content)

                )
                google.maps.event.addListener(marker, 'mouseout', (function (marker, content) {
                    return function () {
                        infowindow.close();
                    }
                })
                    (marker, content)

                )
                initializeMap();

            };
            //////////////////////////////crew marker//////////////////////////////
            for (var i = 0; i < Crewlocations.length; i++) {
                var Crewcontent = '<div class="map-content"><h2>Crew</h2><h3>' + Crewlocations[i].CfullName.toString() + '</h3></div>';
                var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(Crewlocations[i].CrewLatitude, Crewlocations[i].CrewLongitude),
                    //  icon: 'https://faztimate.holtzmanhomeimprovement.com/images/pin_crew.png',

                    icon: {
                        labelOrigin: new google.maps.Point(16, 40),
                        url: "https://faztimate.holtzmanhomeimprovement.com/images/pin_crew.png"
                    },
                    draggable: false,
                    map: map,



                    animation: google.maps.Animation.DROP,




                });

                // Get Addresss Initial Address
                latlng = new google.maps.LatLng(Crewlocations[i].CrewLatitude, Crewlocations[i].CrewLongitude);

                var geocoder = geocoder = new google.maps.Geocoder();

                geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {

                        if (results[1]) {

                        }
                    }
                    else {
                        alert("Location Not Captured.");
                    }
                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, Crewcontent) {
                    return function () {
                        // marker.setLabel(Crewlocations[i].CfullName.toString());
                        infowindow.setContent(Crewcontent);
                        infowindow.open(map, marker);
                    }
                })
                    (marker, Crewcontent)

                )
                google.maps.event.addListener(marker, 'mouseout', (function (marker, Crewcontent) {
                    return function () {
                        infowindow.close();
                    }
                })
                    (marker, Crewcontent)

                )

            };



        }

        function DefaultrCustomerinitializeMap(DefaultrCustomerLat, DefaultrCustomerlong) {

            var geocoder = geocoder = new google.maps.Geocoder();

            var latlng = new google.maps.LatLng(DefaultrCustomerLat, DefaultrCustomerlong);

            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                zoom: 10,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

        }
    </script>














    <div class="container-fluid">

        <div class="panel panel-default">
            <div class="panel-heading panel-heading-ext">

                <h3 class="panel-title">
                    <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                    <strong>Enroute</strong>

                </h3>

            </div>
            <div class="panel-body" style="padding-bottom: 0px; margin-bottom: 50px;">

                <div id="directionpanel" runat="server" style="margin: 5px 0; display: none;"></div>
                <div id="map" runat="server" style="height: 380px; margin: 0;"></div>
                <div class="row">
                    <div class="ol-lg-12  col-sm-12">
                        <div class="form-horizontal">

                            <div class="form-group wrapper">

                                <div class="col-sm-6 profile-center">
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/pin_customer.png" />
                                    <asp:Label ID="lblCrewName" runat="server" Text="" Font-Size="18px"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">

                                <div class="col-sm-12">
                                    <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                    <asp:HyperLink ID="hypAddress" runat="server" Target="_blank" Font-Size="16px" ForeColor="#175bb8" Style="text-decoration: underline"></asp:HyperLink>

                                </div>

                            </div>

                            <div class="form-group">
                                <div class="col-sm-2">
                                    <asp:Label ID="lblDistance" runat="server" Text="Est distance:" Font-Bold="true" Font-Size="16px"></asp:Label>
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label ID="lblDistanceInMile" runat="server" Text="" Font-Size="16px"></asp:Label>
                                </div>
                            </div>
                            <%-- <div class="form-group">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label2" runat="server" Text="Est arrival time:" Font-Bold="true"></asp:Label>
                                    </div>
                                    <div class="col-sm-8">
                                        <asp:Label ID="lblArrivalTime" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>--%>
                            <div class="form-group">
                                <div class="col-sm-2">
                                    <asp:Label ID="Label3" runat="server" Text="Customer phone:" Font-Bold="true" Font-Size="16px"></asp:Label>
                                </div>
                                <div class="col-sm-8">
                                    <a id="hrfOfficePhone" runat="server" style="text-decoration: underline">
                                        <asp:Label ID="lblOfficePhone" runat="server" Text="" Font-Size="16px" ForeColor="#175bb8"></asp:Label></a>

                                </div>
                            </div>
                        </div>


                    </div>
                </div>

            </div>


        </div>


    </div>




</asp:Content>



