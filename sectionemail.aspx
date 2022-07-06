<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sectionemail.aspx.cs" Inherits="sectionemail" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Section Email</title>


</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <table width="100%" cellpadding="5" cellspacing="10">
            <tr>
                <td align="center" valign="top" colspan="4">
                    <h1>Section Email</h1>
                </td>
            </tr>
            <tr>
                <td align="right"><b>To:</b></td>
                <td colspan="3">
                    <asp:TextBox ID="txtTo"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox>
                    <asp:Label ID="lblEmail" runat="server" ForeColor="Red" Text="*"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right"><b>From:</b></td>
                <td colspan="3">
                    <asp:TextBox ID="txtFrom"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox>
                    <asp:Label ID="lblFEmail" runat="server" ForeColor="Red" Text="*"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <b>Cc: </b>
                </td>
                <td align="left" class="style3" colspan="3">
                    <asp:TextBox ID="txtCc" runat="server" CssClass="textBox" TabIndex="1"
                        Width="445px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right"><b>Subject:</b></td>
                <td colspan="3">
                    <asp:TextBox ID="txtSubject"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="199"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>Attachments:</b></td>
                <td colspan="3">
                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td align="right"><b>Add Attachments:</b></td>
                <td>

                    <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" />
                </td>
                <td>

                    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click"
                        Text="Upload Attachment" CssClass="button" />

                </td>

            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td colspan="2">

                    <asp:Label ID="lblMessage" runat="server" />

                </td>
            </tr>
            <tr>
                <td align="left" colspan="4"><b>Message Body:</b></td>

            </tr>
            <tr>
                <td colspan="4" class="style1">
                    <cc1:Editor ID="Editor1" runat="server" EnableTheming="True" Height="500px"
                        Width="100%" ActiveMode="Preview" CssClass="" HtmlPanelCssClass="" />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="4">
                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="4">
                    <asp:ImageButton ID="imgSend" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/cancelMail.gif" />
                </td>
            </tr>
            <tr>
                <td colspan="4">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnCallId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnTypeId" runat="server" Value="0" />
                    &nbsp;</td>
            </tr>
        </table>
        <input type="hidden" name="hdnva" runat="server" id="hdnva" />


    </form>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
    </script>
</body>

</html>
