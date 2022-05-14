<%@ Page Language="C#" AutoEventWireup="true" CodeFile="contract_email.aspx.cs" Inherits="contract_email" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="_scripts/uploadify.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script language="javascript" src="commonScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Compose new Message</title>
    <style type="text/css">
        * {
            padding: 0;
            margin-left: 0;
            margin-right: 0;
            margin-top: 0;
        }

        .style1 {
            height: 25px;
        }
    </style>

</head>
<body>

    <form id="form1" runat="server">
        <input type="hidden" runat="server" id="hdnvalue" value="" name="hdnvaluename" style="display: none;" />
        <input type="hidden" runat="server" id="hdnuploadfile" value="" />
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="top" colspan="2">
                    <h1>
                        <img src="images/icon_compose_mail.png" width="48px" />Compose new Message </h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">
            <tr>
                <td align="right">To:</td>
                <td>
                    <asp:TextBox ID="txtTo"
                        runat="server" CssClass="textBox" Width="445px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">From:</td>
                <td>
                    <asp:TextBox ID="txtFrom"
                        runat="server" CssClass="textBox" Width="445px"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right">
                    <b>Cc: </b>
                </td>
                <td align="left" class="style3">
                    <asp:TextBox ID="txtCc" runat="server" CssClass="textBox" TabIndex="1"
                        Width="446px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">Subject:</td>
                <td>
                    <asp:TextBox ID="txtSubject"
                        runat="server" CssClass="textBox" Width="445px"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right" class="style1">Attachment(s):</td>
                <td class="style1">

                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>

                </td>
            </tr>
            <tr align="right">
                <td align="left">&nbsp;</td>
                <td align="left">Write your message below:(5000 Chars Max)<asp:TextBox
                    ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                    Height="16px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr align="right">
                <td align="left">&nbsp;</td>
                <td align="left">
                    <asp:TextBox ID="txtBody" runat="server" CssClass="textBox"
                        TextMode="MultiLine" Height="220px" Width="540px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1000',document.getElementById('txtDisplay'));"></asp:TextBox>
                </td>
            </tr>

            <tr align="right">
                <td align="left">&nbsp;</td>
                <td align="left">

                    <asp:Label ID="lblMessage" runat="server" />

                </td>
            </tr>

            <tr align="right">
                <td align="left">&nbsp;</td>
                <td align="left">
                    <asp:ImageButton ID="imgSend" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/cancelMail.gif" />
                </td>
            </tr>

            <tr>
                <td align="center" colspan="2">

                    &nbsp;</td>
            </tr>

            <tr>
                <td colspan="2">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnMessageId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnTypeId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False"
                        Value="0" />
                    &nbsp;<asp:HiddenField ID="hdnTempfile" runat="server" Value="0" />
                </td>
            </tr>
        </table>
        <input type="hidden" name="hdnva" runat="server" id="hdnva" />

        <div>

            <CR:CrystalReportViewer ID="CRViewer" runat="server"
                AutoDataBind="true" />

        </div>
    </form>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
    </script>
</body>

</html>
