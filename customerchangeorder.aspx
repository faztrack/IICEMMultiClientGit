<%@ Page Title="Customer Change Order" Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="customerchangeorder.aspx.cs" Inherits="customerchangeorder" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td align="center">
                <div style="margin: 0 auto; width: 970px">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">
                                        <h1>Customer Change Order</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px"></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table cellpadding="2px" cellspacing="2px" width="100%">
                                            <tr>
                                                <td align="right">Customer Name: 
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">Sales Person: 
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblSalesPerson" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">Estimate Name: 
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblEstimateName" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">Change Order Name: 
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblChangeOrderName" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right">Change Order Status: 
                                                </td>
                                                <td align="left">
                                                    <asp:Label ID="lblCoStatus" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td align="right"></td>
                                                <td align="left"></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px"></td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center">
                                                    <asp:RadioButtonList ID="rdoSort" runat="server" AutoPostBack="True"
                                                        RepeatDirection="Horizontal"
                                                        OnSelectedIndexChanged="rdoSort_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Value="1">View by Locations</asp:ListItem>
                                                        <asp:ListItem Value="2">View by Sections</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <h3>
                                                        <asp:Label ID="lblRetailPricingHeader" runat="server" Text="Selected Items" Visible="false"></asp:Label></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:GridView ID="grdGrouping" runat="server" ShowFooter="True"
                                                        OnRowDataBound="grdGrouping_RowDataBound" AutoGenerateColumns="False"
                                                        CssClass="mGrid">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>' CssClass="grid_header" />
                                                                    <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="True"
                                                                        DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                                                        Width="100%" CssClass="mGrid">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id"
                                                                                HeaderStyle-Width="6%">
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL"
                                                                                HeaderStyle-Width="7%">
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name"
                                                                                HeaderStyle-Width="25%">
                                                                                <HeaderStyle Width="25%" />

                                                                            </asp:BoundField>
                                                                             <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblShort" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="25%" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                              <asp:BoundField DataField="measure_unit" HeaderText="UOM"
                                                                                HeaderStyle-Width="5%">
                                                                                <HeaderStyle Width="5%" />
                                                                            </asp:BoundField>
                                                                            
                                                                             <asp:TemplateField HeaderText="Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblQty" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="10%" />
                                                                                 <ItemStyle  HorizontalAlign="Center"/>
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel11" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                           
                                                                            <asp:TemplateField HeaderText="Ext. Price">
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblViewEcon" runat="server"
                                                                                        Text='<%# Eval("EconomicsCost") %>' Width="60px"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>

                                                                </ItemTemplate>
                                                                <FooterStyle HorizontalAlign="Right" />
                                                                <FooterTemplate>
                                                                    <%# GetTax()%><br />
                                                                    <%# GetGrandTotalPrice()%><br />
                                                                    <br />
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
                                                <td align="center">
                                                    <h3>
                                                        <asp:Label ID="lblDirectPricingHeader" runat="server" Text="The following items are Direct / Outsourced" Visible="False"></asp:Label></h3>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False"
                                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound"
                                                        ShowFooter="True" CaptionAlign="Top">
                                                        <FooterStyle CssClass="white_text" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header"
                                                                        Text='<%# Eval("colName").ToString() %>' />
                                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                                        CssClass="mGrid" DataKeyNames="item_id"
                                                                        OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                                        ShowFooter="True" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id"
                                                                                HeaderStyle-Width="6%">
                                                                                <HeaderStyle Width="6%" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="section_serial" HeaderText="SL"
                                                                                HeaderStyle-Width="7%">
                                                                                <HeaderStyle Width="7%" />
                                                                            </asp:BoundField>
                                                                            <asp:TemplateField>
                                                                                <HeaderTemplate>
                                                                                    <asp:Label ID="lblHeader2" runat="server" />
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblItemName2" runat="server"
                                                                                        Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="12%" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name"
                                                                                HeaderStyle-Width="28%">
                                                                                <HeaderStyle Width="28%" />
                                                                            </asp:BoundField>
                                                                           
                                                                              <asp:BoundField DataField="measure_unit" HeaderText="UOM"
                                                                                HeaderStyle-Width="5%">
                                                                                <HeaderStyle Width="5%" />
                                                                             </asp:BoundField>
                                                                            
                                                                             <asp:TemplateField HeaderText="Code">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblQty" runat="server" Text='<%# Eval("quantity") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="7%" />
                                                                                 <ItemStyle  HorizontalAlign="Center"/>
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel21" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Short Notes">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblShort1" runat="server" Text='<%# Eval("short_notes") %>' />
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="27%" />
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Ext. Price">
                                                                                <FooterTemplate>
                                                                                    <asp:Label ID="lblSubTotal2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                                </FooterTemplate>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblViewEcon1" runat="server"
                                                                                        Text='<%# Eval("EconomicsCost") %>' Width="60px"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="6%" />
                                                                                <ItemStyle HorizontalAlign="Right" />
                                                                                <FooterStyle HorizontalAlign="Right" />
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <PagerStyle CssClass="pgr" />
                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                    </asp:GridView>
                                                                </ItemTemplate>

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
                                    <td align="left">
                                        <b>* Please note that any change orders to an existing contract will delay the completion of your remodel. </b></td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px">
                                        <asp:Label ID="lbltax" runat="server" Visible="False"></asp:Label>
                                        <asp:Label ID="lblterms" runat="server" Visible="False"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblAcceptMessage" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:RadioButtonList ID="rdoStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rdoStatus_SelectedIndexChanged" RepeatDirection="Horizontal">
                                            <asp:ListItem Selected="True" Value="1">Pending</asp:ListItem>
                                            <asp:ListItem Value="2">Accept by Credit Card</asp:ListItem>
                                            <asp:ListItem Value="5">Accept Credited C/O</asp:ListItem>
                                            <asp:ListItem Value="3">Reject</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px"></td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="pnlAccept" runat="server" Visible="false">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <table cellpadding="2" cellspacing="2" width="100%" align="center" id="tblNewPayment"
                                                            runat="server" visible="false">
                                                            <tr>
                                                                <td><span style="color: #f00;">*</span> Required</td>
                                                                <td align="center"></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right" style="width: 200px;"><b>Payment Term: </b></td>
                                                                <td align="left">&nbsp;&nbsp;<asp:Label ID="lblPayterm" runat="server"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right"><b><span style="color: #f00;">*</span> Amount: </b></td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtAmount" runat="server" Width="100px" AutoPostBack="True" OnTextChanged="txtAmount_TextChanged"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" colspan="2">
                                                                    <asp:Panel ID="pnlExistCard" runat="server" Height="100%">
                                                                        <table cellpadding="2" cellspacing="2" width="100%">
                                                                            <tr>
                                                                                <td align="center">Existing Credit OR debit Cards</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:GridView ID="grdCardList" runat="server" CssClass="mGrid" AllowSorting="True" AutoGenerateColumns="False" Width="100%"
                                                                                        DataKeyNames="PaymentProfileId" OnSorting="grdCardList_Sorting" OnRowDataBound="grdCardList_RowDataBound">
                                                                                        <AlternatingRowStyle CssClass="alt" />
                                                                                        <Columns>
                                                                                            <asp:TemplateField HeaderText="Card Number" SortExpression="CardType">
                                                                                                <ItemTemplate>
                                                                                                    <asp:RadioButton ID="rdoSelect" runat="server" onclick="check_radio(this);" />
                                                                                                    <asp:Image ID="imggrdCardType" runat="server" CssClass="imgCard" />
                                                                                                    <asp:Label ID="lblgrdCreditCard" runat="server"></asp:Label>
                                                                                                    <asp:Label ID="lblCardMessage" runat="server"></asp:Label>
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle HorizontalAlign="Left" Width="34%" />
                                                                                            </asp:TemplateField>
                                                                                            <asp:TemplateField HeaderText="Card Holder Name" SortExpression="NameOnCard">
                                                                                                <ItemTemplate>
                                                                                                    <asp:Label ID="lblgrdNameOnCard" runat="server" Text='<%# Eval("NameOnCard") %>' />
                                                                                                    <asp:Label ID="lblNameMessage" runat="server"></asp:Label>
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle HorizontalAlign="Left" Width="36%" />
                                                                                            </asp:TemplateField>
                                                                                            <asp:TemplateField HeaderText="Expiration Date" SortExpression="ExpirationDate">
                                                                                                <ItemTemplate>
                                                                                                    <asp:Label ID="lblgrdExpirationDatee" runat="server" Text='<%# Eval("ExpirationDate") %>' />
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                                                            </asp:TemplateField>
                                                                                        </Columns>
                                                                                        <HeaderStyle CssClass="gridHeaderStyle" />
                                                                                        <PagerSettings Position="TopAndBottom" />
                                                                                        <PagerStyle HorizontalAlign="Left" CssClass="pagerStyle" />
                                                                                        <RowStyle CssClass="rowStyle" />
                                                                                    </asp:GridView>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </asp:Panel>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="left" colspan="2">
                                                                    <asp:CheckBox ID="chkNewCard" runat="server" AutoPostBack="True" Font-Bold="True" Text="Use new card to make a Payment" OnCheckedChanged="chkNewCard_CheckedChanged" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" colspan="2">
                                                                    <asp:Panel ID="pnlNewCard" runat="server" Height="100%">
                                                                        <table cellpadding="2" cellspacing="2" width="100%">
                                                                            <tr>
                                                                                <td align="left"></td>
                                                                                <td align="left">
                                                                                    <asp:Label ID="Label12" runat="server" CssClass="col-md-8 control-label textAlignL">The following information is related to your credit card</asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right" style="width: 198px;" align="right"><b><span style="color: #f00;">*</span> Card Holder Name:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtCardHolderName" runat="server" Width="200px"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> Card Holder Address:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Width="175px" Height="45px"></asp:TextBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> City:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> State:</b></td>
                                                                                <td align="left">
                                                                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> Zip:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> Credit Card Number:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtCreditCardNumber" runat="server" MaxLength="16" Width="220px" OnTextChanged="txtCreditCardNumber_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                                                    <asp:Image ID="imgCardType" runat="server" CssClass="imgCard" />
                                                                                    <asp:CheckBox ID="chkSaveCardInfo" runat="server" Text="Save this card for future use" Visible="False" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> Expiration Date:</b></td>
                                                                                <td align="left">
                                                                                    <table class="dropDownNewCenter">
                                                                                        <tr>
                                                                                            <td align="left">
                                                                                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                                                                                                    <asp:ListItem Value="0">Select</asp:ListItem>
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
                                                                                            <td align="left">
                                                                                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                                                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                                                                    <asp:ListItem Value="16">2016</asp:ListItem>
                                                                                                    <asp:ListItem Value="17">2017</asp:ListItem>
                                                                                                    <asp:ListItem Value="18">2018</asp:ListItem>
                                                                                                    <asp:ListItem Value="19">2019</asp:ListItem>
                                                                                                    <asp:ListItem Value="20">2020</asp:ListItem>
                                                                                                    <asp:ListItem Value="21">2021</asp:ListItem>
                                                                                                    <asp:ListItem Value="22">2022</asp:ListItem>
                                                                                                    <asp:ListItem Value="23">2023</asp:ListItem>
                                                                                                    <asp:ListItem Value="24">2024</asp:ListItem>
                                                                                                    <asp:ListItem Value="25">2025</asp:ListItem>
                                                                                                    <asp:ListItem Value="26">2026</asp:ListItem>
                                                                                                    <asp:ListItem Value="27">2027</asp:ListItem>
                                                                                                    <asp:ListItem Value="28">2028</asp:ListItem>
                                                                                                    <asp:ListItem Value="29">2029</asp:ListItem>
                                                                                                    <asp:ListItem Value="30">2030</asp:ListItem>
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="right"><b><span style="color: #f00;">*</span> CVV Code:</b></td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" MaxLength="4"></asp:TextBox>
                                                                                    <asp:Label ID="Label11" runat="server" CssClass="col-md-8 control-label textAlignL">(4-Digit number on front of AMEX, 3-Digit number on back of all other cards.)</asp:Label>
                                                                                    <asp:CustomValidator ID="valCheckOut" runat="server" Visible="False" ForeColor="DarkRed" OnServerValidate="valCheckOut_ServerValidate"></asp:CustomValidator>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </asp:Panel>

                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 200px;" align="right"><b>Comments:</b></td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtPayComments" runat="server" Height="44px" TextMode="MultiLine" Width="622px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" colspan="2">
                                                                    <asp:Label ID="lblReason" runat="server"></asp:Label></td>

                                                            </tr>
                                                            <tr>
                                                                <td align="center" colspan="2">
                                                                    <asp:Button ID="btnFinalizePayment" runat="server" Text="Confirm Payment" OnClick="btnFinalizePayment_Click" CssClass="button" />
                                                                     <asp:Button ID="btnDummy" runat="server" Text="Button" Style="display: none" />
                                                                    <%-- <asp:Button ID="btnSaveCardInfo" CssClass="hideElements" runat="server" OnClick="btnSaveCardInfo_Click" Width="1px" />--%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">&nbsp;</td>
                                                    <td align="left">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2" height="5px"></td>
                                                </tr>

                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="pnlAcceptA" runat="server" Visible="false">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Panel ID="pnlECheckPayment" runat="server" Height="100%">
                                                            <table cellpadding="2" cellspacing="2" width="100%" align="center" id="tblECheckPayment"
                                                                runat="server">
                                                                <tr>
                                                                    <td><span style="color: #f00;">*</span> Required</td>
                                                                    <td align="center"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 200px;"><b>Payment Term: </b></td>
                                                                    <td align="left">&nbsp;&nbsp;<asp:Label ID="lblPayTerm1" runat="server"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right"><b><span style="color: #f00;">*</span> Amount: </b></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtAmount1" runat="server" Width="100px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Account Holder Name:</b></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtBank_acct_name" runat="server" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Account Number:</b></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtBank_acct_num" runat="server" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Bank Name:</b></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtbank_name" runat="server" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Bank Routing Number:</b></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtBank_aba_code" runat="server" Width="200px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 198px;">&nbsp;</td>
                                                                    <td align="left">
                                                                        <asp:Label ID="lblReason0" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" style="width: 198px;">
                                                                        <asp:Button ID="btnAcceptDOWN" runat="server" CssClass="button" OnClick="btnAcceptDOWN_Click" Text="Submit" Visible="False" />
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:Button ID="btnEcheckPayment" runat="server" CssClass="button" OnClick="btnEcheckPayment_Click" Text="Confirm Payment" />

                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:Panel>

                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px"></td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="pnlAcceptCreditedCO" runat="server" Visible="false">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td align="right" style="width: 200px;"><b>Payment Term: </b></td>
                                                    <td align="left">&nbsp;&nbsp;<asp:Label ID="lblPayTermC" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px">&nbsp;</td>
                                                    <td align="left">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="right"><b><span style="color: #f00;">*</span> Amount: </b></td>
                                                    <td align="left">&nbsp;&nbsp;
                                                        <asp:Label ID="lblAmountC" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px">&nbsp;</td>
                                                    <td align="left">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px">&nbsp;</td>
                                                    <td align="left">
                                                        <asp:Label ID="lblCreditedResult" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2" height="5px"></td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px"></td>
                                                    <td align="left">
                                                        <asp:Button ID="btnAcceptCreditedCO" runat="server" Text="Accept"
                                                            OnClick="btnAcceptCreditedCO_Click" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="pnlReject" runat="server" Visible="false">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td align="right" width="30px"></td>
                                                    <td align="left">
                                                        <asp:Label ID="lblReject" runat="server" Text="Reason for reject"
                                                            Font-Bold="True" Font-Size="Small"></asp:Label><br />
                                                        <asp:TextBox ID="txtReject" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2" height="5px"></td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px">&nbsp;</td>
                                                    <td align="left">
                                                        <asp:Label ID="lblRejectMessage" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" colspan="2" height="5px"></td>
                                                </tr>
                                                <tr>
                                                    <td align="right" width="30px"></td>
                                                    <td align="left">
                                                        <asp:Button ID="btnReject" runat="server" Text="Reject"
                                                            OnClick="btnReject_Click" CssClass="button" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px">
                                        <%--<cc1:ConfirmButtonExtender ID="ConfirmButtonExtender2A" TargetControlID="btnAcceptDOWN" OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender2A" runat="server">
                                        </cc1:ConfirmButtonExtender>
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender2A" TargetControlID="btnAcceptDOWN" BackgroundCssClass="modalBackground" CancelControlID="btnCancelA" OkControlID="btnOKA" PopupControlID="pnlConfirmation2A" runat="server">
                                        </cc1:ModalPopupExtender>--%>

                                        <cc1:ConfirmButtonExtender ID="btnEcheckPayment_ConfirmButtonExtender" runat="server" DisplayModalPopupID="btnEcheckPayment_ModalPopupExtender" OnClientCancel="cancelClick" TargetControlID="btnEcheckPayment">
                                        </cc1:ConfirmButtonExtender>
                                        <cc1:ModalPopupExtender ID="btnEcheckPayment_ModalPopupExtender" runat="server" BackgroundCssClass="modalBackground" CancelControlID="btnCancel1" OkControlID="btnOK1" PopupControlID="pnlConfirmation1" TargetControlID="btnEcheckPayment">
                                        </cc1:ModalPopupExtender>
                                        <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="btnFinalizePayment"
                                            OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                                        </cc1:ConfirmButtonExtender>
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="btnFinalizePayment" BackgroundCssClass="modalBackground"
                                            CancelControlID="btnCancel" OkControlID="btnOK" PopupControlID="pnlConfirmation"
                                            runat="server">
                                        </cc1:ModalPopupExtender>
                                        <cc1:ModalPopupExtender ID="ModalPopupExtender3" TargetControlID="btnDummy" BackgroundCssClass="modalBackground"
                                            CancelControlID="btnHide" PopupControlID="pnlPopup"
                                            runat="server">
                                        </cc1:ModalPopupExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:HiddenField ID="hdnCOId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnPricingId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnID" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnCOStatusId" runat="server" EnableViewState="False"
                                            Value="0" />
                                        <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                                        <asp:HiddenField ID="hdnPayId" runat="server" Value="0" />
                                         <asp:HiddenField ID="hdnPayTermId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnChangeOrderView" runat="server" Value="0" />
                                    </td>
                                </tr>
                            </table>

                        </ContentTemplate>
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
                                            <b>Interior Innovations is about to charge 
                                <asp:Label ID="lblamount" runat="server"></asp:Label>
                                                to your credit card.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Do you Authorize this payment?</b>
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
                    <asp:Panel ID="pnlConfirmation1" runat="server" Width="550px" Height="100px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Interior Innovations is about to charge 
                                <asp:Label ID="lblamount1" runat="server"></asp:Label>
                                                to your bank account.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Do you Authorize this payment?</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnOk1" runat="server" Text="Yes" CssClass="button" Width="60px" />
                                            <asp:Button ID="btnCancel1" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <asp:Panel ID="pnlPopup" runat="server" Style="display: none" Width="550px" Height="100px" BackColor="Snow">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                    <tr>
                                        <td align="right">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Sorry, we do not accept AMEX. Please use an alternate credit card.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnHide" runat="server" Text="OK" CssClass="button" Width="60px" />
                                        </td>
                                    </tr>
                                </table>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <%--  <asp:Panel ID="pnlConfirmation2A" runat="server" BackColor="Snow" Height="100px" Width="550px">
                        <asp:UpdatePanel ID="UpdatePanel2A" runat="server">
                            <ContentTemplate>
                                <table align="center" cellpadding="0" cellspacing="2" width="100%">
                                    <tr>
                                        <td align="right">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <b>Are you sure to accept this change order? </b></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnOkA" runat="server" CssClass="button" Text="Yes"
                                                Width="60px" />
                                            <asp:Button ID="btnCancelA" runat="server" CssClass="button" Text="No"
                                                Width="60px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>--%>
                </div>
            </td>

        </tr>
        <tr>
            <td align="center">

                <asp:Label ID="lblSalesPersonEmail" runat="server" Visible="False"></asp:Label>
                &nbsp;<asp:Label ID="lblComProfilelEmail" runat="server" Visible="False"></asp:Label>
                <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
                <asp:HiddenField ID="hdnCCEmail" runat="server" Value="" />
                <asp:Label ID="lblCustEmail" runat="server" Visible="False"></asp:Label>
                <asp:Label ID="lblLastName" runat="server"
                    Visible="False"></asp:Label>

            </td>
        </tr>
    </table>

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
</asp:Content>





