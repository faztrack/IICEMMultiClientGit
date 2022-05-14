<%@ Page Title="Labor Time Tracking List" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="labor_hour_list_Old.aspx.cs" Inherits="labor_hour_list_Old" %>

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
    <style>
        .cellColor {
            font-weight: bold;
        }

        .hidden {
            display: none;
        }
    </style>

    <style type="text/css">
        .MyTabStyle .ajax__tab_header {
            font-family: "Helvetica Neue", Arial, Sans-Serif;
            font-size: 14px;
            font-weight: bold;
            display: block;
        }

            .MyTabStyle .ajax__tab_header .ajax__tab_outer {
                border-color: #222;
                color: #222;
                padding-left: 10px;
                margin-right: 3px;
                border: solid 1px #d7d7d7;
            }

            .MyTabStyle .ajax__tab_header .ajax__tab_inner {
                border-color: #666;
                color: #666;
                padding: 3px 10px 2px 0px;
            }

        .MyTabStyle .ajax__tab_hover .ajax__tab_outer {
            background-color: #9c3;
        }

        .MyTabStyle .ajax__tab_hover .ajax__tab_inner {
            color: #fff;
        }

        .MyTabStyle .ajax__tab_active .ajax__tab_outer {
            border-bottom-color: #ffffff;
            background-color: #d7d7d7;
        }

        .MyTabStyle .ajax__tab_active .ajax__tab_inner {
            color: #000;
            border-color: #333;
        }

        .MyTabStyle .ajax__tab_body {
            font-family: verdana,tahoma,helvetica;
            font-size: 10pt;
            background-color: #fff;
            border-top-width: 0;
            border: solid 1px #d7d7d7;
            border-top-color: #ffffff;
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

                        <cc1:TabContainer ID="TabContainer4" runat="server" Width="100%" CssClass="MyTabStyle">
                            <cc1:TabPanel runat="server" HeaderText="Crew Member" ID="TabCrewMember">
                                <ContentTemplate>

                                    <table cellpadding="0" cellspacing="0" width="90%">
                                        <tr>
                                            <td>

                                                <table width="100%">
                                                    <tr>

                                                        <td align="right"><strong>Crew Member Name:</strong></td>
                                                        <td align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="left">
                                                                      
                                                                        <asp:ListBox ID="lstCrew" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="button" Text="Search"  OnClick="btnSearch_Click"></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="underlineButton" Text="Reset" OnClick="lnkViewAll_Click"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left">
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
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkTotalhours" runat="server" Text="Summary Hours" AutoPostBack="true" OnCheckedChanged="chkTotalhours_CheckedChanged" />


                                                        </td>

                                                        <td align="right">
                                                            <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="ADD NEW TIME ENTRY" CssClass="button" />
                                                            <asp:Button ID="btnUpdateData" runat="server" OnClick="btnUpdateData_Click" Text="UpdateData" CssClass="button" Visible="false"/>
                                                        </td>
                                                        <td align="right" style="padding-right: 30px;">
                                                            <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn nostyle2" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
                                                        </td>

                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top" colspan="7">
                                                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                                        </td>
                                                        <td align="right">&nbsp;

                                                        </td>
                                                        <td align="center">
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
                                                        <td align="left">&nbsp;

                                                        </td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNext" runat="server" Text="Next"
                                                                OnClick="btnNext_Click" CssClass="nextButton" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="7">
                                                            <asp:GridView ID="grdLaberTrack" runat="server"
                                                                AutoGenerateColumns="False" OnRowDataBound="grdLaberTrack_RowDataBound"
                                                                OnPageIndexChanging="grdLaberTrack_PageIndexChanging"
                                                                OnDataBound="grdLaberTrack_DataBound"
                                                                OnRowCreated="grdLaberTrack_RowCreated"
                                                                Width="100%" CssClass="mGrid">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="GPSTrackID" ItemStyle-Font-Underline="true"
                                                                        DataNavigateUrlFormatString="laborhourdetails.aspx?gpid={0}"
                                                                        DataTextField="labor_date" HeaderText="DATE/EDIT" DataTextFormatString="{0:d}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:HyperLinkField>

                                                                    <asp:BoundField HeaderText="Crew Member Name" DataField="UserID">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>


                                                                    <asp:BoundField HeaderText="Start Location" DataField="StartPlace">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndPlace" HeaderText="End Location">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="6%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                      <asp:BoundField HeaderText="Customer Start Address" DataField="StartCustomerAddress" ItemStyle-CssClass="preformatted">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndCustomerAddress" HeaderText="Customer End Address" ItemStyle-CssClass="preformatted">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="left" /> </asp:BoundField>
                                                                    <asp:TemplateField HeaderText="LUNCH (Hrs)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblLunch" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderText="Total (Hr:Min)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTotalHours" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFile" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="">
                                                                        <ItemStyle CssClass="hidden" />
                                                                        <HeaderStyle CssClass="hidden" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="">
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
                                </ContentTemplate>

                            </cc1:TabPanel>
                            <cc1:TabPanel runat="server" HeaderText="Employee" ID="TabEmployee">
                                <ContentTemplate>
                                    <table cellpadding="0" cellspacing="0" width="90%">
                                        <tr>
                                            <td>

                                                <table width="100%">
                                                    <tr>

                                                        <td align="right"><strong>Employee Name:</strong></td>
                                                        <td align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="left">
                                                                       <asp:ListBox ID="lstEmployee" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="btnEmployeeSearch" runat="server" CssClass="button" Text="Search"  OnClick="btnEmployeeSearch_Click"></asp:LinkButton>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="underlineButton" Text="Reset" OnClick="lnkViewAllEmp_Click"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left">
                                                            <table style="padding: 0px; margin: 0px;">
                                                                <tr>
                                                                    <td align="right"><strong>Date Range:</strong></td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtEmployeeStartDate" runat="server" CssClass="textBox" Width="70px" TabIndex="1"></asp:TextBox>
                                                                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDateEmp" PopupPosition="BottomLeft" TargetControlID="txtEmployeeStartDate">
                                                                        </cc1:CalendarExtender>
                                                                        <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat="server" TargetControlID="txtEmployeeStartDate" WatermarkText="Start Date" />
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:ImageButton ID="imgStartDateEmp" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtEmployeeEndDate" runat="server" CssClass="textBox" Width="70px" TabIndex="2"></asp:TextBox>
                                                                        <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEndDateEmp" PopupPosition="BottomLeft" TargetControlID="txtEmployeeEndDate">
                                                                        </cc1:CalendarExtender>
                                                                        <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender4" runat="server" TargetControlID="txtEmployeeEndDate" WatermarkText="End Date" />
                                                                    </td>
                                                                    <td align="right">
                                                                        <asp:ImageButton ID="imgEndDateEmp" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton ID="btnViewEmp" runat="server" CssClass="underlineButton" Text="View" OnClick="btnViewEmp_Click"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left">
                                                            <asp:CheckBox ID="chkEmployeeTotalHours" runat="server" Text="Summary Hours" AutoPostBack="true" OnCheckedChanged="chkEmployeeTotalHours_CheckedChanged" />


                                                        </td>

                                                        <td align="right">
                                                            <asp:Button ID="btnAddNewEmp" runat="server" OnClick="btnAddNewEmp_Click" Text="ADD NEW TIME ENTRY" CssClass="button" />
                                                        </td>

                                                        <td align="right" style="padding-right: 30px;">
                                                            <asp:ImageButton ID="ImabtnEmployee" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn nostyle2" OnClick="btnExpCustListEmp_Click" ToolTip="Export List to CSV" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top" colspan="7">
                                                            <asp:Label ID="lblResultEmp" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Button ID="btnPreviousEmp" runat="server" Text="Previous" OnClick="btnPreviousEmp_Click" CssClass="prevButton" />
                                                        </td>
                                                        <td align="right">&nbsp;

                                                        </td>
                                                        <td align="center">
                                                            <b>Page: </b>
                                                            <asp:Label ID="lblCurrentPageNoEmp" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                            <b>Item per page: </b>
                                                            <asp:DropDownList ID="ddlItemPerPageEmployee" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPageEmployee_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True">30</asp:ListItem>
                                                                <asp:ListItem>40</asp:ListItem>
                                                                <asp:ListItem>50</asp:ListItem>
                                                                <asp:ListItem>60</asp:ListItem>


                                                            </asp:DropDownList>

                                                        </td>
                                                        <td align="left">&nbsp;

                                                        </td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNextEmp" runat="server" Text="Next"
                                                                OnClick="btnNextEmp_Click" CssClass="nextButton" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" colspan="7">
                                                            <asp:GridView ID="gdrEmployeeTimeTracker" runat="server"
                                                                AutoGenerateColumns="False" OnRowDataBound="gdrEmployeeTimeTracker_RowDataBound"
                                                                OnPageIndexChanging="gdrEmployeeTimeTracker_PageIndexChanging"
                                                                OnDataBound="gdrEmployeeTimeTracker_DataBound"
                                                                OnRowCreated="gdrEmployeeTimeTracker_RowCreated"
                                                                Width="100%" CssClass="mGrid">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:HyperLinkField DataNavigateUrlFields="GPSTrackID" ItemStyle-Font-Underline="true"
                                                                        DataNavigateUrlFormatString="laborhourdetails.aspx?gpid={0}"
                                                                        DataTextField="labor_date" HeaderText="DATE/EDIT" DataTextFormatString="{0:d}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:HyperLinkField>

                                                                    <asp:BoundField HeaderText="Employee Name" DataField="UserID">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>


                                                                    <asp:BoundField HeaderText="Start Location" DataField="StartPlace">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndPlace" HeaderText="End Location">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                                        <ItemStyle HorizontalAlign="left" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="6%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:t}">
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:BoundField>
                                                                     
                                                                    <asp:TemplateField HeaderText="LUNCH (Hrs)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblLunch" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderText="Total (Hr:Min)">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTotalHours" runat="server" Text=""></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFileEmp" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="StartTime" HeaderText="">
                                                                        <ItemStyle CssClass="hidden" />
                                                                        <HeaderStyle CssClass="hidden" />
                                                                    </asp:BoundField>
                                                                    <asp:BoundField DataField="EndTime" HeaderText="">
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
                                                            <asp:Button ID="btnPrevious0Emp" runat="server" Text="Previous" CssClass="prevButton" OnClick="btnPrevious_Click" />
                                                        </td>
                                                        <td align="right">&nbsp;</td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="left">&nbsp;</td>
                                                        <td align="left">&nbsp;
                                                          <asp:Label ID="Label3" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNext0Emp" runat="server" Text="Next" CssClass="nextButton" OnClick="btnNext_Click" />

                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>

                                    </table>
                                </ContentTemplate>
                            </cc1:TabPanel>

                        </cc1:TabContainer>


                    </td>
                </tr>

                <tr>
                    <td align="center"></td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnExpCustList" />--%>
            <asp:PostBackTrigger ControlID="TabContainer4$TabCrewMember$btnExpCustList" />
            <asp:PostBackTrigger ControlID="TabContainer4$TabEmployee$ImabtnEmployee" />
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



