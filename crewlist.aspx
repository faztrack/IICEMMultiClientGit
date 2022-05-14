<%@ Page Title="Crew List" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="crewlist.aspx.cs" Inherits="crewlist" %>
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
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Crew List</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="85%">
                            <tr>
                                <td>

                                    <table width="100%">
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Value="1" Selected="True">Last Name</asp:ListItem>
                                                    <asp:ListItem Value="2">First Name</asp:ListItem>
                                                    <asp:ListItem Value="3">User Name</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtSearch" runat="server" onkeypress="return SearchKeyPress(event);"></asp:TextBox>
                                                 <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="button" />
                                            </td>
                                             <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                            </td>
                                            
                                            <td align="left">
                                                <b>Page: </b>
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                &nbsp;
                                                <b>Item per page: </b>
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                   <%-- <asp:ListItem Value="4">All Users</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" valign="middle">
                                                <table>
                                                    <tr>
                                                        <td><b>Status:</b></td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True" Value="1">Active</asp:ListItem>
                                                                <asp:ListItem Value="0">InActive</asp:ListItem>
                                                                <asp:ListItem Value="All">All</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add New Crew" CssClass="button" />
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btnNext" runat="server" Text="Next"
                                                    OnClick="btnNext_Click" CssClass="nextButton" />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td align="center" colspan="7">
                                                <asp:GridView ID="grdCrewList" runat="server" AllowPaging="True" AllowSorting="true"
                                                    AutoGenerateColumns="False" DataKeyNames="user_id"
                                                    OnPageIndexChanging="grdCrewList_PageIndexChanging"
                                                    OnSorting="grdCrewList_Sorting" 
                                                    OnRowDataBound="grdCrewList_RowDataBound" Width="100%" CssClass="mGrid">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataNavigateUrlFields="crew_id"
                                                            DataNavigateUrlFormatString="crewdetails.aspx?crid={0}"
                                                            DataTextField="full_name" HeaderText="Crew Name" SortExpression="full_name" HeaderStyle-Font-Underline="true" ItemStyle-Font-Underline="true">
                                                            <HeaderStyle HorizontalAlign="Center" Width="15%" />
                                                            <ItemStyle HorizontalAlign="Left"  />
                                                        </asp:HyperLinkField>
                                                        <asp:BoundField DataField="username" HeaderText="Username" SortExpression="username" HeaderStyle-Font-Underline="true">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:BoundField>
                                                        <%-- <asp:BoundField HeaderText="Address">
                                                            <HeaderStyle HorizontalAlign="Left" Width="30%"/>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>--%>

                                                         <asp:BoundField DataField="phone" HeaderText="Phone">
                                                            <HeaderStyle HorizontalAlign="Center" Width="15%"/>
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <%--<asp:BoundField DataField="email" HeaderText="Email">
                                                            <HeaderStyle HorizontalAlign="Center" Width="15%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>--%>
                                                        <%-- <asp:BoundField DataField="hourly_rate" DataFormatString="{0:c}"  HeaderText="Labor Rate/Hr.">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%"/>
                                                            <ItemStyle HorizontalAlign="Right" />
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="is_active" HeaderText="Active">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
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
                                                <asp:Button ID="btnPrevious0" runat="server" Text="Previous" CssClass="prevButton" OnClick="btnPrevious_Click" />
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                              <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;
                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                            <td align="right">
                                                <asp:Button ID="btnNext0" runat="server" Text="Next" CssClass="nextButton" OnClick="btnNext_Click" />

                                            </td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSortedBy" runat="server" Visible="False"></asp:Label></td> 
                                 <td>

                                    <asp:HiddenField ID="hdnOrder" runat="server" Value="DESC" />

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
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

