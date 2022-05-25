<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="PricingMangement.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="PricingMangement" Title="Pricing Mangement" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
        // It is important to place this JavaScript code after ScriptManager1
        var xPos, yPos;
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        function BeginRequestHandler(sender, args) {
            if ($get('<%=treePanel.ClientID%>') != null) {
                // Get X and Y positions of scrollbar before the partial postback
                xPos = $get('<%=treePanel.ClientID%>').scrollLeft;
                yPos = $get('<%=treePanel.ClientID%>').scrollTop;
            }
        }

        function EndRequestHandler(sender, args) {
            if ($get('<%=treePanel.ClientID%>') != null) {
                // Set X and Y positions back to the scrollbar
                // after partial postback
                $get('<%=treePanel.ClientID%>').scrollLeft = xPos;
                $get('<%=treePanel.ClientID%>').scrollTop = yPos;
            }
        }

        prm.add_beginRequest(BeginRequestHandler);
        prm.add_endRequest(EndRequestHandler);

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
    </script>
    <%--<table>
        <tr>
            <td>
                <asp:FileUpload ID="ExcelUploader" runat="server"></asp:FileUpload>
                <asp:Button ID="btnUpload" runat="server" CssClass="button" Text="Load" OnClick="btnUpload_Click" />
            </td>
        </tr>
    </table>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Pricing Management</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td align="center">
                        <table style="width: 100%;">
                            <tr>
                                <td width="300" valign="top">
                                    <table cellpadding="0" cellspacing="2">
                                        <tr>
                                            <td align="left" colspan="2" valign="top" style="width: 500px; height: 16px;">
                                                <asp:Button ID="btnHome" runat="server" CssClass="homeButton" OnClick="btnHome_Click"
                                                    Text="HOME" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top">
                                                <asp:Panel runat="Server" ID="treePanel" Style="overflow: auto">
                                                    <asp:TreeView ID="trvSection" runat="server" OnSelectedNodeChanged="trvSection_SelectedNodeChanged"
                                                        ImageSet="Arrows" Height="600px" Width="150px" AutoGenerateDataBindings="False"
                                                        ExpandDepth="0" OnTreeNodeExpanded="trvSection_TreeNodeExpanded" SelectAction="Expand">
                                                        <ParentNodeStyle Font-Bold="False" />
                                                        <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                                        <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD" HorizontalPadding="0px"
                                                            VerticalPadding="0px" />
                                                        <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px"
                                                            NodeSpacing="0px" VerticalPadding="0px" />
                                                    </asp:TreeView>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td align="center" colspan="2" valign="top">
                                                <table width="100%" class="subHeader01">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblMainSection" runat="server" Text="Main Section"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" style="width: 500px; height: 16px;" valign="top">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblMainSecResult" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">&nbsp;&nbsp;
                                                            <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click" Text="Add New Section" Visible="False" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:GridView ID="grdMainSection" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                OnRowCommand="grdMainSection_RowCommand" OnRowDataBound="grdMainSection_RowDataBound"
                                                                OnRowEditing="grdMainSection_RowEditing" OnRowUpdating="grdMainSection_RowUpdating"
                                                                PageSize="200" TabIndex="2" Width="100%" AllowSorting="True" OnSorting="grdMainSection_Sorting">
                                                                <Columns>

                                                                    <%-- Cell 0 --%>
                                                                    <asp:TemplateField HeaderText="Section Name" SortExpression="section_name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSectionName" runat="server" Text='<%# Eval("section_name") %>' />
                                                                            <asp:TextBox ID="txtSectionName" runat="server" Text='<%# Eval("section_name") %>'
                                                                                Visible="false" Width="320px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 1 --%>
                                                                    <asp:TemplateField HeaderText="Color">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblcssClassName" runat="server" CssClass="fc-DarkBlue" />
                                                                            <asp:DropDownList ID="ddlcssClassName" runat="server" Visible="false" OnSelectedIndexChanged="GetCssClassName" AutoPostBack="true">
                                                                                <asp:ListItem Text="RoyalBlue" Value="fc-RoyalBlue" class="fc-RoyalBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="CadetBlue" Value="fc-CadetBlue" class="fc-CadetBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="Blue" Value="fc-Blue" class="fc-Blue"></asp:ListItem>
                                                                                <asp:ListItem Text="Coral" Value="fc-Coral" class="fc-Coral"></asp:ListItem>
                                                                                <asp:ListItem Text="BlueViolet" Value="fc-BlueViolet" class="fc-BlueViolet"></asp:ListItem>
                                                                                <asp:ListItem Text="Brown" Value="fc-Brown" class="fc-Brown"></asp:ListItem>
                                                                                <asp:ListItem Text="Chartreuse" Value="fc-Chartreuse" class="fc-Chartreuse"></asp:ListItem>
                                                                                <asp:ListItem Text="CornflowerBlue" Value="fc-CornflowerBlue" class="fc-CornflowerBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="Crimson" Value="fc-Crimson" class="fc-Crimson"></asp:ListItem>
                                                                                <asp:ListItem Text="DarkOliveGreen" Value="fc-DarkOliveGreen" class="fc-DarkOliveGreen"></asp:ListItem>
                                                                                <asp:ListItem Text="MediumPurple" Value="fc-MediumPurple" class="fc-MediumPurple"></asp:ListItem>
                                                                                <asp:ListItem Text="Yellow" Value="fc-Yellow" class="fc-Yellow"></asp:ListItem>
                                                                                <asp:ListItem Text="GoldenRod" Value="fc-GoldenRod" class="fc-GoldenRod"></asp:ListItem>
                                                                                <asp:ListItem Text="Violet" Value="fc-Violet" class="fc-Violet"></asp:ListItem>
                                                                                <asp:ListItem Text="Tomato" Value="fc-Tomato" class="fc-Tomato"></asp:ListItem>
                                                                                <asp:ListItem Text="Indigo" Value="fc-Indigo" class="fc-Indigo"></asp:ListItem>
                                                                                <asp:ListItem Text="Teal" Value="fc-Teal" class="fc-Teal"></asp:ListItem>
                                                                                <asp:ListItem Text="Tan" Value="fc-Tan" class="fc-Tan"></asp:ListItem>
                                                                                <asp:ListItem Text="SteelBlue" Value="fc-SteelBlue" class="fc-SteelBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="SpringGreen" Value="fc-SpringGreen" class="fc-SpringGreen"></asp:ListItem>
                                                                                <asp:ListItem Text="SlateGrey" Value="fc-SlateGrey" class="fc-SlateGrey"></asp:ListItem>
                                                                                <asp:ListItem Text="SlateBlue" Value="fc-SlateBlue" class="fc-SlateBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="SkyBlue" Value="fc-SkyBlue" class="fc-SkyBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="SeaGreen" Value="fc-SeaGreen" class="fc-SeaGreen"></asp:ListItem>
                                                                                <asp:ListItem Text="SandyBrown" Value="fc-SandyBrown" class="fc-SandyBrown"></asp:ListItem>
                                                                                <asp:ListItem Text="IndianRed" Value="fc-IndianRed" class="fc-IndianRed"></asp:ListItem>
                                                                                <asp:ListItem Text="Aqua" Value="fc-Aqua" class="fc-Aqua"></asp:ListItem>
                                                                                <asp:ListItem Text="RosyBrown" Value="fc-RosyBrown" class="fc-RosyBrown"></asp:ListItem>
                                                                                <asp:ListItem Text="Red" Value="fc-Red" class="fc-Red"></asp:ListItem>
                                                                                <asp:ListItem Text="Purple" Value="fc-Purple" class="fc-Purple"></asp:ListItem>
                                                                                <asp:ListItem Text="YellowGreen" Value="fc-YellowGreen" class="fc-YellowGreen"></asp:ListItem>
                                                                                <asp:ListItem Text="RebeccaPurple" Value="fc-RebeccaPurple" class="fc-RebeccaPurple"></asp:ListItem>
                                                                                <asp:ListItem Text="DarkSlateGray" Value="fc-DarkSlateGray" class="fc-DarkSlateGray"></asp:ListItem>
                                                                                <asp:ListItem Text="DarkOrange" Value="fc-DarkOrange" class="fc-DarkOrange"></asp:ListItem>
                                                                                <asp:ListItem Text="PaleVioletRed" Value="fc-PaleVioletRed" class="fc-PaleVioletRed"></asp:ListItem>
                                                                                <asp:ListItem Text="OliveDrab" Value="fc-OliveDrab" class="fc-OliveDrab"></asp:ListItem>
                                                                                <asp:ListItem Text="DeepPink" Value="fc-DeepPink" class="fc-DeepPink"></asp:ListItem>
                                                                                <asp:ListItem Text="OrangeRed" Value="fc-OrangeRed" class="fc-OrangeRed"></asp:ListItem>
                                                                                <asp:ListItem Text="Orange" Value="fc-Orange" class="fc-Orange"></asp:ListItem>
                                                                                <asp:ListItem Text="Navy" Value="fc-Navy" class="fc-Navy"></asp:ListItem>
                                                                                <asp:ListItem Text="DimGrey" Value="fc-DimGrey" class="fc-DimGrey"></asp:ListItem>
                                                                                <asp:ListItem Text="MediumVioletRed" Value="fc-MediumVioletRed" class="fc-MediumVioletRed"></asp:ListItem>
                                                                                <asp:ListItem Text="Magenta" Value="fc-Magenta" class="fc-Magenta"></asp:ListItem>
                                                                                <asp:ListItem Text="MidnightBlue" Value="fc-MidnightBlue" class="fc-MidnightBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="Green" Value="fc-Green" class="fc-Green"></asp:ListItem>
                                                                                <asp:ListItem Text="MediumSlateBlue" Value="fc-MediumSlateBlue" class="fc-MediumSlateBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="MediumOrchid" Value="fc-MediumOrchid" class="fc-MediumOrchid"></asp:ListItem>
                                                                                <asp:ListItem Text="LimeGreen" Value="fc-LimeGreen" class="fc-LimeGreen"></asp:ListItem>
                                                                                <asp:ListItem Text="Salmon" Value="fc-Salmon" class="fc-Salmon"></asp:ListItem>
                                                                                <asp:ListItem Text="DarkSlateBlue" Value="fc-DarkSlateBlue" class="fc-DarkSlateBlue"></asp:ListItem>
                                                                                <asp:ListItem Text="DarkBlue" Value="fc-DarkBlue" class="fc-DarkBlue"></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 2 --%>
                                                                    <asp:TemplateField HeaderText="Division">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblDivision" runat="server" Text="" Visible="true"></asp:Label>
                                                                            <asp:DropDownList ID="ddlDivision" runat="server" Visible="false" Width="70%"></asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 3 --%>
                                                                    <asp:TemplateField HeaderText="Active">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblIsActive" runat="server" />
                                                                            <asp:CheckBox ID="chkIsActive" runat="server" Checked='<%# Eval("is_active") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 4 --%>
                                                                    <asp:TemplateField HeaderText="Ex. Comm.">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAExcludeCom0" runat="server" />
                                                                            <asp:CheckBox ID="chkIsExcludeCom0" runat="server" Checked='<%# Eval("is_CommissionExclude") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 5 --%>
                                                                    <asp:ButtonField CommandName="Edit" Text="Edit" />

                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2" valign="top">
                                                <asp:Label ID="lblTree" runat="server">Tree Navigation:</asp:Label>
                                                <asp:Label ID="lblParent" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" valign="top">
                                                <table width="100%" class="subHeader01">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblSubSection" runat="server" Text="Sub Section" Visible="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: #eaeff9;" align="center" colspan="2">
                                                <table>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblSubSecResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <%-- <asp:Button ID="btnUpdateSerial" runat="server" CssClass="button" OnClick="btnUpdateSerial_Click" Text="Update Serial" Visible="true" />--%>
                                                            &nbsp;
                                                            <asp:Button ID="btnAddSubnewRow" runat="server" CssClass="button" OnClick="btnAddSubnewRow_Click" Text="Add New Sub Section" Visible="False" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:GridView ID="grdSubSection" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                OnRowCommand="grdSubSection_RowCommand" OnRowDataBound="grdSubSection_RowDataBound"
                                                                OnRowEditing="grdSubSection_RowEditing" OnRowUpdating="grdSubSection_RowUpdating"
                                                                PageSize="200" TabIndex="2" Width="500px" AllowSorting="True" OnSorting="grdSubSection_Sorting">
                                                                <Columns>
                                                                    <%-- Cell 0 --%>
                                                                    <asp:TemplateField HeaderText="Sub Section Name" SortExpression="section_name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSubSectionName" runat="server" Text='<%# Eval("section_name") %>' />
                                                                            <asp:TextBox ID="txtSubSectionName" runat="server" Text='<%# Eval("section_name") %>'
                                                                                Visible="false" Width="320px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 1 --%>
                                                                    <asp:TemplateField HeaderText="Division">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSubDivision" runat="server" Text="" Visible="true"></asp:Label>
                                                                            <asp:DropDownList ID="ddlSubDivision" runat="server" Visible="false"></asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="20%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 2 --%>
                                                                    <asp:TemplateField HeaderText="Active">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblIsActive1" runat="server" />
                                                                            <asp:CheckBox ID="chkIsActive1" runat="server" Checked='<%# Eval("is_active") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 3 --%>
                                                                    <asp:TemplateField HeaderText="Ex. Comm.">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAExcludeCom1" runat="server" />
                                                                            <asp:CheckBox ID="chkIsExcludeCom1" runat="server" Checked='<%# Eval("is_CommissionExclude") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="15%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 4 --%>
                                                                    <asp:ButtonField CommandName="Edit" Text="Edit" />
                                                                    <%-- <asp:TemplateField HeaderText="Serial">
                                                                        <ItemStyle HorizontalAlign="Center" Width="5%"></ItemStyle>
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemSerial" Style="text-align: center;" CssClass="textBox" runat="server"  Width="30px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                                    </asp:TemplateField>--%>
                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" valign="top">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" valign="top">
                                                <table width="100%" class="subHeader02">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblItemList" runat="server" Text="Item List" Visible="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="background-color: #bcc6dd;" align="center" colspan="2">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblItemResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <%--<asp:Button ID="btnUpdateItemSerial" runat="server" CssClass="button" OnClick="btnUpdateItemSerial_Click" Text="Update Item Serial" Visible="true" />--%>
                                                            <%-- <asp:TextBox ID="txtItemList" runat="server" TextMode="MultiLine" Height="50px" Width="150px"></asp:TextBox>
                                                            <asp:TextBox ID="txtMainSectionName" runat="server"></asp:TextBox>                                                           
                                                            <asp:Button ID="btnDisable" runat="server" CssClass="button" OnClick="btnDisable_Click" Text="Disable Item" Visible="true" />--%>

                                                            <cc1:ModalPopupExtender ID="modPriceUpdate" runat="server" PopupControlID="pnlUpdatePrice"
                                                                TargetControlID="lnkDummy3" BackgroundCssClass="modalBackground" DropShadow="false">
                                                            </cc1:ModalPopupExtender>
                                                            <cc1:ModalPopupExtender ID="modUpdateMultiplier" runat="server" PopupControlID="pnlUpdateMultiplier"
                                                                TargetControlID="lnkUpdateMultiplier" BackgroundCssClass="modalBackground" DropShadow="false">
                                                            </cc1:ModalPopupExtender>
                                                            <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>
                                                            <asp:Button ID="InkPriceUpdate" runat="server" CssClass="button" Text="Update Selected Item(s)" OnClick="InkPriceUpdate_Click"></asp:Button>
                                                            <asp:Button ID="lnkUpdateMultiplier" runat="server" CssClass="button" Text="Update Multiplier for Selected Item(s)"></asp:Button>
                                                            <asp:Button ID="btnAddItem" runat="server" CssClass="button" OnClick="btnAddItem_Click" Text="Add New Item" Visible="false" />
                                                            <asp:Button ID="btnDeleteItem" runat="server" CssClass="button" OnClick="btnDeleteItem_Click" Text="Delete Selected Item(s)" Visible="false" OnClientClick="return confirm('Are you sure you want delete item(s)');" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:GridView ID="grdItem_Price" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                OnRowCommand="grdItem_Price_RowCommand" OnRowDataBound="grdItem_Price_RowDataBound"
                                                                OnRowEditing="grdItem_Price_RowEditing" OnRowUpdating="grdItem_Price_RowUpdating"
                                                                PageSize="200" TabIndex="2" Width="98%" AllowSorting="True" OnSorting="grdItem_Price_Sorting">
                                                                <Columns>
                                                                    <%-- Cell 0 --%>
                                                                    <asp:TemplateField HeaderText="Item Name" SortExpression="section_name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblItemnName" runat="server" Text='<%# Eval("section_name") %>' />
                                                                            <asp:TextBox ID="txtItemName" runat="server" Text='<%# Eval("section_name") %>' Visible="false"
                                                                                Width="320px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 1 --%>
                                                                    <asp:TemplateField HeaderText="UOM" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblMeasureUnit" runat="server" Text='<%# Eval("measure_unit") %>' />
                                                                            <asp:TextBox ID="txtMeasureUnit" runat="server" Text='<%# Eval("measure_unit") %>'
                                                                                Visible="false" Width="100px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 2 --%>
                                                                    <asp:TemplateField HeaderText="Cost" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblCost" runat="server" Text='<%# Eval("item_cost") %>' />
                                                                            <asp:TextBox ID="txtCost" runat="server" Text='<%# Eval("item_cost") %>' Visible="false"
                                                                                Width="80px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 3 --%>
                                                                    <asp:TemplateField HeaderText="Minimum Qty" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblMinQty" runat="server" Text='<%# Eval("minimum_qty") %>' />
                                                                            <asp:TextBox ID="txtMinQty" runat="server" Text='<%# Eval("minimum_qty") %>' Visible="false"
                                                                                Width="80px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 4 --%>
                                                                    <asp:TemplateField HeaderText="Retail multiplier" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRetailMulti" runat="server" Text='<%# Eval("retail_multiplier") %>' />
                                                                            <asp:TextBox ID="txtRetailMulti" runat="server" Text='<%# Eval("retail_multiplier") %>'
                                                                                Visible="false" Width="80px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 5 --%>
                                                                    <asp:TemplateField HeaderText="Labor rate" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblLabor" runat="server" Text='<%# Eval("labor_rate") %>' />
                                                                            <asp:TextBox ID="txtLabor" runat="server" Text='<%# Eval("labor_rate") %>' Visible="false"
                                                                                Width="80px" Wrap="False"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Right" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 6 --%>
                                                                    <asp:TemplateField HeaderText="Division">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblItemPriceDivision" runat="server" Text="" Visible="true"></asp:Label>
                                                                            <asp:DropDownList ID="ddlItemPriceDivision" runat="server" Visible="false" Width="70%"></asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 7 --%>
                                                                    <asp:TemplateField HeaderText="Active" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblActive" runat="server" />
                                                                            <asp:CheckBox ID="chkIsActiveItem" runat="server" Checked='<%# Eval("is_active") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 8 --%>
                                                                    <asp:TemplateField HeaderText="Mandatory" HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAMandatory" runat="server" />
                                                                            <asp:CheckBox ID="chkIsMandatory" runat="server" Checked='<%# Eval("is_mandatory") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 9 --%>
                                                                    <asp:TemplateField HeaderText="Ex. Comm." HeaderStyle-CssClass="grdItemHeaderCss">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAExcludeCom" runat="server" />
                                                                            <asp:CheckBox ID="chkIsExcludeCom" runat="server" Checked='<%# Eval("is_CommissionExclude") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                                    </asp:TemplateField>

                                                                    <%-- Cell 10 --%>
                                                                    <asp:ButtonField CommandName="Edit" Text="Edit" />

                                                                    <%-- Cell 11 --%>
                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this);" TextAlign="Left" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkDelete" runat="server" onclick="Check_Click(this)" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--    <asp:TemplateField HeaderText="Disable" Visible="true">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkIsDisable" runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="section_id" HeaderText="section_id" ReadOnly="true"/>--%>
                                                                    <%-- <asp:TemplateField HeaderText="Serial">
                                                                        <ItemStyle HorizontalAlign="Center" Width="5%"></ItemStyle>
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtItemSerial" Style="text-align: center;" CssClass="textBox" runat="server"  Width="30px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                                    </asp:TemplateField>--%>
                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">&nbsp;<td align="left">&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                            &nbsp;
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">&nbsp;&nbsp;<asp:Button ID="btnClose" runat="server" CssClass="redButton" Text="Close"
                                                TabIndex="4" OnClick="btnClose_Click" />
                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnSectionLevel" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnItem" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnMultiplier" runat="server" Value="0" />
                                    &nbsp;<asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                    <asp:Label ID="lblSerial" runat="server" Visible="False"></asp:Label>
                                    <asp:HiddenField ID="hdnTrvSelectedValue" runat="server" Value="0" />
                                    &nbsp;<asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSubItemParentId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="pnlUpdateMultiplier" runat="server" Width="550px" Height="150px" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%">
                    <tr>
                        <td align="center">
                            <b>&nbsp;</b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>&nbsp;</b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table cellpadding="0" cellspacing="2" width="98%">

                                <tr>
                                    <td align="right">
                                        <b>Retail Multiplier: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtMultiplier" runat="server" TabIndex="1" Width="200px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtMultiplier"
                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                        </cc1:FilteredTextBoxExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Update" TabIndex="2" Width="80px"
                                            CssClass="button" OnClick="btnSubmit_Click" />
                                        &nbsp;<asp:Button ID="btnClosePopUp" runat="server" Text="Cancel" TabIndex="3" Width="80px"
                                            CssClass="button" OnClick="btnClosePopUp_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <asp:Panel ID="pnlUpdatePrice" runat="server" Style="width: auto; overflow: scroll; max-height: 700px;" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%">
                    <tr>
                        <td align="center">
                            <b>&nbsp;</b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center"></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table cellpadding="0" cellspacing="2" width="98%">

                                <tr>
                                    <td style="width: 98%">
                                        <asp:GridView ID="grdPriceUpdate" runat="server" OnRowDataBound="grdPriceUpdate_RowDataBound" AutoGenerateColumns="False" CssClass="mGrid"
                                            PageSize="200" TabIndex="2" Width="98%">
                                            <Columns>

                                                <%-- Cell 0 --%>
                                                <asp:TemplateField HeaderText="Item Name" SortExpression="section_name">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtItemName" runat="server" Text='<%# Eval("section_name") %>'
                                                            Width="320px" Wrap="False" TextMode="MultiLine"></asp:TextBox>

                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%-- Cell 1 --%>
                                                <asp:TemplateField HeaderText="UOM" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtMeasureUnit" runat="server" Text='<%# Eval("measure_unit") %>'
                                                            Width="100px" Wrap="False"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <%-- Cell 2 --%>
                                                <asp:TemplateField HeaderText="Cost" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCost" runat="server" Text='<%# Eval("item_cost") %>'
                                                            Width="80px" Wrap="False"></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" TargetControlID="txtCost"
                                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                        </cc1:FilteredTextBoxExtender>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <%-- Cell 3 --%>
                                                <asp:TemplateField HeaderText="Minimum Qty" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtMinQty" runat="server" Text='<%# Eval("minimum_qty") %>'
                                                            Width="80px" Wrap="False"></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" TargetControlID="txtMinQty"
                                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                        </cc1:FilteredTextBoxExtender>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <%-- Cell 4 --%>
                                                <asp:TemplateField HeaderText="Retail multiplier" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRetailMulti" runat="server" Text='<%# Eval("retail_multiplier") %>'
                                                            Width="80px" Wrap="False"></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" TargetControlID="txtRetailMulti"
                                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                        </cc1:FilteredTextBoxExtender>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <%-- Cell 5 --%>
                                                <asp:TemplateField HeaderText="Labor rate" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtLabor" runat="server" Text='<%# Eval("labor_rate") %>'
                                                            Width="80px" Wrap="False"></asp:TextBox>
                                                        <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" TargetControlID="txtLabor"
                                                            FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                        </cc1:FilteredTextBoxExtender>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <%-- Cell 6 --%>
                                                <asp:TemplateField HeaderText="Division">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPriceUpdateDivision" runat="server" Text="" Visible="true"></asp:Label>
                                                        <asp:DropDownList ID="ddlPriceUpdateDivision" runat="server" Visible="false"></asp:DropDownList>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                </asp:TemplateField>

                                                <%-- Cell 7 --%>
                                                <asp:TemplateField HeaderText="Active" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsActiveItem" runat="server" Checked='<%# Eval("is_active") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                </asp:TemplateField>

                                                <%-- Cell 8 --%>
                                                <asp:TemplateField HeaderText="Mandatory" HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsMandatory" runat="server" Checked='<%# Eval("is_mandatory") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                </asp:TemplateField>

                                                <%-- Cell 9 --%>
                                                <asp:TemplateField HeaderText="Ex. Comm." HeaderStyle-CssClass="grdItemHeaderCss">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsExcludeCom" runat="server" Checked='<%# Eval("is_CommissionExclude") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="9%" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>

                                <tr>
                                    <td align="center" colspan="2">
                                        <asp:Button ID="btnUpdatePrice" runat="server" Text="Update" TabIndex="2" Width="80px"
                                            CssClass="button" OnClick="btnUpdatePrice_Click" />
                                        &nbsp;<asp:Button ID="Button2" runat="server" Text="Cancel" TabIndex="3" Width="80px"
                                            CssClass="button" OnClick="btnClosePopUp_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
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

