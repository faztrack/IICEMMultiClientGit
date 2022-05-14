<%@ Page Language="C#" AutoEventWireup="true" CodeFile="gCusCrewTrackermap.aspx.cs" MasterPageFile="~/Main.master" Inherits="gCusCrewTrackermap" Title="Crew Location Tracker" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAg9x2CEaBTyFmwXm75gvfmQVuOGcSND0Y"
        type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(<%=lstCrew.ClientID%>).SumoSelect({
                selectAll: true,
                search: true,
                searchText: 'Search...',
                // triggerChangeCombined: true,
                //locale: ['OK', 'Cancel', 'Select All'],
                placeholder: 'Select Here',
                noMatch: 'No matches for "{0}"',
                //okCancelInMulti: true,
            });
            $(<%=lstJobName.ClientID%>).SumoSelect({
                selectAll: true,
                search: true,
                searchText: 'Search...',
                // triggerChangeCombined: true,
                //locale: ['OK', 'Cancel', 'Select All'],
                placeholder: 'Select Here',
                noMatch: 'No matches for "{0}"',
                //okCancelInMulti: true,
            });

        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function EndRequestHandler(sender, args) {
            //Binding Code Again
            $(<%=lstCrew.ClientID%>).SumoSelect({
                selectAll: true,
                search: true,
                searchText: 'Search...',
                //triggerChangeCombined: true,
                //locale: ['OK', 'Cancel', 'Select All'],
                placeholder: 'Select Here',
                noMatch: 'No matches for "{0}"',
                //okCancelInMulti: true,
            });
            $(<%=lstJobName.ClientID%>).SumoSelect({
                selectAll: true,
                search: true,
                searchText: 'Search...',
                // triggerChangeCombined: true,
                //locale: ['OK', 'Cancel', 'Select All'],
                placeholder: 'Select Here',
                noMatch: 'No matches for "{0}"',
                //okCancelInMulti: true,
            });
        }
    </script>
    <script>
        var map, CrewCoordList, Customerinfo, mapCenter;


        function CustomerCrewTrackerMap(CrewCoordList, Customerinfo) {
            try {
                var CUstomerJsonPArse = JSON.parse(Customerinfo)
                var result = JSON.parse(CrewCoordList)
                var CrewCoordList = [];
                var keys = Object.keys(result);
                keys.forEach(function (key) {
                    CrewCoordList.push(result[key]);
                });
            }
            catch (err) {

            }
            //alert(CrewCoordList[1].StartLatitude);
            mapCenter = new google.maps.LatLng(33.4122784, -112.0836409);//usa
            //mapCenter = new google.maps.LatLng(23.6850, 90.3563);//bangladesh
            var geocoder = geocoder = new google.maps.Geocoder();
            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                zoom: 10,

                center: mapCenter,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

            var infowindow = new google.maps.InfoWindow();
            var crwStratTiemcount = 1;
            var crwEndTiemcount = 1;

            //alert( CrewCoordList[1].CrewfullName);
            for (var i = 0; i < CrewCoordList.length; i++) {
                //var date = new Date(CrewCoordList[i].labor_date).toDateString("yyyy-MM-dd");


                function formatDate(date) {
                    var d = new Date(date),
                        month = '' + (d.getMonth() + 1),
                        day = '' + d.getDate(),
                        year = d.getFullYear();

                    if (month.length < 2)
                        month = '0' + month;
                    if (day.length < 2)
                        day = '0' + day;

                    //return [year, month, day].join('/');
                    return [month, day, year].join('/');
                }

                var contentColockIn = '<div class="map-content"><h3>' + CrewCoordList[i].CrewfullName.toString() + ', Clock In: ' + CrewCoordList[i].StartTime.toString() + ' ' + formatDate(CrewCoordList[i].labor_date) + '<br /><h3> Address: ' + CrewCoordList[i].StartPlace + '</h3></div>';
                //var contentClockOut = '<div class="map-content"><h3>' + CrewCoordList[i].CrewfullName.toString() + ', Clock Out : ' + CrewCoordList[i].StartCount.toString() + ', ' + CrewCoordList[i].EndTime + '<br /></div>';
                var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(CrewCoordList[i].StartLatitude, CrewCoordList[i].StartLogitude),
                    icon: {
                        labelOrigin: new google.maps.Point(16, 40),
                        url: "https://ii.faztrack.com/images/pin_customer.png"
                    },
                    draggable: true,
                    map: map,
                    animation: google.maps.Animation.DROP,


                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, contentColockIn) {
                    return function () {

                        infowindow.setContent(contentColockIn);
                        infowindow.open(map, marker);
                        ++crwStratTiemcount;
                    }
                })
                    (marker, contentColockIn)

                )
                google.maps.event.addListener(marker, 'mouseout', (function (marker, contentColockIn) {
                    return function () {
                        infowindow.close();
                    }
                })
                    (marker, contentColockIn)

                )
                //////////////////////////////////////////////////////////////////////////////////////////
                var contentColockIn1 = '<div class="map-content"><h3>' + CrewCoordList[i].CrewfullName.toString() + ', Clock Out: ' + CrewCoordList[i].EndTime.toString() + ' ' + formatDate(CrewCoordList[i].labor_date) + '<br /><h3> Address: ' + CrewCoordList[i].EndPlace + '</h3></div>';
                //var contentClockOut = '<div class="map-content"><h3>' + CrewCoordList[i].CrewfullName.toString() + ', Clock Out : ' + CrewCoordList[i].StartCount.toString() + ', ' + CrewCoordList[i].EndTime + '<br /></div>';
                var marker = new google.maps.Marker({
                    position: new google.maps.LatLng(CrewCoordList[i].EndLatitude, CrewCoordList[i].EndLogitude),
                    icon: {
                        labelOrigin: new google.maps.Point(16, 40),
                        url: "https://ii.faztrack.com/images/pin_superintendent.png"
                    },
                    draggable: true,
                    map: map,
                    animation: google.maps.Animation.DROP,


                });

                google.maps.event.addListener(marker, 'mouseover', (function (marker, contentColockIn1) {
                    return function () {

                        infowindow.setContent(contentColockIn1);
                        infowindow.open(map, marker);
                        ++crwStratTiemcount;
                    }
                })
                    (marker, contentColockIn1)

                )
                google.maps.event.addListener(marker, 'mouseout', (function (marker, contentColockIn1) {
                    return function () {
                        infowindow.close();
                    }
                })
                    (marker, contentColockIn1)

                )
            }




        }







        function DefaultrCustomerCrewTrackerMap(DefaultrCustomerLat, DefaultrCustomerlong) {

            var geocoder = geocoder = new google.maps.Geocoder();

            var latlng = new google.maps.LatLng(DefaultrCustomerLat, DefaultrCustomerlong);

            map = new google.maps.Map(document.getElementById("<%=map.ClientID%>"), {
                zoom: 7,
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
                                <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Crew Location Tracker</asp:Label></span>
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

                                    <td align="right"><strong>Crew/Employee Name:</strong></td>
                                    <td align="left">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="left">

                                                    <asp:ListBox ID="lstCrew" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                </td>
                                                <%-- <td>
                                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="button" Text="Search" OnClick="btnSearch_Click"></asp:LinkButton>
                                                </td>--%>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="right"><strong>Job Name:</strong></td>
                                    <td align="left">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="left">

                                                    <asp:ListBox ID="lstJobName" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                </td>
                                                <td>
                                                    <asp:LinkButton ID="btnJobList" runat="server" style="padding:3px 10px 3px 10px;" CssClass="button" Text="Search" OnClick="btnSearch_Click"></asp:LinkButton>
                                                </td>

                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="right"><strong>Date Range:</strong></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="textBox" Width="70px" TabIndex="1"></asp:TextBox>
                                                    <cc1:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate" PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                    </cc1:CalendarExtender>
                                                    <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtStartDate" WatermarkText="Start Date" />
                                                </td>
                                                <td align="left">
                                                    <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                </td>
                                                <td>&nbsp;&nbsp;&nbsp;</td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="textBox" Width="70px" TabIndex="2"></asp:TextBox>
                                                    <cc1:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEndDate" PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                    </cc1:CalendarExtender>
                                                    <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtEndDate" WatermarkText="End Date" />
                                                </td>
                                                <td align="right">
                                                    <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                </td>
                                                <td>&nbsp;&nbsp;&nbsp;</td>
                                                <td>
                                                    <asp:LinkButton ID="btnView" runat="server" CssClass="underlineButton" Text="View" OnClick="btnView_Click"></asp:LinkButton>
                                                </td>
                                                <td>&nbsp;&nbsp;&nbsp;</td>
                                                <td align="left">
                                                    <b style="color: #000080">Jobs: </b>
                                                    <asp:Label ID="lblView" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                    <b style="color: #028A0F">Active Crew: </b>
                                                    <asp:Label ID="lblActiveCrew" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>

                                                </td>
                                                <%-- <td>
                                                    
                                                    <asp:DropDownList ID="ddlStartTime" runat="server" OnSelectedIndexChanged="ddlStartTime_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem value="0" Selected="True">Start Time</asp:ListItem>
                                                        <asp:ListItem value="4:00 AM">4:00 AM</asp:ListItem>
                                                        <asp:ListItem value="4:30 AM">4:30 AM</asp:ListItem>
                                                        <asp:ListItem value="5:00 AM">5:00 AM</asp:ListItem>
                                                        <asp:ListItem value="5:30 AM">5:30 AM</asp:ListItem>
                                                        <asp:ListItem value="6:00 AM">6:00 AM</asp:ListItem>
                                                        <asp:ListItem value="6:30 AM">6:30 AM</asp:ListItem>
                                                        <asp:ListItem value="7:00 AM">7:00 AM</asp:ListItem>
                                                        <asp:ListItem value="7:30 AM">7:30 AM</asp:ListItem>
                                                        <asp:ListItem value="8:00 AM">8:00 AM</asp:ListItem>
                                                        <asp:ListItem value="8:30 AM">8:30 AM</asp:ListItem>
                                                        <asp:ListItem value="9:00 AM">9:00 AM</asp:ListItem>
                                                        <asp:ListItem value="9:30 AM">9:30 AM</asp:ListItem>
                                                        <asp:ListItem value="10:00 AM">10:00 AM</asp:ListItem>
                                                        <asp:ListItem value="10:30 AM">10:30 AM</asp:ListItem>
                                                        <asp:ListItem value="11:00 AM">11:00 AM</asp:ListItem>
                                                        <asp:ListItem value="11:30 AM">11:30 AM</asp:ListItem>
                                                        <asp:ListItem value="12:00 PM">12:00 PM</asp:ListItem>
                                                        <asp:ListItem value="12:30 PM">12:30 PM</asp:ListItem>
                                                        <asp:ListItem value="1:00 PM">1:00 PM</asp:ListItem>
                                                        <asp:ListItem value="1:30 PM">1:30 PM</asp:ListItem>
                                                        <asp:ListItem value="2:00 PM">2:00 PM</asp:ListItem>
                                                        <asp:ListItem value="2:30 PM">2:30 PM</asp:ListItem>
                                                        <asp:ListItem value="3:00 PM">3:00 PM</asp:ListItem>
                                                        <asp:ListItem value="3:30 PM">3:30 PM</asp:ListItem>
                                                        <asp:ListItem value="4:00 PM">4:00 PM</asp:ListItem>
                                                        <asp:ListItem value="4:30 PM">4:30 PM</asp:ListItem>
                                                        <asp:ListItem value="5:00 PM">5:00 PM</asp:ListItem>
                                                        <asp:ListItem value="5:30 PM">5:30 PM</asp:ListItem>
                                                        <asp:ListItem value="6:00 PM">6:00 PM</asp:ListItem>
                                                        <asp:ListItem value="6:30 PM">6:30 PM</asp:ListItem>
                                                        <asp:ListItem value="7:00 PM">7:00 PM</asp:ListItem>
                                                        <asp:ListItem value="7:30 PM">7:30 PM</asp:ListItem>
                                                        <asp:ListItem value="8:00 PM">8:00 PM</asp:ListItem>
                                                        <asp:ListItem value="8:30 PM">8:30 PM</asp:ListItem>
                                                        <asp:ListItem value="9:00 PM">9:00 PM</asp:ListItem>
                                                        <asp:ListItem value="9:30 PM">9:30 PM</asp:ListItem>
                                                        <asp:ListItem value="10:00 PM">10:00 PM</asp:ListItem>
                                                        <asp:ListItem value="10:30 PM">10:30 PM</asp:ListItem>
                                                        <asp:ListItem value="11:00 PM">11:00 PM</asp:ListItem>
                                                        <asp:ListItem value="11:30 PM">11:30 PM</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                 <td>
                                                    
                                                    <asp:DropDownList ID="ddlEndTime" runat="server" OnSelectedIndexChanged="ddlEndTime_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem value="0" Selected="True">End Time</asp:ListItem>
                                                        <asp:ListItem value="4:00 AM">4:00 AM</asp:ListItem>
                                                        <asp:ListItem value="4:30 AM">4:30 AM</asp:ListItem>
                                                        <asp:ListItem value="5:00 AM">5:00 AM</asp:ListItem>
                                                        <asp:ListItem value="5:30 AM">5:30 AM</asp:ListItem>
                                                        <asp:ListItem value="6:00 AM">6:00 AM</asp:ListItem>
                                                        <asp:ListItem value="6:30 AM">6:30 AM</asp:ListItem>
                                                        <asp:ListItem value="7:00 AM">7:00 AM</asp:ListItem>
                                                        <asp:ListItem value="7:30 AM">7:30 AM</asp:ListItem>
                                                        <asp:ListItem value="8:00 AM">8:00 AM</asp:ListItem>
                                                        <asp:ListItem value="8:30 AM">8:30 AM</asp:ListItem>
                                                        <asp:ListItem value="9:00 AM">9:00 AM</asp:ListItem>
                                                        <asp:ListItem value="9:30 AM">9:30 AM</asp:ListItem>
                                                        <asp:ListItem value="10:00 AM">10:00 AM</asp:ListItem>
                                                        <asp:ListItem value="10:30 AM">10:30 AM</asp:ListItem>
                                                        <asp:ListItem value="11:00 AM">11:00 AM</asp:ListItem>
                                                        <asp:ListItem value="11:30 AM">11:30 AM</asp:ListItem>
                                                        <asp:ListItem value="12:00 PM">12:00 PM</asp:ListItem>
                                                        <asp:ListItem value="12:30 PM">12:30 PM</asp:ListItem>
                                                        <asp:ListItem value="1:00 PM">1:00 PM</asp:ListItem>
                                                        <asp:ListItem value="1:30 PM">1:30 PM</asp:ListItem>
                                                        <asp:ListItem value="2:00 PM">2:00 PM</asp:ListItem>
                                                        <asp:ListItem value="2:30 PM">2:30 PM</asp:ListItem>
                                                        <asp:ListItem value="3:00 PM">3:00 PM</asp:ListItem>
                                                        <asp:ListItem value="3:30 PM">3:30 PM</asp:ListItem>
                                                        <asp:ListItem value="4:00 PM">4:00 PM</asp:ListItem>
                                                        <asp:ListItem value="4:30 PM">4:30 PM</asp:ListItem>
                                                        <asp:ListItem value="5:00 PM">5:00 PM</asp:ListItem>
                                                        <asp:ListItem value="5:30 PM">5:30 PM</asp:ListItem>
                                                        <asp:ListItem value="6:00 PM">6:00 PM</asp:ListItem>
                                                        <asp:ListItem value="6:30 PM">6:30 PM</asp:ListItem>
                                                        <asp:ListItem value="7:00 PM">7:00 PM</asp:ListItem>
                                                        <asp:ListItem value="7:30 PM">7:30 PM</asp:ListItem>
                                                        <asp:ListItem value="8:00 PM">8:00 PM</asp:ListItem>
                                                        <asp:ListItem value="8:30 PM">8:30 PM</asp:ListItem>
                                                        <asp:ListItem value="9:00 PM">9:00 PM</asp:ListItem>
                                                        <asp:ListItem value="9:30 PM">9:30 PM</asp:ListItem>
                                                        <asp:ListItem value="10:00 PM">10:00 PM</asp:ListItem>
                                                        <asp:ListItem value="10:30 PM">10:30 PM</asp:ListItem>
                                                        <asp:ListItem value="11:00 PM">11:00 PM</asp:ListItem>
                                                        <asp:ListItem value="11:30 PM">11:30 PM</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>--%>
                                                <td>&nbsp;&nbsp;&nbsp;</td>
                                                <td>
                                                    <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="underlineButton" Text="Reset" OnClick="lnkViewAll_Click"></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>


                                </tr>
                                <tr>
                                     <td align="right">&nbsp;</td>
                                    <td align="left">
                                        <asp:RadioButtonList ID="radEmployeeType" runat="server" OnSelectedIndexChanged="radEmployeeType_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="3" Selected="True">All</asp:ListItem>
                                            <asp:ListItem Value="1">Crews</asp:ListItem>
                                            <asp:ListItem Value="0">Employees</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="Center" colspan="3">
                                        <asp:Label ID="lblResult" runat="server"></asp:Label>

                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right">
                            <table style="padding: 0px; margin: 0px;">
                                <tr>
                                    <td style="padding: 0px; margin: 0px; vertical-align: middle;">
                                        <img src="images/pin_customer.png"/>Clock In</td>
                                    <td>&nbsp;</td>
                                    <td style="padding: 0px; margin: 0px; vertical-align: middle;">
                                        <img src="images/pin_superintendent.png" />Clock Out</td>

                                </tr>
                            </table>
                        </td>


                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div id="map" style="height: 800px; margin: 0;" runat="server"></div>


                <asp:HiddenField ID="hdnLatLng" runat="server" Value="0" />
                <div id="directionpanel" runat="server" style="margin: 5px 0;"></div>
            </td>
        </tr>
    </table>



</asp:Content>
