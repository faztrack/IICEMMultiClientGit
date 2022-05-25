<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="changeorder_pricing.aspx.cs" Inherits="changeorder_pricing" Title="Change Order Pricing" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_ItemNameAll(sender, e) {
            document.getElementById('<%=btnSearchAll.ClientID%>').click();
        }
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

    </script>
    <script type="text/javascript">
<!--
    function Check_Click(objRef) {
        //Get the Row based on checkbox
        var row = objRef.parentNode.parentNode;

        //Get the reference of GridView
        var GridView = row.parentNode;

        //Get all input elements in Gridview
        var inputList = GridView.getElementsByTagName("input");

        for (var i = 0; i < inputList.length; i++) {
            //The First element is the Header Checkbox
            var headerCheckBox = inputList[0];

            //Based on all or none checkboxes
            //are checked check/uncheck Header Checkbox
            var checked = true;
            if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                if (!inputList[i].checked) {
                    checked = false;
                    break;
                }
            }
        }
        headerCheckBox.checked = checked;

    }
    function checkAll(objRef) {
        var GridView = objRef.parentNode.parentNode.parentNode;
        var inputList = GridView.getElementsByTagName("input");
        for (var i = 0; i < inputList.length; i++) {
            var row = inputList[i].parentNode.parentNode;
            if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                if (objRef.checked) {
                    inputList[i].checked = true;
                }
                else {
                    inputList[i].checked = false;
                }
            }
        }
    }
    //-->
    </script>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Change Order Pricing</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
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
                            <table width="100%" border="0" cellspacing="4" cellpadding="4" align="center">
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
                                                            <td style="width: 113px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                            <td>
                                                                <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Phone: </b></td>
                                                            <td>
                                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Email: </b></td>
                                                            <td>
                                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table>
                                                        <tr>
                                                            <td style="width: 100px;" align="left" valign="top"><b>Address: </b></td>
                                                            <td align="left" valign="top">
                                                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label></td>


                                                            <td align="left" valign="top">&nbsp;
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Sales Person:</b>&nbsp;</td>
                                                            <td align="left" style="width: auto;" valign="top">
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
                                                            <td>
                                                                <img src="images/icon-change-order-info.png" /></td>
                                                </td>
                                                <td align="left">
                                                    <h2>Change Order Information</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 390px;" align="left" valign="top">
                                        <table style="width: 520px;">
                                            <tr>
                                                <td style="width: 142px; height: 18px;" align="left" valign="top">
                                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                                </td>
                                                <td style="width: auto; height: 18px;">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td style="width: 142px;" align="left" valign="middle"><b>Estimate Name: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblEstimateName" runat="server" Font-Bold="true" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 142px; height: 18px;" align="left" valign="middle"><b>Change Order Name:</b></td>
                                                <td style="width: auto; height: 18px;">
                                                    <asp:Label ID="lblChangeOrderName" runat="server"></asp:Label>
                                                    <asp:LinkButton ID="lnkUpdateCoEstimate" runat="server">
                                                                    <span style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Rename</span>
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 142px;" align="left" valign="middle"><b>Locations: </b></td>
                                                <td style="width: auto;">
                                                    <table style="margin-left: -4px; padding: 0px;">
                                                        <tr>
                                                            <td>
                                                                <asp:DropDownList ID="ddlCustomerLocations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomerLocations_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                                <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:LinkButton ID="lnkAddMoreLocation" runat="server" OnClick="lnkAddMoreLocation_Click">
                                                                                <span style="color:#2d7dcf; text-decoration:underline; font-weight:bold;">Add/Remove Location</span>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblSelectLocation" runat="server" Text=""></asp:Label>
                                                </td>

                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left" valign="top">
                                        <table style="width: 450px;">
                                            <tr>
                                                <td style="width: 64px;" align="left" valign="top"><b>&nbsp;</b></td>
                                                <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="width: 75px;" valign="top"><b>Cust Disp:</b></td>
                                                <td align="left" style="width: auto;" valign="middle">
                                                    <asp:CheckBox ID="chkCustDisp" runat="server" AutoPostBack="True" OnCheckedChanged="chkCustDisp_CheckedChanged" />
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
                                                <td style="width: auto;" align="left" valign="middle"><b>Sections: </b></td>
                                                <td style="width: auto;" align="left" valign="top">
                                                    <asp:DropDownList ID="ddlSections" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSections_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                    <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                                                    <asp:LinkButton ID="lnkAddMoreSections" runat="server" OnClick="lnkAddMoreSections_Click"><span style="color:#2d7dcf; text-decoration:underline; font-weight:bold;">Add/Remove Section</span></asp:LinkButton>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                                <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table class="wrapper" style="width: 100%;" runat="server" id="tblPricingWrapper" visible="false">
                                            <tr>
                                                <td align="left" rowspan="4" valign="top">
                                                    <asp:TreeView ID="trvSection" runat="server" OnSelectedNodeChanged="trvSection_SelectedNodeChanged"
                                                        ImageSet="Msdn" NodeIndent="10">
                                                        <ParentNodeStyle Font-Bold="False" />
                                                        <HoverNodeStyle BackColor="#CCCCCC" BorderColor="#888888" BorderStyle="Solid" Font-Underline="True" />
                                                        <SelectedNodeStyle BackColor="White" BorderColor="#888888" BorderStyle="Solid" BorderWidth="1px"
                                                            Font-Underline="False" HorizontalPadding="3px" VerticalPadding="1px" />
                                                        <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px"
                                                            NodeSpacing="1px" VerticalPadding="2px" />
                                                    </asp:TreeView>
                                                </td>
                                                <td align="left" valign="top">
                                                    <asp:Label ID="lblParent" runat="server" Font-Bold="True" ForeColor="Blue"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">
                                                    <asp:GridView ID="grdItemPrice" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                        DataKeyNames="item_id" OnRowCommand="grdItemPrice_RowCommand" PageSize="200"
                                                        TabIndex="2" Width="100%">
                                                        <Columns>
                                                            <asp:ButtonField CommandName="Add" Text="Add" />
                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" />
                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" />
                                                            <asp:BoundField DataField="section_name" HeaderText="Item Name">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" Width="280px" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtShortNote" runat="server" MaxLength="98"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Labor">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlLabor" runat="server" SelectedValue='<%# Eval("labor_id") %>'>
                                                                        <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                        <asp:ListItem Value="1">No</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Direct">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlDirect" runat="server">
                                                                        <asp:ListItem Value="1">No</asp:ListItem>
                                                                        <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM" NullDisplayText=" ">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="item_cost" DataFormatString="{0:c}" HeaderText="Unit Price">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="LaborUnitCost" DataFormatString="{0:c}" HeaderText="Labor Rate">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="minimum_qty" HeaderText="Min Code">
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <%-- <asp:TemplateField HeaderText="Code">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtQty" runat="server" AutoPostBack="True" Style="text-align: center;" Text='<%# Eval("minimum_qty") %>'
                                                                        Width="40px" OnTextChanged="ItemPrice_calculation"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>--%>
                                                            <asp:TemplateField HeaderText="Code">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtQty" runat="server" Style="text-align: center;" Text='<%# Eval("minimum_qty") %>'
                                                                        Width="40px"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <%--<asp:TemplateField HeaderText="Ext. Price">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTotalPrice" runat="server" Text='<%# Eval("ext_item_cost","{0:c}").ToString() %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>--%>
                                                        </Columns>
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">

                                                    <table class="tabGrid" width="100%" cellpadding="0" cellspacing="0" id="tdlCustomizePrice" runat="server" height="auto" visible="True">
                                                        <tr>
                                                            <td></td>
                                                            <td align="Center" style="width: 400px;"><b style="font-weight: bold; color: #555;">Item Name</b>
                                                            </td>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">UoM</b>
                                                            </td>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">Unit Cost</b>
                                                            </td>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">Code</b>
                                                            </td>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">Ext.Price</b>
                                                            </td>
                                                            <%-- <td align="center">Ext. Price
                                                            </td>--%>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">Direct</b>
                                                            </td>
                                                            <td align="Center"><b style="font-weight: bold; color: #555;">Short Notes</b>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td class="tabGridtd">
                                                                <asp:Button ID="btnCustomizeItemAdd" runat="server" CssClass="button" OnClick="btnCustomizeItemAdd_Click"
                                                                    Text="Add" />
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeItemName" runat="server" Width="400"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeUOM" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeUnitPrice" runat="server" Style="text-align: right;"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeCode" runat="server" Style="text-align: center;"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeExtPrice" runat="server" Style="text-align: center;"></asp:TextBox>
                                                            </td>
                                                            <%-- <td valign="top">
                                                                <asp:TextBox ID="txtO_TotalPrice" Style="text-align: right;" runat="server" ReadOnly="True" Width="40px"></asp:TextBox>
                                                            </td>--%>
                                                            <td class="tabGridtd">
                                                                <asp:DropDownList ID="ddlCustomizeDirect" runat="server">
                                                                    <asp:ListItem Value="1">No</asp:ListItem>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtCustomezeNotes" runat="server" TextMode="MultiLine" MaxLength="500"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                    </table>

                                                    <table class="tabGrid" width="100%" cellpadding="0" cellspacing="0" id="tdlOther" runat="server" height="auto" visible="True">
                                                        <tr>
                                                            <td></td>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">Other Item Name</b>
                                                            </td>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">UoM</b>
                                                            </td>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">Unit Price</b>
                                                            </td>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">Code</b>
                                                            </td>
                                                            <%-- <td align="center">Ext. Price
                                                            </td>--%>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">Direct</b>
                                                            </td>
                                                            <td align="left"><b style="font-weight: bold; color: #555;">Short Notes</b>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:Button ID="btnAddOthers" runat="server" CssClass="button" OnClick="btnAddOthers_Click"
                                                                    Text="Add" />
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtOther" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtO_Unit" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtO_Price" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtO_Qty" runat="server" Style="text-align: center;"></asp:TextBox>
                                                            </td>
                                                            <%--<td>
                                                                <asp:TextBox ID="txtO_Qty" runat="server" Style="text-align: center;" Width="42px" OnTextChanged="txtO_Qty_TextChanged"
                                                                    AutoPostBack="True"></asp:TextBox>
                                                            </td>--%>
                                                            <%-- <td>
                                                                <asp:TextBox ID="txtO_TotalPrice" runat="server" Width="55px" ReadOnly="True"></asp:TextBox>
                                                            </td>--%>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:DropDownList ID="ddlO_Direct" runat="server">
                                                                    <asp:ListItem Value="1">No</asp:ListItem>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtO_ShortNotes" runat="server"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                        </table>
                                    </td>
                                </tr>
                            <tr>
                                <td align="left" valign="top">
                                    <table width="100%" id="tblMultiPricing" runat="server" visible="false" class="wrapper">
                                        <tr>
                                            <td align="left" valign="top" width="70%">
                                                <asp:TextBox ID="txtSearchAll" runat="server" Width="50%"></asp:TextBox>
                                                <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemNameAll" ServiceMethod="GetItemNameAll" TargetControlID="txtSearchAll" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtSearchAll" WatermarkText="Search by Item Name" />
                                                <asp:Button ID="btnSearchAll" runat="server" CssClass="button" OnClick="btnSearchAll_Click" Text="Search" />
                                                <%--<asp:LinkButton ID="lnkAllViewA" runat="server" OnClick="lnkAllViewA_Click">Reset</asp:LinkButton>--%>
                                                  &nbsp;<asp:LinkButton ID="lnkClear" runat="server" OnClick="lnkClear_Click">Clear Search Text Box</asp:LinkButton>
                                                &nbsp;&nbsp;
                                                    <asp:LinkButton ID="lnkResetAll" runat="server" OnClick="lnkResetAll_Click">Reset All Search Item</asp:LinkButton>
                                            </td>
                                            <td align="right">
                                                <table>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:ImageButton ID="imgHelp" runat="server" CssClass="imgInputNoEffect" ImageUrl="~/Images/help.png" ToolTip="Click to Adding item in Pricing help" OnClick="imgHelp_Click" /></td>
                                                            <td>
                                                                <asp:Button ID="btnAddMultiple" runat="server" CssClass="button" Visible="false" Text="Add Selected Item in Pricing" OnClick="btnAddMultiple_Click" /></td>
                                                        </tr>
                                                    </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="gridTitle" colspan="2" align="center">Collector Grid
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:GridView ID="grdItemPriceAll" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                    DataKeyNames="item_id" PageSize="200"
                                                    TabIndex="2" Width="99%" OnRowDataBound="grdItemPriceAll_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="item_id" HeaderText="Item Id" />
                                                        <asp:BoundField HeaderText="Section Name">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="section_name" HeaderText="Item Name">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="40%" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Short Notes">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtShortNote" runat="server" MaxLength="98"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Labor">
                                                            <ItemTemplate>
                                                                <asp:DropDownList ID="ddlLabor" runat="server" SelectedValue='<%# Eval("labor_id") %>'>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                    <asp:ListItem Value="1">No</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Direct">
                                                            <ItemTemplate>
                                                                <asp:DropDownList ID="ddlDirect" runat="server">
                                                                    <asp:ListItem Value="1">No</asp:ListItem>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="measure_unit" HeaderText="UoM" NullDisplayText=" ">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="item_cost" DataFormatString="{0:c}" HeaderText="Unit Price">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="LaborUnitCost" DataFormatString="{0:c}" HeaderText="Labor Rate">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Right" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="minimum_qty" HeaderText="Min Code">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Code">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtQty" runat="server" Style="text-align: center;" Text='<%# Eval("minimum_qty") %>' Width="40px"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <%--<asp:TemplateField HeaderText="Ext. Price">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTotalPrice" runat="server" Text='<%# Eval("ext_item_cost","{0:c}").ToString() %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>--%>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="chkIsSelectedAll" runat="server" onclick="checkAll(this);" Text="All" TextAlign="Left" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkIsSelected" runat="server" onclick="Check_Click(this)" />
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <%--<asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckChanged" onclick="checkAll(this);" Text="All" TextAlign="Left" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chk" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckChanged" onclick="Check_Click(this)" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>--%>
                                                    </Columns>
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="gridTitle" colspan="2" align="center">Recently Added Item(s)
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="center" valign="top">
                                    <asp:Label ID="lblAdd" runat="server"></asp:Label>
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
                                    <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True" OnRowDataBound="grdGrouping_RowDataBound"
                                        AutoGenerateColumns="False" CssClass="mGrid" OnRowCommand="grdGrouping_RowCommand">
                                        <FooterStyle CssClass="white_text" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />
                                                    <asp:LinkButton ID="lnkMove1" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                        CommandName="Move" CssClass="moveUp">Move to Top</asp:LinkButton>
                                                    <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                        DataKeyNames="item_id" OnRowDataBound="grdSelectedItem1_RowDataBound" OnRowDeleting="grdSelectedItem1_RowDeleting" OnRowEditing="grdSelectedItem1_RowEditing"
                                                        Width="100%" CssClass="mGrid" OnRowCommand="grdSelectedItem1_RowCommand" OnRowUpdating="grdSelectedItem1_RowUpdating">
                                                        <Columns>
                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" HeaderStyle-Width="6%" ReadOnly="True" />
                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" HeaderStyle-Width="5%" ReadOnly="True" />
                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                    <asp:TextBox ID="txtshort_notes" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                        TextMode="MultiLine" />
                                                                    <asp:Label ID="lblshort_notes_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                    <asp:LinkButton ID="lnkOpen" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="10%" />
                                                            </asp:TemplateField>
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
                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name" HeaderStyle-Width="18%" ReadOnly="True" />
                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True" NullDisplayText=" ">
                                                                <HeaderStyle Width="6%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" HeaderStyle-Width="5%" ReadOnly="True" />
                                                            <asp:TemplateField HeaderText="Unit Cost">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="7%" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                                <FooterStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblquantity" runat="server" Text='<%# Eval("quantity") %>' />
                                                                    <asp:TextBox ID="txtquantity" runat="server" Visible="false" Width="40px" Style="text-align: center;" Text='<%# Eval("quantity") %>'
                                                                        AutoPostBack="True" OnTextChanged="NonDirect_calculation" />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="5%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
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
                                                                <HeaderStyle Width="7%" />
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
                                                                <HeaderStyle Width="7%" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                                <FooterStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False" HeaderStyle-Width="1%" ReadOnly="True" />
                                                            <asp:ButtonField CommandName="Edit" Text="Edit" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle Width="5%" />
                                                            </asp:ButtonField>
                                                            <asp:ButtonField CommandName="Delete" Text="Delete" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center" />
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <table style="padding: 0px; margin: 0px; width: 99.8%">
                                                        <tr>
                                                            <td style="padding: 0px; margin: 0px; width: 6%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 10%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 12%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 18%;">
                                                                <asp:Label ID="Label1" runat="server" Text='<%# GetTotalPrice()%>' />
                                                            </td>
                                                            <td style="padding: 0px; margin: 0px; width: 6%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 7%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                <asp:Label ID="lblGtotalLabel" runat="server" Text="Grand Total:" />
                                                            </td>
                                                            <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                <asp:Label ID="lblGtotalExtCost" runat="server" Text='<%# GetTotalExtCost()%>' />
                                                            </td>
                                                            <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                <asp:Label ID="lblGtotalExtPrice" runat="server" Text='<%# GetTotalExPrice()%>' />
                                                            </td>
                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                            <td style="padding: 0px; margin: 0px; width: 5%;"></td>
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
                                            <td align="center">
                                                <h3>
                                                    <asp:Label ID="lblDirectPricingHeader" runat="server" Text="The following items are Direct / Outsourced"
                                                        Visible="False"></asp:Label></h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top" width="15%">
                                                <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                    OnRowDataBound="grdGroupingDirect_RowDataBound" ShowFooter="True" CaptionAlign="Top"
                                                    OnRowCommand="grdGroupingDirect_RowCommand">
                                                    <FooterStyle CssClass="white_text" />
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                                &nbsp;<asp:LinkButton ID="lnkMove2" runat="server" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                                    CommandName="Move1" CssClass="moveUp">Move to Top</asp:LinkButton>
                                                                <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                    DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound" OnRowDeleting="grdSelectedItem2_RowDeleting" OnRowEditing="grdSelectedItem2_RowEditing"
                                                                    ShowFooter="True" Width="100%" OnRowCommand="grdSelectedItem2_RowCommand" OnRowUpdating="grdSelectedItem2_RowUpdating">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="item_id" HeaderText="Item Id" HeaderStyle-Width="6%" ReadOnly="True" />
                                                                        <asp:BoundField DataField="section_serial" HeaderText="SL" HeaderStyle-Width="5%" ReadOnly="True" />
                                                                        <asp:TemplateField HeaderText="Short Notes">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblshort_notes1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                <asp:TextBox ID="txtshort_notes1" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                                    TextMode="MultiLine" />
                                                                                <asp:Label ID="lblshort_notes1_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                                <asp:LinkButton ID="lnkOpen1" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen1_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="10%" />
                                                                        </asp:TemplateField>
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
                                                                        <asp:BoundField DataField="item_name" HeaderText="Item Name" HeaderStyle-Width="18%" ReadOnly="True" />
                                                                        <asp:BoundField DataField="measure_unit" HeaderText="UoM" HeaderStyle-Width="6%" ReadOnly="True" NullDisplayText=" " ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" HeaderStyle-Width="5%" ReadOnly="True" />
                                                                        <asp:TemplateField HeaderText="Unit Cost">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUnit_Cost3" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="7%" />
                                                                            <ItemStyle HorizontalAlign="Right" />
                                                                            <FooterStyle HorizontalAlign="Right" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Code" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblquantity1" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                <asp:TextBox ID="txtquantity1" runat="server" Visible="false" Width="30px" Text='<%# Eval("quantity") %>'
                                                                                    AutoPostBack="True" OnTextChanged="Direct_calculation" />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:TemplateField>
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
                                                                            <HeaderStyle Width="7%" />
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
                                                                            <HeaderStyle Width="7%" />
                                                                            <ItemStyle HorizontalAlign="Right" />
                                                                            <FooterStyle HorizontalAlign="Right" />
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False" HeaderStyle-Width="1%" ReadOnly="True" />
                                                                        <asp:ButtonField CommandName="Edit" Text="Edit" ItemStyle-HorizontalAlign="Center">
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:ButtonField>
                                                                        <asp:ButtonField CommandName="Delete" Text="Delete" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center" />
                                                                    </Columns>
                                                                    <PagerStyle CssClass="pgr" />
                                                                    <AlternatingRowStyle CssClass="alt" />
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <table style="padding: 0px; margin: 0px; width: 99.8%">
                                                                    <tr>
                                                                        <td style="padding: 0px; margin: 0px; width: 6%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 10%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 12%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 18%;">
                                                                            <asp:Label ID="Label1" runat="server" Text='<%# GetTotalPriceDirect()%>' />
                                                                        </td>
                                                                        <td style="padding: 0px; margin: 0px; width: 6%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 7%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                            <asp:Label ID="lblGtotalLabel2" runat="server" Text="Grand Total:" />
                                                                        </td>
                                                                        <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                            <asp:Label ID="lblGtotalExtCost2" runat="server" Text='<%# GetTotalExtCostDirect()%>' />
                                                                        </td>
                                                                        <td style="padding: 0px; margin: 0px; width: 7%; text-align: right;">
                                                                            <asp:Label ID="lblGtotalExtPrice2" runat="server" Text='<%# GetTotalExPriceDirect()%>' />
                                                                        </td>
                                                                        <td style="padding: 0px; margin: 0px; width: 5%;"></td>
                                                                        <td style="padding: 0px; margin: 0px; width: 5%;"></td>
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
                                    <asp:Button ID="btnGotoCustomerList" runat="server" OnClick="btnGotoCustomerList_Click"
                                        Text="Go to Customer List" CssClass="button" />
                                    &nbsp;<asp:Button ID="btnGotoWorkSheet" runat="server" CssClass="button" OnClick="btnGotoWorkSheet_Click"
                                        Text="Go to C/O Summary" />
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" valign="top">
                                    <cc1:ModalPopupExtender ID="modUpdateCoEstimate" runat="server" PopupControlID="pnlUpdateCoEstimate"
                                        TargetControlID="lnkUpdateCoEstimate" BackgroundCssClass="modalBackground" DropShadow="false">
                                    </cc1:ModalPopupExtender>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" valign="top"></td>
                            </tr>
                            <tr>
                                <td align="center">&nbsp;
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
                                            <td align="right">
                                                <asp:HiddenField ID="hdnOtherId" runat="server" Value="0" />
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
                                                <asp:HiddenField ID="hdnChEstId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSortDesc" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                            </td>
                                            <td align="left">
                                                <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False" Value="0" />
                                                <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
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
                                            <b>Update ChangeOrder Name</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="right" width="30%">
                                                        <b>ChangeOrder Name: </b>
                                                    </td>
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
                                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" TabIndex="2" Width="80px"
                                                            OnClick="btnSubmit_Click" CssClass="button" />
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
