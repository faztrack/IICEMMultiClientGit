<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="sold_estimate.aspx.cs" Inherits="sold_estimate" Title="Pricing" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function DisplayEmailWindow(FileName) {

            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&sfn=' + FileName, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

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
                        <td align="left"><span class="titleNu">Estimate Pricing</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
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
                                                            <td style="width: auto; height: 18px;">
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
                                                    <table>
                                                        <tr>
                                                            <td style="width: 100px;" align="left" valign="top"><b>Address: </b></td>
                                                            <td align="left" valign="top">
                                                                <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="left" valign="top">&nbsp;
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Sales Person:</b>&nbsp;</td>
                                                            <td align="left" valign="top">
                                                                <asp:Label ID="lblSalesPerson" runat="server"></asp:Label></td>
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
                                                <td style="width: 220px; border-right: 1px solid #ddd;" align="left" valign="middle">
                                                    <table width="100%">
                                                        <tr>
                                                            <td width="74px">
                                                                <img src="images/icon-estimate-info.png" /></td>
                                                            <td align="left">
                                                                <h2>Estimate Information</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 400px;" align="left" valign="top">
                                                    <table style="width: 400px;">
                                                        <tr>
                                                            <td style="width: 114px;" align="left" valign="top"><b>Job Number:</b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblJobNumber" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 160px;" align="left" valign="top"><b>Alternate Job Number:</b></td>
                                                            <td style="width: auto;">
                                                                <asp:TextBox ID="txtAlterJobNumber" runat="server"></asp:TextBox><asp:Button ID="btnSaveAlterJob" runat="server" CssClass="button" OnClick="btnSaveAlterJob_Click" Text="Save" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:Label ID="lblJobMSG" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 114px;" align="left"><b>Estimate Name: </b></td>
                                                            <td style="height: 18px;" align="left" valign="top">
                                                                <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                                <asp:LinkButton ID="lnkUpdateEstimate" runat="server">
                                                                    <span style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Rename</span>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td style="height: 18px;" align="left" valign="top">
                                                                <asp:Button ID="btnTemplateEstimate" Width="220" runat="server" OnClick="btnTemplateEstimate_Click" Text="Save this Estimate as a Template" ToolTip="Click here to Save this Estimate as a Template" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" style="width: 114px;" valign="middle"><b>Superintendent:</b></td>
                                                            <td style="width: auto;">
                                                                <asp:DropDownList ID="ddlSuperintendent" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSuperintendent_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                                <asp:Button ID="btnUpdateSuperintendent" runat="server" CssClass="button" OnClick="btnUpdateSuperintendent_Click" Text="Update" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px; height: 25px;" align="left" valign="top">
                                                                <b>
                                                                    <asp:Label ID="lblTax_label" runat="server" Text="Tax % :"></asp:Label></b>
                                                            </td>
                                                            <td style="width: auto; height: 25px;" align="left" valign="top">
                                                                <asp:Label ID="lblTax" runat="server" Text="0" Width="15%"> </asp:Label>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="left" style="width: 114px;" valign="middle"><b>
                                                                <asp:Label ID="lblSaleDate" runat="server" Text="Sale Date:" Visible="False"></asp:Label>
                                                            </b></td>
                                                            <td style="width: auto;">
                                                                <asp:TextBox ID="txtSaleDate" runat="server" Visible="False"></asp:TextBox>
                                                            </td>
                                                        </tr>

                                                    </table>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table style="width: 520px;">
                                                        <tr>
                                                            <td align="left" style="width: 112px;" valign="middle"><b>Cust Disp:</b></td>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <table style="padding: 0px; margin: -2px;">
                                                                    <tr>
                                                                        <td align="left" valign="middle" style="padding: 0px; margin: 0px;">
                                                                            <asp:CheckBox ID="chkCustDisp" runat="server" AutoPostBack="True" OnCheckedChanged="chkCustDisp_CheckedChanged" /></td>
                                                                        <td style="padding-left: 10px; display: none" align="left" valign="middle">
                                                                            <asp:ImageButton ID="btnExpCostLocList" runat="server" CssClass="imageBtn" ImageUrl="~/images/cloc_csv.png" OnClick="btnExpCostLocList_Click" ToolTip="Cost by Location in CSV" />
                                                                        </td>
                                                                        <td align="left" valign="middle" style="display: none">
                                                                            <asp:ImageButton ID="btnExpCostSecList" runat="server" CssClass="imageBtn" ImageUrl="~/images/csec_csv.png" OnClick="btnExpCostSecList_Click" ToolTip="Cost by Section in CSV" />
                                                                        </td>
                                                                        <td style="padding-left: 10px;" align="left" valign="middle">
                                                                            <asp:ImageButton ID="btnExpAsSoldList" runat="server" CssClass="imageBtn" ImageUrl="~/images/project_as_sold_csv_btn.png" OnClick="btnExpAsSoldList_Click" ToolTip="Project as sold in CSV" />
                                                                        </td>
                                                                        <td align="left" valign="middle">
                                                                            <asp:Button ID="btnPricingPrint" runat="server" CssClass="button" OnClick="btnPricingPrint_Click" Text="Print" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <b>
                                                                    <asp:Label ID="lblPm" runat="server" Text="PM Disp:"></asp:Label></b></td>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <asp:CheckBox ID="chkPMDisplay" runat="server" AutoPostBack="True" OnCheckedChanged="chkPMDisplay_CheckedChanged" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" style="width: 112px;" valign="top"><b>Project Status:</b></td>
                                                            <td align="left" style="width: auto;" valign="top">
                                                                <asp:RadioButtonList ID="rdbEstimateIsActive" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rdbEstimateIsActive_SelectedIndexChanged" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Selected="True" Text="Active" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="InActive" Value="0"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="middle"><b>Estimate Status:</b>&nbsp;</td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True">
                                                                    <asp:ListItem Value="1">Pending</asp:ListItem>
                                                                    <asp:ListItem Value="2">Sit</asp:ListItem>
                                                                    <asp:ListItem Value="3">Sold</asp:ListItem>
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
                                        <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                    </td>
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
                                            <asp:Label ID="lblRetailPricingHeader" runat="server" Text="Selected Items" Visible="false"></asp:Label>
                                        </h3>

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
                                                        <asp:Label ID="lblMSub" Font-Bold="true" Font-Size="15px" runat="server" />
                                                        <asp:LinkButton ID="lnkSendEmail1" runat="server" CssClass="sendEmail" CommandName="sEmail" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton>
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                                            Width="100%" CssClass="mGrid">
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
                                                                        <asp:Label ID="lblProjectedProfit" runat="server" Font-Bold="true" Font-Size="13px" Visible="false" />
                                                                    </FooterTemplate>
                                                                    <HeaderStyle Width="14%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                    <HeaderStyle Width="22%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshNote" runat="server" Text='<%# Eval("short_notes").ToString() %>'></asp:Label>
                                                                        <asp:Label ID="lblshort_notes_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                        <asp:LinkButton ID="lnkOpen" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <HeaderStyle Width="11%" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="UoM">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMeasureUnit" runat="server" Text='<%# Eval("measure_unit").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="8%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Unit Cost">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="quantity" HeaderText="Code">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Labor Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLaborCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
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
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="8%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="8%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                    Visible="False">
                                                                    <HeaderStyle Width="1%" />
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
                                                                <td style="padding: 0px; margin: 0px; width: 14%;" align="left">
                                                                    <asp:Label ID="lblOverallProjectedProfit" runat="server" Text='<%# GetOverallProjectedProfit()%>' />
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px; width: 22%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 11%;">
                                                                    <asp:Label ID="Label1" runat="server" Text='<%# GetTotalPrice()%>' />
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px; width: 8%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 7%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalLabel" runat="server" Text="Grand Total:" />
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px; width: 8%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalExtCost" runat="server" Text='<%# GetTotalExtCost()%>' />
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px; width: 8%; text-align: right;">
                                                                    <asp:Label ID="lblGtotalExtPrice" runat="server" Text='<%# GetTotalExPrice()%>' />
                                                                </td>

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
                                                        <asp:Label ID="lblDirectPricingHeader" runat="server"
                                                            Text="The following items are Direct / Outsourced" Visible="False"></asp:Label></h3>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" colspan="2" valign="top" width="15%">
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False"
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound" OnRowCommand="grdGroupingDirect_RowCommand"
                                                        ShowFooter="True" CaptionAlign="Top">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header"
                                                                        Text='<%# Eval("colName").ToString() %>' />
                                                                    <asp:Label ID="lblMSub2" Font-Bold="true" Font-Size="15px" runat="server" />
                                                                    <asp:LinkButton ID="lnkSendEmail2" runat="server" CssClass="sendEmail" CommandName="sEmail2" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton>
                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" DataKeyNames="item_id"
                                                                        ShowFooter="True" OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        Width="100%" CssClass="mGrid">
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
                                                                                <HeaderStyle Width="14%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                                <HeaderStyle Width="22%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblShNote2" runat="server" Text='<%# Eval("short_notes").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <HeaderStyle Width="11%" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="UoM">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblMeasureUnit2" runat="server" Text='<%# Eval("measure_unit").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Unit Cost">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblUnit_Cost3" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="quantity" HeaderText="Code">
                                                                                <HeaderStyle Width="5%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:BoundField>
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
                                                                                    <asp:Label ID="lblTotal_price2" runat="server" Text='<%# Eval("total_direct_price","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="8%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                                Visible="False">
                                                                                <HeaderStyle Width="1%" />
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
                                                <td align="right" valign="top" width="25%">
                                                    <b>Notes: </b>
                                                    <br />
                                                    <i style="font-size: 11px; margin: 0 5px; color: #2196f3;">(This will appear in the Estimate Summary Only report)</i>
                                                    <br />
                                                    <asp:TextBox ID="txtDisplay" runat="server" BackColor="Transparent" BorderColor="Transparent" CssClass="nostyle"
                                                        BorderStyle="None" BorderWidth="0px" Style="text-align: right; padding-right: 5px !important;" Font-Bold="True" Height="16px" ReadOnly="True"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1" TextMode="MultiLine"
                                                        onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1000',document.getElementById('head_txtDisplay'));" Width="75%"></asp:TextBox>
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
                                            OnClick="btnGotoCustomerList_Click"
                                            Text="Go to Customer List" CssClass="button" />
                                        &nbsp;<asp:Button ID="btnGoToPayment" runat="server" OnClick="btnGoToPayment_Click"
                                            Text="Go to Payment" CssClass="button" />

                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <asp:Button ID="btnSchedule" runat="server" CssClass="button" Style="display: none;"
                                            OnClick="btnSchedule_Click" Text="Schedule" />
                                        <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">&nbsp;</td>
                                                <td align="left"><asp:HiddenField ID="hdnClientId" runat="server" Value="0" /></td>
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
                                    <td>
                                        <cc1:ModalPopupExtender ID="modUpdateEstimate" runat="server" PopupControlID="pnlUpdateEstimate"
                                            TargetControlID="lnkUpdateEstimate" BackgroundCssClass="modalBackground" DropShadow="false">
                                        </cc1:ModalPopupExtender>

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
                                                    <asp:HiddenField ID="hdnLastName" runat="server" />
                                                    <asp:HiddenField ID="hdnSecName" runat="server" />
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False" Value="0" />
                                                    <asp:HiddenField ID="hdnModelEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                                    <!--  Estimate Copy --->
                                                    <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender2" TargetControlID="btnTemplateEstimate"
                                                        OnClientCancel="btnTemplateCancel" DisplayModalPopupID="ModalPopupExtender3" runat="server">
                                                    </cc1:ConfirmButtonExtender>
                                                    <cc1:ModalPopupExtender ID="ModalPopupExtender3" TargetControlID="btnTemplateEstimate" BackgroundCssClass="modalBackground"
                                                        CancelControlID="btnTemplateCancel" OkControlID="btnTemplateOk" PopupControlID="pnlTemplateCopyConfirm"
                                                        runat="server">
                                                    </cc1:ModalPopupExtender>
                                                    <!-- End Estimate Copy --->
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="pnlUpdateEstimate" runat="server" Width="550px" Height="150px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">
                                            <b>Update Estimate Name</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="right" width="30%">
                                                        <b>Estimate Name: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblExistingEstimateName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <b>New Estimate Name: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtNewEstimateName" runat="server" TabIndex="1" Width="200px"></asp:TextBox>
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
                                                            OnClick="btnSubmit_Click" CssClass="button" />
                                                        &nbsp;<asp:Button ID="btnClose" runat="server" Text="Close" TabIndex="3"
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
                    <asp:Panel ID="pnlTemplateCopyConfirm" runat="server" Width="550px" Height="100px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Clicking &#39;Yes&#39; will save this Estimate as a Template.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Are you sure you want to save this Estimate as a Template?</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnTemplateOk" runat="server" Text="Yes" CssClass="button" Width="60px" />
                                            <asp:Button ID="btnTemplateCancel" runat="server" Text="Cancel" CssClass="button" Width="60px" />
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

