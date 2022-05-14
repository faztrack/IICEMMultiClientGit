<%@ Page Title="PM Confirm Tomorrow" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SuperintendentConfirm.aspx.cs" Inherits="SuperintendentConfirm" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">


    <script type="text/javascript">
        $(document).ready(function () {
            $(<%=lstEmployee.ClientID%>).SumoSelect({
               selectAll: true,
               search: true,
               searchText: 'Search...',
               // triggerChangeCombined: true,
               //locale: ['OK', 'Cancel', 'Select All'],
               placeholder: 'Select Here',
               noMatch: 'No matches for "{0}"',
               //okCancelInMulti: true,
           });
       });

       Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
       function EndRequestHandler(sender, args) {
           //Binding Code Again
           $(<%=lstEmployee.ClientID%>).SumoSelect({
               selectAll: true,
               search: true,
               searchText: 'Search...',
               //triggerChangeCombined: true,
               //locale: ['OK', 'Cancel', 'Select All'],
               placeholder: 'Select Here',
               noMatch: 'No matches for "{0}"',
               //okCancelInMulti: true,
           });
       }
    </script>

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
                    <td align="center" style="background-color: #eee; color: #fff; box-shadow: 0 0 3px #aaa;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left" height="32px"><span class="titleNu">PM Confirm Tomorrow List</span>
                                </td>
                                <td align="right" style="padding-right: 30px;">
                                    <%--<asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn nostyle2" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />--%>
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
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                        <asp:ListItem Value="1">First Name</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_Company" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                    </cc1:AutoCompleteExtender>
                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />




                                </td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td align="left" valign="middle"><strong>Superintendent:</strong>&nbsp;</td>
                                            <td>
                                                <asp:ListBox ID="lstEmployee" runat="server" SelectionMode="Multiple"></asp:ListBox></td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="right" width="45%"><span class="required">* </span>
                                                            <strong>Start Date: </strong>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtStartDate" runat="server" Width="120px"></asp:TextBox>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td align="left">
                                                            <cc1:CalendarExtender ID="startdate" runat="server"
                                                                Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                                                PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                            </cc1:CalendarExtender>
                                                            <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="right" width="45%"><span class="required">* </span>
                                                            <strong>End Date: </strong>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtEndDate" runat="server" Width="120px"></asp:TextBox>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td align="left">
                                                            <cc1:CalendarExtender ID="EndDate" runat="server"
                                                                Format="MM/dd/yyyy" PopupButtonID="imgEndDate"
                                                                PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                            </cc1:CalendarExtender>
                                                            <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnView" runat="server" Text="View"
                                                    CssClass="button" OnClick="btnView_Click" />
                                            </td>
                                            <td>&nbsp; &nbsp; 
                                                <asp:LinkButton ID="LinkButton2" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                            </td>
                                            <td align="left">&nbsp;&nbsp;&nbsp;&nbsp;<strong>Status</strong>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="1">No</asp:ListItem>
                                                    <asp:ListItem Value="0">Yes</asp:ListItem>
                                                    <asp:ListItem Value="2">All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="center" valign="middle">
                                    <b>Page: </b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#0088cc"></asp:Label>
                                </td>

                                <td>&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton" OnClick="btnNext_Click"
                                        Text="Next" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="11">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdPMNotesList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" OnPageIndexChanging="grdCustCOList_PageIndexChanging"  Width="100%" PageSize="40" >
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>
                               
                                 <asp:TemplateField HeaderText="Entry Date & Time" >
                                     <HeaderStyle Width="10%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate><%# Eval("CreateDate","{0:MM/dd/yyyy hh:mm tt}").ToString() %></ItemTemplate>
                                </asp:TemplateField>
                            
                                <asp:BoundField DataField="CustomerName" HeaderText="Customer" >
                                    <HeaderStyle Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="superlastname" HeaderText="Superintendent" >
                                    <HeaderStyle Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="estimate_name" HeaderText="Project" >
                                    <HeaderStyle Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="location_name" HeaderText="Location" >
                                    <HeaderStyle Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="section_name" HeaderText="Section" >
                                    <HeaderStyle Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Can Start Tomorrow?" >
                                     <HeaderStyle Width="10%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate><%# (Boolean.Parse(Eval("StartNo").ToString())) ? "No" : "Yes" %></ItemTemplate>
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
                        <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <%-- <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
        </Triggers>--%>
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

