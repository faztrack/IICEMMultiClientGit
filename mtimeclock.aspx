<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mtimeclock.aspx.cs" Inherits="mtimeclock" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; }
    </script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAg9x2CEaBTyFmwXm75gvfmQVuOGcSND0Y"
        type="text/javascript"></script>

    <script type="text/javascript">
        var map,
            latitude,
            longitude,
            directionsDisplay,
            latlng,
            polyline,
            stopover = [],
            totalDistance = 0,
            geocoder = new google.maps.Geocoder(),
            directionsService = new google.maps.DirectionsService();

        function geocodePosition(pos) {
            geocoder.geocode({
                latLng: pos
            }, function (responses) {
                if (responses && responses.length > 0) {

                    updateMarkerAddress(responses[0].formatted_address);
                } else {
                    updateMarkerAddress('Cannot determine address at this location.');
                }
            });
        }
        function updateMarkerStatus(str) {
            document.getElementById("<%=lblmarkerStatus.ClientID%>").innerHTML = "Marker Status: " + str + "\n";
        }
        function updateMarkerPosition(latLng) {
            alert("Current Position");
            document.getElementById("<%=lblInfo.ClientID%>").innerHTML = "Current Position: " + [
              latLng.lat(),
              latLng.lng()
            ].join(', ') + "\n";
        }
        function updateMarkerAddress(str) {
            document.getElementById("<%=lblAddress.ClientID%>").innerHTML = "Current Location: " + str + "\n";
            alert("Ok Address");
        }


        function initialize() {
            $.ajax({
                type: "POST",
                url: "mlandingpage.aspx/SetCrewLocation",
                data: "{'latitude':'" + latitude + "', 'longitude':'" + longitude + "' }",
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

        
            window.setInterval(function () {
                initializeMap();
            }, 5 * 60 * 1000);


        }

        function initializeMap() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    latitude = position.coords.latitude;
                    longitude = position.coords.longitude;
                    document.getElementById("<%=hdnLatitude.ClientID %>").value = latitude;
                    document.getElementById("<%=hdnLongitude.ClientID %>").value = longitude;
                    document.getElementById('<%=lblResult.ClientID%>').innerHTML = "";


                    // Get Addresss Initial Address
                    latlng = new google.maps.LatLng(latitude, longitude);

                    var geocoder = geocoder = new google.maps.Geocoder();

                    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {

                            if (results[1]) {
                                document.getElementById("<%=hdnStartLocation.ClientID %>").value = results[1].formatted_address;
                                document.getElementById("<%=hdnEndLcation.ClientID %>").value = results[1].formatted_address;
                            }
                        }
                        else {
                           // alert("Location Not Captured.");
                        }
                    });


                    // Device Name

                    var isMobile = {
                        Android: function () {
                            return navigator.userAgent.match(/Android/i);
                        },
                        BlackBerry: function () {
                            return navigator.userAgent.match(/BlackBerry/i);
                        },
                        iOS: function () {
                            return navigator.userAgent.match(/iPhone|iPad|iPod/i);
                        },
                        Opera: function () {
                            return navigator.userAgent.match(/Opera Mini/i);
                        },
                        Windows: function () {
                            return navigator.userAgent.match(/IEMobile/i);
                        },
                        any: function () {
                            return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
                        }
                    };
                    /* if the device is not ios hide the download button */
                    if (isMobile.Android()) {
                        document.getElementById("<%=hdnDeviceName.ClientID %>").value = 'Android';

                    }
                    else if (isMobile.iOS()) {
                        document.getElementById("<%=hdnDeviceName.ClientID %>").value = 'iOS';
                    }
                    else if (isMobile.BlackBerry()) {
                        document.getElementById("<%=hdnDeviceName.ClientID %>").value = 'BlackBerry';
                    }
                    else {
                        document.getElementById("<%=hdnDeviceName.ClientID %>").value = 'Other';
                    }


                    //End Device Name


                    latlng = new google.maps.LatLng(latitude, longitude);
                    directionsDisplay = new google.maps.DirectionsRenderer({ polylineOptions: { map: map, strokeColor: "#8E0011" } });
                    map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                        zoom: 10,
                        center: latlng,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    });



                    directionsDisplay.setMap(map);
                    directionsDisplay.setOptions({ suppressMarkers: true });
                    directionsDisplay.setPanel(document.getElementById("<%=directionpanel.ClientID%>"));
                    document.getElementById("<%=hdnLatLng.ClientID %>").value = 1;
                });
                // Add dragging event listeners.
                //google.maps.event.addListener(marker, 'dragstart', function () {
                //    updateMarkerAddress('Dragging...');
                //});
                //google.maps.event.addListener(marker, 'drag', function () {
                //    updateMarkerStatus('Dragging...');
                //    updateMarkerPosition(marker.getPosition());
                //});
                //google.maps.event.addListener(marker, 'dragend', function () {
                //    updateMarkerStatus('Drag ended');
                //    geocodePosition(marker.getPosition());
                //    addLocation(marker.getPosition());
                //});
            } else {
                alert("Geo Location is not supported on your current browser !!!");
            }
        }

        function setLocation(x, y, z) {
            var type, lat, lng, i;
            type = x.split(",");
            lat = y.split(",");
            lng = z.split(",");
            stopover = [];
            for (i = 0; i < type.length; i++) {
                if (type[i] != null && type[i] != "" && lat[i] != "" && lat[i] != null && lat[i] != 0) {
                    latlng = new google.maps.LatLng(lat[i], lng[i]);
                    stopover.push(latlng);
                    if (type[i] == 1) {
                        polyline = new google.maps.Polyline({
                            strokeColor: '#8E0011',
                            strokeOpacity: 1.0,
                            strokeWeight: 3
                        });
                        var path = polyline.getPath();
                        path.push(latlng);
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png'
                            //{
                            //    path: google.maps.SymbolPath.CIRCLE,
                            //    strokeColor: "green",
                            //    scale: 10
                            //},
                        });
                        marker.setTitle("Latitude: " + lat[i] + "\n" + "Longitude: " + lng[i]);
                        marker.setMap(map);
                        marker.setDraggable(false);
                        //map.setCenter(latlng);
                        map.panTo(latlng);
                    }
                    else if (type[i] == 2) {
                        var path = polyline.getPath();
                        path.push(latlng);
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon: 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png'
                            //{
                            //    path: google.maps.SymbolPath.CIRCLE,
                            //    strokeColor: "yellow",
                            //    scale: 10
                            //},
                        });
                        marker.setTitle("Latitude: " + lat[i] + "\n" + "Longitude: " + lng[i]);
                        marker.setMap(map);
                        marker.setDraggable(false);
                        //map.setCenter(latlng);
                        map.panTo(latlng);
                    }
                    else if (type[i] == 4) {
                        var path = polyline.getPath();
                        path.push(latlng);
                        map.panTo(latlng);
                    }
                }
            }
        }

        function getCurrentLocation(value) {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    latitude = position.coords.latitude;
                    longitude = position.coords.longitude;
                    latlng = new google.maps.LatLng(latitude, longitude);
                    stopover.push(latlng);
                    var len = stopover.length;
                    if (len >= 2) {
                        totalDistance = google.maps.geometry.spherical.computeDistanceBetween(stopover[len - 2], stopover[len - 1]); // Returns the distance, in meters, between two LatLngs
                        totalDistance = totalDistance * 3.28084;
                    }


                    if (value == 1) {
                        totalDistance = 0;
                        polyline = new google.maps.Polyline({
                            strokeColor: '#8E0011',
                            strokeOpacity: 1.0,
                            strokeWeight: 3
                        });
                        var path = polyline.getPath();
                        path.push(latlng);
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon: 'http://maps.google.com/mapfiles/ms/icons/green-dot.png'

                        });
                        marker.setTitle("Latitude: " + latitude + "\n" + "Longitude: " + longitude);
                        marker.setMap(map);
                        marker.setDraggable(true);
                        //map.setCenter(latlng);
                        map.panTo(latlng);


                    }
                    else if (value == 2) {
                        var path = polyline.getPath();
                        path.push(latlng);
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon: 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png'

                        });
                        marker.setTitle("Latitude: " + latitude + "\n" + "Longitude: " + longitude);
                        marker.setMap(map);
                        marker.setDraggable(false);
                        //map.setCenter(latlng);
                        map.panTo(latlng);
                    }
                    else if (value == 3) {

                        var path = polyline.getPath();
                        path.push(latlng);
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon: 'http://maps.google.com/mapfiles/ms/icons/red-dot.png'

                        });
                        marker.setTitle("Latitude: " + latitude + "\n" + "Longitude: " + longitude);
                        marker.setMap(map);
                        marker.setDraggable(false);
                        //map.setCenter(latlng);
                        map.panTo(latlng);
                        //polyline.setMap(map);

                        // End Location
                        var geocoder = geocoder = new google.maps.Geocoder();
                        // alert(geocoder);
                        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
                            if (status == google.maps.GeocoderStatus.OK) {

                                if (results[1]) {

                                    document.getElementById("<%=hdnEndLcation.ClientID %>").value = results[1].formatted_address;

                                }
                            }
                            else {
                                alert("Location Not Captured.");
                            }
                        });
                    }
                    else {

                        var path = polyline.getPath();
                        path.push(latlng);
                    }
                    document.getElementById("<%=hdnDistance.ClientID %>").value = totalDistance;
                    document.getElementById("<%=hdnLatitude.ClientID %>").value = latitude;
                    document.getElementById("<%=hdnLongitude.ClientID %>").value = longitude;
                    document.getElementById("<%=hdnLatLng.ClientID %>").value = value;


                });
            } else {
                alert("Geo Location is not supported on your current browser !!!");
            }
        }



        function computeTotalDistance(result) {

            var total = 0;
            var myroute = result.routes[0];
            for (var i = 0; i < myroute.legs.length; i++) {
                total += myroute.legs[i].distance.value;
            }
            total = total * 3.28084;
            document.getElementById("<%=hdnDistance.ClientID %>").value = total;

        }

        function getMapRoute() {
            var waypts = [];
            directionsDisplay.setMap(map);
            for (var i = 1; i < stopover.length - 1; i++) {
                if (i >= 9) { break; }
                waypts.push({
                    location: stopover[i],
                    stopover: false
                });
            }
            var start = stopover[0],
                end = stopover[stopover.length - 1],
                request = {
                    origin: start,
                    destination: end,
                    waypoints: waypts,
                    travelMode: google.maps.DirectionsTravelMode.DRIVING,
                    avoidHighways: false,
                    avoidTolls: false
                };
            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                    computeTotalDistance(directionsDisplay.getDirections());
                }
            });
        }
        window.onload = navigator.geolocation.getCurrentPosition(initializeMap);
    </script>

    <script type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }

        function searchKeyPress(e) {
            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;
        }

    </script>

    <style>
        /* (320x480) Smartphone, Portrait */
        /* and (max-device-width : 480px) and (-webkit-device-pixel-ratio : 3) */
        .cellColor {
            font-weight: bold;
        }

        @media only screen and (min-device-width: 320px) and (orientation: portrait) {
            .gridResponsive {
                transform: scale(.65);
                -webkit-transform: scale(.65); /* Safari and Chrome */
                -moz-transform: scale(.65); /* Firefox */
                margin: -540px -108px !important;
            }
            /* (320x480) Smartphone, Portrait */
        }

        /* iPhone X-XS in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 812px) and (-webkit-device-pixel-ratio : 3) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone X-XS in portrait */
        }

        /* iPhone 6, 7, & 8 in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 667px) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone 6, 7, & 8 in portrait */
        }


        @media (min-width: 1281px) {
            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
        }
        /* 
  ##Device = Laptops, Desktops
  ##Screen = B/w 1025px to 1280px
*/

       
        @media (min-width: 1025px) and (max-width:2000px) {

            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
                 .mapmargin {
                margin-bottom: 240px !important;
            }
        }

        /* 
  ##Device = Tablets, Ipads (portrait)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) {

            .mobileGrid {
                font-size: 13px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 14px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
                 .mapmargin {
                margin-bottom: 240px !important;
            }
        }

        /* 
  ##Device = Tablets, Ipads (landscape)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) and (orientation: landscape) {

            .mobileGrid {
                font-size: 10px;
            }

                .mobileGrid th {
                    padding: 10px;
                    font-size: 10px;
                }

                .mobileGrid tbody tr td {
                    padding: 5px;
                    margin: 5px;
                }
                     .mapmargin {
                margin-bottom: 240px !important;
            }
        }

        /* 
  ##Device = Low Resolution Tablets, Mobiles (Landscape)
  ##Screen = B/w 481px to 767px
*/

        @media (min-width: 481px) and (max-width: 767px) {

            .mobileGrid {
                font-size: 9px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 2px;
                    font-size: 9px;
                }

                .mobileGrid tbody tr td {
                    padding: 2px;
                    margin: 5px;
                }
                  .mapmargin {
                margin-bottom: 8px !important;
        }

        /* 
  ##Device = Most of the Smartphones Mobiles (Portrait)
  ##Screen = B/w 320px to 479px
*/

        @media (min-width: 320px) and (max-width: 480px) {

            .mobileGrid {
                font-size: 10px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 0px;
                    font-size: 10px;
                }

                .mobileGrid tbody tr td {
                    padding: 0px;
                    margin: 5px;
                }
                 .mapmargin {
                margin-bottom: 50px !important;
        }

        body {
            margin-bottom: 50px;
        }

        .grid_header {
            color: #000;
            font-weight: bold;
            font-size: 14px;
        }

        .mobileGrid {
            color: #333;
            border-radius: 0;
        }

            .mobileGrid th {
                display: normal !important;
                background-color: #e1e1e1;
                color: #555;
                font-weight: bold;
                border: 1px solid #ddd;
                text-transform: uppercase;
            }

            /*.mobileGrid tbody tr td {
                width: 12%;
            }*/

            .mobileGrid td {
                background-color: #f9f9f9;
                border: 1px solid #ddd;
            }

                .mobileGrid td td {
                    background-color: transparent;
                    box-shadow: none;
                }
    </style>



    <div class="panel panel-default">
        <div class="panel-heading panel-heading-ext">

            <h3 class="panel-title">
                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                <strong>Time Clock

                </strong>

            </h3>

        </div>
        <div class="panel-body" style="padding-bottom: 0px; margin-bottom: 50px;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>



                    <div class="row">
                        <div class="col-lg-12 col-sm-6 col-md-12">
                            <div class="clearfixM">
                                <div class="row">


                                    <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                        <div style="text-align: center; width: 100px;">
                                            <span style="font-weight: bold;">Date:</span>
                                            <asp:Label ID="lblDateTime" runat="server" Text="" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12 col-sm-6 col-md-12">
                            <div class="table-responsive">
                                <table width="100%">
                                    <tr>
                                        <td colspan="7">
                                            <table style="margin-top: 20px;" width="100%">
                                                <tr>
                                                    <td align="center" class="cssHeader">
                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                            <tr>
                                                                <td style="padding: 10px;" align="center"><span class="titleNu"><span style="border: 2px solid #992a24; border-radius: 25px; padding: 5px 10px;" id="spnPnlSection" class="cssTitleHeader">
                                                                    <asp:ImageButton ID="ImageSectionMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="40" />
                                                                    <font style="font-size: 16px; cursor: pointer;">My Hours</font></span></span>
                                                                    <asp:CollapsiblePanelExtender ID="CollapsiblePanelExtender7" runat="server" CollapseControlID="spnPnlSection" Collapsed="True" CollapsedImage="Images/expand.png" ExpandControlID="spnPnlSection" ExpandedImage="Images/collapse.png" ImageControlID="ImageSectionMain" SuppressPostBack="true" TargetControlID="pnlSection">
                                                                    </asp:CollapsiblePanelExtender>
                                                                </td>
                                                                <td align="right"></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="pnlSection" runat="server">
                                                            <table align="center" cellpadding="0" cellspacing="2" class="wrapper" width="100%">
                                                                <tr>
                                                                    <td align="center">

                                                                        <table cellpadding="5" cellspacing="5" width="100%">

                                                                            <tr>
                                                                                <td align="center">
                                                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table style="margin-top: 10px;" width="100%">
                                                                                                    <tr>

                                                                                                        <td style="width: 25%;" align="left"><strong style="font-size: 14px;">Date Range:</strong></td>
                                                                                                        <td style="width: 65%;" align="left">
                                                                                                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" Style="width: 100%; font-size: 14px;" TabIndex="1"></asp:TextBox>
                                                                                                            <asp:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate" PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                                                                            </asp:CalendarExtender>
                                                                                                            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtStartDate" WatermarkText="Start Date" />
                                                                                                        </td>
                                                                                                        <td align="left">
                                                                                                            <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                                                        </td>
                                                                                                    </tr>

                                                                                                    <tr>

                                                                                                        <td align="right">&nbsp;</td>
                                                                                                        <td style="width: 80%;" align="left">
                                                                                                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" Style="width: 100%; font-size: 14px;" TabIndex="2"></asp:TextBox>
                                                                                                            <asp:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEndDate" PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                                                                            </asp:CalendarExtender>
                                                                                                            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtEndDate" WatermarkText="End Date" />
                                                                                                        </td>
                                                                                                        <td align="left">
                                                                                                            <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                                <table style="margin: 20px 0;" width="100%">
                                                                                                    <tr>
                                                                                                        <td align="left" style="width: 22%">
                                                                                                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="underlineButton" Text="Reset" OnClick="lnkViewAll_Click" Font-Size="16px"></asp:LinkButton>
                                                                                                        </td>
                                                                                                        <td align="center" style="width: 60%">
                                                                                                            <b>Page: </b>
                                                                                                            <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                                                                            &nbsp;
                                                                                                           <b>Per page: </b>
                                                                                                            <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                                                                                <asp:ListItem Selected="True">10</asp:ListItem>
                                                                                                                <asp:ListItem>20</asp:ListItem>
                                                                                                                <asp:ListItem>30</asp:ListItem>
                                                                                                                <asp:ListItem>40</asp:ListItem>

                                                                                                            </asp:DropDownList>
                                                                                                        </td>
                                                                                                        <td align="right" style="width: 20%">

                                                                                                            <asp:LinkButton ID="btnView" runat="server" CssClass="underlineButton" Text="View" OnClick="btnView_Click" Font-Size="16px"></asp:LinkButton>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                                <table width="100%">

                                                                                                    <tr>
                                                                                                        <td align="center" colspan="3">
                                                                                                            <asp:Label ID="lblMSG" runat="server" Text=""></asp:Label>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center" valign="top" colspan="7">
                                                                                                            <asp:Label ID="Label2" runat="server"></asp:Label>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                                                                                        </td>
                                                                                                        <td align="right">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="right">
                                                                                                            <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" CssClass="nextButton" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="center" colspan="7">
                                                                                                            <asp:GridView ID="grdLaberTrack" runat="server" AllowPaging="True"
                                                                                                                AutoGenerateColumns="False" OnRowDataBound="grdLaberTrack_RowDataBound"
                                                                                                                OnPageIndexChanging="grdLaberTrack_PageIndexChanging" ShowFooter="true"
                                                                                                                Width="100%" CssClass="mobileGrid itemName">
                                                                                                                <PagerSettings Position="TopAndBottom" />
                                                                                                                <Columns>
                                                                                                                    <asp:BoundField HeaderText="Labor Date" DataField="labor_date" DataFormatString="{0:d}">
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </asp:BoundField>
                                                                                                                    <asp:BoundField HeaderText="Start Location" DataField="StartPlace">
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="17%" />
                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                    </asp:BoundField>
                                                                                                                    <asp:BoundField DataField="EndPlace" HeaderText="End Location">
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                                                                        <ItemStyle HorizontalAlign="left" />
                                                                                                                    </asp:BoundField>
                                                                                                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:t}">
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </asp:BoundField>
                                                                                                                    <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:t}">
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </asp:BoundField>

                                                                                                                    <asp:TemplateField HeaderText="Total (Hr:Min)">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblTotalHours" runat="server" Text=""></asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </asp:TemplateField>

                                                                                                                </Columns>
                                                                                                                <PagerStyle CssClass="pgr" />
                                                                                                                <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                                                                                <RowStyle BackColor="#f1f1f1" />
                                                                                                            </asp:GridView>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td align="left">
                                                                                                            <asp:Button ID="btnPrevious0" runat="server" Text="Previous" CssClass="prevButton" OnClick="btnPrevious_Click" />
                                                                                                        </td>
                                                                                                        <td align="right">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;</td>
                                                                                                        <td align="left">&nbsp;
                                                                                                       <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                                                                                        <td align="right">
                                                                                                            <asp:Button ID="btnNext0" runat="server" Text="Next" CssClass="nextButton" OnClick="btnNext_Click" />

                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>

                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>&nbsp;
                                                                                              
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>

                                                                        </table>

                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12 col-sm-6 col-md-12">
                            <div class="input-group" style="margin-top: 30px;">
                                <asp:TextBox ID="txtSearch" CssClass="form-control form-control-ext" runat="server" TabIndex="2" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                <span class="input-group-btn">
                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default"
                                        TabIndex="3" Text="GO!" OnClick="btnSearch_Click"></asp:LinkButton>&nbsp;
                                          
                                            <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="btn btn-default"
                                                TabIndex="4" Text="New Lead" Visible="false">
                                            </asp:LinkButton>
                                </span>
                                <asp:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" DelimiterCharacters=""
                                    Enabled="True" TargetControlID="txtSearch" ServiceMethod="GetLastName" MinimumPrefixLength="1"
                                    CompletionSetCount="10" EnableCaching="true" CompletionInterval="500" OnClientItemSelected="selected_LastName"
                                    CompletionListCssClass="AutoExtender" UseContextKey="True">
                                </asp:AutoCompleteExtender>
                                <asp:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch"
                                    WatermarkText="Search by Name" />
                            </div>

                        </div>
                    </div>
                    <div class="row">
                        <div class="ol-lg-12  col-sm-12">

                            <table>


                                <tr>

                                    <td>

                                        <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Customer:</span>
                                        <asp:Label ID="lblSearchCustomerName" runat="server" Text="" Font-Bold="true" Font-Size="16px"></asp:Label>

                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="row">
                        <div class="ol-lg-12  col-sm-12">

                            <table>


                                <tr>

                                    <td>

                                        <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Section:</span>


                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="form-horizontal">

                        <div class="form-group">
                            <asp:Panel ID="pnlStartClock" runat="server">
                                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2 ">
                                    <asp:Button ID="btnStartTrip" runat="server" Text="START CLOCK" OnClick="btnStartTrip_Click" />
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                                    Start Time: 
                                    <asp:Label ID="lblStartTime" runat="server" Text=""></asp:Label>
                                </div>
                            </asp:Panel>
                        </div>

                        <div class="form-group" runat="server" id="pnlEndButton" visible="false">
                            <asp:Panel ID="pnlEndClock" runat="server">
                                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                                    <asp:Button ID="btnEndTrip" runat="server" Text="STOP CLOCK" OnClick="btnEndTrip_Click" Width="125px" />
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-3 col-lg-2">
                                    End Time:
                                    <asp:Label ID="lblEndTime" runat="server" Text=""></asp:Label>
                                </div>
                            </asp:Panel>

                        </div>
                        <div class="mapmargin">


                            <div class="row" runat="server" id="timeClockNote" visible="false">
                                <div class="col-lg-12  col-sm-12">

                                    <div class="form-group form-group-ext-txtArea">
                                        <asp:Label ID="Label4" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label" Text="Notes:" Font-Bold="true">
                                            <asp:TextBox
                                                ID="txtNotesDisPlay" runat="server" BackColor="Transparent" CssClass="blindInput"
                                                BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                                Height="16px" ReadOnly="True"></asp:TextBox>

                                        </asp:Label>
                                        <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                            <asp:TextBox ID="txtNotes" runat="server" TabIndex="3" TextMode="MultiLine" CssClass="textBox form-control " Height="200px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtNotesDisPlay'));" MaxLength="1500"></asp:TextBox>

                                        </div>
                                        <div class="col-sm-12 col-md-3 col-lg-9">
                                            <asp:Label ID="lblNote" runat="server" Text=""></asp:Label>
                                        </div>
                                        <div class="col-sm-12 col-md-3 col-lg-9">
                                            <asp:Button ID="btnNoteUpdate" runat="server" Text="Note Update" CssClass=" btn btn-Info" OnClick="btnNoteUpdate_Click" Style="text-align: center; background-color: blue; color: #fff" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <asp:Label ID="lblmarkerStatus" runat="server" Text="Marker Status: Click Marker To Drag" Style="display: none"></asp:Label>
                        <asp:Label ID="lblAddress" runat="server" Text="Current Location: Click Marker To Drag" Style="display: none"></asp:Label>
                        <asp:Label ID="lblInfo" runat="server" Text="Current Position: Click Marker To Drag" Style="display: none"></asp:Label>
                        <asp:Label ID="lblStartLocation" runat="server" Text="" Style="display: none"></asp:Label>
                        <asp:Label ID="lblEndLocation" runat="server" Text="" Style="display: none"></asp:Label>
                        <asp:Label ID="lblTest" runat="server" Text="" Style="font-weight: bold; display: none;"></asp:Label>
                        <asp:Label ID="lblResult" runat="server" Text="" Style="font-size: 14px; font-weight: bold;"></asp:Label>
                        <br />
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                         <asp:HiddenField ID="hdnCustomerEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnGpsId" runat="server" Value="0" />
                    </div>


                </ContentTemplate>

            </asp:UpdatePanel>

            <div id="directionpanel" runat="server" style="margin: 5px 0; display: none;"></div>
            <div id="map" runat="server" style="height: 300px; margin: 0;"></div>
        </div>

        <asp:HiddenField ID="hdnLatitude" runat="server" Value="0" />
        <asp:HiddenField ID="hdnLongitude" runat="server" Value="0" />
        <asp:HiddenField ID="hdnDistance" runat="server" Value="0" />
        <asp:HiddenField ID="hdnTime" runat="server" Value="0" />
        <asp:HiddenField ID="hdnLatLng" runat="server" Value="0" />
        <asp:HiddenField ID="hdnStartLocation" runat="server" Value="0" />
        <asp:HiddenField ID="hdnEndLcation" runat="server" Value="0" />
        <asp:HiddenField ID="hdnDeviceName" runat="server" Value="0" />
    </div>

</asp:Content>


