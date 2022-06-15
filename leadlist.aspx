<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="leadlist.aspx.cs"
    Inherits="leadlist" Title="Lead List" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
      <script type="text/javascript">
          $(document).ready(function () {


              $('.select2').select2();

          });
          Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
          function EndRequestHandler(sender, args) {

              $('.select2').select2();
          }

      </script>
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_Company(sender, e) {
            // alert("msg");
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
    
  <style>
      .select2-container .select2-selection--single .select2-selection__rendered {
          text-align:left!important;
      }
  </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" style="background-color: #eee; color: #fff; box-shadow: 0 0 3px #aaa;"> 
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Lead List</span></td>
                                <td align="right">
                                    <table style="padding-right: 30px;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnLeadReport" runat="server" CssClass="button" OnClick="btnLeadReport_Click" Text="Lead Report" /></td>
                                            <td>
                                                <asp:ImageButton ID="btnSalesCalendar" ImageUrl="~/images/calendar_money.png" runat="server" CssClass="imageBtn" OnClick="btnSalesCalendar_Click" ToolTip="Go to Sales Calendar" /></td>
                                            <td>
                                                <asp:ImageButton ID="btnAddNew" ImageUrl="~/images/add_user.png" runat="server" CssClass="imageBtn" OnClick="btnAddNew_Click" ToolTip="Add New Lead" /></td>
                                            <td>
                                                <asp:ImageButton ID="btnExpCustList" ImageUrl="~/images/export_csv.png" runat="server" CssClass="imageBtn" OnClick="btnExpCustList_Click" ToolTip="Export List to CSV" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left" valign="middle">
                                    <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                        <asp:ListItem Value="1">First Name</asp:ListItem>
                                        <asp:ListItem Value="6">Company</asp:ListItem>
                                        <asp:ListItem Value="4">Address</asp:ListItem>
                                        <asp:ListItem Value="3">Email</asp:ListItem>
                                         <asp:ListItem Value="7">Phone</asp:ListItem>

                                    </asp:DropDownList>

                                    <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_Company" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                    </cc1:AutoCompleteExtender>
                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />

                                    <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" />

                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All Active</asp:LinkButton>
                                </td>

                                <td align="center" valign="middle">
                                    <b>Page: </b>

                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                    &nbsp;
                                    <b>Item Per Page: </b>

                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">15</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="4">All</asp:ListItem>
                                    </asp:DropDownList>
                                </td>


                                <td align="right" valign="middle">
                                    <b>Division:</b>
                                    <asp:DropDownList ID="ddlDivision" runat="server" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <b>Sales Person:</b> 
                                    <asp:DropDownList ID="ddlSalesRep"  CssClass="select2" runat="server" AutoPostBack="True" width="168px" OnSelectedIndexChanged="ddlSalesRep_SelectedIndexChanged">
                                    </asp:DropDownList> 
                                    <b>Source:</b>
                                    <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="14"  AutoPostBack="True" OnSelectedIndexChanged="ddlLeadSource_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <b>Status:</b>
                                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="7">All</asp:ListItem>
                                        <asp:ListItem Value="6">Sold</asp:ListItem>
                                        <asp:ListItem Value="1">New</asp:ListItem>
                                        <asp:ListItem Value="2">Follow-up</asp:ListItem>
                                        <asp:ListItem Value="3">In-Design</asp:ListItem>
                                        <asp:ListItem Value="4">Archive</asp:ListItem>
                                        <asp:ListItem Value="5">Dead</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>&nbsp;</td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left" style="width: 324px">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton" OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td align="right" style="width: 87px">&nbsp;
                                </td>
                                <td align="right" style="width: 87px">&nbsp;
                                </td>

                                <td align="left" style="width: 245px">&nbsp;
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
                    <td align="center">
                        <asp:GridView ID="grdLeadList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" DataKeyNames="customer_id" OnPageIndexChanging="grdLeadList_PageIndexChanging" OnRowDataBound="grdLeadList_RowDataBound" Width="100%">
                            <PagerSettings Position="TopAndBottom" />
                            <Columns>
                                 <asp:TemplateField HeaderText="Customer Name">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp_Custd" runat="server" Text='<%# Eval("customer_name") %>' CssClass="mGrida2"></asp:HyperLink>                                      
                                        <asp:Label ID="lblActiveEst" runat="server"  Visible="false"></asp:Label>
                                         </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="7%"/>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                             
                                <asp:TemplateField HeaderText="Address, eMail, Phone">
                                    <ItemTemplate>
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="left" width="50%">
                                                    <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                                </td>
                                                <td align="left" width="50%">
                                                    <asp:HyperLink ID="hypEmail" runat="server" Target="_blank">[hypEmail]</asp:HyperLink>
                                                    <br />
                                                    <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sales Person">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSalesPerson" runat="server" Text=""></asp:Label><br />
                                        <asp:Label ID="lblDivision" runat="server" Text=""></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                              


                                <asp:TemplateField HeaderText="Projects">
                                    <ItemTemplate>
                                        <div style="width: 100%;">
                                            <div style="float: left; padding: 0px; vertical-align: middle;">
                                                <asp:DropDownList ID="ddlEst" runat="server" CssClass="iPadDD" Style="max-width: 150px;" AutoPostBack="True" OnSelectedIndexChanged="Load_Est_Info">
                                                </asp:DropDownList>
                                                <br />
                                                <asp:Label ID="lblJobJost" CssClass="paraMar" Style="margin-left: 5px !important;" runat="server"></asp:Label>
                                                <asp:HyperLink ID="hypCommon2" ToolTip="New Estimate" runat="server" ImageUrl="~/images/new_estimate.png"></asp:HyperLink>
                                            </div>
                                            <div class="divEstimate">
                                                <asp:HyperLink ID="hypEstDetail" ToolTip="View Details" runat="server" ImageUrl="~/images/view_details.png"></asp:HyperLink>
                                                <asp:HyperLink ID="hypCommon" ToolTip="New Estimate" runat="server" ImageUrl="~/images/new_estimate.png"></asp:HyperLink>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Latest Activity">
                                    <ItemTemplate>
                                        <asp:Label ID="lblActivity" runat="server" Style="display: inline;"></asp:Label>
                                        <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblActivity_r" runat="server" Visible="false" ></asp:Label></pre>
                                        <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="8%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <%-- <asp:BoundField DataField="notes" HeaderText="Notes">
                                    <HeaderStyle Width="8%" />
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                </asp:BoundField>--%>
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
                                                        </ul>
                                                        <ul style="float: left;">
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_ProjectNotes" runat="server"><img src="images/system_icons/14_icon.png" alt="Project Notes" title="Project Notes" /></asp:HyperLink></li>
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_Allowance" runat="server"><img src="images/system_icons/08_icon.png" alt="Allowance Report" title="Allowance Report" /></asp:HyperLink></li>
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
                                                            <li style="float: left;">
                                                                <asp:HyperLink ID="hyp_vendor" runat="server"><img src="images/system_icons/02_icon.png" alt="Vendor Cost" title="Vendor Cost" /></asp:HyperLink></li>
                                                        </ul>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="10%" />
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
                                                    Visible="False">
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
                        <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                        <asp:HiddenField ID="hdnDivisionName" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPrimaryDivision" runat="server" Value="0" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExpCustList" />
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
