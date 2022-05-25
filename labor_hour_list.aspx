<%@ Page Title="Labor Time Tracking List" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="labor_hour_list.aspx.cs" Inherits="labor_hour_list" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
        $(document).ready(function () {
            $(<%=lstCrew.ClientID%>).SumoSelect({
                selectAll: true,
                search: true,
                searchText: 'Search...',
                // triggerChangeCombined: true,
                //locale: ['OK', 'Cancel', 'Select All'],
                placeholder: 'Select Here',
                noMatch: 'No matches for "{0}"',
                //okCancelInMulti: true,
            });

           
            $(<%=lstJobName.ClientID%>).SumoSelect({
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
            $(<%=lstCrew.ClientID%>).SumoSelect({
               selectAll: true,
               search: true,
               searchText: 'Search...',
               //triggerChangeCombined: true,
               //locale: ['OK', 'Cancel', 'Select All'],
               placeholder: 'Select Here',
               noMatch: 'No matches for "{0}"',
               //okCancelInMulti: true,
           });
          
           $(<%=lstJobName.ClientID%>).SumoSelect({
               selectAll: true,
               search: true,
               searchText: 'Search...',
               // triggerChangeCombined: true,
               //locale: ['OK', 'Cancel', 'Select All'],
               placeholder: 'Select Here',
               noMatch: 'No matches for "{0}"',
               //okCancelInMulti: true,
           });
        }
    </script>
    <style>
        .cellColor {
            font-weight: bold;
        }

        .hidden {
            display: none;
        }
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

    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Labor Time Tracking List</asp:Label></span></td>

                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>

                <tr>
                    <td align="center">

                       

                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td>

                                                <table width="100%">
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                        <td width="10%" align="right"><strong>Crew/Employee Name:</strong></td>
                                                        <td align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="left">

                                                                        <asp:ListBox ID="lstCrew" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="btnSearch" runat="server" style="padding:3px 10px 3px 10px;" CssClass="button" Text="Search" OnClick="btnSearch_Click"></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="underlineButton" Text="Reset" OnClick="lnkViewAll_Click"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td width="25%" align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="right"><strong>Date Range:</strong></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="textBox" Width="70px" TabIndex="1"></asp:TextBox>
                                                                        <cc1:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate" PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                                        </cc1:CalendarExtender>
                                                                        <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtStartDate" WatermarkText="Start Date" />
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="textBox" Width="70px" TabIndex="2"></asp:TextBox>
                                                                        <cc1:CalendarExtender ID="txtEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEndDate" PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                                        </cc1:CalendarExtender>
                                                                        <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtEndDate" WatermarkText="End Date" />
                                                                    </td>
                                                                    <td align="right">
                                                                        <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="btnView" runat="server" CssClass="underlineButton" Text="View" OnClick="btnView_Click"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td width="15%" align="left">
                                                            <asp:CheckBox ID="chkTotalhours" runat="server" Text="Summary Hours" AutoPostBack="true" OnCheckedChanged="chkTotalhours_CheckedChanged" />


                                                        </td>

                                                        <td align="right">
                                                            <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="ADD NEW TIME ENTRY" CssClass="button" />
                                                            <asp:Button ID="btnUpdateData" runat="server" OnClick="btnUpdateData_Click" Text="UpdateData" CssClass="button" Visible="false" />
                                                        </td>
                                                        <td width="8%" align="right">
                                                            <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn nostyle2" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />

                                                            <asp:ImageButton ID="btnGMap1" ImageUrl="~/images/icon_labor_hour_list.png" runat="server" CssClass="imageBtn nostyle2" ToolTip="Map" OnClick="btnGMap_Click" />
                                                        </td>


                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                                        </td>
                                                        <td align="right"><strong>Job Name:</strong></td>
                                                        <td align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="left">

                                                                        <asp:ListBox ID="lstJobName" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                    </td>
                                                                    <%-- <td>
                                                                        <asp:LinkButton ID="btnJobList" runat="server" CssClass="button" Text="Search"  OnClick="btnSearch_Click"></asp:LinkButton>
                                                                    </td>--%>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left">
                                                            <b>Page: </b>
                                                            <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                            <b>Item per page: </b>
                                                            <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True">30</asp:ListItem>
                                                                <asp:ListItem>40</asp:ListItem>
                                                                <asp:ListItem>50</asp:ListItem>
                                                                <asp:ListItem>60</asp:ListItem>


                                                            </asp:DropDownList>
                                                        </td>
                                                        <td align="left">
                                                            <asp:RadioButtonList ID="radEmployeeType" runat="server" OnSelectedIndexChanged="radEmployeeType_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal">
                                                                <asp:ListItem Value="3" Selected="True">All</asp:ListItem>
                                                                <asp:ListItem Value="1">Crews</asp:ListItem>
                                                                <asp:ListItem Value="0">Employees</asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                        <td align="left">
                                                            <b style="color: #000080">Jobs: </b>
                                                            <asp:Label ID="lblView" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                            <b style="color: #028A0F">Active Crew: </b>
                                                            <asp:Label ID="lblActiveCrew" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>

                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNext" runat="server" Text="Next"
                                                                OnClick="btnNext_Click" CssClass="nextButton" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top" colspan="7">
                                                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="8">
                                                            <asp:GridView ID="grdLaberTrack" runat="server"
                                                                AutoGenerateColumns="False" OnRowDataBound="grdLaberTrack_RowDataBound"
                                                                OnPageIndexChanging="grdLaberTrack_PageIndexChanging"
                                                                OnDataBound="grdLaberTrack_DataBound"
                                                                OnRowCreated="grdLaberTrack_RowCreated"
                                                                Width="100%" CssClass="mGrid">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <%--row index=0--%>

                                                                    <asp:TemplateField HeaderText="DATE/EDIT">
                                                                        <ItemTemplate>
                                                                            <asp:HyperLink ID="InkLaborDate" runat="server" Style="text-decoration: underline"></asp:HyperLink></br>
                                                                             <asp:Label ID="lblLaborDayName" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>


                                                                    <%--row index=1--%>
                                                                    <asp:BoundField HeaderText="Crew/Employee Name" DataField="full_name">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=2--%>

                                                                    <asp:BoundField HeaderText="Customer" DataField="CustomerName">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="8%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=3--%>
                                                                    <asp:BoundField HeaderText="Notes" DataField="Notes">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="7%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=4--%>
                                                                    <asp:BoundField DataField="StartPlace" HeaderText="Start Location">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=5--%>
                                                                    <asp:BoundField DataField="EndPlace" HeaderText="End Location">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=6--%>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="6%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                    <%--row index=7--%>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                     <%--row index=8--%>
                                                                    <asp:BoundField DataField="StartCustomerAddress" HeaderText="Customer Start Address">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                     <%--row index=9--%>
                                                                    <asp:BoundField DataField="EndCustomerAddress" HeaderText="Customer End Address">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                    <%--row index=10--%>
                                                                    <asp:TemplateField HeaderText="Type">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSection" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--row index=11--%>
                                                                    <asp:TemplateField HeaderText="LUNCH (Hrs)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblLunch" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--row index=12--%>
                                                                    <asp:TemplateField HeaderText="Regular (Hr:Min)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRegular" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--row index=13--%>
                                                                    <asp:TemplateField HeaderText="OVERTIME (Hr:Min)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblOTHours" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                     <%--row index=14--%>
                                                                     <asp:TemplateField HeaderText="Total (Hr:Min)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTotalHours" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--row index=15--%>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFile" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <%--row index=16--%>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="">
                                                                        <ItemStyle CssClass="hidden" />
                                                                        <HeaderStyle CssClass="hidden" />
                                                                    </asp:BoundField>
                                                                    <%--row index=17--%>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="">
                                                                        <ItemStyle CssClass="hidden" />
                                                                        <HeaderStyle CssClass="hidden" />
                                                                    </asp:BoundField>
                                                                    <%--row index=18--%>
                                                                    <asp:BoundField DataField="WorkingDayName" HeaderText="">
                                                                        <ItemStyle CssClass="hidden" />
                                                                        <HeaderStyle CssClass="hidden" />
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
                                            <td>&nbsp;
                                                
                                                 <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                            </td>
                                        </tr>
                                    </table>
                              


                    </td>
                </tr>

                <tr>
                    <td align="center"></td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
          
           
            <%--  <asp:PostBackTrigger ControlID="chkTotalhours" />
            <asp:PostBackTrigger ControlID="chkEmployeeTotalHours" />--%>
        </Triggers>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
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



