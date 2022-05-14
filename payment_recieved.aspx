<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="payment_recieved.aspx.cs"
    Inherits="payment_recieved" Title="Payment Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">

        function DisplayEmailWindow() {

            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&eid=' + document.getElementById('<%= hdnEstimateId.ClientID%>').value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

        }
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>

    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="0" width="100%" align="center">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" width="100%" align="center">
                                <tr>
                                    <td align="center" class="cssHeader">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="left"><span class="titleNu">Payment Information</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                                                <td align="right" style="padding-right: 30px; float: right;">
                                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table class="wrapper" width="100%">
                                            <tr>
                                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
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
                                                            <td style="width: 112px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                            <td style="width: auto 100%;">
                                                                <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Phone: </b></td>
                                                            <td style="width: auto 100%;">
                                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Email: </b></td>
                                                            <td style="width: auto 100%;">
                                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Estimate Name: </b></td>
                                                            <td style="width: auto 100%;">
                                                                <asp:Label ID="lblEstimateName" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table style="width: 420px;">
                                                        <tr>
                                                            <td style="width: 100px;" align="left" valign="top"><b>Address: </b></td>
                                                            <td style="width: auto 100%;" align="left" valign="top">
                                                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Sales Person:</b>&nbsp;</td>
                                                            <td align="left" style="width: auto;" valign="top">
                                                                <asp:Label ID="lblSalesPerson" runat="server"></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" valign="top"><b>Job Number:</b>&nbsp;</td>
                                                            <td align="left" style="width: auto;" valign="top">
                                                                <asp:Label ID="lblJobNumber" runat="server"></asp:Label></td>
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
                        <td align="center">
                            <table align="center" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">
                                        <table class="wrapper" width="100%" id="Table1">
                                            <tr>
                                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <img src="images/icon-pst.png" /></td>
                                                            <td align="left">
                                                                <h2>Payment Schedule &amp; Terms</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td align="left">
                                                    <!--StartFragment-->
                                                    <table class="wrappermini" id="Table2" border="0">
                                                        <tr>
                                                            <td align="center" colspan="2">
                                                                <b>Schedule Date</b>
                                                            </td>
                                                            <td align="right">&nbsp;
                                                            </td>
                                                            <td>&nbsp;
                                                            </td>
                                                            <td align="right">&nbsp;
                                                            </td>
                                                            <td>&nbsp;
                                                            </td>
                                                            <td align="center" colspan="2">
                                                                <b>Schedule Date</b>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtDepositDate" runat="server" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="DepositDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDepositDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtDepositDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgDepositDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblDepositValue" runat="server" Text="Deposit"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnDeposit" runat="server" TabIndex="-1" Width="73" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblMeasureValue" runat="server" Text="At Final Measure"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnMeasure" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtMeasureDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="MeasureDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgMeasureDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtMeasureDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgMeasureDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtCountertopDate" runat="server" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="CountertopDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgCountertopDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtCountertopDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgCountertopDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblCountertopValue" runat="server" Text="At Countertop Template"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnCountertop" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblDeliveryValue" runat="server" Text="At Delivery of Cabinets"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnDelivery" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtDeliveryDate" runat="server" Width="75px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="DeliveryDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDeliveryDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtDeliveryDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgDeliveryDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtStartOfJobDate" runat="server" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="StartOfJobDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartOfJobDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtStartOfJobDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgStartOfJobDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblStartJobValue" runat="server" Text=" Start of Job"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnJob" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblSubstantialValue" runat="server" Text=" At Substantial Completion"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnSubstantial" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtSubstantialDate" runat="server" Width="75px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="SubstantialDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgSubstantialDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtSubstantialDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgSubstantialDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtStartofFlooringDate" runat="server" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="StartofFlooringDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartofFlooringDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtStartofFlooringDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgStartofFlooringDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblStartofFlooringValue" runat="server" Text=" At Start of Flooring"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnFlooring" runat="server" TabIndex="-1" Width="72px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>

                                                            <td>Others:
                                                            <asp:TextBox ID="txtOthers" runat="server" TabIndex="-1" Width="101" ReadOnly="True"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnOthers" runat="server" TabIndex="-1" Width="73px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtOtherDate" runat="server" Width="75px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="OtherDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgOtherDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtOtherDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgOtherDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>




                                                        </tr>
                                                        <tr>

                                                            <td align="center">
                                                                <asp:TextBox ID="txtStartofDrywallDate" runat="server" Width="75px"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="StartofDrywallDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartofDrywallDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtStartofDrywallDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgStartofDrywallDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblStartofDrywallValue" runat="server" Text=" At Start of Drywall"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnDrywall" runat="server" TabIndex="-1" Width="72px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>

                                                            <td>
                                                                <asp:Label ID="lblBalanceDueValue" runat="server" Text=" Balance Due at Completion"></asp:Label>:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtnBalance" runat="server" TabIndex="-1" Width="72px" ReadOnly="True"
                                                                    CssClass="cssTxtAmount"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <asp:TextBox ID="txtDueCompletionDate" runat="server" Width="75px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="DueCompletionDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDueCompletionDate"
                                                                    PopupPosition="BottomLeft" TargetControlID="txtDueCompletionDate">
                                                                </cc1:CalendarExtender>
                                                            </td>
                                                            <td align="center">
                                                                <asp:ImageButton ID="imgDueCompletionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                            </td>


                                                        </tr>
                                                        <tr>
                                                            <td align="center" colspan="8">
                                                                <asp:Label ID="lblResultMess" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" colspan="8">
                                                                <asp:Button ID="btnSavePayDate" runat="server" CssClass="button" OnClick="btnSavePayDate_Click"
                                                                    Text="Save" Width="80px" />
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
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="center">
                                        <table class="wrapper" width="100%">
                                            <tr>
                                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <img src="images/icon-ecol.png" /></td>
                                                            <td align="left">
                                                                <h2>Executed Change Order List</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="padding: 0 5px;" align="left" colspan="2">
                                                    <asp:GridView ID="grdChangeOrders" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                        DataKeyNames="chage_order_id" OnRowDataBound="grdChangeOrders_RowDataBound" PageSize="20"
                                                        Width="100%">
                                                        <PagerSettings Position="TopAndBottom" />
                                                        <Columns>
                                                            <asp:BoundField DataField="changeorder_name" HeaderText="C/O Title" />
                                                            <asp:BoundField DataField="change_order_status_id" HeaderText="Status" />
                                                            <asp:BoundField DataField="change_order_type_id" HeaderText="Type" />
                                                            <asp:BoundField DataField="changeorder_date" DataFormatString="{0:d}" HeaderText="Change Order Date" />
                                                            <asp:BoundField DataField="last_updated_date" DataFormatString="{0:d}" HeaderText="Updated Date" />
                                                            <asp:BoundField HeaderText="Updated By" />
                                                            <asp:BoundField HeaderText="Payment Term" />
                                                            <asp:BoundField HeaderText="Amount" />
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table class="wrapper" width="100%">
                                            <tr>
                                                <%--<td align="center" colspan="2">
                            <b __designer:mapid="2af">Received Payment</b>
                        </td>--%>
                                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <img src="images/icon-rp.png" /></td>
                                                            <td align="left">
                                                                <h2>Received Payment</h2>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="padding: 0 5px;" align="left">
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="left" colspan="2">
                                                                <asp:GridView ID="grdPyement" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                                    PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdPyement_RowDataBound"
                                                                    OnRowEditing="grdPyement_RowEditing"
                                                                    OnRowUpdating="grdPyement_RowUpdating" OnRowCommand="grdPyement_RowCommand"
                                                                    OnRowDeleting="grdPyement_RowDeleting">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Date">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("pay_date","{0:d}")%>' />
                                                                                <div id="dvCalender" runat="server" visible="false">
                                                                                    <table style="padding: 0px; margin: 0px;">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("pay_date","{0:d}") %>' Width="65px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:ImageButton CssClass="nostyleCalImg" ID="imgDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                                                                <cc1:CalendarExtender ID="extDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgDate" TargetControlID="txtDate">
                                                                                                </cc1:CalendarExtender>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                    <%--<table cellpadding="0" cellspacing="0">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("pay_date","{0:d}") %>' class="Calender"
                                                                                                Width="60px"></asp:TextBox></td>
                                                                                        <td>
                                                                                            <img src="images/calendar.gif" /></td>
                                                                                    </tr>
                                                                                </table>--%>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                                                            <ItemStyle HorizontalAlign="Center" Width="115px" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Payment Term">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblPayTerms" runat="server" />
                                                                                <asp:CheckBoxList ID="chkPayterm" runat="server" RepeatColumns="2" DataValueField="pay_term_id"
                                                                                    DataTextField="term_name" DataSource="<%#dtTerms %>">
                                                                                </asp:CheckBoxList>
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
                                                                                <asp:TextBox ID="txtReference" runat="server" Visible="false" Text='<%# Eval("reference") %>'
                                                                                    Width="120px"></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                            <ItemStyle HorizontalAlign="Left" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Amount">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("pay_amount","{0:c}") %>' />
                                                                                <asp:TextBox ID="txtAmount" runat="server" Visible="false" Text='<%# Eval("pay_amount","{0:c}") %>'
                                                                                    Width="60px"></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <HeaderStyle HorizontalAlign="Center" />
                                                                            <ItemStyle HorizontalAlign="Right" />
                                                                        </asp:TemplateField>
                                                                        <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>
                                                                        <asp:ButtonField CommandName="Delete" Text="Delete"></asp:ButtonField>
                                                                    </Columns>
                                                                    <AlternatingRowStyle CssClass="alt" />
                                                                </asp:GridView>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click"
                                                                    Text="Add New Payment" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left">
                                                                            <b>Contract Amount:</b>
                                                                        </td>
                                                                        <td align="left">
                                                                            <asp:Label CssClass="cssLblAmount" ID="lblProjectTotal" runat="server" Width="84px"
                                                                                Text="0"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left">
                                                                            <b>C/O Amount:</b>
                                                                        </td>
                                                                        <td align="left">
                                                                            <asp:Label CssClass="cssLblAmount" ID="lblTotalCOAmount" runat="server" Text="0"
                                                                                Width="84px"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left">
                                                                            <b>Total Amount (Contract+C/O):</b>
                                                                        </td>
                                                                        <td align="left">
                                                                            <asp:Label CssClass="cssLblAmount" ID="lblTotalAmount" runat="server" Text="0" Width="84px"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left">
                                                                            <b>Total Received Amount:</b>
                                                                        </td>
                                                                        <td align="left">
                                                                            <asp:Label CssClass="cssLblAmount" ID="lblTotalRecievedAmount" runat="server" Text="0"
                                                                                Width="84px"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left">
                                                                            <b>Balance Due:</b>
                                                                        </td>
                                                                        <td align="left">
                                                                            <asp:Label CssClass="cssLblAmount" ID="lblTotalBalanceAmount" runat="server" Text="0"
                                                                                Width="84px"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" colspan="2">
                                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Button ID="btnBackToPaymentInfo" runat="server" CausesValidation="False" CssClass="button"
                                                                                OnClick="btnBackToPaymentInfo_Click" Text="Back to Payment Schedule &amp; Terms"
                                                                                TabIndex="17" />
                                                                            <asp:Button ID="btnCustomerDetails" runat="server" Visible="false" Text="Customer Details" TabIndex="18"
                                                                                CssClass="button" OnClick="btnCustomerDetails_Click" />
                                                                            <asp:Button ID="btnCustomerList" runat="server" Text="Customer List" TabIndex="19"
                                                                                CssClass="button" OnClick="btnCustomerList_Click" />
                                                                            <asp:Button ID="btnSchedule" runat="server" Text="Schedule" TabIndex="19" Visible="false"
                                                                                CssClass="button" OnClick="btnSchedule_Click" Width="114px" />
                                                                        </td>
                                                                        <td>
                                                                            <asp:Button ID="imgStatement" runat="server" CssClass="inputImgBtn" Text="Statement Email"
                                                                                OnClick="imgStatement_Click" />
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
                    <tr>
                        <td>
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="imgStatement" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1"
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
</asp:Content>
