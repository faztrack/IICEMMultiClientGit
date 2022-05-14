<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="user_profile.aspx.cs" Inherits="user_profile" Title="User Information" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table id="Table5" align="center" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">User Profile</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="Table2" width="98%" align="center" border="0" cellpadding="0" cellspacing="3" onclick="return TABLE1_onclick()">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="35%" align="right"><span class="required">*</span>
                                    <b>First Name: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtFirstName" runat="server"
                                        TabIndex="1" Width="187px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">*</span>
                                    <b>Last Name: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtLastName" runat="server"
                                        TabIndex="2" Width="187px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    <b>Address: </b>
                                </td>
                                <td colspan="2" align="left">
                                    <asp:TextBox ID="txtAddress" runat="server" Height="40px"
                                        TabIndex="3" TextMode="MultiLine" Width="312px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>City: </b>
                                </td>
                                <td colspan="2" align="left">
                                    <asp:TextBox ID="txtCity" runat="server"
                                        TabIndex="4"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>State: </b>
                                </td>
                                <td colspan="2" align="left">
                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Zip: </b>
                                </td>
                                <td colspan="2" align="left">
                                    <asp:TextBox ID="txtZip" runat="server" TabIndex="6"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Phone: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtPhone" runat="server"
                                        TabIndex="7"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Fax: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtFax" runat="server"
                                        TabIndex="8"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">*</span>
                                    <b>Email: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtEmail" runat="server" TabIndex="9" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Gmail Address: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtGoogleCalendarAccount" runat="server" TabIndex="10" Width="200px"></asp:TextBox>&nbsp;(this email is for Google Calendar)
                                </td>
                            </tr>


                            <tr>
                                <td align="right"><span class="required">*</span>
                                    <b>User Name: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtUsername" runat="server" TabIndex="11"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblPasswordRequ" runat="server" Font-Bold="True" Visible="true" ForeColor="Red" Text="*"></asp:Label>
                                    <b>Password: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" TabIndex="12"></asp:TextBox>&nbsp;
                                    <asp:Label ID="lblChangePassword" runat="server" Visible="false" Text="Input Password &amp; Re-type Password to reset Password."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="lblReTypePasswordReq" runat="server" Visible="true" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                                    <b>Confirm Password:</b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtConfirmPass" runat="server" TextMode="Password"
                                        TabIndex="13"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">*</span>
                                    <b>Password Verification Question: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlQuestion" runat="server" TabIndex="14"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">*</span>
                                    <b>Answer: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtAnswer" runat="server" TabIndex="15" Width="150px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><b>
                                    <asp:Label ID="lblLandingPage" runat="server" Text="Landing Page"></asp:Label>
                                </b></td>
                                <td align="left" colspan="2">
                                    <asp:DropDownList ID="ddLandingPage" runat="server" TabIndex="19"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    <b>Email Signature:</b></td>
                                <td align="left" valign="top">
                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:TextBox ID="txtEMailSignature" runat="server" Width="300" Height="150px" TextMode="MultiLine"></asp:TextBox></pre>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:Button ID="btnSubmit" runat="server"
                                        TabIndex="16" Text="Submit" CssClass="button" OnClick="btnSubmit_Click"
                                        Width="80px" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" TabIndex="17" Text="Cancel"
                                        CausesValidation="False" CssClass="button" OnClick="btnCancel_Click"
                                        Width="80px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEmailPassword" runat="server" Value="0" />

                                    &nbsp;</td>
                                <td>&nbsp; </td>
                                <td align="right">
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

