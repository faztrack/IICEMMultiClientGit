<%@ Page Title="Labor Time Tracking Details" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="laborhourdetails.aspx.cs" Inherits="laborhourdetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }

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
    <style>
        .redColor {
            background: red;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table id="Table5" align="center" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" CssClass="cssTitleHeader" runat="server">Add New Labor Tracking</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="Table2" width="98%" align="center" border="0" cellpadding="0" cellspacing="3" onclick="return TABLE1_onclick()">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <table width="100%">
                                        <tr>
                                            <td width="40%" align="right">
                                                <span class="required">* </span><b>Labor Date: </b>
                                            </td>
                                            <td align="left" width="125px">
                                                <asp:TextBox ID="txtLaberDate" runat="server" CssClass="textBox" Width="120px" TabIndex="1"></asp:TextBox>
                                                <cc1:CalendarExtender ID="txtLaberDate_CalendarExtender" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate" PopupPosition="BottomLeft" TargetControlID="txtLaberDate">
                                                </cc1:CalendarExtender>
                                                <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtLaberDate" WatermarkText="Date" />
                                            </td>
                                            <td align="left">
                                                <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/Images/calendar.gif" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <asp:Panel ID="pnlCrew" runat="server" Visible="true">
                                <tr>
                                    <td width="40%" align="right">
                                        <span class="required">* </span><b>Crew/Employee Name:</b>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="ddCrewMember" runat="server" CssClass="form-control" TabIndex="2"></asp:DropDownList>


                                    </td>

                                </tr>
                            </asp:Panel>
                       

                            <tr>
                                <td width="40%" align="right">
                                    <b>Customer Name: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtSearch" CssClass="form-control form-control-ext" runat="server" TabIndex="3" onkeypress="return searchKeyPress(event);" Width="200px"></asp:TextBox>
                                    <span class="input-group-btn">
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="noEffectNew"
                                            TabIndex="3" Text="" OnClick="btnSearch_Click"></asp:LinkButton>&nbsp;
                                          
                                            <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="btn btn-default"
                                                TabIndex="4" Text="New Lead" Visible="false">
                                            </asp:LinkButton>
                                    </span>
                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" DelimiterCharacters=""
                                        Enabled="True" TargetControlID="txtSearch" ServiceMethod="GetLastName" MinimumPrefixLength="1"
                                        CompletionSetCount="10" EnableCaching="true" CompletionInterval="500" OnClientItemSelected="selected_LastName"
                                        CompletionListCssClass="AutoExtender" UseContextKey="True">
                                    </cc1:AutoCompleteExtender>
                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch"
                                        WatermarkText="Search by Name" />
                                </td>
                            </tr>
                            <tr>
                                <td width="40%" align="right">
                                    <b>Section: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control" TabIndex="4"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>


                                <td colspan="2">
                                    <table width="100%">
                                        <tr>
                                            <td align="right" width="40%">
                                                <span class="required">* </span><b>Start Time: </b>
                                            </td>
                                            <td align="left" width="42px">
                                                <asp:TextBox ID="txtStartTime" TabIndex="5" runat="server" Width="42px"
                                                    MaxLength="8" Text="00:00"></asp:TextBox>

                                            </td>
                                            <td align="left" width="60px">
                                                <asp:DropDownList ID="ddlStartTime" TabIndex="6" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="1" Selected="True">AM</asp:ListItem>
                                                    <asp:ListItem Value="2">PM</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                                 <td align="left">
                                    <asp:Label ID="Label2" runat="server" Text="(ex 8:23)" Font-Bold="true" style="color:#666"></asp:Label>
                                </td>
                                        </tr>
                                    
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" width="40%">
                                    <b>Lunch: </b>
                                </td>
                                <td align="left" width="42px">
                                    <span style="padding: 5px; font-weight: bold; margin: 3px 0px 3px 0px">12:00 PM-12:30 PM</span>
                            </tr>

                            <tr>


                                <td colspan="2">
                                    <table width="100%">
                                        <tr>
                                            <td align="right" width="40%">
                                                <b>End Time: </b>
                                            </td>
                                            <td align="left" width="42px">
                                                <asp:TextBox ID="txtEndTime" TabIndex="7" runat="server" Width="42px"
                                                    MaxLength="8" Text="00:00"></asp:TextBox>

                                            </td>
                                            <td align="left" width="60px">
                                                <asp:DropDownList ID="ddEndTime" TabIndex="8" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="1" Selected="True">AM</asp:ListItem>
                                                    <asp:ListItem Value="2">PM</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="(ex 8:00)" Font-Bold="true" style="color:#666"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="right" width="40%">
                                    <b>Total: </b>
                                </td>
                                <td align="left" width="42px">
                                    <asp:Label ID="lblTotalTime" runat="server" Text="" Font-Bold="true"></asp:Label>
                            </tr>



                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Button ID="btnSubmit" runat="server"
                                        TabIndex="18" Text="Submit" CssClass="button" OnClick="btnSubmit_Click"
                                        Width="80px" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" TabIndex="19" Text="Cancel"
                                        CausesValidation="False" CssClass="button" OnClick="btnCancel_Click"
                                        Width="80px" />
                                    &nbsp;<asp:Button ID="btnDelete" runat="server" TabIndex="19" Text="Delete"
                                        CausesValidation="False" CssClass="button redColor" OnClick="btnDelete_Click" Width="80px" Visible="false" />
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnGpsId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnIsCrew" runat="server" Value="0" />
                                    &nbsp;</td>
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
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>

