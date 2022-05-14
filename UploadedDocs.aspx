<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="UploadedDocs.aspx.cs" Inherits="UploadedDocs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">       
        .hypimgCss {
            padding: 5px;
        }

        .imgCss {
            padding: 5px;
            height: 150px;
            width: 150px;
        }
        .uGrid {
            border: none;
        }

            .uGrid tr {
                display: inline-block;
                padding: 5px;
                border: none;
            }

            .uGrid td {
                padding: 5px;
                border: none;
            }

        .iconEditCss {
            margin-left: 10px;
        }

        .iconDeleteCss {
            margin-right: 10px;
        }
    </style>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Estimate Pricing</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 100%">
                    <%--  <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>--%>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" align="center">
                        <tr>
                            <td align="center">
                                <table class="wrapper" width="100%">
                                    <tr>
                                        <td style="width: 220px; border-right: 1px solid #ddd;" align="left" valign="top">
                                            <table width="100%">
                                                <tr>
                                                    <td width="74px">
                                                        <img src="images/icon-customer-info.png" /></td>
                                                    <td align="left">
                                                        <h2>Customer Information</h2>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="width: 390px;" align="left" valign="top">
                                            <table style="width: 390px;">
                                                <tr>
                                                    <td style="width: 112px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                    <td style="width: auto; height: 18px;">
                                                        <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="top"><b>Phone: </b></td>
                                                    <td style="width: auto;">
                                                        <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="top"><b>Email: </b></td>
                                                    <td style="width: auto;">
                                                        <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left" valign="top">
                                            <table>
                                                <tr>
                                                    <td style="width: 100px;" align="left" valign="top"><b>Address: </b></td>
                                                    <td align="left" valign="top">
                                                        <asp:Label ID="lblAddress" runat="server"></asp:Label>
                                                    </td>
                                                    <td align="left" valign="top">&nbsp;
                                                                <asp:HyperLink ID="hypGoogleMap" runat="server" ImageUrl="~/images/img_map.gif" Target="_blank"></asp:HyperLink>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td align="left" valign="top"><b>Sales Person:</b>&nbsp;</td>
                                                    <td align="left" valign="top">
                                                        <asp:Label ID="lblSalesPerson" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <table class="wrapper" width="100%">
                                    <tr>
                                        <td style="width: 220px; border-right: 1px solid #ddd;" align="left" valign="top">
                                            <table width="100%">
                                                <tr>
                                                    <td width="74px">
                                                        <img src="images/icon_camera_3.png" /></td>
                                                    <td align="left">
                                                        <h2>Images</h2>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="" align="left" valign="top">
                                         
                                            <asp:GridView ID="grdCustomersFile" runat="server" AutoGenerateColumns="False" ShowHeader="false" 
                                                CssClass="uGrid" OnRowDataBound="grdCustomersFile_RowDataBound" Width="100%"
                                                OnRowDeleting="grdCustomersFile_RowDeleting"
                                                OnRowEditing="grdCustomersFile_RowEditing"
                                                OnRowUpdating="grdCustomersFile_RowUpdating">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Description">
                                                        <ItemTemplate>
                                                            <div style="width: 100%;">
                                                                <div style="width: 100%;">
                                                                    <asp:HyperLink ID="hypImage" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="Sunset in Tanzania">
                                                                        <asp:Image ID="img" runat="server" CssClass="imgCss" />
                                                                    </asp:HyperLink><br />
                                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Desccription") %>'></asp:Label>
                                                                    <asp:TextBox ID="txtDescription" runat="server" Height="40px"
                                                                        Text='<%# Eval("Desccription") %>' TextMode="MultiLine" Visible="false"
                                                                        Width="110px" />
                                                                </div>
                                                                <div style="width: 50%; float: left;">
                                                                    <asp:ImageButton ID="imgEdit" runat="server" CssClass="iconEditCss" ImageUrl="~/images/icon_edit.png" CommandName="Edit" ToolTip="Edit" />
                                                                </div>
                                                                <div style="width: 50%; float: right; text-align: right;">
                                                                    <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss" ImageUrl="~/images/icon_delete.png"  CommandName="Delete" ToolTip="Delete" />
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <PagerStyle CssClass="" />
                                                <AlternatingRowStyle CssClass="" />
                                            </asp:GridView>
                                         
                                        </td>
                                        <td align="left" valign="top"></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </table>
                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                    <%-- </ContentTemplate>
                    </asp:UpdatePanel>--%>
                </div>
            </td>
        </tr>
    </table>
    <%-- <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2"
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

    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/imagelightbox.js"></script>
    <script src="Scripts/main.js"></script>

    <script type="text/javascript">

        //var prm = Sys.WebForms.PageRequestManager.getInstance();
        //if (prm != null) {
        //    prm.add_endRequest(function (sender, e) {
        //        if (sender._postBackSettings.panelsToUpdate != null) {
        //            var gallery = $('a[data-imagelightbox="h"]').imageLightbox({
        //                arrows: true
        //            });
        //            $('.trigger_lightbox').on('click', function () {
        //                gallery.startImageLightbox();
        //            });

        //        }
        //    });
        //};
    </script>
</asp:Content>

