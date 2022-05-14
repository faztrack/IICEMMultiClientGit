<%@ Page Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="nopermissioncustomer.aspx.cs"
    Inherits="nopermissioncustomer" Title="Customer Portal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table style="width: 100%; height: 390px" align="center">
        <tr>
            <td>
                <table style="width: 100%; height: 390px" align="center">
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblNoPermission" runat="server" Font-Bold="True" Font-Names="Trebuchet MS"
                                Font-Size="X-Large" ForeColor="Black" Text="">"Your project portal hasn't been enabled just yet. Please contact your sales associate at Arizona's Interior Innovations." </asp:Label>
                        </td>
                    </tr>
                    <tr style="width: 80%;">
                        <td valign="top" align="center" style="width: 80%;" >
                            <asp:Button ID="btnCancel" runat="server" Text="Close" TabIndex="24" OnClick="btnCancel_Click"
                                Width="80px" CssClass="button" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
