<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="change_section_markup.aspx.cs" Inherits="change_section_markup" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Section Management</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table style="width: 100%;">

                            <tr>
                                <td align="left" valign="top">&nbsp;
                            <asp:TreeView ID="trvSection" runat="server"
                                OnSelectedNodeChanged="trvSection_SelectedNodeChanged" ImageSet="Arrows">
                                <ParentNodeStyle Font-Bold="False" />
                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD"
                                    HorizontalPadding="0px" VerticalPadding="0px" />
                                <NodeStyle Font-Names="Tahoma" Font-Size="10pt" ForeColor="Black"
                                    HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                            </asp:TreeView>
                                </td>
                                <td align="left" valign="top">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblTree" runat="server">Tree Navigation:</asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblParent" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2" valign="top">
                                                <asp:GridView ID="grdItemPrice" runat="server" AutoGenerateColumns="False"
                                                    DataKeyNames="item_id" OnRowEditing="grdItemPrice_RowEditing"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                        <asp:BoundField DataField="section_name" HeaderText="Item Name" />
                                                        <asp:BoundField DataField="measure_unit" HeaderText="UoM"  NullDisplayText=" "/>
                                                        <asp:BoundField DataField="item_cost" HeaderText="Cost" />
                                                        <asp:BoundField DataField="minimum_qty" HeaderText="Minimum Qty" />
                                                        <asp:BoundField DataField="retail_multiplier" HeaderText="Retail Mulitiplier" />
                                                        <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate" />
                                                        <asp:CommandField ShowEditButton="True" />
                                                    </Columns>
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:CheckBox ID="chkNewSection" runat="server"
                                                    OnCheckedChanged="chkNewSection_CheckedChanged" Text="New Section"
                                                    AutoPostBack="True" />
                                                <asp:CheckBox ID="chkNewSubSection" runat="server"
                                                    OnCheckedChanged="chkNewSubSection_CheckedChanged" Text="New Sub Section"
                                                    AutoPostBack="True" />
                                                <asp:CheckBox ID="chkNewItem" runat="server"
                                                    OnCheckedChanged="chkNewItem_CheckedChanged" Text="New Item"
                                                    AutoPostBack="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <asp:Label ID="lblSection" runat="server" Text="Section Name:"></asp:Label>
                                                <td align="left">
                                                    <asp:TextBox ID="txtSectionName" runat="server" Width="308px" TabIndex="1"></asp:TextBox>
                                                </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:Panel ID="pnlItem" runat="server">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblUoM" runat="server" Text="Unit of Measure:"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtUom" runat="server" TabIndex="1" Width="84px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblCost" runat="server" Text="Cost:"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtCost" runat="server" Width="84px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblMinQty" runat="server" Text="Minimum Qty:"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtMinQty" runat="server" Width="84px" Wrap="False"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblRetail" runat="server" Text="Retail Multiplier"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtRetail" runat="server" Style="margin-top: 0px" Width="84px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblLabor" runat="server" Text="Labor:"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:RadioButtonList ID="rdoLabor" runat="server" AutoPostBack="True"
                                                                    OnSelectedIndexChanged="rdoLabor_SelectedIndexChanged"
                                                                    RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="1" Selected="True">No</asp:ListItem>
                                                                    <asp:ListItem Value="2">Yes</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblLabor0" runat="server" Text="Labor Rate:"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtLabor" runat="server" Width="84px" Wrap="False"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Serial:</td>
                                                            <td align="left">
                                                                <asp:Label ID="lblSerial" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">&nbsp;<asp:Button ID="btnSave" runat="server" Text="Update" TabIndex="3" OnClick="btnSave_Click" />
                                                &nbsp;<asp:Button ID="btnClose" runat="server" Text="Close" TabIndex="4" OnClick="btnClose_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnSectionLevel" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnParentId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnItem" runat="server" Value="0" />
                                    &nbsp;<asp:HiddenField ID="hdnSectionId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnTrvSelectedValue" runat="server" Value="0" />
                                    &nbsp;<asp:HiddenField ID="hdnSectionSerial" runat="server" Value="0" />
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

