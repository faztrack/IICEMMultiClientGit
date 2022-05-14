<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mlandingpage.aspx.cs" Inherits="mlandingpage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .red {
            color: red;
        }

        .mobileDropDown {
            border: 2px solid #992a24;
            -webkit-box-shadow: 0 0 8px #aaa inset;
            box-shadow: 0 0 8px #888 inset;
            font-size: 12px;
            font-weight: bold;
            color: #992a24 !important;
            border-radius: 4px !important;
            -webkit-border-radius: 3px;
            -moz-border-radius: 3px;
            display: block;
            width: 100%;
            height: 28px;
            padding: 6px;
            margin: 14px 0;
            line-height: 1.42857143;
            background-color: #fff;
            background-image: none;
        }

        .mobileTextFiled {
            border: 2px solid #992a24;
            -webkit-box-shadow: 0 0 8px #aaa inset;
            box-shadow: 0 0 8px #888 inset;
            font-weight: bold;
            color: #992a24 !important;
            border-radius: 4px !important;
            -webkit-border-radius: 3px;
            -moz-border-radius: 3px;
            display: block;
            width: 100%;
            padding: 6px;
            line-height: 1.42857143;
            background-color: #fff;
            background-image: none;
        }
    </style>
    <script>

        function openModal() {
            $('#leadsNoteModal').modal({ show: true });
        }
        function openEnroutModal() {
            $('#EnroutMap').modal({ show: true });
        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_Customer(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }


    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">
                    <div class="panel panel-default">

                        <div class="panel-body">

                            <div class="row">
                                <div class="col-lg-12 col-sm-6 col-md-12">
                                    <div class="clearfixM">
                                        <div class="row">
                                            <asp:Panel ID="pnlTimeClock" runat="server" Visible="false" Style="margin-bottom: 10px;">
                                                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                    <div class="divImageCssM" style="text-align: center; width: 90px">
                                                        <asp:HyperLink ID="lnkTimeClock" runat="server" CssClass="hypimgCss" ToolTip="Time Clock" NavigateUrl="~/mtimeclock.aspx">
                                                            <asp:Image ID="imgTimeClock" ImageUrl="~/assets/mobileicons/15-time.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                            <asp:Label ID="lblClock" runat="server" Font-Bold="true" Text="Time Clock"></asp:Label>

                                                        </asp:HyperLink>

                                                    </div>

                                                </div>
                                            </asp:Panel>
                                            <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                <div class="divImageCssM" style="text-align: center; width: 90px">
                                                    <asp:HyperLink ID="inkMyJobs" runat="server" ToolTip="My Jobs" CssClass="hypimgCss" NavigateUrl="~/mmyjobs.aspx">
                                                        <asp:Image ID="Image1" ImageUrl="~/assets/mobileicons/16-my-jobs.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                        <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="My Jobs"></asp:Label>

                                                    </asp:HyperLink>

                                                </div>

                                            </div>
                                            <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                <div class="divImageCssM" style="text-align: center; width: 90px">
                                                    <asp:HyperLink ID="InkSchedule" runat="server" ToolTip="Schedule" CssClass="hypimgCss" NavigateUrl="~/mcrewschedulecalendar.aspx">
                                                        <asp:Image ID="Image2" ImageUrl="~/assets/mobileicons/05-Schedule.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                        <asp:Label ID="Label7" runat="server" Font-Bold="true" Text="Schedule"></asp:Label>

                                                    </asp:HyperLink>

                                                </div>

                                            </div>
                                            <asp:Panel ID="PanelMap" runat="server" Visible="false" Style="margin-bottom: 10px;">
                                                <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                    <div class="divImageCssM" style="text-align: center; width: 90px">
                                                        <asp:HyperLink ID="hypMap" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Map" NavigateUrl="~/joblocation.aspx">
                                                            <asp:Image ID="Image5" ImageUrl="~/assets/mobileicons/18-Job-Location.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                            <asp:Label ID="LabelMap" runat="server" Font-Bold="true" Text="Job Locations"></asp:Label>

                                                        </asp:HyperLink>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                        </div>
                                        <div class="row">
                                            <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                <div class="divImageCssM" style="text-align: center; margin-left: 5px; width: 90px">
                                                    <asp:HyperLink ID="hyp_Leads" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Leads" NavigateUrl="~/mcustomerlist.aspx" Visible="false">
                                                        <asp:Image ID="Image3" ImageUrl="~/assets/mobileicons/14-Leads.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                        <asp:Label ID="Label10" runat="server" Font-Bold="true" Text="Leads"></asp:Label>
                                                    </asp:HyperLink>

                                                </div>

                                            </div>
                                            <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                <div class="divImageCssM" style="text-align: center; width: 90px">
                                                    <asp:HyperLink ID="hypVendorList" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Vendors" NavigateUrl="~/mvendorlist.aspx" Visible="false">
                                                        <asp:Image ID="Image4" ImageUrl="~/assets/mobileicons/17-vendor-list.png" runat="server" CssClass="imgCssM blindInput" /><br />
                                                        <asp:Label ID="Label11" runat="server" Font-Bold="true" Text="Vendors"></asp:Label>
                                                    </asp:HyperLink>

                                                </div>

                                            </div>
                                                 <div class="col-xs-4 col-sm-4 col-md-4 col-lg-2">
                                                <div class="divImageCssM" style="text-align: center; width: 90px">
                                                    <asp:HyperLink ID="hypConfirmTomorrow" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Confirm Tomorrow" NavigateUrl="~/confirmtomorrow.aspx" Visible="false">
                                                        <table align="center">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Image ID="ImgConfirmTomorrow" ImageUrl="~/assets/mobileicons/20_ConfirmTomorrow.png" runat="server" CssClass="imgCssM blindInput" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblConfirmTomorrow" runat="server" Font-Bold="true" Text="Confirm Tomorrow"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:HyperLink>
                                                </div>

                                            </div>
                                        </div>
                                     
                                    </div>

                                </div>
                            </div>


                            <div class="row">
                                <div class="ol-lg-12  col-sm-12" style="margin-top: 10px">
                                    <asp:Panel ID="pnlSearchCustomer" runat="server">
                                        <table width="100%">

                                            <tr>
                                                <td colspan="3" width="100%" style="text-align: center">
                                                    <asp:Label ID="Label2" runat="server" Text="Previously Searched Customers:" ForeColor="Black" Font-Bold="true" Font-Size="Large"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td width="10%">
                                                    <asp:LinkButton ID="btnPrevious" runat="server" CssClass="faArrows" OnClick="btnPrevious_Click"><span class="glyphicon glyphicon-chevron-left"></span></asp:LinkButton>

                                                </td>
                                                <td width="80%">

                                                    <asp:GridView ID="grdCustomerList" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                                        CssClass="uGrid uGridCustom" OnRowDataBound="grdCustomerList_RowDataBound" Width="100%"
                                                        OnPageIndexChanging="grdCustomerList_PageIndexChanging" AllowPaging="True" PageSize="4" ShowFooter="false">
                                                        <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" Visible="false" />
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>

                                                                    <div class="SearchCustomerclearfix">
                                                                        <div>

                                                                            <asp:LinkButton ID="InkCustomerName" CssClass="custName1" runat="server" OnClick="GetCustomer"></asp:LinkButton>
                                                                        </div>


                                                                    </div>

                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="" />
                                                        <AlternatingRowStyle CssClass="" />
                                                    </asp:GridView>

                                                </td>

                                                <td width="10%">
                                                    <asp:LinkButton ID="btnNext" runat="server" CssClass="faArrows" OnClick="btnNext_Click"><span class="glyphicon glyphicon-chevron-right"></span></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </div>
                            </div>

                            <!-- Leads Notes View Modal content-->
                            <div id="leadsNoteModal" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false" style="z-index: 9999999">
                                <div class="modal-dialog">

                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                                            <h4 class="modal-title">Lead Notes<asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label></h4>
                                        </div>
                                        <div class="modal-body">



                                            <div class="form-horizontal">
                                                <div class="form-group form-group-ext-txtArea">
                                                    <asp:Label ID="Label9" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label"><span style="font-weight:bold;margin:0px;padding:0px">Notes:</span>
                                      

                                                    </asp:Label>
                                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea col-sm-offset-2">
                                                        <asp:Label ID="lblLeadsNote" runat="server" Text=""></asp:Label>

                                                    </div>
                                                </div>

                                            </div>
                                        </div>

                                    </div>

                                </div>
                            </div>

                            <%--       End Lead Note--%>
                                     <%--       End Lead Note--%>

                            <!-- Enrout Map Modal content-->
                            <div id="EnroutMap" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false" style="z-index: 9999999">
                                <div class="modal-dialog">

                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                                            <h4 class="modal-title">Confirm<asp:Label ID="Label12" runat="server" Text=""></asp:Label></h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="form-horizontal">
                                                <div class="form-group">
                                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea col-sm-offset-2">
                                                        <asp:Label ID="lblMessage" runat="server" Text="" Font-Bold="true" Font-Size="14px"></asp:Label>

                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea col-sm-offset-2">
                                                        <asp:Label ID="lblCustomerFullName" runat="server" Text="" Font-Size="14px"></asp:Label>

                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea col-sm-offset-2">
                                                        <asp:Label ID="lblCustomerAddress" runat="server" Text="" Font-Size="14px"></asp:Label>

                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea col-sm-offset-4 col-xs-offset-4">

                                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-info" Text="Yes" OnClick="btnSubmit_Click" />
                                                        &nbsp;&nbsp;&nbsp;
                                                        <button type="button" class="btn btn-primary color-blue" data-dismiss="modal">No</button>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                </div>
                            </div>

                            <%--       End Enrout Map--%>


                            <div class="row">


                                <div class="col-lg-12 col-sm-6 col-md-12">

                                    <asp:DropDownList ID="ddCustomer" runat="server" Font-Size="16px" Height="45px" AutoPostBack="true" OnSelectedIndexChanged="ddCustomer_SelectedIndexChanged" CssClass="mobileDropDown"></asp:DropDownList>

                                </div>
                                <div class="col-lg-12 col-sm-6 col-md-12">


                                    <div class="input-group custom-search-form">

                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-lg mobileTextFiled" Font-Size="18px"></asp:TextBox>
                                        <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" DelimiterCharacters=""
                                            Enabled="True" TargetControlID="txtSearch" ServiceMethod="GetCustomerName" MinimumPrefixLength="1"
                                            CompletionSetCount="10" EnableCaching="true" CompletionInterval="500" OnClientItemSelected="selected_Customer"
                                            CompletionListCssClass="AutoExtender" UseContextKey="True">
                                        </cc1:AutoCompleteExtender>
                                        <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtSearch" WatermarkText="Search by customer name" />

                                        <span class="input-group-btn" style="padding-top: 4px;">
                                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default btn-lg" OnClick="btnSearch_Click"><i class="fa fa-search"></i></asp:LinkButton>
                                        </span>

                                    </div>
                                    <asp:Label ID="lblResultMSG" runat="server" Text=""></asp:Label>
                                </div>


                                <div class="ol-lg-12  col-sm-12">

                                    <table style="width: 100%;">


                                        <tr>
                                            <td colspan="3">&nbsp;</td>
                                        </tr>
                                        <asp:Panel ID="pnlCustomerFullName" runat="server" Visible="false">
                                            <tr>
                                                <td style="border: 1px solid #dddddd; background: #e9e9e9; padding: 5px 0px 5px 0px">
                                                    <table>

                                                        <tr>

                                                            <td>

                                                                <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Customer Name:</span>
                                                                <asp:Label ID="lblSearchCustomerName" runat="server" Text="" Font-Size="16px"></asp:Label>

                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td>

                                                                <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Phone:</span>
                                                                <a runat="server" id="hrfCustomerPhone">
                                                                    <asp:Label ID="lblCustomerPhone" runat="server" Text="" Font-Size="16px"></asp:Label>
                                                                </a>
                                                                <a runat="server" id="hrfCustomerMobile">
                                                                    <asp:Label ID="lblCustomerMobile" runat="server" Text="" Font-Size="16px" Visible="false"></asp:Label>
                                                                </a>

                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td colspan="3">&nbsp;
                                                    <asp:Panel ID="pnlLeadNotes" runat="server" CssClass="leadtnoteicon">

                                                        <span style="font-size: 16px; font-weight: bold; padding-left: 5px; padding-top: 10px;">Lead Notes:
                                                            <asp:Label ID="lblCustomerLeadNotes" runat="server" Text="" Font-Bold="false"></asp:Label></span>
                                                        <asp:LinkButton ID="inkLeadnoteView" runat="server" OnClick="inkLeadnoteView_Click" Text="More" ForeColor="Blue" Font-Bold="true" Font-Size="18px" Style="text-decoration: underline">
                                                           
                                                        </asp:LinkButton>

                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>

                                                <td colspan="3">

                                                    <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Address:</span>
                                                    <asp:Label ID="lblAddress" runat="server" Text="" Style="font-size: 16px"></asp:Label>
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="left" style="width: 10%">
                                                                <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Superintendent:&nbsp; </span>

                                                            </td>
                                                            <td align="left">
                                                                <asp:Label ID="lblSuperintendent" runat="server" Text="" Style="font-size: 16px"></asp:Label>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <span style="font-size: 16px; font-weight: bold; padding-left: 5px">Phone:&nbsp; </span>

                                                            </td>

                                                            <td align="left">
                                                                <a runat="server" id="hyPhone">
                                                                    <asp:Label ID="lblPhone" runat="server" Text="" Style="font-size: 16px"></asp:Label></a>

                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                        </asp:Panel>
                                        <tr>
                                            <td colspan="3">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:Label ID="lblInitial" runat="server" Text="Initially, the Icons will not be shown until a customer is searched." Visible="false"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">

                                                <asp:Label ID="lblSearhText" runat="server" Text="Search for a customer above" Font-Bold="true" Font-Size="Large" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>

                                                <asp:GridView ID="grdIcon" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                                    CssClass="uGrid" OnRowDataBound="grdIcon_RowDataBound" Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>

                                                                <div class="clearfixM">
                                                                    <div class="row">
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">
                                                                                <asp:HyperLink ID="hypSiteReview" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Site Review">
                                                                                    <asp:Image ID="imgSiteReview" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCssM blindInput" /><br />
                                                                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Site Review"></asp:Label>
                                                                                </asp:HyperLink>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">
                                                                                <asp:HyperLink ID="hyp_DocumentManagement" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Documents">
                                                                                    <img src="assets/mobileicons/13-Documents.png" alt="Document Management" title="Document Management" class="imgCssM blindInput" /><br />
                                                                                    <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="Documents"></asp:Label>
                                                                                </asp:HyperLink>
                                                                            </div>
                                                                        </div>
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">
                                                                                <%--<asp:Panel ID="pnlLead" runat="server" Visible="false">
                                                                                    <asp:HyperLink ID="hyp_Leads" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Leads">
                                                                                        <img src="assets/mobileicons/14-Leads.png" alt="Lead" title="Leads" class="imgCssM blindInput" /><br />
                                                                                        <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Leads"></asp:Label>
                                                                                    </asp:HyperLink>
                                                                                </asp:Panel>--%>
                                                                                <asp:Panel ID="pnlCompoSiteCrew" runat="server">
                                                                                    <asp:HyperLink ID="hypCompositrSowCrew" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="SOW">
                                                                                        <img src="assets/mobileicons/06-Composite-SOW.png" alt="CompositrSow" title="CompositrSow" class="imgCssM blindInput" /><br />
                                                                                        <asp:Label ID="Label5" runat="server" Font-Bold="true" Text="SOW"></asp:Label>
                                                                                    </asp:HyperLink>
                                                                                </asp:Panel>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="clearfixM" style="margin-top: 10px !important;">
                                                                    <div class="row">
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">

                                                                                <asp:HyperLink ID="hypProjectNotes" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Project Notes">
                                                                                    <img src="assets/mobileicons/07-Project-Notes.png" alt="ProjectNotes" title="Project Notes" class="imgCssM blindInput" /><br />
                                                                                    <asp:Label ID="Label8" runat="server" Font-Bold="true" Text="Project Notes"></asp:Label>
                                                                                </asp:HyperLink>

                                                                            </div>
                                                                        </div>
                                                                        <asp:Panel ID="PnlMessage" runat="server" Visible="false">
                                                                            <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                                <div class="divImageCssM" style="text-align: center; width: 90px;">


                                                                                    <asp:LinkButton ID="lnkMessage" runat="server" ToolTip="Enroute" OnClick="lnkMessage_Click">
                                                                                        <img src="assets/mobileicons/21-Crew-Tracking.png" alt="Enroute" title="Enroute" class="imgCssM blindInput" /><br />
                                                                                        <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Text="Enroute"></asp:Label>
                                                                                    </asp:LinkButton>


                                                                                </div>
                                                                            </div>
                                                                        </asp:Panel>
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">

                                                                                <asp:HyperLink ID="hypSelection" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="Selection">
                                                                                    <img src="assets/mobileicons/09-Selection-Sheet.png" alt="Selection" title="Selection" class="imgCssM blindInput" /><br />
                                                                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Selection"></asp:Label>
                                                                                </asp:HyperLink>

                                                                            </div>
                                                                        </div>
                                                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4 ">
                                                                            <div class="divImageCssM" style="text-align: center; width: 90px;">

                                                                                <asp:HyperLink ID="hypPmNotes" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" ToolTip="PM Notes">
                                                                                    <img src="assets/mobileicons/19-PM-Notes.png" alt="PMNotes" title="PM Notes" class="imgCssM blindInput" /><br />
                                                                                    <asp:Label ID="Label11" runat="server" Font-Bold="true" Text="PM Notes"></asp:Label>
                                                                                </asp:HyperLink>

                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="" />
                                                    <AlternatingRowStyle CssClass="" />
                                                </asp:GridView>

                                            </td>
                                        </tr>
                                    </table>

                                </div>
                            </div>

                        </div>
                    </div>

                </div>



                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


