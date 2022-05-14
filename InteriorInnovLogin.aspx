<%@ Page Language="C#" MasterPageFile="~/adminmasterlogin.master" AutoEventWireup="true" CodeFile="InteriorInnovLogin.aspx.cs" Inherits="InteriorInnovLogin" Title="Arizona's Interior Innovations Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <div style="position: relative; background-image: url('./assets/loginbg0.jpg'); background-repeat: no-repeat; background-position: center top; font-family: Tahoma, Arial, sans-serif; -webkit-background-size: 100%; -moz-background-size: 100%; -o-background-size: 100%; background-size: 100%;" id="loginTop50">
        <div>&nbsp;</div>
        <div style="margin: 0 auto; background-color: #fff; width: 600px; padding: 20px; border-radius: 10px; opacity: .8; height: 400px;">
            <div id="header">
                <div>
                    <img width="300px" src="assets/client_login_logo.png" title="Login" alt="Interior Innovations" />
                </div>
            </div>
            <div id="box">
                <p class="main" style="text-align: center;">
                    <asp:Label Style="text-transform: uppercase; color: #f00; font-weight: bold; font-size: 20px;" class="panel-title" ID="lblLoginPanelTitle" runat="server"></asp:Label>
                </p>
                <p class="main">
                    <label style="vertical-align: middle;">Username: </label>
                    <asp:TextBox ID="txtUserName" runat="server" autofocus="autofocus" TabIndex="1"></asp:TextBox>
                </p>
                <p class="main">
                    <label style="vertical-align: middle;">Password:</label>
                    <asp:TextBox ID="txtPassword" runat="server" title="Enter Password" CssClass="tooltip" TabIndex="2" TextMode="Password"></asp:TextBox>
                </p>
                <p class="space">
                    <div align="center">
                        <span>
                            <asp:Label ID="lblResult" runat="server"></asp:Label></span>
                    </div>
                    <div align="center">
                        <table cellpadding="0" cellspacing="0" align="center">
                            <tr>
                                <td align="center" valign="top" colspan="2">
                                    <asp:Button ID="btnLogIn" runat="server"
                                        Text="Login" TabIndex="3" CssClass="login" OnClick="btnLogIn_Click" Width="400px" /></td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:LinkButton ID="lnkForgotPassword" runat="server" CssClass="black" TabIndex="4" OnClick="lnkForgotPassword_Click">Forgot Password?</asp:LinkButton>
                                </td>
                                <td align="right">
                                    <asp:HyperLink  ID="lnkMobileSiteLogin" runat="server" CssClass="black" Target="_blank" TabIndex="4" style="padding-right: 10px;">Mobile Site Login</asp:HyperLink>
                                </td>
                            </tr>
                        </table>
                    </div>
                </p>
            </div>
        </div>
    </div>
    <div align="center" style="padding: 10px 0;">
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding: 0 15px 0 0;">
                    <a style="font-weight:500 !important;" href="refundpolicy.aspx" target="_blank">
                        Refund Policy
                    </a>
               </td>
                <td style="padding: 0 15px 0 0;">
                    <a style="font-weight:500 !important;" href="privacypolicy.aspx" target="_blank">
                      Privacy Policy
                    </a>
               </td>
                <td style="padding: 0 0 0 15px;">
                    <asp:Label ID="lblAddressLabel" style="font-weight:500 !important; color:#555;" runat="server" Text="Address:"></asp:Label>

                </td>
                <td style="padding: 0 0 0 4px;">
                    <asp:Label ID="lblAddress" style="font-weight:500 !important;" runat="server" Text="20853 N 260th Ln, Buckeye AZ 85396"></asp:Label>

                </td>
            </tr>
        </table>
    </div>

    <div align="center" style="padding: 10px 0;">
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding: 0 15px 0 0;">
                    <div id="thawteseal" style="float: left; text-align: center;" title="Click to Verify - This site chose Thawte SSL for secure e-commerce and confidential communications.">
                        <span style="padding: 0px; margin: 0px; display: block;">
                            <script type="text/javascript" src="https://seal.thawte.com/getthawteseal?host_name=faztimate.interiorinnovations.biz&amp;size=S&amp;lang=en"></script>
                        </span>
                        <span style="padding: 0px; margin: 0px; display: block;">
                            <a href="http://www.thawte.com/ssl-certificates/" target="_blank" style="color: #000000; text-decoration: none; font: bold 10px arial,sans-serif; margin: 0px; padding: 0px;">ABOUT SSL CERTIFICATES</a>
                        </span>
                    </div>
                </td>
                <td style="padding: 0 0 0 15px;">
                    <a href="http://www.faztrack.com/" target="_blank">
                        <img alt="FazTrack" src="assets/cem_login_logo.png" title="FazTrack" />
                    </a></td>
            </tr>
        </table>
    </div>
</asp:Content>
