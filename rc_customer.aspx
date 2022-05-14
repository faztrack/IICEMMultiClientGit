<%@ Page Title="Customer Report" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="rc_customer.aspx.cs" Inherits="rc_customer" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="DropDownCheckBoxes" Namespace="Saplin.Controls" TagPrefix="ddlchk" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .overlay {
            position: fixed;
            z-index: 98;
            top: 0px;
            left: 0px;
            right: 0px;
            bottom: 0px;
            background-color: #000;
            filter: alpha(opacity=80);
            opacity: 0.8;
        }

        .overlayContent {
            z-index: 99;
            margin: 250px auto;
            text-align: center;
        }

            .overlayContent p {
                font-size: 16px;
                color: #fff;
                font-family: Arial;
                font-weight: normal;
                background-color: #000;
                margin: 0 auto 10px auto;
            }

            .overlayContent img {
                width: 80px;
                height: 80px;
            }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Customer Report</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious" runat="server" CssClass="prevButton"
                                        OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td>&nbsp;</td>
                                <td align="center">
                                    <b>Page:</b>
                                    <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true"
                                        ForeColor="#992a24"></asp:Label>
                                    &nbsp;
                                                <b>Item per page:</b>
                                    <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True">10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem Value="4">All Locations</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <table width="100%" style="padding: 0px; margin: 0px;">
                                        <tr>
                                            <td align="right"><b>Columns:</b>
                                            </td>
                                            <td align="left">
                                                <ddlchk:DropDownCheckBoxes ID="checkBoxes1" runat="server" AutoPostBack="True"
                                                    CssClass="ddlChkBox" OnSelectedIndexChanged="checkBoxes_SelcetedIndexChanged"
                                                    AddJQueryReference="True" meta:resourcekey="checkBoxes1Resource1" UseButtons="False"
                                                    UseSelectAllNode="True">
                                                    <Style SelectBoxWidth="150" />
                                                    <Texts SelectBoxCaption="Select" />
                                                </ddlchk:DropDownCheckBoxes>                                                
                                            <asp:Label ID="lblMessage" runat="server"></asp:Label></td>
                                        </tr>
                                    </table>
                                </td>
                                <td><asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="button"/></td>
                                <td align="right">
                                    <asp:Button ID="btnNext" runat="server" CssClass="nextButton"
                                        OnClick="btnNext_Click" Text="Next" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdCustomerReport" runat="server" AutoGenerateColumns="true"
                            PageSize="15" AllowPaging="True"
                            OnPageIndexChanging="grdCustomerReport_PageIndexChanging"
                            OnRowDataBound="grdCustomerReport_RowDataBound"
                            CssClass="mGrid rcGrid">
                            <PagerSettings Position="TopAndBottom" />
                            <PagerStyle HorizontalAlign="Left" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="0" cellspacing="4" width="100%" style="padding: 0px; margin: 0px;">
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnPrevious0" runat="server" CssClass="prevButton"
                                        OnClick="btnPrevious_Click" Text="Previous" />
                                </td>
                                <td align="left">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                                <td align="right">
                                    <asp:Button ID="btnNext0" runat="server" CssClass="nextButton"
                                        OnClick="btnNext_Click" Text="Next" />
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


