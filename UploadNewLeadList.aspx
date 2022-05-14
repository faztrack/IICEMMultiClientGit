<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="UploadNewLeadList.aspx.cs" Inherits="UploadNewLeadList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">


    <table cellpadding="0" cellspacing="0" width="100%">

        <tr>
            <td align="center" style="background-color: #ddd; color: #fff;">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left" class="titleNu"><span class="titleNu">Upload New Lead List</span></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">

                <table>
                    <tr>
                        <td>
                            <asp:FileUpload ID="ExcelUploader" runat="server" accept=".xls,.xlsx"></asp:FileUpload>
                            <asp:Button ID="btnUpload" runat="server" CssClass="button" Text="Upload" OnClick="btnUpload_Click" OnClientClick="ShowProgress();" />
                            <br />
                            &nbsp;&nbsp;&nbsp;(Upload Excel file Only)
                        </td>
                        <td style="vertical-align:middle;">&nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="lnkSampleExcelFile" runat="server" Text="Download a sample file" OnClick="lnkSampleExcelFile_Click" ></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkDummy2" runat="server"></asp:LinkButton>
                            <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>
                            <cc1:ModalPopupExtender ID="ModalPopupExtender2" BehaviorID="mpe" runat="server"
                                PopupControlID="pnlPopup" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground" CancelControlID="lnkDummy3">
                            </cc1:ModalPopupExtender>
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
    </table>
    <div id="LoadingProgress" style="display: none">
        <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>
    <asp:Panel ID="pnlPopup" runat="server" Style="width: auto; overflow: scroll; height: 900px;" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <asp:Panel ID="pnlExcelImportMatch" runat="server">
                    <table cellpadding="0" cellspacing="2" width="100%" align="center" style="">
                        <tr>
                            <td align="center" style="background-color: #ddd; color: #fff;">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" class="titleNu"><span class="titleNu">Import Lead List</span></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td align="right"><b>Sales Person:</b> </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlSalesPersonPopUp" runat="server" TabIndex="13" Width="200px">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right"><span class="required">&nbsp;&nbsp;&nbsp;*</span><b>Lead Source:</b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlLeadSourcePopUp" runat="server" TabIndex="14" Width="210px" OnSelectedIndexChanged="ddlLeadSourcePopUp_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="right"><span class="required">&nbsp;&nbsp;&nbsp;*</span><b>Lead Status: </b></td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlLeadStatusPopUp" runat="server" TabIndex="15" Width="212px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td valign="top">
                                            <div id="divLeadSource" runat="server" visible="false">
                                                <span class="required">* </span>Lead Source Name:<br />
                                                <asp:TextBox ID="txtLeadName" runat="server" Width="200px" TabIndex="14"></asp:TextBox><br />
                                                <asp:Button ID="btnSaveLeadSource" runat="server" Text="Save" TabIndex="14" OnClick="btnSaveLeadSource_Click" CssClass="button" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label ID="lblResult2" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="lldd" align="center">
                                <table cellpadding="0" cellspacing="10">
                                    <tr>
                                        <td class="llddtd" valign="top" width="50%" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn0" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName0" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData0" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn1" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName1" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData1" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn2" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName2" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData2" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn3" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName3" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData3" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn4" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName4" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData4" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn5" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName5" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData5" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn6" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName6" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData6" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn7" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName7" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData7" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn8" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName8" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData8" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn9" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName9" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData9" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn10" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName10" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData10" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn11" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName11" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData11" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn12" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName12" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData12" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn13" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName13" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData13" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn14" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName14" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData14" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn15" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName15" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData15" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="llddtd" valign="top" align="right">
                                            <asp:DropDownList ID="ddlLeadColumn16" runat="server" OnSelectedIndexChanged="ddlLeadColumn_SelectedIndexChanged" AutoPostBack="false">
                                                <asp:ListItem Text="Do Not Import" Value="111" Selected="True"></asp:ListItem>
                                                <asp:ListItem Text="First Name 1" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="First Name 2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Last Name 2" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Company" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Email 1" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Email 2" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="Address" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="Cross Street" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="City" Value="9"></asp:ListItem>
                                                <asp:ListItem Text="State" Value="10"></asp:ListItem>
                                                <asp:ListItem Text="ZIP Code" Value="11"></asp:ListItem>
                                                <asp:ListItem Text="Phone" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="Mobile" Value="13"></asp:ListItem>
                                                <asp:ListItem Text="Fax" Value="14"></asp:ListItem>
                                                <asp:ListItem Text="Notes" Value="15"></asp:ListItem>
                                                <asp:ListItem Text="Website" Value="16"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <table class="llddtdch" style="padding: 0px; margin: 0px; width: 100%;">
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnName16" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                <tr style="padding: 0px; margin: 0px;">
                                                    <td style="margin: 0px;">
                                                        <asp:Label ID="lblLeadColumnData16" runat="server"></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>

                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label ID="lblResultPopUp" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnImport" runat="server" CssClass="button" Text="Import" OnClick="btnImport_Click" CausesValidation="false" />&nbsp;
                                <asp:Button ID="btnHide" runat="server" Text="Close" CssClass="button" Width="60px" OnClick="btnClose_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlExcelImportedDataList" runat="server" Visible="false">
                    <table cellpadding="0" cellspacing="2" width="100%" align="center" style="">
                        <tr>
                            <td align="center" style="background-color: #ddd; color: #fff;">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" class="titleNu"><span class="titleNu">Import Lead List</span></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblLeadCount" Style="font-weight: bold; padding: 10px;" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView ID="gridMatchLeadColumn" runat="server" AllowPaging="false" AutoGenerateColumns="False" CssClass="mGrid" Width="100%">
                                    <PagerSettings Position="TopAndBottom" />
                                    <Columns>
                                        <asp:BoundField DataField="first_name1" HeaderText="First Name 1" ItemStyle-Width="5%" />
                                        <asp:BoundField DataField="last_name1" HeaderText="Last Name 1" ItemStyle-Width="5%" />
                                        <asp:BoundField DataField="first_name2" HeaderText="First Name 2" ItemStyle-Width="5%" />
                                        <asp:BoundField DataField="last_name2" HeaderText="Last Name 2" ItemStyle-Width="5%" />
                                        <asp:BoundField DataField="company" HeaderText="Company" ItemStyle-Width="9%" />
                                        <asp:BoundField DataField="address" HeaderText="Address" ItemStyle-Width="9%" />
                                        <asp:BoundField DataField="cross_street" HeaderText="Cross Street" ItemStyle-Width="4%" />
                                        <asp:BoundField DataField="city" HeaderText="City" ItemStyle-Width="6%" />
                                        <asp:BoundField DataField="state" HeaderText="State" ItemStyle-Width="3%" />
                                        <asp:BoundField DataField="zip_code" HeaderText="ZIP Code" ItemStyle-Width="4%" />
                                        <asp:BoundField DataField="email" HeaderText="Email" ItemStyle-Width="7%" />
                                        <asp:BoundField DataField="email2" HeaderText="Email 2" ItemStyle-Width="7%" />
                                        <asp:BoundField DataField="website" HeaderText="Website" ItemStyle-Width="8%" />
                                        <asp:BoundField DataField="phone" HeaderText="Phone" ItemStyle-Width="6%" />
                                        <asp:BoundField DataField="mobile" HeaderText="Mobile" ItemStyle-Width="6%" />
                                        <asp:BoundField DataField="fax" HeaderText="Fax" ItemStyle-Width="2%" />
                                        <asp:BoundField DataField="notes" HeaderText="Notes" ItemStyle-Width="8%" />
                                    </Columns>
                                    <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="alt" />
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnSave" runat="server" CssClass="button" Text="Save" OnClick="btnSave_Click" CausesValidation="false" Visible="false" />&nbsp;    
                                <asp:Button ID="btnBack" runat="server" CssClass="button" Text="Back" OnClick="btnBack_Click" CausesValidation="false" Visible="false" />&nbsp;                           
                                <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="button" Width="60px" OnClick="btnClose_Click" />
                                <asp:HiddenField ID="hdnLeadId" runat="server" Value="0" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel5" DynamicLayout="False">
            <ProgressTemplate>
                <div class="overlay" />
                <div class="overlayContent">
                    <p>
                        Please wait while your data is being processed
                    </p>
                    <img src="Images/ajax_loader.gif" alt="Loading" border="1" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </asp:Panel>

   
</asp:Content>

