<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectNoteEmail.aspx.cs" Inherits="ProjectNoteEmail" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Project Notes Email</title>


</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <table cellpadding="5" cellspacing="10">
            <tr>
                <td align="center" valign="top" colspan="3">
                    <h1>Project Notes Email</h1>
                </td>
            </tr>
            <tr>
                <td align="right"><b>To:</b></td>
                <td colspan="2">
                    <asp:TextBox ID="txtTo"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"
                        ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right"><b>From:</b></td>
                <td colspan="2">
                    <asp:TextBox ID="txtFrom"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"
                        ReadOnly="True"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right">
                    <b>Cc: </b>
                </td>
                <td align="left" class="style3" colspan="2">
                    <asp:TextBox ID="txtCc" runat="server" CssClass="textBox" TabIndex="1"
                        Width="445px" MaxLength="100" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <b>Bcc: </b>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="txtBcc" runat="server" CssClass="textBox" TabIndex="1"
                        Width="445px" MaxLength="100" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right"><b>Subject:</b></td>
                <td colspan="2">
                    <asp:TextBox ID="txtSubject"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="199"
                        ReadOnly="True"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td>&nbsp;</td>
                <td>

                    <asp:Label ID="lblMessage" runat="server" />

                </td>
            </tr>
            <tr>
                <td align="left" colspan="3"><b>Message Body:</b></td>
            </tr>
            <tr>
                <td colspan="3" class="style1">
                    <cc1:Editor ID="Editor1" runat="server" EnableTheming="True" Height="500px"
                        Width="100%" ActiveMode="Design" CssClass="" HtmlPanelCssClass="" />
                </td>
            </tr>
            <tr>
                <td align="right" colspan="3">
                    <table style="padding: 0px; margin: 0px;">
                        <tr>
                            <td><asp:ImageButton ID="imgSend" runat="server" ImageUrl="~/_scripts/send.gif" CssClass="noBorderCss" OnClick="imgSend_Click" /></td>
                            <td><asp:ImageButton ID="imgCencel" runat="server" ImageUrl="~/_scripts/cancelMail.gif" CssClass="noBorderCss" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="3">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                     <asp:HiddenField ID="hdnSalesEmail" runat="server" Value="" />
                     <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
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

