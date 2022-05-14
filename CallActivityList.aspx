<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CallActivityList.aspx.cs" Inherits="CallActivityList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_Company(sender, e) {
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
                                <td align="left"><span class="titleNu">Activity log</span>
                                </td>
                                <td align="right" style="padding-right: 30px;">
                                    <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn nostyle2" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td>&nbsp;</td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                        <asp:ListItem Value="1">First Name</asp:ListItem>
                                        <%--   <asp:ListItem Value="3">Company</asp:ListItem>--%>
                                        <asp:ListItem Value="4">Phone</asp:ListItem>
                                    </asp:DropDownList>

                                    <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_Company" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                    </cc1:AutoCompleteExtender>
                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />

                                    <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" />

                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                </td>

                                <td align="center" valign="middle">
                                    <b>Page: </b>

                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#0088cc"></asp:Label>
                                    &nbsp;
                                    <b>Lead Source:</b>
                                    <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="14" AutoPostBack="True" OnSelectedIndexChanged="ddlLeadSource_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>

                                <td align="right" valign="middle">
                                    <%-- <b>Sales Person:</b>
                                    <asp:DropDownList ID="ddlSalesRep" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSalesRep_SelectedIndexChanged">
                                    </asp:DropDownList>--%>
                                    <b>Action:</b>
                                    <asp:DropDownList ID="ddlCallType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCallType_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="7">All</asp:ListItem>
                                        <asp:ListItem Value="4">Followup</asp:ListItem>
                                        <asp:ListItem Value="1">Called</asp:ListItem>
                                        <asp:ListItem Value="2">Pitched</asp:ListItem>
                                        <asp:ListItem Value="6">Emailed</asp:ListItem>
                                        <asp:ListItem Value="3">Booked</asp:ListItem>
                                        <asp:ListItem Value="5">Do Not Call</asp:ListItem>
                                    </asp:DropDownList>

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
                        <asp:GridView ID="grdCustCallList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" OnPageIndexChanging="grdCustCallList_PageIndexChanging" OnRowDataBound="grdCustCallList_RowDataBound" Width="100%" PageSize="40" AllowSorting="True" OnSorting="grdCustCallList_Sorting">
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>
                                <%--  <asp:TemplateField HeaderText="Company" SortExpression="company" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_company" Text='<%# Eval("company").ToString() %>' runat="server" CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Customer Name" SortExpression="first_name1" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Cust" runat="server" CssClass="mGrida2" Font-Underline="true"></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Phone" SortExpression="phone" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="8%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <%-- <asp:BoundField DataField="company" HeaderText="Company" SortExpression="Company" HeaderStyle-Font-Underline="true">
                                    <HeaderStyle Width="10%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>--%>
                                <asp:TemplateField HeaderText="Notes">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallDescriptionG" runat="server" Text='<%# Eval("Description").ToString() %>' Style="display: inline;"></asp:Label>
                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblCallDescriptionG_r" runat="server" Text='<%# Eval("Description") %>' Visible="false" ></asp:Label></pre>
                                        <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action" SortExpression="CallTypeId" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallType" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="13%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Followup" SortExpression="FollowDateTime" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFollowup" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date Last Called" SortExpression="CallDateTime" HeaderStyle-Font-Underline="true">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCallStartDateTime" Text='<%# Eval("CallDateTime").ToString() %>' runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="lead_name" HeaderText="Lead Source" SortExpression="lead_name" HeaderStyle-Font-Underline="true">
                                    <HeaderStyle Width="10%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
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
                                     <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
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
                        <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
        </Triggers>
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
