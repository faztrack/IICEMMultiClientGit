<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="role_management.aspx.cs" Inherits="role_management" Title="Role Management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Role Management</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="2" width="98%">
                            <tr>
                                <td width="45%" align="right">
                                    <b>Role: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlRoles" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td width="45%" align="right" style="height: 10px"></td>
                                <td align="left" style="height: 10px"></td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <h3>Select User Interfaces</h3>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <table cellpadding="0" cellspacing="2" width="100%" align="center">
                                        <tr>
                                            <td align="center" width="25%"></td>
                                            <td align="left" width="50%">
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                                                    <ContentTemplate>
                                                        <asp:Panel ID="Panel1" runat="server" Height="380px" ScrollBars="Vertical"
                                                            Width="100%">
                                                            <asp:TreeView ID="trvMenu" runat="server" ForeColor="Black"
                                                                OnTreeNodeCheckChanged="trvMenu_TreeNodeCheckChanged" ShowCheckBoxes="All"
                                                                ShowLines="True">
                                                                <ParentNodeStyle Font-Bold="False" />
                                                                <RootNodeStyle Font-Bold="True" />
                                                                <NodeStyle Font-Bold="False" />
                                                            </asp:TreeView>
                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="ddlRoles"
                                                            EventName="SelectedIndexChanged" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                            <td align="center" width="25%"></td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button"
                                        Width="80px" OnClick="btnSubmit_Click" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button"
                                        Width="80px" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
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

