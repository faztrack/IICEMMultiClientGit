<%@ Page Title="Error" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="error500.aspx.cs" Inherits="error500" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <table width="100%" border="0" class="formborder">
        <tr>
            <td align="center" valign="top">
                <table width="616">
                    <tr>
                        <td width="608">
                            <table width="600">
                                <tr>
                                    <td colspan="3" class="titleform1 style1">
                                        <div align="center" class="style2" style="color:#ff0000; font-size:16px; font-weight:bold;">Error</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 21px" colspan="3"></td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table align="center" cellpadding="0" cellspacing="0" width="600">
                                            <tr>
                                                <td align="middle" valign="top">
                                                    <strong><font color="#ff0000"><span><font face="Arial" size="2">We ran into a temporary
                                                technical difficulty. Our technical department has been notified. We will be working
                                                to fix this issue ASAP. Please re-visit our site in a little while.</font></span>
                                            </font></strong>
                                                    <asp:Label ID="lblError" runat="server" Font-Names="Arial" Font-Size="X-Small" ForeColor="White"
                                                        Width="555px"></asp:Label><br />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3"></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

