<%@ Page Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true"
    CodeFile="mcrewschedulecalendar.aspx.cs" Inherits="mcrewschedulecalendar" Title="Schedule Calendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="jquerycalendar/jquery.qtip.min.css" rel="stylesheet" />

    <link href="fullcalendar/fullcalendar.css" rel="stylesheet" type="text/css" />


    <script src="jquerycalendar/moment.min.js" type="text/javascript"></script>






    <script src="jquerycalendar/jquery.qtip.min.js" type="text/javascript"></script>

    <script src="fullcalendar/fullcalendar.js" type="text/javascript"></script>

    <script src="js/crewcalendarscript.js" type="text/javascript"></script>

    <script src="commonScript.js" type="text/javascript"></script>
    <style type='text/css'>
        body {
            font-size: 14px;
            font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif;
        }
         .fc-Link {
            display: none !important;
        }
    </style>

    <div class="panel panel-default">
        <div class="panel-heading panel-heading-ext">

            <h3 class="panel-title">
                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                <strong>Project Calendar
                </strong>

            </h3>

        </div>
        <div class="panel-body" style="padding-bottom: 0px; margin-bottom: 50px;">

            <div id="calendar">
            </div>
        </div>
    </div>


    <div id="loading">
         <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>


    <div runat="server" id="jsonDiv" />
    <%--<input type="hidden" id="hdClient" runat="server" />--%>

    <asp:HiddenField ID="hdnAddEventName" runat="server" Value="" />
    <asp:HiddenField ID="hdnEventDesc" runat="server" Value="" />
    <asp:HiddenField ID="hdnEditeventName" runat="server" Value="" />
    <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnEmployeeID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnTypeID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnEventStartDate" runat="server" Value="" />
    <asp:HiddenField ID="hdnServiceCssClass" runat="server" Value="fc-default" />
    <asp:HiddenField ID="hdnEstIDSelected" runat="server" Value="0" />
    <asp:HiddenField ID="hdnCustIDSelected" runat="server" Value="0" />
    <asp:HiddenField ID="hdnAutoCustID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnEventId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnUpdateDialogShow" runat="server" Value="false" />
    <asp:HiddenField ID="hdnSelectedMonthView" runat="server" Value="false" />

    <br />
    <br />

</asp:Content>

