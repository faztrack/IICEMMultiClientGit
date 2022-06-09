<%@ Page Title="Site Review List" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="sitereviewlist.aspx.cs" Inherits="sitereviewlist" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .opacityimage:hover {
            border-radius: 50px;
            transform: scale(1.5,1.5);
            transition: .3s;
        }
    </style>
    <script>
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left" width="32px">
                                    <asp:ImageButton ID="imgBack" runat="server" CssClass="noEffectNew" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_web.png" Height="25px" /></td>
                                <td align="left"><span class="titleNu">Site Review List &nbsp;(<asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>)</span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label></td>

                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
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
                                                <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">View All</asp:LinkButton>
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
                                    <asp:Button ID="btnAddNew" runat="server" Text="Add New Site Review" CssClass="button" OnClick="btnAddNew_Click" />
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
                                            <%-- cell 0 --%>
                                            <asp:TemplateField HeaderText="Date" ItemStyle-Font-Underline="true">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypSiteReview" runat="server"></asp:HyperLink><br />
                                                    <br />
                                                    <asp:Image ID="imgStateOfMind" runat="server" Width="40px" Height="40px" CssClass="opacityimage" />
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>

                                            <%-- cell 1 --%>
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
                                                                                        <asp:HyperLink ID="hypImg" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" Style="cursor: pointer;" />
                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypPDF" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgPDF" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypExcel" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgExcel" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypDoc" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgDoc" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>
                                                                                        <asp:HyperLink ID="hypTXT" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                            <asp:Image ID="imgTXT" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="gsrImg blindInput" />

                                                                                        </asp:HyperLink>

                                                                                    </div>
                                                                                    <%--<div style="text-align: center; padding: 5px; font-weight: bold;">
                                                                                        <asp:Label ID="lblFileName" runat="server" Text=""></asp:Label>
                                                                                        
                                  
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
                                                <ItemStyle Width="80%" />
                                            </asp:TemplateField>

                                            <%-- cell 2 --%>
                                            <asp:TemplateField HeaderText="Division">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDivisionName" runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>

                                            <%-- cell 3 --%>
                                            <asp:BoundField DataField="CreatedBy" HeaderText="Added By">
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>

                                            <%-- cell 4 --%>
                                            <asp:BoundField HeaderText="Customer View">
                                                <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>

                                            <%-- cell 5 --%>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFile" />
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Center" />
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
                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                    <asp:HiddenField ID="hdnPrimaryDivision" runat="server" Value="0" />
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
</asp:Content>
