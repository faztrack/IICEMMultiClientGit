<%@ Page Title="Labor Hour" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="laborhourinfo.aspx.cs" Inherits="laborhourinfo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left"><span class="titleNu">
                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Labor Hours</asp:Label></span></td>
                        <td align="right"></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <div style="margin: 0 auto; width: 100%">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table width="60%" border="0" cellspacing="4" cellpadding="4" align="center">
                                <tr>
                                    <td align="left" valign="top">
                                        <table width="100%" border="0" cellspacing="8" cellpadding="4" align="center">
                                            <tr>
                                                <td align="right">
                                                    <strong>Start of Week:</strong>
                                                </td>
                                                <td align="left" valign="top">
                                                    <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                        <tr>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtStartDate" runat="server" Width="170px" AutoPostBack="True" OnTextChanged="txtStartDate_TextChanged"></asp:TextBox>
                                                            </td>
                                                            <td>&nbsp;</td>
                                                            <td align="left">
                                                                <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtStartDate" WatermarkText="Weeks starts on Monday" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right"><strong>Crew Member Name:</strong></td>
                                                <td align="left" valign="top">
                                                    <asp:DropDownList ID="ddlInstaller" Width="212px" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlInstaller_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate1" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate1" runat="server" OnRowDataBound="grdLaborDate1_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate1_RowEditing" OnRowUpdating="grdLaborDate1_RowUpdating" OnRowCommand="grdLaborDate1_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate2" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate2" runat="server" OnRowDataBound="grdLaborDate2_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate2_RowEditing" OnRowUpdating="grdLaborDate2_RowUpdating" OnRowCommand="grdLaborDate2_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate3" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate3" runat="server" OnRowDataBound="grdLaborDate3_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate3_RowEditing" OnRowUpdating="grdLaborDate3_RowUpdating" OnRowCommand="grdLaborDate3_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate4" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate4" runat="server" OnRowDataBound="grdLaborDate4_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate4_RowEditing" OnRowUpdating="grdLaborDate4_RowUpdating" OnRowCommand="grdLaborDate4_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate5" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate5" runat="server" OnRowDataBound="grdLaborDate5_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate5_RowEditing" OnRowUpdating="grdLaborDate5_RowUpdating" OnRowCommand="grdLaborDate5_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblDate6" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate6" runat="server" OnRowDataBound="grdLaborDate6_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate6_RowEditing" OnRowUpdating="grdLaborDate6_RowUpdating" OnRowCommand="grdLaborDate6_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lbldate7" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:GridView ID="grdLaborDate7" runat="server" OnRowDataBound="grdLaborDate7_RowDataBound"
                                            AutoGenerateColumns="False" Width="100%" CssClass="mGrid" OnRowEditing="grdLaborDate7_RowEditing" OnRowUpdating="grdLaborDate7_RowUpdating" OnRowCommand="grdLaborDate7_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Customer Name/Job# ">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLastName" runat="server" Text='<%# Eval("last_name") %>' />
                                                        <asp:TextBox ID="txtlast_name" runat="server" Width="96%" AutoPostBack="True" OnTextChanged="Load_Product_Info" Text='<%# Eval("last_name") %>' Visible="false"></asp:TextBox>
                                                        <cc1:AutoCompleteExtender ID="autoComplete1" runat="server" CompletionInterval="10" CompletionSetCount="12" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetLastName" TargetControlID="txtlast_name" />
                                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber1" runat="server" TargetControlID="txtlast_name" WatermarkText="Type Last Name here" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Section">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlSection" runat="server" DataSource="<%#dtSection%>" DataTextField="section_name" DataValueField="section_id" Enabled="false">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="#Hour" ControlStyle-CssClass="hourRight" ItemStyle-Width="72px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbllabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' />
                                                        <asp:TextBox ID="txtlabor_hour" runat="server" Text='<%# Eval("labor_hour") %>' Visible="false" Width="50px"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>
                                                <asp:ButtonField CommandName="Edit" Text="Edit">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" valign="top">
                                        <cc1:CalendarExtender ID="StartDate" runat="server" Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                            PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                        </cc1:CalendarExtender>
                                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel2"
                        DynamicLayout="False">
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
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

