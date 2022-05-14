<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Title="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<table cellpadding="0" cellspacing="2" width="100%">
        <tr>
            <td align="center">
                Log In</td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="2" width="98%">
                    <tr>
                        <td align="right" style="width: 384px">
                            Username: </td>
                        <td align="left">
                            <asp:TextBox ID="txtUserName" runat="server" Width="200px" TabIndex="1">test</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" style="width: 384px">
                            Password: </td>
                        <td align="left">
                            <asp:TextBox ID="txtPassword" runat="server" Width="200px" 
                                TabIndex="2">123456</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnLogIn" runat="server" Text="Log In" TabIndex="3" 
                                onclick="btnLogIn_Click" />
&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="4" onclick="btnCancel_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            &nbsp;</td>
                    </tr>
                    </table>
            </td>
        </tr>
    </table>
</asp:Content>

