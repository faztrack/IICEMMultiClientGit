<%@ Page Title="Pending Payments" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="pendingpayment.aspx.cs" Inherits="pendingpayment" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        input[type=radio] + label, input[type=checkbox] + label {
            display: inline-block;
            margin: -6px 0 0 0;
            padding: 4px 5px;
            margin-bottom: 0;
            font-size: 12px;
            line-height: 20px;
            text-align: center;
            text-shadow: 0 1px 1px rgba(255,255,255,0.75);
            vertical-align: middle;
            cursor: pointer;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function selected_LastName(sender, e) {
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
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Pending Payment List</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>

                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <table cellpadding="5" cellspacing="5" >
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Label ID="lblLastName" runat="server" Font-Bold="true" ForeColor="#000000" Text="Search by Last Name:"></asp:Label>

                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                            <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="20" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                            </cc1:AutoCompleteExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                                        </td>
                                                        <td>&nbsp;&nbsp;<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" OnClick="btnSearch_Click" /></td>
                                                        <td align="left">
                                                            <asp:Label ID="lblPastDue" runat="server" Font-Bold="true" ForeColor="#000000" Text="Past Due:"></asp:Label>

                                                        </td>
                                                        <td align="left">

                                                            <asp:RadioButtonList ID="rabDays" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rabDays_SelectedIndexChanged" Style="margin-top: 5px" Visible="true">
                                                                <asp:ListItem Value="30" Selected="True">30 days</asp:ListItem>
                                                                <asp:ListItem Value="60">60 days</asp:ListItem>
                                                                <asp:ListItem Value="90">90 days</asp:ListItem>
                                                                <asp:ListItem Value="4">90 days+</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        
                                                        <td>&nbsp;&nbsp;</td>
                                                        <td align="right">
                                                            <strong>Status:&nbsp;</strong>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddlStatus" runat="server" TabIndex="20" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="1">All</asp:ListItem>
                                                                <asp:ListItem Value="2" Selected="True">Active</asp:ListItem>
                                                                <asp:ListItem Value="4">Archive</asp:ListItem>
                                                                <asp:ListItem Value="5">InActive</asp:ListItem>
                                                                <asp:ListItem Value="7">Warranty Only</asp:ListItem>

                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>&nbsp;&nbsp;</td>
                                                        <td align="right">
                                                            <strong>Estimate Status:&nbsp;</strong>
                                                        </td>
                                                        <td align="left" valign="middle">
                                                            <asp:DropDownList ID="ddlEstimateStatus" runat="server" TabIndex="20" OnSelectedIndexChanged="ddlEstimateStatus_SelectedIndexChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="All">All</asp:ListItem>
                                                                <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                                                                <asp:ListItem Value="0">InActive</asp:ListItem>

                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>&nbsp;&nbsp;<asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Reset</asp:LinkButton></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="12">
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="13">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                                        </td>
                                                        <td style="width:536px;" align="left">&nbsp;
                                                  <asp:RadioButtonList ID="rabScheduleType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rabScheduleType_SelectedIndexChanged">
                                                      <asp:ListItem Value="1" Selected="True">Scheduled Calendar Payment Terms</asp:ListItem>
                                                      <asp:ListItem Value="2">UnScheduled Calendar Payment Terms</asp:ListItem>
                                                  </asp:RadioButtonList>

                                                            <td align="left">

                                                                <b>Page:</b>
                                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                                <b>Item per page: </b>
                                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                                    <asp:ListItem Selected="True">30</asp:ListItem>
                                                                    <asp:ListItem>50</asp:ListItem>
                                                                    <asp:ListItem>60</asp:ListItem>
                                                                    <asp:ListItem>70</asp:ListItem>
                                                                    <asp:ListItem>80</asp:ListItem>
                                                                    <asp:ListItem>90</asp:ListItem>
                                                                    <asp:ListItem>100</asp:ListItem>
                                                                </asp:DropDownList>

                                                            </td>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNext" runat="server" Text="Next"
                                                                OnClick="btnNext_Click" CssClass="nextButton" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="center" colspan="12">

                                                <asp:GridView ID="grdDueCustomers" runat="server" AllowPaging="True" AllowSorting="false"
                                                    AutoGenerateColumns="False" DataKeyNames="user_id"
                                                    OnPageIndexChanging="grdDueCustomers_PageIndexChanging"
                                                    OnRowDataBound="grdDueCustomers_RowDataBound" Width="100%" CssClass="mGrid">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:BoundField DataField="sale_date" HeaderText="Sale Date" DataFormatString="{0:d}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="job_number" HeaderText="Job#">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="customername" HeaderText="customer Name">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:BoundField>
                                                        
                                                        <asp:BoundField DataField="estimatename" HeaderText="project name">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Payment Term">
                                                            <ItemTemplate>

                                                                <asp:HyperLink ID="hyp_section" runat="server" Text='<%# Eval("section_name") %>' Style="text-decoration: underline"></asp:HyperLink>

                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="12%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="DueAmount" HeaderText="Amount" DataFormatString="{0:c}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Right" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="eventDate" HeaderText="Due Date" DataFormatString="{0:d}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Past Due">
                                                            <ItemTemplate>

                                                                <asp:Label ID="lblPastDueTwo" runat="server"></asp:Label>

                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Status">
                                                            <ItemTemplate>

                                                                <asp:Label ID="lblStatus" runat="server"></asp:Label>

                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>

                                                                <asp:LinkButton ID="lnkReminderEmail" runat="server" OnClick="btnPendingEmail_Click"><img src="images/system_icons/01_icon.png" height="37" style="cursor:pointer;" alt="Send Email" title="Send Email" /></asp:LinkButton>

                                                                <asp:LinkButton ID="lnkSendSMS" runat="server" OnClick="lnkSendSMS_Click" ><img src="images/system_icons/17_icon.png" height="37" style="cursor:pointer;" alt="Send Text" title="Send Text" /></asp:LinkButton>

                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>



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
                                <td>
                                    <asp:Label ID="lblSortedBy" runat="server" Visible="False"></asp:Label></td>
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

