<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Material_Tracking_List.aspx.cs" Inherits="Material_Tracking_List" %>

<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.0.30930.22922, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>
     <script language="javascript" type="text/javascript">
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
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left">
                            <span class="titleNu">
                                <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Material Tracking List</asp:Label>&nbsp;<asp:Label ID="lblProjectName" ForeColor="Blue" runat="server"></asp:Label></span>
                        </td>
                        <td align="right">
                            <%--  <asp:LinkButton ID="btnBack" Font-Bold="true" ForeColor="#555555" runat="server" Style="margin-right: 30px;"
                                Text="Return to Dashboard" Visible="true" OnClick="btnBack_Click"></asp:LinkButton>--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
              <ContentTemplate>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnPrevious" runat="server" Text="Previous"
                                OnClick="btnPrevious_Click" CssClass="prevButton" />
                        </td>
                        <td align="left" valign="middle">
                            <table>
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                            <asp:ListItem Value="1">First Name</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                            <asp:ListItem Value="4">Address</asp:ListItem>
                                            <asp:ListItem Value="3">Email</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                        </cc1:AutoCompleteExtender>
                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                       
                                    </td>
                                    <%--<td>
                                        <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                    <td>
                                        <asp:LinkButton ID="LinkButton2" runat="server" OnClick="lnkViewAll_Click">View All Active</asp:LinkButton></td>--%>
                                </tr>
                            </table>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                <tr>
                                    <td align="right" width="45%"><span class="required">* </span>
                                        <b>Start Order Date: </b>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtStartDate" runat="server" Width="120px"></asp:TextBox>
                                    </td>
                                    <td>&nbsp;</td>
                                    <td align="left">
                                        <cc1:CalendarExtender ID="startdate" runat="server"
                                            Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                        </cc1:CalendarExtender>
                                        <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="left" valign="middle">
                            <table>
                                <tr>
                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="right" width="45%"><span class="required">* </span>
                                                    <b>End Order Date: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtEndDate" runat="server" Width="120px"></asp:TextBox>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td align="left">
                                                    <cc1:CalendarExtender ID="EndDate" runat="server"
                                                        Format="MM/dd/yyyy" PopupButtonID="imgEndDate"
                                                        PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                    </cc1:CalendarExtender>
                                                    <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                  <%--  <td align="left">
                                        <asp:Button ID="btnSearchcalendar" runat="server" Text="View"
                                            OnClick="btnSearch_Click" CssClass="button" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                    </td>--%>
                                </tr>
                            </table>
                        </td>
                        <td align="center">
                            <b>Page: </b>
                            <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                            &nbsp;
                                    <b>Item per page: </b>
                            <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                <asp:ListItem Selected="True">20</asp:ListItem>
                                <asp:ListItem>30</asp:ListItem>
                                <asp:ListItem>40</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <%-- <asp:Button ID="btnAddNew" runat="server" Text="Add New Site Review" CssClass="button" OnClick="btnAddNew_Click" />--%>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnNext" runat="server" Text="Next"
                                OnClick="btnNext_Click" CssClass="nextButton" />
                        </td>
                    </tr>
                    
                    
                    
                    
                    
                    
                    <tr>
                        <td align="center" colspan="7" style="width: 100%;">
                            <table width="50%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="right"><asp:Label ID="Label8" runat="server" Font-Bold="true" Text="Vendor:"></asp:Label></td>
                                    <td align="left"><asp:DropDownList ID="ddlVendor" runat="server" DataTextField="Vendor_name" DataValueField="Vendor_id" AutoPostBack="true" OnSelectedIndexChanged="ddlVendor_SelectedIndexChanged"></asp:DropDownList></td>
                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="right">
                                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Status:"></asp:Label>&nbsp;
                                                </td>
                                                <td align="center">
                                                    <asp:Label ID="Label1" runat="server" Text="Shipped:"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkShipped" runat="server" OnCheckedChanged="chkShipped_CheckedChanged" AutoPostBack="true" /></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center">
                                                    <asp:Label ID="Label2" runat="server" Text="Received:"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkReceived" runat="server"  OnCheckedChanged="chkReceived_CheckedChanged" AutoPostBack="true"/></td>
                                            </tr>
                                        </table>
                                    </td>

                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center">
                                                    <asp:Label ID="Label3" runat="server" Text="Picked Up:"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkPicked" runat="server"   OnCheckedChanged="chkPicked_CheckedChanged" AutoPostBack="true"/></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center">
                                                    <asp:Label ID="Label5" runat="server" Text="Confirmed:"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkConfirmed" runat="server"  OnCheckedChanged="chkConfirmed_CheckedChanged" AutoPostBack="true" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                    <td align="left">
                                        <table cellpadding="5" cellspacing="5">
                                            <tr>
                                                 <td>
                                                <asp:LinkButton ID="LinkButton3" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="Count:"></asp:Label></td>
                                                <td>
                                                    <asp:Label ID="lblcount" runat="server" Text=""></asp:Label></td>
                                                
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left">&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    
                    
                    
                    

                    <tr>
                        <td align="center" colspan="4">
                            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="7">
                            <asp:GridView ID="grdSelection" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdSelection_RowDataBound" AllowPaging="True" OnPageIndexChanging="grdSelection_PageIndexChanging">
                                <PagerSettings Position="TopAndBottom" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Order Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Order_date","{0:d}")%>' />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                        <ItemStyle HorizontalAlign="Center" Width="120px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyp_Custd" runat="server" Text='<%# Eval("first_name1")+ " " +Eval("last_name1") %>' CssClass="mGrida2"></asp:HyperLink>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle CssClass="custNameCL" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Section">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSection" runat="server" Text='<%# Eval("Section_name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Vendor">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("Vendor_name") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Item">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Item_text") %>' />

                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Item_note") %>' Style="display: inline;" />
                                            <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblNotes_r" runat="server" Text='<%# Eval("Item_note") %>' Visible="false" ></asp:Label></pre>
                                            <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Price">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price","{0:c}") %>' />

                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="5%" HorizontalAlign="Right" />
                                    </asp:TemplateField>--%>
                                    <%--  <asp:TemplateField HeaderText="Days Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDayLeft" runat="server" Text='<%# Eval("ValidTillDate","{0:d}")%>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                </asp:TemplateField>--%>
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
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                        <ItemStyle Width="40%" />
                                        <HeaderStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelected" runat="server" />
                                            <asp:Label ID="lblSelected" runat="server"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblSelectionDate" runat="server"></asp:Label>
                                            <asp:Image ID="imgCustomerSign" runat="server" Width="200px" Height="60px" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>--%>
                                </Columns>
                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnPrevious0" runat="server"
                                Text="Previous" CssClass="prevButton"
                                OnClick="btnPrevious_Click" />
                        </td>
                        <td align="left" colspan="5">&nbsp;</td>
                        <td align="right">
                            <asp:Button ID="btnNext0" runat="server"
                                Text="Next" CssClass="nextButton"
                                OnClick="btnNext_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnBackId" runat="server" Value="0" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
           </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2"
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

