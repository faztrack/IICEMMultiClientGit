<%@ Page Language="C#" AutoEventWireup="true" CodeFile="privacypolicy.aspx.cs" Inherits="privacypolicy" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-

transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style>
        .PopUpMsg {
            background-image: linear-gradient(to right, #d42328 , #c8aa62) !important;
            height:80px;
        }
        .PopUpMsg h1 {
            margin: 0 100px 15px 100px !important;
        }
        p {
            margin: 20px 100px;
            font-size:24px !important;
            line-height:36px !important;
            color:#333 !important;
        }
    </style>

    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script language="javascript" src="commonScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script type="text/javascript" src="CommonScript.js"></script>
    <title>Privacy Policy</title>
</head>
    
<body>

    <form id="form1" runat="server">



        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="middle" colspan="2">
                    <h1>
                        <img style="padding-right:20px;" src="images/ii-logo-light.png" width="196px" /><asp:Label ID="lblTitle" runat="server" Style="color: #fff; font-size: 22px; text-transform: uppercase; border-left: 2px solid #FFF; padding:5px;">&nbsp; Privacy Policy</asp:Label></h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">



            <tr>

                <td>
                    <p>
                        This policy is used to inform website visitors regarding our policies with the collection, use, and disclosure of Personal Information.
If you choose to use our Service then you agree to the collection and use of Person information for Billing, tracking of services, and for providing and improving our services. We will not use or share your information with non-related business parties.
For a better experience while using our Service, we may require you to provide Arizona’s Interior Innovations with certain personally identifiable information, including but not limited to your name, phone number, postal address, email address.  The information that we collect will be used to contact or identify you.
We value your trust in providing us your Personal Information, thus we are striving to use commercially acceptable means of protection it.  Please remember that no method of transmission over the internet or method of electronic storage is 100% secure and reliable, and we cannot guarantee its absolute security.
Our Service may contain links to other sites, if you click on a third-party link, you will be directed to that site.  Note that these external sites are not operated by use.  Therefore, we strongly advise you to review the Privacy Policy of those websites.
If you have any questions or suggestions about our Privacy Policy do not hesitate to contact us. 

                    </p>
                </td>
            </tr>


        </table>



    </form>

</body>

</html>
