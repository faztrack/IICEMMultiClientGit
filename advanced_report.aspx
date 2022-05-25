<%@ Page Title="Advanced Report" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="advanced_report.aspx.cs" Inherits="advanced_report" %>

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
                    <td align="center" style="background-color: #ddd; color: #fff;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Lead List</span></td>
                                <td align="right"   style="padding-right: 30px;">
                                    <asp:ImageButton ID="btnExpCustList" runat="server" CssClass="imageBtn" ImageUrl="~/images/export_csv.png" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table class="wrapper" cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td class="wrappermini" align="left" colspan="4">
                                    <asp:Panel ID="Panel1" runat="server">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="ImageTGI1" runat="server" ImageUrl="~/Images/expand.png" TabIndex="40" />
                                                </td>
                                                <td>
                                                    <h2>Customer Information</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server" ImageControlID="ImageTGI1" CollapseControlID="ImageTGI1"
                                        ExpandControlID="ImageTGI1" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGI1" Collapsed="True">
                                    </cc1:CollapsiblePanelExtender>

                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="4">
                                    <asp:Panel ID="pnlTGI1" runat="server" Height="100%">
                                        <table width="99%">
                                            <tr>
                                                <td align="left" class="wrapperminiPanal">
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
                                                </td>
                                            </tr>
                                        </table>


                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="wrappermini" align="left" colspan="4">
                                    <asp:Panel ID="Panel2" runat="server">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="ImageTGI2" runat="server" ImageUrl="~/Images/expand.png" TabIndex="40" /></td>
                                                <td>
                                                    <h2>Lead &amp; Schedule Information</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" ImageControlID="ImageTGI2" CollapseControlID="ImageTGI2"
                                        ExpandControlID="ImageTGI2" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGI2" Collapsed="True">
                                    </cc1:CollapsiblePanelExtender>

                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="4">
                                    <asp:Panel ID="pnlTGI2" runat="server" Height="100%">
                                        <table cellpadding="2" cellspacing="2" width="99%">
                                            <tr>
                                                <td align="left" class="wrapperminiPanal">
                                                    <table style="padding: 0px; margin: 0px;">
                                                        <tr>
                                                            <td align="right"><b>Appointment Date</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtApptStartDate" runat="server" CssClass="textBox" Width="70px" TabIndex="1"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="txtAppStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgApptStartDate" PopupPosition="BottomLeft" TargetControlID="txtApptStartDate">
                                                                </cc1:CalendarExtender>
                                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtApptStartDate" WatermarkText="Start Date" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:ImageButton ID="imgApptStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtApptEndDate" runat="server" CssClass="textBox" Width="70px" TabIndex="2"></asp:TextBox>
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
                                                                <asp:TextBox ID="txtEntStartDate" runat="server" CssClass="textBox" Width="70px" TabIndex="1"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="txtEntStartDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgEntStartDate" PopupPosition="BottomLeft" TargetControlID="txtEntStartDate">
                                                                </cc1:CalendarExtender>
                                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender3" runat="server" TargetControlID="txtEntStartDate" WatermarkText="Start Date" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:ImageButton ID="imgEntStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                            </td>

                                                            <td align="left">
                                                                <asp:TextBox ID="txtEntEndDate" runat="server" CssClass="textBox" Width="70px" TabIndex="2"></asp:TextBox>
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
                                            </tr>
                                            <tr>
                                                <td align="left" class="wrapperminiPanal">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <b>Sales Person:</b><br />
                                                                <asp:ListBox ID="lsbSalesRep" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                            </td>
                                                            <td>
                                                                <b>Status:</b><br />
                                                                <asp:ListBox ID="lsbStatus" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                            </td>

                                                            <td>
                                                                <b>Lead Source:</b><br />
                                                                <asp:ListBox ID="lsbLeadSource" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="wrappermini" align="left" colspan="4">
                                    <asp:Panel ID="Panel3" runat="server">
                                        <table style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td>
                                                    <asp:ImageButton ID="ImageTGI3" runat="server" ImageUrl="~/Images/expand.png" TabIndex="40" /></td>
                                                <td>
                                                    <h2>Sale Information</h2>
                                                </td>
                                            </tr>
                                        </table>

                                    </asp:Panel>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender3" runat="server" ImageControlID="ImageTGI3" CollapseControlID="ImageTGI3"
                                        ExpandControlID="ImageTGI3" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGI3" Collapsed="True">
                                    </cc1:CollapsiblePanelExtender>

                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="4">
                                    <asp:Panel ID="pnlTGI3" runat="server" Height="100%">
                                        <table cellpadding="2" cellspacing="2" width="99%">
                                            <tr>
                                                <td align="left" class="wrapperminiPanal">
                                                    <table style="padding: 0px; margin: 0px;">
                                                        <tr>
                                                            <td align="left"><b>Sale Date</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtSaleStartDate" runat="server" CssClass="textBox" Width="65px" TabIndex="1"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgSaleStartDate" PopupPosition="BottomLeft" TargetControlID="txtSaleStartDate">
                                                                </cc1:CalendarExtender>
                                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender5" runat="server" TargetControlID="txtSaleStartDate" WatermarkText="Start Date" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:ImageButton ID="imgSaleStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtSaleEndDate" runat="server" CssClass="textBox" Width="65px" TabIndex="2"></asp:TextBox>
                                                                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgSaleEndDate" PopupPosition="BottomLeft" TargetControlID="txtSaleEndDate">
                                                                </cc1:CalendarExtender>
                                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender6" runat="server" TargetControlID="txtSaleEndDate" WatermarkText="End Date" />
                                                            </td>
                                                            <td align="left">
                                                                <asp:ImageButton ID="imgSaleEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" class="wrapperminiPanal">
                                                    <table style="padding: 0px; margin: 0px;">
                                                        <tr>
                                                            <td align="right"><b>Sale Amount between</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMinAmount" runat="server" CssClass="textBox" Width="50px" TabIndex="1"></asp:TextBox>
                                                            </td>
                                                            <td align="center">
                                                                <b>to</b></td>

                                                            <td align="left">
                                                                <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="textBox" Width="50px" TabIndex="2"></asp:TextBox>
                                                            </td>
                                                            <td align="left">
                                                                <b>OR</b>
                                                            </td>
                                                            <td align="right"><b>Sale Amount is greater than </b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtGreaterAmount" runat="server" CssClass="textBox" Width="50px" TabIndex="1"></asp:TextBox>
                                                            </td>
                                                            <td align="left">
                                                                <b>OR</b>
                                                            </td>
                                                            <td align="right"><b>Sale Amount is Less than </b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtLessAmount" runat="server" CssClass="textBox" Width="50px" TabIndex="1"></asp:TextBox>

                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right"><b>Received Amount between</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMinRcvdAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="center"><b>to</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMaxRcvdAmount" runat="server" CssClass="textBox" TabIndex="2" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="left"><b>OR</b></td>
                                                            <td align="right"><b>Recevied Amount is greater than </b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtGreaterRcvdAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="left"><b>OR</b></td>
                                                            <td align="right"><b>Receivd Amount is Less than</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtLessRcvdAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right"><b>Due Amount between</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMinDueAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="center"><b>to</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMaxDueAmount" runat="server" CssClass="textBox" TabIndex="2" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="left"><b>OR</b></td>
                                                            <td align="right"><b>Due Amount is greater than</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtDueGreaterAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                            <td align="left"><b>OR</b></td>
                                                            <td align="right"><b>Due Amount is Less than</b></td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtLessDueAmount" runat="server" CssClass="textBox" TabIndex="1" Width="50px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td class="wrappermini" colspan="4">
                                    <table cellpadding="2" cellspacing="2" width="100%">
                                        <tr>
                                            <td align="left" class="wrapperminiPanal">
                                                <table style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td style="text-align: left;">
                                                            <b>Location</b><br />
                                                            <asp:ListBox ID="lsbLocation" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                        </td>
                                                        <td style="text-align: left;">
                                                            <b>Section</b><br />
                                                            <asp:ListBox ID="lsbSection" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                        </td>
                                                        <td style="width: 20px;">&nbsp;</td>
                                                        <td align="left">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <table>
                                                                            <tr>
                                                                                <td>&nbsp;</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Button ID="btnUp" runat="server" CssClass="btnRotate90" Text="^" OnClick="btnUp_Click" />
                                                                                    <br />
                                                                                    <asp:Button ID="btnDown" runat="server" CssClass="btnRotate180" Text="^" OnClick="btnDown_Click" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td>
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Display Columns</b>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:ListBox ID="lsbDisplayColumn" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td>
                                                                        <table>
                                                                            <tr>
                                                                                <td>&nbsp;</td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Button ID="first2second" runat="server" CssClass="btnRotate0" Text="&gt;" OnClick="first2second_Click" />
                                                                                    <br />
                                                                                    <asp:Button ID="second2first" runat="server" CssClass="btnRotate-180" Text="&lt;" OnClick="second2first_Click" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                            <b>Hide Columns</b><br />
                                                            <asp:ListBox ID="lsbHideColumn" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="wrapperminiPanal">
                                                <table style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="left">
                                                            <table>
                                                                <tr>
                                                                    <td><span class="required">*</span>
                                                                        <asp:Label ID="lblSaveSearchAs" runat="server" CssClass="lblCss" Text="Save Search As: "></asp:Label></td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSaveSearch" runat="server"></asp:TextBox></td>
                                                                    <td>
                                                                        <asp:Button ID="btnSaveSearch" CssClass="button" runat="server" Text="Save" OnClick="btnSaveSearch_Click" /></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblModifySearch" runat="server" Text="Modify Current Search: " CssClass="lblCss"></asp:Label></td>
                                                                    <td>
                                                                        <asp:Button ID="btnUpdateSearch" runat="server" Text="Update" Enabled="False" OnClick="btnUpdateSearch_Click" /></td>
                                                                    <td>
                                                                        <asp:Button ID="btnDeleteSearch" runat="server" Text="Delete" Enabled="False" OnClick="btnDeleteSearch_Click" /></td>
                                                                    <td>
                                                                        <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lnkViewAll" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton></td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td colspan="2">
                                                                        <asp:Label ID="lblCurrentSearch" runat="server" Text="" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveSearches" runat="server" CssClass="lblCss" Text="Saved Searches: "></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlSaveSearches" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSaveSearches_SelectedIndexChanged">
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
                                            <td colspan="2" style="text-align: center;">
                                                <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="button" OnClick="btnPrevious_Click" Text="&lt;&lt; Previous" />
                                </td>
                                <td align="center" valign="middle"><b>Page: </b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#992a24"></asp:Label>
                                    &nbsp; <b>Item Per Page: </b>
                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="0">View All</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="button" OnClick="btnNext_Click" Text="Next &gt;&gt;" Width="90px" />
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdCustomerReport" runat="server" AutoGenerateColumns="true"
                            PageSize="15" AllowPaging="True" OnRowDataBound="grdCustomerReport_RowDataBound"
                            OnPageIndexChanging="grdCustomerReport_PageIndexChanging"
                            CssClass="mGrid rcGrid">
                            <PagerSettings Position="TopAndBottom" />
                            <PagerStyle HorizontalAlign="Left" />
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
                        <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />   
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



