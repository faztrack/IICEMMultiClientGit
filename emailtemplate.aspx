<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="emailtemplate.aspx.cs" Inherits="emailtemplate" Title="Email Template " %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .emailForm:first-child {
            margin-top: 10px;
        }

        .emailForm {
            margin-left:  10% !important;
            display: block;
        }

        .textBoxDs {
            margin-left: 20% !important;
            display: block;
        }
        body, table td {
            background:#f5f5f5;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table cellpadding="0" cellspacing="0" width="100%" align="center">
                <tr>
                    <td align="left" class="cssHeader">
                        <span class="titleNu">
                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Email Template Details</asp:Label></span>
                    </td>

                </tr>

                <tr>
                    <td>
                        <table class="emailForm" cellpadding="2" cellspacing="3" width="80%">
                             <tr>
                                <td align="right"><span class="required">* required</span></td>
                                <td></td>                                    
                            </tr>

                            <tr>
                                <td align="right"><b><span class="required">*</span>Name:</b></td>
                                <td>
                                    <asp:TextBox ID="txtName" runat="server" TabIndex="0" CssClass="textBox" Width="297px" MaxLength="200"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <b><span class="required">*</span>To:</b>
                                    <br />
                                    <asp:TextBox ID="lblToCount" runat="server" BackColor="Transparent" CssClass="nostyle textRightAlign"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        ReadOnly="True">
                                    </asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTo" runat="server" TabIndex="1" TextMode="MultiLine" CssClass="textBox" Width="300px" MaxLength="300" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'300',document.getElementById('head_lblToCount'));"></asp:TextBox>
                                </td>
                                <td valign="top">(<font style="color: #2196f3; font-style: italic;">one or multiple emails separated by comma</font>)</td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <asp:TextBox ID="txtFromCount" runat="server" TabIndex="2" BackColor="Transparent" CssClass="nostyle textRightAlign"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        ReadOnly="True">
                                    </asp:TextBox>
                                    <b><span class="required">*</span>From:</b>                                                                       
                                </td>
                                <td>
                                    <asp:TextBox ID="txtFrom"
                                        runat="server" CssClass="textBox" Width="297px" MaxLength="300" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'300',document.getElementById('head_txtFromCount'));"></asp:TextBox>
                                </td>                                
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>CC: </b>
                                    <br />
                                    <asp:TextBox ID="txtCCCount" runat="server" BackColor="Transparent" CssClass="nostyle textRightAlign"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        ReadOnly="True"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCc"
                                        runat="server" CssClass="textBox" TabIndex="3" TextMode="MultiLine" Width="300px" MaxLength="300" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'300',document.getElementById('head_txtCCCount'));"></asp:TextBox>
                                </td>
                                <td valign="top">(<font style="color: #2196f3; font-style: italic;">one or multiple emails separated by comma</font>)</td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <b>BCC: </b>
                                    <br />
                                    <asp:TextBox ID="txtBCCCount" runat="server" BackColor="Transparent" CssClass="nostyle textRightAlign"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        ReadOnly="True"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBcc"
                                        runat="server" CssClass="textBox" TabIndex="4" TextMode="MultiLine" Width="300px" MaxLength="300" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'300',document.getElementById('head_txtBCCCount'));"></asp:TextBox>
                                </td>
                                <td valign="top">(<font style="color: #2196f3; font-style: italic;">one or multiple emails separated by comma</font>)</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:TextBox ID="txtSubjectCount" runat="server" BackColor="Transparent" CssClass="nostyle textRightAlign"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        ReadOnly="True"></asp:TextBox>
                                    <b><span class="required">*</span>Subject:</b></td>
                                <td>
                                    <asp:TextBox ID="txtSubject"
                                        runat="server" CssClass="textBox" Width="297px" TabIndex="5" MaxLength="300" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'300',document.getElementById('head_txtSubjectCount'));"></asp:TextBox>
                                </td>
                                <td valign="top">(<font style="color: #2196f3; font-style: italic;">one or multiple emails separated by comma</font>)</td>
                            </tr>

                        </table>
                    </td>
                </tr>



                <tr>
                    <td align="right">&nbsp;</td>
                    <td>
                    &nbsp;
                </tr>
                <tr align="right">
                    <td class="textBoxDs" colspan="2" align="left">Write your message below:(5000 Chars Max)<asp:TextBox
                        ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                        Height="16px" ReadOnly="True"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2" class="style1">
                        <cc1:Editor ID="txtBody" runat="server" EnableTheming="True" Height="500px"
                            Width="60%" CssClass="" HtmlPanelCssClass="" />
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Button ID="btnSave" runat="server" CssClass="button" Text="Save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" CssClass="button" Text="Cancel" OnClick="btnCancel_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <asp:HiddenField ID="hdnEmailTemplateId" runat="server" Value="0" />
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

