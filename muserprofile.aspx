<%@ Page Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="muserprofile.aspx.cs" Inherits="muserprofile" Title="User Profile" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="">
                <div class="panel panel-default">
                    <div class="panel-heading panel-heading-ext">
                        <h3 class="panel-title">
                            <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png"  Style="margin-bottom:-10px;"/>
                            <strong>
                            <asp:Label ID="lblHearder" runat="server" Text="User Profile"></asp:Label>
                            </strong>
                         

                        </h3>
                    </div>
                    <div class="panel-body panel-body-ext">
                        <div class="form-horizontal">
                            <div class="panel-body">
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label1" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>First Name:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtFirstName" runat="server" TabIndex="1" CssClass="textBox form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label2" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Last Name: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtLastName" runat="server" TabIndex="2" CssClass="textBox form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label9" runat="server" CssClass="col-sm-3 col-md-6 col-lg-3 control-label">Address: </asp:Label>
                                    <div class="col-sm-9 col-md-3 col-lg-9 col-ext-txtArea">
                                        <asp:TextBox ID="txtAddress" runat="server" TabIndex="3" TextMode="MultiLine" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label3" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">City: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtCity" runat="server" TabIndex="4" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="form-group form-group-ext">
                                    <asp:Label ID="Label4" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">State: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:DropDownList ID="ddlState" runat="server" TabIndex="5" Width="120px" CssClass="textBox form-control">
                                    </asp:DropDownList>
                                    </div>
                                </div>
                                 <div class="form-group form-group-ext">
                                    <asp:Label ID="Label5" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Zip: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtZip" runat="server" TabIndex="6" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label6" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Phone:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtPhone" runat="server" TabIndex="7" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label7" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Email:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtEmail" runat="server" TabIndex="8" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label8" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Username:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtUsername" runat="server" TabIndex="9" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label10" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Password:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtPassword" runat="server" TabIndex="10" CssClass="textBox form-control" TextMode="Password"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label11" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Confirm Password:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtConfirmPass" runat="server" TabIndex="11" CssClass="textBox form-control"  TextMode="Password"></asp:TextBox>
                                    </div>
                                </div>
                                 <div class="form-group form-group-ext-txtAreat">
                                    <asp:Label ID="Label12" runat="server" CssClass="col-sm-12 col-md-6 col-lg-3 control-label"><span style="color:red">*</span>Password Verification Question:</asp:Label>
                                    <div class="col-sm-12 col-md-3 col-lg-9  col-ext">
                                          <asp:DropDownList ID="ddlQuestion" runat="server" TabIndex="12" CssClass="textBox form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label13" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label"><span style="color:red">*</span>Answer:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtAnswer" runat="server" TabIndex="13" Width="150px" CssClass="textBox form-control"></asp:TextBox>
                                    </div>
                                </div>
                                 <%--<div class="form-group form-group-ext">
                                    <asp:Label ID="Label8" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Username:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtUserName" runat="server" TabIndex="1" CssClass="textBox form-control"></asp:TextBox>
                                    </div>
                                </div>--%>
                                <%--<div class="form-group form-group-ext">
                                    <asp:Label ID="Label10" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Sales Person:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:DropDownList ID="ddlSalesPerson" runat="server" TabIndex="22" CssClass="dropDownList form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>--%>
                              <%--  <div class="form-group form-group-ext">
                                    <asp:Label ID="Label11" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Lead Source:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="14" CssClass="dropDownList form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>--%>
                                <%--<div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label12" runat="server" CssClass="col-sm-3 col-md-6 col-lg-3 control-label">Notes:</asp:Label>
                                    <div class="col-sm-9 col-md-3 col-lg-9 col-ext-txtArea">
                                        <asp:TextBox ID="txtNotes" runat="server" TabIndex="15" TextMode="MultiLine" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>--%>
                                <%--<div class="form-group">
                                <asp:Label ID="lblRegDate" runat="server" Font-Bold="True" Text="Entry Date:" CssClass="col-md-6 col-sm-6 control-label" Visible="False"></asp:Label>
                                <div class="col-md-6 col-sm-6">
                                    <asp:Label ID="lblRegDateData" runat="server" Visible="False"></asp:Label>
                                </div>
                            </div>
                              <div class="form-group">
                                <asp:Label ID="Label4" runat="server" CssClass="col-md-6 col-sm-6 control-label">State: </asp:Label>

                                <div class="col-md-6 col-sm-6">
                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5" CssClass="dropDownList form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="Label8" runat="server" CssClass="col-md-6 col-sm-6 control-label">Status:</asp:Label>

                                <div class="col-md-6 col-sm-6">
                                    <asp:DropDownList ID="ddlStatus" runat="server" TabIndex="20" CssClass="dropDownList form-control">
                                        <asp:ListItem Value="1">New</asp:ListItem>
                                        <asp:ListItem Value="2">Follow-up</asp:ListItem>
                                        <asp:ListItem Value="3">In-Design</asp:ListItem>
                                        <asp:ListItem Value="4">Archive</asp:ListItem>
                                        <asp:ListItem Value="5">Dead</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>--%>

                                <div class="form-group">
                                    <div class="col-md-10" style="text-align: center;">
                                        <asp:Label ID="lblResult" runat="server" CssClass="control-label"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-11" style="text-align: center;">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="orngButton"  TabIndex="14" Text="Update" Width="80px" OnClick="btnSubmit_Click" />
                                        &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="brownButton"  TabIndex="15" Text="Close" Width="80px"  OnClick="btnCancel_Click"/>
                                    </div>
                                </div>
                                <div>
                                    <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

