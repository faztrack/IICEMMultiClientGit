<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sendemailoutlook.aspx.cs" Inherits="sendemailoutlook" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-

transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script language="javascript" src="commonScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script type="text/javascript" src="CommonScript.js"></script>
    <title>Compose new Message</title>
</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="top" colspan="2">
                    <h1>
                        <img src="images/icon_compose_mail.png" width="48px" /><asp:Label ID="lblTitle" runat="server" Style="color: #fff; font-size: 22px; text-transform: uppercase;">Compose new Message</asp:Label></h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">
            <tr>
                <td align="right"><b>To:</b></td>
                <td>
                    <asp:TextBox ID="txtTo"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right"><b>From:</b></td>
                <td>
                    <asp:TextBox ID="txtFrom"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>CC: </b></td>
                <td>
                    <asp:TextBox ID="txtCc"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr id="Email2" runat="server">
                <td align="right">
                    <b>CC (Email 2):</b></td>
                <td align="left">
                    <asp:TextBox ID="txtCc2" runat="server" CssClass="textBox" TabIndex="1"
                        Width="445px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td align="right"><b>BCC: </b></td>
                <td>
                    <asp:TextBox ID="txtBcc"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>Subject:</b></td>
                <td>
                    <asp:TextBox ID="txtSubject"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="199"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>Attachments:</b></td>
                <td>
                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td width="132px" align="right"><b>Add Attachments:</b></td>
                <td align="left">

                    <table>
                        <tr>
                            <td>

                                <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" onchange="UploadFile()"/> 
                                <%--ClientIDMode="Static" onchange="this.form.submit()"--%>
                            </td>
                           
                        </tr>
                        <tr>
                            <td colspan="2">

                                <asp:Label ID="lblMessage" runat="server" />

                            </td>
                        </tr>
                    </table>

                </td>
                 <td align="right">
                                <asp:Button ID="btnUpload" runat="server" CssClass="noEffectNew" OnClick="btnUpload_Click" Width="2px" OnClientClick="ShowProgress();"
                                    Text="" />

                            </td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td>
                &nbsp;
            </tr>
            <tr align="right">
                <td colspan="2" align="left">Write your message below:(5000 Chars Max)<asp:TextBox
                    ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                    Height="16px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="editorBox" colspan="2" class="style1">

                    <cc1:Editor ID="txtBody" runat="server" EnableTheming="True" Height="500px"
                        Width="100%" CssClass="" HtmlPanelCssClass="" />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:ImageButton ID="imgSend" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/cancelMail.gif" />
                </td>
            </tr>
            <tr>
                <td colspan="2">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnMessageId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnChEstId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                </td>
            </tr>
        </table>
        <input type="hidden" name="hdnva" runat="server" id="hdnva" />
          <div id="LoadingProgress" style="display: none">
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </div>

    </form>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
        function UploadFile() {
           
            $("#<%=btnUpload.ClientID %>")[0].click();
        }
    </script>
</body>

</html>
