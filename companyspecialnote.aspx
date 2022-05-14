<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="companyspecialnote.aspx.cs" Inherits="companyspecialnote" Title="Special Note" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Special Note (By deafult this special notes will show in payment info page of each estimate)</span></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="padding-left:20px;" cellpadding="0" cellspacing="0" width="680px">
                            <tr align="right">
                                <td align="left">Write your special notes below:(500 Chars Max)
                                    <asp:TextBox
                                        ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                                        BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                        Height="16px" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:TextBox ID="txtSpecialNote" runat="server" Height="300px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplay'));" TextMode="MultiLine" Width="650px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" height="15px"></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" height="15px"></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="button"
                                        OnClick="btnSubmit_Click" Width="80px" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                        OnClick="btnCancel_Click" Width="80px" />
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" height="15px"></td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

