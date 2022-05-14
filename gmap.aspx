<%@ Page Language="C#" AutoEventWireup="true" CodeFile="gmap.aspx.cs" MasterPageFile="~/Main.master" Inherits="gmap" Title="map" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAg9x2CEaBTyFmwXm75gvfmQVuOGcSND0Y"
        type="text/javascript"></script>

    <script>

        var map, CustomerDetailesWitheCoord, CreweCoord, mapCenter;

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
            var  Superlocations = [];


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
                zoom: 10,

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
                    //Title: locations[i].CustomerAddress.toString(),

                    animation: google.maps.Animation.DROP,
                    //label: {
                    //    color: '#3a009e',
                    //    fontWeight: 'normal',
                    //    text: locations[i].CfullName.toString(),

                    //},



                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, content) {
                    return function () {
                        // marker.setLabel(locations[i].CustomerAddress.toString());
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
                    // Title: locations[i].CustomerAddress.toString(),

                    animation: google.maps.Animation.DROP,
                    //label: {
                    //    color: '#7d2a01',
                    //    fontWeight: 'normal',
                    //    text: Crewlocations[i].CfullName.toString(),
                    //},
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

            //////////////////////////////Super////////////////////////////////////////////
            for (var i = 0; i < Superlocations.length; i++) {
                //var newLat = Superlocations[i].SuperintendentLatitude + .00004 * Math.cos((+a * i) / 180 * Math.PI);  // x
                //var newLng = Superlocations[i].SuperintendentLongitude + .00004 * Math.sin((+a * i) / 180 * Math.PI);  // Y

                var Supercontent = '<div class="map-content"><h3>' + Superlocations[i].superName.toString() + '</h3>' + Superlocations[i].CfullName.toString() + '<br /></div>';
                var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(Superlocations[i].SuperintendentLatitude, Superlocations[i].SuperintendentLongitude),
                    icon: {
                        labelOrigin: new google.maps.Point(16, 40),
                        size: new google.maps.Size(43, 43),
                        url: "https://faztimate.holtzmanhomeimprovement.com/images/pin_superintendent.png"
                    },
                    draggable: false,
                    map: map,
                   // zIndex: 2,
                    animation: google.maps.Animation.DROP,
                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, Supercontent) {
                    return function () {
                        infowindow.setContent(Supercontent);
                        infowindow.open(map, marker);
                    }
                })
                    (marker, Supercontent)

                )
                google.maps.event.addListener(marker, 'mouseout', (function (marker, Supercontent) {
                    return function () {
                        infowindow.close();
                    }
                })
                    (marker, Supercontent)

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


    <style>
        body {
            background: white;
        }

        .labels {
            color: white;
            background-color: red;
            font-family: "Lucida Grande", "Arial", sans-serif;
            font-size: 10px;
            text-align: center;
            width: 100px;
            white-space: nowrap;
        }

        .panel {
            margin-left: 50px;
            margin-right: 50px;
        }
    </style>

    <table cellpadding="0" cellspacing="0" width="100%">

        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left">
                            <span class="titleNu">
                                <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Customer and Crew Location</asp:Label></span>
                        </td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>


        </tr>
        <tr>
            <td>


                <table style="padding: 4px; margin: 0px; width: 100%;" cellpadding="4px" cellspacing="4px">
                    <tr>
                        <td align="left">
                            <table style="padding: 0px; margin: 0px;">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblScheduleDate" runat="server" Font-Bold="true">Event Date: </asp:Label><asp:TextBox ID="txtScheduleDate" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton CssClass="nostyleCalImg" ID="imgScheduleDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                        <cc1:CalendarExtender ID="CalExtScheduleDate" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgScheduleDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtScheduleDate">
                                        </cc1:CalendarExtender>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnView" runat="server" Text="View" CssClass="button" OnClick="btnView_Click" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td align="left">

                                        <asp:Button ID="btnPrevious" runat="server" Text="<< Previous" CssClass="button" OnClick="btnPrevious_Click" />
                                        <asp:Button ID="btnToday" runat="server" Text="Today" CssClass="button" OnClick="btnToday_Click" />
                                        <asp:Button ID="btnNext" runat="server" Text="Next >>" CssClass="button" OnClick="btnNext_Click" />
                                        <asp:CheckBox ID="chkCustomer" runat="server" Text="Customer" Checked="true" OnCheckedChanged="chkCustomer_CheckedChanged" AutoPostBack="true" />
                                        <asp:CheckBox ID="chkCrew" runat="server" Text="Crew" Checked="true" OnCheckedChanged="chkCrew_CheckedChanged" AutoPostBack="true" />
                                         <asp:CheckBox ID="chkSuper" runat="server" Text="Superintendent" Checked="true" OnCheckedChanged="chkSuperintendent_CheckedChanged" AutoPostBack="true" />
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblResult" runat="server"></asp:Label></td>
                                </tr>
                            </table>
                        </td>

                        <td align="right">
                            <table style="padding: 0px; margin: 0px;">
                                <tr>
                                     <td style="padding: 0px; margin: 0px; vertical-align: middle;">
                                        <img src="images/pin_superintendent.png" />Superintendent</td>
                                    <td>&nbsp;</td>
                                    <td style="padding: 0px; margin: 0px; vertical-align: middle;">
                                        <img src="images/pin_customer.png" />Customer</td>
                                    <td>&nbsp;</td>
                                    <td style="padding: 0px; margin: 0px; vertical-align: middle;">
                                        <img src="images/pin_crew.png" />Crew</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div id="map" style="height: 600px; margin: 0;" runat="server"></div>


                <asp:HiddenField ID="hdnLatLng" runat="server" Value="0" />
                <div id="directionpanel" runat="server" style="margin: 5px 0;"></div>
            </td>
        </tr>
    </table>



</asp:Content>
