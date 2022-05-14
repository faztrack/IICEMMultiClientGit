<%@ Page Title="Project Notes" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ProjectNotesReport.aspx.cs" Inherits="ProjectNotesReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>
    <script type="text/javascript">
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
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" style="background-color: #ddd; color: #fff;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Project Notes Report</span>
                                </td>
                                <td align="right" style="padding-right: 30px;">
                                    <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
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
                                                            <asp:LinkButton ID="lnkViewAll" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton></td>
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
                                                                <asp:ListItem Selected="True">20</asp:ListItem>
                                                                <asp:ListItem>30</asp:ListItem>
                                                                <asp:ListItem>40</asp:ListItem>
                                                                <asp:ListItem>50</asp:ListItem>
                                                                <asp:ListItem Value="4">View All</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="right" colspan="4" valign="middle">
                                                <table style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="right"><b>Date</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtProjectStartDate" runat="server" AutoPostBack="true" CssClass="textBox" Width="70px" TabIndex="1" OnTextChanged="txtProjectStartDate_TextChanged"></asp:TextBox>
                                                            <cc1:CalendarExtender ID="txtAppStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgProjectStartDate" PopupPosition="BottomLeft" TargetControlID="txtProjectStartDate">
                                                            </cc1:CalendarExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtProjectStartDate" WatermarkText="Start Date" />
                                                        </td>
                                                        <td align="left">
                                                            <asp:ImageButton ID="imgProjectStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtProjectEndDate" runat="server" CssClass="textBox" AutoPostBack="true" Width="70px" TabIndex="2" OnTextChanged="txtProjectEndDate_TextChanged"></asp:TextBox>
                                                            <cc1:CalendarExtender ID="txtProjectEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgProjectEndDate" PopupPosition="BottomLeft" TargetControlID="txtProjectEndDate">
                                                            </cc1:CalendarExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtProjectEndDate" WatermarkText="End Date" />
                                                        </td>
                                                        <td align="left">
                                                            <asp:ImageButton ID="imgProjectEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
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
                                                                <asp:ListItem Value="3">All</asp:ListItem>

                                                                <asp:ListItem Value="1">Complete</asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="0">Incomplete</asp:ListItem>

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
                                    <asp:GridView ID="grdProjectNote" runat="server" AutoGenerateColumns="False" AllowPaging="true" CssClass="mGrid" OnRowDataBound="grdProjectNote_RowDataBound"
                                        TabIndex="2" Width="100%" AllowSorting="True" OnSorting="grdProjectNote_Sorting" OnPageIndexChanging="grdProjectNote_PageIndexChanging">
                                        <Columns>
                                           <%-- <asp:HyperLinkField DataNavigateUrlFields="customer_id" DataNavigateUrlFormatString="ProjectNotes.aspx?cid={0}&TypeId=4" ControlStyle-CssClass="mGrida2" HeaderStyle-Font-Underline="true"
                                                DataTextField="customer_name" HeaderText="Customer Name" ItemStyle-Width="0%" SortExpression="last_name1">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="11%" />
                                            </asp:HyperLinkField>--%>
                                             <asp:TemplateField HeaderText="Customer Name" SortExpression="last_name1" ControlStyle-CssClass="mGrida2" HeaderStyle-Font-Underline="true" >
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypCustomerName" runat="server"></asp:HyperLink>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="11%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date" SortExpression="ProjectDate" HeaderStyle-Font-Underline="true">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("ProjectDate","{0:d}")%>' />
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="7%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Section">
                                                <ItemTemplate>

                                                    <asp:Label ID="lblSection" runat="server" Text='<%# Eval("SectionName").ToString() %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Material Track">
                                                <ItemTemplate>

                                                    <asp:Label ID="lblMaterialTrack" runat="server" Text='<%# Eval("MaterialTrack").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblMaterialTrack_r" runat="server" Text='<%# Eval("MaterialTrack") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenMaterialTrack" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenMaterialTrack_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText=" Design Updates">
                                                <ItemTemplate>

                                                    <asp:Label ID="lblDesignUpdates" runat="server" Text='<%# Eval("DesignUpdates").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDesignUpdates_r" runat="server" Text='<%# Eval("DesignUpdates") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenDesignUpdates" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenDesignUpdates_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Superintendent Notes">
                                                <ItemTemplate>

                                                    <asp:Label ID="lblSuperintendentNotes" runat="server" Text='<%# Eval("SuperintendentNotes").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblSuperintendentNotes_r" runat="server" Text='<%# Eval("SuperintendentNotes") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenSuperintendentNotes" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenSuperintendentNotes_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="General Notes">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("NoteDetails").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDescription_r" runat="server" Text='<%# Eval("NoteDetails") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="15%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Completed" SortExpression="is_complete" HeaderStyle-Font-Underline="true">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIsComplete" runat="server" Text='<%# Eval("is_complete") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="7%" />
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Include In SOW">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSOW" runat="server" Text='<%# Eval("isSOW") %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" Width="7%" />
                                            </asp:TemplateField>

                                        </Columns>
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
                                <td align="center">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center" height="10px">
                                    <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1"
        DynamicLayout="False">
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


