<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="companycoverletter.aspx.cs" Inherits="companycoverletter" Title="Cover Letter" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Cover Letter</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="15px"></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:TextBox ID="txtCoverLetter" runat="server" Height="432px"
                            TextMode="MultiLine" Width="911px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="15px"></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="15px"></td>
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
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

