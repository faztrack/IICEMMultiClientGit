<%@ Page Title="Customer Job Status" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customerjobstatus.aspx.cs" Inherits="customerjobstatus" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Job Status Graphic Details for
                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px"></td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="2px" cellspacing="2px" width="80%">
                            <tr>
                                <td align="center">A
                                </td>
                                <td align="center">B
                                </td>
                                <td align="center">C
                                </td>
                                <td align="center">D
                                </td>
                                <td align="center">E
                                </td>
                                <td align="center">F
                                </td>
                                <td align="center">G
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Image ID="imgA" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgB" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgC" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgD" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgE" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgF" runat="server" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgG" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:CheckBox ID="chkA" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkA_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkB" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkB_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkC" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkC_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkD" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkD_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkE" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkE_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkF" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkF_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkG" runat="server" AutoPostBack="True" Enabled="False"
                                        OnCheckedChanged="chkG_CheckedChanged" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px"></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdDescription" runat="server">
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px">
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnJobStatusId" runat="server" Value="0" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

