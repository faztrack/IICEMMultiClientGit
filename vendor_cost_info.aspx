<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="vendor_cost_info.aspx.cs" Inherits="vendor_cost_info" Title="Vendor Cost Details" %>

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
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Vendor Cost Details</span></td>
                                <td align="right"></td>
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
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                            <tr>
                                <td align="right" width="30%">
                                    <b>Contract Amount:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblProjectTotal" runat="server" Width="84px" Text="0"></asp:Label>
                                </td>
                                <td align="right">
                                    <b>Profit/Loss:</b>&nbsp;</td>
                                <td align="left">
                                    <asp:Label ID="lblProfit" runat="server" Width="84px" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Contract Received Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblContractRecive" runat="server" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>C/O&nbsp; Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalCOAmount" runat="server" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>C/O Received Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblCORecive" runat="server" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>&nbsp;Total Received Amount:</b></td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalRecive" runat="server" Text="0" Width="84px"></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Contract Commission:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:GridView ID="grdCom" runat="server" AutoGenerateColumns="False"
                                        CssClass="bGrid" OnRowDataBound="grdCom_RowDataBound"
                                        OnRowEditing="grdCom_RowEditing" OnRowUpdating="grdCom_RowUpdating"
                                        ShowHeader="False" TabIndex="2" Width="33%" BorderStyle="None">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' />
                                                    <asp:TextBox ID="txtComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' Visible="false" Width="50px"></asp:TextBox>
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
                                    <asp:GridView ID="grdCom_CO" runat="server" AutoGenerateColumns="False"
                                        BorderStyle="None" CssClass="bGrid" OnRowDataBound="grdCom_CO_RowDataBound"
                                        OnRowEditing="grdCom_CO_RowEditing" OnRowUpdating="grdCom_CO_RowUpdating"
                                        ShowHeader="False" TabIndex="2" Width="33%">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' />
                                                    <asp:TextBox ID="txtComAmount" runat="server"
                                                        Text='<%# Eval("comission_amount","{0:c}") %>' Visible="false" Width="50px"></asp:TextBox>
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
                                    <asp:Button ID="btnGoToPayment" runat="server" CssClass="greenbutton"
                                        OnClick="btnGoToPayment_Click" Text="Go to Payment" />
                                </td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Total Commission:</b>&nbsp;</td>
                                <td align="left">
                                    <asp:Label ID="lblTotalCom" runat="server" Width="84px">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                            <tr>
                                <td align="right" width="30%">
                                    <b>Labor Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblLabor" runat="server" Width="84px">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Material Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblMaterial" runat="server" Width="84px">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="30%">
                                    <b>Total Cost:</b>&nbsp;</td>
                                <td align="left" width="20%">
                                    <asp:Label ID="lblTotalCost" runat="server" Width="84px">0</asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px">
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
                            <asp:GridView ID="grdVendorCost" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdVendorCost_RowDataBound"
                                OnRowEditing="grdVendorCost_RowEditing"
                                OnRowUpdating="grdVendorCost_RowUpdating"
                                OnRowCommand="grdVendorCost_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("cost_date","{0:d}")%>' />
                                            <div id="dvCalender" runat="server" visible="false">
                                                <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("cost_date","{0:d}") %>'
                                                    class="Calender" Width="60px"></asp:TextBox>
                                                <img src="images/calendar.gif" />
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                        <ItemStyle HorizontalAlign="Center" Width="115px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vendor">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlVendor" runat="server" Enabled="false" DataValueField="vendor_id" DataTextField="vendor_name" DataSource="<%#dtVendor %>" SelectedValue='<%# Eval("vendor_id") %>'>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Trade">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlTrade" runat="server" Enabled="false" DataValueField="section_id" DataTextField="section_name" DataSource="<%#dtSection %>" SelectedValue='<%#Eval("section_id") %>'>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("cost_description") %>' />
                                            <asp:TextBox ID="txtDescription" runat="server" Visible="false"
                                                Text='<%# Eval("cost_description") %>' TextMode="MultiLine" Width="120px" Height="40px"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Left" Width="122px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Category">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlCategory" Enabled="false" runat="server" SelectedValue='<%# Eval("category_id") %>'>
                                                <asp:ListItem Value="1">Material</asp:ListItem>
                                                <asp:ListItem Value="2">Labor</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("cost_amount","{0:c}") %>' />
                                            <asp:TextBox ID="txtAmount" runat="server" Visible="false"
                                                Text='<%# Eval("cost_amount","{0:c}") %>' Width="50px"></asp:TextBox>
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
                        <td align="center">
                            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click" Text="Add New Row" />
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


</asp:Content>

