<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="incentivedetails.aspx.cs" Inherits="incentivedetails" Title="Incentive Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Add New Incentive</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="500px">
                             <tr>
                                <td align="right" >
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">* </span>Incentive Name: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtIncentiveName" runat="server" Width="200px" TabIndex="1"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">Description: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"
                                        Width="204px" TabIndex="2"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top"> Discount by:</td>
                                <td align="left">
                                    <asp:RadioButtonList ID="rdbDiscountType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdbDiscountType_SelectedIndexChanged" >
                                         <asp:ListItem Text="Percentage (%)" Value="1" Selected="True"></asp:ListItem>
                                         <asp:ListItem Text="Amount ($)" Value="2"></asp:ListItem>
                                       
                                    </asp:RadioButtonList> 
                                </td>
                            </tr>
                            <tr>
                                <asp:Panel ID="pnlDiscount" runat="server" Visible="false">
                                     <td align="right"><span class="required">* </span>Discount (%): 
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDiscount" runat="server" TabIndex="3" Width="200px"></asp:TextBox>
                                </td>
                                </asp:Panel>
                                 <asp:Panel ID="pnlAmount" runat="server" Visible="false">
                                     <td align="right"><span class="required">* </span>Discount ($): 
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtAmount" runat="server" TabIndex="3" Width="200px"></asp:TextBox>
                                </td>
                                </asp:Panel>
                               
                            </tr>
                            <tr>
                                <td align="right">Active: </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkActive" runat="server" TabIndex="4" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top"><span class="required">* </span>Start Date: </td>
                                <td align="left">
                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox ID="txtStartDate" runat="server" TabIndex="5" Width="170px"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <asp:Image ID="imgStart" runat="server" ImageUrl="images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top"><span class="required">* </span>End Date: </td>
                                <td align="left">
                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="left">
                                                <asp:TextBox ID="txtEndDate" runat="server" TabIndex="6" Width="170px"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <asp:Image ID="imgEnd" runat="server" ImageUrl="images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>


                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" TabIndex="7"
                                        OnClick="btnSubmit_Click" CssClass="button" />
                                    &nbsp;<asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add New Incentive"
                                        TabIndex="8" CssClass="button" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="9" OnClick="btnCancel_Click"
                                        CssClass="button" />
                                      <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:HiddenField ID="hdnIncentiveId" runat="server" Value="0" />
                                    <cc1:CalendarExtender ID="StartDate" runat="server" Format="MM/dd/yyyy"
                                        PopupButtonID="imgStart" TargetControlID="txtStartDate">
                                    </cc1:CalendarExtender>
                                    <cc1:CalendarExtender ID="EndDate" runat="server" PopupButtonID="imgEnd"
                                        TargetControlID="txtEndDate">
                                    </cc1:CalendarExtender>

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
     <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
</asp:Content>

