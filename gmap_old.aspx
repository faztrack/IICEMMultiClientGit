﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="gmap_old.aspx.cs" Inherits="gmap_old" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .hdnStyle {
            BORDER-RIGHT: medium none;
            BORDER-TOP: medium none;
            BORDER-LEFT: medium none;
            COLOR: #fff;
            BORDER-BOTTOM: medium none;
            BACKGROUND-COLOR: #fff;
            box-shadow: none;
        }
    </style>
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyAR2y2gC7aa__R-JpqEiid_RdWlsJYe23w" type="text/javascript"></script>
    <script type="text/javascript">
        function load(zip) {
            //var sZip = zip;            
            //alert(zip);
            document.getElementById("head_hdnZip").value = zip;
            document.getElementById("head_btnLoad").click();
        }
    </script>
  <%--  <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>


            <asp:TextBox ID="hdnZip" runat="server" CssClass="hdnStyle"></asp:TextBox>
            <asp:TextBox ID="txtZipKey" runat="server" Visible="False"></asp:TextBox>
            <%--<input type="hidden" ID="hdnZip" Name="HiddenControl" runat="server"/>--%>
            <table width="50%">
                <tr>
                    <td align="center">
                        <asp:TextBox ID="txtZip" runat="server" CssClass="controlstyle" Width="122px"></asp:TextBox>&nbsp;
						<asp:Button ID="btnSearch" runat="server" CssClass="ControlButtonStyle" Width="110px" Text="Search by Zip"></asp:Button>&nbsp;
						<asp:Button ID="btnReset" OnClick="btnReset_Click" runat="server" CssClass="ControlButtonStyle"
                            Width="110px" Text="Reset"></asp:Button></td>
                </tr>
            </table>
            <table style="HEIGHT: 600px">
                <tr>
                    <td valign="top" align="left">&nbsp;
						<div id="map" style="WIDTH: 950px; HEIGHT: 600px" runat="server">
                            <asp:Literal ID="js" runat="server"></asp:Literal>
                        </div>
                    </td>
                    <td valign="top" align="right">

                        <asp:Button ID="btnLoad" OnClick="btnLoad_Click" runat="server" CssClass="hdnStyle" Text="Load"></asp:Button>
                        <asp:GridView ID="grdCusDetails" runat="server" AutoGenerateColumns="False"
                            AllowPaging="True">

                            <Columns>
                                <asp:BoundField DataField="Customer" HeaderText="Client">
                                    <HeaderStyle Width="150px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="fullAddress" HeaderText="Address">
                                    <HeaderStyle Width="350px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="SalesRep" HeaderText="Sales Person"></asp:BoundField>
                            </Columns>
                            <PagerStyle></PagerStyle>
                        </asp:GridView>

                    </td>
                </tr>
            </table>
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>

</asp:Content>

