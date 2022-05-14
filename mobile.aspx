<%@ Page Language="C#" MasterPageFile="~/madminlogin.master" AutoEventWireup="true" CodeFile="mobile.aspx.cs" Inherits="mobile" Title="IICEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .customSeal1 img {
    width: 100px !important;
}
        .sealWidth {
            width:100px !important;
        }
        .sealWidth div {
            width:64px !important;
        }
    </style>
    <table width="100%" class="loginBox" cellpadding="5" cellspacing="5">
        <tr>
            <td align="center">
                <h2>Login</h2>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table width="60%">
                    <tr>
                        <td class="loginBoxText" align="right">Username: 
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtUserName" CssClass="textBoxNor" autofocus runat="server" Width="200px" TabIndex="1"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="loginBoxText" align="right">Password: 
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtPassword" AutoComplete="off" runat="server" Width="200px" title="Enter Password" 
                                CssClass="textBoxNor" TabIndex="2" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr><td>&nbsp;</td>
                        <td align="left">                        
                            <asp:Button ID="btnLogIn" runat="server" Text="Login" TabIndex="3" CssClass="loginButton" OnClick="btnLogIn_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID ="rememberme" runat="server"  Text ="Remember Me" />
                        </td>
                        
                    </tr>
                      <tr>
                        <td align="left" colspan="2">                        
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                       <td class="sealWidth" style="padding: 0 15px 0 0; width:64px;">
                    <div id="sealWidth" style="float: left; text-align: right;width:64px !important" title="Click to Verify - This site chose Thawte SSL for secure e-commerce and confidential communications.">
                        <span style="padding: 0px; margin: 0px; display: block;">
                            <script type="text/javascript" src="https://seal.thawte.com/getthawteseal?host_name=faztimate.interiorinnovations.biz&amp;size=S&amp;lang=en"></script>
                        </span>
                        <span style="padding: 0px; margin: 0px; display: block;">
                            <a href="http://www.thawte.com/ssl-certificates/" target="_blank" style="color: #000000; text-decoration: none; font: bold 10px arial,sans-serif; margin: 0px; padding: 0px;">ABOUT SSL CERTIFICATES</a>
                        </span>
                    </div>
                </td>
                        <td align="right" >

                             <a href="http://www.faztrack.com/" target="_blank">
                                <img style="width: 92px;" alt="FazTrack" src="assets/faztimate_login_logo.png" title="FazTrack" /></a>
                            
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>



