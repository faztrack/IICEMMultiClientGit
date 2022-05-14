<%@ Page Language="C#" AutoEventWireup="true" CodeFile="abc.aspx.cs" Inherits="abc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0px" cellspacing="0px" width="100%">
            <tr>
                <td align="center">
                    <asp:Button ID="Button1" runat="server" Text="Test" OnClick="Button1_Click" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:TextBox ID="txt" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
