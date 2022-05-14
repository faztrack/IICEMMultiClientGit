<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="change_order_worksheet.aspx.cs" Inherits="change_order_worksheet" Title="Change Order WorkSheet" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
        function DisplayEmailWindow() {
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&cofn=' + document.getElementById('<%= hdnFileName.ClientID%>').value + '&coid=' + document.getElementById('<%= hdnChEstId.ClientID%>').value + '&eid=' + document.getElementById('<%= hdnEstimateId.ClientID%>').value, 'MyWindow', 'left=200,top=100,width=850,height=550,status=0,toolbar=0,resizable=0,scrollbars=1');

        }

    </script>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Change Order WorkSheet</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                        <td align="right" style="padding-right: 30px; float: right;">
                            <div class="divToolsLeft">
                                <ul style="list-style-type: none; margin: 0; padding: 0;">
                                    <li>
                                        <ul style="float: left;">
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hypMessage" runat="server"><img width="26" height="26" src="images/system_icons/01_icon.png" style="color:green;" alt="Message Center" title="Message Center" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hypCostLoc" runat="server"><img width="26" height="26" src="images/system_icons/16_icon.png" alt="Project Summary Report" title="Project Summary Report" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_CallLog" runat="server"><img width="26" height="26" src="images/system_icons/09_icon.png" alt="Activity Log" title="Activity Log" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_Schedule" runat="server"><img width="26" height="26" src="images/system_icons/05_icon.png" alt="Schedule" title="Schedule" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_Sow" runat="server"><img width="26" height="26" src="images/system_icons/06_icon.png" alt="Composite SOW" title="Composite SOW" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_ProjectNotes" runat="server"><img width="26" height="26" src="images/system_icons/14_icon.png" alt="Project Notes" title="Project Notes" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_Allowance" runat="server"><img width="26" height="26" src="images/system_icons/08_icon.png" alt="Allowance Report" title="Allowance Report" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_PreCon" runat="server"><img width="26" height="26" src="images/system_icons/10_icon.png" alt="Pre-Con Check List" title="Pre-Con Check List" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_SiteReview" runat="server"><img width="26" height="26" src="images/system_icons/11_icon.png" alt="Site Review" title="Site Review" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_DocumentManagement" runat="server"><img width="26" height="26" src="images/system_icons/12_icon.png" alt="Document Management" title="Document Management" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_Section_Selection" runat="server"><img width="26" height="26" src="images/system_icons/15_icon.png" alt="Selection" title="Selection" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_MaterialTracking" runat="server"><img width="26" height="26" src="images/system_icons/19_icon.png" alt="Material Tracking" title="Material Tracking" /></asp:HyperLink></li>

                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_vendor" runat="server"><img width="26" height="26" src="images/system_icons/02_icon.png" alt="Vendor Cost" title="Vendor Cost" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_Payment" runat="server"><img width="26" height="26" src="images/system_icons/03_icon.png" alt="Payment Info" title="Payment Info" /></asp:HyperLink></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_survey" runat="server"><img width="26" height="26" src="images/system_icons/07_icon.png" alt="Exit Questionnaire" title="Exit Questionnaire" /></asp:HyperLink></li>
                                            <%--<asp:HyperLink ID="hyp_Selection" style="display:none;" runat="server"><img width="26" height="26" src="images/section_sheet.png" alt="Selection Sheet" title="Selection Sheet" /></asp:HyperLink>--%>
                                            <li style="float: left;"></li>
                                            <li style="float: left;">
                                                <asp:HyperLink ID="hyp_jstatus" runat="server"><img width="26" height="26" src="images/system_icons/04_icon.png" alt="Job Status Graphics" title="Job Status Graphics" /></asp:HyperLink></li>
                                        </ul>
                                    </li>
                                </ul>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 1080px">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table style="margin-top: 10px;" class="wrapper" width="1080" border="0" cellspacing="0" cellpadding="0" align="center">
                                <tr>
                                    <td align="left" valign="top">
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0" align="center">
                                            <tr>
                                                <td align="left"><b>Viewable by Customer?</b></td>
                                                <td align="left">
                                                    <asp:RadioButtonList ID="rdoconfirm" runat="server"
                                                        OnSelectedIndexChanged="rdoconfirm_SelectedIndexChanged"
                                                        RepeatDirection="Horizontal" AutoPostBack="True">
                                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="2">No</asp:ListItem>
                                                    </asp:RadioButtonList></td>
                                                <td colspan="2">
                                                    <asp:LinkButton ID="lnkResend" runat="server" CssClass="underlineButton" OnClick="lnkResend_Click"><span style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Resend C/O Notification Message</span></asp:LinkButton>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="15%">
                                                    <b>Show UoM and Qty:</b> </td>
                                                <td style="padding-left: 2px;" align="left" valign="middle" width="35%">
                                                    <asp:CheckBox ID="chkChangeOrderQtyshow" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">
                                                    <b>Customer Name:</b> </td>
                                                <td style="padding-left: 8px;" align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                </td>
                                                <td align="left" width="15%" valign="top">
                                                    <b>Address: </b></td>
                                                <td align="left">
                                                    <table style="padding: 0px; margin: 0px;">
                                                        <tr>
                                                            <td align="left" valign="top">
                                                                <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="left" valign="top">
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">
                                                    <b>Phone:</b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="left" valign="top"></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Email:</b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Estimate Name:</b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                    &nbsp;</td>
                                                <td align="left">
                                                    <b>C/O Status:</b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True"
                                                        OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                        <asp:ListItem Value="1">Draft</asp:ListItem>
                                                        <asp:ListItem Value="2">Pending</asp:ListItem>
                                                        <asp:ListItem Value="3">Executed</asp:ListItem>
                                                        <asp:ListItem Value="4">Declined</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Change Order Name:</b></td>
                                                <td style="padding-left: 8px;" align="left">
                                                    <asp:Label ID="lblChangeOrderName" runat="server"></asp:Label>
                                                    <asp:LinkButton ID="lnkUpdateCoEstimate" runat="server"><span 
                            style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Rename</span></asp:LinkButton>
                                                </td>
                                                <td align="left"><b>C/O Type:</b></td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddlChangeOrderType" runat="server" AutoPostBack="True"
                                                        OnSelectedIndexChanged="ddlChangeOrderType_SelectedIndexChanged">
                                                        <asp:ListItem Value="1">Change Order</asp:ListItem>
                                                        <asp:ListItem Value="2">Clarification</asp:ListItem>
                                                        <asp:ListItem Value="3">Internal Use Only</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>
                                                        <asp:Label ID="lblCoDate" runat="server" ForeColor="#717171" Text="Change Order Date:"></asp:Label>
                                                    </b>
                                                </td>
                                                <td align="left">
                                                    <table>
                                                        <tr>
                                                            <td style="padding-left: 4px;">
                                                                <asp:TextBox ID="txtChangeOrderDate" runat="server"></asp:TextBox></td>
                                                            <td>
                                                                <asp:ImageButton ID="imgCODate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" /></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left" valign="middle">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">
                                                    <b>Notes: </b></td>
                                                <td style="padding-left: 5px;" align="left" colspan="3">
                                                    <asp:TextBox ID="txtNotes1" runat="server" Height="44px" TabIndex="1"
                                                        TextMode="MultiLine" Width="600px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <table id="tblReview" runat="server" style="width: 100%;">
                                                        <tr>
                                                            <td align="right" colspan="1">
                                                                <b>Customer Review:</b></td>
                                                            <td align="left">
                                                                <asp:RadioButtonList ID="rdoStatus" runat="server" RepeatDirection="Horizontal"
                                                                    Enabled="False">
                                                                    <asp:ListItem Selected="True" Value="1">Pending</asp:ListItem>
                                                                    <asp:ListItem Value="2">Accept</asp:ListItem>
                                                                    <asp:ListItem Value="3">Reject</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" colspan="1">
                                                                <asp:Label ID="lblAcceptReason" runat="server"></asp:Label>
                                                                &nbsp;&nbsp;
                                                            </td>
                                                            <td align="left">
                                                                <asp:Label ID="lblNameReason" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right" colspan="1">&nbsp;</td>
                                                            <td align="left">&nbsp;</td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:RadioButtonList
                                            ID="rdoSort" runat="server" AutoPostBack="True"
                                            OnSelectedIndexChanged="rdoSort_SelectedIndexChanged"
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Selected="True" Value="1">View by Locations</asp:ListItem>
                                            <asp:ListItem Value="2">View by Sections</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">

                                        <h3>
                                            <asp:Label ID="lblRetailPricingHeader" runat="server" Text="Selected Items" Visible="false"></asp:Label>
                                        </h3>

                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True"
                                            OnRowDataBound="grdGrouping_RowDataBound" AutoGenerateColumns="False"
                                            CssClass="mGrid">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />

                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                                            OnRowDeleting="grdSelectedItem_RowDeleting"
                                                            Width="100%" CssClass="mGrid" OnRowCommand="grdSelectedItem1_RowCommand">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id"
                                                                    HeaderStyle-Width="6%">
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL"
                                                                    HeaderStyle-Width="7%">
                                                                    <HeaderStyle Width="7%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes"
                                                                    HeaderStyle-Width="27%">
                                                                    <HeaderStyle Width="27%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" ForeColor="#427ed8" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="12%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name"
                                                                    HeaderStyle-Width="28%">
                                                                    <HeaderStyle Width="28%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UOM"
                                                                    HeaderStyle-Width="5%">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="quantity" HeaderText="Code"
                                                                    HeaderStyle-Width="5%">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Price">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Select Markup">
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="ddlEconomics" runat="server" AutoPostBack="True"
                                                                            SelectedValue='<%# Eval("EconomicsId") %>'
                                                                            Width="90px" OnSelectedIndexChanged="NonDirect_calculation">
                                                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                                                            <asp:ListItem Value="1">No charge</asp:ListItem>
                                                                            <asp:ListItem Value="2">Cost</asp:ListItem>
                                                                            <asp:ListItem Value="4">Cost x2</asp:ListItem>
                                                                            <asp:ListItem Value="3">Cost +20%</asp:ListItem>
                                                                            <asp:ListItem Value="6">Cost +30%</asp:ListItem>
                                                                            <asp:ListItem Value="7">Cost +50%</asp:ListItem>
                                                                            <asp:ListItem Value="8">Cost +70%</asp:ListItem>
                                                                            <asp:ListItem Value="5">Other</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="8%" />
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtViewEcon" runat="server"
                                                                            Text='<%# Eval("EconomicsCost") %>' Width="60px" AutoPostBack="True" OnTextChanged="NonDirect_calculation" CssClass="cssTxtAmount">0</asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="6%" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:ButtonField CommandName="Delete" Text="Delete" />

                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table width="100%" border="0" cellspacing="4" cellpadding="4">
                                            <tr>
                                                <td colspan="2" align="center">

                                                    <h3>
                                                        <asp:Label ID="lblDirectPricingHeader" runat="server"
                                                            Text="The following items are Direct / Outsourced" Visible="False"></asp:Label></h3>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" colspan="2" valign="top" width="15%">
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False"
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound"
                                                        ShowFooter="True" CaptionAlign="Top">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header"
                                                                        Text='<%# Eval("colName").ToString() %>' />
                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                                        CssClass="mGrid" DataKeyNames="item_id"
                                                                        OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        OnRowDeleting="grdSelectedItem2_RowDeleting"
                                                                        ShowFooter="True" Width="100%" OnRowCommand="grdSelectedItem2_RowCommand">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id"
                                                                                HeaderStyle-Width="6%">
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL"
                                                                                HeaderStyle-Width="7%">
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="short_notes" HeaderText="Short Notes"
                                                                                HeaderStyle-Width="27%">
                                                                                <HeaderStyle Width="27%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server"
                                                                                        Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name"
                                                                                HeaderStyle-Width="28%">
                                                                                <HeaderStyle Width="28%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="measure_unit" HeaderText="UOM"
                                                                                HeaderStyle-Width="5%">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="quantity" HeaderText="Code"
                                                                                HeaderStyle-Width="5%">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Direct Price">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server"
                                                                                        Text='<%# Eval("total_direct_price") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Select Markup">
                                                                                <ItemTemplate>
                                                                                    <asp:DropDownList ID="ddlEconomics1" runat="server" AutoPostBack="True"
                                                                                        SelectedValue='<%# Eval("EconomicsId") %>'
                                                                                        Width="90px" OnSelectedIndexChanged="Direct_calculation">
                                                                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                                                                        <asp:ListItem Value="1">No charge</asp:ListItem>
                                                                                        <asp:ListItem Value="2">Cost</asp:ListItem>
                                                                                        <asp:ListItem Value="4">Cost x2</asp:ListItem>
                                                                                        <asp:ListItem Value="3">Cost +20%</asp:ListItem>
                                                                                        <asp:ListItem Value="6">Cost +30%</asp:ListItem>
                                                                                        <asp:ListItem Value="7">Cost +50%</asp:ListItem>
                                                                                        <asp:ListItem Value="8">Cost +70%</asp:ListItem>
                                                                                        <asp:ListItem Value="5">Other</asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="8%" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true"
                                                                                        Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Ext. Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="txtViewEcon1" runat="server"
                                                                                        Text='<%# Eval("EconomicsCost") %>' Width="60px" AutoPostBack="True" OnTextChanged="Direct_calculation" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:ButtonField CommandName="Delete" Text="Delete" />
                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top" width="20%">
                                                    <b>Comments: </b></td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1"
                                                        TextMode="MultiLine" Width="600px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top" width="20%"><b>Total Due/Payment: </b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlTotalHeader" runat="server" Width="600px">
                                                        <asp:ListItem Value="1">ADDITIONAL CHARGE TOTAL FOR WORK DESCRIBED ABOVE</asp:ListItem>
                                                        <asp:ListItem Value="2">TOTAL CREDIT FOR WORK DESCRIBED ABOVE</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="left" valign="middle" colspan="2">
                                                    <table class="wrappermini" id="Table1" width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td align="middle">
                                                                    <!--StartFragment-->
                                                                    <table id="Table2" width="100%">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td class="nos">1</td>
                                                                                <td style="width: 76px">
                                                                                    <asp:TextBox ID="txtUponSignDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 26px">
                                                                                    <asp:ImageButton ID="imgUponSignDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td style="width: 164px">
                                                                                    <asp:TextBox ID="txtUponSignValue" runat="server" Text="Payment due upon signing" MaxLength="150" Width="190px" TabIndex="1"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 10px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpUponSign" runat="server" Width="34px" TabIndex="2">100</asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td style="width: 90px">
                                                                                    <asp:TextBox ID="txtnUponSign" runat="server" Width="72" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 178px">
                                                                                    <asp:TextBox ID="txtUponCompletionValue" runat="server" Text="Due upon completion of CO Work" Width="190px"
                                                                                        MaxLength="150" TabIndex="3"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 64px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpUponCompletion" runat="server" TabIndex="4" Width="33px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td style="width: 92px">
                                                                                    <asp:TextBox ID="txtnUponCompletion" runat="server" CssClass="cssTxtAmount" Width="73px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 76px">
                                                                                    <asp:TextBox ID="txtUponCompletionDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton ID="imgUponCompletionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">3</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="nos">2</td>
                                                                                <td style="height: 25px">
                                                                                    <asp:TextBox ID="txtBalanceDueDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <asp:ImageButton ID="imgBalanceDueDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <asp:TextBox ID="txtBalanceDue" runat="server" MaxLength="150" TabIndex="13" Text="Balance due at Completion" Width="190px"></asp:TextBox>
                                                                                </td>
                                                                                <td valign="top" align="left">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpBalanceDue" runat="server" TabIndex="14" Width="33px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtnBalanceDue" runat="server" CssClass="cssTxtAmount" Width="73px"></asp:TextBox>
                                                                                </td>
                                                                                <td align="right">Others
                                                                                    <asp:TextBox ID="txtOthers" runat="server" TabIndex="17" Width="138px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpOthers" runat="server" TabIndex="18" Width="33px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnOthers" runat="server" CssClass="cssTxtAmount" Width="73px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtOtherDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton ID="imgOtherDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">4</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="3" align="right">Total Percentage:
                                                                                </td>
                                                                                <td>
                                                                                    <asp:Label ID="lblPr" runat="server"></asp:Label>
                                                                                </td>
                                                                                <td colspan="2">
                                                                                    <asp:RadioButtonList ID="rdoCalc" runat="server" RepeatDirection="Horizontal" Width="250px">
                                                                                        <asp:ListItem Selected="True" Value="1">Calc based on %</asp:ListItem>
                                                                                        <asp:ListItem Value="2">Calc based on $</asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                                <td align="left" colspan="4">
                                                                                    <asp:Button ID="btnCalculate" runat="server" CausesValidation="False" CssClass="button"
                                                                                        OnClick="btnCalculate_Click" Text="Calculate" Width="100px" />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">&nbsp;
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:RadioButtonList ID="rdoList" runat="server" Height="32px" RepeatDirection="Horizontal">
                                                        <asp:ListItem Selected="True" Value="2">Show total only in Printed version</asp:ListItem>
                                                        <asp:ListItem Value="1">Show line-by-line pricing in Printed version</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">
                                                    <b>Sales Representative: </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblSalesRep" runat="server"></asp:Label>
                                                </td>
                                            </tr>


                                            <tr>
                                                <td align="left" width="20%"><b>Total: </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblProjectEcon" Width="75px" runat="server" Style="text-align: right;"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="width: 20%">&nbsp;</td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:CheckBox ID="ChkIsTax" runat="server" Checked="True"
                                                        Text="Tax Applied" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">
                                                    <b>Tax Rate: </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:TextBox ID="txtTaxPer" runat="server" Width="65px" AutoPostBack="True"
                                                        OnTextChanged="txtTaxPer_TextChanged" Style="text-align: right;"></asp:TextBox>
                                                    %</td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%"><b>Total with tax: </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblGtotal" Width="75px" runat="server" Style="text-align: right;"></asp:Label>
                                                </td>
                                            </tr>

                                        </table>

                                        <table align="center" border="0" cellpadding="0" cellspacing="0" width="100%" runat="server">
                                        </table>

                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoChangeOrderList" runat="server" CssClass="button"
                                            OnClick="btnGotoChangeOrderList_Click" Text="Change Order List" />
                                        <asp:Button ID="btnGotoCustomerList" runat="server"
                                            OnClick="btnGotoCustomerList_Click"
                                            Text="Go to Change Order Pricing" CssClass="button" />
                                        <asp:Button ID="btnCustomerDetails" runat="server"
                                            CssClass="button" OnClick="btnCustomerDetails_Click" Text="Customer Details" />
                                        &nbsp;<asp:Button ID="btnSave" runat="server" CssClass="button"
                                            OnClick="btnSave_Click" Text="Save" Width="80px" />
                                        <asp:Button ID="btnSavePopUp" runat="server" CssClass="button" Text="Save"
                                            Width="80px" Visible="False" OnClick="btnSave_Click" />
                                        <asp:Button ID="btnChangeOrder" runat="server" OnClick="btnChangeOrder_Click"
                                            Text="View in PDF" CssClass="button" />
                                        <asp:Button ID="btnbtnDocuSign" runat="server" CssClass="button" OnClick="btnbtnDocuSign_Click" Text="DocuSign" Width="164px" Visible="false" />
                                        <asp:Button ID="btnViewHTML" runat="server" CssClass="button"
                                            Text="View in HTML" OnClick="btnViewHTML_Click" Visible="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <cc1:CalendarExtender ID="UponSign" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgUponSignDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtUponSignDate">
                                        </cc1:CalendarExtender>
                                        <cc1:CalendarExtender ID="UponCompletion" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgUponCompletionDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtUponCompletionDate">
                                        </cc1:CalendarExtender>
                                        <cc1:CalendarExtender ID="BalanceDue" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgBalanceDueDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtBalanceDueDate">
                                        </cc1:CalendarExtender>
                                        <cc1:CalendarExtender ID="OtherDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgOtherDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtOtherDate">
                                        </cc1:CalendarExtender>

                                        <cc1:ModalPopupExtender ID="modUpdateCoEstimate" runat="server" PopupControlID="pnlUpdateCoEstimate" TargetControlID="lnkUpdateCoEstimate" BackgroundCssClass="modalBackground" DropShadow="false">
                                        </cc1:ModalPopupExtender>

                                    </td>
                                </tr>
                                <tr id="trPayterrm" runat="server" visible="false">
                                    <td align="left" width="20%"><b>Payment terms: 
                                        <asp:DropDownList ID="ddlTerms" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTerms_SelectedIndexChanged" Width="600px">
                                            <asp:ListItem Value="3">Payment due upon signing of this Change Order</asp:ListItem>
                                            <asp:ListItem Value="2">Balance due at Completion</asp:ListItem>
                                            <asp:ListItem Value="1">Payment due upon completion of work described on this Change Order</asp:ListItem>
                                            <asp:ListItem Value="6">No payment due, for internal use only</asp:ListItem>
                                            <asp:ListItem Value="7">Credit to be applied to final payment</asp:ListItem>
                                            <asp:ListItem Value="5">Other</asp:ListItem>
                                        </asp:DropDownList>
                                    </b></td>

                                </tr>

                                <tr>
                                    <td align="center"><b>
                                        <asp:Label ID="lblOther" runat="server" Visible="False">Other Payment Terms:
                                        </asp:Label>
                                    </b>
                                        <asp:TextBox ID="txtOtherTerms" runat="server" Visible="False" Width="486px"></asp:TextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <td align="center"></td>
                                </tr>

                                <tr>
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnItemCnt" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnChangeOrderView" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">
                                                    <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="btnSavePopUp" OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                                                    </cc1:ConfirmButtonExtender>
                                                    <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="btnSavePopUp" BackgroundCssClass="modalBackground" CancelControlID="btnCancel" OkControlID="btnOK" PopupControlID="pnlConfirmation" runat="server">
                                                    </cc1:ModalPopupExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSectionLevel" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                                    <cc1:CalendarExtender ID="ChangeOrderDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgCODate" TargetControlID="txtChangeOrderDate">
                                                    </cc1:CalendarExtender>
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnChPaymentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnChEstId" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnFileName" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False"
                                                        Value="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="rdoconfirm" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2"
                        DynamicLayout="False">
                        <ProgressTemplate>
                            <div class="overlay" />
                            <div class="overlayContent">
                                <p>
                                    Please wait while your data is being processed
                                </p>
                                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    <asp:Panel ID="pnlConfirmation" runat="server" BackColor="Snow" Height="100px"
                        Width="550px">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table align="center" cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="right">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Are you sure<b>?</b> </b></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b><b>Clicking &#39;Yes&#39; will freeze this Change Order.</b></b></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnOk" runat="server" CssClass="button" Text="Yes"
                                                Width="60px" />
                                            <asp:Button ID="btnCancel" runat="server" CssClass="button" Text="Cancel"
                                                Width="60px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <asp:Panel ID="pnlUpdateCoEstimate" runat="server" Width="550px" Height="260px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">
                                            <b>Update ChangeOrder Name</b></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="right" width="30%">
                                                        <b>ChangeOrder Name: </b></td>
                                                    <td align="left">
                                                        <asp:Label ID="lblExistingChangeOrderName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <b>New ChangeOrder Name: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtNewChangeOrderName" runat="server" TabIndex="1" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" TabIndex="2"
                                                            Width="80px" OnClick="btnSubmit_Click" CssClass="button" />
                                                        &nbsp;<asp:Button ID="btnClose" runat="server" Text="Close" TabIndex="3" Width="80px"
                                                            OnClick="btnClose_Click" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

                </div>
            </td>
        </tr>
    </table>
</asp:Content>
