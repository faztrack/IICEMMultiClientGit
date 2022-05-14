<%@ Page Title="Site Review Details" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="sitereviewdetails.aspx.cs" Inherits="sitereviewdetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .divImageCss {
            padding: 24px;
        }
        .opacityimage {
            border-radius:50px;
            transform:scale(1.5,1.5);
            transition:.3s;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left" width="32px" > <asp:ImageButton ID="imgBack" runat="server" CssClass="noEffectNew" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_web.png"  Height="25px" /></td>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader"></asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="80%">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>

                                <td align="right"><span class="required">* </span>Date: </td>

                                <td align="left" valign="middle">
                                    <table style="padding: 0px; margin: -4px;">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtSiteReviewDate" runat="server" TabIndex="1"></asp:TextBox></td>
                                            <td>
                                                <asp:ImageButton CssClass="nostyleCalImg" ID="imgsiteReview" runat="server" ImageUrl="~/images/calendar.gif" />
                                                <cc1:CalendarExtender ID="siteReview" runat="server"
                                                    Format="MM/dd/yyyy" PopupButtonID="imgsiteReview"
                                                    PopupPosition="BottomLeft" TargetControlID="txtSiteReviewDate">
                                                </cc1:CalendarExtender>
                                            </td>
                                        </tr>
                                    </table>
                                </td>

                            </tr>

                            <tr>
                                <td align="right" valign="top">Notes: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtSiteReviewNote" runat="server" TabIndex="2" TextMode="MultiLine" CssClass="textBox form-control" Height="250px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'3000',document.getElementById('head_txtDisplay'));" MaxLength="3000"></asp:TextBox>
                                    <asp:TextBox
                                        ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        Height="16px" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">Viewable By: </td>
                                <td align="left">
                                   &nbsp;<asp:CheckBox ID="chkCustomer" runat="server" TabIndex="3" /> &nbsp;Customer
                                </td>
                            </tr>
                            <tr>
                                <td align="right">State of Mind:
                                                
                                </td>
                                <td align="left">

                                    <table width="100%">
                                        <tr>
                                            <td align="left" width="3%">
                                                <asp:CheckBox ID="chkCustomerMind" runat="server" AutoPostBack="true" OnCheckedChanged="chkCustomerMind_CheckedChanged" /></td>
                                            <td align="left" width="100%">

                                                <asp:Panel ID="pnlCustomerStateofMind" CssClass="emojiInput" runat="server" Visible="false">
                                                    <asp:ImageButton CssClass="" ID="imgbtnAngry" runat="server" ImageUrl="~/assets/customerstatemind/angry.png" Width="30px" Height="30px" ToolTip="Angry" OnClick="imgbtnAngry_Click" />
                                                    <asp:ImageButton CssClass="" ID="imgbtnFrustrated" runat="server" ImageUrl="~/assets/customerstatemind/frustrated.png" Width="30px" Height="30px" ToolTip="Frustrated" OnClick="imgbtnFrustrate_Click"/>
                                                    <asp:ImageButton CssClass="" ID="imgbtnConfused" runat="server" ImageUrl="~/assets/customerstatemind/confused.png" Width="30px" Height="30px" ToolTip="Confused"  OnClick="imgbtnConfused_Click"/>
                                                    <asp:ImageButton CssClass="" ID="imgbtnIndifferent" runat="server" ImageUrl="~/assets/customerstatemind/indifferent.png" Width="30px" Height="30px" ToolTip="Indifferent "  OnClick="imgbtnIndifferent_Click"/>
                                                    <asp:ImageButton CssClass="" ID="imgbtnHappy" runat="server" ImageUrl="~/assets/customerstatemind/happy.png" Width="30px" Height="30px" ToolTip="Happy" OnClick="imgbtnHappay_Click" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">Notification: </td>
                                <td align="left">
                                   &nbsp;<asp:CheckBox ID="chkEmail" runat="server" TabIndex="3" Checked="True" /> &nbsp;Email
                                </td>
                            </tr>
                            <tr>
                                <td width="16%" align="right">Upload Attachments: </td>
                                <td align="left">
                                    <asp:Button ID="btnUpload" runat="server" Text="" CssClass="noEffectNew" OnClick="btnUpload_Click" />
                                    <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" onchange="UploadFile()" TabIndex="4" />
                                 
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Save" TabIndex="5"
                                        CssClass="button" OnClick="btnSubmit_Click" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="6" OnClick="btnCancel_Click"
                                        CssClass="button" />
                                    &nbsp;<asp:Button ID="btnAddNew" runat="server" Text="Add New +" TabIndex="7"
                                        CssClass="button" OnClick="btnAddNew_Click" Visible="false" />
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="2">
                                    <asp:Label ID="lblUpload" runat="server" Text="Must Save to Upload All files" Font-Bold="true" Style="text-align: center" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Panel ID="pnlTemporaryImageUpload" runat="server" Visible="false">
                                        <h3 style="font-weight: bold;">Temporary uploaded Files</h3>

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
                                                            <div style="padding-left: 10px; padding-top: 5px;" class="divEditCss">
                                                                <asp:Label ID="lblFileName" runat="server" Font-Bold="true" Text="" Visible="false"></asp:Label>
                                                            </div>
                                                            <div style="float: right;">
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
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Panel ID="pnlDetailImageUpload" runat="server" Visible="false">
                                        <h3 style="font-weight: bold;">All Attached Files:</h3>
                                        <asp:GridView ID="grdImageDetails" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                            CssClass="uGrid" Width="100%" OnRowDataBound="grdImageDetails_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <div class="clearfix">
                                                            <div class="divImageCss">

                                                                <asp:HyperLink ID="hypImg1" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                    <asp:Image ID="img1" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput"  style="cursor:pointer;"/>
                                                                </asp:HyperLink>

                                                                <asp:HyperLink ID="hypPDF1" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                    <asp:Image ID="imgPDF1" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                </asp:HyperLink>
                                                                <asp:HyperLink ID="hypExcel1" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                    <asp:Image ID="imgExcel1" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                </asp:HyperLink>
                                                                <asp:HyperLink ID="hypDoc1" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                    <asp:Image ID="imgDoc1" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                </asp:HyperLink>
                                                                <asp:HyperLink ID="hypTXT1" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                    <asp:Image ID="imgTXT1" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                </asp:HyperLink>



                                                            </div>
                                                            <div style="padding-left: 10px; padding-top: 5px;" class="divEditCss">
                                                                <asp:Label ID="lblFileName1" runat="server" Font-Bold="true" Text="" Visible="false"></asp:Label>
                                                            </div>
                                                            <div style="float: right;">
                                                                <asp:ImageButton ID="imgDelete1" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteViewDetailImage" />
                                                            </div>

                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="" />
                                            <AlternatingRowStyle CssClass="" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>

                            <tr>
                                <td align="center" colspan="2">
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSiteReviewId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerMind" runat="server" Value="0" />
                                     <asp:HiddenField ID="hdnBackId" runat="server" Value="0" />
                                       <asp:HiddenField ID="hdnSalesEmail" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSuperandentEmail" runat="server" Value="" />
                                     <asp:HiddenField ID="hdnLastName" runat="server" Value="" />
                                     <asp:HiddenField ID="hdnEmailType" runat="server" Value="0" />

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
    </asp:UpdatePanel>
    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
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
    <script language="javascript" type="text/javascript">

        function UploadFile() {

            $("#<%=btnUpload.ClientID %>")[0].click();
        }
    </script>
</asp:Content>

