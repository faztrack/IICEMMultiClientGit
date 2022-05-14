<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="joblocation.aspx.cs" Inherits="joblocation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script>
        var map, mapCenter;

        function jobLocationMap(jsonCustomer) {

            // console.log(jsonCustomer);
            /////////for customer///////////////////////
            var result = JSON.parse(jsonCustomer)
            var locations = [];


            var keys = Object.keys(result);
            keys.forEach(function (key) {
                locations.push(result[key]);
            });
            var len = locations.length;

            if (len > 0) {
                mapCenter = new google.maps.LatLng(locations[0].CustLatitude, locations[0].CustLongitude);
            }


            var geocoder = geocoder = new google.maps.Geocoder();
            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                zoom: 10,
                center: mapCenter,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

            var infowindow = new google.maps.InfoWindow();

            for (var i = 0; i < locations.length; i++) {
               var locAddress = "https://www.google.com/maps/search/?api=1&query=" + locations[i].CustLatitude + "," + locations[i].CustLongitude;
               // var locfdff = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q= " + locations[i].CustomerAddress.toString();
               // alert(locfdff);
                var content = '<div class="map-content"><h5>' + locations[i].CfullName.toString() + '</h5> <a target="_blank" href= ' + locAddress + '>' + locations[i].CustomerAddress.toString() +'</a> <br /></div>';
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

                google.maps.event.addListener(marker, 'click', (function (marker, content) {
                    return function () {
                        // marker.setLabel(locations[i].CustomerAddress.toString());
                        infowindow.setContent(content);
                        infowindow.open(map, marker);
                    }
                })
                    (marker, content)
                )
                google.maps.event.addListener(marker, 'ontouchstart', (function (marker, content) {
                    return function () {
                        // marker.setLabel(locations[i].CustomerAddress.toString());
                        infowindow.setContent(content);
                        infowindow.open(map, marker);
                    }
                })
                    (marker, content)
                )
              
            };



        }

        function DefaultCustomerinitializeMap(DefaultrCustomerLat, DefaultrCustomerlong) {

            var geocoder = geocoder = new google.maps.Geocoder();

            var latlng = new google.maps.LatLng(DefaultrCustomerLat, DefaultrCustomerlong);

            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                       zoom: 11,
                       center: latlng,
                       mapTypeId: google.maps.MapTypeId.ROADMAP
                   });

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

        @media (min-width: 1025px) and (max-width: 1280px) {

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
                <strong>Job Locations

                </strong>

            </h3>

        </div>
        <div class="panel-body" style="padding-bottom: 0px; margin-bottom: 50px;">
          

            <div id="directionpanel" runat="server" style="margin: 5px 0; display: none;"></div>
            <div id="map" runat="server" style="height: 500px; margin: 0;"></div>
        </div>

      
    </div>

</asp:Content>


