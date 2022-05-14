<%@ Page Language="C#" MasterPageFile="~/schedulemasterreadonly.master" AutoEventWireup="true"
    CodeFile="schedulecalendarreadonly.aspx.cs" Inherits="schedulecalendarreadonly" Title="Schedule Calendar" %>

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
                                            <asp:Label ID="lbltopHead" CssClass="cssTitleHeader cssTitleHeaderCal" runat="server" Text="Schedule Calendar"></asp:Label></span></td>


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
                            <td valign="top" align="left" style="padding: 0px; margin: 0px; width: 20%;"></td>
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
        <div id="updatedialog" class="popWindow popWindowUpdate" title="Event Details">


            <table cellpadding="0" style="padding: 5px; width: 97%;">
                <tr>
                    <td class="alignRight" valign="top">Section/Title:
                    </td>
                    <td class="alignLeft" valign="top">
                        <label id="eventName"></label>
                    </td>

                </tr>
                <tr>
                    <td class="alignRight">Location:
                    
                    </td>
                    <td class="alignLeft">
                        <label id="eventLocation"></label>
                    </td>

                </tr>
                <tr>
                    <td class="alignRight">Assigned To:</td>
                    <td class="alignLeft">
                        <label id="eventSalesPerson"></label>
                    </td>

                </tr>
                <tr>
                    <td class="alignRight">Start:
                    </td>
                    <td class="alignLeft">
                        <label id="eventStart"></label>

                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="alignRight">End:
                    </td>
                    <td class="alignLeft">
                        <label id="eventEnd"></label>

                    </td>
                    <td>
                        <input type="hidden" id="eventId" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div class="tab">
                            <button class="tablinks" onclick="openTab(event, 'ParentEventLink')" id="btnParentEventLink">Parent Link</button>
                            <button class="tablinks" onclick="openTab(event, 'EventLink')" id="btnEventLink">Child Link</button>
                            <button class="tablinks" onclick="openTab(event, 'Notes')">Notes</button>

                        </div>

                        <!-- Tab content -->
                        <div id="ParentEventLink" class="tabcontent">
                            <h3 style="color: #fff;">Parent Link</h3>
                            <table style="width: 99%;">

                                <tr>
                                    <td >
                                       <table id="ParentLinkTbl" class="mGrid" style="width: 100%;"></table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="EventLink" class="tabcontent">
                            <h3 style="color: #fff;">Child Link</h3>
                            <table style="width: 99%;">

                                <tr>
                                    <td >
                                        <table id="ChildLinkTbl" class="mGrid" style="width: 100%;"></table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="Notes" class="tabcontent">
                            <h3 style="color: #fff;">Notes</h3>
                            <table>
                                <tr>
                                    <td class="alignRight" valign="top">Description:
                                    </td>
                                    <td class="alignLeft" valign="top">
                                        <textarea id="eventDesc" class="" style="width: 400px;" cols="40" rows="3" onchange="txtChange(this.id)"></textarea>
                                    </td>
                                    <td class="alignLeft" valign="bottom">
                                        <label id="lbleventDesc" class="hidden">
                                            Required</label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>


                <tr id="trTradePartner" style="display: none;">
                    <td class="alignRight" style="padding-bottom: 12px;">Trade:
                    </td>
                    <td class="alignLeft">
                        <input id="txtTradePartner" maxlength="40" style="width: 225px" />
                    </td>
                    <td>
                        <label id="lblTradePartner" class="hidden" style="padding-bottom: 10px;">Required</label>
                    </td>
                </tr>
                <tr id="trNotes" style="display: none;">
                    <td class="alignRight" valign="top" style="padding-top: 3px;">Notes:
                    </td>
                    <td class="alignLeft">
                        <textarea id="txtNotes" maxlength="180" class="scTextArea" cols="40" rows="3"></textarea>
                    </td>
                    <td>
                        <label id="lblNotes" class="hidden">Required</label>
                    </td>
                </tr>
                <tr>
                    <td class="alignRight">&nbsp;
                    </td>
                    <td colspan="2">
                        <label id="lblRequired" class="hidden">End time must be later than start time</label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="loading">
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </div>

        <div id="dialog-confirm" class="ui-dialog-content-confirm" title="Schedule change" hidden="true">
            <p style="color: Black; font-size: 16px; line-height: 18px;">
                Make changes to this and the subsequent entries?
            </p>
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

