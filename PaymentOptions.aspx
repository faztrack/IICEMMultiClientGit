<%@ Page Title="Add Cards" Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="PaymentOptions.aspx.cs" Inherits="PaymentOptions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="wrapper" cellpadding="5px" cellspacing="5px" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Add Cards</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td>
                        <table cellpadding="2" cellspacing="2" width="100%">

                            <tr>
                                <td align="center">
                                    <table style="width: 56%;" class="wrapm wrapm-ext">
                                        <tr>
                                            <td colspan="3" align="left" style="padding-bottom: 10px;"><span style="color: #f00;">*</span> Required</td>
                                        </tr>
                                        <tr>
                                            <td colspan="3"><span style="color: #f00;">*&nbsp;</span><asp:Label ID="lblNameOnCard" CssClass="labelCss-5" runat="server" Text="Card Holder Name"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:TextBox ID="txtNameOnCard" Width="544px" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="179px">
                                                <span style="color: #f00;">*&nbsp;</span><asp:Label ID="lblCardNumber" CssClass="labelCss-5" runat="server" Text="Card Number"></asp:Label>
                                            </td>
                                            <td>
                                                <span style="color: #f00;">*&nbsp;</span><asp:Label ID="lblExpirationDate" CssClass="labelCss-5" runat="server" Text="Expiration Date"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16" Width="175px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlExpirationMonth" runat="server">
                                                    <asp:ListItem Text="01" Value="01"></asp:ListItem>
                                                    <asp:ListItem Text="02" Value="02"></asp:ListItem>
                                                    <asp:ListItem Text="03" Value="03"></asp:ListItem>
                                                    <asp:ListItem Text="04" Value="04"></asp:ListItem>
                                                    <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                                    <asp:ListItem Text="06" Value="06"></asp:ListItem>
                                                    <asp:ListItem Text="07" Value="07"></asp:ListItem>
                                                    <asp:ListItem Text="08" Value="08"></asp:ListItem>
                                                    <asp:ListItem Text="09" Value="09"></asp:ListItem>
                                                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="ddlExpirationYear" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td><b><span style="color: #f00;">*</span> CVV Code</b></td>
                                            <td valign="top">&nbsp;</td>
                                            <td valign="top">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" Width="175px" MaxLength="4"></asp:TextBox>
                                            </td>
                                            <td valign="middle">
                                                <asp:Label ID="Label11" runat="server" CssClass="col-md-8 control-label textAlignL">&nbsp;(4-Digit number on front of AMEX, 3-Digit number on back of all other cards.)</asp:Label>
                                            </td>
                                            <td valign="top">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <table width="66%">
                                                    <tr>
                                                        <td width="192px">
                                                            <asp:Label ID="lblAddress" CssClass="labelCss" runat="server" Width="179px" Text="Card Holder Address"></asp:Label>
                                                        </td>
                                                        <td width="150px">
                                                            <asp:Label ID="lblCity" CssClass="labelCss" runat="server" Text="City"></asp:Label>
                                                        </td>
                                                        <td width="64px">&nbsp;
                                                            <asp:Label ID="lblState" CssClass="labelCss" runat="server" Text="State"></asp:Label>
                                                        </td>
                                                        <td>
                                                            &nbsp;&nbsp;
                                                            <asp:Label ID="lblZip" CssClass="labelCss-Zip" runat="server" Text="Zip"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <table width="66%">
                                                    <tr>
                                                        <td width="179px">
                                                            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Width="179px" Height="45px"></asp:TextBox>
                                                        </td>
                                                        <td width="145px" valign="top">
                                                            <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td width="64px" valign="top">
                                                            <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td valign="top">
                                                            <asp:TextBox ID="txtZip" CssClass="TextBoxCss-Zip" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" align="center">
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" align="center">
                                                <asp:Button ID="btnSave" runat="server" CssClass="button" OnClick="btnSave_Click" Text="Save" />
                                                <asp:Button ID="btnCancel" runat="server" CssClass="button" OnClick="btnCancel_Click" Text="Cancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                                    <asp:HiddenField ID="hdnCustomerType" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerFirstName" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnCustomerLastName" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnCustomerEmail" runat="server" Value="" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="2" cellspacing="2" width="100%">
                            <tr>
                                <td class="nameTitle" align="center">
                                    <h2>
                                        <asp:Label ID="lblCardList" runat="server" Visible="false">Credit Cards List</asp:Label></h2>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="grdCardList" CssClass="col-md-12 mGrid" runat="server" AllowSorting="True" AutoGenerateColumns="False" Width="100%"
                                        DataKeyNames="PaymentProfileId" OnSorting="grdCardList_Sorting" OnRowDataBound="grdCardList_RowDataBound" OnRowEditing="grdCardList_RowEditing" OnRowUpdating="grdCardList_RowUpdating" OnRowCommand="grdCardList_RowCommand" OnRowDeleting="grdCardList_RowDeleting">
                                        <AlternatingRowStyle CssClass="alternateRowStyle" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Card Number" SortExpression="CardType">
                                                <ItemTemplate>
                                                    <asp:Image ID="imggrdCardType" runat="server" CssClass="imgCard" />
                                                    <asp:Label ID="lblgrdCreditCard" runat="server">
                                                    </asp:Label>
                                                    <asp:TextBox ID="txtgrdCreditCard" runat="server" Visible="false" MaxLength="16"
                                                        Width="150px" Wrap="False"></asp:TextBox>
                                                    <br /><asp:Label ID="lblCardMessage" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" Width="12%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Card Holder Name" SortExpression="NameOnCard">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblgrdNameOnCard" runat="server" Text='<%# Eval("NameOnCard") %>' />
                                                    <asp:TextBox ID="txtgrdNameOnCard" runat="server" Visible="false" Text='<%# Eval("NameOnCard") %>'
                                                        Width="150px" Wrap="False"></asp:TextBox>
                                                    <asp:Label ID="lblNameMessage" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Card Holder Address">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbAddress" runat="server" />
                                                    <%--  <asp:Label ID="lblbCitySateZip" runat="server"></asp:Label>--%>
                                                    <asp:TextBox ID="txtbAddress" runat="server" Visible="false" ToolTip="Address" Text='<%# Eval("BillAddress") %>'
                                                        Width="100px"></asp:TextBox>
                                                    <asp:TextBox ID="txtbCity" runat="server" Visible="false" ToolTip="City" Text='<%# Eval("BillCity") %>'
                                                        Width="50px"></asp:TextBox>
                                                    <asp:DropDownList ID="ddlbState" runat="server" Visible="false" ToolTip="State" DataValueField="abbreviation" DataTextField="abbreviation" DataSource="<%#dtState%>">
                                                    </asp:DropDownList>
                                                    <asp:TextBox ID="txtbZip" runat="server" Visible="false" ToolTip="Zip" Text='<%# Eval("BillZip") %>'
                                                        Width="50px"></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" Width="32%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Expiration Date" SortExpression="ExpirationDate">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblgrdExpirationDatee" runat="server" Text='<%# Eval("ExpirationDate") %>' />
                                                    <asp:DropDownList ID="ddlgrdExpirationMonth" runat="server" Visible="false">
                                                        <asp:ListItem Text="01" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="02" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="03" Value="3"></asp:ListItem>
                                                        <asp:ListItem Text="04" Value="4"></asp:ListItem>
                                                        <asp:ListItem Text="05" Value="5"></asp:ListItem>
                                                        <asp:ListItem Text="06" Value="6"></asp:ListItem>
                                                        <asp:ListItem Text="07" Value="7"></asp:ListItem>
                                                        <asp:ListItem Text="08" Value="8"></asp:ListItem>
                                                        <asp:ListItem Text="09" Value="9"></asp:ListItem>
                                                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                        <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                                        <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="ddlgrdExpirationYear" runat="server" Visible="false">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="15%" />
                                            </asp:TemplateField>

                                            <asp:ButtonField CommandName="Edit" Text="Edit">
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:ButtonField>
                                            <asp:ButtonField CommandName="CancelRecord" Text="Cancel" Visible="false">
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:ButtonField>
                                            <asp:ButtonField CommandName="Delete" Text="Delete">
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <HeaderStyle CssClass="gridHeaderStyle" />
                                        <PagerSettings Position="TopAndBottom" />
                                        <PagerStyle HorizontalAlign="Left" CssClass="pagerStyle" />
                                        <RowStyle CssClass="rowStyle" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" DynamicLayout="False"
        AssociatedUpdatePanelID="UpdatePanel1">
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
