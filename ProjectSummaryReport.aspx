﻿<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ProjectSummaryReport.aspx.cs" Inherits="ProjectSummaryReport" Title="Project Summary Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <table style="width:100%;">
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                    &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
</asp:Content>

