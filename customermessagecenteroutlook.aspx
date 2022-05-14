<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customermessagecenteroutlook.aspx.cs" Inherits="customermessagecenteroutlook" %>


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
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue(value) {
            window.open('replaymail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&emailId=' + value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue1(value, from, to, type) {
            window.open('messagedetailsoutlook.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value + '&From=' + from + '&To=' + to + '&Type=' + type, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue2(value) {
            window.open('DisplayPop3Email.aspx?emailId=' + value, 'MyWindow', 'left=200,top=100,width=850,height=550,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue3(value) {
            window.open('replayemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

        function GetdatakeyValue1Old(value) {
            window.open('messagedetails.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow2(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
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
                                                        <%-- <tr>
                                                            <td style="width: auto;" align="left" valign="top"><strong>
                                                                <asp:Label ID="lblPendingEst" runat="server" Text="Pending Estimate(s): "></asp:Label>
                                                            </strong></td>
                                                            <td style="width: auto;" align="left" valign="top">

                                                                

                                                            </td>
                                                        </tr>--%>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <table width="100%" border="0" cellspacing="8" cellpadding="4" align="center">

                                            <tr>
                                                <td align="left" colspan="4" class="wrapper">
                                                    <table width="100%" style="margin: 0px; padding: 0px;">
                                                        <tr>
                                                            <td align="left">
                                                                <b>Messages</b></td>
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
                                                            <asp:BoundField DataField="From" HeaderText="From" />
                                                            <asp:BoundField DataField="To" HeaderText="To" />
                                                            <asp:BoundField DataField="mess_subject" HeaderText="Subject" />

                                                            <asp:TemplateField HeaderText="Attachment">
                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypAttachment" runat="server" CssClass="hypg"></asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="sent_by" HeaderText="Sent by" />
                                                            <asp:BoundField DataField="IsRead" HeaderText="Viewed?">
                                                                <ItemStyle HorizontalAlign="center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Protocol" HeaderText="Sent Via" />
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypMessageDetails" runat="server" CssClass="btn_details">Details</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
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


