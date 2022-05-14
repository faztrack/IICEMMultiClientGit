<%@ Page Title="Site Review List" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="msiteviewlist.aspx.cs" Inherits="msiteviewlist" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
     <script type="text/javascript">
         Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
         function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; }
    </script>
    <style>
        div.fileinputs {
            position: relative;
        }

        div.fakefile {
            position: absolute;
            top: 0px;
            left: 0px;
            z-index: 1;
        }

        .file {
            position: relative;
            text-align: right;
            -moz-opacity: 0;
            filter: alpha(opacity: 0);
            opacity: 0;
            z-index: 2;
        }

        .ajax__calendar_container {
            z-index: 1 !important;
        }

        a.fancybox img {
            border: none;
            box-shadow: 0 1px 7px rgba(0,0,0,0.6);
            -o-transform: scale(1,1);
            -ms-transform: scale(1,1);
            -moz-transform: scale(1,1);
            -webkit-transform: scale(1,1);
            transform: scale(1,1);
            -o-transition: all 0.2s ease-in-out;
            -ms-transition: all 0.2s ease-in-out;
            -moz-transition: all 0.2s ease-in-out;
            -webkit-transition: all 0.2s ease-in-out;
            transition: all 0.2s ease-in-out;
        }

        a.fancybox:hover img {
            position: relative;
            z-index: 999;
            -o-transform: scale(1.03,1.03);
            -ms-transform: scale(1.03,1.03);
            -moz-transform: scale(1.03,1.03);
            -webkit-transform: scale(1.03,1.03);
            transform: scale(1.03,1.03);
        }

        .color-blue {
            background-color: blue !important;
            border-color: blue !important;
        }

            .color-blue:hover {
                background-color: darkblue !important;
                border-color: darkblue !important;
            }

        .opacityimage {
            border-radius: 50px;
            transform: scale(1.5,1.5);
            transition: .3s;
        }


    </style>

    <script>

        function openModal() {
            $('#myModal').modal({ show: true });
        }
        function closeModal() {

            $("#<%=btnResetCId.ClientID %>")[0].click();
        }
        function openModalAttachmentView() {
            $('#AttachmentView').modal({ show: true });
        }
    </script>

    <script type="text/javascript">
        //On Page Load.
        $(function () {
            SetDatePicker();
        });

        //On UpdatePanel Refresh.
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function (sender, e) {
                if (sender._postBackSettings.panelsToUpdate != null) {
                    SetDatePicker();
                    $(".datepicker-orient-bottom").hide();
                }
            });
        };

        function SetDatePicker() {
            $('#sandbox-container .input-group.date').datepicker({
                todayBtn: "linked",
                orientation: "bottom left",
                autoclose: true,
                todayHighlight: true
            })
        }


    </script>



    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <!-- Site Review Modal-->


            <div id="myModal" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false">
                <div class="modal-dialog">

                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="closeModal()">x</button>
                            <h4 class="modal-title">Site Review Details &nbsp;(<asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>)</h4>
                        </div>
                        <div class="modal-body">


                            <div class="form-horizontal">


                                <div class="form-group form-group-ext">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" CssClass="col-sm-3 col-md-6 col-lg-6 control-label "><span style="color:red">*</span>Date: </asp:Label></td>
                                            <td>
                                                <div class="col-sm-8 col-md-5 col-lg-6 col-ext" style="margin: 0px">
                                                    <div class="form-group" id="sandbox-container">
                                                        <div class="input-group date">
                                                            <asp:TextBox ID="txtSiteReviewDate" runat="server" CssClass="form-control" Style="margin: 0 !important;" size="16" type="text" Width="130px" TabIndex="1" />
                                                            <div class="input-group-addon">
                                                                <span class="glyphicon glyphicon-th"></span>
                                                            </div>
                                                            </span>
                                                        </div>
                                                    </div>
                                            </td>
                                        </tr>
                                    </table>




                                </div>



                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label9" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label">Notes:
                                        <asp:TextBox
                                            ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                                            BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                            Height="16px" ReadOnly="True"></asp:TextBox>

                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                        <asp:TextBox ID="txtSiteReviewNote" runat="server" TabIndex="2" TextMode="MultiLine" CssClass="textBox form-control" Height="250px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'3000',document.getElementById('head_txtDisplay'));" MaxLength="3000"></asp:TextBox>


                                    </div>
                                </div>

                                <div class="form-group form-group-ext" style="width: 340px">

                                    <table cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td>&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="Label1" Style="padding: 3px 10px 0 0 !important; color: #175bb8" runat="server" Text="Viewable By:"> </asp:Label></td>
                                            <td>
                                                <div>
                                                    <asp:CheckBox ID="chkCustomer" runat="server" />&nbsp;&nbsp;Customer
                                                </div>
                                            </td>
                                        </tr>
                                    </table>


                                    <table width="100%" runat="server" visible="false">
                                        <tr>
                                            <td width="50%" align="left">

                                                <table width="100%" align="left">
                                                    <tr>
                                                        <td width="28%" text-aling="right">&nbsp;</td>
                                                        <td width="60%" colspan="1">Share</td>

                                                    </tr>
                                                    <tr>
                                                        <td width="60%" text-aling="right"><%--<span class="glyphicon glyphicon-user"></span>&nbsp;--%>User</td>
                                                        <td>
                                                            <asp:CheckBox ID="chkUser" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td text-aling="right" width="30%">Customer&nbsp;</td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td text-aling="right" width="30%">Vendor</td>
                                                        <td>
                                                            <asp:CheckBox ID="chkVendor" runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                            <td width="50%" align="left">
                                                <table width="100%" align="right">
                                                    <tr>
                                                        <td width="27%" text-aling="right">&nbsp;</td>
                                                        <td width="100%" colspan="2">Notify</td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%" text-aling="right">&nbsp;</td>
                                                        <td>
                                                            <asp:CheckBox ID="chkUserNotify" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%" text-aling="right">&nbsp;</td>
                                                        <td>
                                                            <asp:CheckBox ID="chkCustomerNotify" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="30%" text-aling="right">&nbsp;</td>
                                                        <td>
                                                            <asp:CheckBox ID="chkVendorNotify" runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>

                                        </tr>

                                    </table>




                                </div>




                                <div class="row">
                                    <div class=" col-lg-12">
                                        <div class="form-group form-group-ext" style="width: 350px; margin-bottom: 10px">
                                            <table width="100%">
                                                <tr>
                                                    <td width="40%">
                                                        <asp:Label ID="Label6" runat="server" CssClass="col-sm-12 col-md-6 col-lg-11 control-label" Style="text-align: left">State of Mind:&nbsp;<asp:CheckBox ID="chkCustomerMind" runat="server" OnCheckedChanged="chkCustomerMind_CheckedChanged" AutoPostBack="true" />
                                                        </asp:Label>
                                                    </td>
                                                    <td width="60%" aling="left">
                                                        <div class="col-sm-12 col-md-6 col-lg-12 col-ext">
                                                            <asp:Panel ID="pnlCustomerStateofMind" runat="server" Visible="false">
                                                                <asp:ImageButton ID="imgbtnAngry" runat="server" ImageUrl="~/assets/customerstatemind/angry.png" Width="26px" Height="26px" ToolTip="Angry" OnClick="imgbtnAngry_Click" />
                                                                &nbsp;&nbsp;<asp:ImageButton ID="imgbtnFrustrated" runat="server" ImageUrl="~/assets/customerstatemind/frustrated.png" Width="26px" Height="26px" ToolTip="Frustrated" OnClick="imgbtnFrustrate_Click" />
                                                                &nbsp; &nbsp;<asp:ImageButton ID="imgbtnConfused" runat="server" ImageUrl="~/assets/customerstatemind/confused.png" Width="26px" Height="26px" ToolTip="Confused" OnClick="imgbtnConfused_Click" />
                                                                &nbsp; &nbsp;<asp:ImageButton ID="imgbtnIndifferent" runat="server" ImageUrl="~/assets/customerstatemind/indifferent.png" Width="26px" Height="26px" ToolTip="Indifferent " OnClick="imgbtnIndifferent_Click" />
                                                                &nbsp; &nbsp;
                                                                <asp:ImageButton ID="imgbtnHappy" runat="server" ImageUrl="~/assets/customerstatemind/happy.png" Width="26px" Height="26px" ToolTip="Happy" OnClick="imgbtnHappay_Click" />



                                                            </asp:Panel>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>


                                        </div>

                                    </div>
                                </div>

                                <div class="form-group form-group-ext" style="width: 340px">

                                    <table cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td>&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="Label7" Style="padding: 3px 10px 0 0 !important; color: #175bb8" runat="server" Text="Notification:"> </asp:Label></td>
                                            <td>
                                                <div>
                                                    &nbsp;
                                                    <asp:CheckBox ID="chkEmail" runat="server" TabIndex="3" Checked="True" />&nbsp;&nbsp;Email
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>




                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 col-md-offset-2" style="float: left; margin-top: -7px;">

                                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="dropdown-toggle" data-toggle="dropdown">
                                        <asp:Image ID="image1" runat="server" ImageAlign="Middle" ImageUrl="~/images/ic_photo_camera_black_24px.png" Style="cursor: pointer; width: 78px; height: 78px; margin-top: 20px;" />

                                        <asp:Image ID="imgPreview" runat="server" ImageAlign="Middle" ImageUrl="~/icons/default.png" Style="width: 150px; height: 70px; margin-left: 10px; margin-top: 5px;" />
                                        <ul class="dropdown-menu animated fadeInLeft" style="top: 50px; min-width: 270px; min-height: 200px; border: 1px solid #000;">
                                            <li class="has-sub">
                                                <a href="javascript:;">
                                                    <span>Select Source</span>
                                                </a>
                                                <ul style="list-style-type: none; margin: 0; padding: 0;">
                                                    <li style="display: inline; float: left; padding: 5px 15px;">
                                                        <div class="form-group" runat="server" id="Div1">
                                                            <div class="col-md-3"></div>
                                                            <div class="col-md-2">
                                                                <div class="fileinputs" id="divFile">
                                                                    <asp:FileUpload ID="imgFileUpload" CssClass="btn btn-primary start file" runat="server" onchange="imagePreview()" TabIndex="8" Style="width: 52px; height: 49px" />

                                                                    <div class="fakefile">
                                                                        <p style="text-align: center">
                                                                            <img src="icons/ic_photo_black_24px.png" style="width: 64px; height: 64px" />
                                                                            <b>Photos</b>
                                                                        </p>
                                                                    </div>
                                                                </div>


                                                            </div>
                                                        </div>
                                                    </li>
                                                    <li style="display: inline; float: left; padding: 5px 20px;">
                                                        <div class="form-group" runat="server" id="Div2">
                                                            <div class="col-md-3"></div>
                                                            <div class="col-md-2">
                                                                <div class="fileinputs" id="divCam">
                                                                    <input id="imgCapture" name="imgCapture" type="file" class="file" runat="server" onchange="imagePreview()" style="width: 52px; height: 49px" accept="image/*" capture="camera" />

                                                                    <div class="fakefile">
                                                                        <p style="text-align: center;">
                                                                            <img src="icons/ic_photo_camera_black_24px.png" style="width: 64px; height: 64px; cursor: pointer;" />
                                                                            <b>Camera</b>
                                                                        </p>

                                                                    </div>
                                                                </div>

                                                            </div>
                                                        </div>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </asp:HyperLink>
                                </div>





                                <div class="form-group">
                                    <div class="col-md-10" style="text-align: center;">
                                        <asp:Label ID="Label4" runat="server" CssClass="control-label"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-11" style="text-align: center; margin-bottom: 20px">
                                        <asp:Label ID="lblImageResult" runat="server"></asp:Label><br />
                                        <asp:Button ID="btnCaptureImage" runat="server" CssClass="btn btn-info" TabIndex="23" Text="Upload" Width="80px" OnClick="btnSubmit_Convert_Click" />
                                        &nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" TabIndex="24" Text="Reset" />

                                    </div>
                                </div>
                                <div class="row">


                                    <div id="divImageCapture" class="col-xs-12 col-sm-12 col-md-12 col-lg-12" runat="server">
                                    </div>

                                </div>
                                <div class="form-group form-group-ext" style="width: 340px;">
                                    <asp:Label ID="Label5" runat="server" CssClass="col-sm-12 col-md-6 col-lg-6 control-label">Upload Attachments: </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-6 col-ext" style="margin-top: 7px;">
                                         <asp:Button ID="btnResetCId" runat="server" Text="" OnClick="btnResetCId_Click" Style="display:none" />
                                        <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" onchange="UploadFile()" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-10" style="text-align: center;">
                                        <asp:Label ID="lblResult" runat="server" CssClass="control-label"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-11" style="text-align: center;">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary color-blue" TabIndex="23" Text="Save" Width="80px" OnClick="btnSubmit_Click"  />
                                        <asp:Button ID="btnSiteViewDetailDelete" runat="server" CssClass="btn btn-danger" TabIndex="23" Text="Delete" Width="80px" OnClick="btnSiteViewDetailDelete_Click" Visible="false" />
                                        <button type="button" class="btn btn-info" data-dismiss="modal" onclick="closeModal()">Close</button>
                                    </div>

                                </div>
                                <div class="form-group">
                                    <div class="col-md-12 col-sm-12" style="text-align: center;">
                                        <asp:Label ID="lblUpload" runat="server" Text="Must Save to Upload All files" Font-Bold="true" Visible="false"></asp:Label>

                                    </div>

                                </div>

                                <div class="row">
                                    <div class=" col-lg-12 col-lg-offset-3 col-sm-offset-1">
                                        &nbsp;
                                    </div>

                                </div>
                                <div class="form-group">
                                    <div class="row">
                                        <div class=" col-lg-12 col-lg-offset-3 col-sm-offset-1">
                                            <div class="row">
                                                <div class="col-sm-12 col-md-3 col-lg-6">
                                                    <asp:Button ID="btnAttachment" runat="server" Text="Show All Attachment" OnClick="btnAttachment_Click" CssClass="btn btn-danger" Visible="false" />
                                                </div>
                                                <div class="col-sm-12 col-md-3 col-lg-6">
                                                    <asp:Label ID="lblAttachmentCount" runat="server" Text="" CssClass="btn btn-primary" Visible="false"></asp:Label>
                                                </div>
                                            </div>

                                        </div>

                                    </div>
                                </div>
                                <asp:Panel ID="pnlTemporaryImageUpload" runat="server" Visible="false">
                                    <div class="panel-heading panel-heading-ext">
                                        <h3 class="panel-title">Temporary uploaded Files</h3>
                                    </div>

                                    <asp:GridView ID="grdTemp" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                        CssClass="uGrid" Width="100%" OnRowDataBound="grdTemp_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="clearfix">
                                                        <div class="divImageCss">

                                                            <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" Visible="false" />

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
                                                        <div class="divEditCss" style="padding-left: 10px; padding-top: 5px;">
                                                            <asp:Label ID="lblFileName" runat="server" Text="" Visible="false"></asp:Label>
                                                        </div>
                                                        <div class="divDeleteCss">
                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFile" />
                                                        </div>

                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="" />
                                        <AlternatingRowStyle CssClass="" />
                                    </asp:GridView>
                                </asp:Panel>

                                <asp:Panel ID="pnlDetailImageUpload" runat="server">
                                    <div class="panel-heading panel-heading-ext">
                                        <h3 class="panel-title">All Attached Files</h3>
                                    </div>
                                    <asp:GridView ID="grdImageDetails" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                        CssClass="uGrid" Width="100%" OnRowDataBound="grdImageDetails_RowDataBound">
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
                                                        <div class="divEditCss" style="padding-left: 10px; padding-top: 5px;">
                                                            <asp:Label ID="lblFileName" runat="server" Text="" Visible="false"></asp:Label>
                                                        </div>
                                                        <div class="divDeleteCss">
                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteViewDetailImage" />
                                                        </div>

                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="" />
                                        <AlternatingRowStyle CssClass="" />
                                    </asp:GridView>
                                </asp:Panel>



                            </div>


                        </div>

                    </div>

                </div>
            </div>


            <!-- End SiteReview Modal-->

            <!-- Attachment View Modal content-->
            <div id="AttachmentView" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false" style="z-index: 9999999">
                <div class="modal-dialog">

                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">All Attached Files</h4>
                        </div>
                        <div class="modal-body">
                            <asp:GridView ID="grdAttchmentViewImage" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                CssClass="uGrid" OnRowDataBound="grdAttchmentViewImage_RowDataBound" Width="100%">
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
                                                <div class="divEditCss1" style="padding-left: 10px; padding-top: 5px;">
                                                    <asp:Label ID="lblFileName" runat="server" Text="" Visible="false"></asp:Label>
                                                </div>

                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="" />
                                <AlternatingRowStyle CssClass="" />
                            </asp:GridView>

                        </div>

                    </div>

                </div>
            </div>
            <!-- Attachment View Modal content-->

            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-ext">

                            <table width="100%">
                                <tr>
                                     <td align="left">
                                          <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                                     </td>
                                    <td>
                                          <span style="font-weight: bold;">
                                             Site Review List<br />

                                          </span>  &nbsp;<asp:Label ID="lblCustomerLastName" runat="server" Text="" Font-Bold="true"></asp:Label>

                                    </td>
                                    <td align="right" >
                                         <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/assets/mobileicons/add_new_site_review.png" Height="32px" ToolTip="Add New Site Review" OnClick="btnhdnValueReset_Click" />
                                    </td>
                                </tr>
                            </table>
                            

                        </div>


                        <div class="panel-body">




                            <div class="row">
                                <div class="col-lg-12 col-sm-12 col-md-12">
                                    <div class="input-group">
                                        <button id="Button2" type="button" runat="server" data-toggle="modal" data-target="#myModal" style="display: none">
                                            Add New Site Review
                                            <asp:Button ID="btnUpload" runat="server" Text="" CssClass="noEffectNew" OnClick="btnUpload_Click" />
                                        </button>
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                               
                            </div>

                            <div class="row">

                                 <div class="col-sm-12 col-lg-3">

                                    <table width="100%">
                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">
                                                    <tr>
                                                        <td align="right"><span class="required" style="color:red;">* </span>
                                                            <b>Start Date: </b>
                                                        </td>
                                                        <td align="left" style="width: 140px;">
                                                            <asp:TextBox ID="txtStartDate" runat="server" Width="130px" CssClass="form-control"></asp:TextBox>
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
                                        </tr>
                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">
                                                    <tr>
                                                        <td align="right"><span class="required"  style="color:red;">* </span>
                                                            <b>End Date: </b>
                                                        </td>
                                                        <td align="left" style="width: 140px;">
                                                            <asp:TextBox ID="txtEndDate" runat="server" Width="132px" CssClass="form-control"></asp:TextBox>
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

                                        </tr>

                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">
                                                    <tr>
                                                        <td align="center" colspan="4">
                                                            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                                        </td>


                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">
                                                    <tr>
                                                        <td align="right" style="width: 120px;">&nbsp;
                                                        </td>
                                                        <td align="center">
                                                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td align="left">
                                                            <asp:Button ID="btnSearch" runat="server" Text="View"
                                                                CssClass="btn btn-default" OnClick="btnSearch_Click" />
                                                        </td>

                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>

                                </div>

                                <div class="col-lg-12  col-sm-12">
                                    <asp:GridView ID="grdSiteViewList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        OnRowDataBound="grdSiteViewList_RowDataBound" OnPageIndexChanging="grdSiteViewList_PageIndexChanging"
                                        CssClass="mGrid col-lg-12  col-sm-12">
                                        <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-calendar"></span>

                                                            <asp:LinkButton ID="InkViewDetails" runat="server" OnClick="ViewSiteViewDetails" ToolTip="Site View Details"></asp:LinkButton>
                                                        </div>

                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-plus"></span>Added By
                                                            <asp:Label ID="lblCreateBy" runat="server" ToolTip="Added By"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-list-alt"></span>
                                                            <asp:Label ID="lblSiteReviewsNotes" runat="server" ToolTip="Site Review Note"></asp:Label>
                                                        </div>
                                                        <asp:Panel ID="pnlAttachmentView" runat="server" Visible="false">
                                                            <div class="col-sm-12">
                                                                <span class="glyphicon glyphicon-file">Attachments<asp:Label ID="lblAttachmentCount" runat="server" Text=""></asp:Label></span>
                                                                <asp:LinkButton ID="InkAttachmentView" runat="server" OnClick="ViewAllAttachment" ToolTip="Attachment View"></asp:LinkButton>

                                                            </div>
                                                        </asp:Panel>

                                                    </div>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div>
                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnSiteReviewId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnCustomerMind" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnDataURL" runat="server" Value="" />
                    <asp:HiddenField ID="hdnCID" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnSalesEmail" runat="server" Value="" />
                    <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
                    <asp:HiddenField ID="hdnLastName" runat="server" Value="" />
                     <asp:HiddenField ID="hdnEmailType" runat="server" Value="0" />
                </div>

            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnCaptureImage" />
        </Triggers>
    </asp:UpdatePanel>

    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
        function UploadFile() {

            $("#<%=btnUpload.ClientID %>")[0].click();
        }
    </script>


    <script type="text/javascript">

        function onClickResetMessage() {
            var objlbl = document.getElementById('<%=lblImageResult.ClientID%>');
            objlbl.innerHTML = '';
        }

        function ImageReset() {
            var imgPreview = document.querySelector('#<%=imgPreview.ClientID %>');
            imgPreview.src = "";
        }

        function imagePreview() {
            try {

                var hdn = document.querySelector('#<%=hdnDataURL.ClientID %>');
                var imgPreview = document.querySelector('#<%=imgPreview.ClientID %>'),
                    file,
                    reader;
                file = document.querySelector('#<%=imgCapture.ClientID %>').files[0] || null;
                if (file == null) {
                    file = document.querySelector('#<%=imgFileUpload.ClientID %>').files[0];

                }


                //wait(1000);
                reader = new FileReader();
                var img = document.createElement("img");
                reader.onloadend = function (e) {

                    getOrientation(file, function (orientation) {

                        var degrees = 0;
                        if (orientation == 6)
                            degrees = 90;
                        else if (orientation == 3)
                            degrees = 180;
                        else if (orientation == 8)
                            degrees = 270;


                        //  document.getElementById('<%=lblImageResult.ClientID%>').innerHTML = "Degree--" + degrees.toString();

                        img.src = e.target.result;

                        img.onload = function () {
                            var canvas = document.createElement("canvas");

                            //var canvas = $("<canvas>", {"id":"testing"})[0];
                            var ctx = canvas.getContext("2d");
                            ctx.drawImage(img, 0, 0);

                            var MAX_WIDTH = 1024;
                            var MAX_HEIGHT = 768;
                            var width = img.width;
                            var height = img.height;

                            if (width > height) {
                                if (width > MAX_WIDTH) {
                                    height *= MAX_WIDTH / width;
                                    width = MAX_WIDTH;
                                }
                            } else {
                                if (height > MAX_HEIGHT) {
                                    width *= MAX_HEIGHT / height;
                                    height = MAX_HEIGHT;
                                }
                            }

                            var dataurl;

                            // Rotate

                            if (degrees == 90 || degrees == 270) {

                                var canvas2 = document.createElement("canvas");

                                var ctx2 = canvas2.getContext("2d");


                                canvas.width = img.height;
                                canvas.height = img.width;
                                canvas2.width = height;
                                canvas2.height = width;


                                ctx.clearRect(0, 0, canvas.width, canvas.height);
                                if (degrees == 90 || degrees == 270) {
                                    ctx.translate(img.height / 2, img.width / 2);
                                } else {
                                    ctx.translate(img.width / 2, img.height / 2);
                                }
                                ctx.rotate(degrees * Math.PI / 180);
                                ctx.drawImage(img, -img.width / 2, -img.height / 2);

                                ctx2.drawImage(canvas, 0, 0, height, width);



                                dataurl = canvas2.toDataURL("image/jpeg", 0.4);

                                imgPreview.style.width = "112px";
                                imgPreview.style.height = "150px";


                            }
                            else if (degrees == 180) {

                                var canvas2 = document.createElement("canvas");

                                var ctx2 = canvas2.getContext("2d");


                                canvas.width = img.width;
                                canvas.height = img.height;
                                canvas2.width = width;
                                canvas2.height = height;


                                ctx.clearRect(0, 0, canvas.width, canvas.height);
                                if (degrees == 90 || degrees == 270) {
                                    ctx.translate(img.height / 2, img.width / 2);
                                } else {
                                    ctx.translate(img.width / 2, img.height / 2);
                                }
                                ctx.rotate(degrees * Math.PI / 180);
                                ctx.drawImage(img, -img.width / 2, -img.height / 2);

                                ctx2.drawImage(canvas, 0, 0, width, height);



                                dataurl = canvas2.toDataURL("image/jpeg", 0.4);

                                imgPreview.style.width = "150px";
                                imgPreview.style.height = "112px";

                            }
                            else {
                                canvas.width = width;
                                canvas.height = height;

                                ctx.drawImage(img, 0, 0, width, height);
                                dataurl = canvas.toDataURL("image/jpeg", 0.4);

                                imgPreview.style.width = "150px";
                                imgPreview.style.height = "112px";
                            }


                            hdn.value = dataurl;
                            imgPreview.src = dataurl;// reader.result;
                        }
                    });
                }
                if (file) {
                    reader.readAsDataURL(file);
                    document.getElementById("divFile").innerHTML = document.getElementById("divFile").innerHTML;
                    document.getElementById("divCam").innerHTML = document.getElementById("divCam").innerHTML;
                } else {
                    imgPreview.src = "";
                }
                document.getElementById('<%=lblImageResult.ClientID%>').innerHTML = "<span style='color:blue;'><b>Please click Upload to save image</b></span>";
            }
            catch (err) {
                document.getElementById('<%=lblImageResult.ClientID%>').innerHTML = err.message;
         }
     }

     //window.onerror = function () {
     //    alert("Error caught");
     //};

     function getOrientation(file, callback) {
         var reader = new FileReader();
         reader.onload = function (e) {

             var view = new DataView(e.target.result);
             if (view.getUint16(0, false) != 0xFFD8) {
                 return callback(-2);
             }
             var length = view.byteLength, offset = 2;
             while (offset < length) {
                 if (view.getUint16(offset + 2, false) <= 8) return callback(-1);
                 var marker = view.getUint16(offset, false);
                 offset += 2;
                 if (marker == 0xFFE1) {
                     if (view.getUint32(offset += 2, false) != 0x45786966) {
                         return callback(-1);
                     }

                     var little = view.getUint16(offset += 6, false) == 0x4949;
                     offset += view.getUint32(offset + 4, little);
                     var tags = view.getUint16(offset, little);
                     offset += 2;
                     for (var i = 0; i < tags; i++) {
                         if (view.getUint16(offset + (i * 12), little) == 0x0112) {
                             return callback(view.getUint16(offset + (i * 12) + 8, little));
                         }
                     }
                 }
                 else if ((marker & 0xFF00) != 0xFF00) {
                     break;
                 }
                 else {
                     offset += view.getUint16(offset, false);
                 }
             }
             return callback(-1);
         };
         reader.readAsArrayBuffer(file);
     }

     function wait(ms) {
         var start = new Date().getTime();
         var end = start;
         while (end < start + ms) {
             end = new Date().getTime();
         }
     }
    </script>


</asp:Content>


