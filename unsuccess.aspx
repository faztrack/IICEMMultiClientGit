<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="unsuccess.aspx.cs" Inherits="unsuccess" Title="Unsuccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <table cellpadding="0" cellspacing="4" width="100%" align="center">
        <tr>
            <td align="center">
                <h1>Unsuccess</h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="2" width="98%" align="center">
                    <tr>
                        <td align="center">
                            <h3><asp:Label ID="lblMessage" runat="server" 
                                    Text="Payment did not process successfully." ForeColor="Red"></asp:Label></h3>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table style="width: 100%">
                                <tr>
                                    <td align="right">
                                        <b>Possible Reason: </b>
                                    </td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;</td>
                                    <td align="left">
                            <asp:Label ID="lblReason" runat="server" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="height: 10px">
                            </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnPaymentInfo" runat="server" 
                                Text="Go to Payment Information Page" CssClass="button" 
                                onclick="btnPaymentInfo_Click"/>
                            <asp:Button ID="btnCustomerDetails" runat="server" Text="Customer Details" 
                                CssClass="button" onclick="btnCustomerDetails_Click"/>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstPaymentId" runat="server" Value="0" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

