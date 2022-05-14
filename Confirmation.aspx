<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Confirmation.aspx.cs" Inherits="Confirmation" Title="Payment Confirmation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Payment Confirmation</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table id="Table1" border="1" bordercolor="silver" width="750px">
                            <tr>
                                <td align="right" width="45%">
                                    <b>Fullt Name: </b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblFullName" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <b>Address: </b></td>
                                <td align="left">
                                    <asp:Label ID="lblAddress" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <asp:Label ID="lblLCCNo" runat="server" Font-Bold="True">Credit Card Number:</asp:Label></td>
                                <td align="left">
                                    <asp:Label ID="lblccNumber" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <asp:Label ID="lblLExpMonth" runat="server" Font-Bold="True">Expire Month: </asp:Label></td>
                                <td align="left">
                                    <asp:Label ID="lblExpMonth" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <asp:Label ID="lblLExpYear" runat="server" Font-Bold="True">Expire Year: </asp:Label></td>
                                <td align="left">
                                    <asp:Label ID="lblExpYear" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <asp:Label ID="lblLcvv" runat="server" Font-Bold="True">CVV:</asp:Label></td>
                                <td align="left">
                                    <asp:Label ID="lblcvv" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right" width="45%">
                                    <b>Amount: </b></td>
                                <td align="left">
                                    <asp:Label ID="lblAmount" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblTrans" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblReason" runat="server" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button"
                                        Width="100px" OnClick="btnBack_Click" />
                                    <asp:Button ID="btnConfirm" runat="server" CssClass="button" Text="Confirm"
                                        Width="100px" OnClick="btnConfirm_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:HiddenField ID="hdn" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstPayId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
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



