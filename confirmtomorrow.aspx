<%@ Page Title="Confirm Tomorrow" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="confirmtomorrow.aspx.cs" Inherits="confirmtomorrow" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function selected_ItemName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }

        function SearchItemNamePress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;

        }
    </script>

     <script>

         function openModal() {
             $('#ConfirmMSG').modal({ show: true });
         }

    </script>
    
    <style>
        /* (320x480) Smartphone, Portrait */
        /* and (max-device-width : 480px) and (-webkit-device-pixel-ratio : 3) */

        @media only screen and (min-device-width: 320px) and (orientation: portrait) {
            .gridResponsive {
                transform: scale(.65);
                -webkit-transform: scale(.65); /* Safari and Chrome */
                -moz-transform: scale(.65); /* Firefox */
                margin: -540px -108px !important;
            }
            /* (320x480) Smartphone, Portrait */
        }

        /* iPhone X-XS in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 812px) and (-webkit-device-pixel-ratio : 3) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone X-XS in portrait */
        }

        /* iPhone 6, 7, & 8 in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 667px) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone 6, 7, & 8 in portrait */
        }


        @media (min-width: 1281px) {
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
  ##Device = Laptops, Desktops
  ##Screen = B/w 1025px to 1280px
*/

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
                    font-size: 9px;
                }

                .mobileGrid tbody tr td {
                    padding: 2px;
                    margin: 5px;
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




        input[type=radio] + label, input[type=checkbox] + label {
            display: inline-block;
            margin: -6px 0 0 0;
            padding: 3px;
            margin-bottom: 0;
            font-size: 12px;
            line-height: 20px;
            color: #333;
            text-align: center;
            text-shadow: 0 1px 1px rgba(255,255,255,0.75);
            vertical-align: middle;
            cursor: pointer;
            font-weight: bold;
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
                                <strong>Confirm Tomorrow
                                    
                                </strong>

                            </h3>

                        </div>
                         <!-- ConfirmMSG View Modal content-->
                            <div id="ConfirmMSG" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false" style="z-index: 9999999">
                                <div class="modal-dialog">

                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" style="margin-top:-10px">X</button>
                                            
                                        </div>
                                        <div class="modal-body">
                                             <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                        </div>

                                    </div>

                                </div>
                            </div>

                            <%--       End ConfirmMSG--%>

                        <div class="panel-body">
                            <div class="table-responsive">
                                <table cellpadding="0" cellspacing="0" width="100%">

                                    <tr runat="server" visible="false">
                                        <td style="padding: 5px 0;" align="left" valign="top">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td width="90%" align="left">
                                                        <asp:TextBox ID="txtSearch" onkeypress="return SearchItemNamePress(event);" runat="server" CssClass="form-control form-control-ext" Width="100%" Style="margin-left: 5px;"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                        </cc1:AutoCompleteExtender>
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                                    </td>
                                                    <td align="right" width="5%">
                                                        <asp:Button ID="btnSearch" runat="server" Width="80px" CssClass="btn btn-default" OnClick="btnSearch_Click" Text="Search" />
                                                    </td>
                                                    <td align="Center" width="2%">
                                                        <asp:LinkButton ID="LinkButton1" runat="server" Width="80px"  OnClick="LinkButton1_Click">View All</asp:LinkButton>
                                                    </td>
                                                    <td>&nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    
                                    </tr>
                                    <tr>
                                        <td align="center" style="padding:5px 0;" width="100%">
                                             <asp:Button ID="btnSave" runat="server" Width="80px" CssClass="btn btn-default"  OnClick="btnSave_Click" Text="Submit" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" width="100%">
                                            <p style="font-size:18px;">Confirm your jobs for tomorrow</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" width="100%">
                                            <asp:GridView ID="gridEvent" runat="server" AutoGenerateColumns="False" CssClass="mobileGrid" OnRowDataBound="gridEvent_RowDataBound"
                                            Width="100%">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Customer">
                                                       
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("last_name1").ToString() %>'></asp:Label>
   
                                                        </ItemTemplate>

                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Project">
                                                       
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblProject" runat="server" Text='<%# Eval("estimate_name").ToString() %>'></asp:Label>
   
                                                        </ItemTemplate>

                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Sec/Loc">
                                                       
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSection" runat="server" Text=''></asp:Label></br>
                                                            <asp:Label ID="lblLocation" runat="server" Text=''></asp:Label>
   
                                                        </ItemTemplate>


                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Can Start Tomorrow?">
                                                       
                                                        <ItemTemplate>

                                                           <asp:RadioButtonList ID="rdbStart" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdbStart_SelectedIndexChanged">
                                                              <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                               <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                          </asp:RadioButtonList>
                                                          
   
                                                        </ItemTemplate>

                                                    </asp:TemplateField>

                                                </Columns>
                                                <PagerStyle CssClass="pgr" />
                                                <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                <RowStyle BackColor="#f1f1f1" />


                                                <FooterStyle CssClass="white_text" />

                                                <PagerStyle CssClass="pgr" />
                                              
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td align="center" style="padding:5px 0;" width="100%">
                                             <asp:Button ID="btnSave2" runat="server" Width="80px" CssClass="btn btn-default"  OnClick="btnSave_Click" Text="Submit" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                </div>


            </div>
        </ContentTemplate>
    </asp:UpdatePanel>





    <%--  <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1"
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


