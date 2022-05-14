<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddingItemInPricing.aspx.cs" Inherits="AddingItemInPricing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Adding item in Pricing help</title>
</head>
    <script type="text/javascript">
        function closepopup() {
            window.close();
        }
</script>
<style>
    body {
        font-family:Arial;
    }
    .auto-style1 {
        height: 39px;
    }
</style>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <a href="javascript: closepopup()">Close</a>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <h2>Adding item in Pricing help</h2>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <table cellpadding="1" cellspacing="1" width="90%">
                        <tr>
                            <td align="left">
                                <b>### Adding item in Pricing:</b>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" height="5px">
                                    
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                1. All search items will be added in the "Collector Grid"
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                               2. Upon clicking on "Add selected item in pricing", the checked items from the "Collector Grid" will be saved in the main pricing grid and be moved in a temporary "Recently added item(s)" Grid"
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                               3. In the "Recently added item(s)" grid, there is an "Undo" link.
                            </td>
                        </tr>
                         <tr>
                            <td align="left" class="auto-style1">
                              4. Clicking on the "Undo" will remove that item from the main pricing list and it will also be removed from the "Recently added item(s)" grid" and will be added back in the "Collector Grid"
                            </td>
                        </tr>
                        <tr>
                            <td align="left" height="5px">
                                    
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                Example Screenshot is as follows: 
                            </td>
                        </tr>
                        <tr>
                            <td align="left" height="5px">
                                    
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/PriceHelp.jpg" Width="900px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
