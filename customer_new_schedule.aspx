﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customer_new_schedule.aspx.cs" Inherits="customer_new_schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="JavaScript" type="text/JavaScript">

        function DisplayWindow4() {
            window.open('jobdesc_popup.aspx?jsid=4&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">
                            <asp:Label ID="lbltopHead" runat="server" Text="" CssClass="cssTitleHeader"></asp:Label></span></td>
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
                                                <td align="right" width="20%">
                                                    <strong>Phone:</strong></td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right" valign="top" width="15%">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Email:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">
                                                    <strong>Estimate Status:</strong></td>
                                                <td align="left">
                                                    <asp:Label ID="lblEstStatus" runat="server"></asp:Label>
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True"
                                                        Visible="False">
                                                        <asp:ListItem Value="1">Pending</asp:ListItem>
                                                        <asp:ListItem Value="2">Sit</asp:ListItem>
                                                        <asp:ListItem Value="3">Sold</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Estimate Name:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td align="right">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>
                                                        <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                    </strong></td>
                                                <td align="left" valign="top">
                                                    <asp:Label ID="lblDateOfSale" runat="server"></asp:Label>
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
                                        <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False"
                                            CssClass="mGrid" DataKeyNames="item_id"
                                            OnRowDataBound="grdSelectedItem_RowDataBound"
                                            OnRowEditing="grdSelectedItem_RowEditing"
                                            OnRowUpdating="grdSelectedItem1_RowUpdating" ShowFooter="False"
                                            Width="100%">
                                            <Columns>
                                                <asp:BoundField DataField="item_id" HeaderText="Item Id">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="section_serial" HeaderText="SL">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHeader1" runat="server" Text="Section" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemName" runat="server"
                                                            Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="12%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label ID="lblHeader" runat="server" Text="Location" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemName" runat="server"
                                                            Text='<%# Eval("location_name").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="12%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                    <HeaderStyle Width="24%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" ">
                                                    <HeaderStyle Width="7%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="quantity" HeaderText="Code">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Ext. Price">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal_price" runat="server"
                                                            Text='<%# Eval("total_retail_price") %>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="7%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                    <HeaderStyle Width="1%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes">
                                                    <HeaderStyle Width="16%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Unit Of Execution">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtUnitExe" runat="server"
                                                            Text='<%# Eval("execution_unit") %>' Width="40px" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="8%" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit" Visible="false">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                            CssClass="mGrid" DataKeyNames="item_id"
                                            OnRowDataBound="grdSelectedItem2_RowDataBound"
                                            OnRowEditing="grdSelectedItem2_RowEditing"
                                            OnRowUpdating="grdSelectedItem2_RowUpdating" ShowFooter="False" Width="100%">
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
                                                    <HeaderStyle Width="12%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                    <HeaderStyle Width="24%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" ">
                                                    <HeaderStyle Width="7%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="quantity" HeaderText="Code">
                                                    <HeaderStyle Width="5%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Direct Price">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal_price2" runat="server"
                                                            Text='<%# Eval("total_direct_price") %>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="7%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                    <HeaderStyle Width="1%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes">
                                                    <HeaderStyle Width="16%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Unit Of Execution">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtUnitExe1" runat="server"
                                                            Text='<%# Eval("execution_unit") %>' Width="40px" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="8%" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit" Visible="false">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
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
                                                <td align="center">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Label ID="lblResult2" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server"
                                            OnClick="btnGotoCustomerList_Click"
                                            Text="Go to Customer List" CssClass="button" CausesValidation="False" />
                                        <asp:Button ID="btnGotoPricing" runat="server"
                                            CausesValidation="False" CssClass="button" OnClick="btnGotoPricing_Click"
                                            Text="Go to Pricing" />
                                        &nbsp;<asp:Button ID="btnSaveUnit" runat="server"
                                            OnClick="btnSaveUnit_Click" Text="Save Unit of Execution"
                                            CssClass="button" CausesValidation="False" />
                                        <asp:Button ID="btnPrev" runat="server" CssClass="button" Text="Preview" />
                                    </td>
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
                                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
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
                    <asp:Button ID="Button1"
                        runat="server" OnClick="Button1_Click" Text="Button" />

                </div>
            </td>
        </tr>
    </table>
</asp:Content>
