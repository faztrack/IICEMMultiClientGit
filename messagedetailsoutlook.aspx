<%@ Page Language="C#" AutoEventWireup="true" CodeFile="messagedetailsoutlook.aspx.cs" Inherits="messagedetailsoutlook" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Customer Message Details</title>


</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="top" colspan="2">
                    <h1>
                        <img src="images/icon_message_details.png" width="48px" />Customer Message Details</h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="10" width="100%">
            <tr>
                <td align="right"><b>To:</b></td>
                <td colspan="3">
                    <asp:Label ID="lblTo" runat="server" Width="445px" MaxLength="100" class="mailIDTO"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right"><b>From:</b></td>
                <td colspan="3">
                    <asp:Label ID="lblFrom" runat="server" Width="445px" CssClass="mailIDFrom"></asp:Label></td>
            </tr>
            <tr>
                <td align="right">
                    <b>CC: </b>
                </td>
                <td colspan="3">
                    <asp:Label ID="lblCc" runat="server" CssClass="mailIDAll"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <b>BCC: </b>
                </td>
                <td colspan="3">
                    <asp:Label ID="lblBcc" runat="server" CssClass="mailIDAll"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="right"><b>Subject:</b></td>
                <td colspan="3">
                    <asp:Label ID="lblSubject" runat="server" Width="445px" CssClass="mailSubject"></asp:Label></td>
            </tr>
            <tr>
                <td width="132px" align="right"><b>Attachments:</b></td>
                <td colspan="3">
                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td class="editorBox" colspan="4">
                    <cc1:Editor ID="txtBody" runat="server" EnableTheming="True" Height="500px"
                        Width="100%" ActiveMode="Preview" CssClass="msgDetails" HtmlPanelCssClass="msgDetails" />
                </td>
            </tr>
            <tr>
                <td align="right" colspan="4">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="4">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnMessageId" runat="server" Value="0" />
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

