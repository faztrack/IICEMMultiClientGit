<%@ Page Title="Site Review" Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="customersitereview.aspx.cs" Inherits="customersitereview" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .opacityimage:hover {
            border-radius: 50px;
            transform: scale(1.5,1.5);
            transition: .3s;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left" width="32px">
                                </td>
                                <td align="left"><span class="titleNu">Site Review</span></td>

                                <td align="right">&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="90%">



                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" Text="Previous"
                                        OnClick="btnPrevious_Click" CssClass="prevButton" />
                                </td>
                                <td align="left" valign="middle">
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="right" width="45%"><span class="required">* </span>
                                                            <b>Start Date: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtStartDate" runat="server" Width="120px"></asp:TextBox>
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
                                            <td align="left">
                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                    <tr>
                                                        <td align="right" width="45%"><span class="required">* </span>
                                                            <b>End Date: </b>
                                                        </td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtEndDate" runat="server" Width="120px"></asp:TextBox>
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
                                            <td align="left">
                                                <asp:Button ID="btnSearch" runat="server" Text="View"
                                                    OnClick="btnSearch_Click" CssClass="button" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                                <td align="center">
                                    <b>Page: </b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                    &nbsp;
                                    <b>Item per page: </b>
                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>

                                    </asp:DropDownList>
                                </td>
                                <td align="right">
                                    <%-- <asp:Button ID="btnAddNew" runat="server" Text="Add New Site Review" CssClass="button" OnClick="btnAddNew_Click" />--%>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" Text="Next"
                                        OnClick="btnNext_Click" CssClass="nextButton" />
                                </td>


                            </tr>
                            <tr>
                                <td align="center" colspan="4">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="5">

                                    <asp:GridView ID="grdSiteViewList" runat="server" AutoGenerateColumns="False"
                                        Width="100%" AllowPaging="True"
                                        CssClass="mGrid" OnRowDataBound="grdSiteViewList_RowDataBound" OnPageIndexChanging="grdSiteViewList_PageIndexChanging">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>

                                              <asp:BoundField DataField="SiteReviewsDate" HeaderText="Date" DataFormatString="{0:d}">
                                                <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                              </asp:BoundField>
                                            <asp:TemplateField HeaderText=" Site Log">
                                                <ItemTemplate>
                                                    <table style="padding: 0px; margin: 0px; border: none; width: 100%">
                                                        <tr>
                                                            <td align="left" style="width: 40px;">
                                                                <asp:Label ID="lblNotesLabel" runat="server" Text=""></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:Label ID="lblSiteReviewNote" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr style="padding: 0px; margin: 0px; border: none;">
                                                            <td>
                                                                <asp:Label ID="lblImages" runat="server" Text=""></asp:Label>
                                                            </td>
                                                            <td style="padding: 0px; margin: 0px; border: none;">

                                                                <asp:GridView ID="grdUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                                    CssClass="uGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                                    OnRowDataBound="grdUploadedFileList_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField>
                                                                            <ItemTemplate>
                                                                                <div class="clearfix">
                                                                                    <div class="divImageCss" style="width: 70px; height: 42px;">
                                                                                        <asp:HyperLink ID="hypImg" runat="server" CssClass="hypimgCss" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput"  style="cursor:pointer;"/>
                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypPDF" runat="server" CssClass="hypimgCss" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgPDF" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypExcel" runat="server" CssClass="hypimgCss" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgExcel" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypDoc" runat="server" CssClass="hypimgCss"  Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgDoc" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypTXT" runat="server" CssClass="hypimgCss"  Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgTXT" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>

                                                                                    </div>
                                                                                   <%-- <div style="text-align: center; padding: 5px; font-weight: bold;">
                                                                                        <asp:Label ID="lblFileName" runat="server" Text="" Visible="false"></asp:Label>


                                                                                    </div>--%>
                                                                                </div>

                                                                            </ItemTemplate>
                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>



                                                            </td>
                                                        </tr>

                                                    </table>
                                                </ItemTemplate>
                                                <ItemStyle Width="60%" />
                                            </asp:TemplateField>
                                         

                                        </Columns>
                                        <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious0" runat="server"
                                        Text="Previous" CssClass="prevButton"
                                        OnClick="btnPrevious_Click" />
                                </td>

                                <td align="left" colspan="3">&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server"
                                        Text="Next" CssClass="nextButton"
                                        OnClick="btnNext_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSiteReviewId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnBackId" runat="server" Value="0" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/imagelightbox.js"></script>
    <script src="Scripts/main.js"></script>
    <script src="Scripts/jquery.elevatezoom.js"></script>
</asp:Content>


