<%@ Page Language="C#" AutoEventWireup="true" CodeFile="refundpolicy.aspx.cs" Inherits="refundpolicy" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-

transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style>
        .PopUpMsg {
            background-image: linear-gradient(to right, #d42328, #c8aa62) !important;
            height: 80px;
        }

            .PopUpMsg h1 {
                margin: 0 100px 15px 100px !important;
            }

        p {
            margin: 20px 100px;
            font-size: 24px !important;
            line-height: 36px !important;
            color: #d42328 !important;
            font-weight:400;
        }
        b {
            font-weight:700 !important;
        }
    </style>

    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script language="javascript" src="commonScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script type="text/javascript" src="CommonScript.js"></script>
    <title>Refund Policy</title>
</head>
<body>

    <form id="form1" runat="server">



        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="middle" colspan="2">
                    <h1>
                        <img style="padding-right:20px;" src="images/ii-logo-light.png" width="196px" /><asp:Label ID="lblTitle" runat="server" Style="color: #fff; font-size: 22px; text-transform: uppercase; border-left: 2px solid #FFF; padding:5px;">&nbsp; Refund Policy</asp:Label></h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">



            <tr>

                <td>
                    <p>
                        Due to the specific nature of our service, and products, <b>no refund</b> is available once the order has been placed with our suppliers or sub-contractors. 
                    </p>
                </td>
            </tr>


        </table>



    </form>

</body>

</html>
