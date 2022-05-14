<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="sales_statistics.aspx.cs" Inherits="sales_statistics" Title="Sales Statistics" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Sales Statistics</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <b>Sales Person: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSalesPersons" runat="server" Width="212px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%"><span class="required">* </span>
                                    <b>Start Date: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox ID="txtStartDate" runat="server" Width="170px"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <cc1:CalendarExtender ID="startdate" runat="server"
                                                    Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                                    PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                </cc1:CalendarExtender>
                                                <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>


                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%"><span class="required">* </span>
                                    <b>End Date: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox ID="txtEndDate" runat="server" Width="170px"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <cc1:CalendarExtender ID="EndDate" runat="server"
                                                    Format="MM/dd/yyyy" PopupButtonID="imgEndDate"
                                                    PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                </cc1:CalendarExtender>
                                                <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Button ID="btnViewReport" runat="server" Text="View Report"
                                        CssClass="button" Width="100px" OnClick="btnViewReport_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                        Width="100px" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

