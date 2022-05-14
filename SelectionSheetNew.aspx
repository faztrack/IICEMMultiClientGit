<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SelectionSheetNew.aspx.cs" Inherits="SelectionSheetNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="2" width="100%">
                <tr>
                    <td align="center">
                        <table style="width: auto; margin-top: 20px;" cellpadding="0" cellspacing="10">
                            <tr>
                                <td style="width: 200px;" align="right" valign="top"><b>Customer Name: </b></td>
                                <td style="width: auto;">
                                    <asp:Label ID="lblCustomerName" runat="server" Text=""> TestTest TestTe </asp:Label>
                                </td>
                                <td style="width: 200px;" align="right" valign="top"><b>Estimate Name:</b> </td>
                                <td style="width: auto;">
                                    <asp:Label ID="lblEstimateName" Font-Bold="true" runat="server"> Test Test Test Test Test Test Test Test </asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Menu ID="customerMenuTab" runat="server" Orientation="Horizontal" OnMenuItemClick="customerMenuTab_MenuItemClick">
                            <Items>
                                <asp:MenuItem Text="Cabinet Selection" Value="0" Selected="true" />
                                <asp:MenuItem Text="Bathroom Selection" Value="1" />
                                <asp:MenuItem Text="Kitchen Selection" Value="2" />
                                <asp:MenuItem Text="Kitchen Tile Selection" Value="3" />
                                <asp:MenuItem Text="Shower Tile Selection" Value="4" />
                                <asp:MenuItem Text="Tub Tile Selection" Value="5" />
                            </Items>
                            <StaticMenuStyle CssClass="csMenuItem" />
                        </asp:Menu>
                        <%--<div>
                <ul class="nav nav-tabs">
                    <li class="active"><a data-toggle="tab" href="#home">Customer Information</a></li>
                    <li><a data-toggle="tab" href="#menu1">Locations</a></li>
                    <li><a data-toggle="tab" href="#menu2">Item List</a></li>
                    <li><a data-toggle="tab" href="#menu3">Suppliers</a></li>
                </ul>
            </div>--%>
                        <asp:MultiView ID="multiViewMenuTab" runat="server" ActiveViewIndex="0">
                            <asp:View ID="View1" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel4" runat="server">
                                                <b>
                                                    <asp:Label ID="Cabinet" runat="server" Style="color: #172087;" Text="Cabinet Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="pnlCabinet" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnSave" runat="server" CssClass="button" OnClick="btnSave_Click" Text="Save" />
                                                            <asp:Button ID="btnAddItem" runat="server" CssClass="button" OnClick="btnAddItem_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdCabinetSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdCabinetSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table width="100%" cellpadding="5" cellspacing="5">
                                                                                <tr>
                                                                                    <td align="right" colspan="4">
                                                                                        <asp:LinkButton ID="lnkDelete" Visible="false" OnClick="lnkDelete_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="3">
                                                                                        <asp:TextBox ID="txtCabinetSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("CabinetSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblCabinetSheetName" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>&nbsp;
                                                                                    </td>
                                                                                    <td align="center" class="nGridHeader">Upper Wall Cabinets</td>
                                                                                    <td align="center" class="nGridHeader">Base Cabinets</td>
                                                                                    <td align="center" class="nGridHeader">Misc Cabinets</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Door Style</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallDoor" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallDoor") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallDoor" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseDoor" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseDoor") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseDoor" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscDoor" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscDoor") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscDoor" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Wood Species</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallWood" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallWood") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallWood" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseWood" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseWood") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseWood" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscWood" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscWood") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscWood" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Stain Color</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallStain" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallStain") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallStain" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseStain" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseStain") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseStain" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscStain" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscStain") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscStain" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Exterior Sheen</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallExterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallExterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallExterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseExterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseExterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseExterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscExterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscExterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscExterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Interior Color</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallInterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallInterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallInterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseInterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseInterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseInterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscInterior" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscInterior") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscInterior" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="font-weight: bold;">Other</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtUpperWallOther" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("UpperWallOther") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblUpperWallOther" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtBaseOther" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseOther") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBaseOther" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtMiscOther" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("MiscOther") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblMiscOther" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View2" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel7" runat="server">
                                                <b>
                                                    <asp:Label ID="Label1" runat="server" Style="color: #172087;" Text="Bathroom Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="pnlBathroom" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblBathroomResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnSaveBathroom" runat="server" CssClass="button" OnClick="btnSaveBathroom_Click" Text="Save" />
                                                            <asp:Button ID="btnAddBathItem" runat="server" CssClass="button" OnClick="btnAddBathItem_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdBathroomSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdBathroomSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; width: 100%">
                                                                                <tr>
                                                                                    <td align="right" colspan="6">
                                                                                        <asp:LinkButton ID="lnkDeleteBath" Visible="false" OnClick="lnkDeleteBath_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="5">
                                                                                        <asp:TextBox ID="txtBathSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("BathroomSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblBathSheetName" runat="server"></asp:Label>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" width="120px">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Style</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Where To Order</td>

                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSinkQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink Faucet: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSinkFaucentQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkFaucentStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkFaucentOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink Drain: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSinkDrainQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkDrainStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkdrainOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink Valve: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSinkValveQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Valve_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkValveStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Valve_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSinkValveOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Valve_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>


                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Bathtub: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBathTubQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Bathtub_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBathTubStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Bathtub_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBathTubOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Bathtub_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tub Faucet: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTubFaucentQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Faucet_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubFaucentStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Faucet_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubFaucentOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Faucet_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tub Valve: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTubValveQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Valve_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubValveStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Valve_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubValveOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Valve_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tub Drain: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTubDrainQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Drain_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubDrainStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Drain_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubDrainOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Drain_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Toilet: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtToiletQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tollet_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtToiletStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tollet_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtToiletOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tollet_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower/Tub System: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShower_TubSystemQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_TubSystem_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShower_TubSystemStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_TubSystem_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShower_TubSystemOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_TubSystem_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Valve: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerValveQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Value_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerValveStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Value_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerValveOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Value_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Handheld Shower: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtHandheldShowerQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Handheld_Shower_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtHandheldShowerStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Handheld_Shower_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtHandheldShowerOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Handheld_Shower_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Body Spray: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBodySprayQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBodySprayStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBodySprayOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Body Spray Valve: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBodySprayValveQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_Valve_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBodySprayValveStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_Valve_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBodySprayValveOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Body_Spray_Valve_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Drain: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerDrainQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Drain Body & Plug: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerDrainBody_PlugQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Body_Plug_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainBody_PlugStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Body_Plug_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainBody_PlugOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Body_Plug_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Drain Cover: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerDrainCoverQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Cover_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainCoverStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Cover_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerDrainCoverOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Drain_Cover_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtCounterTopQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTopStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTopOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Additional places getting countertop: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtAdditionalplacesgettingcountertopQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtAdditionalplacesgettingcountertopStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtAdditionalplacesgettingcountertopOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Granite/Quartz Backsplash: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtGranite_Quartz_BacksplashQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGranite_Quartz_BacksplashStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGranite_Quartz_BacksplashOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top Edge: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtCounterTopEdgeQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTopEdgeStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTopEdgeOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top Overhang: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtCounterTop_OverhangQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTop_OverhangStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCounterTop_OverhangOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tub wall tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTubwalltileQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Wall_Tile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubwalltileStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Wall_Tile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubwalltileOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_Wall_Tile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile layout: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallTilelayoutQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWallTilelayoutStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWallTilelayoutOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tub skirt tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTubskirttileQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_skirt_tile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubskirttileStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_skirt_tile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTubskirttileOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tub_skirt_tile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Wall Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerWallTileQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Wall_Tile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerWallTileStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Wall_Tile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerWallTileOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Wall_Tile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile layout: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWall_Tile_layoutQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWall_Tile_layoutStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWall_Tile_layoutOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wall_Tile_Layout_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Floor Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerFloorTileQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Floor_Tile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerFloorTileStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Floor_Tile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerFloorTileOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Floor_Tile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower/Tub Tile Height: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerTubTileHeightQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Tub_Tile_Height_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerTubTileHeightStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Tub_Tile_Height_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtShowerTubTileHeightOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Shower_Tub_Tile_Height_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorTiletQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtFloorTiletstyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtFloorTiletOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile layout : </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorTilelayoutQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_layout_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtFloorTilelayoutStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_layout_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtFloorTilelayoutOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Floor_Tile_layout_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Bullnose Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBullnoseTileQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BullnoseTile_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBullnoseTileStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BullnoseTile_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtBullnoseTileOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BullnoseTile_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDecobandStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDecobandOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband Height: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandHeightQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_Height_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDecobandHeightStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_Height_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDecobandHeightOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Deco_Band_Height_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tile Baseboard: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTileBaseboardQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tile_Baseboard_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTileBaseboardStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tile_Baseboard_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTileBaseboardOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Tile_Baseboard_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Grout Selection: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtGroutSelectionQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grout_Selection_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGroutSelectionStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grout_Selection_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGroutSelectionOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grout_Selection_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Location: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheLocationQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Location_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtNicheLocationStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Location_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtNicheLocationOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Location_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Size: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheSizeQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Size_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtNicheSizeStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Size_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtNicheSizeOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Niche_Size_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Glass: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtGlassQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Glass_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGlassStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Glass_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGlassOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Glass_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Window: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWindowQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWindowStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWindowOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Door: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDoorQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDoorStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtDoorOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Grab Bar: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtGrabBarQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grab_Bar_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGrabBarStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grab_Bar_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtGrabBarOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Grab_Bar_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Cabinet Door Style and Color: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtCabinetDoorStyleColorQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinet_Door_Style_Color_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCabinetDoorStyleColorStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinet_Door_Style_Color_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtCabinetDoorStyleColorOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinet_Door_Style_Color_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Medicine Cabinet: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtMedicineCabinetQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Medicine_Cabinet_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtMedicineCabinetStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Medicine_Cabinet_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtMedicineCabinetOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Medicine_Cabinet_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Mirror: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtMirrorQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Mirror_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtMirrorStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Mirror_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtMirrorOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Mirror_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wood Baseboard: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWoodBaseboardQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wood_Baseboard_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWoodBaseboardStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wood_Baseboard_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtWoodBaseboardOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Wood_Baseboard_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Paint Color: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtPaintColorQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Paint_Color_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtPaintColorStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Paint_Color_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtPaintColorOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Paint_Color_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Lighting: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtLightingQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtLightingStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtLightingOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Hardware: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtHardwareQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtHardwareStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtHardwareOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Towel Ring: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTowelRingQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelRing_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTowelRingStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelRing_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTowelRingOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelRing_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Towel Bar: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTowelBarQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelBar_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTowelBarStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelBar_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTowelBarOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TowelBar_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tissue Holder: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtTissueHolderQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TissueHolder_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTissueHolderStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TissueHolder_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtTissueHolderOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TissueHolder_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">Series</td>
                                                                                    <td class="nGridHeader" align="center">Opening Size</td>
                                                                                    <td class="nGridHeader" align="center">Number of Panels</td>
                                                                                    <td class="nGridHeader" align="center">Finish</td>
                                                                                    <td class="nGridHeader" align="center">Insert</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Closet Door: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtClosetDoorSeries" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ClosetDoorSeries") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtClosetDoorOpeningSize" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ClosetDoorOpeningSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtClosetDoorNumberOfPanels" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ClosetDoorNumberOfPanels") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtClosetDoorFinish" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ClosetDoorFinish") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtClosetDoorInsert" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ClosetDoorInsert") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Special Notes: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtBathpecialNotes" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Special_Notes") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View3" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel5" runat="server">
                                                <b>
                                                    <asp:Label ID="Label2" runat="server" Style="color: #172087;" Text="Kitchen Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="Panel8" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblKitchenResult2" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnSaveKitchen" runat="server" CssClass="button" OnClick="btnSaveKitchen_Click" TabIndex="2" Text="Save Kitchen Sheet" />
                                                            <asp:Button ID="btnAddItemKitchen" runat="server" CssClass="button" OnClick="btnAddItemKitchen_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdKitchenSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdKitchenSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; width: 100%">
                                                                                <tr>
                                                                                    <td align="right" colspan="4">
                                                                                        <asp:LinkButton ID="lnkDeleteKitchen" Visible="false" OnClick="lnkDeleteKitchen_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="3">
                                                                                        <asp:TextBox ID="txtKitchenSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("KitchenSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblKitchenSheetName" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" width="120px">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Where To Order</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink Faucet: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkFaucetQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkFaucetStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkFaucetOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Fuacet_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Sink Drain: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkDrainQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkDrainStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenSinkDrainOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Sink_Drain_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Granite/Quartz Backsplash: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenGraniteQuartzBacksplashQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenGraniteQuartzBacksplashStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenGraniteQuartzBacksplashOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Granite_Quartz_Backsplash_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top Overhang: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopOverhangQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopOverhangStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopOverhangOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_Top_Overhang_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Additional places getting countertop: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenAdditionalplacesgettingcountertopQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenAdditionalplacesgettingcountertopStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenAdditionalplacesgettingcountertopOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("AdditionalPlacesGettingCountertop_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Counter Top Edge: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopEdgeQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopEdgeStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCounterTopEdgeOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Counter_To_Edge_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Cabinets: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCabinetsQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinets_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCabinetsStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinets_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenCabinetsOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Cabinets_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Disposal: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDisposalQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Disposal_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDisposalStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Disposal_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDisposalOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Disposal_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Baseboard: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenBaseboardQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Baseboard_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenBaseboardStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Baseboard_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenBaseboardOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Baseboard_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Windows: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenWindowsQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenWindowsStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenWindowsOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Window_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Doors: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDoorsQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDoorsStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenDoorsOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Door_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Lighting: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenLightingQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenLightingStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenLightingOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Lighting_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Hardware: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenHardwareQty" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_Qty") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenHardwareStyle" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_Style") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtKitchenHardwareOrder" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Hardware_WhereToOrder") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Special Notes: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtKitchenSpecialNotes" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("Special_Notes") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="center" colspan="7">
                                                                                        <asp:Label ID="lblKitchen2" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>

                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View4" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel6" runat="server">
                                                <b>
                                                    <asp:Label ID="Label3" runat="server" Style="color: #172087;" Text="Kitchen Tile Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="Panel9" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblKitchenTileResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnKitchenTileSave" runat="server" CssClass="button" OnClick="btnKitchenTileSave_Click" Text="Save" />
                                                            <asp:Button ID="btnAddKitchenTileItem" runat="server" CssClass="button" OnClick="btnAddKitchenTileItem_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdKitchenTileSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdKitchenTileSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; width: 100%">
                                                                                <tr>
                                                                                    <td align="right" colspan="7">
                                                                                        <asp:LinkButton ID="lnkKitchenTileDelete" Visible="false" OnClick="lnkKitchenTileDelete_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txKitchenTileSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("KitchenTileSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblKitchenTileSheetName" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="border: 0;" align="right"><b>Backsplash Tile: </b></td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBacksplashVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Backsplash Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtBacksplashPattern1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashPattern") %>'></asp:TextBox>

                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Backsplash Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtBacksplashGroutColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BacksplashGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Backsplash Bullnose: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseSize") %>'></asp:TextBox>

                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBBullnoseVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BBullnoseVendor") %>'></asp:TextBox>

                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center"># Of Sticks</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Profile</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Thickness</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right"><b>Schluter: </b></td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterNOSticks1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterNOSticks") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterProfile1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterProfile") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterThickness1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterThickness") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorPattern1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorPattern") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Direction: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorDirection1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorDirection") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Baseboard Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorGroutColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorGroutColor") %>'></asp:TextBox>

                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="center" colspan="7">
                                                                                        <asp:Label ID="lblKitchen" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>

                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View5" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel3" runat="server">
                                                <b>
                                                    <asp:Label ID="Label5" runat="server" Style="color: #172087;" Text="Shower Tile Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="Panel11" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblShowerTileResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnShowerTileSave" runat="server" CssClass="button" OnClick="btnShowerTileSave_Click" Text="Save" />
                                                            <asp:Button ID="btnAddShowerTileItem" runat="server" CssClass="button" OnClick="btnAddShowerTileItem_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdShowerTileSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdShowerTileSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; width: 100%">
                                                                                <tr>
                                                                                    <td align="right" colspan="7">
                                                                                        <asp:LinkButton ID="lnkShowerTileDelete" Visible="false" OnClick="lnkShowerTileDelete_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txShowerTileSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("ShowerTileSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblShowerTileSheetName" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtWallPattern1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTilePattern") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtWallGroutColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Wall Bullnose: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSWBullnoseVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseSize") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center"># Of Sticks</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Profile</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Thickness</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Schluter: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterNOSticks2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterColor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterProfile2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterProfile") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterThicknes2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterThickness") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Pan: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtShowerPanVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shower Pan Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtShowerPanGroutColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShowerPanGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband : </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandMOU") %>'></asp:TextBox>
                                                                                    </td>

                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandStyle") %>'></asp:TextBox>
                                                                                    </td>

                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandColor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandSize") %>'></asp:TextBox>
                                                                                    </td>

                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband Height: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtDecobandHeight1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandHeight") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtNicheLocation1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheLocation") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Size: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtNicheSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Bench Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileQTY1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileMOU1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileStyle1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileColor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBenchTileVendor1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchTileVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Bench Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtBenchLocation1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchLocation") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Bench Size: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtBenchSize1" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BenchSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorQTY2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorMOU2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorStyle2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorColor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorSize2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorVendor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorPattern2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorPattern") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Direction: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorDirection2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorDirection") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Baseboard Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardQTY2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardMOU2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardStyle2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardColor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardSize2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardVendor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardVendor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorGroutColor2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tile to: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtTileto2" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TileTo") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="View6" runat="server">
                                <table cellpadding="0" cellspacing="4" width="100%">
                                    <tr>
                                        <td class="tabTitle" align="center">
                                            <asp:Panel ID="Panel1" runat="server">
                                                <b>
                                                    <asp:Label ID="Label4" runat="server" Style="color: #172087;" Text="Tub Tile Selection" /></b>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Panel ID="Panel10" runat="server" Width="100%">
                                                <table width="100%">
                                                    <tr>
                                                        <td abbr="center" colspan="2">
                                                            <asp:Label ID="lblTubTileResult" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Button ID="btnTubTileSave" runat="server" CssClass="button" OnClick="btnTubTileSave_Click" Text="Save" />
                                                            <asp:Button ID="btnAddTubTileItem" runat="server" CssClass="button" OnClick="btnAddTubTileItem_Click" Text="Add New Item" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="grdTubTileSelectionSheet" runat="server" AllowPaging="True" AutoGenerateColumns="False" ShowHeader="False"
                                                                PageSize="20" Width="100%" CssClass="selectionGrid" OnRowDataBound="grdTubTileSelectionSheet_RowDataBound">
                                                                <PagerSettings Position="TopAndBottom" />
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <table style="padding: 0px; margin: 0px; width: 100%">
                                                                                <tr>
                                                                                    <td align="right" colspan="7">
                                                                                        <asp:LinkButton ID="lnkTubTileDelete" Visible="false" OnClick="lnkTubTileDelete_Click" OnClientClick="return confirmDelete();" runat="server" Text="Delete"></asp:LinkButton></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Title/Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txTubTileSheetName" runat="server" Style="width: 100%; margin-left: 0;" Text='<%# Eval("TubTileSheetName") %>'></asp:TextBox>
                                                                                        <asp:Label ID="lblTubTileSheetName" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWallVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileVendor") %>'></asp:TextBox>
                                                                                    </td>


                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtWallPattern3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTilePattern") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtWallGroutColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WallTileGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Wall Bullnose: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtWBullnoseVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("WBullnoseVendor") %>'></asp:TextBox>
                                                                                    </td>


                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center"># Of Sticks</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Profile</td>
                                                                                    <td class="nGridHeader" align="center" colspan="2">Thickness</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Schluter: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterNOSticks3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterNOSticks") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtSchluterColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterProfile3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterProfile") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left" colspan="2">
                                                                                        <asp:TextBox ID="txtSchluterThicknes3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("SchluterThickness") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">&nbsp;</td>
                                                                                    <td class="nGridHeader" align="center">QTY</td>
                                                                                    <td class="nGridHeader" align="center">UoM</td>
                                                                                    <td class="nGridHeader" align="center">Style</td>
                                                                                    <td class="nGridHeader" align="center">Color</td>
                                                                                    <td class="nGridHeader" align="center">Size</td>
                                                                                    <td class="nGridHeader" align="center">Vendor</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband : </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtDecobandVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Decoband Height: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtDecobandHeight3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("DecobandHeight") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtNicheTileVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheTileVendor") %>'></asp:TextBox>
                                                                                    </td>



                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtNicheLocation3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheLocation") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Niche Size: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtNicheSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("NicheSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Shelf Location: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtShelfLocation3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("ShelfLocation") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtFloorVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorVendor") %>'></asp:TextBox>
                                                                                    </td>


                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Pattern: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorPattern3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorPattern") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Tile Direction: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorDirection3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorDirection") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Baseboard Tile: </b>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardQTY3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardQTY") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardMOU3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardMOU") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardStyle3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardStyle") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardSize3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardSize") %>'></asp:TextBox>
                                                                                    </td>
                                                                                    <td align="left">
                                                                                        <asp:TextBox ID="txtBaseboardVendor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("BaseboardVendor") %>'></asp:TextBox>
                                                                                    </td>

                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Floor Grout Color: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtFloorGroutColor3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("FloorGroutColor") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right">
                                                                                        <b>Tile to: </b>
                                                                                    </td>
                                                                                    <td align="left" colspan="6">
                                                                                        <asp:TextBox ID="txtTileto3" Style="width: 100%; margin-left: 0;" runat="server" Text='<%# Eval("TileTo") %>'></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                                                <AlternatingRowStyle CssClass="alt" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>

                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnCabinetSheetID" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnBathroomSheetID" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnKitchenSheetID" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnKitchenTileSheetID" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnTubTileSheetID" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnShowerTileSheetID" runat="server" Value="0" />

                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
        <ProgressTemplate>
            <div class="overlay" />
            <div class="overlayContent">
                <p>Please wait while your data is being processed</p>
                <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
