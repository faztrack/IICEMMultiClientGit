<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Vendor_cost_details.aspx.cs" Inherits="Vendor_cost_details" Title="Vendor Cost Details" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/calendar-blue.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="js/jquery.dynDateTime.min.js" type="text/javascript"></script>
    <script src="js/calendar-en.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        function confirmDelete() {
            return confirm("Are you sure that you want to delete this Item?");
        }

        $(document).ready(function () {

            $(".Calender").dynDateTime({
                showsTime: false,
                ifFormat: "%m/%d/%y",
                daFormat: "%l;%M %p, %e %m,  %Y",
                align: "BR",
                electric: false,
                singleClick: true,
                displayArea: ".siblings('.dtcDisplayArea')",
                button: ".next()"
            });
        });
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">

        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Vendor Cost Details</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4" cellspacing="8" width="100%">
                            <tr>
                                <td align="right" width="30%">
                                    <b>Customer Name:</b> </td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <b>Projects:</b>&nbsp;
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlEstimate" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEstimate_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:Label ID="lblCurrentEstimate" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="30%"><b>Sales Person:</b></td>
                                <td align="left">
                                    <asp:Label ID="lblSalesPerson" runat="server"></asp:Label></td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Contract Amount:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblProjectTotal" runat="server" Width="84px" Text="0"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                                <td align="right">
                                    <b>Profit/Loss:</b>&nbsp;</td>
                                <td align="left">
                                    <asp:Label ID="lblProfit" runat="server" Width="84px" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="30%"><b>Commission Eligible Contract Amount:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblComEligibleContract" runat="server" CssClass="cssLblAmount" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>C/O&nbsp; Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalCOAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%"><b>Commission Eligible CO Amount:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblComEligibleCO" runat="server" CssClass="cssLblAmount" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Total Amount (Contract + C/O):</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>&nbsp;Total Received Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalRecive" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Balance Due:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalBalanceAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Contract Commission:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:GridView ID="grdCom" runat="server" AutoGenerateColumns="False" Style="padding: 0px; margin: 0px 0px 0px 33px;"
                                        CssClass="bGrid" OnRowDataBound="grdCom_RowDataBound"
                                        OnRowEditing="grdCom_RowEditing" OnRowUpdating="grdCom_RowUpdating"
                                        ShowHeader="False" TabIndex="2" Width="33%" BorderStyle="None">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' />
                                                    <asp:TextBox ID="txtComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' Style="text-align: right;" Visible="false" Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit" />
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                            <tr>
                                <td align="right" width="30%">
                                    <b>C/O Commission:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:GridView ID="grdCom_CO" runat="server" AutoGenerateColumns="False" Style="padding: 0px; margin: 0px 0px 0px 33px;"
                                        BorderStyle="None" CssClass="bGrid" OnRowDataBound="grdCom_CO_RowDataBound"
                                        OnRowEditing="grdCom_CO_RowEditing" OnRowUpdating="grdCom_CO_RowUpdating"
                                        ShowHeader="False" TabIndex="2" Width="33%">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' />
                                                    <asp:TextBox ID="txtComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' Style="text-align: center;" Visible="false" Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit" />
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnGoToPayment" runat="server" CssClass="greenbutton" OnClientClick="ShowProgress();"
                                        OnClick="btnGoToPayment_Click" Text="Go to Payment" />
                                </td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Total Commission:</b>&nbsp;</td>
                                <td align="left">
                                    <asp:Label ID="lblTotalCom" runat="server" Width="84px" CssClass="cssLblAmount">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                            <tr>
                                <td align="right" width="30%">
                                    <b>Labor Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblLabor" runat="server" Width="84px" CssClass="cssLblAmount">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Material Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblMaterial" runat="server" Width="84px" CssClass="cssLblAmount">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Material & Labor Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblMaterialLabor" runat="server" Width="84px" CssClass="cssLblAmount">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Total Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalCost" runat="server" Width="84px"
                                        CssClass="cssLblAmount">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                        </table>
                        <asp:HiddenField ID="hdnAllowance" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnProjectSubTotal" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnTotalCOWithoutTax" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnProjectSubTotalExCom" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnAVendorId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnTotalCOExCom" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnASectionId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                        <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>

                    </td>
                </tr>
            </table>


        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="ddlEstimate" />
        </Triggers>
    </asp:UpdatePanel>


    <table width="100%">
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblResult2" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="btnViewReport" Style="margin-left: 10px;" runat="server" CssClass="cssCSV nostyleCalImg" ImageUrl="~/images/export_csv_btn.png" OnClick="btnViewReport_Click" /></td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:GridView ID="grdVendorCost" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdVendorCost_RowDataBound"
                                OnRowEditing="grdVendorCost_RowEditing"
                                OnRowUpdating="grdVendorCost_RowUpdating" OnRowDeleting="grdVendorCost_RowDeleting"
                                OnRowCommand="grdVendorCost_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("cost_date","{0:d}")%>' />
                                            <table id="dvCalender" runat="server" visible="false">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("cost_date","{0:d}") %>'
                                                            class="Calender" Width="60px"></asp:TextBox></td>
                                                    <td>
                                                        <img src="images/calendar.gif" /></td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" CssClass="vcdDate" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vendor">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlVendor" CssClass="vcdDD" runat="server" Enabled="false" DataValueField="vendor_id" DataTextField="vendor_name" DataSource="<%#dtVendor %>">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="vcdVendor" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Section">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlTrade" CssClass="secDD" runat="server" Enabled="false" DataValueField="section_id" DataTextField="section_name" DataSource="<%#dtSection %>" SelectedValue='<%#Eval("section_id") %>'>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="vcdSec" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("cost_description") %>' />
                                            <asp:TextBox ID="txtDescription" runat="server" Visible="false"
                                                Text='<%# Eval("cost_description") %>' TextMode="MultiLine" Width="98%" Height="40px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Left" CssClass="vcdDes" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Category">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCategory" Enabled="false" runat="server" SelectedValue='<%# Eval("category_id") %>' OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Value="1">Material</asp:ListItem>
                                                <asp:ListItem Value="2">Labor</asp:ListItem>
                                                <asp:ListItem Value="3">Material & Labor</asp:ListItem>
                                                <asp:ListItem Value="4">Allowance Item</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="vcdCate" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("cost_amount","{0:c}") %>' />
                                            <asp:TextBox ID="txtAmount" runat="server" Visible="false" Style="text-align: right;"
                                                Text='<%# Eval("cost_amount","{0:c}") %>' Width="50px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Right" CssClass="vcdAmou" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tools">
                                        <ItemTemplate>
                                            <table style="padding: 0px; margin: 0px; border: none;">
                                                <tr style="padding: 0px; margin: 0px; border: none;">
                                                    <td style="padding: 0px; margin: 0px; border: none;">

                                                        <asp:GridView ID="grdUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                            CssClass="bGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                            OnRowDataBound="grdUploadedFileList_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:HyperLink ID="hypUploadedFile" runat="server"></asp:HyperLink>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="btnDeleteUploadedFile" Text="Delete" runat="server" OnClick="DeleteUploadedFile" OnClientClick="ShowProgress();" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Right" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>



                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table style="padding: 0px; margin: 0px; border: none;">
                                                            <tr style="padding: 0px; margin: 0px; border: none;">
                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                    <asp:FileUpload ID="file_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                </td>
                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                    <asp:ImageButton ID="imgbtnUpload" Height="24" Width="24" CssClass="nostyleCalImg" ToolTip="Upload New Files" runat="server" ImageUrl="~/images/upload_imag.png" OnClientClick="ShowProgress();" OnClick="btnUpload_Click" />

                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%" />
                                    </asp:TemplateField>
                                    <asp:ButtonField CommandName="Edit" ControlStyle-CssClass="vcdEdit" Text="Edit"></asp:ButtonField>
                                    <%--<asp:ButtonField CommandName="Delete" Text="Delete" />
                                        SelectedValue='<%# Eval("vendor_id") %>'--%>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" OnClientClick="return confirmDelete();" CommandName="Delete" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle CssClass="vcdDel" />
                                    </asp:TemplateField>

                                </Columns>
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click" Text="Add New Row" OnClientClick="ShowProgress();" />
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td align="center"></td>
        </tr>
        <tr>
            <td align="center" class="button" style="height: 15px">
                <asp:Button ID="btnAcceptPayment" runat="server" CssClass="button"
                    Text="Accept Payment" OnClick="btnAcceptPayment_Click"
                    Visible="False" />

            </td>
        </tr>

    </table>



    <div id="myModal" class="modal">

        <!-- Modal content -->
        <div class="modal-content">
            <span class="close">&times;</span>
            <table id="tdAllowance" runat="server" cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="2" width="98%">
                            <tr>
                                <td align="center">
                                    <b>Allowance Items</b>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:GridView ID="grdAllowanceItem" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                        DataKeyNames="item_id" PageSize="200"
                                        TabIndex="2" Width="99%">
                                        <Columns>
                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" />
                                            <asp:BoundField DataField="location_name" HeaderText="Location">
                                                <ItemStyle HorizontalAlign="Left" Width="11%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="section_name" HeaderText="Section">
                                                <ItemStyle HorizontalAlign="Left" Width="11%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="item_name" HeaderText="Item">
                                                <ItemStyle HorizontalAlign="Left" Width="38%" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="short_notes" HeaderText="Short Note">
                                                <ItemStyle HorizontalAlign="Left" Width="20%" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="UOM">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMeasure" runat="server" Text='<%# Eval("measure_unit") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("total_retail_price","{0:c}") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Right" Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtAmount" runat="server" Style="text-align: right;"
                                                        Text='<%# Eval("actual_price","{0:c}") %>' Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>

                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">&nbsp;<asp:Button ID="btnAddAllowanceItem" runat="server" Text="Confirm" Width="80px" TabIndex="2"
                                    CssClass="button" OnClick="btnAddAllowanceItem_Click" />
                                    &nbsp;<asp:Button ID="btnCloseAllowanceItem" runat="server" Text="Cancel" TabIndex="3" Width="80px" OnClientClick="HidePopUp();"
                                        CssClass="button" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Upon clicking on CONFIRM, the above item(s) will be added to the vendor cost and the allowance list.</b>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>

            </table>
            <table id="tdNOAllowance" runat="server" cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="2" width="50%">
                            <tr>
                                <td align="center">
                                    <b>"Allowance not available for this section"</b>
                                </td>
                                <tr>
                                    <td align="center">&nbsp;<asp:Button ID="btnNoItemClose" runat="server" Text="Close" Width="80px" TabIndex="2"
                                        CssClass="button" OnClick="btnNoItemClose_Click" />
                                    </td>
                                </tr>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>

    </div>


    <script>
        // Get the modal
        var modal = document.getElementById('myModal');

        // Get the <span> element that closes the modal
        var span = document.getElementsByClassName("close")[0];

        function ShowPopUp() {
            document.getElementById('myModal').style.display = "block";
        }

        function HidePopUp() {

            document.getElementById('myModal').style.display = "none";
        }

        // When the user clicks on <span> (x), close the modal
        span.onclick = function () {
            modal.style.display = "none";
        }

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        }
    </script>
    <div id="LoadingProgress" style="display: none">
        <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>
</asp:Content>
