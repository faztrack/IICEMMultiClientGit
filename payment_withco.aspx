<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="payment_withco.aspx.cs" Inherits="payment_withco" Title="Payment Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/calendar-blue.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="js/jquery.dynDateTime.min.js" type="text/javascript"></script>
    <script src="js/calendar-en.min.js" type="text/javascript"></script>

    <script type="text/javascript">
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
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Payment Information</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">

                        <table align="center" cellpadding="0" cellspacing="2" width="98%">
                            <tr>
                                <td align="right">
                                    <b>Customer Name: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right" valign="top">
                                    <b>Address: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Phone: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right" valign="top">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Email: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Estimate Name: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEstimateName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center">
                        <table align="center" cellpadding="0" cellspacing="2" width="98%">
                            <tr>
                                <td align="center">
                                    <table id="Table1" border="1" bordercolor="silver">
                                        <tr>
                                            <td align="center">
                                                <h2>Payment Schedule &amp; Terms</h2>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="middle">
                                                <!--StartFragment-->
                                                <table id="Table2" border="1" bordercolor="silver">
                                                    <tr>
                                                        <td align="center">
                                                            <b>Schedule Date</b>
                                                        </td>
                                                        <td align="right">&nbsp;</td>
                                                        <td>&nbsp;</td>
                                                        <td align="right">&nbsp;</td>
                                                        <td>&nbsp;</td>
                                                        <td align="center">
                                                            <b>Schedule Date</b>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblDepositDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblDepositValue" runat="server" Text="Deposit"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnDeposit" runat="server" TabIndex="-1" Width="73"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblMeasureValue" runat="server" Text="At Final Measure"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnMeasure" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblMeasureDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblCountertopDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblCountertopValue" runat="server" Text="At Countertop Template"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnCountertop" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblDeliveryValue" runat="server" Text="At Delivery of Cabinets"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnDelivery" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblDeliveryDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblStartJobDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblStartJobValue" runat="server" Text=" Start of Job"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnJob" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Label ID="lblSubstantialValue" runat="server" Text=" At Substantial Completion"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnSubstantial" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblSubstantialDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblBalanceDueDate" runat="server" Text=""></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblBalanceDueValue" runat="server" Text=" Balance Due at Completion"></asp:Label>:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtnBalance" runat="server" TabIndex="-1" Width="72px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td>Others:
                                                            <asp:TextBox ID="txtOthers" runat="server" TabIndex="-1" Width="101"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtnOthers" runat="server" TabIndex="-1" Width="73px"
                                                                ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                        <td align="center">
                                                            <asp:Label ID="lblOtherDate" runat="server" Text=""></asp:Label>
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
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                        <td align="center" colspan="2">
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                        </td>
                    </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table width="100%">

        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0">

                    <tr>
                        <td align="center" colspan="2">
                            <asp:GridView ID="grdPyement" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdPyement_RowDataBound"
                                OnRowEditing="grdPyement_RowEditing"
                                OnRowUpdating="grdPyement_RowUpdating"
                                OnRowCommand="grdPyement_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("pay_date","{0:d}")%>' />
                                            <div id="dvCalender" runat="server" visible="false">
                                                <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("pay_date","{0:d}") %>'
                                                    class="Calender" Width="60px"></asp:TextBox>
                                                <img src="images/calendar.gif" />
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                        <ItemStyle HorizontalAlign="Center" Width="115px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Payment Term">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlPayTerm" runat="server" Enabled="false" DataValueField="pay_term_id" DataTextField="term_name" DataSource="<%#dtTerms %>" SelectedValue='<%# Eval("pay_term_id") %>'>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Payment Type">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlType" Enabled="false" runat="server" SelectedValue='<%# Eval("pay_type_id") %>'>
                                                <asp:ListItem Value="1">Cash</asp:ListItem>
                                                <asp:ListItem Value="2">Check</asp:ListItem>
                                                <asp:ListItem Value="3">Credit Card</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reference">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReference" runat="server" Text='<%# Eval("reference") %>' />
                                            <asp:TextBox ID="txtReference" runat="server" Visible="false"
                                                Text='<%# Eval("reference") %>' Width="150px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("pay_amount","{0:c}") %>' />
                                            <asp:TextBox ID="txtAmount" runat="server" Visible="false"
                                                Text='<%# Eval("pay_amount","{0:c}") %>' Width="60px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>
                                </Columns>
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;</td>
                        <td align="right">
                            <asp:Button ID="btnAddnewRow" runat="server" CssClass="button"
                                OnClick="btnAddnewRow_Click" Text="Add New Payment" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">&nbsp;</td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">
                            <asp:GridView ID="grdChangeOrders" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid" DataKeyNames="chage_order_id"
                                OnRowDataBound="grdChangeOrders_RowDataBound" PageSize="20" Width="100%">
                                <PagerSettings Position="TopAndBottom" />
                                <Columns>
                                    <asp:BoundField DataField="changeorder_name" HeaderText="C/O Title" />
                                    <asp:BoundField DataField="change_order_status_id" HeaderText="Status" />
                                    <asp:BoundField DataField="change_order_type_id" HeaderText="Type" />
                                    <asp:BoundField DataField="changeorder_date" DataFormatString="{0:d}"
                                        HeaderText="Change Order Date" />
                                    <asp:BoundField DataField="last_updated_date" DataFormatString="{0:d}"
                                        HeaderText="Updated Date" />
                                    <asp:BoundField HeaderText="Updated By" />

                                    <asp:BoundField HeaderText="Payment Term" />
                                    <asp:BoundField HeaderText="Amount" />

                                </Columns>
                                <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">Payment for C/O</td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">
                            <asp:GridView ID="grdCOPayment" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdCOPayment_RowDataBound"
                                OnRowEditing="grdCOPayment_RowEditing"
                                OnRowUpdating="grdCOPayment_RowUpdating"
                                OnRowCommand="grdCOPayment_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCoDate" runat="server" Text='<%# Eval("co_pay_date","{0:d}")%>' />
                                            <div id="dvCoCalender" runat="server" visible="false">
                                                <asp:TextBox ID="txtCoDate" runat="server" Text='<%# Eval("co_pay_date","{0:d}") %>'
                                                    ReadOnly="true" class="Calender" Width="60px"></asp:TextBox>
                                                <img src="images/calendar.gif" />
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                        <ItemStyle HorizontalAlign="Center" Width="115px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CO Title">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCO" runat="server" Enabled="false" DataValueField="change_order_id" DataTextField="changeorder_name" DataSource="<%#dtCO_NAME %>" SelectedValue='<%# Eval("change_order_id") %>'>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Payment Term">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCOTerm" Enabled="false" runat="server"
                                                SelectedValue='<%# Eval("co_pay_term_id") %>'>
                                                <asp:ListItem Value="2">Balance due at Completion</asp:ListItem>
                                                <asp:ListItem Value="1">Payment due upon completion of work described on this 
                        Change Order</asp:ListItem>
                                                <asp:ListItem Value="3">Payment due upon signing of this Change Order</asp:ListItem>
                                                <asp:ListItem Value="6">No payment due, for internal use only</asp:ListItem>
                                                <asp:ListItem Value="7">Credit to be applied to final payment</asp:ListItem>
                                                <asp:ListItem Value="5">Other</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Payment Type">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCOType" Enabled="false" runat="server"
                                                SelectedValue='<%# Eval("co_pay_type_id") %>'>
                                                <asp:ListItem Value="1">Cash</asp:ListItem>
                                                <asp:ListItem Value="2">Check</asp:ListItem>
                                                <asp:ListItem Value="3">Credit Card</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reference">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCOReference" runat="server" Text='<%# Eval("co_reference") %>' />
                                            <asp:TextBox ID="txtCOReference" runat="server" Visible="false"
                                                Text='<%# Eval("co_reference") %>' Width="70px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCOAmount" runat="server"
                                                Text='<%# Eval("co_pay_amount","{0:c}") %>' />
                                            <asp:TextBox ID="txtCOAmount" runat="server" Visible="false"
                                                Text='<%# Eval("co_pay_amount","{0:c}") %>' Width="60px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>
                                </Columns>
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">&nbsp;</td>
                        <td align="right">
                            <asp:Button ID="btnAddnewCOPay" runat="server" CssClass="button"
                                OnClick="btnAddnewCOPay_Click" Text="Add New C/O Payment" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">&nbsp;</td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">
                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">&nbsp;</td>
                    </tr>

                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnBackToPaymentInfo" runat="server" CausesValidation="False"
                                CssClass="button" OnClick="btnBackToPaymentInfo_Click"
                                Text="Back to Payment Schedule &amp; Terms" TabIndex="17" />
                            <asp:Button ID="btnCustomerDetails" runat="server" Text="Customer Details"
                                TabIndex="18" CssClass="button" OnClick="btnCustomerDetails_Click" />
                            <asp:Button ID="btnCustomerList" runat="server" Text="Customer List"
                                TabIndex="19" CssClass="button" OnClick="btnCustomerList_Click" />
                            <asp:Button ID="btnSubmit" runat="server" CssClass="button"
                                OnClick="btnSubmit_Click" Text="Submit" Width="80px" TabIndex="20" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2"></td>
                    </tr>
                    
                    <tr>
                        <td align="center"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
