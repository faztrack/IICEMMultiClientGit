<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="gMessageList.aspx.cs" Inherits="gMessageList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function selected_SentBy(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>

    <script language="JavaScript" type="text/JavaScript">
        function GetdatakeyValue1Old(custId, value) {
            window.open('messagedetails.aspx?custId=' + custId + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="mainTable" cellpadding="0" cellspacing="0" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderMess" runat="server"></asp:Label>
                                    &nbsp;<asp:CheckBox ID="chkGlobal" runat="server" AutoPostBack="True" OnCheckedChanged="chkGlobal_CheckedChanged" Text="Show Global Message List" />
                                </span></td>
                                <td align="left"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="top">

                        <table width="100%" border="0" cellspacing="0" cellpadding="0" align="center">
                            <tr>
                                <td class="" align="center" valign="top">
                                    <table class="mainTable" width="100%" border="0" cellspacing="8" cellpadding="4" align="center">
                                        <tr>
                                            <td align="center">
                                                <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="left" valign="middle">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                                            <asp:ListItem Selected="True" Value="1">Sent by</asp:ListItem>
                                                                            <asp:ListItem Value="2">Email From</asp:ListItem>
                                                                            <asp:ListItem Value="3">Email To</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_SentBy" ServiceMethod="GetSentBy" TargetControlID="txtSearch" UseContextKey="True">
                                                                        </cc1:AutoCompleteExtender>
                                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Sent By" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                                                    <td>
                                                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton></td>
                                                                </tr>
                                                            </table>
                                                        </td>

                                                        <td align="right" valign="middle"></td>

                                                    </tr>
                                                </table>
                                                <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="left" valign="middle">
                                                            <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                                        </td>
                                                        <td align="center" valign="middle">
                                                            <table>
                                                                <tr>
                                                                    <td><b>Page: </b></td>
                                                                    <td>
                                                                        <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#1574c4"></asp:Label></td>
                                                                    <td><b>Item Per Page: </b></td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                                            <asp:ListItem Selected="True">20</asp:ListItem>
                                                                            <asp:ListItem>30</asp:ListItem>
                                                                            <asp:ListItem>40</asp:ListItem>
                                                                            <asp:ListItem Value="4">View All</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="right">
                                                            <asp:Button ID="btnNext" runat="server" CssClass="nextButton" OnClick="btnNext_Click"
                                                                Text="Next" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:GridView ID="grdCustomersMessage" runat="server" AutoGenerateColumns="False"
                                                    CssClass="mGrid" OnRowDataBound="grdCustomersMessage_RowDataBound" PageSize="50"
                                                    Width="100%" AllowPaging="True" OnPageIndexChanging="grdCustomersMessage_PageIndexChanging">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:BoundField DataField="create_date" DataFormatString="{0:d}" HeaderText="Date">
                                                            <ItemStyle HorizontalAlign="center" CssClass="iPXDate" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Type" HeaderText="Type" ItemStyle-CssClass="iPXTime" />
                                                        <asp:BoundField DataField="From" HeaderText="From" ItemStyle-CssClass="iPXFrom" />
                                                        <asp:BoundField DataField="To" HeaderText="To" ItemStyle-CssClass="iPXTo" />
                                                        <asp:BoundField DataField="mess_subject" HeaderText="Subject" ItemStyle-CssClass="iPXSubject" />

                                                        <asp:TemplateField HeaderText="Attachment">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypAttachment" runat="server" CssClass="hypg"></asp:HyperLink>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" CssClass="iPXSubject" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="sent_by" HeaderText="Sent by" ItemStyle-CssClass="iPXSentBy" />
                                                        <asp:BoundField DataField="IsRead" HeaderText="Viewed?">
                                                            <ItemStyle HorizontalAlign="center" CssClass="iPXViewed" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="Protocol" HeaderText="Sent Via" ItemStyle-CssClass="iPXSentVia" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypMessageDetails" runat="server" CssClass="btn_details">Details</asp:HyperLink>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" CssClass="iPXDetails" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" />
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
                                            <td align="center">
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
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
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

