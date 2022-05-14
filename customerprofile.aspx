<%@ Page Title="Customer Profile" Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="customerprofile.aspx.cs" Inherits="customerprofile" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <table cellpadding="5px" cellspacing="5px" width="100%">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">
                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Customer Profile</asp:Label></span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table cellpadding="2px" cellspacing="2px" width="100%">
                            <tr>
                                <td align="center">
                                    <table cellpadding="2px" cellspacing="2px" width="50%">
                                        <tr>
                                            <td align="center" colspan="2" style="background-color: Silver; font-size: medium; font-weight: bold;">
                                                <asp:LinkButton ID="lnkChangePassword" runat="server" OnClick="lnkChangePassword_Click">Change Password</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Panel ID="pnlChangePassword" runat="server">
                                        <table cellpadding="2px" cellspacing="2px" width="50%">
                                            <tr>
                                                <td align="left" colspan="2"></td>
                                            </tr>
                                            <tr>
                                                <td align="right">Current Password:
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCurrentPassword" runat="server" AutoComplete="Off"
                                                        TextMode="Password"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">New Password:
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" AutoComplete="Off"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">Re-type Password:
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" AutoComplete="Off"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">
                                                    <asp:CompareValidator ID="CompareValidator1" runat="server"
                                                        ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                                                        ErrorMessage="Please Confirm Password"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" height="10px" colspan="2"></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" height="10px" colspan="2"></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save Password"
                                                        CssClass="button" OnClick="btnSave_Click" />
                                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table cellpadding="2px" cellspacing="2px" width="50%">
                                        <tr>
                                            <td align="center" colspan="2" style="background-color: Silver; font-size: medium; font-weight: bold;">
                                                <asp:LinkButton ID="lnkSecurityQuestion" runat="server" OnClick="lnkSecurityQuestion_Click">Security Question</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Panel ID="pnlSecurityQuestion" runat="server">
                                        <table cellpadding="2px" cellspacing="2px" width="50%">
                                            <tr>
                                                <td align="right">Security Question
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlSecurityQuestion" runat="server">
                                                        <asp:ListItem Text="Your First Car" Value="Your First Car"></asp:ListItem>
                                                        <asp:ListItem Text="Your First Pet" Value="Your First Pet"></asp:ListItem>
                                                        <asp:ListItem Text="Your City of Birth" Value="Your City of Birth"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">Answer: 
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtAnswer" runat="server"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" height="10px" colspan="2"></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" height="10px" colspan="2"></td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Button ID="btnSaveAnswer" runat="server" Text="Save Answer"
                                                        CssClass="button" OnClick="btnSaveAnswer_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
