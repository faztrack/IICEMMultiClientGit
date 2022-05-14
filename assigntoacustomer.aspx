<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="assigntoacustomer.aspx.cs" Inherits="assigntoacustomer" Title="Assign to a Customer" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <script language="javascript" type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }
    </script>
    <script language="javascript" type="text/javascript">
        function searchKeyPress(e) {
            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">

                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Assign to a Customer</span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <td align="left">
                                    <b>Sales Person Name:</b></td>
                                <td align="left">
                                    <asp:Label ID="lblSalesPersonName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Address:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Phone:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Email Address:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Model Estimate Name:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblModelEstimateName" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="left">
                                    <b>Create Date:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblCreateDate" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <b>Last Updated Date:</b>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblLastUpdatedDate" runat="server" Text=""></asp:Label>
                                </td>
                                <td align="right">&nbsp;</td>
                                <td align="left">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <%--<img width="25" height="25" src="images/08_call_log.png" alt="Search and Select Customer to Assign" title="Search and Select Customer to Assign" style="padding: 0px; vertical-align: middle;" />--%>
                                            <span class="cssTitleHeader" style="padding: 0px; vertical-align: middle;">Search and Select Customer to Assign                                                      
                                            </span>
                                        </span>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>


                <tr>
                    <td>
                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">First Name</asp:ListItem>
                                                    <asp:ListItem Selected="True" Value="2">Last Name</asp:ListItem>
                                                    <asp:ListItem Value="4">Address</asp:ListItem>
                                                    <asp:ListItem Value="3">Email</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtSearch" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                                            <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_LastName" ServiceMethod="GetLastName" TargetControlID="txtSearch" UseContextKey="True">
                                                            </cc1:AutoCompleteExtender>
                                                            <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch" WatermarkText="Search by Last Name" />
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnSearch" runat="server" CssClass="button" OnClick="btnSearch_Click" Text="Search" /></td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="left" colspan="2">
                                    <asp:GridView ID="grdLeadList" runat="server" AllowPaging="True" AutoGenerateColumns="False" CssClass="mGrid" DataKeyNames="customer_id" OnPageIndexChanging="grdLeadList_PageIndexChanging" OnRowDataBound="grdLeadList_RowDataBound" Width="1080px" PageSize="5" OnRowCommand="grdLeadList_RowCommand">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <%-- <asp:BoundField DataField="customer_name" HeaderText="Customer Name">
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>--%>
                                            <asp:ButtonField Text="Select" CommandName="Select">
                                                <HeaderStyle Width="4%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                            <asp:BoundField DataField="last_name1" HeaderText="Last Name">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="first_name1" HeaderText="First Name">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Address, eMail, Phone">
                                                <ItemTemplate>
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td align="left" width="50%">
                                                                <asp:Label ID="lAddress" runat="server"></asp:Label>
                                                            </td>
                                                            <td align="left" width="50%">
                                                                <asp:Label ID="lEmail" runat="server"></asp:Label>
                                                                <br />
                                                                <asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="sales_person_id" HeaderText="Sales Person">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField>
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td align="left">
                                                <b>Customer Name:</b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblSelectedCustName" runat="server"></asp:Label>
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                        <tr id="tblNoExistingEstimates" runat="server" visible="false">
                                            <td align="right"><b>Estimate Name:</b></td>
                                            <td align="left">
                                                <asp:Label ID="lblNewEstimateName" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td align="right">&nbsp;</td>
                                            <td align="left">&nbsp;</td>
                                        </tr>
                                        <tr id="tblExistingEstimates" runat="server" visible="false">
                                            <td align="left"><b>Choose an Existing Estimate:</b></td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlChoseEstimate" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlChoseEstimate_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td></td>

                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td align="left" colspan="3">
                                                <asp:Label ID="lblError" runat="server" Text=""></asp:Label></td>

                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td align="center" colspan="4"></td>
                            </tr>

                            <tr>
                                <td align="right">
                                    <asp:HiddenField ID="hdnModelEstimateId" runat="server" Value="0" />
                                </td>
                                <td align="left">
                                    <asp:HiddenField ID="hdnSalesPersonId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustName" runat="server" Value="0" />
                                </td>
                                <td align="right">
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                </td>
                                <td align="left">
                                    <asp:HiddenField ID="hdnCustomerEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnHdnPrevIndex" runat="server" Value="-1" />
                                    
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <%--<img width="25" height="25" src="images/08_call_log.png" alt="Assign Estimate Sections" title="Assign Estimate Sections" style="padding: 0px; vertical-align: middle;" />--%>
                                            <span class="cssTitleHeader" style="padding: 0px; vertical-align: middle;">Assign Estimate Sections                                                     
                                            </span>
                                        </span>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td>
                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="center" colspan="4">
                                    <table id="tblModelEstimateSections" runat="server" cellpadding="0"
                                        cellspacing="2" width="100%" visible="false" style="border: thin solid #C0C0C0">
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBox ID="chkCheckAllSections" runat="server" AutoPostBack="true"
                                                    OnCheckedChanged="chkCheckAllSections_CheckedChanged"
                                                    Text="Check All Sections" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBoxList ID="chkModelEstimateSections" runat="server" RepeatColumns="4" Width="100%">
                                                </asp:CheckBoxList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <%--<img width="25" height="25" src="images/08_call_log.png" alt="Assign Estimate Locations " title="Assign Estimate Locations " style="padding: 0px; vertical-align: middle;" />--%>
                                            <span class="cssTitleHeader" style="padding: 0px; vertical-align: middle;">Assign Estimate Locations                                                    
                                            </span>
                                        </span>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">

                            <tr>
                                <td align="center" colspan="4">
                                    <table id="tblModelEstimateLocations" runat="server" cellpadding="0" cellspacing="2" width="100%" visible="false" style="border: thin solid #C0C0C0">
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBox ID="chkCheckAllLocation" runat="server" AutoPostBack="true"
                                                    OnCheckedChanged="chkCheckAllLocation_CheckedChanged"
                                                    Text="Check All Locations" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="height: 10px"></td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:CheckBoxList ID="chkModelEstimatelocations" runat="server" RepeatColumns="4" Width="100%">
                                                </asp:CheckBoxList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="btnAssign" runat="server" Text="Assign" CssClass="button"
                            Width="80px" OnClick="btnAssign_Click" />
                        <asp:Button ID="btnGotoModelEstimate" runat="server"
                            Text="Go to Model Estimate" CssClass="button"
                            OnClick="btnGotoModelEstimate_Click" />
                        <asp:Button ID="btnGotoCustomer" runat="server" Text="Go to Customer Estimate"
                            CssClass="button" OnClick="btnGotoCustomer_Click" Visible="False" />
                        &nbsp;<asp:Button ID="btnClose" runat="server" CssClass="button" OnClick="btnClose_Click" Text="Close" />
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

