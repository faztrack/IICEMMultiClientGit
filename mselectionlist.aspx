<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mselectionlist.aspx.cs" Inherits="mselectionlist" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .clearfix2 {
            width: 100%;
            box-shadow: 0 0 20px #cdcdcd;
            border-radius: 5px;
        }

        .divImageCssSelection {
            height: 70px;
            width: 70px;
            position: relative;
        }
    </style>
    <div class="row">
        <div class="col-lg-12 col-sm-12 col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading panel-heading-ext">

                    <h3 class="panel-title">
                        <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                        <strong>Selection&nbsp;<asp:Label ID="lblCustomerLastName" runat="server" Text=""></asp:Label>
                        </strong>

                    </h3>

                </div>
                <div class="panel-body">

                    <asp:GridView ID="grdSelection" runat="server" AutoGenerateColumns="False"
                        CssClass="mGrid"
                        PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdSelection_RowDataBound" ShowHeader="false">

                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <table style="padding: 0px; margin: 0px; border: none;">
                                        <tr style="padding: 0px; margin: 0px; border: 0px;">
                                            <td style="padding: 0px; margin: 0px; border:none;">
                                                  <asp:GridView ID="grdUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                    CssClass="uGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                    OnRowDataBound="grdUploadedFileList_RowDataBound">
                                                    <Columns>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>

                                                                <div class="clearfix2">
                                                                    <div class="divImageCssSelection">

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
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                                <div class="row">
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblDate" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblSection" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblLocation" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblTitle" runat="server" Text=""></asp:Label>
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblNotes" runat="server" Text=""></asp:Label>
                                                        <asp:Label ID="lblNotes_r" runat="server" Text='<%# Eval("Notes") %>' Visible="false" Style="text-align:justify"></asp:Label>
                                                        <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                    </div>
                                                    <div class="col-sm-12">
                                                        <asp:Label ID="lblPrice" runat="server" Text=""></asp:Label>
                                                    </div>
                                                </div>
                                            </td>


                                        </tr>

                                    </table>
                                </ItemTemplate>
                                <ItemStyle Width="100%" HorizontalAlign="left" />
                            </asp:TemplateField>

                        </Columns>
                        <AlternatingRowStyle CssClass="alt" />
                    </asp:GridView>






                </div>
            </div>

        </div>

        <div>
            <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
            <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />

        </div>

    </div>

</asp:Content>

