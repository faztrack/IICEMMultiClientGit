<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="me_sections.aspx.cs" Inherits="me_sections" Title="Estimation Template Sections" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table cellpadding="0" cellspacing="2" width="100%">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Estimation Template Sections</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="4px" cellspacing="4px" width="100%" align="center">
                    <script language="Javascript" type="text/javascript">
                        function ChangeImage(id) {
                            document.getElementById(id).src = 'Images/loading.gif';
                        }
                    </script>
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
                                                    <h2>Sales Person</h2>
                                                    <h2>Information</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width: 390px;" align="left" valign="top">
                                        <table style="width: 390px;">
                                            <tr>
                                                <td style="width: 200px;" align="left" valign="top">
                                                    <b>Sales Person Name: </b></td>
                                                <td style="width: auto;">
                                                    <asp:Label ID="lblSalesPersonName" runat="server" Text=""></asp:Label>
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
                                                    <asp:Label ID="lblModelEstimateName" runat="server"></asp:Label>
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
                                                <td style="width: auto;" align="left" valign="top">&nbsp;</td>
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
                                            <td align="center" valign="top">
                                                <h2>Select Estimation Template Sections</h2>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBoxList ID="chkSections" runat="server" RepeatColumns="4"
                                                    Width="100%" TabIndex="2">
                                                </asp:CheckBoxList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnBacktoLocations" runat="server" CssClass="button"
                                                    OnClick="btnBacktoLocations_Click" Text="Back to Locations" />
                                                <asp:Button ID="btnContinue" runat="server" Text="Continue to Pricing"
                                                    TabIndex="3" CssClass="button" OnClick="btnContinue_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
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

