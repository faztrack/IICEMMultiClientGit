<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="salesperson.aspx.cs" Inherits="salesperson" Title="Sales Person Information" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table id="Table5" align="center" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Sales Person Details</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="Table2" width="100%" align="center" border="0" cellpadding="0" cellspacing="0" onclick="return TABLE1_onclick()">
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
                                                            <h2>Sales Person Information</h2>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td style="width: 390px;" align="left" valign="top">
                                                <table style="width: 390px;">
                                                    <tr>
                                                        <td style="width: 200px;" align="left" valign="top"><b>Sales Person Name: </b></td>
                                                        <td style="width: auto;">
                                                            <asp:Label ID="lblSalesPersonName" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 200px;" align="left" valign="top"><b>Phone: </b></td>
                                                        <td style="width: auto;">
                                                            <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 200px;" align="left" valign="top"><b>Email: </b></td>
                                                        <td style="width: auto;">
                                                            <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left" valign="top">
                                                <table style="width: 420px;">
                                                    <tr>
                                                        <td style="width: 64px;" align="left" valign="top"><b>Address: </b></td>
                                                        <td style="width: auto;" align="left" valign="top">
                                                            <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                                        <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <tr>
                                        <td class="cssHeader" align="center" style="width:100%;">
                                            <table width="100%" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td align="left">
                                                        <span class="titleNu">
                                                            <span id="head_lblHeaderTitle" class="cssTitleHeader">Estimate Information</span></span>
                                                    </td>
                                                    <td align="right"></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="wrapper" colspan="3" align="center">
                                            <asp:GridView ID="grdCustomerList" runat="server"
                                                AutoGenerateColumns="False" DataKeyNames="customer_id"
                                                OnRowDataBound="grdCustomerList_RowDataBound" Width="100%"
                                                CssClass="mGrid">
                                                <PagerSettings Position="TopAndBottom" />
                                                <Columns>
                                                    <asp:HyperLinkField DataNavigateUrlFields="customer_id"
                                                        DataNavigateUrlFormatString="customer_details.aspx?cid={0}"
                                                        DataTextField="last_name1" HeaderText="Last Name">
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:HyperLinkField>
                                                    <asp:BoundField DataField="first_name1" HeaderText="First Name">
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Address">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Email">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hypEmail" runat="server" Target="_blank">[hypEmail]</asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="phone" HeaderText="Phone">
                                                        <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="registration_date" DataFormatString="{0:d}"
                                                        HeaderText="Entry Date">
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="status_id" HeaderText="Status" />
                                                    <asp:TemplateField HeaderText="Projects">
                                                        <ItemTemplate>
                                                            <asp:Table ID="tdLink" runat="server">
                                                            </asp:Table>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Change Orders">
                                                        <ItemTemplate>
                                                            <asp:Table ID="tblChangeOrder" runat="server">
                                                            </asp:Table>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                                <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                                <AlternatingRowStyle CssClass="alt" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                        </td>
                                        <td>&nbsp; </td>
                                        <td align="right">
                                    </tr>
                                </td>
                            </tr>
                        </table>
                    </td>
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

