<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Report.aspx.cs"
    Inherits="Report" Title="Report" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">


   <script type="text/javascript">
    
                    $(function() {
                    $("img.imgThNail").mouseover(function(e) {
                    var imgName = $(this).attr("src").replace("-s", "");
                    $(this).css({
                    border: 'solid 1px red'
                    });
                    //alert("donr");
                    $("#loadImage").css({
                    display: 'none'
                    });
                    $("#Img1").css({
                    display: 'block'
                    });
                    //alert("2");
                    
                    $("#loadImage").attr('src', imgName).load(function() {
                    $("#Img1").css({
                    display: 'none'
                    });
                    //alert("3");
                    $("#loadImage").css({
                    display: 'block'
                    });
                    });
                    });

                    $("img.imgThNail").mouseout(function(e) {
                    $(this).css({
                    border: 'solid 0px red'
                    });
                    });
                    });
    </script>
   
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                </ContentTemplate>
    </asp:UpdatePanel>
            <table width="100%">
                <tr>
                    <td align="center">
                        <h1>
                            Report</h1>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4">
                            <tr>
                                <td align="center">
                                    <h3>
                                        Customer</h3>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td align="right">
                                    <h3>
                                        &nbsp;Customer Estoimate</h3>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td align="center">
                                    <h3>
                                        Customer Payments</h3>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBoxList ID="chkCustomer" runat="server">
                                    </asp:CheckBoxList>
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <asp:CheckBoxList ID="chkEstimate" runat="server">
                                    </asp:CheckBoxList>
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <asp:CheckBoxList ID="chkPayments" runat="server">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

    <br />
    <table width="100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td align="center">
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlSearchBy" runat="server">
                                <asp:ListItem Value="1">First Name</asp:ListItem>
                                <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnViewReport" runat="server" CssClass="button" Text="View Report"
                                Width="80px" OnClick="btnViewReport_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table> 
</asp:Content>
