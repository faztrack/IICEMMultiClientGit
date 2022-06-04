<%@ Page Title="Pre-Construction Checklist" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="PreconstructionCheckList.aspx.cs" Inherits="PreconstructionCheckList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
      <script>
            function DisplayWindow(cid) {
                window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
            }
      </script>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Pre-Construction Checklist</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
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
                                    <td align="left" valign="top">
                                        <table class="wrapper" width="100%" border="0" cellspacing="8" cellpadding="4" align="center">
                                            <tr>
                                                <td align="right" width="20%">
                                                    <b>Job Number:</b></td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblJobNumber" runat="server"></asp:Label>
                                                </td>
                                                <td align="right" width="15%" valign="top">&nbsp;</td>
                                                <td align="left">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" width="20%">
                                                    <b>Customer Name:</b> </td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                </td>
                                                <td align="right" width="15%" valign="top">
                                                    <strong>Address:</strong></td>
                                                <td align="left">
                                                    <table style="padding: 0px; margin-left: 10px;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAddress" runat="server"></asp:Label></td>
                                                            <td align="left" valign="top">&nbsp;
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" width="20%">
                                                    <strong>Phone:</strong></td>
                                                <td align="left" valign="middle" width="35%">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right" valign="top" width="15%"><strong>Project Status:</strong></td>
                                                <td align="left">

                                                    <asp:RadioButtonList ID="rdbEstimateIsActive" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rdbEstimateIsActive_SelectedIndexChanged" RepeatDirection="Horizontal">
                                                        <asp:ListItem Selected="True" Text="Active" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="InActive" Value="0"></asp:ListItem>
                                                    </asp:RadioButtonList>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Email:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right"><strong>Estimate Status:</strong></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True">
                                                        <asp:ListItem Value="1">Pending</asp:ListItem>
                                                        <asp:ListItem Value="2">Sit</asp:ListItem>
                                                        <asp:ListItem Value="3">Sold</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Label ID="lblTax_label" runat="server" Text="Tax % :"></asp:Label>
                                                    <asp:Label ID="lblTax" runat="server" Text="0" Width="15%"> </asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>Estimate Name:</strong></td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td align="right"><strong>Sales Person:</strong></td>
                                                <td align="left">
                                                    <asp:Label ID="lblSalesPerson" Style="margin-left: 10px;" runat="server"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <strong>
                                                        <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                    </strong></td>
                                                <td align="left" valign="top">
                                                    <asp:TextBox ID="txtSaleDate" runat="server" Visible="False"></asp:TextBox>
                                                </td>
                                                <td align="right">
                                                    <strong>Superintendent:</strong>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlSuperintendent" runat="server" AutoPostBack="True"
                                                        OnSelectedIndexChanged="ddlSuperintendent_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                    <asp:Button ID="btnUpdateSuperintendent" runat="server" CssClass="button"
                                                        OnClick="btnUpdateSuperintendent_Click" Text="Update" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="4">
                                                    <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                                </td>
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
                                    <td align="left">

                                        <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True"
                                            OnRowDataBound="grdGrouping_RowDataBound" OnRowCommand="grdGrouping_RowCommand" AutoGenerateColumns="False"
                                            CssClass="mGrid">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />
                                                        &nbsp;<asp:Label ID="lblLocNoteAsSold" runat="server" CssClass="noteDspl" />
                                                        <asp:Label ID="lblLocNote" runat="server" CssClass="noteDspl" />
                                                        <asp:LinkButton ID="lnkAddLocNotes" runat="server" CssClass="addNote" CommandName="AddNotes"
                                                            CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Text="Add Notes"></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkEditNotes" runat="server" CssClass="EditNote" CommandName="EditNotes"
                                                            CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Text="Edit Notes"></asp:LinkButton>
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                                            Width="100%" CssClass="mGrid">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="As Sold Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                        <asp:TextBox ID="txtshort_notes" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                            TextMode="MultiLine" />
                                                                        <asp:Label ID="lblshort_notes_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                        <asp:LinkButton ID="lnkOpen" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                    <HeaderStyle Width="28%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                        <asp:Label ID="lblDleted" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>

                                                                    <HeaderStyle Width="14%" />

                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Checklist Notes">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtshort_notes_New" runat="server" Text='<%# Eval("short_notes_new") %>'
                                                                            TextMode="MultiLine" />
                                                                        <asp:Label ID="lblCoTremId1" runat="server" Visible="false" Text='<%# Eval("co_pricing_item_id") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="18%" />
                                                                </asp:TemplateField>
                                                                 <asp:TemplateField HeaderText="flagged Item">
                                                                        
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkFlagItem" runat="server"  />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM">
                                                                    <HeaderStyle Width="8%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="quantity" HeaderText="Qty">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                        <asp:Label ID="lblT_price1" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_retail_price")) %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
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
                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" DataKeyNames="item_id"
                                                                        ShowFooter="True" OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        Width="100%" CssClass="mGrid">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="As Sold Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblshort_notes1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                    <asp:TextBox ID="txtshort_notes1" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                                        TextMode="MultiLine" />
                                                                                    <asp:Label ID="lblshort_notes1_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                                    <asp:LinkButton ID="lnkOpen1" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen1_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="15%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                                <HeaderStyle Width="28%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server"
                                                                                        Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                    <asp:Label ID="lblDleted1" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true"
                                                                                        Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <HeaderStyle Width="14%" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Checklist Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:TextBox ID="txtshort_notes_New2" runat="server" Text='<%# Eval("short_notes_new") %>'
                                                                                        TextMode="MultiLine" />
                                                                                    <asp:Label ID="lblCoTremId2" runat="server" Visible="false" Text='<%# Eval("co_pricing_item_id") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="18%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM">
                                                                                <HeaderStyle Width="8%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="quantity" HeaderText="Qty">
                                                                                <HeaderStyle Width="5%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Direct Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server" Text='<%# Eval("total_direct_price","{0:c}").ToString() %>' />
                                                                                    <asp:Label ID="lblT_price2" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_direct_price")) %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
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
                                                <td align="right" valign="top" width="15%">Comments:</td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1"
                                                        TextMode="MultiLine" Width="85%"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server"
                                            OnClick="btnGotoCustomerList_Click"
                                            Text="Go to Customer List" CssClass="button" />
                                        <asp:Button ID="btnSave" runat="server" CssClass="button" OnClick="btnSave_Click" Text="Save" />
                                        &nbsp;<asp:Button ID="btnPrintCheckList" runat="server" CssClass="button"
                                            OnClick="btnPrintCheckList_Click" Text="Pre-Construction Check List Report" />
                                         &nbsp;<asp:Button ID="btnEmailFlagItem" runat="server" CssClass="inputEmailBtn"
                                            OnClick="btnEmailFlagItem_Click" Text="Email flagged Item(s)" />
                                        <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                        <asp:LinkButton ID="lnkDummy4" runat="server"></asp:LinkButton>
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
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <%--<asp:HiddenField ID="hdnFinanceValue" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnIsCash" runat="server" Value="0" />--%>
                                                </td>
                                            </tr>
                                            <cc1:ModalPopupExtender ID="ModalPopupExtender5" TargetControlID="lnkDummy4" BackgroundCssClass="modalBackground"
                                                CancelControlID="btnLocMscCancel" PopupControlID="pnlLocationNotes"
                                                runat="server">
                                            </cc1:ModalPopupExtender>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="pnlLocationNotes" runat="server" Width="550px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">
                                            <b>Location Notes</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="right" width="30%">
                                                        <b>Location Name: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblLocationName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td align="left">
                                                        
                                                        <asp:TextBox ID="txtDisplay" runat="server" BackColor="Transparent" BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" CssClass="nostyle" Font-Bold="True" ReadOnly="True"></asp:TextBox>


                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top">
                                                        <b>Notes: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtLocationNotes" runat="server"
                                                            TextMode="MultiLine" Height="150px" Width="350px"
                                                            onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1000',document.getElementById('head_txtDisplay'));"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Label ID="lblLocationNotesMsg" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Button ID="btnLocMscSubmit" runat="server" Text="Submit" TabIndex="2" Width="80px"
                                                            OnClick="btnLocMscSubmit_Click" CssClass="button" />
                                                        &nbsp;
                                                        <asp:Button ID="btnLocMscCancel" runat="server" Text="Cancel" TabIndex="3" Width="80px"
                                                            CssClass="button" />
                                                        <asp:HiddenField ID="hdnLocId" runat="server" Value="0" />
                                                        <asp:HiddenField ID="hdnCOOrderExist" runat="server" Value="0" />
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
</asp:Content>
