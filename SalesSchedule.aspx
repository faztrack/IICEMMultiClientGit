<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SalesSchedule.aspx.cs" Inherits="SalesSchedule" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table bordercolor="silver" border="1">
        <!--tr vAlign="top">
					<td vAlign="top" colSpan="2" height="140"></td>
				</tr-->
        <!--content start-->
        <tr valign="top">
            <td>
                <table cellpadding="4px" cellspacing="4px">
                    <tr>
                        <td>
                            <table bordercolor="silver" border="1">

                                <tr>
                                    <td>
                                        <br>
                                        <table cellpadding="4px" cellspacing="4px">
                                            <tr>
                                                <td width="108">
                                                    <asp:Label ID="Label2" Width="112px" runat="server">Appointment Type:</asp:Label></td>
                                                <td valign="top" align="left" height="17">
                                                    <asp:DropDownList ID="ddlAppointmentType" Width="184px" runat="server"
                                                        Height="24px" Enabled="False">
                                                        <asp:ListItem Value="1">Sales</asp:ListItem>
                                                        <asp:ListItem Value="2">Measurement</asp:ListItem>
                                                        <asp:ListItem Value="3">Installation</asp:ListItem>
                                                        <asp:ListItem Value="4">Service</asp:ListItem>
                                                        <asp:ListItem Value="5">Other</asp:ListItem>
                                                    </asp:DropDownList><asp:Label ID="Label6" runat="server"> LeadID
                                                    </asp:Label>&nbsp;<asp:TextBox ID="txtKey" runat="server" Width="48px" Enabled="False">0</asp:TextBox>&nbsp;<asp:CheckBox ID="chkReserveOnly" runat="server" Width="264px" AutoPostBack="True"
                                                        Text="Reserve-Only Appointment for Emp/Rep."></asp:CheckBox></td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="Label7" runat="server">Customer Info:</asp:Label></td>
                                                <td valign="middle" align="left" width="150" height="117">
                                                    <asp:Label ID="lblCustomer" runat="server" Width="554px" Height="100px"
                                                        BorderColor="Silver" BorderStyle="Solid" BorderWidth="1pt"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td style="" align="right">
                                                    <asp:Label ID="Label10" Width="80px" runat="server">Notes:</asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtNotes" runat="server" Width="552px" Height="61px"
                                                        BorderColor="Silver" TextMode="MultiLine" Columns="20"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2" rowspan="1">
                                                    <asp:RadioButtonList ID="rbnApplicance" runat="server" RepeatDirection="Horizontal"
                                                        CellPadding="0" CellSpacing="0">
                                                        <asp:ListItem Value="0" Selected="True">None</asp:ListItem>
                                                        <asp:ListItem Value="1">Confirmed</asp:ListItem>
                                                        <asp:ListItem Value="2">Reminder</asp:ListItem>
                                                    </asp:RadioButtonList></td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" align="center" colspan="2" rowspan="1">
                                                    <asp:CheckBoxList ID="chkDay" TabIndex="33" runat="server" CssClass="SmallTitleBlackBold10" RepeatDirection="Horizontal"
                                                        CellPadding="0" CellSpacing="0" Visible="False">
                                                        <asp:ListItem Value="0">Sunday</asp:ListItem>
                                                        <asp:ListItem Value="1">Monday</asp:ListItem>
                                                        <asp:ListItem Value="2">Tuesday</asp:ListItem>
                                                        <asp:ListItem Value="3">Wednesday</asp:ListItem>
                                                        <asp:ListItem Value="4">Thursday</asp:ListItem>
                                                        <asp:ListItem Value="5">Friday</asp:ListItem>
                                                        <asp:ListItem Value="6">Saturday</asp:ListItem>
                                                    </asp:CheckBoxList></td>
                                            </tr>
                                            <tr>
                                                <td valign="middle" align="center" colspan="2">Month:
															<asp:DropDownList ID="ddlMonth" runat="server">
                                                                <asp:ListItem Value="0">Select</asp:ListItem>
                                                                <asp:ListItem Value="1">January</asp:ListItem>
                                                                <asp:ListItem Value="2">February</asp:ListItem>
                                                                <asp:ListItem Value="3">March</asp:ListItem>
                                                                <asp:ListItem Value="0">April</asp:ListItem>
                                                                <asp:ListItem Value="5">May</asp:ListItem>
                                                                <asp:ListItem Value="6">June</asp:ListItem>
                                                                <asp:ListItem Value="7">July</asp:ListItem>
                                                                <asp:ListItem Value="8">August</asp:ListItem>
                                                                <asp:ListItem Value="9">September</asp:ListItem>
                                                                <asp:ListItem Value="10">October</asp:ListItem>
                                                                <asp:ListItem Value="11">November</asp:ListItem>
                                                                <asp:ListItem Value="12">December</asp:ListItem>
                                                            </asp:DropDownList>Year&nbsp;:
															<asp:DropDownList ID="ddlYear" runat="server">
                                                                <asp:ListItem Value="2008">2008</asp:ListItem>
                                                                <asp:ListItem Value="2009">2009</asp:ListItem>
                                                                <asp:ListItem Value="2010">2010</asp:ListItem>
                                                                <asp:ListItem Value="2011">2011</asp:ListItem>
                                                                <asp:ListItem Value="2012" Selected="True">2012</asp:ListItem>
                                                                <asp:ListItem Value="2013">2013</asp:ListItem>
                                                                <asp:ListItem Value="2014">2014</asp:ListItem>
                                                                <asp:ListItem Value="2015">2015</asp:ListItem>
                                                                <asp:ListItem Value="2016">2016</asp:ListItem>
                                                                <asp:ListItem Value="2017">2017</asp:ListItem>
                                                                <asp:ListItem Value="2018">2018</asp:ListItem>
                                                                <asp:ListItem Value="2019">2019</asp:ListItem>
                                                                <asp:ListItem Value="2020">2020</asp:ListItem>
                                                            </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td style="WIDTH: 108px; HEIGHT: 34px">
                                                    <asp:Label ID="Label3" runat="server">Start Time:</asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtStartDate" Width="96px" runat="server"></asp:TextBox>
                                                    <asp:TextBox ID="txtStartTime" Width="50px" runat="server" MaxLength="8">08:00</asp:TextBox>
                                                    <asp:DropDownList ID="ddlStartAMPM" Width="50px" runat="server"
                                                        EnableViewState="False">
                                                        <asp:ListItem Value="AM">AM</asp:ListItem>
                                                        <asp:ListItem Value="PM">PM</asp:ListItem>
                                                    </asp:DropDownList>&nbsp;&nbsp;
															<asp:Button ID="btnFind" Width="128px" CssClass="ControlButtonStyle" runat="server" Text="Goto Calendar"
                                                                Font-Name="verdana"></asp:Button>&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="WIDTH: 108px; HEIGHT: 32px">
                                                    <asp:Label ID="Label4" runat="server">End Time:</asp:Label></td>
                                                <td>
                                                    <asp:TextBox ID="txtEndDate" Width="96px" runat="server"></asp:TextBox>
                                                    <asp:TextBox ID="txtEndTime" Width="50px" runat="server" MaxLength="8">08:00</asp:TextBox>
                                                    <asp:DropDownList ID="ddlEndAMPM" runat="server" EnableViewState="False"
                                                        Width="50px">
                                                        <asp:ListItem Value="AM">AM</asp:ListItem>
                                                        <asp:ListItem Value="PM">PM</asp:ListItem>
                                                    </asp:DropDownList>&nbsp;&nbsp;&nbsp;
															<asp:TextBox ID="txtEmailTime" runat="server" Width="96px"></asp:TextBox>
                                                    <asp:Label ID="Label8" runat="server">Actual Appt.Time</asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td style="WIDTH: 108px; HEIGHT: 34px">
                                                    <asp:Label ID="Label5" runat="server">Employee/Rep:</asp:Label></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlSalesRep" Width="184px" runat="server"></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblResult" runat="server"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblEmailMessage" runat="server"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Button ID="btnAssignRep" CssClass="button"
                                                        runat="server" Text="Save Appointment"
                                                        Font-Name="verdana"></asp:Button>&nbsp;
															<asp:Button ID="btnMail" runat="server" Text="Save &amp; Send Email"
                                                                Font-Name="verdana" CssClass="button"></asp:Button>&nbsp;<asp:Button ID="btnLeadCard" CssClass="button" runat="server" Text="Lead Card"
                                                                    Font-Name="verdana"></asp:Button>&nbsp;<asp:Button ID="btnDelete" CssClass="button" runat="server" Text="Delete Schedule"
                                                                        Visible="False" Font-Name="verdana"></asp:Button>&nbsp;<asp:Button ID="btnClose" CssClass="button" runat="server" Text="Close"
                                                                            Font-Name="verdana"></asp:Button></td>
                                            </tr>
                                            <tr>
                                                <td align="left" colspan="2"></td>
                                            </tr>
                                        </table>
                                        <input id="hdnApptId" type="hidden" value="0" runat="server">
                                        <asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label>
                                        <asp:Label ID="lblCustFirstName" runat="server" Visible="False"></asp:Label></td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

