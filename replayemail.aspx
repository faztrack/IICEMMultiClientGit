﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="replayemail.aspx.cs" Inherits="replayemail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Customer Message Reply</title>


</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="top" colspan="2">
                    <h1>
                        <img src="images/icon_message_details.png" width="48px" />Customer Message Reply</h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="10" width="100%">
            <tr>
                <td align="right"><b>Attachments:</b></td>
                <td colspan="3">
                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td width="124px" align="right"><b>Add Attachments:</b></td>
                <td align="left">

                    <table>
                        <tr>
                            <td>

                                <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" />
                            </td>
                            <td>
                                <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Width="180px"
                                    Text="Upload Attachment" CssClass="button" />

                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">

                                <asp:Label ID="lblMessage" runat="server" />

                            </td>
                        </tr>
                    </table>

                </td>
            </tr>
            <tr>
                <td class="editorBox" colspan="4">
                    <cc1:Editor ID="txtBody" runat="server" EnableTheming="True" Height="500px"
                        Width="80%" CssClass="" HtmlPanelCssClass="" />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="4">
                    <asp:ImageButton ID="imgSend" CssClass="blindInput" runat="server" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" CssClass="blindInput" runat="server" ImageUrl="~/_scripts/cancelMail.gif"
                        OnClick="imgCencel_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="4">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnMessageId" runat="server" Value="0" />
                    <asp:Literal ID="DebugLiteral" runat="server" Visible="False" />

                    <asp:Literal ID="DateLiteral" runat="server" Visible="False" />
                    <asp:Literal ID="FromLiteral" runat="server" Visible="False" />
                    <asp:Literal ID="SubjectLiteral" runat="server" Visible="False" />
                    <asp:Literal ID="AttachmentsLiteral" runat="server" Visible="False" />
                    <asp:Literal ID="HeadersLiteral" runat="server" Visible="False" />
                    <asp:Literal ID="BodyLiteral" runat="server" Visible="False" />
                    &nbsp;<asp:Literal ID="EmailIdLiteral" runat="server" Visible="False" /></td>
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
