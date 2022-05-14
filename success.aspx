<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="success.aspx.cs" Inherits="success" Title="Success" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <table cellpadding="0" cellspacing="4" width="100%" align="center">
        <tr>
            <td align="center">
                <h1>Success</h1>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="2" width="98%" align="center">
                    <tr>
                        <td align="center">
                            <h3><asp:Label ID="lblMessage" runat="server" 
                                    Text="Payment processed successfully." ForeColor="Green"></asp:Label></h3>
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

