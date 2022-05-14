<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="VendorManger.aspx.cs"
    Inherits="VendorManger" Title="Vendor Manager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        
        </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Vendor Manager</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="center">
                                    <asp:GridView ID="grdVendor" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                        OnRowCommand="grdVendor_RowCommand" PageSize="200" TabIndex="2" Width="500px"
                                        OnRowDataBound="grdVendor_RowDataBound" OnRowEditing="grdVendor_RowEditing" OnRowUpdating="grdVendor_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Vendor Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("vendor_name") %>' />
                                                    <asp:TextBox ID="txtVendorName" runat="server" Visible="false" Text='<%# Eval("vendor_name") %>'
                                                        Width="320px" Wrap="False"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Active">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked='<%# Eval("is_active") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" height="10px"></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table cellpadding="0" cellspacing="0" width="500px">
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click"
                                                    Text="Add New Row" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
