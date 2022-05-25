<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AllowanceReport.aspx.cs" Inherits="AllowanceReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
        <script>
            function DisplayWindow(cid) {
                window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
            }
        </script>
    <table cellpadding="0" cellspacing="2" width="100%">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Allowance Report</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>
                        <td align="right" style="padding-right: 30px; float: right;">
                            <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="4px" cellspacing="4px" width="100%" align="center">
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
                                                <td style="width: 120px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td  align="left" valign="top"><b>Job Number: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblJobNumber" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td  align="left" valign="top"><b>Phone: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td  align="left" valign="top"><b>Email: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td  align="left" valign="top"><b>Estimate Name:</b> </td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblEstimateName" runat="server"></asp:Label>
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
                                                <td style="width: auto;" align="left" valign="top">
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server" Target="_blank"
                                                        ImageUrl="~/images/img_map.gif"></asp:HyperLink>
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
                            <script language="Javascript" type="text/javascript">
                                function ChangeImage(id) {
                                    document.getElementById(id).src = 'Images/loading.gif';
                                }
                            </script>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <table cellpadding="4" cellspacing="8" width="100%" align="center" class="wrapper">
                                        <tr>
                                            <td>
                                                <table width="100%" align="center">
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <h2>Selected Allowance Items</h2>
                                                        </td>
                                                        <td align="right" valign="top">
                                                            <asp:ImageButton ID="btnExpList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn" OnClick="btnExpList_Click" ToolTip="Export List to CSV" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:GridView ID="grdAllowance" runat="server"
                                                    AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowDataBound="grdAllowance_RowDataBound" ShowFooter="True">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
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
                                                            <FooterTemplate>
                                                                <asp:Label ID="lblItemTotal" runat="server" Text='Total:' />
                                                            </FooterTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="5%" />
                                                            <FooterStyle HorizontalAlign="Right" Font-Bold="true" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Cost">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("total_retail_price","{0:c}") %>' />
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <%# GetTotalPrice()%>
                                                            </FooterTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Right" Width="5%" />
                                                            <FooterStyle HorizontalAlign="Right" Font-Bold="true" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Actual Cost">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblActualAmount" runat="server" Text='<%# Eval("actual_price","{0:c}") %>' />
                                                                <asp:TextBox ID="txtActualCost" Visible="false" Style="text-align: right; width: 62px" runat="server" Text='<%# Eval("actual_price","{0:c}") %>' AutoPostBack="true" OnTextChanged="txtActualCost_TextChanged"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <%# GetActualTotalPrice()%>
                                                            </FooterTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <FooterStyle HorizontalAlign="Right" Font-Bold="true" />
                                                            <ItemStyle HorizontalAlign="Right" Width="5%" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Difference">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblPriceDifference" runat="server" Text='<%# Eval("price_difference","{0:c}") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <%# GetDiffTotalPrice()%>
                                                            </FooterTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Right" Width="5%" />
                                                            <FooterStyle HorizontalAlign="Right" Font-Bold="true" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnCancel" runat="server" TabIndex="19" Text="Close"
                                                    CausesValidation="False" CssClass="button" OnClick="btnCancel_Click"
                                                    Width="80px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnAllowance" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False" Value="0" />
                                                <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                            </td>
                                        </tr>
                                    </table>

                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnExpList" />
                                </Triggers>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

