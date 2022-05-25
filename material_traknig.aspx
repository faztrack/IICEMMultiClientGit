<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="material_traknig.aspx.cs" Inherits="material_traknig" %>

<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.0.30930.22922, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript">
        function confirmDelete() {
            return confirm("Are you sure that you want to delete this Order?");
        }
        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>

    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="cssHeader" align="center">
                <table cellpadding="0" cellspacing="0" width="100%" align="center">
                    <tr>
                        <td align="center" class="cssHeader">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <%--<asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Material Tracking</asp:Label></span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label>--%>
                                            <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Material Tracking</asp:Label></span><asp:Label runat="server" CssClass="titleNu" ID="lblTitelJobNumber"></asp:Label>
                                    </td>
                                    <td align="right" style="padding-right: 30px;">
                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="left" valign="middle">
                                                    <%--<asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" OnCheckedChanged="chkPMDisplay_CheckedChanged" />--%>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%">
                    <tr>
                        <td>

                            <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="center">

                                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td align="center">
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr style="padding: 0px; margin: 0px;">
                                                            <td align="left" style="width: 50px;">Project:</td>
                                                            <td align="left" style="width: 250px;">
                                                                <asp:DropDownList ID="ddlEst" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlEst_SelectedIndexChanged">
                                                                </asp:DropDownList></td>
                                                            <td align="left">
                                                                <asp:Panel ID="Panel2" runat="server">
                                                                    <span id="PnlCtrlID" runat="server">
                                                                        <asp:LinkButton ID="lnkAddNewCall" Style="padding-left: 6px;" runat="server" CssClass="button">
                                                                            <asp:ImageButton ID="ImageTGI2" runat="server" ImageUrl="~/Images/expand.png" CssClass="blindInput" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="100" />
                                                                            Add Material Tracking Info
                                                                        </asp:LinkButton>
                                                                    </span>
                                                                </asp:Panel>


                                                            </td>



                                                            <td style="padding: 0px; margin: 0px; text-align: right;">

                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server" ImageControlID="ImageTGI2" CollapseControlID="lnkAddNewCall"
                                                                    ExpandControlID="lnkAddNewCall" SuppressPostBack="true" CollapsedImage="Images/expand.png" ExpandedImage="Images/collapse.png" TargetControlID="pnlTGI2" Collapsed="True">
                                                                </cc1:CollapsiblePanelExtender>
                                                            </td>
                                                        </tr>


                                                    </table>
                                                    <table cellpadding="0" cellspacing="0" width="62%">
                                                        <tr style="padding: 0px; margin: 0px;">
                                                            <td style="padding: 0px; margin: 0px;" align="center">
                                                                <asp:Panel ID="pnlTGI2" runat="server" Height="100%">
                                                                    <table class="wrappermini" style="padding: 0px; margin: 0px;" width="100%">

                                                                        <tr>
                                                                            <td align="right" style="width: 25%"><span class="required">* </span>Order Date: </td>
                                                                            <td align="left" style="width: 75%">
                                                                                <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                                                                    <tr>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtSelectionDate" runat="server"></asp:TextBox>
                                                                                            <asp:Label ID="lblSelectionDate" runat="server" Visible="false"></asp:Label>
                                                                                        </td>
                                                                                        <td>&nbsp;</td>
                                                                                        <td align="left">
                                                                                            <asp:ImageButton ID="imgSelectionDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                            <cc1:CalendarExtender ID="extSelectionDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgSelectionDate" TargetControlID="txtSelectionDate">
                                                                                            </cc1:CalendarExtender>
                                                                                        </td>

                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>

                                                                        <tr>
                                                                            <td align="right"><span class="required">* </span>Section: </td>
                                                                            <td align="left">
                                                                                <table style="padding: 0px; margin: 0px; border: none;">
                                                                                    <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                        <td style="padding: 0px; margin: 0px; border: none;">
                                                                                            <asp:DropDownList ID="ddlSection" runat="server" DataValueField="section_id" DataTextField="section_name" AutoPostBack="True" OnSelectedIndexChanged="ddlSection_IndexChanged">
                                                                                            </asp:DropDownList>
                                                                                            <asp:Label ID="lblSection" runat="server" Visible="false"></asp:Label>
                                                                                        </td>
                                                                                        <td align="right" style="padding: 0px; margin: 0px; border: none;">Vendor: </td>
                                                                                        <td align="left">
                                                                                            <asp:DropDownList ID="ddlVendor" runat="server" DataTextField="vendor_name" DataValueField="vendor_id">
                                                                                            </asp:DropDownList>
                                                                                            <asp:Label ID="lblVendor" runat="server" Visible="false"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>

                                                                        </tr>

                                                                        <tr>
                                                                            <td align="right"><span class="required">* </span>Item Name: </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtTitle" runat="server" Style="width: 840px;"></asp:TextBox>
                                                                                <asp:Label ID="lblTitle" runat="server" Visible="false"></asp:Label>
                                                                            </td>

                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right" valign="top">
                                                                                <asp:Label ID="Label2" runat="server"><span class="required">* </span> Notes:</asp:Label>
                                                                                <br />
                                                                                (Up to 500 Characters)
                                                                                    <br />
                                                                                <asp:TextBox ID="txtDisplayCall" runat="server" BackColor="Transparent" CssClass="nostyle"
                                                                                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                                                                                    ReadOnly="True">
                                                                                </asp:TextBox>
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtNotes" runat="server" TabIndex="109" Height="60px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplayCall'));" TextMode="MultiLine" Width="844px"></asp:TextBox>
                                                                                <asp:Label ID="lblNotes" runat="server" Visible="false"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="lblPShipped" runat="server" Text="Shipped:"></asp:Label>


                                                                                <asp:CheckBox ID="chkShipped" runat="server" />
                                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtenderShipped" runat="server" CollapseControlID="chkShipped"
                                                                                    ExpandControlID="chkShipped" SuppressPostBack="False" TargetControlID="PaneTrackingInfo" Collapsed="true">
                                                                                </cc1:CollapsiblePanelExtender>

                                                                            </td>

                                                                            <td align="left">
                                                                                <asp:Panel ID="PaneTrackingInfo" runat="server">
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td align="right" width="88px">Tracking Info:</td>
                                                                                            <td align="left" width="18%">
                                                                                                <asp:TextBox ID="txtShippedTrackingInfo" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>

                                                                                            <td align="left">
                                                                                                <asp:TextBox ID="txtShippedTrackingInfoDate" runat="server" Width="78px"></asp:TextBox>
                                                                                            </td>

                                                                                            <td align="left" align="left" width="32px">
                                                                                                <asp:ImageButton ID="imgShippedTrackingInfoDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                <cc1:CalendarExtender ID="CalendarExtenderShippedTrackingInfoDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgShippedTrackingInfoDate" TargetControlID="txtShippedTrackingInfoDate">
                                                                                                </cc1:CalendarExtender>
                                                                                            </td>
                                                                                            <td align="left" width="5px">&nbsp;</td>

                                                                                            <td align="left" width="5%">Notes: 
                                                                                            </td>
                                                                                            <td align="left" width="35%">
                                                                                                <asp:TextBox ID="txtShippedNotes" runat="server" Width="350px"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </asp:Panel>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label1" runat="server" Text="Received:"></asp:Label>


                                                                                <asp:CheckBox ID="chkReceived" runat="server" />
                                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtenderReceived" runat="server" CollapseControlID="chkReceived"
                                                                                    ExpandControlID="chkReceived" SuppressPostBack="False" TargetControlID="PanelReceived" Collapsed="true">
                                                                                </cc1:CollapsiblePanelExtender>
                                                                            </td>

                                                                            <td align="left">
                                                                                <asp:Panel ID="PanelReceived" runat="server">
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td align="right" width="88px">Received By:
                                                                                            </td>
                                                                                            <td align="left" width="18%">
                                                                                                <asp:TextBox ID="txtRecReceivedBy" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td align="left">
                                                                                                <asp:TextBox ID="txtRecReceivedByDate" runat="server" Width="78px"></asp:TextBox>
                                                                                            </td>

                                                                                            <td align="left" align="left" width="32px">
                                                                                                <asp:ImageButton ID="ImgRecReceivedByDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                <cc1:CalendarExtender ID="CalendarExtenderRecReceivedByDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="ImgRecReceivedByDate" TargetControlID="txtRecReceivedByDate">
                                                                                                </cc1:CalendarExtender>
                                                                                            </td>
                                                                                            <td align="left" width="5px">&nbsp;</td>
                                                                                            <td align="left" width="5%">Notes: 
                                                                                            </td>
                                                                                            <td align="left" width="35%">
                                                                                                <asp:TextBox ID="txtRecNotes" runat="server" Width="350px"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>

                                                                                </asp:Panel>
                                                                            </td>

                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label3" runat="server" Text="Picked Up:"></asp:Label>

                                                                                <asp:CheckBox ID="chkPicked" runat="server" />
                                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtenderPicked" runat="server" CollapseControlID="chkPicked"
                                                                                    ExpandControlID="chkPicked" SuppressPostBack="False" TargetControlID="PanelPicked" Collapsed="true">
                                                                                </cc1:CollapsiblePanelExtender>
                                                                            </td>

                                                                            <td align="left">
                                                                                <asp:Panel ID="PanelPicked" runat="server">
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td align="right" width="88px">Picked By:
                                                                                            </td>
                                                                                            <td align="left" width="18%">
                                                                                                <asp:TextBox ID="txtPickedReceivedBy" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td align="left">
                                                                                                <asp:TextBox ID="txtPickedReceivedByDate" runat="server" Width="78px"></asp:TextBox>
                                                                                            </td>

                                                                                            <td align="left" align="left" width="32px">
                                                                                                <asp:ImageButton ID="ImgPickedReceivedByDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                <cc1:CalendarExtender ID="CalendarExtenderPickedReceivedByDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="ImgPickedReceivedByDate" TargetControlID="txtPickedReceivedByDate">
                                                                                                </cc1:CalendarExtender>
                                                                                            </td>
                                                                                            <td align="left" width="5px">&nbsp;</td>
                                                                                            <td align="left" width="5%">Notes: 
                                                                                            </td>
                                                                                            <td align="left" width="35%">
                                                                                                <asp:TextBox ID="txtPickedNotes" runat="server" Width="350px"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </asp:Panel>
                                                                            </td>

                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="Label4" runat="server" Text="Confirmed:"></asp:Label>


                                                                                <asp:CheckBox ID="chkConfirmed" runat="server" />
                                                                                <cc1:CollapsiblePanelExtender ID="CollapsiblePanelExtenderConfirmed" runat="server" CollapseControlID="chkConfirmed"
                                                                                    ExpandControlID="chkConfirmed" SuppressPostBack="False" TargetControlID="PanelConfirmed" Collapsed="true">
                                                                                </cc1:CollapsiblePanelExtender>
                                                                            </td>

                                                                            <td align="left">
                                                                                <asp:Panel ID="PanelConfirmed" runat="server">
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td align="right" width="88px">Confirmed By:
                                                                                            </td>
                                                                                            <td align="left" width="18%">
                                                                                                <asp:TextBox ID="txtConfimedBy" runat="server" Width="200px"></asp:TextBox>
                                                                                            </td>
                                                                                            <td align="left">
                                                                                                <asp:TextBox ID="txtConfimedByDate" runat="server" Width="78px"></asp:TextBox>
                                                                                            </td>

                                                                                            <td align="left" align="left" width="32px">
                                                                                                <asp:ImageButton ID="ImgConfimedByDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                <cc1:CalendarExtender ID="CalendarExtenderConfimedByDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="ImgConfimedByDate" TargetControlID="txtConfimedByDate">
                                                                                                </cc1:CalendarExtender>
                                                                                            </td>
                                                                                            <td align="left" width="5px">&nbsp;</td>
                                                                                            <td align="left" width="5%">Notes: 
                                                                                            </td>
                                                                                            <td align="left" width="35%">
                                                                                                <asp:TextBox ID="txtConfirmedNotes" runat="server" Width="350px"></asp:TextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>


                                                                                </asp:Panel>
                                                                            </td>

                                                                        </tr>

                                                                        <tr>
                                                                            <td align="right" valign="top">&nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right" valign="top">&nbsp;</td>
                                                                            <td>
                                                                                <table style="padding: 0px; margin: 0px; border: none;">
                                                                                    <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                        <td style="padding: 0px; margin: 0px; border: none;">
                                                                                            <asp:FileUpload ID="file_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                                        </td>
                                                                                        <td style="padding: 0px; margin: 0px; border: none;"></td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right" valign="top">&nbsp;</td>
                                                                            <td align="left">
                                                                                <asp:Button ID="btnSave" runat="server" CssClass="button" TabIndex="110" Text=" Save " OnClick="btnSave_click" OnClientClick="ShowProgress();" />
                                                                                <asp:Button ID="btnCancel" runat="server" CssClass="button" TabIndex="110" Text=" Cancel " OnClick="btnCancel_click" OnClientClick="ShowProgress();" />

                                                                            </td>
                                                                        </tr>


                                                                    </table>

                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>

                                                    <asp:Label ID="lblResult2" runat="server"></asp:Label>

                                                </td>
                                            </tr>

                                            <tr>
                                                <td align="center">
                                                    <asp:GridView ID="grdMaterial" runat="server" AutoGenerateColumns="False"
                                                        CssClass="mGrid"
                                                        PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdMaterial_RowDataBound" OnRowUpdating="grdSelection_RowUpdating">

                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Project">
                                                                <ItemTemplate>

                                                                    <asp:Label ID="lblProject" runat="server" Text='<%# Eval("estimate_name")%>' />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <%--cell=0--%>
                                                            <asp:TemplateField HeaderText="Order Date">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblOrderDate" runat="server" Text='<%# Eval("Order_date","{0:d}")%>' />
                                                                    <div id="dvCalender" runat="server" visible="false">
                                                                        <table style="padding: 0px; margin: 0px;">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtOrderDate" runat="server" Text='<%# Eval("Order_date","{0:d}") %>' Width="65px"></asp:TextBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ImageButton CssClass="nostyleCalImg" ID="imgDate" runat="server" ImageUrl="~/images/calendar.gif" />
                                                                                    <cc1:CalendarExtender ID="extOrderDate" runat="server" Format="MM/dd/yyyy" PopupPosition="BottomLeft" PopupButtonID="imgDate" TargetControlID="txtOrderDate">
                                                                                    </cc1:CalendarExtender>
                                                                                </td>
                                                                            </tr>
                                                                        </table>


                                                                    </div>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                            </asp:TemplateField>
                                                            <%--cell=1--%>
                                                            <asp:TemplateField HeaderText="Section">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlgSectiong" CssClass="secDD" runat="server" Visible="false" Enabled="true" DataValueField="section_id" DataTextField="section_name" AutoPostBack="True" OnSelectedIndexChanged="ddlgVendor_SelectedIndexChanged">
                                                                    </asp:DropDownList>



                                                                    <asp:Label ID="lblSectiong" runat="server" Text='<%# Eval("Section_name") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <%--cell=2--%>
                                                            <asp:TemplateField HeaderText="Vendor">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlgVendor" CssClass="secDD" runat="server" Visible="false" Enabled="True" AutoPostBack="true">
                                                                    </asp:DropDownList>
                                                                    <asp:Label ID="lblVendor" runat="server" Text='<%# Eval("Vendor_name") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <%--cell=3--%>
                                                            <asp:TemplateField HeaderText="Item">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblItem" runat="server" Text='<%# Eval("Item_text") %>' />
                                                                    <asp:TextBox ID="txtItem" runat="server" Visible="false" TextMode="MultiLine"
                                                                        Text='<%# Eval("Item_text") %>' Width="98%" Height="40px"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="5%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <%--cell=4--%>
                                                            <asp:TemplateField HeaderText="Notes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblItemnote" runat="server" Text='<%# Eval("Item_note") %>' Style="display: inline;" />
                                                                    <asp:TextBox ID="txtItemnote" runat="server" Visible="false" TextMode="MultiLine"
                                                                        Text='<%# Eval("Item_note") %>' Width="8%" Height="40px"></asp:TextBox>

                                                                    <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblItemnote_r" runat="server" Text='<%# Eval("Item_note") %>' Visible="false" ></asp:Label></pre>
                                                                    <asp:LinkButton ID="lnkOpen_Item_note" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click_Item_note" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>
                                                            <%--cell=5--%>
                                                            <asp:TemplateField HeaderText="Information">
                                                                <ItemTemplate>
                                                                    <table class="iGrid" width="100%">
                                                                        <tr id="trShipped" runat="server" visible='<%# Convert.ToBoolean(Eval("Is_Shipped")) == true %>' width="20%">
                                                                            <td>Shipped Date: </td>
                                                                            <td>
                                                                                <asp:Label ID="lblgShippedTrackingDate" runat="server" Text='<%# Eval("Shipped_date","{0:d}") %>' /></td>
                                                                            <td>Shipped Tracking Info: </td>
                                                                            <td>
                                                                                <asp:Label ID="lblgShippedTrackingInfo" runat="server" Text='<%# Eval("Shipped_by") %>' /></td>
                                                                            <td>Note: </td>
                                                                            <td>
                                                                                <asp:Label ID="lblgShippedTrackingNote" runat="server" Text='<%# Eval("Shipped_note") %>' /></td>
                                                                        </tr>

                                                                        <tr id="trRecieved" runat="server" visible='<%# Convert.ToBoolean(Eval("Is_Received")) == true %>'>
                                                                            <td>Received Date:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgRecevedDate" runat="server" Text='<%# Eval("Received_date","{0:d}") %>' /></td>
                                                                            <td>Received By</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgRecevedInfo" runat="server" Text='<%# Eval("Received_by") %>' /></td>
                                                                            <td>Note:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgRecevedNote" runat="server" Text='<%# Eval("Received_note") %>' /></td>
                                                                        </tr>

                                                                        <tr id="trPicked" runat="server" visible='<%# Convert.ToBoolean(Eval("Is_Picked")) == true %>'>
                                                                            <td>Picked Date:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgPickedDate" runat="server" Text='<%# Eval("Picked_date","{0:d}") %>' /></td>
                                                                            <td>Picked By:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgPickedInfo" runat="server" Text='<%# Eval("Picked_by") %>' /></td>
                                                                            <td>Note:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgPickedNote" runat="server" Text='<%# Eval("Picked_note") %>' /></td>
                                                                        </tr>

                                                                        <tr id="trConfirmed" runat="server" visible='<%# Convert.ToBoolean(Eval("Is_Confirmed")) == true %>'>
                                                                            <td>Confirmed Date:</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgConfirmedDate" runat="server" Text='<%# Eval("Confirmed_date","{0:d}") %>' /></td>
                                                                            <td>Confirmed By</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgConfirmedInfo" runat="server" Text='<%# Eval("Confirmed_by") %>' /></td>
                                                                            <td>Note</td>
                                                                            <td>
                                                                                <asp:Label ID="lblgConfirmedNote" runat="server" Text='<%# Eval("Confirmed_note") %>' /></td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" />
                                                                <ItemStyle Width="35%" HorizontalAlign="Left" />
                                                            </asp:TemplateField>

                                                            <%--cell=13--%>
                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <table style="padding: 0px; margin: 0px; border: none;">
                                                                        <tr style="padding: 0px; margin: 0px; border: none;">
                                                                            <td style="padding: 0px; margin: 0px; border: none;">

                                                                                <asp:GridView ID="grdUploadedFileList" runat="server" AutoGenerateColumns="False"
                                                                                    CssClass="uGrid" ShowHeader="false" ShowFooter="false" BorderStyle="None" Style="padding: 0px; margin: 0px; border: none;"
                                                                                    OnRowDataBound="grdUploadedFileList_RowDataBound">
                                                                                    <Columns>
                                                                                        <asp:TemplateField>
                                                                                            <ItemTemplate>
                                                                                                <div class="clearfix">
                                                                                                    <div class="divImageCss">
                                                                                                        <asp:HyperLink ID="hypImg" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                            <asp:Image ID="img" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />
                                                                                                        </asp:HyperLink>
                                                                                                        <asp:HyperLink ID="hypPDF" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                            <asp:Image ID="imgPDF" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                        </asp:HyperLink>
                                                                                                        <asp:HyperLink ID="hypExcel" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                            <asp:Image ID="imgExcel" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                        </asp:HyperLink>
                                                                                                        <asp:HyperLink ID="hypDoc" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                            <asp:Image ID="imgDoc" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                        </asp:HyperLink>
                                                                                                        <asp:HyperLink ID="hypTXT" runat="server" CssClass="hypimgCss" data-imagelightbox="g" data-ilb2-caption="" Visible="false" ToolTip="Click here to view file">
                                                                                                            <asp:Image ID="imgTXT" onerror="this.src='Images/No_image_available.jpg';" runat="server" CssClass="imgCss blindInput" />

                                                                                                        </asp:HyperLink>

                                                                                                    </div>
                                                                                                    <div style="text-align: center; padding: 5px; font-weight: bold;">
                                                                                                        <asp:Label ID="lblFileName" runat="server" Text=""></asp:Label>
                                                                                                        <asp:Button ID="btnDeleteUploadedFile" Text="Delete" runat="server" OnClick="DeleteUploadedFile" CommandArgument='<%#Eval("upload_fileId")%>' OnClientClick="ShowProgress();" />
                                                                                                    </div>
                                                                                                </div>

                                                                                            </ItemTemplate>
                                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                                        </asp:TemplateField>

                                                                                    </Columns>
                                                                                </asp:GridView>

                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <table style="padding: 0px; margin: 0px; border: none;">
                                                                                    <tr style="padding: 0px; margin: 0px; border: none;">
                                                                                        <td style="padding: 0px; margin: 0px; border: none;">
                                                                                            <asp:FileUpload ID="grdfile_upload" class="blindInput" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" runat="server" Width="170" />
                                                                                        </td>
                                                                                        <td style="padding: 0px; margin: 0px; border: none;">
                                                                                            <asp:ImageButton ID="imgbtngrdUpload" Height="24" Width="24" CssClass="nostyleCalImg" ToolTip="Upload New Files" runat="server" ImageUrl="~/images/upload_imag.png" CommandArgument='<%#Eval("Order_id")%>' OnClientClick="ShowProgress();" OnClick="imgbtngrdUpload_Click" />

                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" />
                                                            </asp:TemplateField>
                                                            <%--cell=14--%>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>

                                                                    <asp:ImageButton ID="imgEdit" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_edit_16x16.png" ToolTip="Edit" CommandArgument='<%#Eval("Order_id")%>' OnClick="EditSelection" OnClientClick="ShowProgress();" />
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="3%" />
                                                                <ItemStyle HorizontalAlign="Center" Width="3%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgDelete" runat="server" CssClass="iconDeleteCss blindInput" ImageUrl="~/images/icon_delete_16x16.png" ToolTip="Delete" CommandArgument='<%#Eval("Order_id")%>' OnClick="DeleteSelection" OnClientClick="return confirmDelete();" />
                                                                </ItemTemplate>
                                                                <HeaderStyle HorizontalAlign="Center" Width="2%" />
                                                                <ItemStyle HorizontalAlign="Center" Width="2%" />
                                                            </asp:TemplateField>


                                                        </Columns>
                                                        <AlternatingRowStyle CssClass="alt" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>

                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:HiddenField ID="hdnCustomerID" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEstimateID" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnOrderId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">



                            <%--    <asp:Button ID="btnEmailSelection" Text="Email Selection(s) to the Customer" runat="server" CssClass="inputEmailBtn" data-action="save1" OnClick="btnEmailSelection_Click"/>
                <asp:Button ID="btnSelectionApproved" Text="Approve Selection" runat="server" CssClass="button" data-action="save1" OnClick="btnSubmitSelection_Click" />--%>
                        </td>
                    </tr>
                </table>
    </table>
</asp:Content>

