<%@ Page Title="Lead Report" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="lead_report.aspx.cs" Inherits="lead_report" %>

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

    <%--<table width="100%">
        <tr>
            <td>&nbsp;
            </td>
            <td width="50%" align="right">
                <h1>Lead Report</h1>
            </td>
            <td align="right">
                <asp:ImageButton ID="btnExpCustList" CssClass="cssCSV" ImageUrl="~/images/button_csv.png" runat="server"
                    OnClick="btnExpCustList_Click" />
            </td>
        </tr>
    </table>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                  <tr>
                    <td align="center" style="background-color: #eee; color: #fff;box-shadow:0 0 3px #aaa;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Lead Report</span></td>
                                <td align="right"  style="padding-right: 30px;">
                                     <asp:ImageButton ID="btnExpCustList" runat="server" CssClass="imageBtn" ImageUrl="~/images/export_csv.png" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
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
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="button" OnClick="btnPrevious_Click" Text="&lt;&lt; Previous" />
                                </td>
                                
                                <td align="left" colspan="2" valign="middle">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                        <asp:ListItem Value="1">First Name</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                        <asp:ListItem Value="4">Address</asp:ListItem>
                                        <asp:ListItem Value="3">Email</asp:ListItem>
                                    </asp:DropDownList>

                                    <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                    </cc1:AutoCompleteExtender>
                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />

                                    <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" />

                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
                                </td>

                                <td align="center" colspan="2" valign="middle">
                                    <b>Page: </b>

                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                    &nbsp;
                                    <b>Item Per Page: </b>

                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="4">View All</asp:ListItem>
                                    </asp:DropDownList>
                                </td>

                                
                                <td>&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="button" OnClick="btnNext_Click" Text="Next &gt;&gt;" Width="90px" />
                                </td>
                            </tr>
                            <tr>                                
                                <td align="left" colspan="4" valign="middle">
                                    <table style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="right"><b>Appointment Date</b></td>
                                            <td align="left">
                                                <asp:TextBox ID="txtApptStartDate" runat="server" AutoPostBack="true" CssClass="textBox" Width="70px" TabIndex="1" OnTextChanged="txtApptStartDate_TextChanged"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtAppStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgApptStartDate" PopupPosition="BottomLeft" TargetControlID="txtApptStartDate">
                                                </cc1:CalendarExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtApptStartDate" WatermarkText="Start Date" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgApptStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                            </td>                                            
                                            <td align="left">
                                                <asp:TextBox ID="txtApptEndDate" runat="server" CssClass="textBox" AutoPostBack="true" Width="70px" TabIndex="2" OnTextChanged="txtApptEndDate_TextChanged"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtApptEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgApptEndDate" PopupPosition="BottomLeft" TargetControlID="txtApptEndDate">
                                                </cc1:CalendarExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server" TargetControlID="txtApptEndDate" WatermarkText="End Date" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgApptEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                            </td>
                                            <td>&nbsp;&nbsp;&nbsp;</td>
                                            <td align="right"><b>Entry Date</b></td>
                                            <td align="left">
                                                <asp:TextBox ID="txtEntStartDate" runat="server" CssClass="textBox" AutoPostBack="true" Width="70px" TabIndex="1" OnTextChanged="txtEntStartDate_TextChanged"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtEntStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEntStartDate" PopupPosition="BottomLeft" TargetControlID="txtEntStartDate">
                                                </cc1:CalendarExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat="server" TargetControlID="txtEntStartDate" WatermarkText="Start Date" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgEntStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                            </td>
                                            
                                            <td align="left">
                                                <asp:TextBox ID="txtEntEndDate" runat="server" CssClass="textBox" AutoPostBack="true" Width="70px" TabIndex="2" OnTextChanged="txtEntEndDate_TextChanged"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtEntEndDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEntEndDate" PopupPosition="BottomLeft" TargetControlID="txtEntEndDate">
                                                </cc1:CalendarExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender4" runat="server" TargetControlID="txtEntEndDate" WatermarkText="End Date" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgEntEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                               
                                <td align="right" colspan="3" valign="middle">
                                    <table>
                                        <tr>
                                            <td>
                                                <b>Sales Person:</b>
                                                <asp:DropDownList ID="ddlSalesRep" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSalesRep_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <b>Status:</b>
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="7">All</asp:ListItem>
                                                    <asp:ListItem Value="6">Sold</asp:ListItem>
                                                    <asp:ListItem Value="1">New</asp:ListItem>
                                                    <asp:ListItem Value="2">Follow-up</asp:ListItem>
                                                    <asp:ListItem Value="3">In-Design</asp:ListItem>
                                                    <asp:ListItem Value="4">Archive</asp:ListItem>
                                                    <asp:ListItem Value="5">Dead</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <b>Lead Source:</b>
                                                <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="14" AutoPostBack="True" OnSelectedIndexChanged="ddlLeadSource_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>                                            
                                        </tr>
                                    </table>
                                </td>                               
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdLeadList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" DataKeyNames="customer_id" OnPageIndexChanging="grdLeadList_PageIndexChanging" OnRowDataBound="grdLeadList_RowDataBound" Width="100%">
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>

                                <asp:HyperLinkField DataNavigateUrlFields="customer_id" DataNavigateUrlFormatString="lead_details.aspx?cid={0}" DataTextField="customer_name" HeaderText="Customer Name">
                                    <HeaderStyle HorizontalAlign="Center" Width="6%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:HyperLinkField>

                                <asp:TemplateField HeaderText="Address&lt;/br&gt;Phone / eMail">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                        <br />
                                        <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                        <br />
                                        <asp:HyperLink ID="hypEmail" runat="server" Target="_blank">[hypEmail]</asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="sales_person_id" HeaderText="Sales Person">
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="appointment_date" HeaderText="Appt. Date" DataFormatString="{0:d}">
                                    <HeaderStyle Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="appointment_date" HeaderText="Appt. Time" DataFormatString="{0:T}">
                                    <HeaderStyle Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="lead_source_id" HeaderText="Lead Source">
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="status_id" HeaderText="Lead Status">
                                    <HeaderStyle Width="4%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="registration_date" HeaderText="Entry Date" DataFormatString="{0:d}">
                                    <HeaderStyle Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="notes" HeaderText="Notes">
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
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
                                    <asp:Button ID="btnPrevious0" runat="server" CssClass="button" OnClick="btnPrevious_Click" Text="&lt;&lt; Previous"/>
                                </td>
                                <td align="right" style="width: 87px">&nbsp;
                                </td>
                                <td align="left">&nbsp;
                                                <asp:DropDownList ID="ddlSuperintendent" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlSuperintendent_SelectedIndexChanged"
                                                    Visible="False">
                                                </asp:DropDownList>
                                </td>
                                <td align="left" style="width: 245px">&nbsp;
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server" CssClass="button" OnClick="btnNext_Click" Text="Next &gt;&gt;" Width="90px" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
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

