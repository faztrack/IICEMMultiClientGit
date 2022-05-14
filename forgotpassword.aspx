<%@ Page Language="C#" MasterPageFile="~/adminmasterlogin.master" AutoEventWireup="true" CodeFile="forgotpassword.aspx.cs" Inherits="forgotpassword" Title="Interior Innovations Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <div style="position: relative; background-image: url('./assets/loginbg0.jpg'); background-repeat: no-repeat; background-position: center top; font-family: Tahoma, Arial, sans-serif; -webkit-background-size: 100%; -moz-background-size: 100%; -o-background-size: 100%; background-size: 100%;" id="loginTop50">
        <div>&nbsp;</div>
        <div style="margin: 0 auto; background-color: #fff; width: 600px; padding: 20px; border-radius: 10px; opacity: .9; height: 440px;">
            <div id="header">
                <div>
                    <img width="300px" src="assets/client_login_logo.png" title="Login" alt="Interior Innovations" />
                </div>
            </div>
            <div id="box">
                <p class="main" style="text-align: center; margin-top: 15px;">
                    <asp:Label Style="text-transform: uppercase; color: #000; font-weight: bold; font-size: 18px; border: 2px solid #000; border-radius: 5px; padding: 6px 14px;" class="panel-title" ID="lblLoginPanelTitle" runat="server">Forgot Password</asp:Label>
                </p>
                <p id="pVerificationTitle" runat="server" style="text-align: center; margin: 12px 0; font-size: 14px; color: #f00; font-weight: bold; letter-spacing: 1px;">Password Verification Question</p>

                <p class="main" style="margin: 3px 0;">
                    <asp:Label ID="lblQuestion" runat="server" Text=""></asp:Label>
                </p>
                <p class="main">
                    <asp:TextBox ID="txtAnswer" runat="server" title="Enter Answer" CssClass="tooltip" TabIndex="2"></asp:TextBox>
                </p>
                <div id="divPasswordReset" runat="server" visible="false">
                    <p class="main" style="margin: 3px 0;">
                        <asp:Label ID="lblPassword" runat="server" Text="">New Password: </asp:Label>
                    </p>
                    <p class="main">
                        <asp:TextBox ID="txtPassword" runat="server" title="Enter Password" CssClass="tooltip" TabIndex="2" TextMode="Password"></asp:TextBox>
                    </p>
                    <p class="main" style="margin: 3px 0;">
                        <asp:Label ID="lblReTypePassword" runat="server" Text="">Re-type Password: </asp:Label>
                    </p>
                    <p class="main">
                        <asp:TextBox ID="txtConfirmPass" runat="server" title="Enter Confirm Password" CssClass="tooltip" TabIndex="2" TextMode="Password"></asp:TextBox>
                    </p>
                </div>
                <p class="space">
                    <div align="center">
                        <span>
                            <asp:Label ID="lblResult" runat="server"></asp:Label></span>
                    </div>
                    <div align="center">
                        <table cellpadding="0" cellspacing="0" align="center">
                            <tr>
                                <td align="center" valign="top">
                                    <asp:Button ID="btngotoLogin" runat="server" Text="Go to Login" OnClick="btngotoLogin_Click" TabIndex="3" CssClass="btnNormal" />
                                    <asp:Button ID="btnReset" runat="server" Text="Reset Password" OnClick="btnReset_Click" TabIndex="2" CssClass="btnDefault" Visible="false" />
                                    &nbsp;<asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" TabIndex="2" CssClass="btnDefault" /></td>
                            </tr>
                        </table>
                    </div>
                </p>
            </div>
        </div>
    </div>
    <div align="center" style="padding: 10px 0;">
        <a href="http://www.faztrack.com/" target="_blank">
            <img alt="FazTrack" src="assets/cem_login_logo.png" title="FazTrack" />
        </a>
    </div>
    <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />
</asp:Content>
