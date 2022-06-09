<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="selectionofsection.aspx.cs" Inherits="selectionofsection" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">

        function DisplayEmailWindow() {
            alert("bb");
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('<%= hdnCustomerID.ClientID%>').value + '&ssfn=a&eid=' + document.getElementById('<%= hdnEstimateID.ClientID%>').value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');

        }

        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function CheckAllSubmit(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdSelection.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[12].getElementsByTagName("INPUT")[0].checked != "undefined")
                    GridVwHeaderChckbox.rows[i].cells[12].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }

        function CheckUnselect(Checkbox) {
            var bFlag = false;
            var GridVwHeaderChckbox = document.getElementById("<%=grdSelection.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[12].getElementsByTagName("INPUT")[0].checked == false) {
                    bFlag = true;
                    break;
                }
            }
            if (bFlag) {
                GridVwHeaderChckbox.rows[0].cells[12].getElementsByTagName("INPUT")[0].checked = false;
            }
            else {
                GridVwHeaderChckbox.rows[0].cells[12].getElementsByTagName("INPUT")[0].checked = true;
            }
        }


        function CheckAllDeclinedSubmit(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=grdDeclinedSelection.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[11].getElementsByTagName("INPUT")[0].checked != "undefined")
                    GridVwHeaderChckbox.rows[i].cells[11].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }

        function CheckDeclinedUnselect(Checkbox) {
            var bFlag = false;
            var GridVwHeaderChckbox = document.getElementById("<%=grdDeclinedSelection.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[11].getElementsByTagName("INPUT")[0].checked == false) {
                    bFlag = true;
                    break;
                }
            }
            if (bFlag) {
                GridVwHeaderChckbox.rows[0].cells[11].getElementsByTagName("INPUT")[0].checked = false;
            }
            else {
                GridVwHeaderChckbox.rows[0].cells[11].getElementsByTagName("INPUT")[0].checked = true;
            }
        }
    </script>

    <style>
        .noteYellow {
            background-color: #ffd800;
            border-radius: 15px;
            color: #000;
            padding: 5px 20px;
            font-size: 12px;
            font-weight: 600;
        }
    </style>
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="cssHeader" align="center">
                <table cellpadding="0" cellspacing="0" width="100%" align="center">
                    <tr>
                        <td align="center" class="cssHeader">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Selection</asp:Label></span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label>
                                    </td>
                                    <td align="right" style="padding-right: 30px;">
                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="left" valign="middle">
                                                    <asp:LinkButton ID="btnBack" Font-Bold="true" ForeColor="#555555" runat="server"
                                                        Text="Return to Customer list" Visible="true" OnClick="btnBack_Click"></asp:LinkButton>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <uc1:ToolsMenu runat="server" id="ToolsMenu" />
                                                    
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
            <td>
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Panel ID="pnlTGICallLogMain" runat="server" Height="100%">
                                <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="lblSelectionMSG" runat="server" CssClass="noteYellow" Text="Note: Only Admin or Manager may edit or delete line items after a selection is approved by the customer"></asp:Label>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td align="left">Project:</td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlEst" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEst_SelectedIndexChanged">
                                                        </asp:DropDownList></td>
                                                    <td align="left">
                                                        <asp:Panel ID="Panel2" runat="server">
                                                            <span id="PnlCtrlID" runat="server">
                                                                <asp:LinkButton ID="lnkAddNewCall" Style="padding-left: 6px;" runat="server" CssClass="button" Width="160px">
                                                                    <asp:ImageButton ID="ImageTGI2" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="100" />
                                                                    Add New Choice
                                                                </asp:LinkButton>
                                                            </span>
                                                        </asp:Panel>


                                                    </td>



                                                    <td style="padding: 0px; margin: 0px; text-align: right;">
                                                        <asp:Button ID="btnSelectionApproved2" Text="Approve Selection" runat="server" CssClass="button" data-action="save1" OnClick="btnSubmitSelection_Click" />
                                                        <asp:Button ID="Button2" runat="server" CssClass="inputPDFBtn" data-action="save1" OnClick="Button2_Click" Text="Selection(s) Report" />
                                                        <asp:Button ID="Button1" Text="Email Selection(s) to the Customer" runat="server" CssClass="inputEmailBtn" data-action="save1" OnClick="btnEmailSelection_Click" />
                                                        <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" ImageControlID="ImageTGI2" CollapseControlID="lnkAddNewCall"
                                                            ExpandControlID="lnkAddNewCall" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGI2" Collapsed="True">
                                                        </cc1:CollapsiblePanelExtender>
                                                    </td>
                                                </tr>


                                            </table>
                                            <table cellpadding="0" cellspacing="0" width="50%">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="padding: 0px; margin: 0px;" align="center">
                                                        <asp:Panel ID="pnlTGI2" runat="server" Height="100%">
                                                            <table class="wrappermini" style="padding: 0px; margin: 0px;" width="100%">
                                                                <tr>
                                                                    <td align="right"><span class="required">* </span>Selection Date: </td>
                                                                    <td align="left" style="width: 400px">
                                                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                                            <tr>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtSelectionDate" runat="server"></asp:TextBox>
                                                                                </td>
                                                                                <td>&nbsp;</td>
                                                                                <td align="left">
                                                                                    <asp:ImageButton ID="imgSelectionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                    <cc1:CalendarExtender ID="extSelectionDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgSelectionDate" TargetControlID="txtSelectionDate">
                                                                                    </cc1:CalendarExtender>
                                                                                </td>
                                                                                <td>&nbsp;</td>
                                                                                <td align="right"><span class="required">* </span>Valid Till: </td>
                                                                                <td align="left">
                                                                                    <asp:TextBox ID="txtValidDate" runat="server"></asp:TextBox>
                                                                                </td>
                                                                                <td>&nbsp;</td>
                                                                                <td align="left">
                                                                                    <asp:ImageButton ID="imgValidDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                    <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgValidDate" TargetControlID="txtValidDate">
                                                                                    </cc1:CalendarExtender>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td align="right"><span class="required">* </span>Section: </td>
                                                                    <td align="left">
                                                                        <asp:DropDownList ID="ddlSection" runat="server" DataTextField="location_name" DataValueField="location_id">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td align="right"><span class="required">* </span>Location: </td>
                                                                    <td align="left">
                                                                        <table style="padding: 0px; margin: 0px; border: none;">
                                                                            <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:DropDownList ID="ddlLocation" runat="server" DataTextField="location_name" DataValueField="location_id">
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                                <td align="right" style="padding: 0px; margin: 0px; border: none;">Price: </td>
                                                                                <td align="left" style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:TextBox ID="txtPrice" runat="server"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td align="right"><span class="required">* </span>Title: </td>
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtTitle" runat="server" Style="width: 675px;"></asp:TextBox></td>

                                                                </tr>
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
                                                                    <td align="left">
                                                                        <asp:TextBox ID="txtNotes" runat="server" TabIndex="109" Height="60px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplayCall'));" TextMode="MultiLine" Width="680px"></asp:TextBox>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td align="right" valign="top">&nbsp;</td>
                                                                    <td align="left">
                                                                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" valign="top">&nbsp;</td>
                                                                    <td>
                                                                        <table style="padding: 0px; margin: 0px; border: none;">
                                                                            <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:FileUpload ID="file_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                                </td>
                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:ImageButton ID="imgbtnUpload" Height="24" Width="24" CssClass="nostyleCalImg" ToolTip="Upload New Files" runat="server" ImageUrl="~/images/upload_imag.png" OnClientClick="ShowProgress();" OnClick="btnUpload_Click" />

                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="right" valign="top">&nbsp;</td>
                                                                    <td align="left">
                                                                        <asp:Button ID="btnSave" runat="server" CssClass="button" TabIndex="110" Text=" Save " OnClick="btnSave_click" OnClientClick="ShowProgress();" />
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

                                            <asp:Label ID="lblResult2" runat="server"></asp:Label>

                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="center">
                                            <asp:GridView ID="grdSelection" runat="server" AutoGenerateColumns="False"
                                                CssClass="mGrid"
                                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdSelection_RowDataBound"
                                                OnRowEditing="grdSelection_RowEditing"
                                                OnRowUpdating="grdSelection_RowUpdating"
                                                OnRowCommand="grdSelection_RowCommand">
                                                <Columns>
                                                    <%-- Cell 0 --%>
                                                    <asp:TemplateField HeaderText="Project">
                                                        <ItemTemplate>
                                                            <%--   <asp:DropDownList ID="ddlEst" CssClass="secDD" runat="server" Enabled="false" DataValueField="estimate_id" DataTextField="estimate_name" DataSource="<%#dtProject%>" SelectedValue='<%#Eval("estimate_id") %>'>
                                                            </asp:DropDownList>--%>
                                                            <asp:Label ID="lblProject" runat="server" Text='<%# Eval("estimate_name")%>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 1 --%>
                                                    <asp:TemplateField HeaderText="Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("CreateDate","{0:d}")%>' />
                                                            <div id="dvCalender" runat="server" visible="false">
                                                                <table style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("CreateDate","{0:d}") %>' Width="65px"></asp:TextBox>
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

                                                    <%-- Cell 2 --%>
                                                    <asp:TemplateField HeaderText="Section">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddlSectiong" CssClass="secDD" runat="server" Enabled="false" DataValueField="section_id" DataTextField="section_name" DataSource="<%#dtSection%>" SelectedValue='<%#Eval("section_id") %>'>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="lblSectiong" runat="server" Text='<%# Eval("section_name") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 3 --%>
                                                    <asp:TemplateField HeaderText="Location">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="ddlLocation" CssClass="secDD" runat="server" Enabled="false" DataValueField="location_id" DataTextField="location_name" DataSource="<%#dtLocation%>" SelectedValue='<%#Eval("location_id") %>'>
                                                            </asp:DropDownList>
                                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("location_name") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 4 --%>
                                                    <asp:TemplateField HeaderText="Title">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' />
                                                            <asp:TextBox ID="txtTitle" runat="server" Visible="false" TextMode="MultiLine"
                                                                Text='<%# Eval("Title") %>' Width="98%" Height="40px"></asp:TextBox>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 5 --%>
                                                    <asp:TemplateField HeaderText="Notes">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Notes") %>' Style="display: inline;" />
                                                            <asp:TextBox ID="txtNotes" runat="server" Visible="false" TextMode="MultiLine"
                                                                Text='<%# Eval("Notes") %>' Width="98%" Height="40px"></asp:TextBox>

                                                            <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblNotes_r" runat="server" Text='<%# Eval("Notes") %>' Visible="false" ></asp:Label></pre>
                                                            <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 6 --%>
                                                    <asp:TemplateField HeaderText="Price">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price","{0:c}") %>' />
                                                            <asp:TextBox ID="txtPrice" runat="server" Visible="false" Style="text-align: right;"
                                                                Text='<%# Eval("Price","{0:c}") %>' Width="50px"></asp:TextBox>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle Width="5%" HorizontalAlign="Right" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 7 --%>
                                                    <asp:TemplateField HeaderText="Valid Till">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblVDate" runat="server" Text='<%# Eval("ValidTillDate","{0:d}")%>' />
                                                            <div id="dvValidCalender" runat="server" visible="false">
                                                                <table style="padding: 0px; margin: 0px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtVDate" runat="server" Text='<%# Eval("ValidTillDate","{0:d}") %>' Width="65px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:ImageButton CssClass="nostyleCalImg" ID="imgvDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                                            <cc1:CalendarExtender ID="extvDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgvDate" TargetControlID="txtVDate">
                                                                            </cc1:CalendarExtender>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                                        <ItemStyle HorizontalAlign="Center" Width="120px" />
                                                    </asp:TemplateField>
                                             
                                                    <%-- Cell 8 --%>
                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <table style="padding: 0px; margin: 0px; border: none;">
                                                                <tr style="padding: 0px; margin: 0px; border: none;">
                                                                    <td style="padding: 0px; margin: 0px; border: none;">

                                                                        <asp:GridView ID="grdUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                                            CssClass="uGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                                            OnRowDataBound="grdUploadedFileList_RowDataBound">
                                                                            <Columns>
                                                                                <asp:TemplateField>
                                                                                    <ItemTemplate>
                                                                                        <div class="clearfix">
                                                                                            <div class="divImageCss">
                                                                                                <asp:HyperLink ID="hypImg" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                    <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />
                                                                                                </asp:HyperLink>
                                                                                                <asp:HyperLink ID="hypPDF" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                    <asp:Image ID="imgPDF" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                </asp:HyperLink>
                                                                                                <asp:HyperLink ID="hypExcel" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                    <asp:Image ID="imgExcel" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                </asp:HyperLink>
                                                                                                <asp:HyperLink ID="hypDoc" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                    <asp:Image ID="imgDoc" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                </asp:HyperLink>
                                                                                                <asp:HyperLink ID="hypTXT" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                    <asp:Image ID="imgTXT" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                </asp:HyperLink>

                                                                                            </div>
                                                                                            <div style="text-align: center; padding: 5px; font-weight: bold;">
                                                                                                <asp:Label ID="lblFileName" runat="server" Text=""></asp:Label>
                                                                                                <asp:Button ID="btnDeleteUploadedFile" Text="Delete" runat="server" OnClick="DeleteUploadedFile" OnClientClick="ShowProgress();" />
                                                                                            </div>
                                                                                        </div>

                                                                                    </ItemTemplate>
                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                </asp:TemplateField>

                                                                            </Columns>
                                                                        </asp:GridView>

                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table style="padding: 0px; margin: 0px; border: none;">
                                                                            <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:FileUpload ID="grdfile_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                                </td>
                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                    <asp:ImageButton ID="imgbtngrdUpload" Height="24" Width="24" CssClass="nostyleCalImg" ToolTip="Upload New Files" runat="server" ImageUrl="~/images/upload_imag.png" OnClientClick="ShowProgress();" OnClick="imgbtngrdUpload_Click" />

                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="40%" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 9 --%>
                                                    <asp:ButtonField CommandName="Edit" Text="Edit" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center"></asp:ButtonField>

                                                    <%-- Cell 10 --%>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteSelection" OnClientClick="ShowProgress();" />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 11 --%>
                                                    <asp:TemplateField HeaderText="Selected">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelected" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblSelectionDate" runat="server"></asp:Label>
                                                            <asp:Image ID="imgCustomerSign" runat="server" Width="200px" Height="60px" Visible="false" />
                                                            <br></br>
                                                             <asp:Label ID="lblSignatureBy" runat="server" Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                    <%-- Cell 12 --%>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:CheckBox ID="chkAll" runat="server" TextAlign="Left" onClick="CheckAllSubmit(this)" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkSelected" runat="server" onClick="CheckUnselect(this)" />

                                                        </ItemTemplate>
                                                        <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                </Columns>
                                                <AlternatingRowStyle CssClass="alt" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                                <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td>
                                            <table width="100%">
                                                <tr>
                                                    <td align="center" class="cssHeader">
                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                            <tr>
                                                                <td align="left"><span class="titleNu"><span id="spnPnlSection" class="cssTitleHeader">
                                                                    <asp:ImageButton ID="ImageSectionMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="40" />
                                                                    <font style="font-size: 16px; cursor: pointer;">Declined Selection(s) </font>
                                                                    <asp:Label ID="lblDeclinedCount" Font-Size="12" runat="server"></asp:Label></span></span>
                                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender7" runat="server" CollapseControlID="spnPnlSection" Collapsed="True" CollapsedImage="Images/expand.png" ExpandControlID="spnPnlSection" ExpandedImage="Images/collapse.png" ImageControlID="ImageSectionMain" SuppressPostBack="true" TargetControlID="pnlSection">
                                                                    </cc1:CollapsiblePanelExtender>
                                                                </td>
                                                                <td align="right"></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="pnlSection" runat="server">
                                                            <asp:GridView ID="grdDeclinedSelection" runat="server" AutoGenerateColumns="False"
                                                                CssClass="mGrid"
                                                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdDeclinedSelection_RowDataBound"
                                                                OnRowEditing="grdDeclinedSelection_RowEditing"
                                                                OnRowUpdating="grdDeclinedSelection_RowUpdating"
                                                                OnRowCommand="grdDeclinedSelection_RowCommand">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Date">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("CreateDate","{0:d}")%>' />
                                                                            <div id="dvCalender" runat="server" visible="false">
                                                                                <table style="padding: 0px; margin: 0px;">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:TextBox ID="txtDate" runat="server" Text='<%# Eval("CreateDate","{0:d}") %>' Width="65px"></asp:TextBox>
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
                                                                            <asp:DropDownList ID="ddlSectiong" CssClass="secDD" runat="server" Enabled="false" DataValueField="section_id" DataTextField="section_name" DataSource="<%#dtSection%>" SelectedValue='<%#Eval("section_id") %>'>
                                                                            </asp:DropDownList>
                                                                            <asp:Label ID="lblSectiong" runat="server" Text='<%# Eval("section_name") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Location">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlLocation" CssClass="secDD" runat="server" Enabled="false" DataValueField="location_id" DataTextField="location_name" DataSource="<%#dtLocation%>" SelectedValue='<%#Eval("location_id") %>'>
                                                                            </asp:DropDownList>
                                                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("location_name") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderText="Title">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' />
                                                                            <asp:TextBox ID="txtTitle" runat="server" Visible="false" TextMode="MultiLine"
                                                                                Text='<%# Eval("Title") %>' Width="98%" Height="40px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Notes">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Notes") %>' Style="display: inline;" />
                                                                            <asp:TextBox ID="txtNotes" runat="server" Visible="false" TextMode="MultiLine"
                                                                                Text='<%# Eval("Notes") %>' Width="98%" Height="40px"></asp:TextBox>

                                                                            <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblNotes_r" runat="server" Text='<%# Eval("Notes") %>' Visible="false" ></asp:Label></pre>
                                                                            <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Price">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price","{0:c}") %>' />
                                                                            <asp:TextBox ID="txtPrice" runat="server" Visible="false" Style="text-align: right;"
                                                                                Text='<%# Eval("Price","{0:c}") %>' Width="50px"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle Width="5%" HorizontalAlign="Right" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Valid Till">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblVDate" runat="server" Text='<%# Eval("ValidTillDate","{0:d}")%>' />
                                                                            <div id="dvValidCalender" runat="server" visible="false">
                                                                                <table style="padding: 0px; margin: 0px;">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:TextBox ID="txtVDate" runat="server" Text='<%# Eval("ValidTillDate","{0:d}") %>' Width="65px"></asp:TextBox>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:ImageButton CssClass="nostyleCalImg" ID="imgvDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                                                            <cc1:CalendarExtender ID="extvDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgvDate" TargetControlID="txtVDate">
                                                                                            </cc1:CalendarExtender>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                                                        <ItemStyle HorizontalAlign="Center" Width="120px" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="">
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; border: none;">
                                                                                <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                    <td style="padding: 0px; margin: 0px; border: none;">

                                                                                        <asp:GridView ID="grdDeclinedUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                                                            CssClass="uGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                                                            OnRowDataBound="grdDeclinedUploadedFileList_RowDataBound">
                                                                                            <Columns>
                                                                                                <asp:TemplateField>
                                                                                                    <ItemTemplate>
                                                                                                        <div class="clearfix">
                                                                                                            <div class="divImageCss">
                                                                                                                <asp:HyperLink ID="hypImg" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                                    <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />
                                                                                                                </asp:HyperLink>
                                                                                                                <asp:HyperLink ID="hypPDF" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                                    <asp:Image ID="imgPDF" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                                </asp:HyperLink>
                                                                                                                <asp:HyperLink ID="hypExcel" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                                    <asp:Image ID="imgExcel" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                                </asp:HyperLink>
                                                                                                                <asp:HyperLink ID="hypDoc" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                                    <asp:Image ID="imgDoc" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                                </asp:HyperLink>
                                                                                                                <asp:HyperLink ID="hypTXT" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                                    <asp:Image ID="imgTXT" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                                </asp:HyperLink>

                                                                                                            </div>
                                                                                                            <div style="text-align: center; padding: 5px; font-weight: bold;">
                                                                                                                <asp:Label ID="lblFileName" runat="server" Text=""></asp:Label>
                                                                                                                <asp:Button ID="btnDeleteUploadedFile" Text="Delete" runat="server" OnClick="DeleteUploadedFile" OnClientClick="ShowProgress();" />
                                                                                                            </div>
                                                                                                        </div>

                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                                                </asp:TemplateField>

                                                                                            </Columns>
                                                                                        </asp:GridView>

                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <table style="padding: 0px; margin: 0px; border: none;">
                                                                                            <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                                    <asp:FileUpload ID="grdfile_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                                                </td>
                                                                                                <td style="padding: 0px; margin: 0px; border: none;">
                                                                                                    <asp:ImageButton ID="imgbtngrdUpload" Height="24" Width="24" CssClass="nostyleCalImg" ToolTip="Upload New Files" runat="server" ImageUrl="~/images/upload_imag.png" OnClientClick="ShowProgress();" OnClick="imgbtngrdUpload_Click" />

                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="35%" />
                                                                    </asp:TemplateField>
                                                                    <asp:ButtonField CommandName="Edit" Text="Edit" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center"></asp:ButtonField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteSelection" OnClientClick="ShowProgress();" />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderText="Declined">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblDeclined" runat="server"></asp:Label><br />
                                                                            <asp:Label ID="lblDeclinedDate" runat="server"></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField>
                                                                        <HeaderTemplate>
                                                                            <asp:CheckBox ID="chkAll" runat="server" TextAlign="Left" onClick="CheckAllDeclinedSubmit(this)" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelected" runat="server" onClick="CheckDeclinedUnselect(this)" />

                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </asp:Panel>
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
                        <td>
                            <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnSiteReviewId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">

                <asp:Button ID="btnEmailSelection" Text="Email Selection(s) to the Customer" runat="server" CssClass="inputEmailBtn" data-action="save1" OnClick="btnEmailSelection_Click" />
                <asp:Button ID="btnSelectionApproved" Text="Approve Selection" runat="server" CssClass="button" data-action="save1" OnClick="btnSubmitSelection_Click" />
            </td>
        </tr>
    </table>

    <div id="LoadingProgress" style="display: none">
        <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>
</asp:Content>

