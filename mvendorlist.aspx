<%@ Page Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mvendorlist.aspx.cs"
    Inherits="mvendorlist" Title="Vendor List" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function selected_LastName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
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

    </script>
    <script>
        function openModal() {
            $('#myModal').modal({ show: true });
        }
    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>


            <!-- Attachment View Modal content-->
            <div id="myModal" class="modal fade" role="dialog" data-backdrop="static" data-keyboard="false" style="z-index: 9999999">
                <div class="modal-dialog">

                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <button id="Button1" class="close" data-dismiss="modal" runat="server">&times;</button>

                            <h4 class="modal-title">Contact Information</h4>
                        </div>
                        <div class="modal-body" style="padding: 7px;margin:-7px;">
                            <asp:GridView ID="grdVendorDetails" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="grdVendorDetails_RowDataBound"
                                CssClass="mGrid col-lg-12  col-sm-12">
                                <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <div  class="row">
                                                <div class="col-sm-12">

                                                    <span class="glyphicon glyphicon-user"></span>
                                                    <asp:Label ID="hypSalesRep" runat="server"></asp:Label>
                                                </div>


                                                <div class="col-sm-12">
                                                    <span class="glyphicon glyphicon-phone"></span>
                                                    <asp:HyperLink ID="hypVPhone" runat="server"></asp:HyperLink>

                                                </div>
                                                <div style="padding-bottom:15px;" class="col-sm-12">
                                                    <span class="glyphicon glyphicon-envelope"></span>
                                                    <asp:HyperLink ID="hypVEmail" runat="server"></asp:HyperLink>
                                                </div>


                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </div>

                    </div>

                </div>
            </div>
            <!-- Attachment View Modal content-->


            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">



                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-ext">

                            <h3 class="panel-title">
                                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                                <strong>Vendor List

                                </strong>

                            </h3>

                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-4 col-sm-6 col-md-6">

                                    <div class="input-group">


                                        <span class="input-group-btn">
                                            <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged" CssClass="form-control form-control-ext" Width="110px">
                                                <asp:ListItem Value="1" Selected="True">Vendor Name</asp:ListItem>
                                                <asp:ListItem Value="2">Section</asp:ListItem>

                                            </asp:DropDownList>
                                            <asp:TextBox ID="txtSearch" CssClass="form-control form-control-ext" runat="server" TabIndex="2" onkeypress="return searchKeyPress(event);" style="width:155px"></asp:TextBox>
                                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default" OnClick="btnSearch_Click"
                                                TabIndex="3"><span class="glyphicon glyphicon-search"></span></asp:LinkButton>&nbsp;
                                            <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="btn btn-default" OnClick="lnkViewAll_Click"
                                                TabIndex="4">
                                                <span class="glyphicon glyphicon-refresh"></span>
                                            </asp:LinkButton>
                                           
                                           
                                        </span>
                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" DelimiterCharacters=""
                                            Enabled="True" TargetControlID="txtSearch" ServiceMethod="GetVendorName" MinimumPrefixLength="1"
                                            CompletionSetCount="10" EnableCaching="true" CompletionInterval="500" OnClientItemSelected="selected_LastName"
                                            CompletionListCssClass="AutoExtender" UseContextKey="True">
                                        </cc1:AutoCompleteExtender>
                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch"
                                            WatermarkText="Search by Vendor Name" />
                                    </div>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-4 col-sm-6 col-md-6">
                                    <div class="input-group">
                                        <span class="input-group-btn">
                                            

                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12  col-sm-12">
                                    <asp:GridView ID="grdVendorList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        OnRowDataBound="grdVendorList_RowDataBound" OnPageIndexChanging="grdVendorList_PageIndexChanging"
                                         CssClass="mGrid col-lg-12  col-sm-12">
                                        <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="row">
                                                        <div class="col-sm-12">

                                                            <span class="fa fa-industry"></span>
                                                            <asp:Label ID="hypVendorName" runat="server"></asp:Label>
                                                            <%--   <asp:HyperLink ID="hypVendorName" runat="server"></asp:HyperLink>--%>
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-map-marker"></span>
                                                            <asp:HyperLink ID="hypAddress" runat="server" Target="_blank"></asp:HyperLink>
                                                        </div>

                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-phone"></span>
                                                            <asp:HyperLink ID="hypPhone" runat="server"></asp:HyperLink>

                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-envelope"></span>
                                                            <asp:HyperLink ID="hypEmail" runat="server"></asp:HyperLink>
                                                        </div>
                                                        <div class="col-sm-12">

                                                            <asp:Label ID="lblSection" runat="server"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <asp:LinkButton ID="inkViewSalesRep" runat="server" OnClick="viewDetails" Visible="false"></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="9%" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
