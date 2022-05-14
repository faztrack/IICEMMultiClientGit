<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="GeneralSelectionReview.aspx.cs" Inherits="GeneralSelectionReview" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .popup {
            position: absolute;
            width: 800px;
            max-height: 600px;
            overflow-y: scroll;
            background: #fff;
            /*  left: 50% !important;*/
            top: 10% !important;
            border-radius: 5px;
            /*padding: 60px 0;*/
            text-align: justify;
            box-shadow: 0 0 10px 0 #fff !important;
        }

        .popup2 {
            position: absolute;
            width: 800px;
            max-height: 600px;
            overflow-y: scroll;
            background: #fff;
            top: 10% !important;
            border-radius: 5px;
            padding: 20px;
            text-align: justify;
            box-shadow: 0 0 10px 0 #555 !important;
        }

            .popup2 div tr td {
                padding: 5px;
            }

        .btnRight {
            float: right !important;
        }

        .btnRight {
            float: right !important;
            padding: 10px 15px;
        }

        .m-signature-pad--body {
            background-color: #fff;
            width: 800px;
            height: 250px;
            border: 1px solid #000;
            margin-top: 5px;
            margin-bottom: 5px;
        }

        .canvascontainer {
            background-color: #fff;
            width: 800px;
            height: 250px;
        }
    </style>


    <script type="text/javascript">
        //function CreateSignaturePade2() {
        //    console.log("CreateSignaturePade2");
        //};

        function ConfirmSelection(obj, objHdnBtn) {

            if (confirm('Are you sure you want to choose this selection?')) {
                document.getElementById(objHdnBtn.id).click();
            }
            else {
                document.getElementById(obj.id).checked = false;
            }
        }

        function confirmOperation() {
            var r = confirm("Are you sure you want to choose this selection?");
            if (r == true) {
                return true;
            }
            else {
                return false;
            }
        }

        function SetCollapsiblePanel(obj) {
            //  console.log(obj.id);
            $.ajax({
                type: 'POST',
                url: "SectionSelection.aspx/SetCollapsiblePanel",
                data: "{'CollapsiblePanel':'" + JSON.stringify(obj.id) + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //  console.log(data.d);
                    var result = data.d;
                    if (result == "Ok") {
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        }
        function Check_Click(objRef) {
            //Get the Row based on checkbox
            var row = objRef.parentNode.parentNode;

            //Get the reference of GridView
            var GridView = row.parentNode;

            //Get all input elements in Gridview
            var inputList = GridView.getElementsByTagName("input");

            for (var i = 0; i < inputList.length; i++) {
                //The First element is the Header Checkbox
                var headerCheckBox = inputList[0];

                //Based on all or none checkboxes
                //are checked check/uncheck Header Checkbox
                var checked = true;
                if (inputList[i].type == "checkbox" && inputList[i] != headerCheckBox) {
                    if (!inputList[i].checked) {
                        checked = false;
                        break;
                    }
                }
            }
            headerCheckBox.checked = checked;

        }
        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {
                        inputList[i].checked = true;
                    }
                    else {
                        inputList[i].checked = false;
                    }
                }
            }
        }
    </script>

    <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
    <table cellpadding="0" cellspacing="0" width="100%" align="center">
        <tr>
            <td align="center" class="cssHeader">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="left">
                            <span class="titleNu">
                                <asp:Label ID="lblHeaderTitle" runat="server" CssClass="cssTitleHeader">Selection</asp:Label>&nbsp;<asp:Label ID="lblProjectName" ForeColor="Blue" runat="server"></asp:Label></span>
                        </td>
                        <td align="right">
                            <%--  <asp:LinkButton ID="btnBack" Font-Bold="true" ForeColor="#555555" runat="server" Style="margin-right: 30px;"
                                Text="Return to Dashboard" Visible="true" OnClick="btnBack_Click"></asp:LinkButton>--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0" width="90%">



                    <tr>
                        <td align="left">
                            <asp:Button ID="btnPrevious" runat="server" Text="Previous"
                                OnClick="btnPrevious_Click" CssClass="prevButton" />
                        </td>
                        <td align="left" valign="middle">
                            <table>
                                <tr>
                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="right" width="45%"><span class="required">* </span>
                                                    <b>Start Date: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtStartDate" runat="server" Width="120px"></asp:TextBox>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td align="left">
                                                    <cc1:CalendarExtender ID="startdate" runat="server"
                                                        Format="MM/dd/yyyy" PopupButtonID="imgStartDate"
                                                        PopupPosition="BottomLeft" TargetControlID="txtStartDate">
                                                    </cc1:CalendarExtender>
                                                    <asp:ImageButton ID="imgStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left">
                                        <table cellpadding="0" cellspacing="0" style="padding: 0px; margin: 0px;">
                                            <tr>
                                                <td align="right" width="45%"><span class="required">* </span>
                                                    <b>End Date: </b>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtEndDate" runat="server" Width="120px"></asp:TextBox>
                                                </td>
                                                <td>&nbsp;</td>
                                                <td align="left">
                                                    <cc1:CalendarExtender ID="EndDate" runat="server"
                                                        Format="MM/dd/yyyy" PopupButtonID="imgEndDate"
                                                        PopupPosition="BottomLeft" TargetControlID="txtEndDate">
                                                    </cc1:CalendarExtender>
                                                    <asp:ImageButton ID="imgEndDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="left">
                                        <asp:Button ID="btnSearch" runat="server" Text="View"
                                            OnClick="btnSearch_Click" CssClass="button" />
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkViewAll_Click">Reset</asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>

                        <td align="center">
                            <b>Page: </b>
                            <asp:Label ID="lblCurrentPageNo" runat="server" Font-Bold="true" ForeColor="#000000"></asp:Label>
                            &nbsp;
                                    <b>Item per page: </b>
                            <asp:DropDownList ID="ddlItemPerPage" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlItemPerPage_SelectedIndexChanged">
                                <asp:ListItem Selected="True">20</asp:ListItem>
                                <asp:ListItem>30</asp:ListItem>
                                <asp:ListItem>40</asp:ListItem>

                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <%-- <asp:Button ID="btnAddNew" runat="server" Text="Add New Site Review" CssClass="button" OnClick="btnAddNew_Click" />--%>
                        </td>
                        <td align="right">
                            <asp:Button ID="btnNext" runat="server" Text="Next"
                                OnClick="btnNext_Click" CssClass="nextButton" />
                        </td>


                    </tr>
                    <tr>
                        <td align="center" colspan="4">
                            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="5">

                            <asp:GridView ID="grdSelection" runat="server" AutoGenerateColumns="False"
                                CssClass="mGrid"
                                PageSize="200" TabIndex="2" Width="100%" OnRowDataBound="grdSelection_RowDataBound" AllowPaging="True" OnPageIndexChanging="grdSelection_PageIndexChanging">
                                <PagerSettings Position="TopAndBottom" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Eval("CreateDate","{0:d}")%>' />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" Width="120px" />
                                        <ItemStyle HorizontalAlign="Center" Width="120px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyp_Custd" runat="server" Text='<%# Eval("customer_name") %>' CssClass="mGrida2"></asp:HyperLink>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle CssClass="custNameCL" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Section">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSection" runat="server" Text='<%# Eval("section_name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Location">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLocation" runat="server" Text='<%# Eval("location_name") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Title">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' />

                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNotes" runat="server" Text='<%# Eval("Notes") %>' Style="display: inline;" />
                                            <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblNotes_r" runat="server" Text='<%# Eval("Notes") %>' Visible="false" ></asp:Label></pre>
                                            <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="15%" HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Price">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price","{0:c}") %>' />

                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="5%" HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <%--  <asp:TemplateField HeaderText="Days Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDayLeft" runat="server" Text='<%# Eval("ValidTillDate","{0:d}")%>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                                                </asp:TemplateField>--%>
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
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                        <ItemStyle Width="40%" />
                                        <HeaderStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelected" runat="server" />
                                            <asp:Label ID="lblSelected" runat="server"></asp:Label>
                                            <br />
                                            <asp:Label ID="lblSelectionDate" runat="server"></asp:Label>
                                            <asp:Image ID="imgCustomerSign" runat="server" Width="200px" Height="60px" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="5%" HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle HorizontalAlign="Left" CssClass="pgr" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnPrevious0" runat="server"
                                Text="Previous" CssClass="prevButton"
                                OnClick="btnPrevious_Click" />
                        </td>

                        <td align="left" colspan="3">&nbsp;</td>
                        <td align="right">
                            <asp:Button ID="btnNext0" runat="server"
                                Text="Next" CssClass="nextButton"
                                OnClick="btnNext_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                            <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnBackId" runat="server" Value="0" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

    </table>

    <script type="text/javascript">
        var objExtender;
        // this will run automatically when the page has finished loading
        function pageLoad(sender, args) {
            if (document.getElementById("head_hdnSetCollapsiblePanel").value != '') {
                var panelId = document.getElementById("head_hdnSetCollapsiblePanel").value;
                objExtender = $find(panelId);

                //objExtender.add_expandComplete(getCollapsibleState);
                //objExtender.add_collapseComplete(getCollapsibleState);

                objExtender._doOpen();
            }
            else { }
        }

        function getCollapsibleState() {
            if (objExtender.get_Collapsed()) {
                // console.log("Now it is getting collapsed!");
            }
            else {
                //console.log("Now it is getting expanded!");
            }
        }
    </script>
    <script src="js/signature_pad.js"></script>
    <script src="js/signatureShipperApp.js"></script>
    <div id="LoadingProgress" style="display: none">
        <div class="overlay" />
        <div class="overlayContent">
            <p>
                Please wait while your data is being processed
            </p>
            <img src="images/ajax_loader.gif" alt="Loading" border="1" />
        </div>
    </div>
</asp:Content>
