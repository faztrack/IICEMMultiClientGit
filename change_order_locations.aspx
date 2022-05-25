<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="change_order_locations.aspx.cs" Inherits="change_order_locations" Title="Change Order Locations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <table cellpadding="0" cellspacing="2" width="100%">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">Change Order Locations</span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="4px" cellspacing="4px" width="900px" align="center">
                    <tr>
                        <td align="right">&nbsp;</td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table cellpadding="4" cellspacing="8" width="100%">
                                <tr>
                                    <td align="right" width="30%">
                                        <b>Customer Name:</b> </td>
                                    <td align="left" width="20%">
                                        <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td align="right" width="30%" valign="top">
                                        <b>Address:</b> </td>
                                    <td align="left">
                                        <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" width="30%">&nbsp;</td>
                                    <td align="left" width="20%">&nbsp;</td>
                                    <td align="right" width="30%" valign="top">&nbsp;</td>
                                    <td align="left">
                                        <asp:HyperLink ID="hypGoogleMap" runat="server" Target="_blank"
                                            ImageUrl="~/images/img_map.gif"></asp:HyperLink>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <b>Phone: </b></td>
                                    <td align="left">
                                        <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td align="right">
                                        <b>Email:</b> </td>
                                    <td align="left">
                                        <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <b>Estimate Name:</b> </td>
                                    <td align="left">
                                        <asp:Label ID="lblEstimateName" runat="server"></asp:Label>
                                    </td>
                                    <td align="right">&nbsp;
                                    </td>
                                    <td align="left">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <b>Change Order Name: </b></td>
                                    <td align="left">
                                        <asp:Label ID="lblChangeOrderName" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td align="right">&nbsp;</td>
                                    <td align="left">&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <table cellpadding="4" cellspacing="8" width="100%" align="center">
                                        <tr>
                                            <td align="center" valign="top">
                                                <h2>Select Change Order Locations</h2>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBoxList ID="chkLocations" runat="server" RepeatColumns="4"
                                                    Width="100%" TabIndex="2" ForeColor="Red">
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
                                                <asp:Button ID="btnContinueChangeOrder" runat="server" CssClass="button"
                                                    OnClick="btnContinueChangeOrder_Click" Text="Continue Change Order" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                                <asp:HiddenField ID="hdnChEstId" runat="server" Value="0" />
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
