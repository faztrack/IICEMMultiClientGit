<%@ Page Language="C#" AutoEventWireup="true" CodeFile="design_graphics.aspx.cs" Inherits="design_graphics" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link type="text/css" href="css/core.css" rel="stylesheet" />
    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <%--   <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>--%>
    <link href="css/imagelightbox.css" rel="stylesheet" />
    <link href="css/imagelightbox-ext.css" rel="stylesheet" />
    <title>Job Status Descriptions</title>

</head>
<body>
    <script language="javascript" type="text/javascript">
        function PrintWindow() {
            window.print();
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
                                    <asp:Label Style="color: #333333; font-size: 16px; font-weight: 500;" ID="lblCustomerName" runat="server" Text=""></asp:Label>
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
                    <td align="center" colspan="2">
                        <asp:GridView ID="grdDescriptions" runat="server" AutoGenerateColumns="False"
                            Width="99%">
                            <RowStyle CssClass="grid_row" />
                            <Columns>
                                <asp:BoundField DataField="status_description" HeaderText="Description">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                            <HeaderStyle CssClass="grid_header_style" />
                            <AlternatingRowStyle CssClass="grid_row_alt" />
                        </asp:GridView>
                    </td>
                </tr>



                <tr>
                    <td colspan="2" align="center">                       

                        <table class="wrapper" width="100%">
                            <tr>
                               <td style="text-align: left; color:#000000; font-weight:bold;"><asp:Label ID="lblImageGalleryTitle" runat="server" Visible="false">Image Gallery:</asp:Label></td>
                            </tr>
                            <tr>
                                <td style="" align="center" valign="top">
                                    <asp:GridView ID="grdCustomersImage" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                        CssClass="uGrid" OnRowDataBound="grdCustomersImage_RowDataBound" Width="100%"
                                        OnRowDeleting="grdCustomersImage_RowDeleting"
                                        OnRowEditing="grdCustomersImage_RowEditing"
                                        OnRowUpdating="grdCustomersImage_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="clearfix">
                                                        <div class="divImageCss">
                                                            <asp:HyperLink ID="hypImage" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="">
                                                                <asp:Image ID="img" runat="server" onerror="this.src='Images/No_image_available.jpg';" CssClass="imgCss blindInput" /><br />
                                                                <asp:Label ID="lblDescription" CssClass="lblDescripCss" runat="server" Text='<%# Eval("Desccription") %>'></asp:Label>
                                                            </asp:HyperLink><br />
                                                            <asp:TextBox ID="txtDescription" runat="server" Height="40px" Text='<%# Eval("Desccription") %>' TextMode="MultiLine" Visible="false" Width="110px" />
                                                        </div>
                                                        <div class="divEditCss">
                                                            <asp:ImageButton ID="imgEdit" runat="server" CssClass="iconEditCss blindInput" ImageUrl="~/images/icon_edit_16x16.png" CommandName="Edit" ToolTip="Edit" />
                                                        </div>
                                                        <div class="divDeleteCss">
                                                            <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" CommandName="Delete" ToolTip="Delete" />
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
                            <tr>
                              <td style="text-align: left; color:#000000; font-weight:bold;"><asp:Label ID="lblDocumentsTitle" runat="server" Visible="false">Documents:</asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="grdCustomersFile" runat="server" AutoGenerateColumns="False"
                                        CssClass="mGrid"
                                        OnRowDataBound="grdCustomersFile_RowDataBound" Width="100%"
                                        OnRowDeleting="grdCustomersFile_RowDeleting"
                                        OnRowEditing="grdCustomersFile_RowEditing"
                                        OnRowUpdating="grdCustomersFile_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Description">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Desccription") %>'></asp:Label>
                                                    <asp:TextBox ID="txtDescription" runat="server" Height="40px"
                                                        Text='<%# Eval("Desccription") %>' TextMode="MultiLine" Visible="false"
                                                        Width="110px" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ImageName" HeaderText="File Name"/>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypView" runat="server">View</asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit" />
                                            <asp:ButtonField CommandName="Delete" Text="Delete" />
                                        </Columns>
                                        <PagerStyle CssClass="pgr" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td colspan="2" align="center">
                         <asp:Button ID="btnDocumentManagement" runat="server" OnClick="btnDocumentManagement_Click" Text="Document Manager" CssClass="button" OnClientClick="ShowProgress();" />
                        <asp:Label ID="lblMessage" runat="server" />
                    </td>
                </tr>

                <tr style="display:none;">
                    <td align="right">

                        <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" />
                    </td>
                    <td align="left">
                        <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click"
                            Text="Upload New File for Design" CssClass="button" />

                    </td>
                </tr>

                  <tr style="display:none;">
                    <td align="center" colspan="2">
                        <asp:GridView ID="grdTemp" runat="server" AutoGenerateColumns="False"
                            OnRowDataBound="grdTemp_RowDataBound" class="mGrid" Width="314px">
                            <Columns>
                                <asp:TemplateField HeaderText="File Name">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hyp" runat="server">[hyp]</asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="File Description">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtDes" runat="server"
                                            Text='<%# Eval("file_description").ToString() %>' Width="221px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

                   <tr style="display:none;">
                    <td align="center" colspan="2">
                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click"
                            Text="Save Uploaded File" CssClass="button" Width="164px" Visible="False" />
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
    </form>
    <script type="text/javascript" src="Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="Scripts/imagelightbox.js"></script>
    <script type="text/javascript" src="Scripts/main.js"></script>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
    </script>
</body>
</html>
