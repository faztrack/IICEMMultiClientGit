<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="lead_details.aspx.cs" Inherits="lead_details" Title="Lead Information" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>

    <script language="JavaScript" type="text/JavaScript">

        function confirmDelete() {
            return confirm("Are you sure that you want to delete this Item?");
        }
        function DisplayWindow() {
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue(value) {
            window.open('replaymail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&emailId=' + value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue1(value) {
            window.open('messagedetailsoutlook.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue2(value) {
            window.open('DisplayPop3Email.aspx?emailId=' + value, 'MyWindow', 'left=200,top=100,width=850,height=550,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue3(value) {
            window.open('replayemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

        function GetdatakeyValue1Old(value) {
            window.open('messagedetails.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow1() {
            window.open('image_gallery.aspx?jsid=3&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=900,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>

    <script language="Javascript" type="text/javascript">

        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Add New Lead</asp:Label></span>
                                </td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="1200px" align="center">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                             <tr>
                                <td align="right" width="15%">
                                    <span class="required">* </span><b>First Name 1: </b>
                                </td>
                                <td align="left" width="30%">
                                    <asp:TextBox ID="txtFirstName1" runat="server" Width="200px" TabIndex="1" autocomplete="off"></asp:TextBox>
                                </td>
                                <td align="right" width="15%"><b>First Name 2: </b></td>
                                <td align="left" width="40%">
                                    <asp:TextBox ID="txtFirstName2" runat="server" TabIndex="16" Width="200px" autocomplete="off"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">* </span><b>Last Name 1: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtLastName1" runat="server" TabIndex="2" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right"><b>Last Name 2: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtLastName2" runat="server" Width="200px" TabIndex="17"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    <span class="required">* </span><b>Address: </b>
                                </td>
                                <td align="left" rowspan="2">
                                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Width="205px"
                                        TabIndex="3"></asp:TextBox>
                                </td>
                                <td align="right"><b>Company: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtCompany" runat="server" TabIndex="18" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    <asp:HyperLink ID="hypMap" runat="server" Target="_blank" Visible="False"
                                        ImageUrl="~/images/img_map.gif"></asp:HyperLink></td>
                                <td align="right"><b>Cross Street: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtCrossStreet" runat="server" TabIndex="19" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">* </span><b>City: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtCity" runat="server" TabIndex="4" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right"><b>Lead Status: </b></td>
                                <td align="left">
                                    <%-- <asp:DropDownList ID="ddlStatus" runat="server" TabIndex="20">
                                <asp:ListItem Value="1">New</asp:ListItem>
                                <asp:ListItem Value="2">Follow-up</asp:ListItem>
                                <asp:ListItem Value="3">In-Design</asp:ListItem>
                                <asp:ListItem Value="4">Archive</asp:ListItem>
                                <asp:ListItem Value="5">Dead</asp:ListItem>
                            </asp:DropDownList>--%>
                                    <asp:DropDownList ID="ddlLeadStatus" runat="server" TabIndex="20" Width="212px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>State: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5" Width="212px">
                                    </asp:DropDownList>
                                </td>
                                <td align="right"><b>Sales Person:</b> </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlSalesPerson" runat="server" TabIndex="22" Width="212px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">* </span><b>Zip Code: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtZipCode" runat="server" Width="200px" TabIndex="6"></asp:TextBox>
                                </td>
                                <td align="right"><span class="required">* </span><b>Lead Source:</b></td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="22" Width="212px">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Phone: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtPhone" runat="server" TabIndex="7" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <b>Appointment Date: </b></td>
                                <td align="left">
                                    <table cellpadding="0" cellspacing="0" class="tblAppDate">
                                        <tr>
                                            <td align="left" width="90px">
                                                <%--<asp:TextBox ID="txtAppointmentDate" runat="server" TabIndex="22" Width="110px"></asp:TextBox>--%>
                                                &nbsp;<asp:Label ID="lblAppointmentDate" Style="color: #555;" runat="server"></asp:Label>
                                            </td>
                                            <%-- <td align="left">
                                                <asp:ImageButton ID="imgAppointmentDate" runat="server" TabIndex="22"
                                                    CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" /></td>--%>
                                            <td align="left">
                                                <asp:ImageButton ID="btnSalesCalendar" runat="server" CssClass="nostyleCalImg" Height="30" ImageUrl="~/images/sales_calendar.png" OnClick="btnSalesCalendar_Click" ToolTip="Go to Sales Calendar" Width="27" />
                                                <%-- <cc1:CalendarExtender ID="appointmentdate" runat="server"
                                                    Format="MM/dd/yyyy" PopupButtonID="imgAppointmentDate"
                                                    PopupPosition="BottomLeft" TargetControlID="txtAppointmentDate">
                                                </cc1:CalendarExtender>--%>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><b>Mobile:</b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtMobile" runat="server" TabIndex="7" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right"><b>Appointment Time:</b></td>
                                <td align="left">
                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin-left: 2px;">
                                        <tr>
                                            <td><b>Start:</b></td>
                                            <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                <asp:Label runat="server" Style="color: #555;" ID="lblStartTime"></asp:Label>
                                                <%--  <div class="cbox">
                                                    <cc1:ComboBox ID="cmbStartTime" runat="server" AutoPostBack="true" DropDownStyle="Simple" TabIndex="22"
                                                        CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                        OnSelectedIndexChanged="cmbStartTime_SelectedIndexChanged"
                                                        AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px">
                                                        <asp:ListItem></asp:ListItem>
                                                    </cc1:ComboBox>
                                                </div>--%>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td><b>End:</b></td>
                                            <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                <asp:Label runat="server" Style="color: #555;" ID="lblEndTime"></asp:Label>
                                                <%--<div class="cbox">
                                                    <cc1:ComboBox ID="cmbEndTime" runat="server" AutoPostBack="true" DropDownStyle="Simple" TabIndex="22"
                                                        CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                        AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px">
                                                        <asp:ListItem></asp:ListItem>
                                                    </cc1:ComboBox>
                                                </div>--%>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Fax: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtFax" runat="server" TabIndex="8" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right">
                                    <asp:Label ID="lblRegDate" runat="server" ForeColor="#717171" Text="Entry Date:" Visible="False"></asp:Label>
                                </td>
                                <td align="left" style="padding-left: 2px; color: #000000;">
                                    <asp:Label ID="lblRegDateData" runat="server" Visible="False"></asp:Label>
                                    &nbsp;<asp:Label ID="lblCreatedBy" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required">* </span><b>Email: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtEmail" runat="server" TabIndex="9" Width="200px"></asp:TextBox>
                                </td>
                                <td align="right"><b>Website: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtWebsite" runat="server" TabIndex="19" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Email 2: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtEmail2" runat="server" TabIndex="9" Width="200px"></asp:TextBox>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDisplay" runat="server" BackColor="Transparent" BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" CssClass="nostyle" Font-Bold="True" ReadOnly="True">
                                    </asp:TextBox>
                                </td>
                                <td align="left">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:RadioButtonList ID="rdbEstimateIsActive" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem Selected="True" Text="Active" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="InActive" Value="0"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                            <td>(must SAVE)
                                            </td>
                                        </tr>
                                    </table>


                                </td>
                            </tr>

                            <tr>
                                <td align="right" valign="top">
                                    <asp:Label ID="Label4" runat="server">Notes:</asp:Label>
                                    <br />
                                    (Up to 500 Characters)
                                </td>
                                <td align="left" colspan="3">
                                    <asp:TextBox ID="txtNotes" runat="server" TabIndex="22" Height="60px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplay'));" TextMode="MultiLine" Width="100%"></asp:TextBox>

                                </td>
                            </tr>

                        </table>

                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2"></td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Button ID="btnSubmit" runat="server" Text="Save" TabIndex="23"
                            OnClick="btnSubmit_Click" Width="80px" CssClass="button" />
                        <asp:Button ID="btnCancel" runat="server" Text="Close" TabIndex="24"
                            OnClick="btnCancel_Click" Width="80px" CssClass="button" />
                        <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                         <asp:Button ID="btnUpdateLatlong" runat="server" Text="Update Lat long" TabIndex="24" Visible="false"
                            OnClick="btnUpdateLatlong_Click"  CssClass="button" />

                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <span id="spnPnlDocAttach" class="cssTitleHeader">
                                            <asp:ImageButton ID="ImageDocAttachMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="24" />
                                            <font style="font-size: 16px; cursor: pointer;">Documents & Attachments</font>
                                        </span>
                                    </span>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender5" runat="server" ImageControlID="ImageDocAttachMain" CollapseControlID="spnPnlDocAttach"
                                        ExpandControlID="spnPnlDocAttach" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGIDocAttachMain" Collapsed="True">
                                    </cc1:CollapsiblePanelExtender>
                                </td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlTGIDocAttachMain" class="wrapper" runat="server" Height="100%">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">
                                        <table style="padding: 0px; margin: 0px; width: 75%;" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="left">
                                                    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click"
                                                        Text="Document Manager" CssClass="button" OnClientClick="ShowProgress();" />
                                                    &nbsp;
                                                    <asp:LinkButton ID="btnImageGallery" runat="server"
                                                        Text="Image Gallery" CssClass="imgBtn" OnClientClick="DisplayWindow1();" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td>
                                        <div id="LoadingProgress" style="display: none;">
                                            <div id="divOverlay" class="overlay" />
                                            <div id="divOverlayContent" class="overlayContent">
                                                <p>
                                                    Please wait while your data is being processed
                                                </p>
                                                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>

                            </table>

                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdTemp" runat="server" AutoGenerateColumns="False"
                                                    OnRowDataBound="grdTemp_RowDataBound" class="mGrid" Width="314px" TabIndex="26">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="File Name">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hyp" runat="server">[hyp]</asp:HyperLink>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="File Description">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtDes" runat="server"
                                                                    Text='<%# Eval("file_description").ToString() %>' Width="221px"></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblMessage" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnSaveFiles" runat="server" OnClick="btnSaveFiles_Click" TabIndex="27"
                                                    Text="Save Uploaded Files" CssClass="button" Width="164px" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdLeadsFile" runat="server" AutoGenerateColumns="False" TabIndex="28"
                                                    CssClass="mGrid"
                                                    OnRowDataBound="grdLeadsFile_RowDataBound" Width="75%"
                                                    OnRowDeleting="grdLeadsFile_RowDeleting"
                                                    OnRowEditing="grdLeadsFile_RowEditing"
                                                    OnRowUpdating="grdLeadsFile_RowUpdating">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Description">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Desccription") %>'></asp:Label>
                                                                <asp:TextBox ID="txtDescription" runat="server" Height="40px"
                                                                    Text='<%# Eval("Desccription") %>' TextMode="MultiLine" Visible="false"
                                                                    Width="110px" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="ImageName" HeaderText="File Name" ReadOnly="true" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypView" runat="server" Style="cursor: pointer;">View</asp:HyperLink>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:ButtonField CommandName="Edit" Text="Edit" />
                                                        <%-- <asp:ButtonField CommandName="Delete" Text="Delete" />--%>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" OnClientClick="return confirmDelete();" CommandName="Delete" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </asp:Panel>

                    </td>
                </tr>



            </table>

            <table width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu"><span id="spnPnlCompMessage" class="cssTitleHeader">
                                    <asp:ImageButton ID="ImageCompMessageMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="outline: none; margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="40" />
                                    <font style="font-size: 16px; cursor: pointer;">Message Center</font></span></span>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender6" runat="server" CollapseControlID="spnPnlCompMessage" Collapsed="True" CollapsedImage="Images/expand.png" ExpandControlID="spnPnlCompMessage" ExpandedImage="Images/collapse.png" ImageControlID="ImageCompMessageMain" SuppressPostBack="true" TargetControlID="pnlTGICompMessageMain">
                                    </cc1:CollapsiblePanelExtender>
                                </td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlTGICompMessageMain" runat="server" Height="100%">
                            <table cellpadding="0" cellspacing="0" class="wrapper" width="100%">
                                <tr>
                                    <td align="left" colspan="4" class="wrapper">
                                        <table width="100%" style="margin: 0px; padding: 0px;">
                                            <tr>
                                                <td align="left">&nbsp;</td>
                                                <td align="right">
                                                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="hyp">Compose New Message</asp:HyperLink>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="trCustomersMessageBody" runat="server">
                                    <td align="center" colspan="2">
                                        <asp:GridView ID="grdCustomersMessage" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowDataBound="grdCustomersMessage_RowDataBound" PageSize="50" Width="100%">
                                            <Columns>
                                                <asp:BoundField DataField="create_date" DataFormatString="{0:d}" HeaderText="Date">
                                                    <ItemStyle HorizontalAlign="center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Type" HeaderText="Type" />
                                                <asp:BoundField DataField="From" HeaderText="From" />
                                                <asp:BoundField DataField="To" HeaderText="To" />
                                                <asp:BoundField DataField="mess_subject" HeaderText="Subject" />
                                                <asp:TemplateField HeaderText="Attachment">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypAttachment" runat="server" CssClass="hypg"></asp:HyperLink>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="sent_by" HeaderText="Sent by" />
                                                <asp:BoundField DataField="IsRead" HeaderText="Viewed?">
                                                    <ItemStyle HorizontalAlign="center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Protocol" HeaderText="Sent Via" />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypMessageDetails" runat="server" CssClass="btn_details">Details</asp:HyperLink>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr id="trEmailSentBody" runat="server">
                                    <td align="center" colspan="4">&nbsp;</td>
                                </tr>
                                <tr id="trEmailInboxHeadeer" runat="server">
                                    <td align="left" colspan="4" class="wrapper">&nbsp;</td>
                                </tr>
                                <tr id="trEmailInboxBody" runat="server">
                                    <td align="center" colspan="4">&nbsp;</td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <span id="spnPnlCallLog" class="cssTitleHeader">
                                            <asp:ImageButton ID="ImageCallLogMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="30" />
                                            <font style="font-size: 16px; cursor: pointer;">Activity log Information</font>
                                        </span>
                                    </span>
                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender3" runat="server" ImageControlID="ImageCallLogMain" CollapseControlID="spnPnlCallLog"
                                        ExpandControlID="spnPnlCallLog" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGICallLogMain" Collapsed="True">
                                    </cc1:CollapsiblePanelExtender>
                                </td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlTGICallLogMain" runat="server" Height="100%">
                            <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr style="padding: 0px; margin: 0px;">
                                                <td align="left">
                                                    <asp:HyperLink ID="HyperLink2" runat="server" CssClass="hyp">Compose New Message</asp:HyperLink>
                                                </td>
                                                <td style="padding: 0px; margin: 0px; text-align: right;">
                                                    <asp:Panel ID="Panel2" runat="server">
                                                        <span id="PnlCtrlID" runat="server">
                                                            <asp:LinkButton ID="lnkAddNewCall" runat="server" CssClass="button" Width="160px" OnClick="lnkAddNewCall_Click">
                                                                <asp:ImageButton ID="ImageTGI2" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="100" />
                                                                Add New Activity
                                                            </asp:LinkButton>
                                                        </span>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr style="padding: 0px; margin: 0px;">
                                                <td style="padding: 0px; margin: 0px;" align="center">
                                                    <asp:Panel ID="pnlTGI2" runat="server" Height="100%">
                                                        <table style="padding: 0px; margin: 0px;" width="100%">
                                                            <tr>
                                                                <td align="right">Type: </td>
                                                                <td align="left">
                                                                    <asp:DropDownList ID="ddlCallType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCallType_SelectedIndexChanged" TabIndex="101">
                                                                        <asp:ListItem Value="1">Called</asp:ListItem>
                                                                        <asp:ListItem Value="2">Pitched</asp:ListItem>
                                                                        <asp:ListItem Value="3">Booked</asp:ListItem>
                                                                        <asp:ListItem Value="6">Emailed</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <table id="tblApptDate" runat="server" visible="false">
                                                                        <tr>
                                                                            <td align="right"><span class="required">*</span>Appointment Date: </td>
                                                                            <td align="left">
                                                                                <table cellpadding="0" cellspacing="0" class="tblAppDate">
                                                                                    <tr>
                                                                                        <td align="left" width="120px">
                                                                                            <asp:TextBox ID="txtAppointmentDateC" runat="server" TabIndex="105" Width="110px"></asp:TextBox>
                                                                                        </td>
                                                                                        <td>&nbsp;</td>
                                                                                        <td align="left">
                                                                                            <asp:ImageButton ID="imgAppointmentDateC" runat="server" TabIndex="105"
                                                                                                CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" /></td>
                                                                                        <td>&nbsp;</td>
                                                                                        <td align="left">
                                                                                            <asp:ImageButton ID="btnSalesCalendarC" runat="server" TabIndex="105" CssClass="nostyleCalImg" Height="30" ImageUrl="~/images/sales_calendar.png" OnClick="btnSalesCalendarC_Click" ToolTip="Go to Sales Calendar" Width="27" />
                                                                                        </td>

                                                                                    </tr>
                                                                                </table>

                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="right"><span class="required">* </span>Subject: </td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtCallSubject" runat="server" TabIndex="102" Width="285px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <table id="tblApptTime" runat="server" visible="false">
                                                                        <tr>
                                                                            <td align="right">Appointment Time:</td>
                                                                            <td align="left">
                                                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin-left: 2px;">
                                                                                    <tr>
                                                                                        <td><b>Start:</b></td>
                                                                                        <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                                                            <div class="cbox">
                                                                                                <cc1:ComboBox ID="cmbStartTimec" runat="server" AutoPostBack="true" DropDownStyle="Simple"
                                                                                                    CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                                                                    OnSelectedIndexChanged="cmbStartTimec_SelectedIndexChanged"
                                                                                                    AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px" TabIndex="106">
                                                                                                    <asp:ListItem></asp:ListItem>
                                                                                                </cc1:ComboBox>
                                                                                            </div>
                                                                                        </td>
                                                                                        <td><b>End:</b></td>
                                                                                        <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                                                            <div class="cbox">
                                                                                                <cc1:ComboBox ID="cmbEndTimec" runat="server" AutoPostBack="true" DropDownStyle="Simple"
                                                                                                    CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                                                                    AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px" TabIndex="106">
                                                                                                    <asp:ListItem></asp:ListItem>
                                                                                                </cc1:ComboBox>
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="right"><span class="required">* </span>Start Date and Time:</td>
                                                                <td align="left">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="txtCallStartDate" runat="server" TabIndex="103" Width="68px"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:ImageButton ID="imgCallStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" TabIndex="103" />
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlCallHour" runat="server" TabIndex="103">
                                                                                    <asp:ListItem>00</asp:ListItem>
                                                                                    <asp:ListItem>01</asp:ListItem>
                                                                                    <asp:ListItem>02</asp:ListItem>
                                                                                    <asp:ListItem>03</asp:ListItem>
                                                                                    <asp:ListItem>04</asp:ListItem>
                                                                                    <asp:ListItem>05</asp:ListItem>
                                                                                    <asp:ListItem>06</asp:ListItem>
                                                                                    <asp:ListItem>07</asp:ListItem>
                                                                                    <asp:ListItem>08</asp:ListItem>
                                                                                    <asp:ListItem>09</asp:ListItem>
                                                                                    <asp:ListItem>10</asp:ListItem>
                                                                                    <asp:ListItem>11</asp:ListItem>
                                                                                    <asp:ListItem>12</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td>:</td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlCallMinutes" runat="server" TabIndex="103">
                                                                                    <asp:ListItem>00</asp:ListItem>
                                                                                    <asp:ListItem>01</asp:ListItem>
                                                                                    <asp:ListItem>02</asp:ListItem>
                                                                                    <asp:ListItem>03</asp:ListItem>
                                                                                    <asp:ListItem>04</asp:ListItem>
                                                                                    <asp:ListItem>05</asp:ListItem>
                                                                                    <asp:ListItem>06</asp:ListItem>
                                                                                    <asp:ListItem>07</asp:ListItem>
                                                                                    <asp:ListItem>08</asp:ListItem>
                                                                                    <asp:ListItem>09</asp:ListItem>
                                                                                    <asp:ListItem>10</asp:ListItem>
                                                                                    <asp:ListItem>11</asp:ListItem>
                                                                                    <asp:ListItem>12</asp:ListItem>
                                                                                    <asp:ListItem>13</asp:ListItem>
                                                                                    <asp:ListItem>14</asp:ListItem>
                                                                                    <asp:ListItem>15</asp:ListItem>
                                                                                    <asp:ListItem>16</asp:ListItem>
                                                                                    <asp:ListItem>17</asp:ListItem>
                                                                                    <asp:ListItem>18</asp:ListItem>
                                                                                    <asp:ListItem>19</asp:ListItem>
                                                                                    <asp:ListItem>20</asp:ListItem>
                                                                                    <asp:ListItem>21</asp:ListItem>
                                                                                    <asp:ListItem>22</asp:ListItem>
                                                                                    <asp:ListItem>23</asp:ListItem>
                                                                                    <asp:ListItem>24</asp:ListItem>
                                                                                    <asp:ListItem>25</asp:ListItem>
                                                                                    <asp:ListItem>26</asp:ListItem>
                                                                                    <asp:ListItem>27</asp:ListItem>
                                                                                    <asp:ListItem>28</asp:ListItem>
                                                                                    <asp:ListItem>29</asp:ListItem>
                                                                                    <asp:ListItem>30</asp:ListItem>
                                                                                    <asp:ListItem>31</asp:ListItem>
                                                                                    <asp:ListItem>32</asp:ListItem>
                                                                                    <asp:ListItem>33</asp:ListItem>
                                                                                    <asp:ListItem>34</asp:ListItem>
                                                                                    <asp:ListItem>35</asp:ListItem>
                                                                                    <asp:ListItem>36</asp:ListItem>
                                                                                    <asp:ListItem>37</asp:ListItem>
                                                                                    <asp:ListItem>38</asp:ListItem>
                                                                                    <asp:ListItem>39</asp:ListItem>
                                                                                    <asp:ListItem>40</asp:ListItem>
                                                                                    <asp:ListItem>41</asp:ListItem>
                                                                                    <asp:ListItem>42</asp:ListItem>
                                                                                    <asp:ListItem>43</asp:ListItem>
                                                                                    <asp:ListItem>44</asp:ListItem>
                                                                                    <asp:ListItem>45</asp:ListItem>
                                                                                    <asp:ListItem>46</asp:ListItem>
                                                                                    <asp:ListItem>47</asp:ListItem>
                                                                                    <asp:ListItem>48</asp:ListItem>
                                                                                    <asp:ListItem>49</asp:ListItem>
                                                                                    <asp:ListItem>50</asp:ListItem>
                                                                                    <asp:ListItem>51</asp:ListItem>
                                                                                    <asp:ListItem>52</asp:ListItem>
                                                                                    <asp:ListItem>53</asp:ListItem>
                                                                                    <asp:ListItem>54</asp:ListItem>
                                                                                    <asp:ListItem>55</asp:ListItem>
                                                                                    <asp:ListItem>56</asp:ListItem>
                                                                                    <asp:ListItem>57</asp:ListItem>
                                                                                    <asp:ListItem>58</asp:ListItem>
                                                                                    <asp:ListItem>59</asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td>

                                                                                <asp:DropDownList ID="ddlCallAMPM" runat="server" TabIndex="103">
                                                                                    <asp:ListItem>AM</asp:ListItem>
                                                                                    <asp:ListItem>PM</asp:ListItem>
                                                                                </asp:DropDownList>

                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td align="left">
                                                                    <table style="width: 400px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:CheckBox ID="chkFollowup" runat="server" Text="Followup" TextAlign="Left" TabIndex="107" AutoPostBack="True" OnCheckedChanged="chkFollowup_CheckedChanged" />
                                                                            </td>
                                                                            <td align="left">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td align="left">
                                                                                            <table id="tblFollowUp" runat="server" visible="false">
                                                                                                <tr>
                                                                                                    <td>
                                                                                                        <asp:TextBox ID="txtFollowupDate" runat="server" TabIndex="107" Width="68px"></asp:TextBox>
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <asp:ImageButton ID="imgFollowupDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <asp:DropDownList ID="ddlFollowHour" runat="server" TabIndex="107">
                                                                                                            <asp:ListItem>00</asp:ListItem>
                                                                                                            <asp:ListItem>01</asp:ListItem>
                                                                                                            <asp:ListItem>02</asp:ListItem>
                                                                                                            <asp:ListItem>03</asp:ListItem>
                                                                                                            <asp:ListItem>04</asp:ListItem>
                                                                                                            <asp:ListItem>05</asp:ListItem>
                                                                                                            <asp:ListItem>06</asp:ListItem>
                                                                                                            <asp:ListItem>07</asp:ListItem>
                                                                                                            <asp:ListItem>08</asp:ListItem>
                                                                                                            <asp:ListItem>09</asp:ListItem>
                                                                                                            <asp:ListItem>10</asp:ListItem>
                                                                                                            <asp:ListItem>11</asp:ListItem>
                                                                                                            <asp:ListItem>12</asp:ListItem>
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                    <td>:</td>
                                                                                                    <td>
                                                                                                        <asp:DropDownList ID="ddlFollowMin" runat="server" TabIndex="107">
                                                                                                            <asp:ListItem>00</asp:ListItem>
                                                                                                            <asp:ListItem>01</asp:ListItem>
                                                                                                            <asp:ListItem>02</asp:ListItem>
                                                                                                            <asp:ListItem>03</asp:ListItem>
                                                                                                            <asp:ListItem>04</asp:ListItem>
                                                                                                            <asp:ListItem>05</asp:ListItem>
                                                                                                            <asp:ListItem>06</asp:ListItem>
                                                                                                            <asp:ListItem>07</asp:ListItem>
                                                                                                            <asp:ListItem>08</asp:ListItem>
                                                                                                            <asp:ListItem>09</asp:ListItem>
                                                                                                            <asp:ListItem>10</asp:ListItem>
                                                                                                            <asp:ListItem>11</asp:ListItem>
                                                                                                            <asp:ListItem>12</asp:ListItem>
                                                                                                            <asp:ListItem>13</asp:ListItem>
                                                                                                            <asp:ListItem>14</asp:ListItem>
                                                                                                            <asp:ListItem>15</asp:ListItem>
                                                                                                            <asp:ListItem>16</asp:ListItem>
                                                                                                            <asp:ListItem>17</asp:ListItem>
                                                                                                            <asp:ListItem>18</asp:ListItem>
                                                                                                            <asp:ListItem>19</asp:ListItem>
                                                                                                            <asp:ListItem>20</asp:ListItem>
                                                                                                            <asp:ListItem>21</asp:ListItem>
                                                                                                            <asp:ListItem>22</asp:ListItem>
                                                                                                            <asp:ListItem>23</asp:ListItem>
                                                                                                            <asp:ListItem>24</asp:ListItem>
                                                                                                            <asp:ListItem>25</asp:ListItem>
                                                                                                            <asp:ListItem>26</asp:ListItem>
                                                                                                            <asp:ListItem>27</asp:ListItem>
                                                                                                            <asp:ListItem>28</asp:ListItem>
                                                                                                            <asp:ListItem>29</asp:ListItem>
                                                                                                            <asp:ListItem>30</asp:ListItem>
                                                                                                            <asp:ListItem>31</asp:ListItem>
                                                                                                            <asp:ListItem>32</asp:ListItem>
                                                                                                            <asp:ListItem>33</asp:ListItem>
                                                                                                            <asp:ListItem>34</asp:ListItem>
                                                                                                            <asp:ListItem>35</asp:ListItem>
                                                                                                            <asp:ListItem>36</asp:ListItem>
                                                                                                            <asp:ListItem>37</asp:ListItem>
                                                                                                            <asp:ListItem>38</asp:ListItem>
                                                                                                            <asp:ListItem>39</asp:ListItem>
                                                                                                            <asp:ListItem>40</asp:ListItem>
                                                                                                            <asp:ListItem>41</asp:ListItem>
                                                                                                            <asp:ListItem>42</asp:ListItem>
                                                                                                            <asp:ListItem>43</asp:ListItem>
                                                                                                            <asp:ListItem>44</asp:ListItem>
                                                                                                            <asp:ListItem>45</asp:ListItem>
                                                                                                            <asp:ListItem>46</asp:ListItem>
                                                                                                            <asp:ListItem>47</asp:ListItem>
                                                                                                            <asp:ListItem>48</asp:ListItem>
                                                                                                            <asp:ListItem>49</asp:ListItem>
                                                                                                            <asp:ListItem>50</asp:ListItem>
                                                                                                            <asp:ListItem>51</asp:ListItem>
                                                                                                            <asp:ListItem>52</asp:ListItem>
                                                                                                            <asp:ListItem>53</asp:ListItem>
                                                                                                            <asp:ListItem>54</asp:ListItem>
                                                                                                            <asp:ListItem>55</asp:ListItem>
                                                                                                            <asp:ListItem>56</asp:ListItem>
                                                                                                            <asp:ListItem>57</asp:ListItem>
                                                                                                            <asp:ListItem>58</asp:ListItem>
                                                                                                            <asp:ListItem>59</asp:ListItem>
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                    <td>

                                                                                                        <asp:DropDownList ID="ddlFollowAMPM" runat="server" TabIndex="107">
                                                                                                            <asp:ListItem>AM</asp:ListItem>
                                                                                                            <asp:ListItem>PM</asp:ListItem>
                                                                                                        </asp:DropDownList>

                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>

                                                            </tr>
                                                            <tr>
                                                                <td align="right">Duration:</td>
                                                                <td align="left">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="txtDurationH" runat="server" Width="62px" TabIndex="104"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlDurationMin" runat="server" TabIndex="104">
                                                                                    <asp:ListItem>00</asp:ListItem>
                                                                                    <asp:ListItem>01</asp:ListItem>
                                                                                    <asp:ListItem>02</asp:ListItem>
                                                                                    <asp:ListItem>03</asp:ListItem>
                                                                                    <asp:ListItem>04</asp:ListItem>
                                                                                    <asp:ListItem>05</asp:ListItem>
                                                                                    <asp:ListItem>06</asp:ListItem>
                                                                                    <asp:ListItem>07</asp:ListItem>
                                                                                    <asp:ListItem>08</asp:ListItem>
                                                                                    <asp:ListItem>09</asp:ListItem>
                                                                                    <asp:ListItem>10</asp:ListItem>
                                                                                    <asp:ListItem>11</asp:ListItem>
                                                                                    <asp:ListItem>12</asp:ListItem>
                                                                                    <asp:ListItem>13</asp:ListItem>
                                                                                    <asp:ListItem>14</asp:ListItem>
                                                                                    <asp:ListItem>15</asp:ListItem>
                                                                                    <asp:ListItem>16</asp:ListItem>
                                                                                    <asp:ListItem>17</asp:ListItem>
                                                                                    <asp:ListItem>18</asp:ListItem>
                                                                                    <asp:ListItem>19</asp:ListItem>
                                                                                    <asp:ListItem>20</asp:ListItem>
                                                                                    <asp:ListItem>21</asp:ListItem>
                                                                                    <asp:ListItem>22</asp:ListItem>
                                                                                    <asp:ListItem>23</asp:ListItem>
                                                                                    <asp:ListItem>24</asp:ListItem>
                                                                                    <asp:ListItem>25</asp:ListItem>
                                                                                    <asp:ListItem>26</asp:ListItem>
                                                                                    <asp:ListItem>27</asp:ListItem>
                                                                                    <asp:ListItem>28</asp:ListItem>
                                                                                    <asp:ListItem>29</asp:ListItem>
                                                                                    <asp:ListItem>30</asp:ListItem>
                                                                                    <asp:ListItem>31</asp:ListItem>
                                                                                    <asp:ListItem>32</asp:ListItem>
                                                                                    <asp:ListItem>33</asp:ListItem>
                                                                                    <asp:ListItem>34</asp:ListItem>
                                                                                    <asp:ListItem>35</asp:ListItem>
                                                                                    <asp:ListItem>36</asp:ListItem>
                                                                                    <asp:ListItem>37</asp:ListItem>
                                                                                    <asp:ListItem>38</asp:ListItem>
                                                                                    <asp:ListItem>39</asp:ListItem>
                                                                                    <asp:ListItem>40</asp:ListItem>
                                                                                    <asp:ListItem>41</asp:ListItem>
                                                                                    <asp:ListItem>42</asp:ListItem>
                                                                                    <asp:ListItem>43</asp:ListItem>
                                                                                    <asp:ListItem>44</asp:ListItem>
                                                                                    <asp:ListItem>45</asp:ListItem>
                                                                                    <asp:ListItem>46</asp:ListItem>
                                                                                    <asp:ListItem>47</asp:ListItem>
                                                                                    <asp:ListItem>48</asp:ListItem>
                                                                                    <asp:ListItem>49</asp:ListItem>
                                                                                    <asp:ListItem>50</asp:ListItem>
                                                                                    <asp:ListItem>51</asp:ListItem>
                                                                                    <asp:ListItem>52</asp:ListItem>
                                                                                    <asp:ListItem>53</asp:ListItem>
                                                                                    <asp:ListItem>54</asp:ListItem>
                                                                                    <asp:ListItem>55</asp:ListItem>
                                                                                    <asp:ListItem>56</asp:ListItem>
                                                                                    <asp:ListItem>57</asp:ListItem>
                                                                                    <asp:ListItem>58</asp:ListItem>
                                                                                    <asp:ListItem>59</asp:ListItem>
                                                                                </asp:DropDownList>

                                                                            </td>
                                                                            <td>(hours/minutes)
                                                                            </td>
                                                                        </tr>
                                                                    </table>

                                                                </td>
                                                                <td align="left">
                                                                    <table style="width: 400px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:CheckBox ID="ChkDoNotCall" runat="server" Text="Do Not Call" TabIndex="108" TextAlign="Left" AutoPostBack="True" OnCheckedChanged="chkFollowup_CheckedChanged" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <%--  <tr>
                                                            <td align="right">User:</td>
                                                            <td align="left">
                                                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>--%>
                                                            <tr>
                                                                <td align="right" valign="top">
                                                                    <asp:Label ID="Label2" runat="server"><span class="required">* </span> Notes:</asp:Label>
                                                                    <br />
                                                                    (Up to 500 Characters)
                                                                <br />
                                                                    <asp:TextBox ID="txtDisplayCall" runat="server" BackColor="Transparent" CssClass="nostyle"
                                                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                                                        ReadOnly="True">
                                                                    </asp:TextBox>
                                                                </td>
                                                                <td align="left" colspan="2">
                                                                    <asp:TextBox ID="txtCallDescription" runat="server" TabIndex="109" Height="60px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplayCall'));" TextMode="MultiLine" Width="680px"></asp:TextBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td align="right" valign="top">&nbsp;</td>
                                                                <td align="left">
                                                                    <asp:Label ID="lblResultCallLog" runat="server"></asp:Label>
                                                                </td>
                                                                <td></td>
                                                            </tr>

                                                            <tr>
                                                                <td align="right" valign="top">&nbsp;</td>
                                                                <td align="left">
                                                                    <asp:Button ID="btnSaveCall" runat="server" CssClass="button" TabIndex="110" Text=" Save Activity " OnClick="btnSaveCall_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>

                                        </table>

                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMessageGrdCallLog" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:GridView ID="grdCallLog" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowCommand="grdCallLog_RowCommand" OnRowDataBound="grdCallLog_RowDataBound" PageSize="50" Width="100%" TabIndex="111">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Subject">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallSubjectG" runat="server" Text='<%# Eval("CallSubject").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Notes">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallDescriptionG" runat="server" Text='<%# Eval("Description").ToString() %>' Style="display: inline;"></asp:Label>
                                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblCallDescriptionG_r" runat="server" Text='<%# Eval("Description") %>' Visible="false" ></asp:Label></pre>
                                                        <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="45%" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallType" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="13%" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Followup">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFollowup" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Date Last Called">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallStartDateTime" runat="server" Text='<%# Eval("CallDateTime").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="8%" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <%-- <asp:TemplateField HeaderText="Duration">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallDuration" runat="server" Text='<%# Eval("CallDuration").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>--%>
                                                <asp:ButtonField CommandName="Select" Text="Edit" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="9%"></asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnCallLogId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                        <asp:HiddenField ID="hdnCustEmail" runat="server" Value="" />
                                         <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                        <%--<cc1:CalendarExtender ID="appointmentdate" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgAppointmentDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtAppointmentDate">
                                        </cc1:CalendarExtender>--%>
                                        <cc1:CalendarExtender ID="CallDate" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgCallStartDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtCallStartDate">
                                        </cc1:CalendarExtender>
                                        <cc1:CalendarExtender ID="followDate" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgFollowupDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtFollowupDate">
                                        </cc1:CalendarExtender>
                                        <cc1:CalendarExtender ID="AppointmentDateC" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgAppointmentDateC"
                                            PopupPosition="BottomLeft" TargetControlID="txtAppointmentDateC">
                                        </cc1:CalendarExtender>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <span id="spnPnlContact" class="cssTitleHeader">
                                                <asp:ImageButton ID="ImageContactMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="112" />
                                                <font style="font-size: 16px; cursor: pointer;">Contact Information</font>
                                            </span>

                                        </span>
                                        <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server" ImageControlID="ImageContactMain" CollapseControlID="spnPnlContact"
                                            ExpandControlID="spnPnlContact" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGIContactMain" Collapsed="True">
                                        </cc1:CollapsiblePanelExtender>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlTGIContactMain" runat="server" Height="100%">
                            <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr style="padding: 0px; margin: 0px;">
                                                <td style="padding: 0px; margin: 0px; text-align: right;">
                                                    <asp:Panel ID="Panel1" runat="server">
                                                        <span id="PnlCtrlID1" runat="server">
                                                            <asp:LinkButton ID="lnkAddNewContact" runat="server" CssClass="button" Width="160px">
                                                                <asp:ImageButton ID="ImageTGI1" CssClass="blindInput" runat="server" ImageUrl="~/Images/expand.png" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="113" />
                                                                Add New Contact
                                                            </asp:LinkButton>
                                                        </span>
                                                    </asp:Panel>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server" ImageControlID="ImageTGI1" CollapseControlID="PnlCtrlID1"
                                                        ExpandControlID="PnlCtrlID1" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/expand.png" TargetControlID="pnlTGI1" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                </td>
                                            </tr>
                                            <tr style="padding: 0px; margin: 0px;">
                                                <td style="padding: 0px; margin: 0px;" align="center">
                                                    <asp:Panel ID="pnlTGI1" runat="server" Height="100%">
                                                        <table style="padding: 0px; margin: 0px;" width="70%">
                                                            <tr>
                                                                <td valign="top">
                                                                    <table style="padding: 0px; margin: 0px;" width="100%">
                                                                        <tr>
                                                                            <td align="right"><span class="required">* </span>First Name:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactFirstName" runat="server" Width="341px" TabIndex="301"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right"><span class="required">* </span>Last Name:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactLastName" runat="server" Width="341px" TabIndex="302"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">Title:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactTitle" runat="server" Width="341px" TabIndex="303"></asp:TextBox>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align="right" valign="top">&nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblResultContact" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Button ID="btnSaveContact" runat="server" CssClass="button" TabIndex="307" Text="Save Contact" OnClick="btnSaveContact_Click" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td valign="top">
                                                                    <table style="padding: 0px; margin: 0px;" width="100%">
                                                                        <tr>
                                                                            <td align="right">Email:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactEmail" runat="server" Width="341px" TabIndex="304"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">Phone:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactPhone" runat="server" Width="341px" TabIndex="305"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">Mobile:
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtContactMobile" runat="server" Width="341px" TabIndex="306"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                            <td>&nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                            <td>&nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                            <td>&nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                            <td>&nbsp;</td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMessageGrdContact" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:GridView ID="grdContact" runat="server" AutoGenerateColumns="False" CssClass="mGrid" PageSize="50" Width="100%" OnRowEditing="grdContact_RowEditing" TabIndex="318"
                                            OnRowUpdating="grdContact_RowUpdating">
                                            <Columns>
                                                <asp:TemplateField HeaderText="First Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFirstNameG" runat="server" Text='<%# Eval("FirstName").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtFirstNameG" runat="server" Text='<%# Eval("FirstName").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastNameG" runat="server" Text='<%# Eval("LastName").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtLastNameG" runat="server" Text='<%# Eval("LastName").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Title">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTitleG" runat="server" Text='<%# Eval("Title").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtTitleG" runat="server" Text='<%# Eval("Title").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmailG" runat="server" Text='<%# Eval("Email").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtEmailG" runat="server" Text='<%# Eval("Email").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Phone">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPhoneG" runat="server" Text='<%# Eval("Phone").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtPhoneG" runat="server" Text='<%# Eval("Phone").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mobile">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMobileG" runat="server" Text='<%# Eval("Mobile").ToString() %>'></asp:Label>
                                                        <asp:TextBox ID="txtMobileG" runat="server" Text='<%# Eval("Mobile").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="CreatedBy" HeaderText="User" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
                                                <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>

                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>

                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
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


