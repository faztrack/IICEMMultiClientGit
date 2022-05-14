<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="me_estimate_list.aspx.cs"
    Inherits="me_estimate_list" Title="Estimation Template List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="Javascript" type="text/javascript">
        function ChangeImage(id) {
            document.getElementById(id).src = 'Images/loading.gif';
        }
    </script>
    <script type="text/javascript">

        function confirmDelete() {
            return confirm("Are you sure that you want to delete Template(s) ?");
        }

        function searchPublicKeyPress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnPublicSearch.ClientID%>').click();
                return false;
            }
            return true;

        }

        function searchKeyPress(e) {
            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;
        }



        function CheckPublicAllSubmit(Checkbox) {
            // alert("msg");
            var GridVwHeaderChckbox = document.getElementById("<%=grdPublicEstimationList.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                var elements = GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0]
                for (var e in elements) {
                    if (e.type != "text") {
                        elements.checked = Checkbox.checked;
                    }
                }

            }
        }

        function CheckPublicUnselect(Checkbox) {
            var bFlag = false;
            var GridVwHeaderChckbox = document.getElementById("<%=grdPublicEstimationList.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                if (GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0].checked == false) {
                    bFlag = true;
                    break;
                }
            }
            if (bFlag) {
                GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = false;
            }
            else {
                GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = true;
            }
        }

        function CheckAllSubmit(Checkbox) {
            // alert("msg");
            var GridVwHeaderChckbox = document.getElementById("<%=grdEstimationList.ClientID %>");
                for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                    var elements = GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0]
                    for (var e in elements) {
                        if (e.type != "text") {
                            elements.checked = Checkbox.checked;
                        }
                    }

                }
            }

            function CheckUnselect(Checkbox) {
                var bFlag = false;
                var GridVwHeaderChckbox = document.getElementById("<%=grdEstimationList.ClientID %>");
                for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                    if (GridVwHeaderChckbox.rows[i].cells[3].getElementsByTagName("INPUT")[0].checked == false) {
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag) {
                    GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = false;
                }
                else {
                    GridVwHeaderChckbox.rows[0].cells[3].getElementsByTagName("INPUT")[0].checked = true;
                }
            }

    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%" align="center">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Estimation Template List</span></td>
                                <td align="right" style="padding-right: 30px;">
                                    <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" Text="Add New Estimation Template"
                                        CssClass="button" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="4" cellspacing="4" width="900px">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr id="trPublicSearchAdd" runat="server">
                                            <td style="width: 180px;">
                                                <b>Estimation Template Name: </b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtPublicSearch" Style="margin-left: 0px;" onkeypress="return searchPublicKeyPress(event);" runat="server"></asp:TextBox>
                                                <asp:Button ID="btnPublicSearch" Style="margin-left: 0px;" runat="server" Text="Search" OnClick="btnPublicSearch_Click"
                                                    CssClass="button" />
                                            </td>
                                            <td align="right">&nbsp;
                                            </td>
                                            <td align="left">&nbsp;
                                            </td>
                                            <td align="right"></td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="4">
                                                <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td id="tdlabelTitleGrn" class="labelTitleGrn" runat="server" align="center" colspan="5">
                                                <table style="padding: 0px; margin: 0px; width: 100%;">
                                                    <tr>
                                                        <td style="padding: 0px; margin: 0px; width: 52%; text-align: right;">
                                                            <asp:Label ID="lblEstimate2" style="color:#fff;" runat="server" Text="Public Templates"></asp:Label>
                                                        </td>
                                                        <td style="padding: 0px; margin: 0px; text-align: right;">
                                                            <asp:LinkButton Style="color: #fff; font-weight: bold;" ID="btnPublicDelete" runat="server" Text="Delete Selected Template(s)" OnClick="btnPublicDelete_Click" OnClientClick="return confirmDelete();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="5">
                                                <asp:GridView ID="grdPublicEstimationList" runat="server" AutoGenerateColumns="False"
                                                    CellPadding="5" CssClass="mGrid" OnRowDataBound="grdPublicEstimationList_RowDataBound"
                                                    PageSize="10" Width="100%">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Estimate Name">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypEstName1" runat="server">[hypEstName]</asp:HyperLink>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="sales_person_name" HeaderText="Sales Person">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="create_date" HeaderText="Create Date">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="chkboxPublicSelectAll" TextAlign="Left" Style="margin-left: -14px;" ForeColor="White" Text="All" runat="server" onclick="CheckPublicAllSubmit(this);" />
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkPublicDelete" runat="server" onclick="CheckPublicUnselect(this);" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr id="trSearch" runat="server">
                                            <td style="width: 180px;">
                                                <b>Estimation Template Name: </b>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtSearch" Style="margin-left: 0px;" onkeypress="return searchKeyPress(event);" runat="server"></asp:TextBox>
                                                <asp:Button ID="btnSearch" Style="margin-left: 0px;" runat="server" Text="Search" OnClick="btnSearch_Click"
                                                    CssClass="button" />
                                            </td>
                                            <td align="right">&nbsp;
                                            </td>
                                            <td align="left">&nbsp;
                                            </td>
                                            <td align="right"></td>
                                        </tr>
                                        <tr>
                                            <td id="tdlabelTitleBlu" runat="server" class="labelTitleBlu" align="center" colspan="5">
                                                <table style="padding: 0px; margin: 0px; width: 100%;">
                                                    <tr>
                                                        <td style="padding: 0px; margin: 0px; width: 52%; text-align: right;">
                                                            <asp:Label ID="lblTemplate1" style="color:#fff;" runat="server" Text="Your Templates"></asp:Label>
                                                        </td>
                                                        <td style="padding: 0px; margin: 0px; text-align: right;">
                                                            <asp:LinkButton Style="color: #fff; font-weight: bold;" ID="btnDelete" runat="server" Text="Delete Selected Template(s)" OnClick="btnDelete_Click" OnClientClick="return confirmDelete();" />
                                                        </td>
                                                    </tr>
                                                </table>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="5">
                                                <asp:GridView ID="grdEstimationList" runat="server" AutoGenerateColumns="False" CellPadding="5"
                                                    CssClass="mGrid" OnRowDataBound="grdEstimationList_RowDataBound" PageSize="10"
                                                    Width="100%">
                                                    <PagerSettings Position="TopAndBottom" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Estimate Name">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="hypEstName" runat="server">[hypEstName]</asp:HyperLink>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="sales_person_name" HeaderText="Sales Person">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="create_date" HeaderText="Create Date">
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox ID="chkboxSelectAll" Text="ALL" TextAlign="Left" Style="margin-left: -22px;" ForeColor="White" runat="server" onclick="CheckAllSubmit(this);" />

                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkDelete" runat="server" onclick="CheckUnselect(this);" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Center" Width="15%" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
     <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1"
        DynamicLayout="False">
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
