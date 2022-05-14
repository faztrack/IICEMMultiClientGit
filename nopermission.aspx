<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="nopermission.aspx.cs"
    Inherits="nopermission" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table style="width: 100%; height: 290px" align="center">
        <tr>
            <td align="center">
                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                <asp:Label ID="lblNoPermission" runat="server" Font-Bold="True" Font-Names="Trebuchet MS"
                    Font-Size="X-Large" ForeColor="Red" Height="1px" Text="Authorization required to view this page. Contact your System Administrator."></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
