<%@ Page Title="Schedule SMS Notification" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="schedulenotification.aspx.cs" Inherits="schedulenotification" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
        function CheckAllSubmit(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdScheduleNotification.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }

        function CheckUnselect(Checkbox) {
            var bFlag = false;
            var GridVwHeaderChckbox = document.getElementById("<%=grdScheduleNotification.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0].checked == false) {
                    bFlag = true;
                    break;
                }
            }
            if (bFlag) {
                GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = false;
            }
            else {
                GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = true;
            }
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Schedule SMS Notification</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="90%">
                            <tr>
                                <%-- <td>
                                    <asp:Label ID="lblCount" runat="server" Font-Bold="true" Visible="false"></asp:Label>&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="lblToday" runat="server" Font-Bold="true" Visible="false"></asp:Label>&nbsp;&nbsp;&nbsp;
                                     <asp:Label ID="lblTomorrow" runat="server" Font-Bold="true" Visible="false"></asp:Label>
                                </td>--%>
                                <td>
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>

                                    <table style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblScheduleDate" runat="server" Font-Bold="true">Event Date: </asp:Label><asp:TextBox ID="txtScheduleDate" runat="server"></asp:TextBox></td>
                                            <td>
                                                <asp:ImageButton CssClass="nostyleCalImg" ID="imgScheduleDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                <cc1:CalendarExtender ID="CalExtScheduleDate" runat="server"
                                                    Format="MM/dd/yyyy" PopupButtonID="imgScheduleDate"
                                                    PopupPosition="BottomLeft" TargetControlID="txtScheduleDate">
                                                </cc1:CalendarExtender>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnView" runat="server" Text="View" CssClass="button" OnClick="btnView_Click" />
                                            </td>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:Button ID="btnPrevious" runat="server" Text="<< Previous" CssClass="button" OnClick="btnPrevious_Click" />
                                                <asp:Button ID="btnToday" runat="server" Text="Today" CssClass="button" OnClick="btnToday_Click" />
                                                <asp:Button ID="btnNext" runat="server" Text="Next >>" CssClass="button" OnClick="btnNext_Click" />
                                            </td>
                                        </tr>
                                    </table>

                                    <%--style="display:none;" Enabled="false"--%>

                                </td>
                                <td align="right">
                                    <asp:Button ID="btnSubmit" style="display:none;" Enabled="false"  runat="server" Text="Submit Notification" CssClass="button" OnClick="btnSubmit_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <table width="100%">

                                        <tr>
                                            <td align="center">

                                                <asp:GridView ID="grdScheduleNotification" runat="server"
                                                    AutoGenerateColumns="False" Width="100%" OnRowDataBound="grdScheduleNotification_RowDataBound"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                      <%--  <asp:TemplateField ItemStyle-Width="5%">
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="chkboxSelectAll" runat="server" OnCheckedChanged="chkboxSelectAll_CheckedChanged" AutoPostBack="true" />&nbsp;ALL                                        
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkSendAll" runat="server" OnCheckedChanged="chkSendAll_CheckedChanged" AutoPostBack="true" />
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>--%>
                                                        <asp:BoundField DataField="title" HeaderText="Event" ItemStyle-Width="25%">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Customer" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <table style="padding: 0px; margin: 0px; width: 80%;">
                                                                    <tr>
                                                                        <td style="padding: 0px; margin: 0px; width: 80%;">
                                                                            <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval("customer_name").ToString() %>'></asp:Label>
                                                                            <asp:LinkButton ID="lnkCustomer" style="text-decoration:underline;" runat="server" Text='<%# Eval("customer_name").ToString() %>' Visible="false" OnClick="lnkCustomer_Click"></asp:LinkButton>
                                                                        </td>
                                                                      <%--  <td style="padding: 0px; margin: 0px;">
                                                                            <asp:CheckBox ID="chkSendToCustomer" runat="server" OnCheckedChanged="chkSendToCustomer_CheckedChanged" AutoPostBack="true" /></td>--%>
                                                                    </tr>
                                                                </table>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Crew" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:GridView ID="grdCrewList" runat="server" ShowHeader="false" CssClass="noBorderCss" DataKeyNames="crew_id"
                                                                    AutoGenerateColumns="False" Width="90%" OnRowDataBound="grdCrewList_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField ShowHeader="false">
                                                                            <ItemTemplate>
                                                                                <table style="padding: 0px; margin: 0px; width: 90%;">
                                                                                    <tr>
                                                                                        <td style="padding: 0px; margin: 0px; width: 80%;">
                                                                                            <asp:Label ID="lblCrewName" runat="server" Text='<%# Eval("full_name").ToString() %>'></asp:Label>
                                                                                            <asp:LinkButton ID="lnkCrew" style="text-decoration:underline;" runat="server" Text='<%# Eval("full_name").ToString() %>' Visible="false" OnClick="lnkCrew_Click"></asp:LinkButton>
                                                                                        </td>
                                                                                      <%--  <td style="padding: 0px; margin: 0px;">
                                                                                            <asp:CheckBox ID="chkSendToCrew" runat="server" OnCheckedChanged="chkSendToCrew_CheckedChanged" AutoPostBack="true" /></td>--%>
                                                                                    </tr>
                                                                                </table>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="15%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSubmittedBy" runat="server"></asp:Label>
                                                                <br />
                                                                <br />
                                                                <asp:Label ID="lblMessageStatus" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Delivery Status" ItemStyle-Width="25%">
                                                            <ItemTemplate>
                                                                <asp:GridView ID="grdDeliveryStatus" runat="server" ShowHeader="false" CssClass="noBorderCss" DataKeyNames="ScheduleSMSId"
                                                                    AutoGenerateColumns="False" Width="100%" >
                                                                    <Columns>
                                                                        <asp:TemplateField ShowHeader="false">
                                                                            <ItemTemplate>
                                                                                <table style="padding: 0px; margin: 0px; width: 100%;">
                                                                                    <tr>
                                                                                        <td style="padding: 0px; margin: 0px; width: 30%;">
                                                                                            <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("mobile").ToString() %>' CssClass='<%# Eval("cssStatus").ToString() %>'></asp:Label>                                                                                          
                                                                                        </td>
                                                                                        <td style="padding: 0px; margin: 0px;">
                                                                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status").ToString() %>' CssClass='<%# Eval("cssStatus").ToString() %>'></asp:Label></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>

                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="right">
                                    <asp:Button ID="btnSubmitBtm" style="display:none;" Enabled="false" runat="server" Text="Submit Notification" CssClass="button" OnClick="btnSubmit_Click" />
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

