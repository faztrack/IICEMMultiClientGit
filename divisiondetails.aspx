<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="divisiondetails.aspx.cs" Inherits="divisiondetails" Title="Division Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left">
                                    <span class="titleNu">
                                        <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader" Text="Division List"></asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4px" cellspacing="4px" width="500px">
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>  
                            <tr>

                                <td align="right"><span class="required">* </span>Division Name: </td>
                                <td align="left">
                                    <asp:TextBox ID="txtDivisionName" runat="server" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">Active: </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkActive" runat="server" Checked="True" />
                                </td>
                            </tr>                          
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblResult" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td align="left">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit"  TabIndex="1"
                                        OnClick="btnSubmit_Click" CssClass="button" />
                                    &nbsp;<asp:Button ID="btnCancel" runat="server" Text="Cancel" TabIndex="2" OnClick="btnCancel_Click"
                                        CssClass="button" />
                                </td>
                            </tr>
                           
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="hdnDivisionId" runat="server" Value="0"/>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="1px" cellspacing="4px" style="margin-top:35px!important">
                            <asp:GridView ID="grddivision" runat="server" AutoGenerateColumns="false" width="550px" CssClass="mGrid">
                                <PagerSettings Position="TopAndBottom" />
                                <Columns>

                                    <%--Cell 0--%>
                                    <asp:TemplateField HeaderText="Division Name">
                                        <ItemTemplate>
                                            &nbsp;<asp:Label ID="lblGrdDivisionName" runat="server" Text='<%#Eval("division_name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="70%" />
                                    </asp:TemplateField>

                                    <%--Cell 1--%>
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrdStaus" runat="server" Text='<%#Eval("status") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center"  Width="20%" />
                                    </asp:TemplateField>

                                    <%--Cell 2--%>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/images/icon_edit.png"  CssClass="iconDeleteCss blindInput" ToolTip="Edit Division" OnClick="imgEdit_Click" Font-Underline="true" ForeColor="Blue" CommandArgument='<%#Eval("id")%>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center"  Width="10%" />
                                    </asp:TemplateField>

                                </Columns>
                                  <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                            <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
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

