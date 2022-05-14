<%@ Page Title="Customer Dashboard" Language="C#" MasterPageFile="~/CustomerMain.master" AutoEventWireup="true" CodeFile="customerdashboard.aspx.cs" Inherits="customerdashboard" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="JavaScript" type="text/JavaScript">
        function DisplayWindow1() {
            window.open('design_graphics.aspx?type=1&jsid=1&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow2() {
            window.open('jobdesc_popup.aspx?jsid=2&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow3() {
            //window.open('jobdesc_popup.aspx?jsid=3&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
            window.open('SiteProgress_graphics.aspx?type=3&jsid=3&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow4() {
            window.location.href = 'customerschedulecalendar.aspx?TypeID=1&cid=' + document.getElementById('head_hdnCustomerId').value + '&eid=' + document.getElementById('head_hdnEstimateId').value;
            //window.open('jobdesc_popup.aspx?jsid=4&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
           // window.open('customerschedulecalendar.aspx?TypeID=1&cid=' + document.getElementById('head_hdnCustomerId').value + '&eid=' + document.getElementById('head_hdnEstimateId').value, 'MyWindow', 'left=150,top=100,width=1050,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow6() {
            alert("The schedule for your project is still pending. Please check back at a later time.");
        }
        function DisplayWindow5() {
            window.open('jobdesc_popup.aspx?jsid=5&eid=' + document.getElementById('head_hdnEstimateId').value + '&cid=' + document.getElementById('head_hdnCustomerId').value, 'MyWindow', 'left=150,top=100,width=850,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function GetdatakeyValue1Old(value) {
            window.open('messagedetails.aspx?custId=' + document.getElementById('head_hdnCustomerId').value + '&MessId=' + value, '_blank', 'left=200,top=100,width=900,height=700,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

        function check_radio(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "radio" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = false;
                    }
                }
            }
        }




    </script>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="wrapper" cellpadding="5px" cellspacing="5px" width="100%">
                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">
                                    <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Dashboard</asp:Label></span></td>
                                <td align="right"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table align="center" cellpadding="0" cellspacing="2" width="100%">
                            <tr>
                                <td align="left" width="30%">
                                    <table align="center" cellpadding="0" cellspacing="2" width="98%">
                                        <tr>
                                            <td align="right">
                                                <b>Customer Name: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="right" valign="top">
                                                <b>Address: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Email: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <b>Phone: </b>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" width="70%">
                                    <asp:GridView ID="grdEstimates" runat="server" AutoGenerateColumns="False"
                                        BorderStyle="None" CssClass="mGrid" OnRowCommand="grdEstimates_RowCommand"
                                        OnRowDataBound="grdEstimates_RowDataBound" ShowHeader="False" TabIndex="2"
                                        Width="60%">
                                        <Columns>
                                            <asp:ButtonField CommandName="view" DataTextField="estimate_name" />
                                            <asp:BoundField />
                                        </Columns>
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>

                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <h2>Current Project:
                        <asp:Label ID="lblEstimateName" Style="color: inherit; font-size: inherit;" runat="server"></asp:Label>
                            <asp:HyperLink ID="hypComposite" runat="server"  style="display:none;">Composite SOW</asp:HyperLink>
                        </h2>
                    </td>
                </tr>
                <tr id="trJobStatus" runat="server">
                    <td align="center">
                        <table cellpadding="2px" cellspacing="2px" width="100%">
                            <tr>
                                <td align="center">
                                    <h2>Process Progress </h2>
                                    <span style="font-family: Arial; font-size: 14px; font-weight: bold;">Please click on any of the blocks below to view details</span>
                                    <table cellpadding="2px" cellspacing="2px">

                                        <tr>
                                            <td align="center">
                                                <asp:Image ID="imgA" runat="server" ImageUrl="~/JobImages/WhiteA.jpg"
                                                    ToolTip="Click here to view Design descriptions" />
                                            </td>
                                            <td align="center">
                                                <asp:Image ID="imgB" runat="server" ImageUrl="~/JobImages/WhiteB.jpg"
                                                    ToolTip="Click here to view Selection descriptions" />
                                            </td>
                                            <td align="center">
                                                <asp:Image ID="imgC" runat="server" ImageUrl="~/JobImages/WhiteC.jpg"
                                                    ToolTip="Click here to view Site Progress & Photos" />
                                            </td>
                                            <td align="center">
                                                <asp:Image ID="imgD" runat="server" ImageUrl="~/JobImages/WhiteD.jpg"
                                                    ToolTip="Click here to view Schedule descriptions" />
                                            </td>
                                            <td align="center">
                                                <asp:Image ID="imgE" runat="server" ImageUrl="~/JobImages/WhiteE.jpg"
                                                    ToolTip="Click here to view Final Project Review descriptions" />
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="imgButtonF" runat="server"
                                                    ImageUrl="~/JobImages/WhiteF.jpg" OnClick="imgButtonF_Click" />
                                            </td>
                                            <td align="center">
                                                <asp:ImageButton ID="imgButtonG" runat="server"
                                                    ImageUrl="~/JobImages/WhiteG.jpg" OnClick="imgButtonG_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="6" style="font-family: Arial; font-size: 14px; font-weight: bold;"><span style="color: #f00;">*</span> Please enable browser pop-up for any of the blocks above</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px">
                        <asp:Label ID="lblDisMessage" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="center" height="10px">
                        <table id="tblMessage" runat="server" cellpadding="2" cellspacing="2" width="100%">
                            <tr>
                                <td align="left">
                                    <h2>My Messages </h2>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:GridView ID="grdCustomersMessage" runat="server"
                                        AutoGenerateColumns="False" CssClass="mGrid"
                                        OnRowDataBound="grdCustomersMessage_RowDataBound" Width="100%" PageSize="5" AllowPaging="True" OnPageIndexChanging="grdCustomersMessage_PageIndexChanging">
                                        <Columns>
                                            <asp:BoundField DataField="create_date" DataFormatString="{0:d}"
                                                HeaderText="Date" >
                                            <ItemStyle HorizontalAlign="center" />
                                            </asp:BoundField>
                                           <%-- <asp:BoundField DataField="Type" HeaderText="Type" />--%>
                                            <asp:BoundField DataField="From" HeaderText="From" />


                                            <asp:BoundField DataField="To" HeaderText="To" />
                                            <asp:BoundField DataField="mess_subject" HeaderText="Subject" />
                                            <asp:TemplateField HeaderText="Attachment">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypAttachment" runat="server" CssClass="hypg"></asp:HyperLink>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="sent_by" HeaderText="Sent by" />


                                           <%-- <asp:BoundField DataField="IsRead" HeaderText="Viewed?">
                                            <ItemStyle HorizontalAlign="center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="Protocol" HeaderText="Sent Via" />--%>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypMessageDetails" runat="server" CssClass="btn_details">Details</asp:HyperLink>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>


                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table id="tblChangeOrders" runat="server" cellpadding="2" cellspacing="2" width="100%">
                            <tr>
                                <td align="left">
                                    <h2>My Change Orders </h2>
                                </td>
                                <td align="left">
                                    <table cellpadding="2px" cellspacing="2px">
                                        <tr>
                                            <td align="right">Show
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlShowChangesOrders" runat="server" AutoPostBack="True"
                                                    OnSelectedIndexChanged="ddlShowChangesOrders_SelectedIndexChanged">
                                                    <asp:ListItem Selected="True" Value="15">15</asp:ListItem>
                                                    <asp:ListItem Value="25">25</asp:ListItem>
                                                    <asp:ListItem Value="50">50</asp:ListItem>
                                                    <asp:ListItem Value="4">All</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:GridView ID="grdChangeOrders" runat="server" AllowPaging="True"
                                        AutoGenerateColumns="False" Width="100%" CssClass="mGrid"
                                        OnRowDataBound="grdChangeOrders_RowDataBound"
                                        OnPageIndexChanging="grdChangeOrders_PageIndexChanging">
                                        <PagerSettings Position="TopAndBottom" />
                                        <Columns>
                                            <asp:BoundField DataField="chage_order_id" HeaderText="C/O No">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="create_date" DataFormatString="{0:d}"
                                                HeaderText="Date">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="changeorder_name" HeaderText="Change Order Title">
                                                <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="View C/O">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypViewCO" runat="server">View</asp:HyperLink>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="change_order_status_id" HeaderText="Status">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="By">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                            <asp:BoundField HeaderText="Last Viewed" DataField="lastviewed">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <PagerStyle CssClass="pgr" HorizontalAlign="Left" />
                                        <AlternatingRowStyle CssClass="alt" />
                                    </asp:GridView>
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <%-- <tr>
                    <td align="left">
                        <h2>My Payments </h2>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdPyement" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                            PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdPyement_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("pay_date","{0:d}")%>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                    <ItemStyle HorizontalAlign="Center" Width="115px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Term">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPayTerms" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPayType" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Info">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCardInfo" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reference">
                                    <ItemTemplate>
                                        <asp:Label ID="lblReference" runat="server" Text='<%# Eval("reference") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("pay_amount","{0:c}") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle CssClass="alt" />
                        </asp:GridView>
                    </td>
                </tr>--%>
                <tr>
                    <td align="left">
                        <h2>My Payment Terms</h2>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:GridView ID="grdPaymentTerm" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                            PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdPaymentTerm_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%# Eval("pay_term_date")%>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="115px" />
                                    <ItemStyle HorizontalAlign="Center" Width="115px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Term">
                                    <ItemTemplate>
                                        <asp:Label ID="lblPayTerms" runat="server" Text='<%# Eval("term_name")%>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="500px" />
                                    <ItemStyle HorizontalAlign="Left" Width="500px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Info">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCardInfo" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="25%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Amount">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("pay_term_amount","{0:c}") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Right" Width="140px" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label ID="lblPayInfo" runat="server" />
                                        <asp:LinkButton ID="lnkPayment" runat="server" OnClick="CreatePayment" ToolTip="Click here to make a payment"></asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="30%" />
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle CssClass="alt" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Label ID="lblCardResult" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <table cellpadding="2" cellspacing="2" width="100%" align="center" id="tblNewPayment"
                            runat="server" visible="false">
                            <tr>
                                <td><span style="color: #f00;">*</span> Required</td>
                                <td align="center"></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:RadioButtonList ID="rdbPaymentOption" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rdbPaymentOption_SelectedIndexChanged" Visible="False">
                                        <asp:ListItem Value="1" Text="Pay by Credit Card" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Pay by Bank Check"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 200px;"><b>Payment Term: </b></td>
                                <td align="left">&nbsp;&nbsp;<asp:Label ID="lblPayterm" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="right"><b><span style="color: #f00;">*</span> Amount: </b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtAmount" runat="server" Width="100px" AutoPostBack="True" OnTextChanged="txtAmount_TextChanged"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Panel ID="pnlExistCard" runat="server" Height="100%">
                                        <table cellpadding="2" cellspacing="2" width="100%">
                                            <tr>
                                                <td align="center">Existing Credit OR debit Cards</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:GridView ID="grdCardList" runat="server" CssClass="mGrid" AllowSorting="True" AutoGenerateColumns="False" Width="100%"
                                                        DataKeyNames="PaymentProfileId" OnSorting="grdCardList_Sorting" OnRowDataBound="grdCardList_RowDataBound">
                                                        <AlternatingRowStyle CssClass="alt" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Card Number" SortExpression="CardType">
                                                                <ItemTemplate>
                                                                    <asp:RadioButton ID="rdoSelect" runat="server" onclick="check_radio(this);" />
                                                                    <asp:Image ID="imggrdCardType" runat="server" CssClass="imgCard" />
                                                                    <asp:Label ID="lblgrdCreditCard" runat="server"></asp:Label>
                                                                    <asp:Label ID="lblCardMessage" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" Width="34%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Card Holder Name" SortExpression="NameOnCard">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblgrdNameOnCard" runat="server" Text='<%# Eval("NameOnCard") %>' />
                                                                    <asp:Label ID="lblNameMessage" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Left" Width="36%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Expiration Date" SortExpression="ExpirationDate">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblgrdExpirationDatee" runat="server" Text='<%# Eval("ExpirationDate") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" Width="12%" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <HeaderStyle CssClass="gridHeaderStyle" />
                                                        <PagerSettings Position="TopAndBottom" />
                                                        <PagerStyle HorizontalAlign="Left" CssClass="pagerStyle" />
                                                        <RowStyle CssClass="rowStyle" />
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>

                            <tr>
                                <td align="left" colspan="2">
                                    <table>
                                        <tr>

                                            <td align="left">
                                                <asp:CheckBox ID="chkNewCard" runat="server" AutoPostBack="True" Font-Bold="True" Text="Use new card to make a Payment" OnCheckedChanged="chkNewCard_CheckedChanged" />
                                            </td>
                                            <%--<td align="left">
                                                <asp:CheckBox ID="chkPaymentECheck" runat="server" AutoPostBack="True" Font-Bold="True" Text="Use E-Check Payment Option to make a Payment" OnCheckedChanged="chkPaymentECheck_CheckedChanged" />
                                            </td>--%>
                                        </tr>
                                    </table>
                                </td>

                            </tr>
                            <tr>
                                <td align="left" colspan="2">
                                    <asp:Panel ID="pnlNewCard" runat="server" Height="100%">
                                        <table cellpadding="2" cellspacing="2" width="100%">
                                            <tr>
                                                <td align="left"></td>
                                                <td align="left">
                                                    <asp:Label ID="Label12" runat="server" CssClass="col-md-8 control-label textAlignL">The following information is related to your credit card</asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 198px;" align="right"><b><span style="color: #f00;">*</span> Card Holder Name:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCardHolderName" runat="server" Width="200px" MaxLength="20"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> Card Holder Address:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Width="175px" Height="45px"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> City:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> State:</b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlState" runat="server" TabIndex="5">
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> Zip:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> Credit Card Number:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCreditCardNumber" runat="server" MaxLength="16" Width="220px" OnTextChanged="txtCreditCardNumber_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                    <asp:Image ID="imgCardType" runat="server" CssClass="imgCard" />
                                                    <asp:CheckBox ID="chkSaveCardInfo" runat="server" Text="Save this card for future use" Visible="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> Expiration Date:</b></td>
                                                <td align="left">
                                                    <table class="dropDownNewCenter">
                                                        <tr>
                                                            <td align="left">
                                                                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                                    <asp:ListItem Value="01">01</asp:ListItem>
                                                                    <asp:ListItem Value="02">02</asp:ListItem>
                                                                    <asp:ListItem Value="03">03</asp:ListItem>
                                                                    <asp:ListItem Value="04">04</asp:ListItem>
                                                                    <asp:ListItem Value="05">05</asp:ListItem>
                                                                    <asp:ListItem Value="06">06</asp:ListItem>
                                                                    <asp:ListItem Value="07">07</asp:ListItem>
                                                                    <asp:ListItem Value="08">08</asp:ListItem>
                                                                    <asp:ListItem Value="09">09</asp:ListItem>
                                                                    <asp:ListItem Value="10">10</asp:ListItem>
                                                                    <asp:ListItem Value="11">11</asp:ListItem>
                                                                    <asp:ListItem Value="12">12</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td align="left">
                                                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                                                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                                                    <asp:ListItem Value="16">2016</asp:ListItem>
                                                                    <asp:ListItem Value="17">2017</asp:ListItem>
                                                                    <asp:ListItem Value="18">2018</asp:ListItem>
                                                                    <asp:ListItem Value="19">2019</asp:ListItem>
                                                                    <asp:ListItem Value="20">2020</asp:ListItem>
                                                                    <asp:ListItem Value="21">2021</asp:ListItem>
                                                                    <asp:ListItem Value="22">2022</asp:ListItem>
                                                                    <asp:ListItem Value="23">2023</asp:ListItem>
                                                                    <asp:ListItem Value="24">2024</asp:ListItem>
                                                                    <asp:ListItem Value="25">2025</asp:ListItem>
                                                                    <asp:ListItem Value="26">2026</asp:ListItem>
                                                                    <asp:ListItem Value="27">2027</asp:ListItem>
                                                                    <asp:ListItem Value="28">2028</asp:ListItem>
                                                                    <asp:ListItem Value="29">2029</asp:ListItem>
                                                                    <asp:ListItem Value="30">2030</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right"><b><span style="color: #f00;">*</span> CVV Code:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" MaxLength="4"></asp:TextBox>
                                                    <asp:Label ID="Label11" runat="server" CssClass="col-md-8 control-label textAlignL">(4-Digit number on front of AMEX, 3-Digit number on back of all other cards.)</asp:Label>
                                                    <asp:CustomValidator ID="valCheckOut" runat="server" Visible="False" ForeColor="DarkRed" OnServerValidate="valCheckOut_ServerValidate"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>

                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="pnlECheckPayment" runat="server" Height="100%" Visible="false">
                                        <table cellpadding="2" cellspacing="2" width="100%" align="center" id="tblECheckPayment"
                                            runat="server">
                                            <tr>
                                                <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Account Holder Name:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtBank_acct_name" runat="server" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Account Number:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtBank_acct_num" runat="server" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Bank Name:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtbank_name" runat="server" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td align="right" style="width: 198px;"><b><span style="color: #f00;">*</span>Bank Routing Number:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtBank_aba_code" runat="server" Width="200px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>

                                </td>
                            </tr>


                            <tr>
                                <td style="width: 200px;" align="right"><b>Comments:</b></td>
                                <td align="left">
                                    <asp:TextBox ID="txtPayComments" runat="server" Height="44px" TextMode="MultiLine" Width="622px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Label ID="lblReason" runat="server"></asp:Label></td>

                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnEcheckPayment" runat="server" Text="Confirm Payment" OnClick="btnEcheckPayment_Click" CssClass="button" Visible="False" />
                                    <asp:Button ID="btnFinalizePayment" runat="server" CssClass="button" OnClick="btnFinalizePayment_Click" Text="Confirm Payment" />
                                    <asp:Button ID="btnDummy" runat="server" Text="Button" Style="display: none" />
                                    <%-- <asp:Button ID="btnSaveCardInfo" CssClass="hideElements" runat="server" OnClick="btnSaveCardInfo_Click" Width="1px" />--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">&nbsp;</td>
                </tr>
                <tr>
                    <td align="left">
                        <table style="width: 68%;" class="wrapm">
                            <tr>
                                <td align="center" colspan="2">
                                    <h2>Payment Information </h2>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>C/O Amount:</b></td>
                                <td>
                                    <asp:Label ID="lblTotalCOAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Contract Amount:</b></td>
                                <td>
                                    <asp:Label ID="lblProjectTotal" runat="server" Width="84px" Text="0"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Total Paid Amount:</b></td>
                                <td>
                                    <asp:Label ID="lblTotalRecievedAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td width="50%" align="right">
                                    <b>Total Amount (Contract+C/O):</b></td>
                                <td>
                                    <asp:Label ID="lblTotalAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <b>Balance Due:</b></td>
                                <td>
                                    <asp:Label ID="lblTotalBalanceAmount" runat="server" Text="0" Width="84px"
                                        CssClass="cssLblAmount"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table cellpadding="2" cellspacing="2" width="98%">
                            <tr>
                                <td align="left" colspan="2">
                                    <h2>Project Status</h2>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 160px;">Project Complete?
                                </td>
                                <td align="left" style="height: 26px">
                                    <asp:RadioButtonList ID="rdoProjectComplete" runat="server" AutoPostBack="True"
                                        RepeatDirection="Horizontal"
                                        OnSelectedIndexChanged="rdoProjectComplete_SelectedIndexChanged">
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="2">No</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="width: 168px">
                                    <asp:Label ID="lblProjectCompletion" runat="server"
                                        Text="Project Completion Date:" Visible="False"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:Label ID="lblProjectCompletionDate" runat="server" Visible="False"></asp:Label>
                                    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" colspan="2" height="15px"></td>
                            </tr>
                            <tr>
                                <td align="left" style="width: 168px"></td>
                                <td align="left">
                                    <asp:Panel ID="pnlSurvey" runat="server" Visible="false">
                                        <table cellpadding="0" cellpadding="0" width="100%">
                                            <tr>
                                                <td align="left">Q1. How would you rate the company on job site cleanliness?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer1" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q2.	Did the workers act courteously and polite?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer2" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q3.	Would you recommend us to your friends and family?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer3" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q4.	Why did you hire Arizona's Interior Innovations Kitchen & Bath Design originally?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer4" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q5.	Did you use our online tools and were they easy to use?                                                   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer5" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q6.	Are you happy with the quality of the workmanship?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer6" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q7.	Is there an area you would like us to improve?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer7" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Q8.	Would you use our company again in the future?
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">Answer:
                                                    <br />
                                                    <asp:TextBox ID="txtAnswer8" runat="server" TextMode="MultiLine" Width="800px"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" height="10px">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" height="10px">&nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <asp:Button ID="btnSaveAnswers" runat="server" CssClass="button"
                                                        Text="Save Answers" OnClick="btnSaveAnswers_Click" />
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
                    <td align="center">
                        <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnSalesPersonId" runat="server" EnableViewState="False"
                            Value="0" />
                        <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnOrder" runat="server" Value="ASC" />
                        <asp:HiddenField ID="hdnPayTermId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnPayId" runat="server" Value="0" />
                        <asp:HiddenField ID="hdnPayTerm" runat="server" Value="0" />

                        <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender1" TargetControlID="btnFinalizePayment"
                            OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender1" runat="server">
                        </cc1:ConfirmButtonExtender>
                        <cc1:ModalPopupExtender ID="ModalPopupExtender1" TargetControlID="btnFinalizePayment" BackgroundCssClass="modalBackground"
                            CancelControlID="btnCancel" OkControlID="btnOK" PopupControlID="pnlConfirmation"
                            runat="server">
                        </cc1:ModalPopupExtender>
                        <cc1:ConfirmButtonExtender ID="ConfirmButtonExtender2" TargetControlID="btnEcheckPayment"
                            OnClientCancel="cancelClick" DisplayModalPopupID="ModalPopupExtender2" runat="server">
                        </cc1:ConfirmButtonExtender>
                        <cc1:ModalPopupExtender ID="ModalPopupExtender2" TargetControlID="btnEcheckPayment" BackgroundCssClass="modalBackground"
                            CancelControlID="btnCancel1" OkControlID="btnOK1" PopupControlID="pnlConfirmation1"
                            runat="server">
                        </cc1:ModalPopupExtender>
                        <%-- <cc1:ModalPopupExtender ID="ModalPopupExtender3" runat="server" PopupControlID="Panel1" TargetControlID="lnkDummy" 
                            BackgroundCssClass="modalBackground" CancelControlID="btnHide">
                        </cc1:ModalPopupExtender>--%>
                        <cc1:ModalPopupExtender ID="ModalPopupExtender3" TargetControlID="btnDummy" BackgroundCssClass="modalBackground"
                            CancelControlID="btnHide" PopupControlID="pnlPopup"
                            runat="server">
                        </cc1:ModalPopupExtender>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <%-- <Triggers>
            <asp:PostBackTrigger ControlID="grdCardList" />
        </Triggers>--%>
    </asp:UpdatePanel>
    <asp:Panel ID="pnlPopup" runat="server" Style="display: none" Width="550px" Height="100px" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                    <tr>
                        <td align="right">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>Sorry, we do not accept AMEX. Please use an alternate credit card.</b>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnHide" runat="server" Text="OK" CssClass="button" Width="60px" />
                        </td>
                    </tr>
                </table>

            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>


    <asp:Panel ID="pnlConfirmation" runat="server" Width="550px" Height="100px" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                    <tr>
                        <td align="right">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>Interior Innovations is about to charge 
                                <asp:Label ID="lblamount" runat="server"></asp:Label>
                                to your credit card.</b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>Do you Authorize this payment?</b>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnOk" runat="server" Text="Yes" CssClass="button" Width="60px" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel ID="pnlConfirmation1" runat="server" Width="550px" Height="100px" BackColor="Snow">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="2" width="100%" align="center">
                    <tr>
                        <td align="right">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>interior innovations is about to charge 
                                <asp:Label ID="lblamount1" runat="server"></asp:Label>
                                to your bank account.</b>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <b>Do you Authorize this payment?</b>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnOk1" runat="server" Text="Yes" CssClass="button" Width="60px" />
                            <asp:Button ID="btnCancel1" runat="server" Text="Cancel" CssClass="button" Width="60px" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" DynamicLayout="False"
        AssociatedUpdatePanelID="UpdatePanel1">
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
    <%-- <Triggers>
            <asp:PostBackTrigger ControlID="grdCardList" />
        </Triggers>--%>
</asp:Content>

