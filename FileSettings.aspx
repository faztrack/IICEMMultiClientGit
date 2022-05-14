<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="FileSettings.aspx.cs" Inherits="FileSettings" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="javascript" type="text/javascript">
        function selected_CustomerID(sender, e) {
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


            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Document File check</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="0" cellspacing="0" width="85%">
                            <tr>
                                <td>

                                    <table>
                                        <tr>
                                            <td>Customer</td>
                                            <td>
                                                <asp:TextBox ID="txtCustomerID" runat="server" onkeypress="return searchKeyPress(event);" Width="250px"></asp:TextBox>
                                                 <cc1:AutoCompleteExtender ID="txtCustomerID_AutoCompleteExtender" runat="server" CompletionInterval="500" CompletionListCssClass="AutoExtender" CompletionSetCount="10" DelimiterCharacters="" EnableCaching="true" Enabled="True" MinimumPrefixLength="1" OnClientItemSelected="selected_CustomerID" ServiceMethod="GetLastName" TargetControlID="txtCustomerID" UseContextKey="True">
                                                </cc1:AutoCompleteExtender>
                                                <cc1:TextBoxWatermarkExtender ID="wtmFileNumber" runat="server" TargetControlID="txtCustomerID" WatermarkText="Search by CustomerID" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Estimate</td>
                                            <td>
                                                <asp:TextBox ID="txtEstimateID" runat="server" onkeypress="return searchKeyPress(event);"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
                                        </tr>
                                    </table>

                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                                                <asp:HyperLink ID="hyp_ToolSettings" Target="_blank" runat="server" NavigateUrl="~/ToolSettings.aspx"></asp:HyperLink>
                                            </td>
                                        </tr>
                                    </table>

                                    <table style="text-align: left; width: 60%;" class="mGrid">
                                        <tr>
                                            <td valign="top" colspan="2">
                                                <asp:GridView ID="grdCustomer" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>



                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" colspan="2">
                                                <asp:HyperLink ID="hyp_DocumentManagement" Target="_blank" runat="server" Font-Bold="true" Style="text-align: right;"></asp:HyperLink></td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck1" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount1" Style="font-weight: bold; color: #cdcdcd;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck1" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck9" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount9" Style="font-weight: bold; color: #cdcdcd;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck9" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck10" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount10" Style="font-weight: bold; color: #cdcdcd;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck10" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck2" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount2" Style="font-weight: bold; color: #cdcdcd;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck2" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck3" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount3" Style="font-weight: bold; color: #cdcdcd;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck3" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck4" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount4" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck4" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck5" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount5" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck5" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck6" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount6" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck6" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck7" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount7" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck7" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <asp:Label ID="lblFileCheck8" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblFileCheckCount8" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
                                                <asp:GridView ID="grdFileCheck8" runat="server" AutoGenerateColumns="true" Width="100%"
                                                    CssClass="mGrid">
                                                    <Columns>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="right" style="font-weight: bold; color: #000;">Total Categorize File(s)
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblTotalCategorizeFile" Style="font-weight: bold; color: #000;" runat="server"></asp:Label>
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
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1" DynamicLayout="False">
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

