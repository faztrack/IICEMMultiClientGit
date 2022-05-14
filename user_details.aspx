<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="user_details.aspx.cs" Inherits="user_details" Title="User Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .customScroll div {
            height: 760px;
            overflow: scroll;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table id="Table5" align="center" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td colspan="2" align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Add New User</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="50%" valign="top">
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
                                    <b>
                                        <asp:Label ID="lblEmail" runat="server">Email:</asp:Label></b>
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

                            <%--   <tr>
                                <td align="right">
                                    <b>&nbsp;</b></td>
                                <td align="left" colspan="2">
                                    <b>Canyon State Bus Sales Company eMail</b></td>
                            </tr>
                            <tr>
                                <td align="right">&nbsp;</td>
                                <td align="left" colspan="2">
                                    <asp:Panel ID="pnlCompanyEmail" runat="server" BackColor="Silver">
                                        <table cellpadding="4px" cellspacing="4px" width="100%">
                                            <tr>
                                                <td align="right">
                                                    <b>Email:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCompanyEmail" runat="server" TabIndex="9" Width="300px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right"></td>
                                                <td align="left">
                                                    <asp:Label ID="lblChangePassword" runat="server" ForeColor="#0033CC"
                                                        Text="ex: username@domainname.com"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <b>eMail Password: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtEmailPassword" runat="server" AutoComplete="Off" TabIndex="9"
                                                        TextMode="Password" MaxLength="9"></asp:TextBox>
                                                    <asp:CompareValidator ID="CompareValidator1" runat="server"
                                                        ControlToCompare="txtEmailPassword" ControlToValidate="txtConfirmEmailPassword"
                                                        ErrorMessage="Please confirm password"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <b>Re-type Password: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtConfirmEmailPassword" runat="server" AutoComplete="false"
                                                        TabIndex="9" TextMode="Password" MaxLength="9"></asp:TextBox>
                                                    <asp:Button ID="btnSavePassword" runat="server" CssClass="button"
                                                        OnClick="btnSavePassword_Click" Text="Save Password" TabIndex="9" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <asp:Button ID="btnVerify" runat="server" CssClass="button"
                                                        OnClick="btnVerify_Click" Text="Verify eMail" TabIndex="9" />
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblResult1" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>--%>
                            <tr>
                                <td align="right"><span class="required">*</span>
                                    <b>User Name: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtUsername" runat="server" TabIndex="11" Width="200px"></asp:TextBox>
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
                                <td align="right">
                                    <b>Role:</b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:DropDownList ID="ddlRole" runat="server" TabIndex="16">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Status: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="True" TabIndex="17" Text="Active" />
                                    <asp:CheckBox ID="chkIsSales" runat="server" Checked="True" TabIndex="18" Text="Sales" />
                                    <asp:CheckBox ID="chkIsTimeClock" runat="server" TabIndex="18" Text="Enable Time clock" />
                                    <asp:CheckBox ID="IsPriceChange" runat="server" TabIndex="19"
                                        Text="Allow Price Change in Estimate" />
                                    <asp:CheckBox ID="chkIsService" runat="server" Checked="True" TabIndex="19"
                                        Text="Service" Visible="False" />
                                    <asp:CheckBox ID="chkIsInstall" runat="server" Checked="True" TabIndex="20"
                                        Text="Install" Visible="False" />
                                    <asp:CheckBox ID="chkIsSMS" runat="server" Checked="True" TabIndex="20"
                                        Text="Enable SMS" />
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>
                                        <asp:Label ID="lblComission" runat="server" Text="Commission %"></asp:Label>
                                    </b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtCom" runat="server" Width="57px" TabIndex="21">0.0</asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><b>
                                    <asp:Label ID="lblCOComission" runat="server" Text="C/O Commission %"></asp:Label>
                                </b></td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtCOCom" runat="server" TabIndex="22" Width="57px">0.0</asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">&nbsp;</td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <b>Email Integration: </b>
                                </td>
                                <td align="left" colspan="2">
                                    <%-- <asp:RadioButtonList ID="rdbEmailIntegrationType" runat="server" TabIndex="9" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdbEmailIntegrationType_SelectedIndexChanged">
                                        <asp:ListItem Text="Outlook" Value="1" Selected="True"></asp:ListItem>                                        
                                    </asp:RadioButtonList>--%>
                                    <asp:CheckBox ID="chkEmailIntegrationType" runat="server" TabIndex="9" Checked="True" AutoPostBack="true" OnSelectedIndexChanged="chkEmailIntegrationType_SelectedIndexChanged" OnCheckedChanged="chkEmailIntegrationType_CheckedChanged" Text="Outlook/Exchange"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>
                                        <asp:Label ID="lblEmailIntegrationRequred" runat="server" CssClass="required">*&nbsp;</asp:Label>
                                        <asp:Label ID="lblEmailIntegration" runat="server"></asp:Label></b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtEmailIntegration" runat="server" TabIndex="9" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>
                                        <asp:Label ID="lblEmailPasswordRequred" runat="server" CssClass="required">*&nbsp;</asp:Label>
                                        <asp:Label ID="lblEmailPassword" runat="server"></asp:Label></b>
                                </td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtEmailPassword" runat="server" TextMode="Password" TabIndex="9" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><b>
                                    <asp:Label ID="lblEmailPasswordConRequred" runat="server" CssClass="required">*&nbsp;</asp:Label>
                                    <asp:Label ID="lblEmailPasswordCon" runat="server"></asp:Label>
                                </b></td>
                                <td align="left" colspan="2">
                                    <asp:TextBox ID="txtEmailPasswordCon" runat="server" TextMode="Password" TabIndex="9" Width="200px"></asp:TextBox>
                                    <asp:LinkButton ID="lnkEmail" runat="server" OnClick="lnkEmail_Click" Text="Test Your email connectivity"></asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">&nbsp;</td>
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
                                    <b>Email Signature:</b>
                                    <br />
                                    <asp:TextBox ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput" BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True" Height="16px" ReadOnly="True"></asp:TextBox>
                                </td>
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
                                        TabIndex="23" Text="Submit" CssClass="button" OnClick="btnSubmit_Click"
                                        Width="80px" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" TabIndex="24" Text="Cancel"
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
                    <td width="50%" valign="top" class="customScroll">
                        <asp:GridView ID="grdPagePermission" runat="server" AllowPaging="false" Width="75%"
                            AutoGenerateColumns="False"
                            CssClass="mGrid">
                            <Columns>
                                <asp:BoundField DataField="menu_name" HeaderText="Page Name">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Write Permission" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkIsWrite" runat="server" Checked='<%# Eval("IsWrite") %>'></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                            <AlternatingRowStyle CssClass="alt" />
                        </asp:GridView>
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

