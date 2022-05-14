<%@ Page Title="" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mmyjobs.aspx.cs" Inherits="mmyjobs" %>

<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        /* (320x480) Smartphone, Portrait */
        /* and (max-device-width : 480px) and (-webkit-device-pixel-ratio : 3) */

        @media only screen and (min-device-width: 320px) and (orientation: portrait) {
            .gridResponsive {
                transform: scale(.65);
                -webkit-transform: scale(.65); /* Safari and Chrome */
                -moz-transform: scale(.65); /* Firefox */
                margin: -540px -108px !important;
            }
            /* (320x480) Smartphone, Portrait */
        }

        /* iPhone X-XS in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 812px) and (-webkit-device-pixel-ratio : 3) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone X-XS in portrait */
        }

        /* iPhone 6, 7, & 8 in portrait */
        @media only screen and (min-device-width : 375px) and (max-device-width : 667px) and (orientation : portrait) {
            .gridResponsive {
                transform: scale(.7);
                -webkit-transform: scale(.7); /* Safari and Chrome */
                -moz-transform: scale(.7); /* Firefox */
                margin: -400px -80px !important;
            }
            /* iPhone 6, 7, & 8 in portrait */
        }


        @media (min-width: 1281px) {
            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
        }
        /* 
  ##Device = Laptops, Desktops
  ##Screen = B/w 1025px to 1280px
*/

        @media (min-width: 1025px) and (max-width: 1280px) {

            .mobileGrid {
                font-size: 12px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 13px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }

        }

        /* 
  ##Device = Tablets, Ipads (portrait)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) {

            .mobileGrid {
                font-size: 13px;
            }

                .mobileGrid th {
                    padding: 5px;
                    font-size: 14px;
                }

                .mobileGrid tbody tr td {
                    padding: 10px;
                    margin: 10px;
                }
        }

        /* 
  ##Device = Tablets, Ipads (landscape)
  ##Screen = B/w 768px to 1024px
*/

        @media (min-width: 768px) and (max-width: 1024px) and (orientation: landscape) {

            .mobileGrid {
                font-size: 10px;
            }

                .mobileGrid th {
                    padding: 10px;
                    font-size: 10px;
                }

                .mobileGrid tbody tr td {
                    padding: 5px;
                    margin: 5px;
                }
        }

        /* 
  ##Device = Low Resolution Tablets, Mobiles (Landscape)
  ##Screen = B/w 481px to 767px
*/

        @media (min-width: 481px) and (max-width: 767px) {

            .mobileGrid {
                font-size: 9px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 2px;
                    font-size: 9px;
                }

                .mobileGrid tbody tr td {
                    padding: 2px;
                    margin: 5px;
                }
        }

        /* 
  ##Device = Most of the Smartphones Mobiles (Portrait)
  ##Screen = B/w 320px to 479px
*/

        @media (min-width: 320px) and (max-width: 480px) {

            .mobileGrid {
                font-size: 12px;
                margin-bottom: 15px;
            }

                .mobileGrid th {
                    padding: 2px;
                    font-size: 11px;
                }

                .mobileGrid tbody tr td {
                    padding: 2px;
                    margin: 5px;
                }
        }

        body {
            margin-bottom: 50px;
        }

        .grid_header {
            color: #000;
            font-weight: bold;
            font-size: 14px;
        }

        .mobileGrid {
            color: #333;
            border-radius: 0;
        }

            .mobileGrid th {
                display: normal !important;
                background-color: #e1e1e1;
                color: #555;
                font-weight: bold;
                border: 1px solid #ddd;
                text-transform: uppercase;
            }

            /*.mobileGrid tbody tr td {
                width: 12%;
            }*/

            .mobileGrid td {
                background-color: #f9f9f9;
                border: 1px solid #ddd;
            }

                .mobileGrid td td {
                    background-color: transparent;
                    box-shadow: none;
                }

       
    </style>
    <style type="text/css">
        .MyTabStyle .ajax__tab_header
        {
            font-family: "Helvetica Neue" , Arial, Sans-Serif;
            font-size: 14px;
            font-weight:bold;
            display: block;

        }
        .MyTabStyle .ajax__tab_header .ajax__tab_outer
        {
            border-color: #222;
            color: #222;
            padding-left: 10px;
            margin-right: 3px;
            border:solid 1px #d7d7d7;
        }
        .MyTabStyle .ajax__tab_header .ajax__tab_inner
        {
            border-color: #666;
            color: #666;
            padding: 3px 10px 2px 0px;
        }
        .MyTabStyle .ajax__tab_hover .ajax__tab_outer
        {
            background-color:#9c3;
        }
        .MyTabStyle .ajax__tab_hover .ajax__tab_inner
        {
            color: #fff;
        }
        .MyTabStyle .ajax__tab_active .ajax__tab_outer
        {
            border-bottom-color: #ffffff;
            background-color: #d7d7d7;
        }
        .MyTabStyle .ajax__tab_active .ajax__tab_inner
        {
            color: #000;
            border-color: #333;
        }
        .MyTabStyle .ajax__tab_body
        {
            font-family: verdana,tahoma,helvetica;
            font-size: 10pt;
            background-color: #fff;
            border-top-width: 0;
            border: solid 1px #d7d7d7;
            border-top-color: #ffffff;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12 col-sm-12 col-md-12">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-ext">

                            <h3 class="panel-title">
                                <asp:ImageButton ID="imgBack" runat="server" OnClick="imgBack_Click" ImageUrl="~/assets/mobileicons/back_header.png" Style="margin-bottom: -10px;" />
                                <strong>My Jobs &nbsp;

                                </strong>

                            </h3>

                        </div>
                        <div class="panel-body">
                            <asp:TabContainer ID="TabContainer4" runat="server" ActiveTabIndex="1"  Width="100%" CssClass="MyTabStyle">
                                <asp:TabPanel runat="server" HeaderText="Past" ID="TabPast">
                                         <ContentTemplate>
                                              <div class="table-responsive">
                                        <table cellpadding="5" cellspacing="5" width="100%">

                                            <tr>
                                                <td align="left" width="100%">
                                                    <asp:GridView ID="grdPast" runat="server" AutoGenerateColumns="False" CssClass="mobileGrid itemName"
                                                        OnRowDataBound="grdPast_RowDataBound"
                                                        OnRowCommand="grdPast_RowCommand"
                                                        Width="100%">
                                                        <Columns>

                                                            <asp:TemplateField HeaderText="Last Name" ItemStyle-Font-Underline="true">

                                                                <ItemTemplate>
                                                                  <asp:LinkButton ID="InkLastName" runat="server" CommandName="LastName" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>


                                                            <%-- <asp:BoundField DataField="ProjectName" HeaderText="Project Name">
                                                       <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                       <ItemStyle HorizontalAlign="Left" />
                                                     </asp:BoundField>--%>
                                                            <asp:TemplateField HeaderText="Address">

                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>
                                                            <asp:BoundField HeaderText="Schedule Date" DataField="ScheduleDate" DataFormatString="{0:d}">
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                        <RowStyle BackColor="#f1f1f1" />
                                                    </asp:GridView>

                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                   </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel ID="TabToday" runat="server" HeaderText="Today">
                                   <ContentTemplate>
                                              <div class="table-responsive">
                                        <table cellpadding="5" cellspacing="5" width="100%">

                                            <tr>
                                                <td align="left" width="100%">
                                                    <asp:GridView ID="grdMyJobs" runat="server" AutoGenerateColumns="False" CssClass="mobileGrid itemName"
                                                        OnRowDataBound="grdMyJobs_RowDataBound"
                                                        OnRowCommand="grdMyJobs_RowCommand"
                                                        Width="100%">
                                                        <Columns>

                                                            <asp:TemplateField HeaderText="Last Name" ItemStyle-Font-Underline="true">

                                                                <ItemTemplate>
                                                                  <asp:LinkButton ID="InkLastName" runat="server" CommandName="LastName" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>


                                                            <%-- <asp:BoundField DataField="ProjectName" HeaderText="Project Name">
                                                       <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                       <ItemStyle HorizontalAlign="Left" />
                                                     </asp:BoundField>--%>
                                                            <asp:TemplateField HeaderText="Address">

                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>
                                                            <asp:BoundField HeaderText="Schedule Date" DataField="ScheduleDate" DataFormatString="{0:d}">
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                        <RowStyle BackColor="#f1f1f1" />
                                                    </asp:GridView>

                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                   </ContentTemplate>
                                </asp:TabPanel>
                                <asp:TabPanel ID="TapFuture" runat="server" HeaderText="Future">
                                      <ContentTemplate>
                                              <div class="table-responsive">
                                        <table cellpadding="5" cellspacing="5" width="100%">

                                            <tr>
                                                <td align="left" width="100%">
                                                    <asp:GridView ID="grdFuture" runat="server" AutoGenerateColumns="False" CssClass="mobileGrid itemName"
                                                        OnRowDataBound="grdFuture_RowDataBound"
                                                        OnRowCommand="grdFuture_RowCommand"
                                                        Width="100%">
                                                        <Columns>

                                                            <asp:TemplateField HeaderText="Last Name" ItemStyle-Font-Underline="true">

                                                                <ItemTemplate>
                                                                   <asp:LinkButton ID="InkLastName" runat="server" CommandName="LastName" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>


                                                            <%-- <asp:BoundField DataField="ProjectName" HeaderText="Project Name">
                                                       <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                       <ItemStyle HorizontalAlign="Left" />
                                                     </asp:BoundField>--%>
                                                            <asp:TemplateField HeaderText="Address">

                                                                <ItemTemplate>
                                                                    <asp:HyperLink ID="hypAddress" runat="server" Target="_blank">[hypAddress]</asp:HyperLink>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                                                <ItemStyle HorizontalAlign="Left" />

                                                            </asp:TemplateField>
                                                            <asp:BoundField HeaderText="Schedule Date" DataField="ScheduleDate" DataFormatString="{0:d}">
                                                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:BoundField>

                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                        <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                        <RowStyle BackColor="#f1f1f1" />
                                                    </asp:GridView>

                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                   </ContentTemplate>
                                </asp:TabPanel>
                            </asp:TabContainer>









                        </div>
                    </div>

                </div>

                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />

                <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

