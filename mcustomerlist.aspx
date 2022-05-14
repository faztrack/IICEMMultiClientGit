<%@ Page Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mcustomerlist.aspx.cs"
    Inherits="mcustomerlist" Title="Customer List" %>

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
   

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">
                      <div class="panel panel-default">
                        <div class="panel-heading panel-heading-ext">

                            <h3 class="panel-title">
                                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                                <strong>Lead List

                                </strong>

                            </h3>

                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-4 col-sm-6 col-md-6">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtSearch" CssClass="form-control form-control-ext" runat="server" TabIndex="2" onkeypress="return searchKeyPress(event);"></asp:TextBox>
                                        <span class="input-group-btn">
                                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default" OnClick="btnSearch_Click"
                                                TabIndex="3"><span class="glyphicon glyphicon-search"></span></asp:LinkButton>&nbsp;
                                            <asp:LinkButton ID="lnkViewAll" runat="server" CssClass="btn btn-default" OnClick="lnkViewAll_Click"
                                                TabIndex="4" Text="Reset">
                                            </asp:LinkButton>
                                            <asp:HyperLink ID="HyperLink1" runat="server" Style="text-align: center; padding: 12px !important;" NavigateUrl="~/mcustomer_details.aspx">
                                                  <img  src="assets/mobileicons/new_lead.png" />
                                          
                                            </asp:HyperLink>

                                        </span>
                                        <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" DelimiterCharacters=""
                                            Enabled="True" TargetControlID="txtSearch" ServiceMethod="GetLastName" MinimumPrefixLength="1"
                                            CompletionSetCount="10" EnableCaching="true" CompletionInterval="500" OnClientItemSelected="selected_LastName"
                                            CompletionListCssClass="AutoExtender" UseContextKey="True">
                                        </cc1:AutoCompleteExtender>
                                        <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearch"
                                            WatermarkText="Search by Last Name" />
                                    </div>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12  col-sm-12">
                                    <asp:GridView ID="grdCustomerList" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                        OnRowDataBound="grdCustomerList_RowDataBound" OnPageIndexChanging="grdCustomerList_PageIndexChanging"
                                        DataKeyNames="customer_id" CssClass="mGrid col-lg-12  col-sm-12">
                                        <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast" PageButtonCount="6" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="row">
                                                        <div class="col-sm-12">
                                                            <%-- <span class="glyphicon glyphicon-edit-ext"></span>--%>
                                                            
                                                            <asp:HyperLink ID="hypCustomerName" runat="server"></asp:HyperLink></span> 
                                                            <asp:HyperLink ID="hypEdit" Visible="true" runat="server" CssClass="glyphicon glyphicon-edit"></asp:HyperLink>
                                                            <asp:HyperLink ID="hypUpload" runat="server" Visible="false"></asp:HyperLink><br />
                                                            <asp:DropDownList ID="ddlEst" runat="server" CssClass="autoWidth" AutoPostBack="True" OnSelectedIndexChanged="Load_Est_Info" >
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-map-marker"></span>
                                                            <asp:HyperLink ID="hypAddress" runat="server" Target="_blank"></asp:HyperLink>
                                                        </div>
                                                       
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-phone"></span>
                                                            
                                                                <%--<asp:Label ID="lblPhone" runat="server" CssClass="phone"></asp:Label>--%>
                                                                  <asp:HyperLink ID="hypPhone" runat="server"></asp:HyperLink>  &nbsp;<asp:HyperLink ID="hypMobile" runat="server"></asp:HyperLink>
                                                           
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-envelope"></span>
                                                               <asp:HyperLink ID="hypEmail" runat="server"></asp:HyperLink>
                                                        </div>
                                                        <div class="col-sm-12">
                                                            <span class="glyphicon glyphicon-user"></span>
                                                            <asp:Label ID="lblSaleaPerson" runat="server"></asp:Label>
                                                        </div>
                                                        <div class="col-sm-12">
      
                                                           <asp:HyperLink ID="hyp_SiteReview" runat="server"><img width="40" height="40" src="assets/mobileicons/12-Site-review-List.png" alt="Site Review" title="Site Review" /></asp:HyperLink>
                                                           <asp:HyperLink ID="hyp_DocumentManagement" runat="server"><img width="40" height="40" src="assets/mobileicons/13-Documents.png" alt="Document Management" title="Document Management" /></asp:HyperLink>
                                                           <asp:HyperLink ID="hypProjectNotes" runat="server"><img width="40" height="40" src="assets/mobileicons/07-Project-Notes.png" alt="Project Notes" title="Project Notes" /></asp:HyperLink>
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
