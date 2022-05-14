<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="leadsource_list.aspx.cs" Inherits="leadsource_list" Title="Lead Source List" %>

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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Lead Source List</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="986px">
                            <tr>
                                <td>
                                    <table width="100%">                                        
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton"
                                                    Text="Previous" OnClick="btnPrevious_Click" />
                                            </td>
                                            <td align="left">
                                                <b>Lead Source Name:</b>
                                                <asp:TextBox ID="txtSearch" onkeypress="return SearchKeyPress(event);" runat="server"></asp:TextBox>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button" OnClick="btnSearch_Click" />
                                            </td>
                                            <td align="center">
                                                <b>Page:</b>
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#992a24"></asp:Label>
                                                &nbsp;
                                                 <b>Item per page:</b>                                            
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem Value="4">All Lead Source</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right">
                                                 <asp:Button ID="btnAddNew" runat="server"
                                                    Text="Add New Lead Source" CssClass="button" OnClick="btnAddNew_Click" />
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btnNext" runat="server" CssClass="nextButton" Text="Next"
                                                    OnClick="btnNext_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="5" class="mGrid">

                                                <asp:GridView ID="grdLeadSource" runat="server" AllowPaging="True"
                                                    AutoGenerateColumns="False" PageSize="20" Width="100%"
                                                    CssClass="mGrid" OnPageIndexChanging="grdLeadSource_PageIndexChanging"
                                                    OnRowDataBound="grdLeadSource_RowDataBound">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:HyperLinkField DataNavigateUrlFields="lead_source_id"
                                                            DataNavigateUrlFormatString="leadsourcedetails.aspx?lid={0}"
                                                            DataTextField="lead_name" HeaderText="Lead Source Name">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:HyperLinkField>
                                                        <asp:BoundField DataField="is_active" HeaderText="Active">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="description" HeaderText="Description">
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
                                            <td align="left">&nbsp; <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                            <td align="right">
                                                <asp:Button ID="btnNext0" runat="server" CssClass="nextButton" Text="Next"
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

