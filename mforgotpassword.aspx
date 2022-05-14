<%@ Page Language="C#" MasterPageFile="~/madminlogin.master" AutoEventWireup="true" CodeFile="mforgotpassword.aspx.cs" Inherits="mforgotpassword" Title="Interior Innovations Kitchen & Bath Design Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <div style="position: relative; background-image: url('./assets/loginbg0.jpg'); background-repeat: no-repeat; background-position: center top; font-family: Tahoma, Arial, sans-serif; -webkit-background-size: 100%; -moz-background-size: 100%; -o-background-size: 100%; background-size: 100%;" id="loginTop50">
        <div>&nbsp;</div>
        <div style="background-color: #fff; width: 350px; padding: 20px 20px; border-radius: 4px; opacity: .9; box-shadow: 0 0 3px #aaa;">
            
            <div style="max-width: 300px; min-width: 320px; margin: 0 auto;" id="box">
                <p class="main" style="text-align: center; margin-top: 15px;">
                    <asp:Label Style="text-transform: uppercase; color: #000; font-weight: bold; font-size: 18px; border: 2px solid #000; border-radius: 5px; padding: 6px 14px;" class="panel-title" ID="lblLoginPanelTitle" runat="server">Forgot Password</asp:Label>
                </p>
                <p id="pVerificationTitle" runat="server" style="text-align: center; margin: 12px 0; font-size: 14px; color: #f00; font-weight: bold; letter-spacing: 1px;">Password Verification Question</p>

                <p class="main" style="margin: 3px 0;">
                    <asp:Label ID="lblQuestion" runat="server" Text=""></asp:Label>
                </p>
                <p class="main">
                    <asp:TextBox ID="txtAnswer" Width="98%" runat="server" title="Enter Answer" CssClass="tooltip textBoxNor" TabIndex="2"></asp:TextBox>
                </p>
                <div id="divPasswordReset" runat="server" visible="false">
                    <p class="main" style="margin: 3px 0;">
                        <asp:Label ID="lblPassword" runat="server" Text="">New Password: </asp:Label>
                    </p>
                    <p class="main">
                        <asp:TextBox ID="txtPassword" Width="98%" runat="server" title="Enter Password" CssClass="tooltip" TabIndex="2" TextMode="Password"></asp:TextBox>
                    </p>
                    <p class="main" style="margin: 3px 0;">
                        <asp:Label ID="lblReTypePassword" runat="server" Text="">Re-type Password: </asp:Label>
                    </p>
                    <p class="main">
                        <asp:TextBox ID="txtConfirmPass" Width="98%" runat="server" title="Enter Confirm Password" CssClass="tooltip" TabIndex="2" TextMode="Password"></asp:TextBox>
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
                                    <table cellpadding="0" cellspacing="10">
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" TabIndex="2" CssClass="loginButton" /></td>
                                            <td>
                                                <asp:Button ID="btngotoLogin" runat="server" Text="Go to Login" OnClick="btngotoLogin_Click" TabIndex="3" CssClass="submitButton" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnReset" runat="server" Text="Reset Password" OnClick="btnReset_Click" TabIndex="2" CssClass="loginButton" Visible="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </p>
            </div>
            <div id="Div1">
                <p class="main">
                    <table style="padding: 20px 0; background-color: #fff;" align="center" width="100%">
                        <tr>
                            <td style="padding: 0 10px;" align="left" valign="bottom"><span class="customSeal" id="siteseal">
                                <script type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=R4j4jnWQLB6KzvieEealJG4CTYnxZrK0FLjSIpiwtzbIzAaNJb6DT1aj9Czx"></script>
                            </span></td>
                            <%--<td align="center">
                                <div class="AuthorizeNetSeal">
                                    <script type="text/javascript" language="javascript">var ANS_customer_id = "eeac5ee7-7130-4546-b8a6-cdd2fb915c1e";</script>
                                    <script type="text/javascript" language="javascript" src="//verify.authorize.net/anetseal/seal.js"></script>
                                    <a href="http://www.authorize.net/" id="AuthorizeNetText" target="_blank">Online Payments</a>
                                </div>
                            </td>--%>
                            <td style="padding: 0 30px 0 24px;" align="right" valign="bottom">
                                <a href="http://www.faztrack.com/" target="_blank">
                                    <img style="width: 92px;" alt="FazTrack" src="assets/cem_login_logo.png" title="FazTrack" />
                                </a>
                            </td>
                        </tr>
                    </table>
                </p>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnEmployeeId" runat="server" Value="0" />
</asp:Content>
