<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="payment_info.aspx.cs"
    Inherits="payment_info" Title="Payment Schedule & Terms" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }

        function DisplayEmailWindow(FileName) {

            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerId.ClientID%>').value + '&cfn=' + FileName, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

        }
    </script>




    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="0" width="100%" align="center">
                    <tr>
                        <td align="center" class="cssHeader">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="left"><span class="titleNu">Payment Schedule & Terms</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
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
                                            <tr>
                                                <td align="left" valign="top"><b>Estimate Name: </b></td>
                                                <td>
                                                    <asp:Label ID="lblEstimateName" Font-Bold="true" runat="server" Text=""></asp:Label></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left" valign="top">
                                        <table>
                                            <tr>
                                                <td style="width: 100px;" align="left" valign="top"><b>Address: </b></td>
                                                <td align="left" valign="top">
                                                    <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label></td>

                                                <td align="left" valign="top">
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink></td>
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
                        <td align="left">
                            <table class="wrapper" width="100%">
                                <tr>
                                    <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <img src="images/icon-pfi.png" /></td>
                                                <td align="left">
                                                    <h2>Project Financial Information</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td align="center">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 200px;" align="left"><b>Project Total: </b></td>
                                                            <td align="left"></td>
                                                            <td style="width: 96px;" align="right">
                                                                <asp:Label ID="lblProjectSubtotal" runat="server" CssClass="amountNos" Text="$0.00"></asp:Label></td>
                                                            <td align="left">
                                                                <asp:Button ID="btnReCalculate" runat="server" Visible="false" Text="Re-Calculate" OnClick="btnReCalculate_Click" CssClass="button" /></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px;" align="left"><b>Tax %: </b></td>
                                                            <td style="width: 48px;" align="left">
                                                                <asp:TextBox ID="txtTax" runat="server" Width="48px" OnTextChanged="txtTax_TextChanged" AutoPostBack="True" CssClass="cssTxtAmount">0.00</asp:TextBox></td>
                                                            <td style="width: 96px;" align="right" valign="middle">
                                                                <asp:Label ID="lblTax" runat="server" CssClass="amountNos"></asp:Label></td>
                                                            <td align="center" valign="middle"></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px;" align="left"><b>Total with Tax:</b></td>
                                                            <td align="right"></td>
                                                            <td style="width: 96px;" align="right">
                                                                <asp:Label ID="lblTotalWithTax" CssClass="amountNos" runat="server">$0.00</asp:Label></td>
                                                            <td align="left">&nbsp;</td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="4">
                                                                <table id="tblTax" width="100%" runat="server">
                                                                    <tr>
                                                                        <td align="right">
                                                                            <cc1:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtTax"
                                                                                FilterType="Custom" FilterMode="ValidChars" InvalidChars=" " ValidChars="1234567890.">
                                                                            </cc1:FilteredTextBoxExtender>
                                                                        </td>
                                                                        <td align="right">
                                                                            <asp:RequiredFieldValidator ID="TReq" runat="server" ControlToValidate="txtTax" Display="None"
                                                                                ErrorMessage="&lt;b&gt;Tax% is required.&lt;/b&gt;&lt;br /&gt;"></asp:RequiredFieldValidator>
                                                                        </td>
                                                                        <td align="right">
                                                                            <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" TargetControlID="TReq"
                                                                                HighlightCssClass="validatorCalloutHighlight">
                                                                            </cc1:ValidatorCalloutExtender>
                                                                        </td>
                                                                        <td align="left">&nbsp;
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <table id="tblAdjustment" runat="server" visible="false">
                                                        <tr>
                                                            <td style="width: 200px;" align="left">
                                                                <b>Adjusted Price: </b>
                                                            </td>
                                                            <td style="width: 64px;" align="left">&nbsp;
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblAdjustedPrice" runat="server" CssClass="amountNos">$0.00</asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3">
                                                                <table id="tblAdjTax" width="100%" runat="server">
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left" valign="top">
                                                                            <b>Tax % : </b>
                                                                        </td>
                                                                        <td align="left" style="width: 90px;">
                                                                            <i>
                                                                                <asp:Label ID="lblAdjustedTaxRate" runat="server"></asp:Label></i>
                                                                        </td>
                                                                        <td align="right" style="width: 50px;">
                                                                            <asp:Label ID="lblAdjustedTax" runat="server" CssClass="amountNos">$0.00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 200px;" align="left" valign="top">
                                                                            <b>New Total with Tax: </b>
                                                                        </td>
                                                                        <td></td>
                                                                        <td align="right" style="width: 50px;">
                                                                            <asp:Label ID="lblNewTotalWithTax" runat="server" CssClass="amountNos">$0.00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4" align="center">
                                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:Panel ID="pnlIncentive" runat="server">
                                                        <table width="100%" id="tblIncentives" runat="server" visible="false">
                                                            <tr>
                                                                <td align="left" colspan="2">
                                                                    <h3>Promotions & Incentives:</h3>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" colspan="2">
                                                                    <asp:GridView ID="grdIncentives" runat="server" AutoGenerateColumns="False" Width="100%"
                                                                        DataKeyNames="incentive_id" CssClass="mGrid">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Check">
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chk" runat="server" AutoPostBack="True" OnCheckedChanged="chk_CheckedChanged"
                                                                                        CausesValidation="True" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="incentive_name" HeaderText="Current Incentives">
                                                                                <HeaderStyle HorizontalAlign="Left" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="discount" HeaderText="Discount" Visible="False" />
                                                                            <asp:BoundField HeaderText="Amount">
                                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField HeaderText="Expiration Date" DataField="end_date" DataFormatString="{0:d}">
                                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                                <ItemStyle HorizontalAlign="Center" />
                                                                            </asp:BoundField>
                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
                                                                    <b>Total Incentives: </b>
                                                                </td>
                                                                <td align="left">&nbsp;
                                                            <asp:Label ID="lblTotalIncentives" runat="server" Text="$0.00"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="width: 200px;" align="left">
                                                                <b>Project Completion Duration: </b>
                                                            </td>
                                                            <td align="left" colspan="2">
                                                                <asp:RadioButtonList ID="rdoCompletionType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdoCompletionType_SelectedIndexChanged"
                                                                    RepeatDirection="Horizontal">
                                                                    <asp:ListItem Selected="True" Value="1">Lead Time Based</asp:ListItem>
                                                                    <asp:ListItem Value="2">Start &amp; End Date Based</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <b>
                                                                    <asp:Label ID="lblLeadTime" runat="server" Text="Lead Time:" ForeColor="#717171"></asp:Label></b>
                                                            </td>
                                                            <td align="left" style="width: 200px">
                                                                <asp:TextBox ID="txtLeadTime" runat="server" Width="135px"></asp:TextBox>
                                                            </td>
                                                            <td></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px;" align="left">
                                                                <b>
                                                                    <asp:Label ID="lblStartDate" runat="server" Text="Start Date:" ForeColor="#717171"></asp:Label></b>
                                                            </td>
                                                            <td align="left" style="width: 200px">
                                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td align="left">
                                                                            <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                                                                        </td>
                                                                        <td>&nbsp;</td>
                                                                        <td align="left">
                                                                            <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td valign="top" align="left" style="font-weight: bold">
                                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td align="left">
                                                                            <b>
                                                                                <asp:Label ID="lblCompletionDate" runat="server" Text="Completion Date:" ForeColor="#717171"></asp:Label></b>
                                                                            <asp:TextBox ID="txtCompletionDate" runat="server"></asp:TextBox>
                                                                        </td>
                                                                        <td>&nbsp;</td>
                                                                        <td align="left">
                                                                            <asp:ImageButton ID="imgCompletionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px;" align="left">
                                                                <b>Contract Date: </b>
                                                            </td>
                                                            <td align="left" style="width: 180px">
                                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td align="left">
                                                                            <asp:TextBox ID="txtContractDate" runat="server"></asp:TextBox>
                                                                        </td>
                                                                        <td>&nbsp;</td>
                                                                        <td align="left">
                                                                            <asp:ImageButton ID="imgContractDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td align="left">
                                                                <table width="48%" style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td align="right" style="padding: 0px; margin: 0px;"></td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px;" align="left" valign="top">
                                                                <b>Special Notes: </b>
                                                                <br />
                                                                <i style="font-size: 11px; margin: 0 5px; color: #2196f3;">(Printed on the Signature Page)</i>
                                                                <br />
                                                                <asp:TextBox ID="txtDisplay" runat="server" BackColor="Transparent" BorderColor="Transparent" CssClass="nostyle"
                                                                    BorderStyle="None" BorderWidth="0px" Font-Bold="True" Height="16px" ReadOnly="True"></asp:TextBox>
                                                            </td>
                                                            <td align="left" valign="top" colspan="2">
                                                                <asp:TextBox ID="txtSpecialNote" runat="server" Height="45px" MaxLength="50" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplay'));" TextMode="MultiLine" Width="475px"></asp:TextBox>
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
                        <td align="center">
                            <table class="wrapper" width="100%">
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
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td align="middle">
                                                    <callbackpanel id="CallbackPanel1" runat="server" triggers="{ControlID:btnCalculate;Parameter:},{ControlID:btnSave;Parameter:},{ControlID:txtDiscountFee;Parameter:},{ControlID:chkDiscountFee;Parameter:}">
                                        </callbackpanel>
                                                    <table class="wrappermini" id="Table1" width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td align="middle">
                                                                    <!--StartFragment-->
                                                                    <table id="Table2" width="100%">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td class="nos">1</td>
                                                                                <td style="width: 76px">
                                                                                    <asp:TextBox ID="txtDepositDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 26px">
                                                                                    <asp:ImageButton ID="imgDepositDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td style="width: 164px">
                                                                                    <asp:TextBox ID="txtDepositValue" runat="server" Text="Deposit" MaxLength="150" Width="160px" TabIndex="1"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 64px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpDeposit" runat="server" Width="34px" TabIndex="2">30</asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td style="width: 90px">
                                                                                    <asp:TextBox ID="txtnDeposit" runat="server" Width="72" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 178px">
                                                                                    <asp:TextBox ID="txtMeasureValue" runat="server" Text="At Final Measure" Width="160px"
                                                                                        MaxLength="150" TabIndex="11"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 64px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpMeasure" runat="server" Width="34px" TabIndex="12"></asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td style="width: 92px">
                                                                                    <asp:TextBox ID="txtnMeasure" runat="server" Width="72px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td style="width: 76px">
                                                                                    <asp:TextBox ID="txtMeasureDate" runat="server" Width="75px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton ID="imgMeasureDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">6</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="nos">2</td>
                                                                                <td style="height: 25px">
                                                                                    <asp:TextBox ID="txtCountertopDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <asp:ImageButton ID="imgCountertopDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <asp:TextBox ID="txtCountertopValue" runat="server" Text="At Countertop Template" TabIndex="3"
                                                                                        Width="160px" MaxLength="150"></asp:TextBox>
                                                                                </td>
                                                                                <td valign="top">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpCountertop" runat="server" Width="33px" TabIndex="4">30</asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnCountertop" runat="server" Width="73px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtDeliveryValue" runat="server" Text="At Delivery of Cabinets"
                                                                                        Width="160px" MaxLength="150" TabIndex="13"></asp:TextBox>
                                                                                </td>
                                                                                <td style="height: 25px">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpDelivery" runat="server" Width="33px" TabIndex="14"></asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnDelivery" runat="server" Width="73px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton ID="imgDeliveryDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">7</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="nos">3</td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartOfJobDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton ID="imgStartOfJobDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartOfJobValue" runat="server" Text="Start of Job" Width="160px"
                                                                                        MaxLength="150" TabIndex="5"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpJob" runat="server" Width="33px" TabIndex="6">30</asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnJob" runat="server" Width="73px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtSubstantialValue" runat="server" Text="At Substantial Completion"
                                                                                        Width="160px" MaxLength="150" TabIndex="15"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpSubstantial" runat="server" TabIndex="16" Width="33px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnSubstantial" runat="server" Width="73px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtSubstantialDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td width="32px">
                                                                                    <asp:ImageButton ID="imgSubstantialDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">8</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="nos">4</td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartofFlooringDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td width="32px">
                                                                                    <asp:ImageButton ID="imgStartofFlooringDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartofFlooringValue" runat="server" Text="At Start of Flooring"
                                                                                        Width="160px" MaxLength="150" TabIndex="7"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpFlooring" runat="server" Width="33px" TabIndex="8"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <table style="padding: 0px; margin-left: -2px; border: none; width: 100%;">
                                                                                        <tr>
                                                                                            <td style="padding: 0px; margin: 0px; border: none; text-align: left;">
                                                                                                <asp:TextBox ID="txtnFlooring" runat="server" Width="72px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                            </td>
                                                                                            <td style="padding: 0px; margin: 0px; border: none; text-align: right;">Other:
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>

                                                                                </td>

                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtOthers" runat="server" TabIndex="17" Style="width: 62%;"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpOthers" runat="server" Width="33px" TabIndex="18"></asp:TextBox>
                                                                                            </td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnOthers" runat="server" Width="73px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtOtherDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td width="32px">
                                                                                    <asp:ImageButton ID="imgOtherDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">9</td>





                                                                            </tr>
                                                                            <tr>
                                                                                <td class="nos">5</td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartofDrywallDate" runat="server" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td width="32px">
                                                                                    <asp:ImageButton ID="imgStartofDrywallDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>

                                                                                <td>
                                                                                    <asp:TextBox ID="txtStartofDrywallValue" runat="server" Text="At Start of Drywall"
                                                                                        Width="160px" MaxLength="150" TabIndex="9"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpDrywall" runat="server" TabIndex="10" Width="33px"></asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnDrywall" runat="server" Width="72px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>

                                                                                <td>
                                                                                    <asp:TextBox ID="txtDueCompletionValue" runat="server" Text="Balance Due at Completion"
                                                                                        Width="160px" MaxLength="150" TabIndex="19"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:TextBox ID="txtpBalance" runat="server" Width="33px" TabIndex="20">10</asp:TextBox></td>
                                                                                            <td>%</td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtnBalance" runat="server" Width="72px" CssClass="cssTxtAmount"></asp:TextBox>
                                                                                </td>

                                                                                <td>
                                                                                    <asp:TextBox ID="txtDueCompletionDate" runat="server" CssClass="cssTxtAmount" Width="75px"></asp:TextBox>
                                                                                </td>
                                                                                <td width="32px">
                                                                                    <asp:ImageButton ID="imgDueCompletionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                </td>
                                                                                <td class="nos">10</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="3" align="right">Total Percentage:
                                                                                </td>
                                                                                <td>
                                                                                    <asp:Label ID="lblPr" runat="server"></asp:Label>
                                                                                </td>
                                                                                <td colspan="2">
                                                                                    <asp:RadioButtonList ID="rdoCalc" runat="server" RepeatDirection="Horizontal" Width="250px">
                                                                                        <asp:ListItem Selected="True" Value="1">Calc based on %</asp:ListItem>
                                                                                        <asp:ListItem Value="2">Calc based on $</asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </td>
                                                                                <td align="left" colspan="4">
                                                                                    <asp:Button ID="btnCalculate" runat="server" CausesValidation="False" CssClass="button"
                                                                                        OnClick="btnCalculate_Click" Text="Calculate" Width="100px" />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:Button ID="btnGotoPricing" runat="server" Text="Go to Pricing" OnClick="btnGotoPricing_Click"
                                                        CssClass="button" CausesValidation="False" />
                                                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" Width="80px"
                                                        CssClass="button" />
                                                    <asp:Button ID="btnAcceptPayment" Visible="false" runat="server" CssClass="button" Text="Accept Payment"
                                                        OnClick="btnAcceptPayment_Click" />
                                                    <asp:Button ID="btnFinalize" runat="server" CssClass="button" Text="Finalize this Estmate"
                                                        OnClick="btnFinalize_Click" />
                                                    <asp:Button ID="btnCancelPayment" runat="server" CssClass="button" Text="Cancel"
                                                        OnClick="btnCancelPayment_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <table>
                                                        <tr>
                                                            <td align="right" valign="top">&nbsp;</td>
                                                            <td align="right" valign="top">Contract View Options: </td>
                                                            <td align="left" valign="top">
                                                                <asp:CheckBoxList ID="chkCVOptions" runat="server">
                                                                    <asp:ListItem Value="1" Selected="True">Quantity</asp:ListItem>
                                                                    <asp:ListItem Value="2" Selected="True">Sub-Total</asp:ListItem>
                                                                </asp:CheckBoxList>
                                                            </td>
                                                            <td align="left" valign="top">

                                                                <asp:CheckBox ID="chkBath" runat="server" Text="Print Bathroom Selections Sheet" />
                                                                <%-- <asp:LinkButton ID="lnkBath" runat="server" Text="(Edit)"></asp:LinkButton>--%>
                                                                <br />
                                                                <asp:CheckBox ID="chkKitchen" runat="server" Text="Print Kitchen Selections Sheet" />

                                                            </td>
                                                            <td align="left" valign="top" >
                                                                <asp:CheckBox ID="chkShower" runat="server" Text="Print Shower Selections Sheet" />
                                                                <br />
                                                                <asp:CheckBox ID="chkTub" runat="server" Text="Print Tub Selections Sheet" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" style="height: 34px">
                                                                <asp:Button ID="btnPrintSummary" runat="server" CssClass="button" OnClick="btnPrintSummary_Click"
                                                                    Text="Print Estimate Summary" />
                                                            </td>
                                                            <td align="center" style="height: 34px">
                                                                <asp:Button ID="btnContact" runat="server" CssClass="button" OnClick="btnContact_Click" Text="Print Contract Document" />
                                                            </td>
                                                            <td style="height: 34px">
                                                                <asp:RadioButtonList ID="rdoSort" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Selected="True" Value="1">By Locations</asp:ListItem>
                                                                    <asp:ListItem Value="2">By Sections</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                            <td style="height: 34px">
                                                                <asp:Button ID="btnHTML" runat="server" CssClass="button" OnClick="btnHTML_Click"
                                                                    Text="Quick Contract" />
                                                            </td>
                                                            <td align="left" style="height: 34px">
                                                                <asp:Button ID="btnDocuSign" runat="server" CssClass="button" OnClick="btnDocuSign_Click" Text="DocuSign" Width="164px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:Button ID="btnEmailSummary" runat="server" CssClass="button" OnClick="btnEmailSummary_Click" Text="Email Estimate Summary" />
                                                            </td>
                                                            <td align="center">
                                                                <asp:Button ID="btnContactMail" runat="server" CssClass="button" OnClick="btnContactMail_Click" Text="Email Contract Document" />
                                                            </td>
                                                            <td>&nbsp;</td>
                                                            <td>
                                                                <asp:Button ID="btnQuickMail" runat="server" CssClass="button" OnClick="btnQuickMail_Click" Text="EMail Quick Contract" />
                                                            </td>
                                                            <td align="left">&nbsp;</td>
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
                            <cc1:CalendarExtender ID="DepositDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDepositDate"
                                PopupPosition="BottomLeft" TargetControlID="txtDepositDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="ContractDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgContractDate"
                                PopupPosition="BottomLeft" TargetControlID="txtContractDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="CountertopDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgCountertopDate"
                                PopupPosition="BottomLeft" TargetControlID="txtCountertopDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="StartOfJobDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartOfJobDate"
                                PopupPosition="BottomLeft" TargetControlID="txtStartOfJobDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="DueCompletionDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDueCompletionDate"
                                PopupPosition="BottomLeft" TargetControlID="txtDueCompletionDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="StartofDrywall" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartofDrywallDate"
                                PopupPosition="BottomLeft" TargetControlID="txtStartofDrywallDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="StartofFlooring" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartofFlooringDate"
                                PopupPosition="BottomLeft" TargetControlID="txtStartofFlooringDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="MeasureDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgMeasureDate"
                                PopupPosition="BottomLeft" TargetControlID="txtMeasureDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="DeliveryDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgDeliveryDate"
                                PopupPosition="BottomLeft" TargetControlID="txtDeliveryDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="SubstantialDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgSubstantialDate"
                                PopupPosition="BottomLeft" TargetControlID="txtSubstantialDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="OtherDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgOtherDate"
                                PopupPosition="BottomLeft" TargetControlID="txtOtherDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="StartDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                            </cc1:CalendarExtender>
                            <cc1:CalendarExtender ID="CompletionDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgCompletionDate"
                                PopupPosition="BottomLeft" TargetControlID="txtCompletionDate">
                            </cc1:CalendarExtender>
                            <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="btnFinalize"
                                OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                            </cc1:ConfirmButtonExtender>
                            <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="btnFinalize" BackgroundCssClass="modalBackground"
                                CancelControlID="btnCancel" OkControlID="btnOK" PopupControlID="pnlConfirmation"
                                runat="server">
                            </cc1:ModalPopupExtender>
                            <cc1:ConfirmButtonExtender ID="ConfirmButtonExtenderDocu" TargetControlID="btnDocuSign"
                                OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtenderDocu" runat="server">
                            </cc1:ConfirmButtonExtender>
                            <cc1:ModalPopupExtender ID="ModalPopupExtenderDocu" TargetControlID="btnDocuSign" BackgroundCssClass="modalBackground"
                                CancelControlID="btnDocuCancel" OkControlID="btnDocuOk" PopupControlID="pnlDocuConfirmation"
                                runat="server">
                            </cc1:ModalPopupExtender>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnProjectTotal" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnfpId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnLastName" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>

                <asp:AsyncPostBackTrigger ControlID="btnQuickMail" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnContactMail" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:Panel ID="pnlConfirmation" runat="server" Width="550px" Height="100px" BackColor="Snow">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <table cellpadding="0" cellspacing="2" width="100%" align="center">
                        <tr>
                            <td align="right">&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <b>Clicking &#39;Yes&#39; will freeze this Estimate.</b>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <b>Are you sure you want to save this Sold Estimate?</b>
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
        <asp:Panel ID="pnlDocuConfirmation" runat="server" Width="550px" Height="100px" BackColor="Snow">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <table cellpadding="0" cellspacing="2" width="100%" align="center">
                        <tr>
                            <td align="right">&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <b>The customer is going to receive the contract in an email from DocuSign for Signature. </b>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <b>Please OK to send.</b>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnDocuOk" runat="server" Text="OK" CssClass="button" Width="60px" />
                                <asp:Button ID="btnDocuCancel" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                            </td>
                        </tr>

                    </table>
                </ContentTemplate>

            </asp:UpdatePanel>
        </asp:Panel>
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
