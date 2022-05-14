<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="coverpage.aspx.cs" Inherits="coverpage" Title="Cover Page" %>
<%@ Register TagPrefix="CE" Namespace="CuteEditor" Assembly="CuteEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <table  width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Cover Page</span></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="15px"></td>
                </tr>
                <tr>
                    <td align="center" style="width:100%">
                        <table style="padding-left:20px;" cellpadding="0" cellspacing="0" width="1000px">
                           
                            <tr>
                                 
                                <td align="center" >
                                    <CE:Editor id="txtCoverPage" runat="server" Height="600px" Width="850px" />
                                     
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
                                         Width="80px" OnClick="btnSave_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                         Width="80px" OnClick="btnClose_Click" />
                                     <asp:Button ID="Button1" runat="server" Text="Preview" CssClass="button"
                                         Width="80px" OnClick="GetPDF" />
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                     <asp:HiddenField ID="hdnCustomerId" runat="server" Value="1" />
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

