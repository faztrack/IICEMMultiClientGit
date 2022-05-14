<%@ Page Language="C#" AutoEventWireup="true" CodeFile="image_gallery.aspx.cs" Inherits="image_gallery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link type="text/css" href="css/core.css" rel="stylesheet" />
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <%-- <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>--%>
    <link href="css/imagelightbox.css" rel="stylesheet" />
    <link href="css/imagelightbox-ext.css" rel="stylesheet" />
    <script src="CommonScript.js"></script>
    <title>Job Status Descriptions</title>
</head>
<body>
    <script language="javascript" type="text/javascript">
        function PrintWindow() {
            window.print();
        }

        function confirmDelete() {
            return confirm("Are you sure that you want to delete this Item?");
        }
    </script>
    <form id="form1" runat="server">
        <div>
            <table class="body2bg" align="center" border="0" cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td align="center" style="background-color: #ddd; color: #fff;" colspan="2">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left" class="titleNu">
                                    <asp:Label ID="lblJobStatusFor" CssClass="titleNu" runat="server" Text=""></asp:Label>
                                    for:
                                    <asp:Label Style="color: #330f02; font-size: 16px; font-weight: 500;" ID="lblCustomerName" runat="server" Text=""></asp:Label>

                                </td>
                                <td align="right">
                                    <asp:LinkButton CssClass="button" ID="lnkPrint" runat="server" Style="padding-left: 10px;">Print</asp:LinkButton>
                                    <asp:LinkButton CssClass="button" ID="lnkClose" runat="server" Style="padding-left: 10px;">Close</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td colspan="2" align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <%--<td style="text-align: left; color:#992a24; font-weight:bold; "><asp:Label ID="lblImageGalleryTitle" runat="server" Visible="false">Image Gallery:</asp:Label>                                  
                                </td>--%>
                            </tr>
                            <tr>
                                <td style="" align="center" valign="top">
                                    <asp:GridView ID="grdCustomersImage" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                        CssClass="uGrid" OnRowDataBound="grdCustomersImage_RowDataBound" Width="100%"
                                        OnRowDeleting="grdCustomersImage_RowDeleting"
                                        OnRowEditing="grdCustomersImage_RowEditing"
                                        OnRowUpdating="grdCustomersImage_RowUpdating" OnRowCommand="grdCustomersImage_RowCommand">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="clearfix">
                                                        <asp:HyperLink ID="hypImage" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="">
                                                            <div class="divImageCss">
                                                                <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />
                                                            </div>
                                                            <div style="width: 100%;">
                                                                <asp:Label ID="lblDescription" CssClass="lblDescripCss" runat="server" Text='<%# Eval("Desccription") %>'></asp:Label>
                                                                <asp:TextBox ID="txtDescription" CssClass="cssDesc" runat="server" Height="40px" Text='<%# Eval("Desccription") %>' TextMode="MultiLine" Visible="false" Width="110px" />
                                                            </div>
                                                        </asp:HyperLink>
                                                        <div class="divEditCss">
                                                            <asp:ImageButton ID="imgEdit" runat="server" CssClass="iconEditCss blindInput" ImageUrl="~/images/icon_edit_16x16.png" CommandName="Edit" ToolTip="Edit" />
                                                        </div>
                                                        <div class="divRotateCss">
                                                            <asp:ImageButton ID="imgRotate" runat="server" CssClass="iconEditCss blindInput" ImageUrl="~/images/icon_rotate_16x16.png" CommandName="Rotate" ToolTip="Rotate" CommandArgument='<%# Container.DataItemIndex%>' />
                                                        </div>
                                                        <div class="divDeleteCss">
                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" OnClientClick="return confirmDelete();" ImageUrl="~/images/icon_delete_16x16.png" CommandName="Delete" ToolTip="Delete" />
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
                    </td>
                </tr>

                <tr>
                    <td colspan="2" align="center">
                        <asp:Label ID="lblMessage" runat="server" />
                    </td>
                </tr>


                <tr>
                    <td align="left" valign="top" colspan="2">
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="LoadingProgress" style="display: none">
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </div>
    </form>
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/imagelightbox.js"></script>
    <script src="Scripts/main.js"></script>
    <script src="Scripts/jquery.elevatezoom.js"></script>
    <script type="text/javascript">

        //$(".imgCss").elevateZoom({
        //    responsive: true,
        //    //   zoomCaption: 'qweqwewqe',
        //    //zoomWindowWidth: 550,
        //    //zoomWindowHeight: 450,         
        //    cursor: '-webkit-zoom-in; cursor: -moz-zoom-in; cursor: zoom-in',
        // //   galleryActiveClass: 'active',
        // //   imageCrossfade: false,
        //    loadingIcon: 'Images/Imgloading.gif',
        //    scrollZoom: true,
        //    easing: true,
        //    zoomWindowPosition: 1,
        //    zoomWindowOffetx: 10
        //});

        function CloseWindow() {
            window.close();
        }
    </script>
</body>
</html>
