<%@ Page Title="Project Notes" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ProjectNotes.aspx.cs" Inherits="ProjectNotes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/calendar-blue.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>
    <%--  <Triggers>
            <asp:PostBackTrigger ControlID="grdProjectNote" />
        </Triggers>--%>
     <style>
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
    <script>
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">

                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Project Notes</span></td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <img src="images/icon-customer-info.png" /></td>
                                            <td align="left">
                                                <h2>Customer Information</h2>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 390px;" align="left" valign="top">
                                    <table style="width: 390px;">
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Customer Name: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Phone: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Email: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="top">
                                    <table style="width: 420px;">
                                        <tr>
                                            <td style="width: 64px;" align="left" valign="top"><b>Address: </b></td>
                                            <td style="width: auto;" align="left" valign="top">
                                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                    <table width="100%">
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <h2>&nbsp;</h2>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 390px;" align="left" valign="top">
                                    <table style="width: 390px;">
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Sales Person: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblSalesPerson" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Superintendent: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblSuperintendent" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="top">
                                    <table style="width: 420px;">
                                        <tr>
                                            <td style="width: 64px;" align="left" valign="top"></td>
                                            <td style="width: auto;" align="left" valign="top"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>

                            </tr>
                            <tr>
                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                    <table width="100%">
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td align="left">
                                                <h2>&nbsp;</h2>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="top" colspan="2">
                                    <table style="width: 810px;">
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top">
                                                <table style="padding: 0; margin: 0;">
                                                    <tr>
                                                        <td style="padding: 0; margin: 0; width: 124px;"><b>Additional email(s): </b></td>
                                                        <td align="left" style="padding: 0; margin: 0;">
                                                            <img src="images/tooltip.png" alt="Add additional emails with comma separation to be included in the project notes." title="Add additional emails with comma separation to be included in the project notes." /></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" colspan="2">
                                                            <asp:TextBox ID="txtDisplay" runat="server" CssClass="noEffectNew" BackColor="Transparent"
                                                                BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                                                Height="16px" ReadOnly="True"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td style="width: auto; padding: 8px 0 8px 0;" align="left" valign="top">

                                                <asp:Label ID="lblAdditionalEmail" runat="server" Style="max-width: 225px; display: block;"></asp:Label>
                                                <asp:TextBox ID="txtAdditionalEmail" runat="server" CssClass="textBox" TabIndex="1"
                                                    Width="500px" TextMode="MultiLine" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1000',document.getElementById('head_txtDisplay'));"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"></td>
                                            <td style="width: auto;">
                                                <span style="color: black;">(Example: john@domain.com, jane@domain.com)</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;<asp:LinkButton ID="lnkEditAddEmail" runat="server" Visible="false" OnClick="lnkEditAddEmail_Click">Edit Additional email</asp:LinkButton>
                                                &nbsp;
                                                <asp:LinkButton ID="lnkUpdateAddEmail" runat="server" Visible="false" OnClick="lnkUpdateAddEmail_Click">Update Additional email</asp:LinkButton>
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
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table width="100%">
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click" Text="Add New Notes" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnClose" runat="server" Text="Close" TabIndex="3" Width="80px"
                                                    OnClick="btnClose_Click" CssClass="button" />
                                            </td>
                                            <td align="right">
                                                <asp:ImageButton ID="imgSentEmail" runat="server" CssClass="noBorderCss" ImageUrl="~/assets/send_notes_button.png" OnClick="imgSentEmail_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:GridView ID="grdProjectNote" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowCommand="grdProjectNote_RowCommand" OnRowDataBound="grdProjectNote_RowDataBound" OnRowEditing="grdProjectNote_RowEditing" OnRowUpdating="grdProjectNote_RowUpdating" PageSize="200" TabIndex="2" Width="100%" AllowSorting="True" OnSorting="grdProjectNote_Sorting">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Date" SortExpression="ProjectDate" HeaderStyle-Font-Underline="true">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("ProjectDate","{0:d}")%>' />
                                                    <div id="dvCalender" runat="server" visible="false">
                                                        <%--  <asp:TextBox ID="txtDate" runat="server" class="Calender" Text='<%# Eval("ProjectDate","{0:d}") %>' Width="60px"></asp:TextBox>
                                                <img src="images/calendar.gif" />--%>
                                                        <table style="padding: 0px; margin: 0px;">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("ProjectDate","{0:d}") %>' Width="65px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:ImageButton CssClass="nostyleCalImg" ID="imgDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                                    <cc1:CalendarExtender ID="extDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgDate" TargetControlID="txtDate">
                                                                    </cc1:CalendarExtender>
                                                                </td>
                                                            </tr>
                                                        </table>


                                                    </div>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                                <ItemStyle HorizontalAlign="Center" Width="120px" />
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Section">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddlSectiong" CssClass="secDD" runat="server" DataValueField="section_id" DataTextField="section_name" DataSource="<%#dtSection%>" SelectedValue='<%#Eval("section_id") %>'>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="lblSection" runat="server" Text='<%# Eval("SectionName") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Material Track">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtMaterialTrack" runat="server" Height="80px" Text='<%# Eval("MaterialTrack") %>' Style="display: inline;" TextMode="MultiLine" Visible="false" Width="200px"></asp:TextBox>
                                                    <asp:Label ID="lblMaterialTrack" runat="server" Text='<%# Eval("MaterialTrack").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblMaterialTrack_r" runat="server" Text='<%# Eval("MaterialTrack") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenMaterialTrack" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenMaterialTrack_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="200px" />
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText=" Design Updates">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDesignUpdates" runat="server" Height="80px" Text='<%# Eval("DesignUpdates") %>' Style="display: inline;" TextMode="MultiLine" Visible="false" Width="200px"></asp:TextBox>
                                                    <asp:Label ID="lblDesignUpdates" runat="server" Text='<%# Eval("DesignUpdates").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDesignUpdates_r" runat="server" Text='<%# Eval("DesignUpdates") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenDesignUpdates" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenDesignUpdates_Click"  runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="200px" />
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Superintendent Notes">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtSuperintendentNotes" runat="server" Height="80px" Text='<%# Eval("SuperintendentNotes") %>' Style="display: inline;" TextMode="MultiLine" Visible="false" Width="200px"></asp:TextBox>
                                                    <asp:Label ID="lblSuperintendentNotes" runat="server" Text='<%# Eval("SuperintendentNotes").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblSuperintendentNotes_r" runat="server" Text='<%# Eval("SuperintendentNotes") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpenSuperintendentNotes" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more"  OnClick="lnkOpenSuperintendentNotes_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="200px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="General Notes">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDescription" runat="server" Height="80px" Text='<%# Eval("NoteDetails") %>' Style="display: inline;" TextMode="MultiLine" Visible="false" Width="200px"></asp:TextBox>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("NoteDetails").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"> <asp:Label ID="lblDescription_r" runat="server" Text='<%# Eval("NoteDetails") %>' Visible="false" /></pre>
                                                    <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" Width="200px" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Completed?" SortExpression="is_complete" HeaderStyle-Font-Underline="true">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkIsComplete" runat="server" AutoPostBack="True" Checked='<%# Eval("is_complete") %>' OnCheckedChanged="CheckUnCheckIsProcessed" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Include in SOW">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkSOW"  runat="server"  Checked='<%# Eval("isSOW") %>'     />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit" />
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center" height="10px">
                                    <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnAddEmailId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSalesEmail" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnProjectNotesEmail" runat="server" Value="" />

                                    <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="imgSentEmail"
                                        OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                                    </cc1:ConfirmButtonExtender>
                                    <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="imgSentEmail" BackgroundCssClass="modalBackground"
                                        CancelControlID="btnCancel" OkControlID="btnOK" PopupControlID="pnlConfirmation"
                                        runat="server">
                                    </cc1:ModalPopupExtender>
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <%--  <Triggers>
            <asp:PostBackTrigger ControlID="grdProjectNote" />
        </Triggers>--%>
    </asp:UpdatePanel>
    <asp:Panel ID="pnlConfirmation" runat="server" Width="550px" Height="100px" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                    <tr>
                        <td align="right">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>Please click OK to send the Project Notes email.</b>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnOk" runat="server" Text="OK" CssClass="button" Width="60px" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1"
        DynamicLayout="False">
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


