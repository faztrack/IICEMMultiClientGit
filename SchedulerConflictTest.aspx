<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SchedulerConflictTest.aspx.cs" Inherits="SchedulerConflictTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">

        <ContentTemplate>


            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Scheduler Conflict Test</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="85%">
                            <tr>
                                <td>

                                    <table>
                                        <tr>
                                            <td>Customer</td>
                                            <td>
                                                <asp:TextBox ID="txtCustomerID" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>Estimate</td>
                                            <td>
                                                <asp:TextBox ID="txtEstimateID" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnCheck" runat="server" Text="Check" OnClick="btnCheck_Click" /></td>
                                        </tr>
                                    </table>

                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>

                                    <table style="text-align: left; width: 60%;" class="mGrid">
                                        <tr>
                                            <td valign="top">
                                              <asp:Label ID="lblDuplicateCheck1" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck1" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                              <asp:Label ID="lblDuplicateCheck2" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck2" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblDuplicateCheck3" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck3" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheck4" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck4" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheck5" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck5" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheck6" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheck6" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheckTemp1" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp1" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheckTemp2" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp2" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheckTemp3" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp3" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                              <asp:Label ID="lblDuplicateCheckTemp4" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp4" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                              <asp:Label ID="lblDuplicateCheckTemp5" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp5" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                               <asp:Label ID="lblDuplicateCheckTemp6" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:GridView ID="grdDuplicateCheckTemp6" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
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

