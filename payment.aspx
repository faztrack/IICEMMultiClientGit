<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="payment.aspx.cs" Inherits="payment" Title="Payment Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
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
                    <td align="center">
                        <asp:CheckBox ID="chkDifferentName" runat="server"
                            Text="Different Name and Address than above" Width="477px"
                            AutoPostBack="True" OnCheckedChanged="chkDifferentName_CheckedChanged"
                            TabIndex="1" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Panel ID="pnlDifferentName" runat="server" Visible="false">
                            <table align="center" cellpadding="0" cellspacing="2" width="98%">
                                <tr>
                                    <td align="center" colspan="2"></td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>First Name: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtFirstName" runat="server" Width="150px" TabIndex="2"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>Last Name: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtLastName" runat="server" Width="150px" TabIndex="3"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>Address: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtAddress" runat="server" Width="300px" TabIndex="3"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>City: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtCity" runat="server" TabIndex="4" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>State: </b>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>Zip: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtZip" runat="server" Width="100px" TabIndex="6"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="40%">
                                        <b>Phone: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtPhone" runat="server" TabIndex="7" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2" width="40%">
                                        <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Panel ID="pnlPaymentHistory" runat="server" Visible="false">
                            <table align="center" cellpadding="0" cellspacing="2" width="98%">
                                <tr>
                                    <td align="center">
                                        <h2>Payment History</h2>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:GridView ID="grdPaymentHistory" runat="server" AutoGenerateColumns="False"
                                            CssClass="mGrid" OnRowDataBound="grdPaymentHistory_RowDataBound"
                                            PageSize="20" DataKeyNames="payment_id">
                                            <Columns>
                                                <asp:BoundField DataField="date" DataFormatString="{0:d}" HeaderText="Date" />
                                                <asp:BoundField DataField="payment_method_id" HeaderText="Method" />
                                                <asp:BoundField DataField="amount" DataFormatString="{0:c}"
                                                    HeaderText="Amount" />
                                                <asp:BoundField DataField="payment_term" HeaderText="Item" />
                                                <asp:BoundField DataField="credit_card_number" HeaderText="Reference" />
                                                <asp:BoundField DataField="Notes" HeaderText="Notes" />
                                                <asp:TemplateField HeaderText="Delete" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" OnClick="DeletePayment">Delete</asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
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
                            <tr>
                                <td align="center">
                                    <table border="1" bordercolor="silver" cellpadding="2" cellspacing="2" width="100%">
                                        <tr>
                                            <td align="right">
                                                <b>Financed Projects: </b>
                                            </td>
                                            <td align="left">
                                                <asp:CheckBox ID="chkFinancedProjects" runat="server" AutoPostBack="true"
                                                    OnCheckedChanged="chkFinancedProjects_CheckedChanged"
                                                    Text="Financed Projects" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Panel ID="pnlFinanceProjects" runat="server" Visible="False">
                                        <table cellpadding="0" cellspacing="2" width="98%" align="center">
                                            <tr>
                                                <td align="left" colspan="2">
                                                    <h3>Financed Projects</h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <b>Lending Institution: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtLendingInst" runat="server" TabIndex="23" Width="250px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <b>Approval Code: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtApprovalCode" runat="server" TabIndex="24"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">
                                                    <b>Amount approved: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtAmountApproved" runat="server" TabIndex="25"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table border="1" bordercolor="silver" cellpadding="2" cellspacing="2" width="100%" align="center">
                                        <tr>
                                            <td align="center"></td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:RadioButtonList ID="rdlPaymentType" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="rdlPaymentType_SelectedIndexChanged" RepeatColumns="4"
                                                    RepeatDirection="Horizontal" TabIndex="8">
                                                    <asp:ListItem Selected="True" Value="0">Card</asp:ListItem>
                                                    <asp:ListItem Value="8">Cash</asp:ListItem>
                                                    <asp:ListItem Value="7">Check</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <table id="tabCard" runat="server" width="50%">
                                                    <tr>
                                                        <td align="right">
                                                            <b>Card Type: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlCardType" runat="server" TabIndex="9">
                                                                <asp:ListItem Value="4">American Express</asp:ListItem>
                                                                <asp:ListItem Value="3">Discover</asp:ListItem>
                                                                <asp:ListItem Value="1">Master Card</asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="2">Visa</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Credit Card Number: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCreditCardNumber" runat="server" MaxLength="16"
                                                                TabIndex="10" Width="130px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Exp. Month: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlMonth" runat="server" TabIndex="11">
                                                                <asp:ListItem Selected="True" Value="0">Select</asp:ListItem>
                                                                <asp:ListItem Value="01">01</asp:ListItem>
                                                                <asp:ListItem Value="02">02</asp:ListItem>
                                                                <asp:ListItem Value="03">03</asp:ListItem>
                                                                <asp:ListItem Value="04">04</asp:ListItem>
                                                                <asp:ListItem Value="05">05</asp:ListItem>
                                                                <asp:ListItem Value="06">06</asp:ListItem>
                                                                <asp:ListItem Value="07">07</asp:ListItem>
                                                                <asp:ListItem Value="08">08</asp:ListItem>
                                                                <asp:ListItem Value="09">09</asp:ListItem>
                                                                <asp:ListItem Value="10">10</asp:ListItem>
                                                                <asp:ListItem Value="11">11</asp:ListItem>
                                                                <asp:ListItem Value="12">12</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Exp Year: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlYear" runat="server" TabIndex="12">
                                                                <asp:ListItem Selected="True" Value="0">Select</asp:ListItem>
                                                                <asp:ListItem Value="11">2011</asp:ListItem>
                                                                <asp:ListItem Value="12">2012</asp:ListItem>
                                                                <asp:ListItem Value="13">2013</asp:ListItem>
                                                                <asp:ListItem Value="14">2014</asp:ListItem>
                                                                <asp:ListItem Value="15">2015</asp:ListItem>
                                                                <asp:ListItem Value="16">2016</asp:ListItem>
                                                                <asp:ListItem Value="17">2017</asp:ListItem>
                                                                <asp:ListItem Value="18">2018</asp:ListItem>
                                                                <asp:ListItem Value="19">2019</asp:ListItem>
                                                                <asp:ListItem Value="20">2020</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>CVV: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCVV" runat="server" TabIndex="13" Width="50px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Amount: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCardAmount" runat="server" TabIndex="14"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Payment Terms: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlCardPaymentTerms" runat="server" TabIndex="15">
                                                                <asp:ListItem Selected="True" Value="1">Deposit</asp:ListItem>
                                                                <asp:ListItem Value="2">At Countertop Template</asp:ListItem>
                                                                <asp:ListItem Value="3">Start of Job</asp:ListItem>
                                                                <asp:ListItem Value="4">Balance Dua at Completion</asp:ListItem>
                                                                <asp:ListItem Value="5">At Final Measure</asp:ListItem>
                                                                <asp:ListItem Value="6">Al Delivery of Cabinets</asp:ListItem>
                                                                <asp:ListItem Value="7">At Substantial Completion</asp:ListItem>
                                                                <asp:ListItem Value="8">Others</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <b>Notes: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCardNotes" runat="server" Height="50px" TabIndex="16"
                                                                TextMode="MultiLine" Width="250px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <table id="tabCheck" runat="server">
                                                    <tr>
                                                        <td align="right">
                                                            <b>Check No: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCheckNo" runat="server" TabIndex="9" Width="150px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Identification No: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtIdentificationNo" runat="server" MaxLength="10"
                                                                TabIndex="9" Width="150px"></asp:TextBox>
                                                            (ex. Driver&#39;s Licence or any other ID)</td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Amount: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCheckAmount" runat="server" TabIndex="11"
                                                                Width="100px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Payment Terms: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlCheckPaymentTerms" runat="server" TabIndex="12">
                                                                <asp:ListItem Selected="True" Value="1">Deposit</asp:ListItem>
                                                                <asp:ListItem Value="2">At Countertop Template</asp:ListItem>
                                                                <asp:ListItem Value="3">Start of Job</asp:ListItem>
                                                                <asp:ListItem Value="4">Balance Dua at Completion</asp:ListItem>
                                                                <asp:ListItem Value="5">At Final Measure</asp:ListItem>
                                                                <asp:ListItem Value="6">Al Delivery of Cabinets</asp:ListItem>
                                                                <asp:ListItem Value="7">At Substantial Completion</asp:ListItem>
                                                                <asp:ListItem Value="8">Others</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <b>Notes: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCheckNotes" runat="server" Height="50px" TabIndex="13"
                                                                TextMode="MultiLine" Width="250px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <table id="tabCash" runat="server">
                                                    <tr>
                                                        <td align="right">
                                                            <b>Amount: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCashAmount" runat="server" TabIndex="9"
                                                                Width="100px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Payment Terms: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:DropDownList ID="ddlCashPaymentTerms" runat="server" TabIndex="10">
                                                                <asp:ListItem Selected="True" Value="1">Deposit</asp:ListItem>
                                                                <asp:ListItem Value="2">At Countertop Template</asp:ListItem>
                                                                <asp:ListItem Value="3">Start of Job</asp:ListItem>
                                                                <asp:ListItem Value="4">Balance Dua at Completion</asp:ListItem>
                                                                <asp:ListItem Value="5">At Final Measure</asp:ListItem>
                                                                <asp:ListItem Value="6">Al Delivery of Cabinets</asp:ListItem>
                                                                <asp:ListItem Value="7">At Substantial Completion</asp:ListItem>
                                                                <asp:ListItem Value="8">Others</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" valign="top">
                                                            <b>Notes: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtCashNotes" runat="server" Height="50px" TabIndex="11"
                                                                TextMode="MultiLine" Width="250px"></asp:TextBox>
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
                    <td align="center">
                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
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
                    <td align="center"></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnDepositTotal" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnfpId" runat="server" Value="0" />
                    </td>
                </tr>
                <tr>
                    <td align="center"></td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
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

