<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customerlist.aspx.cs"
    Inherits="customerlist" Title="Customer List" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
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
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" style="background-color: #ddd; color: #fff;">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Customer List</span>
                                </td>
                                <td align="right" style="padding-right: 30px;">
                                    <asp:ImageButton ID="btnOperationCalendar" ToolTip="Go to Operation Calendar" CssClass="imageBtn" runat="server" ImageUrl="~/images/helmet_celendar.png" OnClick="btnOperationCalendar_Click" />
                                    <%--<asp:ImageButton ID="btnAddNew" ImageUrl="~/images/add_user.png" runat="server" CssClass="imageBtn" OnClick="btnAddNew_Click" ToolTip="Add New Customer" />--%>
                                    <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left" valign="middle">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td>&nbsp;</td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">First Name</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                                    <asp:ListItem Value="4">Address</asp:ListItem>
                                                    <asp:ListItem Value="3">Email</asp:ListItem>
                                                    <asp:ListItem Value="7">Phone</asp:ListItem>
                                                    <asp:ListItem Value="5">Job Number</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All Active</asp:LinkButton></td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="center" valign="middle">
                                    <table>
                                        <tr>
                                            <td><b>Page: </b></td>
                                            <td>
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#992a24"></asp:Label></td>
                                            <td><b>Item Per Page: </b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem Value="4">View All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="right" valign="middle">
                                    <table>
                                        <tr>
                                            <td>
                                                <b>Division:</b>                                    
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlDivision" runat="server" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </td>

                                            <td><b>Sales Person:</b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlSalesRep" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSalesRep_SelectedIndexChanged"></asp:DropDownList></td>
                                            <td><b>Status:</b></td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">All</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="2">Active</asp:ListItem>
                                                    <asp:ListItem Value="6">Sold</asp:ListItem>
                                                    <asp:ListItem Value="4">Archive</asp:ListItem>
                                                    <asp:ListItem Value="5">InActive</asp:ListItem>
                                                    <asp:ListItem Value="8">Est.InActive</asp:ListItem>
                                                    <asp:ListItem Value="7">Warranty Only</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton" OnClick="btnNext_Click"
                                        Text="Next" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdCustomerList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                            DataKeyNames="customer_id" OnPageIndexChanging="grdCustomerList_PageIndexChanging"
                            OnRowDataBound="grdCustomerList_RowDataBound" Width="100%" CssClass="mGrid">
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>
                                <asp:TemplateField HeaderText="Job #">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Job" runat="server" CssClass="mGrida2"></asp:HyperLink>
                                        <asp:Label ID="lblActiveEst" CssClass="paraMar" runat="server"></asp:Label>
                                        <asp:Label ID="lblOtherJob" CssClass="paraMar" runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" CssClass="jobNumberCL" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer Name">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Custd" runat="server" Text='<%# Eval("customer_name") %>' CssClass="mGrida2"></asp:HyperLink>
                                        <asp:Label ID="lblActiveCust" CssClass="paraMar" runat="server"></asp:Label>
                                        <br />
                                        <asp:HyperLink ID="hypCusLogin" runat="server" Target="_blank">Login</asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" CssClass="custNameCL" />
                                </asp:TemplateField>
                                <%--  <asp:HyperLinkField DataNavigateUrlFields="customer_id" DataNavigateUrlFormatString="customer_details.aspx?cid={0}" ControlStyle-CssClass="mGrida2"
                                    DataTextField="customer_name" HeaderText="Customer Name" ItemStyle-Width="0%">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:HyperLinkField>--%>
                                <asp:TemplateField HeaderText="Address">
                                    <ItemTemplate>

                                        <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                        <%-- <br />
                                        <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                        <br />
                                        <asp:HyperLink ID="hypEmail" runat="server" Target="_blank">[hypEmail]</asp:HyperLink>--%>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" CssClass="addressCL" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Projects">
                                    <ItemTemplate>
                                        <div style="width: 100%;">
                                            <div style="float: left; padding: 0px; vertical-align: middle;">
                                                <asp:DropDownList ID="ddlEst" runat="server" CssClass="autoWidth" AutoPostBack="True" OnSelectedIndexChanged="Load_Est_Info">
                                                </asp:DropDownList><br />
                                                <asp:Label ID="lblJobJost" CssClass="paraMar" Style="margin-left: 5px !important;" runat="server"></asp:Label>
                                            </div>
                                            <div class="divEstimate">
                                                <asp:HyperLink ID="hypEstDetail" ToolTip="View Details" runat="server" ImageUrl="~/images/view_details.png"></asp:HyperLink>

                                                <%--   <asp:HyperLink ID="hypCostLoc" ToolTip="Cost by Location" runat="server" ImageUrl="~/images/cbl.png"></asp:HyperLink>

                                                <asp:HyperLink ID="hypCostSec" ToolTip="Cost by Section" ImageUrl="~/images/cbs.png" runat="server"></asp:HyperLink>--%>

                                                <asp:HyperLink ID="hypCommon" ToolTip="New Estimate" runat="server" ImageUrl="~/images/new_estimate.png"></asp:HyperLink>

                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" CssClass="projectsCL" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Change Orders">
                                    <ItemTemplate>
                                        <div style="padding: 0px !important; margin: 0px !important;" width="100%">
                                            <div>
                                                <div style="vertical-align: middle !important; text-align: left !important; border: 0 none !important; padding: 1px;">
                                                    <ul style="float: left; list-style-type: none;">
                                                        <li style="padding: 0px; display: inline; vertical-align: top;">
                                                            <asp:DropDownList ID="ddlEstCO" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Load_COEst_Info">
                                                            </asp:DropDownList>
                                                        </li>
                                                    </ul>
                                                    <ul style="float: left; list-style-type: none;">
                                                        <li style="padding: 0px; display: inline; vertical-align: top;">
                                                            <asp:HyperLink ID="hypEstCODetail" runat="server" ImageUrl="~/images/view_details.png"></asp:HyperLink>
                                                        </li>
                                                        <li style="padding: 0px; display: inline; vertical-align: top;">
                                                            <asp:HyperLink ID="hypCOCommon" ToolTip="New Change Order" runat="server" ImageUrl="~/images/new_estimate.png"></asp:HyperLink>
                                                        </li>

                                                    </ul>
                                                </div>
                                            </div>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" CssClass="changeOrCL" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sales">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSales" runat="server"></asp:Label>
                                        <br />
                                        <asp:Label ID="lblSaleDate" runat="server"></asp:Label>
                                        <br />
                                        <asp:Label ID="lblDivision" runat="server"></asp:Label>
                                        <br />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" CssClass="salesCL" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tools">
                                    <ItemTemplate>
                                        <div style="width: 100%;">
                                            <div class="divToolsLeft">
                                                <ul style="list-style-type: none; margin: 0; padding: 0;">
                                                    <li>
                                                        <ul style="float: left;">
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hypMessage" runat="server"><img src="images/system_icons/01_icon.png" style="color:green;" alt="Message Center" title="Message Center" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hypCostLoc" runat="server"><img src="images/system_icons/16_icon.png" alt="Project Summary Report" title="Project Summary Report" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_CallLog" runat="server"><img src="images/system_icons/09_icon.png" alt="Activity Log" title="Activity Log" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_SMS" runat="server"><img src="images/system_icons/17_icon.png" height="37" style="cursor:pointer;" alt="SMS" title="SMS" /></asp:HyperLink></li>
                                                        </ul>
                                                        <ul style="float: left;">
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Schedule" runat="server"><img src="images/system_icons/05_icon.png" alt="Schedule" title="Schedule" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Sow" runat="server"><img src="images/system_icons/06_icon.png" alt="Composite SOW" title="Composite SOW" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_ProjectNotes" runat="server"><img src="images/system_icons/14_icon.png" alt="Project Notes" title="Project Notes" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Allowance" runat="server"><img src="images/system_icons/08_icon.png" alt="Allowance Report" title="Allowance Report" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_PreCon" runat="server"><img src="images/system_icons/10_icon.png" alt="Pre-Con Check List" title="Pre-Con Check List" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_SiteReview" runat="server"><img src="images/system_icons/11_icon.png" alt="Site Review" title="Site Review" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_DocumentManagement" runat="server"><img src="images/system_icons/12_icon.png" alt="Document Management" title="Document Management" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Section_Selection" runat="server"><img src="images/system_icons/15_icon.png" alt="Selection" title="Selection" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_MaterialTracking" runat="server"><img src="images/system_icons/19_icon.png" alt="Material Tracking" title="Material Tracking" /></asp:HyperLink></li>



                                                        </ul>
                                                    </li>
                                                </ul>
                                                <ul style="list-style-type: none; margin: 0; padding: 0;">
                                                    <li>
                                                        <ul style="float: left;">
                                                            <%--<li style="float: left;"></li>--%>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_vendor" runat="server"><img src="images/system_icons/02_icon.png" alt="Vendor Cost" title="Vendor Cost" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Payment" runat="server"><img src="images/system_icons/03_icon.png" alt="Payment Info" title="Payment Info" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hypWarrenty" runat="server"><img src="images/system_icons/18_icon.png" alt="Project completion & warranty certificate" title="Project Completion & Warranty Certificate" /></asp:HyperLink></li>

                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hypChangeOrderList" runat="server"><img src="images/system_icons/20_icon.png" alt="Change Order List" title="Change Order List" /></asp:HyperLink></li>
                                                    </li>


                                                    <li style="float: left;">
                                                        <asp:HyperLink ID="hyp_survey" runat="server"><img src="images/system_icons/07_icon.png" alt="Exit Questionnaire" title="Exit Questionnaire" /></asp:HyperLink></li>
                                                    <%--<asp:HyperLink ID="hyp_Selection" style="display:none;" runat="server"><img width="25" height="25" src="images/section_sheet.png" alt="Selection Sheet" title="Selection Sheet" /></asp:HyperLink>--%>
                                                    <li style="float: left;"></li>
                                                    <li style="float: left;">
                                                        <asp:HyperLink ID="hyp_jstatus" runat="server"><img src="images/system_icons/04_icon.png" alt="Job Status Graphics" title="Job Status Graphics" /></asp:HyperLink></li>

                                                </ul>
                                                </li>
                                                </ul>
                                            </div>

                                        </div>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle CssClass="toolsCL" />
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
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
                                                <asp:DropDownList ID="ddlSuperintendent" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlSuperintendent_SelectedIndexChanged"
                                                    Visible="false">
                                                </asp:DropDownList>
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
                    <td>
                        <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
        </Triggers>
    </asp:UpdatePanel>
    <div id="myModal" class="modal">

        <!-- Modal content -->
        <div class="modal-content" style="width: 300px; text-align: center;">

            <table style="padding: 0px; margin: 0px; width: 100%;">
                <tr>
                    <td style="padding: 0px; margin: 0px;" align="center" colspan="2" valign="top">
                        <b>This estimate is marked inactive.<br />
                            Estimate must be Active for it be Scheduled.</b>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 0px; margin: 0px;" align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td style="padding: 0px; margin: 0px;" align="center">
                        <input type="button" id="Close" class="button" value="Close" onclick="HidePopUp();" />
                    </td>

                </tr>
            </table>
        </div>

    </div>
    <script type="text/javascript">
        // Get the modal
        var modal = document.getElementById('myModal');

        // Get the <span> element that closes the modal
        // var span = document.getElementsByClassName("close")[0];

        function ShowPopUp(e) {
            //  alert(e);
            if (e == "False") {
                document.getElementById('myModal').style.display = "block";
                return false;
            }
            else {
                document.getElementById('myModal').style.display = "none";
                return true;
            }
        }

        function HidePopUp() {

            document.getElementById('myModal').style.display = "none";
        }

        // When the user clicks on <span> (x), close the modal
        //span.onclick = function () {
        //    modal.style.display = "none";
        //}

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == modal) {
                modal.style.display = "none";
            }
        }


        function IsSchedule(e) {

            //if (e == "False") {

            alert("This estimate is marked inactive." + '\n' + "Estimate must be Active for it be Scheduled.");
            return false;
            // }

        }
    </script>
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
