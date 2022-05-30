<%@ Page Title="Verndor List" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="VendorList.aspx.cs" Inherits="VendorList" %>

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
                                <td align="left"><span class="titleNu">Vendor List</span></td>
                                <td align="right">
                                    <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click"
                                        Text="Add New Vendor" CssClass="button" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="80%">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td align="left">
                                    <table>
                                        <tr>
                                            <td align="right">
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">Phone</asp:ListItem>
                                                    <asp:ListItem Value="2" Selected="True">Vendor Name</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetVendorName" TargetControlID="txtSearch" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Vendor Name" />
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnSearch" runat="server" Text="Search"
                                                    OnClick="btnSearch_Click" CssClass="button" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton></td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left">
                                    <table style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="right">
                                                <b>Page: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true"
                                                    ForeColor="#992a24"></asp:Label>
                                            </td>
                                            <%-- <td align="right">
                                                <b>Item per page: </b></td>
                                           <td align="left">
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem Value="4">All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>--%>
                                        </tr>
                                    </table>
                                </td>



                                

                                <td align="right">
                                    <table style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>
                                                <b>Division:</b>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlDivision" runat="server" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </td>
                                            <td align="right">
                                                <b>Status: </b></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">Active</asp:ListItem>
                                                    <asp:ListItem Value="2">InActive</asp:ListItem>
                                                    <asp:ListItem Value="0">All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="right">
                                                <b>Section: </b>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSection" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSection_SelectedIndexChanged" Width="98%">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton" OnClick="btnNext_Click"
                                        Text="Next" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="5">
                                    <asp:GridView ID="grdVendorList" runat="server" AllowPaging="True"
                                        AutoGenerateColumns="False" DataKeyNames="user_id"
                                        OnPageIndexChanging="grdVendorList_PageIndexChanging"
                                        OnRowDataBound="grdVendorList_RowDataBound" Width="100%" CssClass="mGrid">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <%-- Cell 0 --%>
                                            <asp:HyperLinkField DataNavigateUrlFields="vendor_id" ItemStyle-Font-Underline="true"
                                                DataNavigateUrlFormatString="VendorDetails.aspx?vid={0}"
                                                DataTextField="vendor_name" HeaderText="Vendor Name">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" Width="25%" />
                                            </asp:HyperLinkField>

                                            <%-- Cell 1 --%>
                                            <asp:BoundField HeaderText="Address">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" Width="25%" />
                                            </asp:BoundField>
                                            <%--  <asp:BoundField DataField="city" HeaderText="City">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                         <asp:BoundField DataField="state" HeaderText="State">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                         <asp:BoundField DataField="zip_code" HeaderText="Zip">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>--%>
                                            <%-- <asp:BoundField DataField="email" HeaderText="Email">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>--%>


                                            <%-- Cell 2 --%>
                                            <asp:TemplateField HeaderText="Division">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDivision" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            </asp:TemplateField>

                                            <%-- Cell 3 --%>
                                            <asp:BoundField DataField="phone" HeaderText="Phone">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            </asp:BoundField>

                                            <%-- Cell 4 --%>
                                            <asp:BoundField DataField="fax" HeaderText="Fax">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            </asp:BoundField>

                                            <%-- Cell 5 --%>
                                            <asp:BoundField DataField="section" HeaderText="section" ItemStyle-CssClass="VendSectionCss">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="25%" />
                                            </asp:BoundField>

                                            <%-- Cell 6 --%>
                                            <asp:BoundField DataField="is_active" HeaderText="Active">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
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
                                <td align="left">&nbsp;  
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server"
                                        Text="Next" CssClass="nextButton"
                                        OnClick="btnNext_Click" />


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
