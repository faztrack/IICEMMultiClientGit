<%@ Page Language="C#" MasterPageFile="~/schedulemastercrewview.master" AutoEventWireup="true"
    CodeFile="schedulecalendarreadcrewview.aspx.cs" Inherits="schedulecalendarreadcrewview" Title="Schedule Calendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .ui-autocomplete-loading {
            background: white url("images/ui-anim_basic_16x16.gif") right center no-repeat;
        }
    </style>

    <div>
        <table cellpadding="0" cellspacing="0" width="100%" align="center">
            <tr>
                <td align="center">
                    <table cellpadding="0" cellspacing="0" width="100%" style="padding: 0px; margin: 0px;">
                        <tr>
                            <td align="center" class="cssHeader" style="padding: 0px !important;">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" style="padding: 10px;"><span class="titleNu">
                                            <asp:Label ID="lbltopHead" CssClass="cssTitleHeader cssTitleHeaderCal" runat="server" Text="Schedule Calendar"></asp:Label></span><img src="images/Resource.png" class="header-ResourceIconImg" /></td>


                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table style="width: 100%; padding: 0px; margin: 0px;">
                        <tr>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                            <td valign="top" align="right" style="padding: 0px; margin: 0px; width: 15%;">
                                <table style="padding: 0px; margin-top: -6px;">

                                    <tr id="trSearchCal" runat="server">
                                        <td align="left">
                                            <table style="padding: 0px; margin: 0px;">
                                                <tr>
                                                    <td align="right">Last Name:&nbsp;</td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtSearch" CssClass="acSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">Section:&nbsp;</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSection" CssClass="acSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">Assigned To:&nbsp;</td>
                                                    <td>
                                                        <asp:TextBox ID="txtUser" CssClass="acSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">Superintendent:&nbsp;</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSuperintendent" CssClass="acSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right"></td>
                                                    <td align="left">
                                                        <asp:Button ID="btnSearch" runat="server" CssClass="btnSearch" Text="Search" OnClientClick="return false;" />&nbsp;
                                                    <asp:LinkButton ID="lnkViewAll" runat="server" Text="View All" OnClientClick="return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <a id="linkCalendarProjectLink" href="schedulecalendar.aspx?TypeID=1" title="Go to Schedule"></a>
                                        </td>
                                    </tr>



                                </table>
                            </td>
                            <td valign="top" align="center" style="padding: 0px; margin: 0px; width: 60%;">
                                <div id="calendar">
                                </div>
                            </td>
                            <td valign="top" align="left" style="padding: 0px; margin: 0px; width: 20%;">
                                <table width="80%">
                                    <tr>
                                        <td>
                                            <p style="color: #000; font-weight: bold;">Superintendent</p>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td>

                                            <asp:GridView ID="grdSuperintendent" runat="server" AutoGenerateColumns="false" CssClass="mGrid" Width="100%" ShowHeader="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="" ShowHeader="false">
                                                        <ItemTemplate>

                                                            <asp:Label ID="lblSuperintendentColor" runat="server" CssClass='<%# "fc-eventcolor " + Eval("cssClassName")%>' Style="cursor: default;" Text="" Width="90%"></asp:Label>


                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" HorizontalAlign="Left" />
                                                        <HeaderStyle Width="40%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="" ShowHeader="false">
                                                        <ItemTemplate>

                                                            <asp:Label ID="lblSuperintendentName" runat="server" Text='<%# Eval("employee_name")%>' Width="100%"></asp:Label>


                                                        </ItemTemplate>
                                                        <ItemStyle Width="60%" HorizontalAlign="Left" />
                                                        <HeaderStyle Width="60%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                                </Columns>

                                            </asp:GridView>

                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                </td>
            </tr>
            <tr>
                <%-- <td align="center">
                <table style="width: 100%; padding: 0px; margin: 0px;">
                    
                </table>

            </td>--%>
            </tr>
        </table>

        <div id="loading">
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </div>


        <div runat="server" id="jsonDiv" />
        <%--<input type="hidden" id="hdClient" runat="server" />--%>

        <asp:HiddenField ID="hdnAddEventName" runat="server" Value="" />
        <asp:HiddenField ID="hdnEventDesc" runat="server" Value="" />
        <asp:HiddenField ID="hdnEditeventName" runat="server" Value="" />
        <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnEmployeeID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnTypeID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnEventStartDate" runat="server" Value="" />
        <asp:HiddenField ID="hdnServiceCssClass" runat="server" Value="fc-default" />
        <asp:HiddenField ID="hdnEstIDSelected" runat="server" Value="0" />
        <asp:HiddenField ID="hdnCustIDSelected" runat="server" Value="0" />
        <asp:HiddenField ID="hdnAutoCustID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnEventId" runat="server" Value="0" />
        <asp:HiddenField ID="hdnUpdateDialogShow" runat="server" Value="false" />
        <asp:HiddenField ID="hdnEventLinkCount" runat="server" Value="0" />

        <br />
        <br />
    </div>
</asp:Content>

