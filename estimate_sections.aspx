<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="estimate_sections.aspx.cs" Inherits="estimate_sections" Title="Estimate Sections" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <style>
        input[type=radio] + label, input[type=checkbox] + label {
            display: inline-block;
            margin: -6px 0 0 0;
            padding: 4px 5px;
            margin-bottom: 0;
            font-size: 12px;
            line-height: 20px;
            text-align: center;
            text-shadow: 0 1px 1px rgba(255,255,255,0.75);
            vertical-align: middle;
            cursor: pointer;
        }
    </style>
    <table cellpadding="0" cellspacing="2" width="100%">

        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Estimate Sections</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="4px" cellspacing="4px" width="100%" align="center">
                    <tr>
                        <td align="center">
                            <table class="wrapper" width="100%">
                                <tr>
                                    <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td>
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
                                                <td style="width: 200px;" align="left" valign="top"><b>Customer Name: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 200px;" align="left" valign="top"><b>Phone: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 200px;" align="left" valign="top"><b>Email: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 200px;" align="left" valign="top"><b>Estimate Name:</b> </td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblEstimateName" Font-Bold="true" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left" valign="top">
                                        <table style="width: 420px;">
                                            <tr>
                                                <td style="width: 64px;" align="left" valign="top"><b>Address: </b></td>
                                                <td style="width: auto;" align="left" valign="top">
                                                    <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                                <td style="width: auto;" align="left" valign="top">
                                                    <asp:HyperLink ID="hypGoogleMap" runat="server" Target="_blank"
                                                        ImageUrl="~/images/img_map.gif"></asp:HyperLink>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <table cellpadding="4" cellspacing="8" width="100%" align="center" class="wrapper">
                                        <tr>
                                            <td colspan="2">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="left" width="100px;">
                                                            <asp:CheckBox ID="chkAll" runat="server" Text="Select All" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="true"  Font-Bold="true"/>

                                                        </td>
                                                        <td align="left" valign="top" width="200px;">
                                                            <h2>Select Estimate Sections</h2>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <asp:CheckBoxList ID="chkSections" runat="server" RepeatColumns="4"
                                                    Width="100%" TabIndex="2" OnSelectedIndexChanged="chkSections_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:CheckBoxList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Button ID="btnContinue" runat="server" Text="Continue to Pricing"
                                                    OnClick="btnContinue_Click" TabIndex="3" CssClass="button" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                &nbsp;<asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False"
                                                    Value="0" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2" DynamicLayout="False">
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

                        </td>
                    </tr>


                </table>
            </td>
        </tr>
    </table>
</asp:Content>

