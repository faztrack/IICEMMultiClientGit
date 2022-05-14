<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="incentive_list.aspx.cs" Inherits="incentive_list" Title="Incentive List" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function SearchKeyPress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;

        }
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Incentive List</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="1200px">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton"
                                        Text="Previous" OnClick="btnPrevious_Click" />
                                </td>
                                <td align="left">
                                    <table>
                                        <tr>
                                            <td align="right">
                                                <b>Incentive Name:</b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                <cc1:autocompleteextender id="txtSearch_AutoCompleteExtender" runat="server" completioninterval="500" completionlistcssclass="AutoExtender" completionsetcount="10" delimitercharacters="" enablecaching="true" enabled="True" minimumprefixlength="1" onclientitemselected="selected_LastName" servicemethod="GetIncentiveName" targetcontrolid="txtSearch" usecontextkey="True">
                                                            </cc1:autocompleteextender>
                                                <cc1:textboxwatermarkextender id="wtmFileNumber" runat="server" targetcontrolid="txtSearch" watermarktext="Search by Incentive Name" />
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="button" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>                                  
                                </td>
                                <td align="center">
                                    <b>Page:</b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                    &nbsp;
                                                 <b>Item per page:</b>
                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="4">All Lead Status</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <asp:CheckBox ID="chkIsActive" runat="server" AutoPostBack="true" Text="Active on Payment Page" OnCheckedChanged="chkIsActive_click" />
                                    <asp:Button ID="btnAddNew" runat="server" Text="Add New Incentive"
                                        OnClick="btnAddNew_Click" CssClass="button" />
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton" Text="Next"
                                        OnClick="btnNext_Click" />
                                </td>
                            </tr>
                            <tr>
                                 <td colspan="5" align="center"><asp:Label ID="lblResult" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:GridView ID="grdIncentive" runat="server" AllowPaging="True"
                                        AutoGenerateColumns="False" PageSize="15" Width="100%"
                                        OnPageIndexChanging="grdIncentive_PageIndexChanging"
                                        OnRowDataBound="grdIncentive_RowDataBound" CssClass="mGrid">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <asp:HyperLinkField DataNavigateUrlFields="incentive_id" ItemStyle-Font-Underline="true"
                                                DataNavigateUrlFormatString="incentivedetails.aspx?iid={0}"
                                                DataTextField="incentive_name" HeaderText="Incentive Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:HyperLinkField>
                                            <asp:BoundField DataField="start_date" HeaderText="Start Date"
                                                DataFormatString="{0:d}" />
                                            <asp:BoundField DataField="end_date" HeaderText="End Date"
                                                DataFormatString="{0:d}" />
                                            <asp:BoundField DataField="is_active" HeaderText="Active">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <%--  <asp:BoundField DataField="discount" HeaderText="Discount (%)">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>--%>
                                            <asp:TemplateField HeaderText="Discount" SortExpression="MohavePrice">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDiscount" runat="server"></asp:Label>

                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="incentive_desc" HeaderText="Description">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                        </Columns>
                                        <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious0" runat="server" CssClass="prevButton"
                                        Text="Previous" OnClick="btnPrevious_Click" />
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server" CssClass="nextButton" Text="Next"
                                        OnClick="btnNext_Click" />
                                    <br />
                                    <asp:Label ID="Label1" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
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

