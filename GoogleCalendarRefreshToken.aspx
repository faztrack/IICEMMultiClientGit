<%@ Page Title="GoogleCalendarTest" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="GoogleCalendarRefreshToken.aspx.cs" Inherits="GoogleCalendarRefreshToken" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
   <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center">
                <h1>Google Calendar Authenticate & Test</h1>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>

            </td>
        </tr>
        <tr>
            <td  align="center"><b>Google Calendar Test by SalesPerson</b></td>
        </tr>
        <tr>
            <td align="center">
                <asp:DropDownList ID="ddlSalesPerson" runat="server" TabIndex="22">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnAuthenticate" runat="server" Text="Authenticate" CssClass="button" OnClick="btnAuthenticate_Click" />
                &nbsp;
                <asp:Button ID="btnTest" runat="server" Text="Test" CssClass="button" OnClick="btnTest_Click" />

            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center"><b>Google Calendar Test by Calender ID and User ID</b></td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblCalendarID" runat="server" Text="Calendar ID"></asp:Label>
                <asp:TextBox ID="txtCalendarID" runat="server" Width="450px">
                </asp:TextBox>
            </td>
        </tr>
         <tr>
            <td align="center">
                <asp:Label ID="lblUserID" runat="server" Text="User ID"></asp:Label>
                <asp:TextBox ID="txtUserID" runat="server">
                </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnAuthenticateByID" runat="server" Text="Authenticate" CssClass="button" OnClick="btnAuthenticateByID_Click" />
                &nbsp;
                <asp:Button ID="btnTestByID" runat="server" Text="Test" CssClass="button" OnClick="btnTestByID_Click" />

            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

