<%@ Page Title="Customer Job Status" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="customer_job_status_info.aspx.cs" Inherits="customer_job_status_info" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="JavaScript" type="text/JavaScript">
        function DisplayWindow1() {
            window.open('design_graphics.aspx?jsid=1&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=900,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow2() {
            window.open('jobdesc_popup.aspx?jsid=2&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow3() {
            window.open('SiteProgress_graphics.aspx?jsid=3&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=900,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
            //window.open('jobdesc_popup.aspx?jsid=3&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow4() {
            window.open('jobdesc_popup.aspx?jsid=4&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow5() {
            window.open('jobdesc_popup.aspx?jsid=5&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader" colspan="3">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Job Status Graphic Details for:&nbsp;
                                    <asp:Label Style="color: #330f02; font-size: 16px; font-weight: 500;" ID="lblCustomerName" runat="server" Text=""></asp:Label></span></td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px" colspan="3"></td>
                </tr>
                <tr>
                    <td></td>
                    <td align="center" height="10px">
                        <table style="color: #000000;">
                            <tr>
                                <td align="left" style="font-weight: bold;">Viewable by Customer?</td>
                                <td align="left" style="font-weight: bold;">
                                    <asp:RadioButtonList ID="rdoconfirm" runat="server" OnClick="ShowProgress();"
                                        OnSelectedIndexChanged="rdoconfirm_SelectedIndexChanged"
                                        RepeatDirection="Horizontal" AutoPostBack="True">
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td align="center" height="10px" colspan="3"></td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <table cellpadding="2px" cellspacing="2px">
                            <tr>
                                <td align="center">
                                    <asp:Label ID="Label1" runat="server" Text="A" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label2" runat="server" Text="B" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label3" runat="server" Text="C" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label4" runat="server" Text="D" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label5" runat="server" Text="E" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label6" runat="server" Text="F" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                                <td align="center">
                                    <asp:Label ID="Label7" runat="server" Text="G" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="7" height="10px"></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Image ID="imgA" runat="server" ImageUrl="~/JobImages/WhiteA.jpg" ToolTip="Click here to view Design descriptions" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgB" runat="server" ImageUrl="~/JobImages/WhiteB.jpg" ToolTip="Click here to view Selection descriptions" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgC" runat="server" ImageUrl="~/JobImages/WhiteC.jpg" ToolTip="Click here to view Site Progress & Photos" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgD" runat="server" ImageUrl="~/JobImages/WhiteD.jpg" ToolTip="Click here to view Schedule descriptions" />
                                </td>
                                <td align="center">
                                    <asp:Image ID="imgE" runat="server" ImageUrl="~/JobImages/WhiteE.jpg" ToolTip="Click here to view Final Project Review descriptions" />
                                </td>
                                <td align="center">
                                    <asp:ImageButton ID="imgButtonF" runat="server" Style="border: none; background: none; box-shadow: none;"
                                        ImageUrl="~/JobImages/WhiteF.jpg" OnClick="imgButtonF_Click" />
                                </td>
                                <td align="center">
                                    <asp:ImageButton ID="imgButtonG" runat="server" Style="border: none; background: none; box-shadow: none;"
                                        ImageUrl="/JobImages/WhiteG.jpg" OnClick="imgButtonG_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="7" height="10px"></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:CheckBox ID="chkA" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkA_CheckedChanged" EnableTheming="True" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkB" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkB_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkC" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkC_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkD" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkD_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkE" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkE_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkF" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkF_CheckedChanged" />
                                </td>
                                <td align="center">
                                    <asp:CheckBox ID="chkG" runat="server" AutoPostBack="True"
                                        OnCheckedChanged="chkG_CheckedChanged" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px" colspan="3"></td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <table cellpadding="0" cellspacing="0" width="56%">
                            <tr>
                                <td align="center" colspan="3">
                                    <asp:GridView ID="grdStatusDesc" runat="server" AutoGenerateColumns="False"
                                        CssClass="mGrid" DataKeyNames="item_id" OnRowCommand="grdStatusDesc_RowCommand"
                                        PageSize="200" TabIndex="2" Width="100%"
                                        OnRowDataBound="grdStatusDesc_RowDataBound"
                                        OnRowEditing="grdStatusDesc_RowEditing"
                                        OnRowUpdating="grdStatusDesc_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Stage">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStage" runat="server" Text='<%# Eval("jobstatusid") %>' />
                                                    <asp:DropDownList ID="ddlStage" runat="server" Visible="false"
                                                        SelectedValue='<%# Eval("jobstatusid") %>'>
                                                        <asp:ListItem Value="1">A</asp:ListItem>
                                                        <asp:ListItem Value="2">B</asp:ListItem>
                                                        <asp:ListItem Value="3">C</asp:ListItem>
                                                        <asp:ListItem Value="4">D</asp:ListItem>
                                                        <asp:ListItem Value="5">E</asp:ListItem>
                                                        <asp:ListItem Value="6">F</asp:ListItem>
                                                        <asp:ListItem Value="7">G</asp:ListItem>
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description">
                                                <ItemTemplate>
                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblDescription" runat="server" Text='<%# Eval("status_description") %>' /></pre>
                                                    <asp:TextBox ID="txtDescription" runat="server" Visible="false"
                                                        Text='<%# Eval("status_description") %>' TextMode="MultiLine"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Serial">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatusSerial" runat="server" Text='<%# Eval("status_serial") %>' />
                                                    <asp:TextBox ID="txtStatusSerial" runat="server" Visible="false" Text='<%# Eval("status_serial") %>' Width="50px" Wrap="False"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="Edit" Text="Edit"></asp:ButtonField>
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <asp:Button ID="btnCancel" runat="server" Text="Close" TabIndex="24"
                                        OnClick="btnCancel_Click" Width="80px" CssClass="button" />
                                </td>
                                <td align="center">
                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">
                                    <asp:Button ID="btnAddnewRow" runat="server" CssClass="button" OnClick="btnAddnewRow_Click" Text="Add New Row" />
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3"></td>
                </tr>
                <tr>
                    <td align="center" class="button" colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td align="center" height="10px" colspan="3">
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnJobStatusId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="LoadingProgress" style="display: none">
        <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>
</asp:Content>
