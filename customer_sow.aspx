<%@ Page Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="customer_sow.aspx.cs" Inherits="customer_sow" Title="Composite SOW" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Composite SOW</span></td>
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
                                                <td align="right" width="20%">
                                                    <strong>Customer Name:</strong>&nbsp;</td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                </td>
                                                <td align="right" width="15%" valign="top"><strong>Projects:</strong>
                                                    &nbsp;</td>
                                                <td align="left">
                                                   <asp:DropDownList ID="ddlEstimate" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEstimate_SelectedIndexChanged">
                                                   </asp:DropDownList>
                                                     <asp:Label ID="lblCurrentEstimate" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" width="20%"><strong>Phone:</strong> </td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right" valign="top" width="15%"><strong>Address: </strong></td>
                                                <td align="left">
                                                    <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" width="20%">
                                                    <strong>Email:</strong>
                                                </td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right" valign="top" width="15%">&nbsp;
                                                </td>
                                                <td align="left">
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Estimate Name:</strong>
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td align="right"><strong>Estimate Status:</strong>&nbsp;
                                                &nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                     <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>
                                                    <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                    </strong>
                                                </td>
                                                <td align="left" valign="middle">
                                                     <asp:Label ID="lblSaleDateValue" runat="server" Visible="False"></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <strong>Tax % : </strong></td>
                                                <td align="left">&nbsp;                                                    
                                                    <asp:Label ID="lblTax" runat="server" Width="15%" Text="0"> </asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    &nbsp;</td>
                                                <td align="left" valign="top">
                                                    &nbsp;</td>
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left" valign="middle">&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table id="tblTotalProjectPrice" runat="server" cellpadding="8" cellspacing="0" style="border: 1px solid #c0c0c0;"
                                            align="center" width="60%">
                                            <tr>
                                                <td style="border: 1px solid #c0c0c0;" align="center">
                                                    <h3>Project Payment Information</h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" style="border: 1px solid #c0c0c0;">
                                                    <table style="width: 50%;">
                                                        <tr>
                                                            <td align="right">
                                                                <b>Contract Amount:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblProjectTotal" runat="server" Width="84px" Text="0" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <b>C/O Amount:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblTotalCOAmount" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <b>Total Amount (Contract+C/O):</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblTotalAmount" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <b>Total Received Amount:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblTotalRecievedAmount" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <b>Balance Due:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblTotalBalanceAmount" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:RadioButtonList ID="rdoSort" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdoSort_SelectedIndexChanged"
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
                                        <asp:GridView ID="grdGrouping" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                            OnRowDataBound="grdGrouping_RowDataBound" ShowFooter="True">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound" ShowFooter="True"
                                                            Width="100%">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderStyle-Width="6%" HeaderText="Item Id">
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderStyle-Width="5%" HeaderText="SL">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="12%" />
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Item Name">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblitem_name" runat="server" Text='<%# Eval("item_name") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="28%" />
                                                                </asp:TemplateField>
                                                                <%--  <asp:BoundField DataField="measure_unit" HeaderStyle-Width="6%" HeaderText="UoM" NullDisplayText=" ">
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:BoundField>--%>
                                                               <%-- <asp:BoundField DataField="quantity" HeaderStyle-Width="5%" HeaderText="Code">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>--%>
                                                               <%-- <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price") %>' />
                                                                        <asp:Label ID="lblT_price1" runat="server" Visible="false" Text='<%# Eval("total_retail_price") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                </asp:TemplateField>--%>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                        <asp:Label ID="lblT_price1" runat="server" Visible="false" Text='<%# Eval("total_retail_price") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="12%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="tmpCo" HeaderStyle-Width="8%" HeaderText="Item Status">
                                                                    <HeaderStyle Width="8%" />
                                                                </asp:BoundField>
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
                                                        <asp:Label ID="lblDirectPricingHeader" runat="server" Text="The following items are Direct / Outsourced"
                                                            Visible="False"></asp:Label></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" colspan="2" valign="top" width="15%">
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" CaptionAlign="Top"
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound" ShowFooter="True">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                                    &nbsp;<asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                                        CssClass="mGrid" DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        ShowFooter="True" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderStyle-Width="6%" HeaderText="Item Id" />
                                                                            <asp:BoundField DataField="section_serial" HeaderStyle-Width="5%" HeaderText="SL" />
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Item Name">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblitem_name2" runat="server" Text='<%# Eval("item_name") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="28%" />
                                                                            </asp:TemplateField>
                                                                           <%-- <asp:BoundField DataField="measure_unit" HeaderStyle-Width="6%" HeaderText="UoM" NullDisplayText=" " />--%>
                                                                           <%-- <asp:BoundField DataField="quantity" HeaderStyle-Width="5%" HeaderText="Code" />--%>
                                                                           <%-- <asp:TemplateField HeaderText="Direct Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server" Text='<%# Eval("total_direct_price") %>' />
                                                                                    <asp:Label ID="lblT_price2" runat="server" Visible="false" Text='<%# Eval("total_direct_price") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:TemplateField>--%>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblshort_notes2" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                    <asp:Label ID="lblT_price2" runat="server" Visible="false" Text='<%# Eval("total_direct_price") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="tmpCo" HeaderStyle-Width="8%" HeaderText="Item Status">
                                                                                <HeaderStyle Width="8%" />
                                                                            </asp:BoundField>
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
                                                <td align="right" valign="top" width="15%">Comments:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1" TextMode="MultiLine"
                                                        Width="603px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server" OnClick="btnGotoCustomerList_Click"
                                            Text="Back to Dashboard" CssClass="button" />
                                        &nbsp;<asp:Button ID="btnPrintByLoc" runat="server" CssClass="button" OnClick="btnPrintByLoc_Click"
                                            Text="Print SOW by Location" Width="164px" />
                                        <asp:Button ID="btnPrintBySec" runat="server" CssClass="button" OnClick="btnPrintBySec_Click"
                                            Text="Print SOW by Section" Width="164px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                </td>
                                                <td align="right">&nbsp;
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
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                                </td>
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
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
                </div>
            </td>
        </tr>
    </table>
</asp:Content>


