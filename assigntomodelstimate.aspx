<%@ Page Title="Assign to a Customer" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="assigntomodelstimate.aspx.cs" Inherits="assigntomodelstimate" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
     <script language="javascript" type="text/javascript">
         function selected_LastName(sender, e) {
             document.getElementById('<%=btnSearch2.ClientID%>').click();
         }
        

    </script>
    <script language="javascript" type="text/javascript">
        function searchKeyPress(e) {
            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch2.ClientID%>').click();
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
    <table cellpadding="0" cellspacing="2" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Assign to a Customer</span></td>
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
                                    <td align="center">
                                        <table class="wrapper" width="100%">
                                             <tr>
                    <td align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <td align="left">
                                    <b>Sales Person Name:</b></td>
                                <td align="left">
                                    <asp:Label ID="lblSalesPersonName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Address:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Phone:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Email Address:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Model Estimate Name:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblModelEstimateName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Create Date:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblCreateDate" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Last Updated Date:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblLastUpdatedDate" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>

                                        </table>
                                    </td>
                                </tr>
                              
                                
                                
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <%--<img width="25" height="25" src="images/08_call_log.png" alt="Search and Select Customer to Assign" title="Search and Select Customer to Assign" style="padding: 0px; vertical-align: middle;" />--%>
                                            <span class="cssTitleHeader" style="padding: 0px; vertical-align: middle;">Search and Select the Customer to Assign this Template                                                      
                                            </span>
                                        </span>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td>
                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">First Name</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                                    <asp:ListItem Value="4">Address</asp:ListItem>
                                                    <asp:ListItem Value="3">Email</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                            <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                            </cc1:AutoCompleteExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnSearch2" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="left" colspan="2">
                                    <asp:GridView ID="grdLeadList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" DataKeyNames="customer_id" OnPageIndexChanging="grdLeadList_PageIndexChanging" OnRowDataBound="grdLeadList_RowDataBound" Width="1080px" PageSize="5" OnRowCommand="grdLeadList_RowCommand">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <%-- <asp:BoundField DataField="customer_name" HeaderText="Customer Name">
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>--%>
                                            <asp:ButtonField Text="Select" CommandName="Select">
                                                <HeaderStyle Width="4%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                            <asp:BoundField DataField="last_name1" HeaderText="Last Name">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="first_name1" HeaderText="First Name">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Address, eMail, Phone">
                                                <ItemTemplate>
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td align="left" width="50%">
                                                                <asp:Label ID="lAddress" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="left" width="50%">
                                                                <asp:Label ID="lEmail" runat="server"></asp:Label>
                                                                <br />
                                                                <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="sales_person_id" HeaderText="Sales Person">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField>
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <b>Customer Name:</b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblSelectedCustName" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                        <tr id="tblNoExistingEstimates" runat="server" visible="false">
                                            <td align="right"><b>Estimate Name:</b></td>
                                            <td align="left">
                                                <asp:Label ID="lblNewEstimateName" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                        <tr id="tblExistingEstimates" runat="server" visible="false">
                                            <td align="left"><b>Choose an Existing Estimate:</b></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlChoseEstimate" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlChoseEstimate_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td></td>

                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="left" colspan="3">
                                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label></td>

                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="center" colspan="4"></td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <asp:HiddenField ID="hdnModelEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                </td>
                                <td align="left">
                                    <asp:HiddenField ID="HiddenField1" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustName" runat="server" Value="0" />
                                </td>
                                <td align="right">
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                </td>
                                <td align="left">
                                    <asp:HiddenField ID="hdnCustomerEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnHdnPrevIndex" runat="server" Value="-1" />
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
                                        <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True" OnRowDataBound="grdGrouping_RowDataBound"
                                            AutoGenerateColumns="False" CssClass="mGrid" >
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />
                                                       
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound" 
                                                             Width="100%" CssClass="mGrid" >
                                                            <Columns>
                                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
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
                                                              
                                                                 <asp:TemplateField HeaderText="Item Name">
                                                                   
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("item_name").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    
                                                                    <HeaderStyle Width="25%" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True"  NullDisplayText=" ">
                                                                    <HeaderStyle Width="6%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                    <HeaderStyle Width="5%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Code">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblquantity" runat="server" Text='<%# Eval("quantity") %>' />
                                                                        <asp:TextBox ID="txtquantity" runat="server" Style="text-align: center;" Visible="false" Width="40px" Text='<%# Eval("quantity") %>'
                                                                            AutoPostBack="True" OnTextChanged="NonDirect_calculation" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="5%" />
                                                                    <ItemStyle HorizontalAlign="Center" />
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
                                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" Visible="False" ReadOnly="True">
                                                                    <HeaderStyle Width="1%" />
                                                                </asp:BoundField>
                                                                <asp:TemplateField HeaderText="Short Notes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblshort_notes" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                        <asp:TextBox ID="txtshort_notes" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                           TextMode="MultiLine" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="20%" />
                                                                </asp:TemplateField>
                                                               

                                                                 <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="chkIsSelectedAll" runat="server" onclick="checkAll(this);" Text="All" TextAlign="Left"  Checked="true" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkIsSelected" runat="server" onclick="Check_Click(this)" Checked="true" />
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
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
                                                        <asp:Label ID="lblDirectPricingHeader" runat="server" Text="The following items are Direct / Outsourced"
                                                            Visible="False"></asp:Label></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" colspan="2" valign="top" width="15%">
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                        OnRowDataBound="grdGroupingDirect_RowDataBound" ShowFooter="True" CaptionAlign="Top">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                                 
                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                        DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound" OnRowDeleting="grdSelectedItem2_RowDeleting"
                                                                        OnRowEditing="grdSelectedItem2_RowEditing" ShowFooter="True" Width="100%" OnRowUpdating="grdSelectedItem2_RowUpdating">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" ReadOnly="True">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" ReadOnly="True">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <HeaderStyle Width="11%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name" ReadOnly="True">
                                                                                <HeaderStyle Width="25%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM" ReadOnly="True"  NullDisplayText=" ">
                                                                                <HeaderStyle Width="6%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblquantity1" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                    <asp:TextBox ID="txtquantity1" runat="server" Style="text-align: center;" Visible="false" Width="40px" Text='<%# Eval("quantity") %>'
                                                                                        AutoPostBack="True" OnTextChanged="Direct_calculation" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="5%" />
                                                                                <ItemStyle HorizontalAlign="Center" />
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
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblshort_notes1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                    <asp:TextBox ID="txtshort_notes1" runat="server" Visible="false" Text='<%# Eval("short_notes") %>'
                                                                                       TextMode="MultiLine" />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="20%" />
                                                                            </asp:TemplateField>
                                                                            <asp:ButtonField CommandName="Edit" Text="Edit">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:ButtonField>
                                                                           
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

                
                                            <tr style="display:none">
                                                <td align="right" valign="top" width="15%">
                                                    <b>Comments: </b>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtComments" runat="server" Height="44px" TabIndex="1" TextMode="MultiLine"
                                                        Width="85%"></asp:TextBox>
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
                        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Button ID="btnSave" runat="server" CssClass="button" OnClick="btnSave_Click"
                                            Text="Update" Width="80px" Visible="false" />
                                        <asp:Button ID="btnAssignToCustomer" runat="server" CssClass="button" Text="Assign"
                                            EnableViewState="False" OnClick="btnAssignToCustomer_Click" />
                                         <asp:Button ID="btnGotoModelEstimate" runat="server"
                            Text="Go to Model Estimate" CssClass="button"
                            OnClick="btnGotoModelEstimate_Click" />
                        <asp:Button ID="btnGotoCustomer" runat="server" Text="Go to Customer Estimate"
                            CssClass="button" OnClick="btnGotoCustomer_Click" Visible="False" />
                        
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
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                </td>
                                                <td align="right">
                                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                                </td>
                                                <td align="left">
                                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnOtherId" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">&nbsp;
                                                </td>
                                                <td align="left">&nbsp;
                                                </td>

                                                <td align="left">
                                                    <asp:HiddenField ID="hdnItemCnt" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSectionLevel" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSortDesc" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnModEstimatePaymentId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnProjectTotal" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnLocationId" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnType" runat="server" Value="0" />
                                                </td>
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


