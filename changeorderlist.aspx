<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="changeorderlist.aspx.cs" Inherits="changeorderlist" Title="Change Order List" %>

<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">

        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Change Order List</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="900px" align="center">
                            <tr>
                                <td align="center">
                                    <table cellpadding="0" cellspacing="2" width="100%">
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
                                            <td align="left" valign="top">
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
                                            <td align="right"></td>
                                            <td align="left">
                                                <asp:HyperLink ID="hypGoogleMap" runat="server"
                                                    ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Email: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td align="right"></td>
                                            <td align="left"></td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Estimate Name: </b>
                                            </td>
                                            <td align="left">

                                                <asp:Label ID="lblEstimateName" runat="server" Font-Bold="True"></asp:Label>

                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                         <tr>
                                            <td align="right">
                                                
                                            </td>
                                            <td align="left">

                                                <asp:Label ID="lblResult" runat="server" ></asp:Label>

                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>

                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdChangeOrders" runat="server"
                                                    AutoGenerateColumns="False" Width="100%" PageSize="20" CssClass="mGrid"
                                                    OnRowDataBound="grdChangeOrders_RowDataBound"
                                                    DataKeyNames="chage_order_id">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="C/O Title">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypTitle" runat="server">[hypTitle]</asp:HyperLink>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="change_order_status_id" HeaderText="Status"></asp:BoundField>
                                                        <asp:BoundField DataField="change_order_type_id"
                                                            HeaderText="Type" />
                                                        <asp:BoundField DataField="changeorder_date" DataFormatString="{0:d}"
                                                            HeaderText="Change Order Date" />
                                                        <asp:BoundField DataField="last_updated_date"
                                                            HeaderText="Updated Date" DataFormatString="{0:d}" />
                                                        <asp:BoundField
                                                            HeaderText="Updated By" />
                                                        <asp:TemplateField HeaderText="View Change Orders">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkCO" runat="server" OnClick="GetReport">PDF</asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
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

                                    <asp:Button ID="btnCustomerDetails" runat="server" CssClass="button"
                                        OnClick="btnCustomerDetails_Click" Text="Customer Details" />
                                    <asp:Button ID="btnSoldEstimate" runat="server" CssClass="button"
                                        OnClick="btnSoldEstimate_Click" Text="Go to Sold Estimate" />
                                    <asp:Button ID="btnCustomerList" runat="server" CssClass="button"
                                        OnClick="btnCustomerList_Click" Text="Customer List" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False" Value="0" />
                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

