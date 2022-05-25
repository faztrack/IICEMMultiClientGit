<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="composite_sow.aspx.cs"
    Inherits="composite_sow" Title="Composite SOW" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function DisplayEmailWindow(FileName) {

            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&sfn=' + FileName, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

        }
        function selected_ItemName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }

        function SearchItemNamePress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;

        }
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>

    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Composite SOW (Retail)</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
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
                                                            <td width="74px">
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
                                                            <td style="width: 113px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Phone: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Email: </b></td>
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
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
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
                                        <table class="wrapper" width="100%">
                                            <tr>
                                                <td style="width: 220px; border-right: 1px solid #ddd;" align="left" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td width="74px">
                                                                <img src="images/icon-estimate-info.png" /></td>
                                                            <td align="left">
                                                                <h2>Estimate<br />
                                                                    Information</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 390px;" align="left" valign="top">
                                                    <table style="width: 390px;">
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>Estimate Name:</b> </td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>
                                                                <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                            </b></td>
                                                            <td style="width: auto;">
                                                                <asp:TextBox ID="txtSaleDate" runat="server" Visible="False"></asp:TextBox>
                                                            </td>
                                                        </tr>

                                                    </table>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table style="width: 420px;">
                                                        <tr>
                                                            <td style="width: 110px;" align="left" valign="top"><b>Estimate Status:</b> </td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <table style="padding: 0px; margin: -4px;">
                                                                    <tr style="padding: 0px; margin: 0px;">
                                                                        <td style="padding: 0px; margin: 0px;">
                                                                            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True">
                                                                                <asp:ListItem Value="1">Pending</asp:ListItem>
                                                                                <asp:ListItem Value="2">Sit</asp:ListItem>
                                                                                <asp:ListItem Value="3">Sold</asp:ListItem>
                                                                            </asp:DropDownList></td>
                                                                        <td style="padding: 0px; margin: 0px;"><b>Tax % :</b>
                                                                            <asp:Label ID="lblTax" runat="server" Text="0" Width="15%"> </asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: auto;" align="right" valign="middle">
                                                                <b>
                                                                    <asp:Label ID="lblPm" runat="server" Text="PM Disp:"></asp:Label></b></td>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <asp:CheckBox ID="chkPMDisplay" runat="server" AutoPostBack="True" OnCheckedChanged="chkPMDisplay_CheckedChanged" /></td>
                                                        </tr>

                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
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
                                                        <tr>
                                                            <td align="right">
                                                                <b>Running Item Total:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblRunning" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr id="trIncentive" runat="server" visible="false">
                                                            <td align="right">
                                                                <b>Total Incentives:</b>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblIncentive" runat="server" Text="0" Width="84px" CssClass="cssLblAmount"></asp:Label>
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
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr style="padding: 0px; margin: 0px;">
                                                <td style="padding: 0px; margin: 0px;">
                                                    <asp:RadioButtonList ID="rdoSort" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdoSort_SelectedIndexChanged"
                                                        RepeatDirection="Horizontal">
                                                        <asp:ListItem Selected="True" Value="1">View by Locations</asp:ListItem>
                                                        <asp:ListItem Value="2">View by Sections</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td style="padding: 0px; margin: 0px;">&nbsp;</td>
                                                <td style="padding: 0px; margin: 0px;">
                                                    <asp:HyperLink ID="hyp_SowCost" runat="server">View SOW (Cost)</asp:HyperLink>
                                                </td>
                                                <td style="padding: 0px; margin: 0px;">&nbsp;</td>
                                                
                                                <td style="padding-left: 10px;" align="left" valign="middle">
                                                    <asp:ImageButton ID="btnExpCostLocList" runat="server" CssClass="imageBtn" ImageUrl="~/images/export_csv.png" OnClick="btnExpCostLocList_Click" ToolTip="Composite SOW in CSV" />
                                                </td>
                                                
                                                <td style="padding: 0px; margin: 0px;">
                                                    <asp:Button ID="btnPrintWOCost" runat="server" CssClass="button" OnClick="btnPrintWOCost_Click" Text="Print SOW w/o Cost" Width="160px" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <asp:TextBox ID="txtSearchItemName" onkeypress="return SearchItemNamePress(event);" runat="server" Width="30%" Style="margin-left: 5px;"></asp:TextBox>
                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemName" ServiceMethod="GetItemName" TargetControlID="txtSearchItemName" UseContextKey="True">
                                        </cc1:AutoCompleteExtender>
                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearchItemName" WatermarkText="Search by Item Name" />
                                        <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" />
                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblResult" runat="server"></asp:Label></td>
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
                                            OnRowDataBound="grdGrouping_RowDataBound" OnRowCommand="grdGrouping_RowCommand" ShowFooter="True">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                        <asp:LinkButton ID="lnkSendEmail1" runat="server" CssClass="sendEmail" CommandName="sEmail" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton>
                                                        &nbsp;&nbsp;<asp:LinkButton ID="inkProjectNotes" runat="server" CssClass="projectNotes" CommandName="projectNotes" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Style="margin-right: 5px;">Project Notes</asp:LinkButton>
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
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshNote" runat="server" Text='<%# Eval("short_notes").ToString() %>'></asp:Label>
                                                                        <asp:Label ID="lblshort_notes_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                        <asp:LinkButton ID="lnkOpen" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>

                                                                    <HeaderStyle Width="11%" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                        <asp:Label ID="lblDleted" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="12%" />
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Item Name">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemNameDetail" runat="server" Text='<%# Eval("item_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="25%" />

                                                                </asp:TemplateField>
                                                                <%-- <asp:BoundField DataField="item_name" HeaderStyle-Width="28%" HeaderText="Item Name">
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:BoundField>--%>
                                                                <asp:BoundField DataField="measure_unit" HeaderStyle-Width="6%" HeaderText="UoM" NullDisplayText=" ">
                                                                    <HeaderStyle Width="6%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Unit Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblTotalUnitCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                        <asp:Label ID="lblUnit_Cost1" runat="server" Visible="false" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_cost" HeaderStyle-Width="5%" HeaderText="Unit Price"
                                                                    Visible="false">
                                                                    <HeaderStyle Width="1%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Code">

                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("quantity").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="4%" />
                                                                    <ItemStyle HorizontalAlign="Right" />

                                                                </asp:TemplateField>
                                                                <%--  <asp:BoundField DataField="quantity" HeaderStyle-Width="5%" HeaderText="Code" ItemStyle-HorizontalAlign="Right">
                                                                    <HeaderStyle Width="4%" />
                                                                </asp:BoundField>--%>

                                                                <asp:TemplateField HeaderText="Labor Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLaborCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalLabor_Cost1" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_labor_cost")) %>' />
                                                                        <asp:Label ID="lblTotalLabor_Cost" runat="server" Text='<%# Eval("total_labor_cost","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Ext. Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_Cost" runat="server" Text='<%# Eval("total_unit_cost","{0:c}").ToString() %>' />
                                                                        <asp:Label ID="lblTotal_Cost1" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_unit_cost")) %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="8%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Ext. Price" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# String.Format("{0:C}", Eval("total_retail_price")) %>' />
                                                                        <asp:Label ID="lblT_price1" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_retail_price")) %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="short_notes_new" HeaderText="Checklist Notes" ItemStyle-HorizontalAlign="Left">
                                                                    <HeaderStyle Width="8%" />
                                                                </asp:BoundField>


                                                                <asp:BoundField DataField="tmpCo" HeaderStyle-Width="8%" HeaderText="Item Status" ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderStyle Width="8%" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <table style="padding: 0px; margin: 0px; width: 99.8%">
                                                            <tr>
                                                                <td style="width: 6%;">&nbsp;</td>
                                                                <td style="width: 5%;">&nbsp;</td>
                                                                <td style="width: 11%;">&nbsp;</td>
                                                                <td style="width: 12%;">&nbsp;</td>
                                                                <td style="width: 25%; text-align: right;">
                                                                    <asp:Label ID="Label1" runat="server" Text='<%# GetTotalPrice()%>' /></td>
                                                                <td style="width: 6%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalLabel" runat="server" Text="Grand Total:" /></td>
                                                                <asp:Panel ID="pnlAdminView" runat="server" Visible="false">
                                                                    <td style="width: 5%; text-align: right;" id="UnitCostTotal" runat="server">
                                                                        <asp:Label ID="lblGrandTotalUnitCost" runat="server" Text='<%# GetTotalUnitCost()%>' /></td>
                                                                    <td style="display: none; width: 1%;"></td>
                                                                    <td style="width: 4%;">&nbsp;</td>
                                                                    <td style="width: 7%; text-align: right;" id="LaborCostTotal" runat="server">
                                                                        <asp:Label ID="lblGtrandTotalLaborCost" runat="server" Text='<%# GetTotalLaborCost()%>' /></td>
                                                                     
                                                                </asp:Panel>
                                                                <asp:Panel ID="pnlSuperView" runat="server" Visible="false">

                                                                    <td style="display: none; width: 1%;"></td>
                                                                    <td style="width: 10%;">&nbsp;</td>

                                                                </asp:Panel>

                                                                <td style="width: 8%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalExtCost" runat="server" Text='<%# GetTotalExtCost()%>' /></td>
                                                                 <asp:Panel ID="pnlAdminView2" runat="server" Visible="false">
                                                                <td style="width: 7%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalExtPrice" runat="server" Text='<%# GetTotalExPrice()%>' /></td>
                                                                </asp:Panel>
                                                                <td style="width: 8%; color: #646464; font-size: 11px;">CHECKLIST NOTES</td>
                                                                <td style="width: 8%; color: #646464; font-size: 12px;">ITEM STATUS</td>
                                                            </tr>
                                                        </table>
                                                        <%--  <%# GetTotalPrice()%>--%>
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
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound" OnRowCommand="grdGroupingDirect_RowCommand" ShowFooter="True">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                                    <asp:LinkButton ID="lnkSendEmail2" runat="server" CssClass="sendEmail" CommandName="sEmail2" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton>
                                                                    &nbsp;&nbsp;
                                                                    <asp:LinkButton ID="inkProjectNotes2" runat="server" CssClass="projectNotes" CommandName="projectNotes2" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Project Notes</asp:LinkButton>
                                                                    &nbsp;<asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                                        CssClass="mGrid" DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        ShowFooter="True" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderStyle-Width="6%" HeaderText="Item Id" />
                                                                            <asp:BoundField DataField="section_serial" HeaderStyle-Width="5%" HeaderText="SL" />
                                                                            <asp:BoundField DataField="short_notes" HeaderStyle-Width="12%" HeaderText="Short Notes" />
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                    <asp:Label ID="lblDleted2" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderStyle-Width="28%" HeaderText="Item Name" />
                                                                            <asp:BoundField DataField="measure_unit" HeaderStyle-Width="6%" HeaderText="UoM" NullDisplayText=" " />
                                                                            <asp:BoundField DataField="item_cost" HeaderStyle-Width="5%" HeaderText="Unit Price"
                                                                                Visible="false" />
                                                                            <asp:TemplateField HeaderText="Unit Cost">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblUnit_Cost3" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="quantity" HeaderStyle-Width="5%" HeaderText="Code" />
                                                                            <asp:TemplateField HeaderText="Labor Cost">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLaborCost3" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotalLabor_Cost3" runat="server" Text='<%# Eval("total_labor_cost","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Ext. Cost">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalCost3" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_Cost3" runat="server" Text='<%# Eval("total_unit_cost","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="8%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Direct Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblTotal_price2" runat="server" Text='<%# String.Format("{0:C}", Eval("total_direct_price")) %>' />
                                                                                    <asp:Label ID="lblT_price2" runat="server" Visible="false" Text='<%# String.Format("{0:C}", Eval("total_direct_price")) %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="labor_rate" HeaderStyle-Width="1%" HeaderText="Labor Rate"
                                                                                Visible="False" />

                                                                            <asp:BoundField DataField="tmpCo" HeaderStyle-Width="8%" HeaderText="Item Status">
                                                                                <HeaderStyle Width="8%" />
                                                                            </asp:BoundField>
                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <table style="padding: 0px; margin: 0px; width: 99.8%">
                                                                        <tr>
                                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 14%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 22%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 11%;">
                                                                                <asp:Label ID="Label2" runat="server" Text='<%# GetTotalPriceDirect()%>' /></td>
                                                                            </td>
                                                                            <td style="padding: 0px; margin: 0px; width: 8%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 7%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                                <asp:Label ID="lblGtotalLabel2" runat="server" Text="Grand Total:" /></td>
                                                                            </td>
                                                                            <td style="padding: 0px; margin: 0px; width: 8%; text-align: right;">
                                                                                <asp:Label ID="lblGtotalExtCost2" runat="server" Text='<%# GetTotalExtCostDirect()%>' /></td>
                                                                            </td>
                                                                            <td style="padding: 0px; margin: 0px; width: 8%; text-align: right;">
                                                                                <asp:Label ID="lblGtotalExtPrice2" runat="server" Text='<%# GetTotalExPriceDirect()%>' /></td>

                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                    <%--   <%# GetTotalPriceDirect()%>--%>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td class="cssHeader" align="center" colspan="2">
                                                    <table width="100%" cellspacing="0" cellpadding="0">
                                                        <tbody>
                                                            <tr>
                                                                <td align="left">
                                                                    <span class="titleNu">
                                                                        <span id="spnPnlContact" class="cssTitleHeader">
                                                                            <asp:ImageButton ID="ImageContactMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="112" />
                                                                            <font style="font-size: 16px; cursor: pointer;"> General Project Notes</font>
                                                                        </span>

                                                                    </span>
                                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server" ImageControlID="ImageContactMain" CollapseControlID="spnPnlContact"
                                                                        ExpandControlID="spnPnlContact" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGIContactMain" Collapsed="True">
                                                                    </cc1:CollapsiblePanelExtender>
                                                                </td>
                                                                <td align="right"></td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Panel ID="pnlTGIContactMain" runat="server" Height="100%">
                                                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">


                                                            <tr>
                                                                <td align="center" colspan="2">
                                                                    <asp:Panel ID="pnlTGI1" runat="server" Height="100%">
                                                                        <asp:GridView ID="grdGeneralProjectNotes" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowDataBound="grdGeneralProjectNotes_RowDataBound" PageSize="200" TabIndex="2" Width="100%">
                                                                            <Columns>
                                                                                <asp:TemplateField HeaderText="Date" SortExpression="ProjectDate">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("ProjectDate","{0:d}")%>' />

                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                                                                    <ItemStyle HorizontalAlign="Center" Width="120px" />
                                                                                </asp:TemplateField>


                                                                                <asp:TemplateField HeaderText="General Notes">
                                                                                    <ItemTemplate>

                                                                                        <asp:Label ID="lblDescription2" runat="server" Text='<%# Eval("NoteDetails").ToString() %>' Style="display: inline;"></asp:Label>
                                                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDescription2_r" runat="server" Text='<%# Eval("NoteDetails") %>' Visible="false" /></pre>
                                                                                        <asp:LinkButton ID="lnkGeneral2Open" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkGeneralOpe2_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                                    </ItemTemplate>
                                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                                    <ItemStyle HorizontalAlign="Left" Width="70%" />
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Completed" SortExpression="is_complete">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblCompleted" runat="server" Text=''></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                </asp:TemplateField>


                                                                            </Columns>
                                                                            <AlternatingRowStyle CssClass="alt" />
                                                                        </asp:GridView>
                                                                    </asp:Panel>
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </asp:Panel>
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
                                    <td align="center" valign="top">
                                        <table>
                                            <tr>
                                                <td align="right">Report View Options: </td>
                                                <td align="center" valign="top">
                                                    <asp:CheckBoxList ID="chkCVOptions" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Selected="False" Value="1">&nbsp;Show Code</asp:ListItem>
                                                        <asp:ListItem Selected="False" Value="2">&nbsp;Show UOM</asp:ListItem>
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server" CssClass="button" OnClick="btnGotoCustomerList_Click" Text="Go to Customer List" />
                                        &nbsp;<asp:Button ID="btnPrintByLoc" runat="server" CssClass="button" OnClick="btnPrintByLoc_Click" Text="Print SOW by Location" />
                                        <asp:Button ID="btnPrintBySec" runat="server" CssClass="button" OnClick="btnPrintBySec_Click" Text="Print SOW by Section" />
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
                                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
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
                                                    <%--<asp:HiddenField ID="hdnFinanceValue" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnIsCash" runat="server" Value="0" />--%>
                                                    <asp:HiddenField ID="hdnLastName" runat="server" />
                                                    <asp:HiddenField ID="hdnSecName" runat="server" />
                                                    <asp:HiddenField ID="hdnJob" runat="server" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                                    <asp:HiddenField ID="hdnSuperExCostDecrease" runat="server" Value="0" />
                                                </td>
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                         <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>
                                                    <cc1:ModalPopupExtender ID="modProjectNotes" TargetControlID="lnkDummy3" BackgroundCssClass="modalBackground"
                                                        CancelControlID="btnCancelItem2" PopupControlID="pnlProjectNotes"
                                                        runat="server">
                                                    </cc1:ModalPopupExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>



                        </ContentTemplate>
                    </asp:UpdatePanel>


                    <asp:Panel ID="pnlProjectNotes" runat="server" Style="width: auto; overflow: scroll; max-height: 600px;" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">&nbsp;
                                          
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="100%">

                                                <tr>
                                                    <td align="center" colspan="2">

                                                        <asp:GridView ID="grdProjectNote" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowDataBound="grdProjectNote_RowDataBound" PageSize="200" TabIndex="2" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Date" SortExpression="ProjectDate">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("ProjectDate","{0:d}")%>' />

                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                                                    <ItemStyle HorizontalAlign="Center" Width="120px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Section">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lblSection" runat="server" Text=''></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Material Track">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lblMaterialTrack" runat="server" Text='<%# Eval("MaterialTrack").ToString() %>' Style="display: inline;"></asp:Label>
                                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblMaterialTrack_r" runat="server" Text='<%# Eval("MaterialTrack") %>' Visible="false" /></pre>
                                                                        <asp:LinkButton ID="lnkOpenMaterialTrack" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenMaterialTrack_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText=" Design Updates">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lblDesignUpdates" runat="server" Text='<%# Eval("DesignUpdates").ToString() %>' Style="display: inline;"></asp:Label>
                                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDesignUpdates_r" runat="server" Text='<%# Eval("DesignUpdates") %>' Visible="false" /></pre>
                                                                        <asp:LinkButton ID="lnkOpenDesignUpdates" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenDesignUpdates_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Superintendent Notes">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lblSuperintendentNotes" runat="server" Text='<%# Eval("SuperintendentNotes").ToString() %>' Style="display: inline;"></asp:Label>
                                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblSuperintendentNotes_r" runat="server" Text='<%# Eval("SuperintendentNotes") %>' Visible="false" /></pre>
                                                                        <asp:LinkButton ID="lnkOpenSuperintendentNotes" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenSuperintendentNotes_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="General Notes">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("NoteDetails").ToString() %>' Style="display: inline;"></asp:Label>
                                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDescription_r" runat="server" Text='<%# Eval("NoteDetails") %>' Visible="false" /></pre>
                                                                        <asp:LinkButton ID="lnkGeneralOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkGeneralOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Left" Width="200px" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Completed" SortExpression="is_complete">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblCompleted" runat="server" Text=''></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>


                                                            </Columns>
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>

                                                    </td>
                                                </tr>

                                            </table>
                                        </td>
                                    </tr>
                                    <tr>

                                        <td align="center">
                                            <table width="100%">
                                                <tr>
                                                    <td align="right" style="width: 50%">&nbsp;
                                                                   
                                                      &nbsp;<asp:Button ID="btnCancelItem2" runat="server" Text="Close" TabIndex="3" Width="80px"
                                                          CssClass="button" />

                                                    </td>

                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

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
