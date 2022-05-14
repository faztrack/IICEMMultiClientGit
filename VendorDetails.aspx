<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="VendorDetails.aspx.cs" Inherits="VendorDetails" Title="Vendor Details" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table id="Table5" align="center" width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Vendor Details</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="Table2" width="98%" align="center" border="0" cellpadding="0" cellspacing="3" onclick="return TABLE1_onclick()">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td width="35%" align="right"><span class="required">*</span>
                                    <b>Vendor Name: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtVendorName" runat="server"
                                        TabIndex="1" Width="312px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top">
                                    <b>Address: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtAddress" runat="server" Height="40px"
                                        TabIndex="3" TextMode="MultiLine" Width="312px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>City: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtCity" runat="server"
                                        TabIndex="4"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>State: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Zip: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtZip" runat="server" TabIndex="6"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Phone: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtPhone" runat="server"
                                        TabIndex="7"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Fax: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtFax" runat="server"
                                        TabIndex="8"></asp:TextBox></td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <b>Email: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtEmailAddress" runat="server" TabIndex="8"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <b>Status: </b></td>
                                <td align="left">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Checked="True" TabIndex="14" Text="Active" />
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <table width="100%">
                                        <tr>
                                            <td align="center" class="cssHeader">
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td align="left"><span class="titleNu"><span id="spnPnlSection" class="cssTitleHeader">
                                                            <asp:ImageButton ID="ImageSectionMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="40" />
                                                            <font style="font-size: 16px; cursor: pointer;">Vendor Sections</font></span></span>
                                                            <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender7" runat="server" CollapseControlID="spnPnlSection" Collapsed="True" CollapsedImage="Images/expand.png" ExpandControlID="spnPnlSection" ExpandedImage="Images/collapse.png" ImageControlID="ImageSectionMain" SuppressPostBack="true" TargetControlID="pnlSection">
                                                            </cc1:CollapsiblePanelExtender>
                                                        </td>
                                                        <td align="right"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Panel ID="pnlSection" runat="server">
                                                    <table align="center" cellpadding="0" cellspacing="2" class="wrapper" width="100%">
                                                        <tr>
                                                            <td align="center">
                                                                <asp:CheckBoxList ID="chkSections" runat="server" RepeatColumns="4"
                                                                    Width="100%" TabIndex="2">
                                                                </asp:CheckBoxList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="cssHeader" align="center" colspan="2">
                                    <table width="100%" cellspacing="0" cellpadding="0">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <span class="titleNu">
                                                        <span id="spnPnlContact" class="cssTitleHeader">
                                                            <asp:ImageButton ID="ImageContactMain" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0 0 4px; vertical-align: middle;" TabIndex="112" />
                                                            <font style="font-size: 16px; cursor: pointer;">Contact Information</font>
                                                        </span>

                                                    </span>
                                                    <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender4" runat="server" ImageControlID="ImageContactMain" CollapseControlID="spnPnlContact"
                                                        ExpandControlID="spnPnlContact" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGIContactMain" Collapsed="True">
                                                    </cc1:CollapsiblePanelExtender>
                                                </td>
                                                <td align="right"></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="pnlTGIContactMain" runat="server" Height="100%">
                                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center">
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr style="padding: 0px; margin: 0px;">
                                                            <td style="padding: 0px; margin: 0px; text-align: right;">
                                                                <asp:Panel ID="Panel1" runat="server">
                                                                    <span id="PnlCtrlID1" runat="server">
                                                                        <asp:LinkButton ID="lnkAddNewContact" runat="server" CssClass="button" Width="160px">
                                                                            <asp:ImageButton ID="ImageTGI1" CssClass="blindInput" runat="server" ImageUrl="~/Images/expand.png" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="113" />
                                                                            Add New Contact
                                                                        </asp:LinkButton>
                                                                    </span>
                                                                </asp:Panel>
                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server" ImageControlID="ImageTGI1" CollapseControlID="PnlCtrlID1"
                                                                    ExpandControlID="PnlCtrlID1" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/expand.png" TargetControlID="pnlTGI1" Collapsed="True">
                                                                </cc1:CollapsiblePanelExtender>
                                                            </td>
                                                        </tr>
                                                        <tr style="padding: 0px; margin: 0px;">
                                                            <td style="padding: 0px; margin: 0px;" align="center">
                                                                <asp:Panel ID="pnlTGI1" runat="server" Height="100%">
                                                                    <table style="padding: 0px; margin: 0px;" width="70%">
                                                                        <tr>
                                                                            <td valign="top">
                                                                                <table style="padding: 0px; margin: 0px;" width="100%">
                                                                                    <tr>
                                                                                        <td align="right">First Name:
                                                                                        </td>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtFirstName" runat="server" Width="341px" TabIndex="301"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td align="right">Last Name:
                                                                                        </td>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtLastName" runat="server" Width="341px" TabIndex="302"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>


                                                                                    <tr>
                                                                                        <td align="right" valign="top">&nbsp;</td>
                                                                                        <td align="left">
                                                                                            <asp:Label ID="lblResultContact" runat="server"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>&nbsp;</td>
                                                                                        <td align="left">
                                                                                            <asp:Button ID="btnSaveVD" runat="server" CssClass="button" TabIndex="307" Text="Save Contact" OnClick="btnSaveVD_Click" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                            <td valign="top">
                                                                                <table style="padding: 0px; margin: 0px;" width="100%">
                                                                                    <tr>
                                                                                        <td align="right">Email:
                                                                                        </td>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtEmail" runat="server" Width="341px" TabIndex="304"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td align="right">Phone:
                                                                                        </td>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtVdPhone" runat="server" Width="341px" TabIndex="305"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>

                                                                                    <tr>
                                                                                        <td>&nbsp;</td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>&nbsp;</td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>&nbsp;</td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td>&nbsp;</td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblMessageGrdContact" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:GridView ID="grdVendorDetails" runat="server" AutoGenerateColumns="False" CssClass="mGrid" PageSize="50" Width="100%" OnRowEditing="grdVendorDetails_RowEditing" TabIndex="318"
                                                        OnRowUpdating="grdVendorDetails_RowUpdating">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="First Name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblFirstNameG" runat="server" Text='<%# Eval("FirstName").ToString() %>'></asp:Label>
                                                                    <asp:TextBox ID="txtFirstNameG" runat="server" Text='<%# Eval("FirstName").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Last Name">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblLastNameG" runat="server" Text='<%# Eval("LastName").ToString() %>'></asp:Label>
                                                                    <asp:TextBox ID="txtLastNameG" runat="server" Text='<%# Eval("LastName").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Left" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Email">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblEmailG" runat="server" Text='<%# Eval("Email").ToString() %>'></asp:Label>
                                                                    <asp:TextBox ID="txtEmailG" runat="server" Text='<%# Eval("Email").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Phone">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPhoneG" runat="server" Text='<%# Eval("Phone").ToString() %>'></asp:Label>
                                                                    <asp:TextBox ID="txtPhoneG" runat="server" Text='<%# Eval("Phone").ToString() %>' Visible="false" Width="90%" TabIndex="17"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>

                                                            <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>

                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>

                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>

                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button ID="btnSubmit" runat="server"
                                        TabIndex="18" Text="Submit" CssClass="button" OnClick="btnSubmit_Click"
                                        Width="80px" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" TabIndex="19" Text="Cancel"
                                        CausesValidation="False" CssClass="button" OnClick="btnCancel_Click"
                                        Width="80px" />
                                    <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="hdnVendorId" runat="server" Value="0" />
                                    &nbsp;</td>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
</asp:Content>

