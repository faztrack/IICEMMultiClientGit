<%@ Page Title="Project Notes" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mProjectNotes.aspx.cs" Inherits="mProjectNotes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; }
    </script>
    <script type="text/javascript">
        function openModal() {
            $('#myModal').modal({ show: true });
        }
        function closeModal() {

            $("#<%=btnResetCId.ClientID %>")[0].click();
        }
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


        @media (min-width: 1025px) and (max-width: 1280px) {

            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
        }

        /* 
  ##Device = Tablets, Ipads (portrait)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) {

            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
        }

        /* 
  ##Device = Tablets, Ipads (landscape)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) and (orientation: landscape) {

            .mobileGrid {
                font-size: 10px;
            }

                .mobileGrid th {
                    padding: 10px;
                    font-size: 10px;
                }

                .mobileGrid tbody tr td {
                    padding: 5px;
                    margin: 5px;
                }
        }

        /* 
  ##Device = Low Resolution Tablets, Mobiles (Landscape)
  ##Screen = B/w 481px to 767px
*/

        @media (min-width: 481px) and (max-width: 767px) {

            .mobileGrid {
                font-size: 9px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 2px;
                    font-size: 9px;
                }

                .mobileGrid tbody tr td {
                    padding: 2px;
                    margin: 5px;
                }
        }

        /* 
  ##Device = Most of the Smartphones Mobiles (Portrait)
  ##Screen = B/w 320px to 479px
*/

        @media (min-width: 320px) and (max-width: 480px) {

            .mobileGrid {
                font-size: 9px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 2px;
                    font-size: 11px;
                }

                .mobileGrid tbody tr td {
                    padding: 6px;
                    margin: 5px;
                    font-size: 12px;
                }
        }

        body {
            margin-bottom: 50px;
        }

        .grid_header {
            color: #000;
            font-weight: bold;
            font-size: 14px;
        }

        .mobileGrid {
            color: #333;
            border-radius: 0;
        }

            .mobileGrid th {
                display: normal !important;
                background-color: #e1e1e1;
                color: #555;
                font-weight: bold;
                border: 1px solid #ddd;
                text-transform: uppercase;
            }

            .mobileGrid tbody tr td {
                width: 12%;
            }

            .mobileGrid td {
                background-color: #f9f9f9;
                border: 1px solid #ddd;
            }

                .mobileGrid td td {
                    background-color: transparent;
                    box-shadow: none;
                }

        .overlayContent {
            z-index: 99;
            margin: 250px auto;
            text-align: center;
        }

            .overlayContent p {
                font-size: 16px;
                color: #fff;
                font-family: Arial;
                font-weight: normal;
                background-color: #000;
                margin: 0 auto 10px auto;
            }

            .overlayContent img {
                width: 80px;
                height: 80px;
            }

        .popUpShadow {
            box-shadow: 0 0 500px #ddd !important;
            -moz-box-shadow: 0 0 500px #ddd !important;
            -webkit-box-shadow: 0 0 500px #ddd !important;
            background-color: none !important;
        }
    </style>


    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="myModal" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="closeModal()">x</button>
                            <h4 class="modal-title">Project Notes Details</h4>
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
                                                            <asp:TextBox ID="txtProjectDate" runat="server" CssClass="form-control" Style="margin: 0 !important;" size="16" type="text" Width="130px" TabIndex="1" />
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
                                    <asp:Label ID="Label3" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label"><span style="color:red">*</span>Section:
                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">
                                        <asp:DropDownList ID="ddlSection" runat="server" CssClass="textBox form-control" TabIndex="2"></asp:DropDownList>

                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label4" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label">Material Track:
                                        <asp:TextBox
                                            ID="txtMaterialDis" runat="server" BackColor="Transparent" CssClass="blindInput"
                                            BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                            Height="16px" ReadOnly="True"></asp:TextBox>

                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                        <asp:TextBox ID="txtMaterialTrack" runat="server" TabIndex="3" TextMode="MultiLine" CssClass="textBox form-control" Height="200px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtMaterialDis'));" MaxLength="1500"></asp:TextBox>

                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label5" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label">Design Updates:
                                        <asp:TextBox
                                            ID="txtDesignDis" runat="server" BackColor="Transparent" CssClass="blindInput"
                                            BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                            Height="16px" ReadOnly="True"></asp:TextBox>

                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                        <asp:TextBox ID="txtDesignUpdates" runat="server" TabIndex="4" TextMode="MultiLine" CssClass="textBox form-control" Height="200px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtDesignDis'));" MaxLength="1500"></asp:TextBox>

                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label6" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label">Superintenden Notes:
                                        <asp:TextBox
                                            ID="txtSuperDis" runat="server" BackColor="Transparent" CssClass="blindInput"
                                            BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                            Height="16px" ReadOnly="True"></asp:TextBox>

                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                        <asp:TextBox ID="txtSuperintendentNotes" runat="server" TabIndex="5" TextMode="MultiLine" CssClass="textBox form-control" Height="200px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtSuperDis'));" MaxLength="1500"></asp:TextBox>

                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label9" runat="server" CssClass="col-sm-12 col-md-6 col-lg-1 control-label">General Notes:
                                        <asp:TextBox
                                            ID="txtGeneralDis" runat="server" BackColor="Transparent" CssClass="blindInput"
                                            BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                            Height="16px" ReadOnly="True"></asp:TextBox>

                                    </asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">

                                        <asp:TextBox ID="txtGeneralNotes" runat="server" TabIndex="6" TextMode="MultiLine" CssClass="textBox form-control" Height="200px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtGeneralDis'));" MaxLength="1500"></asp:TextBox>

                                    </div>
                                </div>
                                <div class="form-group form-group-ext" style="width: 340px">

                                    <table cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td>&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="Label7" Style="padding: 3px 10px 0 0 !important; color: #175bb8" runat="server" Text="Incude In SOW:"> </asp:Label></td>
                                            <td>
                                                <div>
                                                    &nbsp;
                                                    <asp:CheckBox ID="chkSOW" runat="server" TabIndex="7" Checked="True" />&nbsp;&nbsp;
                                                </div>
                                            </td>
                                            <td>
                                                <table cellpadding="5" cellspacing="5">
                                                    <tr>
                                                        <td>&nbsp;&nbsp;&nbsp;
                                                <asp:Label ID="Label8" Style="padding: 3px 10px 0 0 !important; color: #175bb8" runat="server" Text="Completed:"> </asp:Label></td>
                                                        <td>
                                                            <div>
                                                                &nbsp;
                                                    <asp:CheckBox ID="chkComplete" runat="server" TabIndex="8" />&nbsp;&nbsp;
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div class="form-group form-group-ext" style="width: 340px">
                                </div>

                                <div class="form-group">
                                    <div class="col-md-10" style="text-align: center;">
                                        <asp:Button ID="btnResetCId" runat="server" Text="" OnClick="btnResetCId_Click" Style="display: none" />
                                        <asp:Label ID="lblMSG" runat="server" CssClass="control-label"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-11" style="text-align: center;">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-info e" TabIndex="23" Text="Save" Width="80px" OnClick="btnSubmit_Click" />
                                        <button type="button" class="btn btn-primary color-blue" data-dismiss="modal" onclick="closeModal()">Close</button>
                                    </div>

                                </div>
                            </div>


                        </div>

                    </div>

                </div>
            </div>




            <div class="panel panel-default">
                <div class="panel-heading panel-heading-ext">
                    <h3 class="panel-title">
                        <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                        <strong>Project Notes</strong>

                    </h3>

                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-user"></span>
                            <asp:Label ID="lblCustomerName" runat="server" Text="" Font-Bold="true"></asp:Label>
                        </div>
                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-map-marker"></span>
                            <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                        </div>

                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-phone"></span>
                            <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-envelope"></span>
                            <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-user"></span>
                            <asp:Label ID="lblSalesPerson" runat="server"></asp:Label>
                        </div>
                        <div class="col-sm-12">
                            <span class="glyphicon glyphicon-user"></span>
                            <asp:Label ID="lblSuperintendent" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="form-group row">

                        <label class="control-label col-sm-4">
                            Additional email(s):                               
                                <asp:TextBox ID="txtDisplay" runat="server" CssClass="textboxInputRight" BackColor="Transparent"
                                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                    Height="16px" ReadOnly="True"></asp:TextBox><br />
                            <font style="font-style: italic;">(Add additional emails with comma separation to be included in the project notes.)</font>
                        </label>
                        <div class="col-sm-3">
                            <asp:Label ID="lblAdditionalEmail" runat="server"></asp:Label>
                            <asp:TextBox ID="txtAdditionalEmail" runat="server" CssClass="textBox" TabIndex="1" Style="width: 100%;"
                                TextMode="MultiLine" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1000',document.getElementById('head_txtDisplay'));" Height="60px"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <span style="color: black;">(Example: john@domain.com, jane@domain.com)</span>
                        </div>
                        <div class="col-sm-3">
                            <asp:LinkButton ID="lnkEditAddEmail" runat="server" OnClick="lnkEditAddEmail_Click" Visible="false" CssClass="btn btn-info">Edit Additional email</asp:LinkButton>
                            &nbsp;<asp:LinkButton ID="lnkUpdateAddEmail" runat="server" OnClick="lnkUpdateAddEmail_Click" Visible="false" CssClass="btn btn-info">Update Additional email</asp:LinkButton></br>
                               <asp:Button ID="btnAddnewRow" runat="server" CssClass="orngButton" OnClick="btnAddnewRow_Click" Text="Add New Notes" Style="margin-top:10px;" />
                             <asp:ImageButton ID="imgSentEmail" runat="server" Style="margin: 3px 3px -10px;" CssClass="noBorderCss" ImageUrl="~/assets/send_email_button.png" OnClick="imgSentEmail_Click" />
                        </div>
                    </div>
                       <div class="form-group row">
                            <div class="col-lg-12 col-md-2 col-sm-2 col-xs-12">
                               <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                            </div>

                      </div>

                    

                    <div class="row">
                        <div class="col-lg-12  col-sm-12">
                            <asp:GridView ID="grdProjectNote" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="grdProjectNote_RowDataBound" OnPageIndexChanging="grdProjectNote_PageIndexChanging" ShowHeader="false"
                                CssClass="uGrid col-lg-12  col-sm-12">
                                <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <asp:LinkButton ID="inkDate" runat="server" Visible="false" OnClick="viewDetails" Font-Underline="true"></asp:LinkButton>
                                                    <asp:Label ID="lblDate" runat="server" Visible="false"></asp:Label>
                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblSow" runat="server"></asp:Label>

                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblCompleted" runat="server"></asp:Label>

                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblSection" runat="server"></asp:Label>

                                                </div>

                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblMNotes" runat="server" Text='Material Track:' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                    <asp:Label ID="lblMaterialTrack" runat="server" Text='<%# Eval("MaterialTrack").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <asp:Label ID="lblMaterialTrack_r" runat="server" Text='<%# Eval("MaterialTrack") %>' Visible="false" />
                                                    <asp:LinkButton ID="lnkOpenMaterialTrack" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenMaterialTrack_Click" runat="server" ForeColor="Blue"></asp:LinkButton>

                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblDUNotes" runat="server" Text='Design Updates:' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                    <asp:Label ID="lblDesignUpdates" runat="server" Text='<%# Eval("DesignUpdates").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <asp:Label ID="lblDesignUpdates_r" runat="server" Text='<%# Eval("DesignUpdates") %>' Visible="false" />
                                                    <asp:LinkButton ID="lnkOpenDesignUpdates" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenDesignUpdates_Click" runat="server" ForeColor="Blue"></asp:LinkButton>

                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblSNotes" runat="server" Text='Superintendent Notes:' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                    <asp:Label ID="lblSuperintendentNotes" runat="server" Text='<%# Eval("SuperintendentNotes").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <asp:Label ID="lblSuperintendentNotes_r" runat="server" Text='<%# Eval("SuperintendentNotes") %>' Visible="false" />
                                                    <asp:LinkButton ID="lnkOpenSuperintendentNotes" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpenSuperintendentNotes_Click" runat="server" ForeColor="Blue"></asp:LinkButton>

                                                </div>
                                                <div class="col-sm-12">
                                                    <asp:Label ID="lblGNotes" runat="server" Text='General Notes:' Style="display: inline;" Font-Bold="true"></asp:Label>

                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("NoteDetails").ToString() %>' Style="display: inline;"></asp:Label>
                                                    <asp:Label ID="lblDescription_r" runat="server" Text='<%# Eval("NoteDetails") %>' Visible="false" />
                                                    <asp:LinkButton ID="lnkOpenDescription" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnklnkOpenDescription_Click" runat="server" ForeColor="Blue"></asp:LinkButton>

                                                </div>


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
                    <table cellpadding="0" cellspacing="2" width="100%">
                        <tr>
                            <td align="center">



                                <tr>
                                    <td align="center">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="center">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="center" height="10px">
                                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnSalesEmail" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnAddEmailId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnProjectNotesEmail" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnCID" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnProjectId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnLastName" runat="server" Value="0" />
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

                    <
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            </td>
                </tr>
            </table>




        </ContentTemplate>
        <%--  <Triggers>
            <asp:PostBackTrigger ControlID="grdProjectNote" />
        </Triggers>--%>
    </asp:UpdatePanel>

    <asp:Panel ID="pnlConfirmation" runat="server" Width="310px" Height="100px" BackColor="Snow" Style="box-shadow: #555 0px 0px 250px 150px !important;">
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
                                        <td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnOk" runat="server" Text="OK" CssClass="btn btn-info" />
                                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

    <%--   <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1"
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
    </asp:UpdateProgress>--%>
</asp:Content>


