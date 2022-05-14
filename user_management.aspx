<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="user_management.aspx.cs" Inherits="user_management" Title="User Management" %>

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
                                <td align="left"><span class="titleNu">User Management</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
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
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                            <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" 
                                                                CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" 
                                                                ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
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
                                                        <td>&nbsp;&nbsp;<b>Count:</b></td>
                                                        <td><asp:Label runat="server" ID="lblCount"></asp:Label></td>
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
                                                <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add New User" CssClass="button" />
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btnNext" runat="server" Text="Next"
                                                    OnClick="btnNext_Click" CssClass="nextButton" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="6">
                                                <asp:GridView ID="grdUserList" runat="server" AllowPaging="True"
                                                    AutoGenerateColumns="False" DataKeyNames="user_id"
                                                    OnPageIndexChanging="grdUserList_PageIndexChanging"
                                                    OnRowDataBound="grdUserList_RowDataBound" Width="100%" CssClass="mGrid">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataNavigateUrlFields="user_id" ItemStyle-Font-Underline="true"
                                                            DataNavigateUrlFormatString="user_details.aspx?uid={0}"
                                                            DataTextField="last_name" HeaderText="Last Name">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="8%" />
                                                        </asp:HyperLinkField>
                                                        <asp:BoundField DataField="first_name" HeaderText="First Name">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="8%" />
                                                        </asp:BoundField>
                                                        <%-- <asp:BoundField HeaderText="Address">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="18%"/>
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="email" HeaderText="Email">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" Width="14%"/>
                                                        </asp:BoundField>--%>
                                                        <%-- <asp:BoundField DataField="company_email" HeaderText="Outlook / Exchange Email">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="EmailIntegration" HeaderText="Outlook / Exchange">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="6%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="phone" HeaderText="Phone">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="last_login_time" HeaderText="Last Login">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="role_id" HeaderText="Role">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                        </asp:BoundField>
                                                        <asp:BoundField HeaderText="Active">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" Width="6%" />
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
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="right">
                                                <asp:Button ID="btnNext0" runat="server"
                                                    Text="Next" CssClass="nextButton"
                                                    OnClick="btnNext_Click" />
                                            </td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
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

