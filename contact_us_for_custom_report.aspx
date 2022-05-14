<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="contact_us_for_custom_report.aspx.cs" Inherits="contact_us_for_custom_report" Title="Contact Us for Your Custom Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Contact Us for Your Custom Report</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="4" width="98%" align="center">
                            <tr>
                                <td align="left" colspan="2">Note: Please fill out the following form to tell us about the report you want.&nbsp; 
                                    Explain what you would like to see on the report.&nbsp; Let us know of any 
                                    formula that needs to be used and the data fields that need to be used as input 
                                    variables.&nbsp; For best result please eMail a Word or Excel version of the 
                                    report layout to faztrackbd@gmail.com.</td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Red" Text="* required"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td align="right" width="35%"><span class="required">* </span>
                                    <b>Name: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtName" runat="server" TabIndex="1" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">* </span>
                                    <b>eMail: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtEmail" runat="server" TabIndex="2" Width="200px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                                        ControlToValidate="txtEmail" ErrorMessage="Invalid email address"
                                        ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right"><span class="required">* </span>
                                    <b>Phone: </b>
                                </td>
                                <td align="left">
                                    <asp:TextBox ID="txtPhone" runat="server" TabIndex="3" Width="200px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Report output format: </b>
                                </td>
                                <td align="left">
                                    <asp:DropDownList ID="ddlReportFormat" runat="server" TabIndex="4" Width="212px">
                                        <asp:ListItem Value="1">PDF</asp:ListItem>
                                        <asp:ListItem Value="2">Excel</asp:ListItem>
                                        <asp:ListItem Value="3">CSV</asp:ListItem>
                                        <asp:ListItem Value="4">TXT</asp:ListItem>
                                        <asp:ListItem Value="5">HTML</asp:ListItem>
                                        <asp:ListItem Value="6">Word</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="top"><span class="required">* </span>
                                    <b>Instruction: 
                                    <br />
                                    </b>
                                    <asp:Label ID="Label1" runat="server" ForeColor="Green"
                                        Text="( 500 characters max )"></asp:Label>
                                </td>
                                <td align="left" valign="top">
                                    <asp:TextBox ID="txtInstruction" runat="server" MaxLength="500" TabIndex="5"
                                        TextMode="MultiLine" Width="350px" Height="80px"></asp:TextBox>
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
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="button" TabIndex="6"
                                        Text="Submit" Width="80px" OnClick="btnSubmit_Click" />
                                    <asp:Button ID="btnCancel" runat="server" CssClass="button" TabIndex="7"
                                        Text="Cancel" Width="80px" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center"></td>
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

