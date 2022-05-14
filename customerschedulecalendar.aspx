<%@ Page Title="" Language="C#" MasterPageFile="~/CustomerScheduleMaster.master" AutoEventWireup="true" CodeFile="customerschedulecalendar.aspx.cs" Inherits="customerschedulecalendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <div>


        <table cellpadding="0" cellspacing="2" width="100%" align="center">
            <tr>
                <td style="padding: 0px; margin: 0px; width: 18%;" valign="top"></td>
                <td valign="top" align="center" style="padding: 0px; margin: 0px; width: 60%;">
                    <h1>
                        <asp:Label ID="lbltopHead" runat="server" Text="Your Project Calendar"></asp:Label>
                    </h1>
                </td>
                <td></td>
            </tr>
            <tr>
                <td style="padding: 0px; margin: 0px; width: 18%;" valign="top"></td>
                <td valign="top" align="center" style="padding: 0px; margin: 0px; width: 60%;">
                    <p class="noteYellow">The calendar is subject to change at any time. Flashing items are yet to be scheduled.</p>
                </td>
                <td></td>
            </tr>
            <tr>
                <td style="padding: 0px; margin: 0px; width: 18%;" valign="top">
                    <div style="margin-left: 50px;">
                        <font style="font-weight: bold;">Projects:</font>
                        <asp:CheckBoxList ID="chkEst" runat="server" AutoPostBack="True" OnSelectedIndexChanged="chkEst_SelectedIndexChanged"></asp:CheckBoxList>
                    </div>
                </td>
                <td valign="top" align="center" style="padding: 0px; margin: 0px; width: 60%;">
                    <div id="calendar">
                    </div>
                </td>
                <td></td>
            </tr>
        </table>
        <div runat="server" id="jsonDiv" />
        <%--<input type="hidden" id="hdClient" runat="server" />--%>

        <asp:HiddenField ID="hdnEstimateID" runat="server" />
        <asp:HiddenField ID="hdnCustomerID" runat="server" />
        <asp:HiddenField ID="hdnEmployeeID" runat="server" />
        <asp:HiddenField ID="hdnTypeID" runat="server" />
        <asp:HiddenField ID="hdnEventStartDate" runat="server" />
        <asp:HiddenField ID="hdnServiceCssClass" runat="server" Value="fc-default" />
        <asp:HiddenField ID="hdnEstIDSelected" runat="server" Value="0" />
        <asp:HiddenField ID="hdnCustIDSelected" runat="server" Value="0" />
        <asp:HiddenField ID="hdnAutoCustID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnCustCalWeeklyView" runat="server" Value="0" />
        <asp:HiddenField ID="StartofJob" runat="server" Value="" />
        <br />
        <br />
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
</asp:Content>

