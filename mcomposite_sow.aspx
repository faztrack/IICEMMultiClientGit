<%@ Page Title="Composite SOW" Language="C#" MasterPageFile="~/MobileSite.master" AutoEventWireup="true" CodeFile="mcomposite_sow.aspx.cs" Inherits="mcomposite_sow" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script language="javascript" type="text/javascript">
        function selected_ItemName(sender, e) {
            document.getElementById('<%=btnSearch.ClientID%>').click();
        }

        function SearchItemNamePress(e) {

            // look for window.event in case event isn't passed in
            e = e || window.event;
            if (e.keyCode == 13) {
                document.getElementById('<%=btnSearch.ClientID%>').click();
                return false;
            }
            return true;

        }
    </script>
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
                padding:10px;
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
                padding:10px;
                margin: 10px;
              
          }
  
}

/* 
  ##Device = Tablets, Ipads (portrait)
  ##Screen = B/w 768px to 1024px
*/

@media (min-width: 768px) and (max-width: 1024px) {
  
   .mobileGrid {
            font-size: 12px;
         }
        .mobileGrid th {
              
                padding: 5px;
                font-size: 13px;
          }

       .mobileGrid tbody tr td {
                padding:10px;
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
                padding:5px;
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
            margin-bottom:15px;
         }
        .mobileGrid th {
              
                padding: 2px;
                font-size: 9px;
          }

      .mobileGrid tbody tr td {
                padding:2px;
                margin: 5px;
              
          }
  
  
}

/* 
  ##Device = Most of the Smartphones Mobiles (Portrait)
  ##Screen = B/w 320px to 479px
*/

@media (min-width: 320px) and (max-width: 480px) {
  
       .mobileGrid {
            font-size: 9px;
            margin-bottom:15px;
         }
        .mobileGrid th {
              
                padding: 2px;
                font-size: 9px;
          }

      .mobileGrid tbody tr td {
                padding:2px;
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

            .mobileGrid tbody tr td {
             
                width: 12%;
            }

            .mobileGrid td {
                background-color: #f9f9f9;
                border: 1px solid #ddd;
            }

                .mobileGrid td td {
                    background-color: transparent;
                    box-shadow: none;
                }

        .overlayContent {
            z-index: 99;
            margin: 250px auto;
            text-align: center;
        }

            .overlayContent p {
                font-size: 16px;
                color: #fff;
                font-family: Arial;
                font-weight: normal;
                background-color: #000;
                margin: 0 auto 10px auto;
            }

            .overlayContent img {
                width: 80px;
                height: 80px;
            }


               

    input[type=radio] + label, input[type=checkbox] + label {
    display:inline-block;
    margin:-6px 0 0 0;
    padding: 4px 10px;
    margin-bottom: 0;
    font-size: 12px;
    line-height: 20px;
    color: #333;
    text-align: center;
    text-shadow: 0 1px 1px rgba(255,255,255,0.75);
    vertical-align: middle;
    cursor: pointer;
    font-weight:bold;
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
                                <strong>
                                    
                                    SOW &nbsp;<asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                    
                                </strong>

                            </h3>

                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                            <table cellpadding="5" cellspacing="5">
                                <asp:Panel ID="pnlEstimate" runat="server" Visible="false">
                                 <tr>
                                     <td style="padding: 5px 5px;" align="left" valign="top">
                                        <asp:Label ID="Label3" runat="server" Text="Project Name:" Font-Bold="true" Font-Size="16px"></asp:Label>
                                    </td>
                                 </tr>
                              
                                <tr>
                                    
                                    <td style="padding: 5px 5px;" align="left" valign="top">
                                        <asp:RadioButtonList ID="rdbEstimate" runat="server"  RepeatDirection="Vertical" OnSelectedIndexChanged="rdbEstimate_SelectedIndexChanged" AutoPostBack="true" >

                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                </asp:Panel>  
                                <tr>
                                    <td style="padding: 5px 0;" align="left" valign="top">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td width="90%" align="left"> 
                                                    <asp:TextBox ID="txtSearchItemName" onkeypress="return SearchItemNamePress(event);" runat="server" CssClass="form-control form-control-ext" Width="100%" Style="margin-left: 5px;"></asp:TextBox>
                                                    <cc1:AutoCompleteExtender ID="txtSearch_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_ItemName" ServiceMethod="GetItemName" TargetControlID="txtSearchItemName" UseContextKey="True">
                                                    </cc1:AutoCompleteExtender>
                                                    <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtSearchItemName" WatermarkText="Search by Item Name" />
                                                </td>
                                                <td  align="right" width="5%">
                                                    <asp:Button ID="btnSearch" runat="server" Width="80px" CssClass="btn btn-default" OnClick="btnSearch_Click" Text="Search" />
                                                </td>
                                                <td align="Center"  width="2%">
                                                    <asp:LinkButton ID="LinkButton1" runat="server" Width="80px" OnClick="lnkViewAll_Click">View All</asp:LinkButton> 
                                                </td>
                                                <td>&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td align="left" width="100%">
                                        <asp:GridView ID="grdGrouping" runat="server" AutoGenerateColumns="False"  ShowHeader="false" CssClass="mobileGrid"
                                            OnRowDataBound="grdGrouping_RowDataBound">
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" CssClass="mobileGrid itemName"
                                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound" 
                                                            Width="100%">
                                                            <Columns>
                                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes" ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                        <asp:Label ID="lblDleted1" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name" ItemStyle-Width="40%"></asp:BoundField>
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" NullDisplayText=" "></asp:BoundField>
                                                                <asp:BoundField DataField="quantity" HeaderText="Code" ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                                                                <asp:BoundField DataField="short_notes_new" HeaderText="Checklist Notes" ItemStyle-HorizontalAlign="Left"></asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" BackColor="#ffffff" />
                                                            <RowStyle BackColor="#f1f1f1" />
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" width="100%">
                                        <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" CaptionAlign="Top"
                                            CssClass="mobileGrid" OnRowDataBound="grdGroupingDirect_RowDataBound"  ShowHeader="false" >
                                            <FooterStyle CssClass="white_text" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" CssClass="grid_header" Text='<%# Eval("colName").ToString() %>' />
                                                        &nbsp;<asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False"
                                                            CssClass="mobileGrid" DataKeyNames="item_id" OnRowDataBound="grdSelectedItem2_RowDataBound"
                                                             Width="100%">
                                                            <Columns>
                                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes" />
                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblHeader2" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemName2" runat="server" Text='<%# Eval("section_name").ToString() %>'></asp:Label>
                                                                         <asp:Label ID="lblDleted2" runat="server" Text=" (Deleted Later)" Visible="false"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        <asp:Label ID="lblSubTotalLabel2" runat="server" Font-Bold="true" Font-Size="13px" />
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="item_name" HeaderText="Item Name" />
                                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" NullDisplayText=" " />
                                                                <asp:BoundField DataField="quantity" HeaderText="Code" />
                                                                <asp:BoundField DataField="tmpCo" HeaderText="Item Status"></asp:BoundField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                            <AlternatingRowStyle CssClass="alt" />
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                                </div>
                        </div>
                    </div>

                </div>

                <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnCrewId" runat="server" Value="0" />
                <asp:HiddenField ID="hdnCurrentPageNo" runat="server" Value="0" />
                 <asp:HiddenField ID="hdnCOMasterDataExist" runat="server" Value="0" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
