<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="disclaimerui.aspx.cs" Inherits="disclaimerui" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="cc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="border: 1px solid #ddd" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Disclaimers</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left"></td>
                </tr>
                <tr>
                    <td align="center" valign="top" width="80%">
                        <asp:GridView ID="grdDisclaimer" runat="server" AutoGenerateColumns="False" 
                            CssClass="mGrid" DataKeyNames="item_id"
                            PageSize="200" TabIndex="2" Width="80%"
                            OnRowCommand="grdDisclaimer_RowCommand" OnRowDataBound="grdDisclaimer_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Section">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlSectiong" CssClass="secDD" runat="server" Width="250px">
                                        </asp:DropDownList>

                                    </ItemTemplate>
                                    <ItemStyle Width="15%" HorizontalAlign="Left" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Section Header">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtHeader" runat="server" Text='<%# Eval("section_heading") %>' TextMode="MultiLine" Width="90%" Height="40px"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" Width="28%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Disclaimer Details">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtDetails" runat="server" Text='<%# Eval("disclaimer_name") %>' TextMode="MultiLine" Width="95%" Height="40px"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" Width="60%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Initial Req'red" >
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkInitial" runat="server" Checked='<%# Eval("IsInitilal") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="6%" />
                                </asp:TemplateField>
                                <asp:ButtonField CommandName="Add" Text="Add">
                                    <ItemStyle HorizontalAlign="Center" Width="6%" />
                                </asp:ButtonField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" OnClick="DeleteFile" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle CssClass="alt" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="center" valign="top">
                        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td align="center" valign="top">
                        <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save"
                            CssClass="button" Width="80px" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                            OnClick="btnCancel_Click" Width="80px" />
                        <asp:HiddenField ID="hdnterms_condition_id" runat="server" Value="0" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

