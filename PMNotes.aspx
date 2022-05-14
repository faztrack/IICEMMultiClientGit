<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="PMNotes.aspx.cs" Inherits="PMNotes" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

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

        .chkTest {
        }

            .chkTest td {
            }

                .chkTest td input {
                    margin: 6px 4px 0 0;
                }

                .chkTest td label {
                }
    </style>


    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-ext">
                            <h3 class="panel-title">
                                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                                <strong>PM Notes &nbsp;<label runat="server" style="color: #fff; font-weight: bold;" id="lblCustomereHeader" /></strong>

                            </h3>

                        </div>

                        <div class="panel-body">
                            <div class="row">
                                <div class="col-sm-12 col-lg-12">
                                    <table width="100%">
                                        <tr>
                                            <td align="left" width="100%">

                                                <div class="row">
                                                    <div class="col-sm-6 col-md-3 col-lg-6  col-xs-6 " style="padding-left: 14px; margin-top: 14px;">
                                                        <asp:Label ID="lblSNotes" runat="server" Text='Section' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                    </div>
                                              
                                                    <div class="col-sm-6 col-md-3 col-lg-6 col-xs-6 text-right" style="padding-right: 15px; ">
                                                        <asp:ImageButton ID="imgSentEmail" runat="server" Style="align-items: flex-end" CssClass="noBorderCss" ImageUrl="~/assets/send_email_button.png" OnClick="imgSentEmail_Click" />
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-sm-12 col-md-3 col-lg-6 col-xs-12 pull-left" style="padding-left: 15px; padding-right: 15px; text-align:left;">
                                                        <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control form-control-ext" DataValueField="section_id" DataTextField="section_name" AutoPostBack="True" OnSelectedIndexChanged="ddlSection_IndexChanged">
                                                        </asp:DropDownList>
                                                        <asp:Label ID="lblSection" runat="server" Visible="false"></asp:Label>
                                                    </div>
                                                     <div class="col-lg-6"></div>
                                                

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">

                                                    <tr>
                                                        <td width="100px" align="left">
                                                            <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">
                                                                <asp:Label ID="lblVendorName" Visible="false" runat="server" Text='Vendor Name' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                            </div>
                                                        </td>

                                                    </tr>
                                                    <tr>

                                                        <td>
                                                            <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">
                                                                <asp:Panel ID="pnlSection" runat="server">
                                                                    <table align="center" cellpadding="0" cellspacing="2" class="wrapper" width="100%">
                                                                        <tr>
                                                                            <td align="center">
                                                                                <asp:CheckBoxList ID="chkVendors" runat="server" CssClass="chkTest"
                                                                                    Width="100%" RepeatColumns="1" TabIndex="2">
                                                                                </asp:CheckBoxList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:Panel>
                                                            </div>
                                                        </td>
                                                    </tr>

                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top">&nbsp;</td>
                                            <td align="left"></td>
                                        </tr>
                                        <tr>
                                            <td align="left" width="100%">


                                                <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">
                                                    <asp:Label ID="Label5" runat="server" Text='Notes' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                    <asp:TextBox
                                                        ID="txtNoteCount" runat="server" BackColor="Transparent" CssClass="blindInput"
                                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                                        Height="16px" ReadOnly="True"></asp:TextBox>


                                                </div>

                                                <div class="col-sm-12 col-md-3 col-lg-4 col-ext-txtArea">

                                                    <asp:TextBox ID="txtNote" runat="server" CssClass="textBox form-control" Height="120px"
                                                        TabIndex="3" TextMode="MultiLine" Width="100%"
                                                        onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'1500',document.getElementById('head_txtNoteCount'));" MaxLength="1500"></asp:TextBox>
                                                    <asp:Label ID="lblResultNote" runat="server"></asp:Label>
                                                </div>


                                                <div class="col-sm-8 col-md-5 col-lg-6 col-ext" style="margin: 0px">
                                                    <div class="form-group" id="sandbox-container">
                                                    </div>
                                                </div>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" width="100%">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;" width="100%">
                                                    <tr>

                                                        <td colspan="2" align="left">
                                                            <div class="col-sm-12 col-md-3 col-lg-9 col-ext-txtArea">
                                                                <asp:Button ID="btnSubmit" runat="server"
                                                                    TabIndex="18" Text="Submit" CssClass="btn btn-info" OnClick="btnSubmit_Click"
                                                                    Width="80px" />
                                                                &nbsp;<asp:Button ID="btnCancel" runat="server" TabIndex="19" Text="Reset"
                                                                    CausesValidation="False" CssClass="btn btn-primary color-blue" Visible="true" OnClick="btnCancel_Click"
                                                                    Width="80px" />
                                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>

                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdPMnotes" runat="server" AutoGenerateColumns="False"
                                                    CssClass="mobileGrid table table-hover" AllowPaging="true"
                                                    PageSize="5" TabIndex="2" Width="100%" EnableViewState="true"
                                                    OnPageIndexChanging="grdPMnotes_PageIndexChanging"
                                                    SelectedRowStyle-BackColor="Yellow">
                                                    <PagerSettings Position="Bottom" Mode="NumericFirstLast" PageButtonCount="4" FirstPageText="First" LastPageText="Last" />
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <div class="row">
                                                                    <div class="col-sm-12">
                                                                        <asp:Label ID="lblSNotes" runat="server" Text='Created Date:' Style="display: inline;" Font-Bold="true"></asp:Label>
                                                                        <asp:LinkButton ID="lnkEditNote" Font-Underline="true" OnClick="lnkEditNote_Click" runat="server">
                                                                            <asp:Label ID="lblCreateDate" runat="server" Text='<%# Eval("CreateDate","{0:d}")%>' />
                                                                        </asp:LinkButton></div><div class="col-sm-12">
                                                                        <asp:Label ID="Label1" runat="server" Text='Section:' Style="display: inline;" Font-Bold="true"></asp:Label><asp:Label ID="lblSection" runat="server" Text='<%# Eval("section_name")%>' />
                                                                    </div>
                                                                    <div class="col-sm-12">
                                                                        <asp:Label ID="Label2" runat="server" Text='Vendors:' Style="display: inline;" Font-Bold="true"></asp:Label><asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("vendor_name")%>' />
                                                                    </div>
                                                                    <div class="col-sm-12">
                                                                        <asp:Label ID="Label3" runat="server" Text='PM Notes:' Style="display: inline;" Font-Bold="true"></asp:Label><asp:Label ID="lblNoteDetails" runat="server" Text='<%# Eval("NoteDetails")%>' />
                                                                    </div>
                                                                          <div class="col-sm-12">
                                                                        <asp:Label ID="Label4" runat="server" Text='Done: ' Style="display: inline;" Font-Bold="true"></asp:Label><asp:Label ID="Label6" runat="server" Text='<%# Eval("Complete")%>' />
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>

                                                        </asp:TemplateField>




                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>




                                    </table>
                                </div>

                            </div>






                            <div class="form-group row">
                            </div>




                            <div class="row">
                                <div class="col-lg-12  col-sm-12">
                                </div>
                            </div>
                            <table cellpadding="0" cellspacing="2" width="100%">
                                <tr>
                                    <td align="center">



                                        <tr>
                                            <td align="center">&nbsp;</td></tr><tr>
                                            <td align="center">&nbsp;</td></tr><tr>
                                            <td align="center" height="10px">
                                                <asp:HiddenField ID="HiddenFieldGrd" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnPMnoteID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnCID" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnPageNo" runat="server" Value="0" />
                                                 <asp:HiddenField ID="hdnCustomerName" runat="server" Value="0" />
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


                        </div>


                    </div>
                </div>
            </div>


            </label>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Panel ID="pnlConfirmation" runat="server" Width="90%" Height="200px" BackColor="Snow" Style="box-shadow: #555 0px 0px 250px 150px !important;">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                    <tr>
                        <td align="right">&nbsp; </td></tr><tr>
                        <td align="center">
                            <b>Please Enter To Email</b> </td></tr><tr>
                        <td align="center">
                           <asp:TextBox ID="txtToEmail" runat="server" CssClass="textBox" 
                                                         Width="75%" Height="1%" TextMode="MultiLine"></asp:TextBox></td></tr><tr>
                        <td align="center">
                          
                              <div class="col-sm-12">
                            <span style="color: black;">(Example: john@domain.com, jane@domain.com)</span> </div></td></tr><tr>
                        <td align="center">
                          
                              <div class="col-sm-12">
                            <asp:Label id="lblEmailAddress" runat="server" style="text-emphasis-color: red;"></asp:Label></div></td></tr><tr>
                        <td align="center" style="height:10px">
                          
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnOk" runat="server" Text="OK" CssClass="btn btn-info" />
                            &nbsp;&nbsp; <asp:Button ID="Button1" runat="server" Text="Cancel" CssClass="btn btn-default" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>

