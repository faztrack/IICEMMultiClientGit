<%@ Page Title="Customer Information" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mcustomer_details.aspx.cs" Inherits="mcustomer_details" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="">
                <div class="panel panel-default">
                    <div class="panel-heading panel-heading-ext">
                        <h3 class="panel-title">
                            <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png"  Style="margin-bottom:-10px;"/>
                            <strong>
                            <asp:Label ID="lblHearder" runat="server" Text="Add New Lead"></asp:Label></strong></h3>
                    </div>
                    <div class="panel-body panel-body-ext">
                        <div class="form-horizontal">
                            <div class="panel-body">
                                 <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="lblDivision" runat="server" CssClass="col-sm-3 col-md-6 col-lg-3 control-label">Division:</asp:Label>
                                    <div class="col-sm-9 col-md-3 col-lg-9 col-ext-txtArea">
                                        <div class="row">
                                            <div class="col-sm-4 col-md-3 col-lg-4">
                                                <asp:DropDownList ID="ddlDivision" runat="server" TabIndex="1" CssClass="textBox form-control" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label1" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">First Name 1:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtFirstName1" runat="server" TabIndex="1" CssClass="textBox form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label2" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Last Name 1: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtLastName1" runat="server" TabIndex="2" CssClass="textBox form-control"></asp:TextBox>
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
                                    <asp:Label ID="Label5" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Zip Code: </asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="textBox form-control" TabIndex="6" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext-phone">
                                    <asp:Label ID="Label6" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Phone:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:TextBox ID="txtPhone" runat="server" TabIndex="7" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext-email">
                                    <asp:Label ID="Label7" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Email:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6  col-ext">
                                        <asp:TextBox ID="txtEmail" runat="server" TabIndex="9" CssClass="textBox form-control" ></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label10" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Sales Person:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:DropDownList ID="ddlSalesPerson" runat="server" TabIndex="22" CssClass="dropDownList form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext">
                                    <asp:Label ID="Label11" runat="server" CssClass="col-sm-6 col-md-6 col-lg-6 control-label">Lead Source:</asp:Label>
                                    <div class="col-sm-6 col-md-3 col-lg-6 col-ext">
                                        <asp:DropDownList ID="ddlLeadSource" runat="server" TabIndex="14" CssClass="dropDownList form-control">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group form-group-ext-txtArea">
                                    <asp:Label ID="Label12" runat="server" CssClass="col-sm-3 col-md-6 col-lg-3 control-label">Notes:</asp:Label>
                                    <div class="col-sm-9 col-md-3 col-lg-9 col-ext-txtArea">
                                        <asp:TextBox ID="txtNotes" runat="server" TabIndex="15" TextMode="MultiLine" CssClass="textBox form-control" MaxLength="500" ></asp:TextBox>
                                    </div>
                                </div>
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
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-info" OnClick="btnSubmit_Click" TabIndex="23" Text="Save" Width="80px" />
                                        &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="btn btn-default" OnClick="btnCancel_Click" TabIndex="24" Text="Close" Width="80px" />
                                    </div>
                                </div>
                                <div>
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
