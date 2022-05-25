<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="change_order_worksheet_readonly.aspx.cs" Inherits="change_order_worksheet_readonly" Title="Change Order WorkSheet" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }

    </script>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Change Order WorkSheet</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                        <td align="right" style="padding-right: 30px; float: right;">
                            <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
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
                                                <td align="left" width="20%">
                                                    <b>Customer Name:</b> </td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                </td>
                                                <td align="left" width="15%" valign="top">
                                                    <b>Address: </b></td>
                                                <td align="left">
                                                    <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" width="20%">&nbsp;</td>
                                                <td align="left" valign="middle" width="35%">&nbsp;</td>
                                                <td align="right" valign="top" width="15%">&nbsp;</td>
                                                <td align="left">
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server"
                                                        ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Phone: </b></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="left">
                                                    <b>Email: </b></td>
                                                <td align="left">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Estimate Name: </b></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                    &nbsp;</td>
                                                <td align="left">
                                                    <b>C/O Status: </b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" Enabled="False">
                                                        <asp:ListItem Value="1">Draft</asp:ListItem>
                                                        <asp:ListItem Value="2">Pending</asp:ListItem>
                                                        <asp:ListItem Value="3">Executed</asp:ListItem>
                                                        <asp:ListItem Value="4">Declined</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Change Order Name: </b></td>
                                                <td align="left">
                                                    <asp:Label ID="lblChangeOrderName" runat="server"></asp:Label>
                                                     <asp:LinkButton ID="lnkUpdateCoEstimate" runat="server"><span 
                                                        style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Rename</span></asp:LinkButton>
                                                </td>
                                                <td align="left"><b>C/O Type: </b></td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddlChangeOrderType" runat="server" AutoPostBack="True" Enabled="False">
                                                        <asp:ListItem Value="1">Change Order</asp:ListItem>
                                                        <asp:ListItem Value="2">Clarification</asp:ListItem>
                                                        <asp:ListItem Value="3">Internal Use Only</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>
                                                        <asp:Label ID="lblCoDate" runat="server" Text="Change Order Date:"></asp:Label>
                                                    </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtChangeOrderDate" runat="server"></asp:TextBox>
                                                </td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left" valign="middle">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">
                                                    <b>Notes: </b></td>
                                                <td align="left" colspan="3">
                                                    <asp:TextBox ID="txtNotes1" runat="server" Height="44px" TabIndex="1"
                                                        TextMode="MultiLine" Width="603px" ReadOnly="True"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:Label ID="lblMessagefinal" runat="server" Font-Bold="True"></asp:Label>
                                                    &nbsp;                  &nbsp;                  &nbsp;&nbsp;</td>
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
                                                            Width="100%" CssClass="mGrid">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" HeaderStyle-Width="6%" />
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" HeaderStyle-Width="7%" />
                                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes" HeaderStyle-Width="27%" />
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="12%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name" HeaderStyle-Width="28%" />
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
                                                                <asp:TemplateField HeaderText="Markup">
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="ddlEconomics" runat="server" AutoPostBack="True"
                                                                            SelectedValue='<%# Eval("EconomicsId") %>'
                                                                            Width="90px" Enabled="False">
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
                                                                            Text='<%# Eval("EconomicsCost") %>' Width="60px" CssClass="cssTxtAmount">0</asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="6%" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

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
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
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
                                                                        ShowFooter="True" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" HeaderStyle-Width="6%" />
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" HeaderStyle-Width="7%" />
                                                                            <asp:BoundField DataField="short_notes" HeaderText="Short Notes" HeaderStyle-Width="27%" />
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
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name" HeaderStyle-Width="28%" />
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
                                                                                        Width="90px" Enabled="False">
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
                                                                                        Text='<%# Eval("EconomicsCost") %>' Width="60px"></asp:TextBox>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:TemplateField>
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
                                                        TextMode="MultiLine" Width="603px" ReadOnly="True"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="20%">
                                                    <b>Total Due/Payment: </b>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:DropDownList ID="ddlTotalHeader" runat="server" Enabled="False" Width="610px">
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
                                                                                    <asp:TextBox ID="txtUponSignValue" runat="server" Text="Payment due upon signing" MaxLength="150" Width="190px" TabIndex="1" ReadOnly="True"></asp:TextBox>
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
                                                                                        MaxLength="150" TabIndex="3" ReadOnly="True"></asp:TextBox>
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
                                                                                    <asp:TextBox ID="txtBalanceDue" runat="server" MaxLength="150" TabIndex="13" Text="Balance due at Completion" Width="190px" ReadOnly="True"></asp:TextBox>
                                                                                </td>
                                                                                <td valign="top" align="left">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpBalanceDue" runat="server" TabIndex="14" Width="33px" ReadOnly="True"></asp:TextBox>
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
                                                <td align="left"><b>
                                                    <asp:Label ID="lblRate1" runat="server" Text="Tax Rate:"></asp:Label>
                                                </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblTax" Width="75px" Style="text-align: right;" runat="server">0</asp:Label>
                                                    <asp:Label ID="lblPer" runat="server" Text="%"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left"><b>
                                                    <asp:Label ID="lblTotal1" runat="server" Text="Total with tax:"></asp:Label>
                                                </b></td>
                                                <td style="padding-left: 8px;" align="left" valign="middle">
                                                    <asp:Label ID="lblGtotal" Width="75px" Style="text-align: right;" runat="server"></asp:Label>
                                                </td>
                                            </tr>

                                        </table>

                                        <table id="Table3" align="center" border="0" cellpadding="0" cellspacing="0" width="100%" runat="server">
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
                                            Text="Go to Customer List" CssClass="button" />
                                        <asp:Button ID="btnCustomerDetails" runat="server"
                                            CssClass="button" OnClick="btnCustomerDetails_Click" Text="Customer Details" />
                                        &nbsp;<asp:Button ID="btnCOReport" runat="server" CssClass="button"
                                            OnClick="btnCOReport_Click" Text="View in PDF" />
                                        <asp:Button ID="btnViewHTML" runat="server" Text="View in HTML"
                                            CssClass="button" OnClick="btnViewHTML_Click" Visible="False" />
                                        <cc1:ModalPopupExtender ID="modUpdateCoEstimate" runat="server" PopupControlID="pnlUpdateCoEstimate" TargetControlID="lnkUpdateCoEstimate" BackgroundCssClass="modalBackground" DropShadow="false">
                                        </cc1:ModalPopupExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;</td>
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
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnItemCnt" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
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
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnChEstId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnChPaymentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False"
                                                        Value="0" />
                                                    <asp:HiddenField ID="hdnChangeOrderView" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
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
