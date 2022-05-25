<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="graphics_snapshot.aspx.cs" Inherits="graphics_snapshot" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

   <script language="Javascript" type="text/javascript">
       function ChangeImage(id) {
           document.getElementById(id).src = 'Images/loading.gif';
       }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_LastName(sender, e) {
            // alert("msg");
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>
    <script language="javascript" type="text/javascript">
        function searchKeyPress(e) {
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
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" style="background-color: #ddd; color: #fff;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Snapshot</span>
                                </td>
                                <td align="right" style="padding-right: 30px;"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                  <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left" valign="middle">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td>&nbsp;</td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">First Name</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton></td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="center" valign="middle">
                                    <table>
                                        <tr>
                                            <td><b>Page: </b></td>
                                            <td>
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#992a24"></asp:Label></td>
                                            <td><b>Item Per Page: </b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem Value="4">View All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="right" valign="middle">
                                    <table>
                                        <tr>
                                            <td><b>Status:</b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">All</asp:ListItem>
                                                    <asp:ListItem  Selected="True" Value="2">Active</asp:ListItem>
                                                    <asp:ListItem Value="6">Sold</asp:ListItem>
                                                    <asp:ListItem Value="4">Archive</asp:ListItem>
                                                    <asp:ListItem Value="5">InActive</asp:ListItem>
                                                    <asp:ListItem Value="8">Est.InActive</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton" OnClick="btnNext_Click"
                                        Text="Next" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdGraphicsSnap" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" OnRowDataBound="grdGraphicsSnap_RowDataBound" OnPageIndexChanging="grdGraphicsSnap_PageIndexChanging">
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>
                                <asp:TemplateField HeaderText="Design">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Design" runat="server" Text='<%# Eval("Design") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblDesign" runat="server" CssClass="paraMar" Text='<%# Eval("DesignEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Selection">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Selection" runat="server" Text='<%# Eval("Selection") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblSelection" runat="server" CssClass="paraMar" Text='<%# Eval("SelectionEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Site Reivew">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_SiteProgress" runat="server" Text='<%# Eval("SiteProgress") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblSiteProgress" runat="server" CssClass="paraMar" Text='<%# Eval("SiteProgressEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Schedule">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Schedule" runat="server" Text='<%# Eval("Schedule") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblSchedule" runat="server" CssClass="paraMar" Text='<%# Eval("ScheduleEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Final Project Review">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_FinalProject" runat="server" Text='<%# Eval("FinalProject") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblFinalProject" runat="server" CssClass="paraMar" Text='<%# Eval("FinalProjectEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Completion Certificate">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Completion" runat="server" Text='<%# Eval("Completion") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblCompletion" runat="server" CssClass="paraMar" Text='<%# Eval("CompletionEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Warranty">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Warranty" runat="server" Text='<%# Eval("Warranty") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblWarranty" runat="server" CssClass="paraMar" Text='<%# Eval("WarrantyEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Completed">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Completed" runat="server" Text='<%# Eval("Completed") %>' CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblCompleted" runat="server" CssClass="paraMar" Text='<%# Eval("CompletedEst") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                            </Columns>
                            <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                            <AlternatingRowStyle CssClass="alt" />
                        </asp:GridView>
                    </td>
                </tr>
                 <tr>
                    <td>
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left" style="width: 324px">
                                    <asp:Button ID="btnPrevious0" runat="server" OnClick="btnPrevious_Click" Text="Previous"
                                        CssClass="prevButton" />
                                </td>
                                <td align="right" style="width: 87px">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                               
                                </td>
                                <td align="left" style="width: 245px">&nbsp;
                                     
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server" OnClick="btnNext_Click" Text="Next"
                                        CssClass="nextButton" />


                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
</asp:Content>
