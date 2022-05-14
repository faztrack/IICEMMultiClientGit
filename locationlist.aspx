<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="locationlist.aspx.cs" Inherits="locationlist" Title="Location List" %>

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
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Location List</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="1200px">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton"
                                                    OnClick="btnPrevious_Click" Text="Previous" />
                                            </td>
                                            <td align="left">
                                                <table>
                                                    <tr>
                                                        <td align="right">
                                                            <b>Location Name:</b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                            <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLocationName" TargetControlID="txtSearch" UseContextKey="True">
                                                            </cc1:AutoCompleteExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Location Name" />
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
                                                    <asp:ListItem Value="4">All Locations</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>

                                            <td align="right">
                                                <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click"
                                                    Text="Add New Location" CssClass="button" />
                                            </td>

                                            <td align="right">
                                                <asp:Button ID="btnNext" runat="server" CssClass="nextButton"
                                                    OnClick="btnNext_Click" Text="Next" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="6" class="mGrid">

                                                <asp:GridView ID="grdLocationList" runat="server" AllowPaging="True"
                                                    AutoGenerateColumns="False"
                                                    OnPageIndexChanging="grdLocationList_PageIndexChanging"
                                                    OnRowDataBound="grdLocationList_RowDataBound" PageSize="20" Width="100%"
                                                    CssClass="mGrid">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataNavigateUrlFields="location_id" ItemStyle-Font-Underline="true"
                                                            DataNavigateUrlFormatString="locationdetails.aspx?lid={0}"
                                                            DataTextField="location_name" HeaderText="Location Name">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:HyperLinkField>
                                                        <asp:BoundField DataField="is_active" HeaderText="Active">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="loation_desc" HeaderText="Description">
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
                                                    OnClick="btnPrevious_Click" Text="Previous" />
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;
                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                            <td align="right">
                                                <asp:Button ID="btnNext0" runat="server" CssClass="nextButton"
                                                    OnClick="btnNext_Click" Text="Next" />

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

