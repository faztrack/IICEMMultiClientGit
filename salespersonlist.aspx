<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="salespersonlist.aspx.cs" Inherits="salespersonlist" Title="Sales Person List" %>

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
    </script>
    <script language="javascript" type="text/javascript">
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
                                <td align="left"><span class="titleNu">Sales Persons List</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="1086px">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" Text="Previous"
                                        OnClick="btnPrevious_Click" CssClass="prevButton" />
                                </td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">First Name</asp:ListItem>
                                                    <asp:ListItem Value="2" Selected="True">Last Name</asp:ListItem>
                                                    <asp:ListItem Value="3">Email</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnSearch" runat="server" Text="Search"
                                                    OnClick="btnSearch_Click" CssClass="button" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td><b>Status:</b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="1">Active</asp:ListItem>
                                                    <asp:ListItem Value="2">InActive</asp:ListItem>
                                                    <asp:ListItem Value="3">All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center">
                                    <b>Page: </b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                    &nbsp;
                                    <b>Item per page: </b>
                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="4">All Users</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" Text="Next"
                                        OnClick="btnNext_Click" CssClass="nextButton" />
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnAddNew" runat="server" Text="Add New Sales Person"
                                        OnClick="btnAddNew_Click" CssClass="button" Visible="False" />
                                </td>

                            </tr>
                            <tr>
                                <td colspan="5">
                                    <asp:GridView ID="grdSalesPersonList" runat="server" AutoGenerateColumns="False"
                                        PageSize="15" Width="100%" AllowPaging="True"
                                        OnPageIndexChanging="grdSalesPersonList_PageIndexChanging"
                                        OnRowDataBound="grdSalesPersonList_RowDataBound" CssClass="mGrid">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <asp:HyperLinkField DataNavigateUrlFields="sales_person_id" ItemStyle-Font-Underline="true"
                                                DataNavigateUrlFormatString="salesperson.aspx?sid={0}"
                                                DataTextField="last_name" HeaderText="Last Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:HyperLinkField>
                                            <asp:BoundField DataField="first_name" HeaderText="First Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="email" HeaderText="Email">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="address" HeaderText="Address">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="is_active" HeaderText="Active">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="last_login_time" HeaderText="Last Login">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>

                                        </Columns>
                                        <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>

                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious0" runat="server"
                                        Text="Previous" CssClass="prevButton"
                                        OnClick="btnPrevious_Click" />
                                </td>

                                <td align="left" colspan="3">&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server"
                                        Text="Next" CssClass="nextButton"
                                        OnClick="btnNext_Click" />
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


