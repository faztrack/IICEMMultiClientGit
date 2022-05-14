<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="emailtemplatelist.aspx.cs" Inherits="emailTemplateList" Title="Email Template List" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .emailForm:first-child {
            margin-top: 10px;
        }

        .emailForm {
            margin-left: 26% !important;
            display: block;
        }

        .textBoxDs {
            margin-left: 20% !important;
            display: block;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table cellpadding="0" cellspacing="0" width="100%" align="center">
                <tr>
                    <td colspan="3" align="left" class="cssHeader">
                        <span class="titleNu">
                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Email Template</asp:Label></span>
                    </td>
                </tr>
                 <tr>
                    <td colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td  align="right"></td>
                    <td align="left">                        
                        
                    </td>
                    <td  align="right">
                        <asp:Button ID="btnAddNewEmail" runat="server" Text="Add New Email" CssClass="button" OnClick="btnAddNewEmail_Click"/>                        
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3"></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:GridView ID="grdEmailTemplate" runat="server" AutoGenerateColumns="False"
                            Width="100%" CssClass="mGrid">
                            <Columns>
                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdName" CssClass="paraMar" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                                    </ItemTemplate>
                                     <HeaderStyle HorizontalAlign="Center" Width="15%" />
                                    <ItemStyle HorizontalAlign="Left" Width="15%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="To">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdTo" CssClass="paraMar" runat="server" Text='<%#Eval("ToEmailAddress")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="From">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdFrom" CssClass="paraMar" runat="server" Text='<%#Eval("FromAddress")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="CC">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdCC" CssClass="paraMar" runat="server" Text='<%#Eval("CcAddress")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="BCC">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdBCC" CssClass="paraMar" runat="server" Text='<%#Eval("BccAddress")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                               <%-- <asp:TemplateField HeaderText="Subject">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdSubject" CssClass="paraMar" runat="server" Text='<%#Eval("Subject")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Message">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdMessage" CssClass="paraMar" runat="server" Text='<%#Eval("Message")%>'></asp:Label>                                       
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Created Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdCreatedDate" CssClass="paraMar" runat="server" Text='<%#Eval("CreatedDate","{0:d}")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Created By">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdCreatedBy" CssClass="paraMar" runat="server" Text='<%#Eval("createby")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Last Updated Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblUpdatedDate" CssClass="paraMar" runat="server" Text='<%#Eval("LastUpdatedDate","{0:d}")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Updated by">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGrdUpdatedby" CssClass="paraMar" runat="server" Text='<%#Eval("updatedby")%>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>--%>


                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgEditBtn" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_edit_16x16.png" CommandArgument='<%#Eval("EmailTemplateId")%>' OnClientClick="ShowProgress();" Style="margin: 0px !important" OnClick="imgEditBtn_Click"/>

                                       <%-- <asp:ImageButton ID="imgDeleteBtn" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" CommandArgument='<%#Eval("EmailTemplateId")%>' OnClientClick="ShowProgress();" Style="margin: 0px !important" />--%>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <asp:HiddenField ID="hdnEmailTemplateId" runat="server" Value="0" />
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

