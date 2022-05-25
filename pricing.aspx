<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="pricing.aspx.cs"
    Inherits="pricing" Title="Estimate Pricing" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }

        function DisplayEmailWindow(FileName) {

            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&sfn=' + FileName, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_ItemName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
        function selected_ItemNameAll(sender, e) {
            document.getElementById('<%=btnSearchAll.ClientID%>').click();
        }
        function SearchAllPress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearchAll.ClientID%>').click();
                return false;
            }
            return true;

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
                        <td align="left"><span class="titleNu">Estimate Pricing</span></td>
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
                                                            <td>
                                                                <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Phone: </b></td>
                                                            <td>
                                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Email: </b></td>
                                                            <td>
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
                                                            <td align="left" valign="top">
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Sales Person:</b>&nbsp;</td>
                                                            <td>
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
                                                <td style="width: 390px;" align="left" valign="top">
                                                    <table style="width: 520px;">
                                                        <tr>
                                                            <td style="height: 18px;" align="left" valign="top" colspan="2">
                                                                <asp:Label ID="Label6" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 112px;" align="left" valign="top"><b>Estimate Name: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>
                                                                <asp:LinkButton ID="lnkUpdateEstimate" runat="server">
                                                                    <span style="color:#2d7dcf; text-decoration:underline; font-weight:bold; ">Rename</span>
                                                                </asp:LinkButton>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td style="height: 18px;" align="left" valign="top">
                                                                <asp:Button ID="btnDuplicate" runat="server" OnClick="btnDuplicate_Click" Text="Duplicate" ToolTip="Click here to Duplicate this Estimate" />
                                                                <asp:Button ID="btnTemplateEstimate" Width="220" runat="server" OnClick="btnTemplateEstimate_Click" Text="Save this Estimate as a Template" ToolTip="Click here to Save this Estimate as a Template" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" style="width: 112px;" valign="middle"><b>Superintendent:</b></td>
                                                            <td style="width: auto;">
                                                                <asp:DropDownList ID="ddlSuperintendent" runat="server">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <b>
                                                                    <asp:Label ID="lblTax_label" runat="server" Text="Tax % :"></asp:Label></b>
                                                            </td>
                                                            <td style="width: auto; height: 25px;" align="left" valign="top">
                                                                <asp:TextBox ID="txtTaxPer" runat="server" Width="58px"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td align="left" style="width: 112px;" valign="middle"><b>Locations: </b></td>
                                                            <td style="width: auto;">
                                                                <asp:DropDownList ID="ddlCustomerLocations" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomerLocations_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                                <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                                                                <asp:LinkButton ID="lnkAddMoreLocation" runat="server" OnClick="lnkAddMoreLocation_Click">
                                                                    <span style="color:#2d7dcf; text-decoration:underline; font-weight:bold;">Add/Remove Location</span>
                                                                </asp:LinkButton>
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
                                                    <table style="width: 580px;">
                                                        <tr>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <b>Cust Disp:</b></td>
                                                            <td style="width: auto;" align="left" valign="middle">
                                                                <table style="padding: 0px; margin: -2px;">
                                                                    <tr>
                                                                        <td align="left" valign="middle" style="padding: 0px; margin: 0px;">
                                                                            <asp:CheckBox ID="chkCustDisp" runat="server" AutoPostBack="True" OnCheckedChanged="chkCustDisp_CheckedChanged" /></td>
                                                                        <td align="left" style="padding-left: 10px; display: none;" valign="middle">
                                                                            <asp:ImageButton ID="btnExpCostLocList" runat="server" CssClass="imageBtn" ImageUrl="~/images/cloc_csv.png" OnClick="btnExpCostLocList_Click" ToolTip="Cost by Location in CSV" />
                                                                        </td>
                                                                        <td align="left" valign="middle" style="display: none;">
                                                                            <asp:ImageButton ID="btnExpCostSecList" runat="server" CssClass="imageBtn" ImageUrl="~/images/csec_csv.png" OnClick="btnExpCostSecList_Click" ToolTip="Cost by Section in CSV" />
                                                                        </td>
                                                                        <td align="left" valign="middle">&nbsp; &nbsp; &nbsp;<asp:Button ID="btnPricingPrint" runat="server" CssClass="button" OnClick="btnPricingPrint_Click" Text="Print" />
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
                                                            <td style="width: auto;" align="left" valign="top"><b>Project Status:</b></td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <asp:RadioButtonList ID="rdbEstimateIsActive" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Selected="True" Text="Active" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="InActive" Value="0"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td style="width: 120px;" align="left" valign="top"><b>Estimate Status:</b>&nbsp;</td>
                                                            <td style="width: auto;" align="left" valign="top">
                                                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="left" style="width: auto;" valign="top"><b>Sections: </b></td>
                                                            <td align="left" style="width: auto;" valign="top">
                                                                <asp:DropDownList ID="ddlSections" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSections_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                                <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label>
                                                                <asp:LinkButton ID="lnkAddMoreSections" runat="server" OnClick="lnkAddMoreSections_Click"><span style="color:#2d7dcf; text-decoration:underline; font-weight:bold;">Add/Remove Section</span></asp:LinkButton>
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
                                        <asp:Label ID="lblResult" runat="server" Visible="false"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <table style="width: 100%;" class="wrapper" runat="server" id="tblPricingWrapper" visible="false">
                                            <tr>
                                                <td align="left" rowspan="5" valign="top">
                                                    <asp:TreeView ID="trvSection" runat="server" ImageSet="Msdn" NodeIndent="10" OnSelectedNodeChanged="trvSection_SelectedNodeChanged">
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
                                                    <asp:TextBox ID="txtSearchItemName" onkeypress="return SearchItemNamePress(event);" runat="server" Width="50%" Visible="False"></asp:TextBox>
                                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemName" ServiceMethod="GetItemName" TargetControlID="txtSearchItemName" UseContextKey="True">
                                                    </cc1:AutoCompleteExtender>
                                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearchItemName" WatermarkText="Search by Item Name" />
                                                    <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" Visible="False" />
                                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click" Visible="False">View All</asp:LinkButton>
                                                </td>
                                                <td>&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">
                                                    <asp:GridView ID="grdItemPrice" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                        DataKeyNames="item_id" OnRowCommand="grdItemPrice_RowCommand" PageSize="200"
                                                        TabIndex="2" Width="100%" OnRowDataBound="grdItemPrice_RowDataBound">
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
                                                                    <asp:TextBox ID="txtShortNote" runat="server" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
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
                                                            <%-- <asp:TemplateField HeaderText="Ext. Price">
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
                                                            <td align="Center" style=" width:400px;"><b style="font-weight: bold; color: #555;">Item Name</b>
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
                                                            <td class="tabGridtd" >
                                                                <asp:TextBox ID="txtCustomizeUOM" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeUnitPrice" runat="server" Style="text-align: right;" ></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtCustomizeCode" runat="server" Style="text-align: center;" ></asp:TextBox>
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
                                                                <asp:TextBox ID="txtCustomezeNotes" runat="server"  TextMode="MultiLine" MaxLength="500"></asp:TextBox>
                                                            </td>

                                                        </tr>
                                                    </table>
                                                    <table class="tabGrid" width="100%" cellpadding="0" cellspacing="0" id="tdlOther" runat="server" height="auto" visible="false">
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
                                                            <td>&nbsp;</td>

                                                        </tr>
                                                        <tr>
                                                            <td class="tabGridtd">
                                                                <asp:Button ID="btnAddOthers" runat="server" CssClass="button" OnClick="btnAddOthers_Click"
                                                                    Text="Add" />
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtOther" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd" >
                                                                <asp:TextBox ID="txtO_Unit" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtO_Price" runat="server" Style="text-align: right;"></asp:TextBox>
                                                            </td>
                                                            <td class="tabGridtd">
                                                                <asp:TextBox ID="txtO_Qty" runat="server" Style="text-align: center;"></asp:TextBox>
                                                            </td>
                                                            <%-- <td valign="top">
                                                                <asp:TextBox ID="txtO_TotalPrice" Style="text-align: right;" runat="server" ReadOnly="True" Width="40px"></asp:TextBox>
                                                            </td>--%>
                                                            <td class="tabGridtd">
                                                                <asp:DropDownList ID="ddlO_Direct" runat="server">
                                                                    <asp:ListItem Value="1">No</asp:ListItem>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td class="tabGridtd" valign="top">
                                                                <asp:TextBox ID="txtO_ShortNotes" runat="server"  TextMode="MultiLine" MaxLength="500"></asp:TextBox>
                                                            </td>
                                                            <td>&nbsp;</td>
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
                                                    <asp:TextBox ID="txtSearchAll" onkeypress="return SearchAllPress(event);" runat="server" Width="50%"></asp:TextBox>
                                                    <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemNameAll" ServiceMethod="GetItemNameAll" TargetControlID="txtSearchAll" UseContextKey="True">
                                                    </cc1:AutoCompleteExtender>
                                                    <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtSearchAll" WatermarkText="Search by Item Name" />
                                                    <asp:Button ID="btnSearchAll" runat="server" CssClass="button" OnClick="btnSearchAll_Click" Text="Search" />
                                                    &nbsp;<asp:LinkButton ID="lnkClear" runat="server" OnClick="lnkClear_Click">Clear Search Text Box</asp:LinkButton>
                                                    &nbsp;&nbsp;
                                                    <asp:LinkButton ID="lnkResetAll" runat="server" OnClick="lnkResetAll_Click">Reset All Search Item</asp:LinkButton>
                                                </td>
                                                <td align="right">
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
                                                                    <asp:TextBox ID="txtShortNote" runat="server" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
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
                                            <tr>
                                                <td colspan="2">
                                                    <asp:GridView ID="grdRecentlyAdded" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                        DataKeyNames="item_id" PageSize="200"
                                                        TabIndex="2" Width="100%" OnRowDeleting="grdRecentlyAdded_RowDeleting">
                                                        <Columns>
                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                <HeaderStyle Width="5%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                <HeaderStyle Width="5%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="location_name" HeaderText="Location name" ReadOnly="True">
                                                                <HeaderStyle Width="10%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="section_name" HeaderText="Section name" ReadOnly="True">
                                                                <HeaderStyle Width="10%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name" ReadOnly="True">
                                                                <HeaderStyle Width="25%" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True" NullDisplayText=" ">
                                                                <HeaderStyle Width="6%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                <HeaderStyle Width="5%" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Code">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblquantity" runat="server" Text='<%# Eval("quantity") %>' />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="5%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Ext. Price">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="7%" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                <HeaderStyle Width="1%" />
                                                                <ItemStyle HorizontalAlign="Right" />
                                                            </asp:BoundField>

                                                            <asp:ButtonField CommandName="Delete" Text="Undo">
                                                                <HeaderStyle Width="5%" />
                                                            </asp:ButtonField>
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
                                        <asp:Label ID="lblAdd" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table id="tblTotalProjectPrice" runat="server" cellpadding="8" cellspacing="0" style="border: 1px solid #c0c0c0;"
                                            align="center" width="60%">
                                            <tr>
                                                <td colspan="3" style="border: 1px solid #c0c0c0;" align="center">
                                                    <h3>Total Project Price</h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <strong>Total Price </strong>
                                                </td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <strong>Direct Price</strong>
                                                </td>
                                                <td style="border: 1px solid #c0c0c0;" align="center" valign="top">
                                                    <strong>Total Price +&nbsp; Direct Price</strong>
                                                </td>
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
                                        <asp:RadioButtonList ID="rdoSort" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdoSort_SelectedIndexChanged"
                                            RepeatDirection="Horizontal">
                                            <asp:ListItem Selected="True" Value="1">View by Locations</asp:ListItem>
                                            <asp:ListItem Value="2">View by Sections</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" style="height: 26px">
                                        <h3>
                                            <asp:Label ID="lblRetailPricingHeader" runat="server" CssClass="gridTitle" Text="Selected Items" Visible="false"></asp:Label>
                                        </h3>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:LinkButton ID="lnkUpdateLocation" runat="server" CssClass="imageButton">Change Item Locations</asp:LinkButton>
                                        <cc1:ModalPopupExtender ID="lnkUpdateLocation_ModalPopupExtender" runat="server"
                                            BackgroundCssClass="modalBackground" DropShadow="false" PopupControlID="pnlUpdateEstimate"
                                            TargetControlID="lnkUpdateLocation">
                                        </cc1:ModalPopupExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True" OnRowDataBound="grdGrouping_RowDataBound"
                                            AutoGenerateColumns="False" CssClass="mGrid" Width="100%" OnRowCommand="grdGrouping_RowCommand">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <%--<asp:CheckBox ID="chkAllItem" runat="server" AutoPostBack="True" OnCheckedChanged="Chek_UnChek_All" />--%>

                                                        <table style="padding: 0px; margin: 0px; width: 100%">
                                                            <tr>
                                                                <td style="padding: 0px; margin: 0px;" align="left">
                                                                    <table style="padding: 0px; margin: 0px;">
                                                                        <tr>
                                                                            <td style="padding: 0px; margin: 0px;">
                                                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" /></td>
                                                                            <td>
                                                                                <asp:Label ID="lblMSub" Font-Bold="true" Font-Size="15px" runat="server" />
                                                                            </td>
                                                                            <td style="padding: 0px; margin: 0px;">
                                                                                <asp:LinkButton ID="lnkMove1" runat="server" CssClass="moveUp" CommandName="Move" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Move to Top</asp:LinkButton></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px;" align="right">
                                                                    <table style="padding: 0px; margin: 0px;">
                                                                        <tr>
                                                                            <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                <asp:LinkButton ID="btnCopyItems" runat="server" CssClass="rightFloatManual" Style="text-decoration: underline;" CommandName="CopyItem">Copy Selected Item(s)</asp:LinkButton></td>
                                                                            <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                <asp:LinkButton ID="btnEditItemGrid" runat="server" CssClass="rightFloatManual" Style="text-decoration: underline;" CommandName="EditItem">Edit Selected Item(s)</asp:LinkButton></td>
                                                                            <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                <asp:LinkButton ID="btnDeleteItemGrid" runat="server" CssClass="rightFloatManual" Style="text-decoration: underline; padding-left: 5px;" CommandName="Del">Delete Selected Item(s)</asp:LinkButton></td>
                                                                            <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                <asp:LinkButton ID="lnkSendEmail1" runat="server" CssClass="sendEmail" CommandName="sEmail" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>

                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                                            Width="100%" CssClass="mGrid">
                                                            <Columns>
                                                                <%--<asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkCheck" runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>--%>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                        <asp:TextBox ID="txtshort_notes" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                            TextMode="MultiLine" />
                                                                        <asp:Label ID="lblshort_notes_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                        <asp:LinkButton ID="lnkOpen" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="13%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <%-- <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>--%>
                                                                    <HeaderStyle Width="11%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Item Name">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName1" runat="server" Text='<%# Eval("item_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <HeaderStyle Width="22%" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="UoM">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMeasureUnit" runat="server" Text='<%# Eval("measure_unit").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="6%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
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
                                                                        <br />
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
                                                                        <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" /><br />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                    <HeaderStyle Width="1%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                                <%-- <asp:ButtonField CommandName="Edit" Text="Edit">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:ButtonField>--%>
                                                                <%--<asp:ButtonField CommandName="Delete" Text="Delete">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:ButtonField>--%>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this);" TextAlign="Left" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSingle" runat="server" onclick="Check_Click(this)" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>

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
                                                                <td style="padding: 0px; margin: 0px; width: 13%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 11%;"></td>
                                                                <td style="padding: 0px; margin: 0px; width: 22%;">
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
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" Width="100%"
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound" ShowFooter="True"
                                                        CaptionAlign="Top" OnRowCommand="grdGroupingDirect_RowCommand">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <table style="padding: 0px; margin: 0px; width: 100%">
                                                                        <tr>
                                                                            <td style="padding: 0px; margin: 0px;" align="left">
                                                                                <table style="padding: 0px; margin: 0px;">
                                                                                    <tr>
                                                                                        <td style="padding: 0px; margin: 0px;">
                                                                                            <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' /></td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblMSub2" Font-Bold="true" Font-Size="15px" runat="server" />
                                                                                        </td>
                                                                                        <td style="padding: 0px; margin: 0px;">
                                                                                            <asp:LinkButton ID="lnkMove2" runat="server" CommandName="Move1" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                                                                CssClass="moveUp">Move to Top</asp:LinkButton></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                            <td style="padding: 0px; margin: 0px;" align="right">
                                                                                <table style="padding: 0px; margin: 0px;">
                                                                                    <tr>
                                                                                        <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                                  <asp:LinkButton ID="btnCopyItems2" runat="server" CssClass="rightFloatManual" Style="text-decoration: underline;" CommandName="CopyItem1">Copy Selected Item(s)</asp:LinkButton></td>
                                                                                                                                <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                            <asp:LinkButton ID="btnEditItemGrid2" runat="server" CssClass="rightFloatManual" Style="text-decoration: underline;" CommandName="EditItem1">Edit Selected Item(s)</asp:LinkButton></td>
                                                                                        <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                            <asp:LinkButton ID="btnDeleteItemGrid2" runat="server" CssClass="rightFloatManual" CommandName="Del1">Delete Selected Item(s)</asp:LinkButton></td>
                                                
                                                                                        <td style="padding: 0px; margin: 0px;" valign="middle">
                                                                                            <asp:LinkButton ID="lnkSendEmail2" runat="server" CssClass="sendEmail" CommandName="sEmail" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'>Request for Bid</asp:LinkButton></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>





                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                        DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound" OnRowDeleting="grdSelectedItem2_RowDeleting"
                                                                        OnRowEditing="grdSelectedItem2_RowEditing" ShowFooter="True" Width="100%" OnRowUpdating="grdSelectedItem2_RowUpdating">
                                                                        <Columns>
                                                                            <%-- <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chkCheck1" runat="server" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>--%>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblshort_notes1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                    <asp:TextBox ID="txtshort_notes1" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                                        TextMode="MultiLine" />
                                                                                    <asp:Label ID="lblshort_notes1_r" runat="server" Text='<%# Eval("short_notes") %>' Visible="false" />
                                                                                    <asp:LinkButton ID="lnkOpen1" Text="More" ToolTip="Click here to view more" OnClick="lnkOpen1_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="13%" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <%-- <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>--%>
                                                                                <HeaderStyle Width="11%" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Item Name">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName3" runat="server" Text='<%# Eval("item_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <HeaderStyle Width="22%" />
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
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                                <HeaderStyle Width="5%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Unit Cost">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblUnit_Cost3" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Qty">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblquantity1" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                    <asp:TextBox ID="txtquantity1" runat="server" Visible="false" Style="text-align: center;" Width="40px" Text='<%# Eval("quantity") %>'
                                                                                        AutoPostBack="True" OnTextChanged="Direct_calculation" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="5%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
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
                                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                                <HeaderStyle Width="1%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                            </asp:BoundField>
                                                                            <%--   <asp:ButtonField CommandName="Edit" Text="Edit">
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:ButtonField>--%>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:CheckBox ID="chkAllD" runat="server" onclick="checkAll(this);" TextAlign="Left" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chkSingleD" runat="server" onclick="Check_Click(this)" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:TemplateField>
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
                                                                            <td style="padding: 0px; margin: 0px; width: 13%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 11%;"></td>
                                                                            <td style="padding: 0px; margin: 0px; width: 22%;">
                                                                                <asp:Label ID="Label2" runat="server" Text='<%# GetTotalPriceDirect()%>' />
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
                                    <td align="center" valign="top">
                                        <asp:Label ID="lblResult1" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnGotoCustomerList" runat="server" OnClick="btnGotoCustomerList_Click"
                                            Text="Go to Customer List" CssClass="button" />
                                        <%--  <asp:Button ID="btnSavePopUp" runat="server" Text="Save" Width="80px" CssClass="button"
                                            OnClick="btnSavePopUp_Click" Visible="False" />--%>
                                        &nbsp;<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"
                                            Width="80px" CssClass="button" />
                                        <asp:Button ID="btnGoToPayment" runat="server" OnClick="btnGoToPayment_Click" Text="Go to Payment"
                                            CssClass="button" />
                                        <asp:Button ID="btnSchedule" runat="server" CssClass="button" OnClick="btnSchedule_Click"
                                            Text="Schedule" Visible="False" />
                                        <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
                                        <asp:LinkButton ID="lnkDummy2" runat="server"></asp:LinkButton>
                                        <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>
                                         <asp:LinkButton ID="InkCopyDummy" runat="server"></asp:LinkButton>
                                        <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <cc1:ModalPopupExtender ID="modUpdateEstimate" runat="server" PopupControlID="pnlUpdateEstimate"
                                            TargetControlID="lnkUpdateEstimate" BackgroundCssClass="modalBackground" DropShadow="false">
                                        </cc1:ModalPopupExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <cc1:ModalPopupExtender ID="modUpdateLocation" runat="server" PopupControlID="pnlChangeLocation"
                                            TargetControlID="lnkUpdateLocation" BackgroundCssClass="modalBackground" DropShadow="false">
                                        </cc1:ModalPopupExtender>
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender4" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground"
                                            CancelControlID="btnCancelItem" PopupControlID="pnlDeletedItem"
                                            runat="server">
                                        </cc1:ModalPopupExtender>
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender5" TargetControlID="lnkDummy3" BackgroundCssClass="modalBackground"
                                            CancelControlID="btnCancelItem2" PopupControlID="pnlEditItem"
                                            runat="server">
                                        </cc1:ModalPopupExtender>
                                          <cc1:ModalPopupExtender ID="modCopyItems" TargetControlID="InkCopyDummy" BackgroundCssClass="modalBackground"
                                            CancelControlID="btnCancelCopyItem" PopupControlID="pnlCopyItem"
                                            runat="server">
                                        </cc1:ModalPopupExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender2" BehaviorID="mpe" runat="server"
                                            PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground" CancelControlID="btnHide">
                                        </cc1:ModalPopupExtender>
                                    </td>
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
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnItemCnt" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnLastName" runat="server" />
                                                    <asp:HiddenField ID="hdnSecName" runat="server" />
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False" Value="0" />
                                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                                    <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="btnDuplicate"
                                                        OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                                                    </cc1:ConfirmButtonExtender>
                                                    <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="btnDuplicate" BackgroundCssClass="modalBackground"
                                                        CancelControlID="btnCancel" OkControlID="btnOk" PopupControlID="pnlConfirmation"
                                                        runat="server">
                                                    </cc1:ModalPopupExtender>
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
                                <tr>
                                    <td align="left" valign="top">
                                        <table cellpadding="2" cellspacing="2" align="center" width="100%">
                                            <tr>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSectionLevel" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnOtherId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnModelEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSortDesc" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnMandatoryId" runat="server" EnableViewState="False" Value="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="pnlChangeLocation" runat="server" Width="550px" Height="150px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">
                                            <b>Change Location</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="right">
                                                        <b>New Location Name: </b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlNewLocations" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Label ID="lblMessLoc" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">&nbsp;<asp:Button ID="btnChangeLoc" runat="server" Text="Proceed to change" TabIndex="2"
                                                        OnClick="btnChangeLoc_Click" CssClass="button" />
                                                        &nbsp;<asp:Button ID="btnCloseLoc" runat="server" Text="Close" TabIndex="3" Width="80px"
                                                            OnClick="btnCloseLoc_Click" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
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
                    <asp:Panel ID="pnlPopup" runat="server" Width="550px" Height="100px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>
                                                <asp:Label ID="lblReqmsg1" runat="server"></asp:Label></b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Above item(s) must be included in this section</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnHide" runat="server" Text="Close" CssClass="button" Width="60px" />
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
                    <asp:Panel ID="pnlConfirmation" runat="server" Width="550px" Height="100px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Clicking &#39;Yes&#39; will create a duplicate instance of this estimate pricing.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Are you sure you want to save this duplicate estimate pricing?</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnOk" runat="server" Text="Yes" CssClass="button" Width="60px" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                                        </td>
                                    </tr>

                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <asp:Panel ID="pnlDeletedItem" runat="server" Style="width: auto; overflow: scroll; max-height: 700px;" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="center">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">
                                                <tr>
                                                    <td align="center">
                                                        <b>Following items will be deleted upon clicking on confirm.</b>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <b>Are you sure that you want to delete the Item(s)? </b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:GridView ID="grdDeletedItem" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                            DataKeyNames="item_id" PageSize="200"
                                                            TabIndex="2" Width="100%">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="location_name" HeaderText="Location name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_name" HeaderText="Section name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name" ReadOnly="True">
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True">
                                                                    <HeaderStyle Width="6%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblquantity" runat="server" Text='<%# Eval("quantity") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                    <HeaderStyle Width="1%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">&nbsp;<asp:Button ID="btnDeleteItem" runat="server" Text="Confirm Delete" TabIndex="2"
                                                        CssClass="button" OnClick="btnDeleteItem_Click" />
                                                        &nbsp;<asp:Button ID="btnCancelItem" runat="server" Text="Cancel" TabIndex="3" Width="80px"
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
                       <!--- Copy Item  -->
                    <asp:Panel ID="pnlCopyItem" runat="server" Style="width: auto; overflow: scroll; max-height: 600px;" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td style="padding:10px;" align="center">
                                            <strong style="padding:10px 5px 5px 5px;">Copy the following items to the desired location from the Location Name drop-down below.</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="2" width="98%">

                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:GridView ID="grdCopyItem" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                            DataKeyNames="item_id" PageSize="200"
                                                            TabIndex="2" Width="100%" OnRowDataBound="grdCopyItem_RowDataBound">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="location_name" HeaderText="Location name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_name" HeaderText="Section name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Item Name">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblitem_name" runat="server" Text='<%# Eval("item_name") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtshort_notes" CssClass="csstComments" TextMode="MultiLine" runat="server" Text='<%# Eval("short_notes") %>' Width="150px"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True">
                                                                    <HeaderStyle Width="6%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Unit Cost">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' Width="50px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="RETAIL MULTIPLIER">

                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblretail_multiplier" runat="server" Text='<%# Eval("retail_multiplier").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtretail_multiplier" runat="server" Text='<%# Eval("retail_multiplier").ToString() %>' Width="40px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />

                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Labor Rate">

                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalLabor_Rate" runat="server" Text='<%# Eval("labor_rate","{0:c}").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtTotalLabor_Rate" runat="server" Text='<%# Eval("labor_rate","{0:c}").ToString() %>' Width="50px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Labor Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLaborCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalLabor_Cost" runat="server" Text='<%# Eval("total_labor_cost","{0:c}").ToString() %>' Visible="true" />

                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>



                                                                <asp:TemplateField HeaderText="Ext. Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                        <br />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_Cost" runat="server" Text='<%# Eval("total_unit_cost","{0:c}").ToString() %>' />

                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtquantity" runat="server" Text='<%# Eval("quantity") %>' Width="30px"></asp:TextBox>
                                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" TargetControlID="txtquantity"
                                                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                                        </cc1:FilteredTextBoxExtender>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Price w/cash discount">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtTotalPrice" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' Width="60px"></asp:TextBox>
                                                                        <asp:Label ID="lblTotal_priceORG" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                    <HeaderStyle Width="1%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>

                                                    <td align="center">
                                                        <table align="center" width="100%">
                                                            <tr>
                                                                <td width="100%" align="center">
                                                                    <table align="center" width="100%">
                                                                        <tr>
                                                                            <td style="width: 50%;" align="right">
                                                                                <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="*"></asp:Label> <b>Location Name: </b>
                                                                            </td>
                                                                            <td style="width: 50%;" align="left">
                                                                                <asp:DropDownList ID="ddCopyLocation" runat="server">
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
                                                    <td style="width: 100%;">
                                                        <table align="center" width="100%">
                                                            <tr>
                                                                <td style="width: 100%;" align="center">
                                                                    <table align="center" width="100%">
                                                                        <tr>
                                                                            <td style="width: 50%;" align="right">&nbsp;
                                                                            </td>
                                                                            <td style="width: 50%;" align="left">
                                                                                <table cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td style="width: 100%;" align="left">
                                                                                            <asp:Label ID="lblLocationCopy" runat="server" Text=""></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <table align="center" width="100%">
                                                                        <tr>
                                                                            <td style="width: 50%;" align="right">&nbsp;
                                                                            </td>
                                                                            <td style="width: 50%;" align="left">
                                                                                <table cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Button ID="btnCopy" runat="server" Text="Copy & Save" TabIndex="2"
                                                                                                CssClass="button" OnClick="btnCopy_Click" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Button ID="btnCancelCopyItem" runat="server" Text="Cancel" TabIndex="3"
                                                                                                CssClass="button" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <asp:Panel ID="pnlEditItem" runat="server" Style="width: auto; overflow: scroll; max-height: 600px;" BackColor="Snow">
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
                                                        <asp:GridView ID="grdEditItem" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                            DataKeyNames="item_id" PageSize="200"
                                                            TabIndex="2" Width="100%" OnRowDataBound="grdEditItem_RowDataBound">
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="location_name" HeaderText="Location name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_name" HeaderText="Section name" ReadOnly="True">
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Item Name">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblitem_name" runat="server" Text='<%# Eval("item_name") %>' />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtshort_notes" CssClass="csstComments" TextMode="MultiLine" runat="server" Text='<%# Eval("short_notes") %>' Width="150px"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True">
                                                                    <HeaderStyle Width="6%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Unit Cost">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtUnit_Cost" runat="server" Text='<%# Eval("unit_cost","{0:c}").ToString() %>' Width="50px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="RETAIL MULTIPLIER">

                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblretail_multiplier" runat="server" Text='<%# Eval("retail_multiplier").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtretail_multiplier" runat="server" Text='<%# Eval("retail_multiplier").ToString() %>' Width="40px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />

                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Labor Rate">

                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalLabor_Rate" runat="server" Text='<%# Eval("labor_rate","{0:c}").ToString() %>' Visible="false" />
                                                                        <asp:TextBox ID="txtTotalLabor_Rate" runat="server" Text='<%# Eval("labor_rate","{0:c}").ToString() %>' Width="50px" Visible="false"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Labor Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLaborCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalLabor_Cost" runat="server" Text='<%# Eval("total_labor_cost","{0:c}").ToString() %>' Visible="true" />

                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>



                                                                <asp:TemplateField HeaderText="Ext. Cost">
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalCost" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                        <br />
                                                                    </FooterTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal_Cost" runat="server" Text='<%# Eval("total_unit_cost","{0:c}").ToString() %>' />

                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                    <FooterStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtquantity" runat="server" Text='<%# Eval("quantity") %>' Width="30px"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                 <asp:TemplateField HeaderText="Direct">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlDirect" runat="server">
                                                                        <asp:ListItem Value="1">No</asp:ListItem>
                                                                        <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center"  Width="5%"/>
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Ext. Price">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtTotalPrice" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' Width="60px"></asp:TextBox>
                                                                        <asp:Label ID="lblTotal_price" runat="server" Text='<%# Eval("total_retail_price","{0:c}").ToString() %>' Visible="false" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="7%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False">
                                                                    <HeaderStyle Width="1%" />
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>

                                                    <td align="center">
                                                        <table width="100%">
                                                            <tr>
                                                                <td align="right" style="width: 50%">&nbsp;<asp:Button ID="btnSaveItem" runat="server" Text="Save" Width="80px" TabIndex="2"
                                                                    CssClass="button" OnClick="btnSaveItem_Click" />
                                                                    &nbsp;<asp:Button ID="btnCancelItem2" runat="server" Text="Cancel" TabIndex="3" Width="80px"
                                                                        CssClass="button" />



                                                                </td>
                                                                <td align="center">
                                                                    <asp:RadioButtonList ID="rdbmulExprice" runat="server" RepeatDirection="Horizontal" Visible="false" OnSelectedIndexChanged="rdbmulExprice_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Selected="True" Value="0">Calc based on Retail Multiplier</asp:ListItem>
                                                                        <asp:ListItem Value="1">Calc based on Ex Price</asp:ListItem>
                                                                    </asp:RadioButtonList>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblMSG" runat="server" Text=""></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
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
