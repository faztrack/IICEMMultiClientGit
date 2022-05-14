<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="PublicMe_Pricing.aspx.cs" Inherits="PublicMe_Pricing" Title="Estimation Template Pricing" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Estimation Template Pricing</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 100%">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table width="100%" border="0" cellspacing="4" cellpadding="4" align="center">
                                <tr>
                                    <td align="left" valign="top">
                                        <table width="100%" border="0" cellspacing="8" cellpadding="4" align="center">
                                            <tr>
                                                <td align="right" style="width: 232px">&nbsp;</td>
                                                <td align="left" valign="middle">
                                                    <asp:CheckBox ID="chkIsPublic" runat="server" Checked="True" TabIndex="14"
                                                        Text="Public" />
                                                    &nbsp;(Viewable to Others)</td>
                                                <td align="left" colspan="1">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 232px">
                                                    <b>Estimate Name:</b></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblModelEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                    &nbsp;</td>
                                                <td align="left" colspan="1">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table id="tblTotalProjectPrice" visible="false" runat="server" cellpadding="8" cellspacing="0" style="border: 1px solid #c0c0c0;" align="center" width="60%">
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
                                        <asp:RadioButtonList ID="rdoSort" runat="server" AutoPostBack="True"
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
                                            CssClass="mGrid" OnRowCommand="grdGrouping_RowCommand">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />
                                                         &nbsp;&nbsp;<asp:LinkButton ID="InkAssign" runat="server"  CssClass="lnkBtn" CommandName="assigntocustomer" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Style="margin-right: 50px;">ASSIGN THIS LOCATION</asp:LinkButton>
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound" CssClass="mGrid">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
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

                                                                    <HeaderStyle Width="11%" />

                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" ">
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblquantity" runat="server" Text='<%# Eval("quantity") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                    Visible="False">
                                                                    <HeaderStyle Width="1%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                        <asp:TextBox ID="txtshort_notes" runat="server" Visible="false"
                                                                            Text='<%# Eval("short_notes") %>' Width="110px" Height="40px" TextMode="MultiLine" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="20%" />
                                                                </asp:TemplateField>
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
                                                                    &nbsp;<asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                                        CssClass="mGrid" DataKeyNames="item_id"
                                                                        OnRowDataBound="grdSelectedItem2_RowDataBound">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
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
                                                                                <HeaderStyle Width="11%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                                <HeaderStyle Width="25%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" ">
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblquantity1" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Direct Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server"
                                                                                        Text='<%# Eval("total_direct_price") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                                Visible="False">
                                                                                <HeaderStyle Width="1%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblshort_notes1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                    <asp:TextBox ID="txtshort_notes1" runat="server" Visible="false"
                                                                                        Text='<%# Eval("short_notes") %>' Width="110px" Height="40px" TextMode="MultiLine" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="20%" />
                                                                            </asp:TemplateField>

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
                                                <td align="right" valign="top" style="width: 1%">
                                                    <b>Comments: </b></td>
                                                <td>
                                                    <asp:Label ID="lblComments" runat="server"></asp:Label>
                                                </td>
                                            </tr>
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

                                        <asp:Button ID="btnCopyEstimate" runat="server" CssClass="button"
                                            OnClick="btnCopyEstimate_Click" Text="Copy to your Templates" />

                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top"></td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top"></td>
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
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnModEstimateNewId" runat="server"
                                                        EnableViewState="False" Value="0" />
                                                    <asp:HiddenField ID="hdnSalesId" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top"></td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2" DynamicLayout="False">
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

                </div>
            </td>
        </tr>
    </table>
</asp:Content>

