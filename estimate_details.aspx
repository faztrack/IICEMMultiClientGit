<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="estimate_details.aspx.cs" Inherits="estimate_details" Title="Estimate Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Estimate Details</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 900px">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table width="900" border="0" cellspacing="4" cellpadding="4" align="center">
                                <tr>
                                    <td align="left" valign="top">
                                        <table width="100%" border="0" cellspacing="8" cellpadding="4" align="center">
                                            <tr>
                                                <td align="right" width="20%">
                                                    <b>Customer Name:</b> </td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                </td>
                                                <td align="right" width="15%" valign="top">
                                                    <strong>Address:                    </strong></td>
                                                <td align="left">
                                                    <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Phone:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <strong>Email:</strong></td>
                                                <td align="left">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Estimate Name:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <strong>Estimate Status:</strong></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True">
                                                        <asp:ListItem Value="1">Pending</asp:ListItem>
                                                        <asp:ListItem Value="2">Sit</asp:ListItem>
                                                        <asp:ListItem Value="3">Sold</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>
                                                        <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                    </strong></td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtSaleDate" runat="server" Visible="False"></asp:TextBox>
                                                </td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left" valign="middle">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table id="tblTotalProjectPrice" runat="server" cellpadding="8" cellspacing="0" style="border: 1px solid #c0c0c0;" align="center" width="60%">
                                            <tr>
                                                <td colspan="3" style="border: 1px solid #c0c0c0;" align="center">
                                                    <h3>Total Project 
                  Price</h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top"><strong>Total Price                </strong></td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top"><strong>Direct Price</strong></td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top"><strong>Total Price +&nbsp; Direct Price</strong></td>
                                            </tr>
                                            <tr>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <asp:Label ID="lblRetailTotalCost" runat="server"></asp:Label>
                                                </td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <asp:Label ID="lblDirctTotalCost" runat="server"></asp:Label>
                                                </td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <asp:Label ID="lblGrandTotalCost" runat="server"></asp:Label>
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
                                            <asp:Label ID="lblRetailPricingHeader" runat="server" Text="Selected Item" Visible="false"></asp:Label></h3>

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
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound" Width="100%" CssClass="mGrid">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" />
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" />
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>

                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name" />
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" "/>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" />
                                                                <asp:BoundField DataField="quantity" HeaderText="Qty" />
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                    Visible="False" />
                                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes" />

                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <%# GetTotalPrice()%>
                                                    </FooterTemplate>
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
                                                                        OnRowDataBound="grdSelectedItem2_RowDataBound" ShowFooter="True" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" />
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" />
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server"
                                                                                        Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true"
                                                                                        Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name" />
                                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" "/>
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" />
                                                                            <asp:BoundField DataField="quantity" HeaderText="Qty" />
                                                                            <asp:TemplateField HeaderText="Direct Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server"
                                                                                        Text='<%# Eval("total_direct_price") %>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                                Visible="False" />
                                                                            <asp:BoundField DataField="short_notes" HeaderText="Short Notes" />
                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <%# GetTotalPriceDirect()%>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top" width="15%">Comments:</td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1"
                                                        TextMode="MultiLine" Width="603px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server"
                                            OnClick="btnGotoCustomerList_Click" Text="Go to Customer List" />
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;</td>
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
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>


                </div>
            </td>
        </tr>
    </table>
</asp:Content>

