<%@ Page Title="Sales Report" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SalesReportMonthy.aspx.cs" Inherits="SalesReportMonthy" %>

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
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Sales Report</span></td>
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
                                        <asp:DropDownList ID="ddlDivision" runat="server" Width="153px"></asp:DropDownList>
                                    </td>
                                </tr>
                            </asp:Panel>
                            <tr>
                                <td align="right" width="45%">
                                    <b>Salesperson: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSalesPersons" runat="server" CssClass="select2" Width="153px"></asp:DropDownList>
                                </td>
                            </tr>
                           
                            <tr>
                                <td align="right" width="45%">
                                     <b>Status: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlStatus" runat="server" Width="153px">
                                                    <asp:ListItem Value="1" Selected="True">All</asp:ListItem>
                                                    <asp:ListItem  Value="2">Active</asp:ListItem>
                                                    <asp:ListItem Value="4">Archive</asp:ListItem>
                                                    <asp:ListItem Value="5">InActive</asp:ListItem>
                                                    <asp:ListItem Value="7">Warranty Only</asp:ListItem>
                                        
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                     <b>Estimate Status: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlEstimateStatus" runat="server" Width="153px">
                                        <asp:ListItem Value="All" Selected="True">All</asp:ListItem>
                                        <asp:ListItem Value="1">Active</asp:ListItem>
                                        <asp:ListItem Value="0">InActive</asp:ListItem>
                                        
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="45%"><span class="required">*</span>
                                   
                                    <asp:Label ID="lblStartDate" runat="server" Text="Sold End Date:"></asp:Label>
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
                                   
                                    <asp:Label ID="lblEndDate" runat="server" Text="Sold Start Date:"></asp:Label>
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
                                <td align="right" width="45%">
                                    <b>Date Type: </b>
                                </td>
                                <td align="left" valign="middle">
                                    <asp:RadioButtonList ID="rdbReportType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rdbReportType_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="1" Selected="True">Sold Date Based</asp:ListItem>
                                        <asp:ListItem Value="2">Lead Entry Date Based</asp:ListItem>
                                    </asp:RadioButtonList>  
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
                                   
                                     <asp:Button ID="btnViewExcel" runat="server" Text="View"
                                        CssClass="button" OnClick="btnViewExcel_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                        OnClick="btnCancel_Click" />
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


