<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="leadstatusdetails.aspx.cs" Inherits="leadstatusdetails" Title="Lead Status Information" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblLeadStatus" runat="server" Text="Add New Lead Status"></asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="500px">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">* </span>Lead Status Name: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtLeadName" runat="server" Width="200px" TabIndex="1"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">Active: </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkActive" runat="server" TabIndex="2" Checked="True" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">Description: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"
                                        Width="204px" TabIndex="3"></asp:TextBox>
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
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" TabIndex="4"
                                        OnClick="btnSubmit_Click" CssClass="button" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="6" OnClick="btnCancel_Click"
                                        CssClass="button" />
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
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

