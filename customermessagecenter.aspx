<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customermessagecenter.aspx.cs"
    Inherits="customermessagecenter" Title="Customer Message Center" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--<style type="text/css">
        #loadImg
        {
            position: relative;
            z-index: 999;
        }
        #loadImg div
        {
            background: #fff;
            text-align: center;
            vertical-align: middle;
        }
    </style>--%>

    <script language="JavaScript" type="text/JavaScript">
        function DisplayWindow() {
            window.open('sendemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue(value) {
            window.open('replaymail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&emailId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue1(value) {
            window.open('messagedetails.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue2(value) {
            window.open('DisplayPop3Email.aspx?emailId=' + value, '_blank', 'left=200,top=100,width=850,height=550,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue3(value) {
            window.open('resendemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

    </script>

    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Customer message Center</span></td>
                        <td align="right" style="padding-right: 30px; float: right;">
                            <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 100%">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0" align="center">
                                <tr>
                                    <td align="center">
                                        <table class="wrapper" width="100%">
                                            <tr>
                                                <td style="width: 220px; border-right: 1px solid #ddd;" align="left" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <img src="images/icon-customer-info.png" /></td>
                                                            <td align="left">
                                                                <h2>Customer Information</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 390px;" align="left" valign="top">
                                                    <table style="width: 390px;">
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>Phone: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>Email: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table style="width: 420px;">
                                                        <tr>
                                                            <td style="width: 64px;" align="left" valign="top"><b>Address: </b></td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: auto;" align="left" valign="top"><strong>
                                                                <asp:Label ID="lblPendingEst" runat="server" Text="Pending Estimate(s): "></asp:Label>
                                                            </strong></td>
                                                            <td style="width: auto;" align="left" valign="top">

                                                                <asp:DropDownList ID="ddlEstimate" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEstimate_SelectedIndexChanged">
                                                                </asp:DropDownList>

                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <table width="60%" border="0" cellspacing="8" cellpadding="4" align="center">

                                            <tr>
                                                <td align="left" colspan="4" class="wrapper">
                                                    <table width="100%" style="margin: 0px; padding: 0px;">
                                                        <tr>
                                                            <td align="left">
                                                                <b>Sent Messages to Customer from the system:</b>
                                                            </td>
                                                            <td align="right">
                                                                <asp:HyperLink ID="HyperLink1" runat="server" CssClass="hyp">Compose New Message</asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:GridView ID="grdCustomersMessage" runat="server" AutoGenerateColumns="False"
                                                        CssClass="mGrid" OnRowDataBound="grdCustomersMessage_RowDataBound" PageSize="50"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="create_date" DataFormatString="{0:d}" HeaderText="Date">
                                                                <ItemStyle HorizontalAlign="center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Type" HeaderText="Type" />
                                                            <asp:BoundField DataField="mess_from" HeaderText="From" />
                                                            <asp:BoundField DataField="mess_to" HeaderText="To" />
                                                            <asp:BoundField DataField="mess_subject" HeaderText="Subject" />
                                                            <asp:TemplateField HeaderText="Attachment">
                                                                <ItemTemplate>
                                                                    <asp:Table ID="tdLink" runat="server">
                                                                    </asp:Table>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="sent_by" HeaderText="Sent By" />
                                                            <%--<asp:BoundField DataField="last_view" DataFormatString="{0:d}" HeaderText="Customer View">
                                                                <ItemStyle HorizontalAlign="center" />
                                                            </asp:BoundField>--%>
                                                            <asp:BoundField DataField="IsView" HeaderText="Viewed?">
                                                                <ItemStyle HorizontalAlign="center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Protocol" HeaderText="Sent Via" />
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypMessageDetails" runat="server" CssClass="hypg">Message Details</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" Font-Underline="true" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypResend" runat="server" CssClass="hypg">Resend Message</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" Font-Underline="true" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trfollow1" runat="server">
                                                <td class="wrapper" align="left" colspan="4">
                                                    <asp:Panel ID="pnlfollow1" runat="server">
                                                        <asp:ImageButton ID="Imagefollow1" runat="server" CssClass="imgBtnDefault" ImageUrl="~/Images/expand.png" TabIndex="40" />
                                                        <asp:Label ID="lblInitial" runat="server" CssClass="imgBtnTxtBlue" Text="Initial eMail"></asp:Label>
                                                    </asp:Panel>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" ImageControlID="Imagefollow1" CollapseControlID="Imagefollow1"
                                                        ExpandControlID="Imagefollow1" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlIntialeMail" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                    <asp:Panel ID="pnlIntialeMail" runat="server" Height="100%">
                                                        <table class="newWrapper" cellpadding="2" cellspacing="2" width="100%">
                                                            <tr>
                                                                <td>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Hello, </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>From all of us in Arizona's Interior Innovations Kitchen & Bath Design, we would like to thank you for giving us the </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>opportunity to present you with an estimate. We look forward to providing you with our award </td>
                                                            </tr>

                                                            <tr>
                                                                <td align='left'>winning craftsmanship and exceptional customer service. If you have any questions concerning</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>your estimate, please do not hesitate to call us.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Interior Innovations Kitchen & Bath Design is a family owned and operated, five star rated, licensed, bonded  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>and insured contracting company. We specialize in whole house redesign, bathroom remodeling </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>and kitchen remodeling. With an "A" rating with the BBB, we've been in business in the Valley </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>for more than 14 years with a talented team of tradespeople, designers and project managers </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>voted year after year as the Valley's Best!  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>As a family business, we take pride in building lasting relationships with our clients  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>and helping them build or redesign the homes they’ve always dreamed of. And now we look  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">forward to adding you to our family of customers (more than 5,000 strong) we are so proud to serve. </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Sincerely,</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>President</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Interior Innovations Kitchen & Bath Design</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Phone : 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Fax: 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="btnSendInitailMail" runat="server" CssClass="button" OnClick="btnSendInitailMail_Click" Text="Send this Email" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr id="trfollow2" runat="server">
                                                <td class="wrapper" align="left" colspan="4">
                                                    <asp:Panel ID="pnlfollow2" runat="server">
                                                        <asp:ImageButton ID="Imagefollow2" runat="server" CssClass="imgBtnDefault" ImageUrl="~/Images/expand.png" TabIndex="40" />
                                                        <asp:Label ID="lblfollow2" runat="server" CssClass="imgBtnTxtBlue" Text="2nd eMail with follow-up (Scheduled 3-days after the initial email)"></asp:Label>
                                                    </asp:Panel>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server" ImageControlID="Imagefollow2" CollapseControlID="Imagefollow2"
                                                        ExpandControlID="Imagefollow2" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlfolloweMail2" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                    <asp:Panel ID="pnlfolloweMail2" runat="server" Height="100%">
                                                        <table class="newWrapper" cellpadding="2" cellspacing="2" width="100%">
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design!</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>We are just following-up to see if you are closer to making a decision on the estimate we sent you. </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>We understand that it can be a challenge to find a competent partner to fulfill your home improvement needs.  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Your concerns are very important to us and we will do whatever it takes to make your project a success.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Please let us know if you have any questions. We look forward to hearing from you soon.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Sincerely,</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>President</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Interior Innovations Kitchen & Bath Design</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Phone : 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Fax: 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="btnSend2FollowUp" runat="server" CssClass="button" OnClick="btnSend2FollowUp_Click" Text="Send this Email" Visible="False" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr id="trfollow3" runat="server">
                                                <td class="wrapper" align="left" colspan="4">
                                                    <asp:Panel ID="pnlfollow3" runat="server">
                                                        <asp:ImageButton ID="Imagefollow3" runat="server" CssClass="imgBtnDefault" ImageUrl="~/Images/expand.png" TabIndex="40" />
                                                        <asp:Label ID="lblfollow3" runat="server" CssClass="imgBtnTxtBlue" Text="3rd eMail with Coupon"></asp:Label>
                                                    </asp:Panel>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender3" runat="server" ImageControlID="Imagefollow3" CollapseControlID="Imagefollow3"
                                                        ExpandControlID="Imagefollow3" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlfolloweMail3" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                    <asp:Panel ID="pnlfolloweMail3" runat="server" Height="100%">
                                                        <table class="newWrapper" cellpadding="2" cellspacing="2" width="100%">
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design! </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Just checking in to see if you're one step closer to working with us. We understand that budget </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>can be a concern in making a decision on which contractor to work with. In order to help ease </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>your concern, we are including a coupon to put towards your estimate. This is a limited time </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">offer valid for 30 days.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Please let us know if you'd like to take advantage of this offer and join the 5,000 satisfied </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>customers that have worked with us. We look forward to hearing from you soon.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Sincerely,</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>President</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Interior Innovations Kitchen & Bath Design</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Phone : 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Fax: 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblResult2" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="btnSend3FollowUp" runat="server" CssClass="button" OnClick="btnSend3FollowUp_Click" Text="Send this Email" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>

                                            <tr id="trWelcome" runat="server">
                                                <td class="wrapper" align="left" colspan="4">
                                                    <asp:Panel ID="pnlWelCome" runat="server">
                                                        <asp:ImageButton ID="ImageWelcome" runat="server" CssClass="imgBtnDefault" ImageUrl="~/Images/expand.png" TabIndex="40" />
                                                        <asp:Label ID="lblWelcome" runat="server" CssClass="imgBtnTxtBlue" Text="Welcome Email"></asp:Label>
                                                    </asp:Panel>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server" ImageControlID="ImageWelcome" CollapseControlID="ImageWelcome"
                                                        ExpandControlID="ImageWelcome" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlWelComeMail" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                    <asp:Panel ID="pnlWelComeMail" runat="server" Height="100%">
                                                        <table class="newWrapper" cellpadding="2" cellspacing="2" width="100%">
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design! </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>We're delighted that you've selected to work with us. We understand the gravity of trust you've  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>imparted in us to help build your dream project and look forward to providing nothing short of  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>exceptional customer service and fantastic craftsmanship. </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Should you have any additional questions about your estimate, please reach out. In the  </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>meantime, please stay tuned for a phone call from one of our top notch project coordinators to go </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>over next steps.</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>We're honored to be a part of your project's process and can't wait to work with you!</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>With gratitude, </td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>President</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Interior Innovations Kitchen & Bath Design</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Phone : 520- 461-1570</td>
                                                            </tr>
                                                            <tr>
                                                                <td align='left'>Fax: 520- 461-1570</td>
                                                            </tr>

                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblWelcomeResult" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="btnSendWelcome" runat="server" CssClass="button" OnClick="btnSendWelcome_Click" Text="Send this Email" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>

    </table>
</asp:Content>
