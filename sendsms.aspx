<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sendsms.aspx.cs" Inherits="sendsms" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc" %>--%>
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
                        &nbsp;&nbsp;&nbsp;<img src="images/icon_compose_mail.png" width="48px" alt="img" /><asp:Label ID="lblTitle" runat="server" Style="color: #fff; font-size: 22px; text-transform: uppercase;">Compose new SMS</asp:Label></h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
             <tr>
                <td align="right"style=" font-size:14px;"><label style="font-weight:bold;">Customer Name: </label></td>
                 <td ><asp:Label ID="lblCustomer" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td align="right" style=" font-size:12px;"><label style="font-weight:bold;">Mobile: </label></td>
                <td > <asp:Label ID="lblTo" runat="server"></asp:Label></td>
            </tr>

            <tr>
                <td align="right">&nbsp;</td>
                <td>&nbsp;</td>

            </tr>
            <tr align="right">
                <td colspan="2" align="left">Write your message below:(500 Chars Max)<asp:TextBox MaxLength="500"
                    ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                    Height="16px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="editorBox " colspan="2">

                    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Rows="5" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('txtDisplay'));"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">

                    <asp:Label ID="lblMessage" runat="server" />

                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:ImageButton ID="imgSend" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" runat="server" CssClass="blindInput" ImageUrl="~/images/CloseMail.gif" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                     <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
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

    </script>
</body>

</html>
