<%@ Page Title="Sales Report By Lead" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SalesReportByLeadExcel.aspx.cs" Inherits="SalesReportByLeadExcel" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
	$(document).ready(function () {
	
		$('.select2').select2();
		
	});
	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
	function EndRequestHandler(sender, args) {
	
		$('.select2').select2();
		
	}
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Sales Report By Lead</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="2" width="100%">
                            <tr>
                                <td align="right" width="45%">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                             <asp:Panel ID="pnlDivision" runat="server" Visible="true">
                                <tr>
                                    <td align="right" width="45%">
                                        <b>Division: </b>
                                    </td>
                                    <td align="left" valign="middle">
                                        <asp:DropDownList ID="ddlDivision" runat="server" Width="152px"></asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:Panel>
                            <tr>
                                <td align="right" width="45%">
                                    <b>Sales Person: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSalesPersons" runat="server" Width="152px" CssClass="select2">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%"><span class="required">*</span>
                                    <b>Start Date: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <table style="padding: 0px; margin: -4px;">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:ImageButton CssClass="nostyleCalImg" ID="imgStartDate" runat="server" ImageUrl="~/images/calendar.gif" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%"><span class="required">*</span>
                                    <b>End Date: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <table style="padding: 0px; margin: -4px;">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox></td>
                                            <td>
                                                <asp:ImageButton CssClass="nostyleCalImg" ID="imgEndDate" runat="server" ImageUrl="~/images/calendar.gif" /></td>
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
                                <td align="center" colspan="2" style="height: 11px">
                                    <asp:Button ID="btnViewReport" runat="server" Text="View Report"
                                        CssClass="button" Width="100px" OnClick="btnViewReport_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                        Width="100px" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <cc1:CalendarExtender ID="startdate" runat="server"
                                        Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                        PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                    </cc1:CalendarExtender>
                                    <cc1:CalendarExtender ID="EndDate" runat="server"
                                        Format="MM/dd/yyyy" PopupButtonID="imgEndDate"
                                        PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                    </cc1:CalendarExtender>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnPrimaryDivision" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnDivisionName" runat="server" Value="" />
                                </td>
                                <td align="left"></td>
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
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>


