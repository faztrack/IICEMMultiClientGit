<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="undolabor_hour_list.aspx.cs" Inherits="undolabor_hour_list" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Labor Tracking Archive List</asp:Label></span></td>
                               
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="90%">
                            <tr>
                                <td>

                                    <table width="100%">
                                        <tr>
                                             <td align="left" style="width:12%"><strong>Crew Member Name:</strong></td>
                                            <td align="left">
                                                <table style="padding: 0px; margin: 0px; width:100%" >
                                                    <tr>
                                                        <td align="left" style="width:12%">
                                                            <asp:DropDownList ID="ddlInstaller" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlInstaller_SelectedIndexChanged" Width="200px">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="underlineButton" Text="View All" OnClick="lnkViewAll_Click"></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                           
                                            <td align="left">
                                                <b>Page: </b>
                                                <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                                                &nbsp;
                                                <b>Item per page: </b>
                                                <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True">10</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>

                                                </asp:DropDownList>
                                            </td>

                                          


                                        </tr>
                                        <tr>
                                            <td align="center" valign="top" colspan="7">
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnPrevious" runat="server" Text="Previous" OnClick="btnPrevious_Click" CssClass="prevButton" />
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="right">
                                                <asp:Button ID="btnNext" runat="server" Text="Next"
                                                    OnClick="btnNext_Click" CssClass="nextButton" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="7">
                                                <asp:GridView ID="grdLaberTrack" runat="server" AllowPaging="True"
                                                    AutoGenerateColumns="False" OnRowDataBound="grdLaberTrack_RowDataBound"
                                                    OnPageIndexChanging="grdLaberTrack_PageIndexChanging"
                                                    Width="100%" CssClass="mGrid">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                       

                                                         <asp:BoundField HeaderText="Labor Date" DataField="labor_date" DataFormatString="{0:d}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>

                                                        <asp:BoundField HeaderText="Crew Member Name" DataField="UserID">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>

                                                        <%-- <asp:BoundField HeaderText="Customer Name" DataField="CustomerName">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField HeaderText="Section" DataField="SectionName">
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField HeaderText="Start Location" DataField="StartPlace">
                                                            <HeaderStyle HorizontalAlign="Center" Width="17%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="EndPlace" HeaderText="End Location">
                                                            <HeaderStyle HorizontalAlign="Center" Width="16%" />
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:t}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="EndTime" HeaderText="End Time" DataFormatString="{0:t}">
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="LUNCH (Hrs)">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblLunch" runat="server" Text=""></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Total (Hrs)">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblTotalHours" runat="server" Text=""></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="imgUndo" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/assets/mobileicons/icon_undo_16x16.png" ToolTip="Undo" OnClick="UndoFile" />
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="btnPrevious0" runat="server" Text="Previous" CssClass="prevButton" OnClick="btnPrevious_Click" />
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                            <td align="left">&nbsp;
                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label></td>
                                            <td align="right">
                                                <asp:Button ID="btnNext0" runat="server" Text="Next" CssClass="nextButton" OnClick="btnNext_Click" />

                                            </td>
                                        </tr>
                                    </table>

                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
       
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress2" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>
                    Please wait while your data is being processed
                </p>
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>

