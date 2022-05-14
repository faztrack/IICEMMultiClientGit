<%@ Page Language="C#" AutoEventWireup="true" CodeFile="jobdesc_popup.aspx.cs" Inherits="jobdesc_popup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link type="text/css" href="css/core.css" rel="stylesheet" />
    <title>Job Status Descriptions</title>
</head>
<body>
    <script language="javascript" type="text/javascript">
        function DisplayWindow() {
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('hdnCustomerId').value, 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function PrintWindow() {
            window.print();
        }
    </script>
    <form id="form1" runat="server">
        <div>
            <table class="body2bg" align="center" border="0" cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td align="right" valign="top">
                        <asp:LinkButton CssClass="titleSaving" ID="lnkPrint" runat="server">Print</asp:LinkButton>
                        &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton CssClass="titleSaving" ID="lnkClose" runat="server">Close</asp:LinkButton></td>
                </tr>
                <tr>
                    <td align="center" height="15px">
                        <h2>
                            <asp:Label ID="lblJobStatusFor" runat="server" Text=""></asp:Label></h2>
                    </td>
                </tr>

                <tr>
                    <td align="right" height="15px">
                        <asp:HyperLink ID="HyperLink1" runat="server" CssClass="hyp" Visible="False">Compose New Message</asp:HyperLink>
                    </td>
                </tr>

                <tr>
                    <td align="center">
                        <asp:GridView ID="grdDescriptions" runat="server" AutoGenerateColumns="False"
                            Width="99%">
                            <RowStyle CssClass="grid_row" />
                            <Columns>
                                <asp:BoundField DataField="status_description" HeaderText="Description">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                            <HeaderStyle CssClass="grid_header_style" />
                            <AlternatingRowStyle CssClass="grid_row_alt" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:RadioButtonList
                            ID="rdoSortByWeek" runat="server"
                            RepeatDirection="Horizontal" Visible="False">
                            <asp:ListItem Value="1" Selected="True">Sort by Week of Execution</asp:ListItem>
                            <asp:ListItem Value="2">Sort by Sections</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Label ID="lblResult1" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:GridView ID="grdGrouping" runat="server" ShowFooter="False"
                            OnRowDataBound="grdGrouping_RowDataBound" AutoGenerateColumns="False" Width="100%"
                            CssClass="mGrid">
                            <FooterStyle CssClass="white_text" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("colName").ToString() %>'
                                            CssClass="grid_header" />

                                        <asp:GridView ID="grdSelectedItem1" runat="server" AutoGenerateColumns="False" ShowFooter="False"
                                            DataKeyNames="item_id" OnRowDataBound="grdSelectedItem_RowDataBound"
                                            Width="100%" CssClass="mGrid">
                                            <Columns>
                                                <asp:BoundField DataField="item_id" HeaderText="Item Id" Visible="false"></asp:BoundField>
                                                <asp:BoundField DataField="section_serial" HeaderText="SL" Visible="false"></asp:BoundField>
                                                <asp:BoundField DataField="section_name" HeaderText="Section">
                                                    <HeaderStyle Width="12%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="location_name" HeaderText="Location">
                                                    <HeaderStyle Width="12%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                    <HeaderStyle Width="24%" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="measure_unit" HeaderText="UoM" Visible="false" NullDisplayText=" "></asp:BoundField>
                                                <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false"></asp:BoundField>
                                                <asp:BoundField DataField="quantity" HeaderText="Code" Visible="false"></asp:BoundField>
                                                <asp:TemplateField HeaderText="Ext. Price" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal_price" runat="server"
                                                            Text='<%# Eval("total_retail_price") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                    Visible="False"></asp:BoundField>
                                                <asp:BoundField DataField="short_notes" HeaderText="Short Notes">
                                                    <HeaderStyle Width="18%" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Week Of Execution" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlWeek" runat="server"
                                                            SelectedValue='<%# Eval("week_id") %>'
                                                            Width="90px">
                                                            <asp:ListItem Value="0">NO Week</asp:ListItem>
                                                            <asp:ListItem Value="1">Week 1</asp:ListItem>
                                                            <asp:ListItem Value="2">Week 2</asp:ListItem>
                                                            <asp:ListItem Value="3">Week 3</asp:ListItem>
                                                            <asp:ListItem Value="4">Week 4</asp:ListItem>
                                                            <asp:ListItem Value="5">Week 5</asp:ListItem>
                                                            <asp:ListItem Value="6">Week 6</asp:ListItem>
                                                            <asp:ListItem Value="7">Week 7</asp:ListItem>
                                                            <asp:ListItem Value="8">Week 8</asp:ListItem>
                                                            <asp:ListItem Value="9">Week 9</asp:ListItem>
                                                            <asp:ListItem Value="10">Week 10</asp:ListItem>
                                                            <asp:ListItem Value="11">Week 11</asp:ListItem>
                                                            <asp:ListItem Value="12">Week 12</asp:ListItem>
                                                            <asp:ListItem Value="13">Week 13</asp:ListItem>
                                                            <asp:ListItem Value="14">Week 14</asp:ListItem>
                                                            <asp:ListItem Value="15">Week 15</asp:ListItem>
                                                            <asp:ListItem Value="16">Week 16</asp:ListItem>
                                                            <asp:ListItem Value="17">Week 17</asp:ListItem>
                                                            <asp:ListItem Value="18">Week 18</asp:ListItem>
                                                            <asp:ListItem Value="19">Week 19</asp:ListItem>
                                                            <asp:ListItem Value="20">Week 20</asp:ListItem>
                                                            <asp:ListItem Value="21">Week 21</asp:ListItem>
                                                            <asp:ListItem Value="22">Week 22</asp:ListItem>
                                                            <asp:ListItem Value="23">Week 23</asp:ListItem>
                                                            <asp:ListItem Value="24">Week 24</asp:ListItem>
                                                            <asp:ListItem Value="25">Week 25</asp:ListItem>
                                                            <asp:ListItem Value="26">Week 26</asp:ListItem>
                                                            <asp:ListItem Value="27">Week 27</asp:ListItem>
                                                            <asp:ListItem Value="28">Week 28</asp:ListItem>
                                                            <asp:ListItem Value="29">Week 29</asp:ListItem>
                                                            <asp:ListItem Value="30">Week 30</asp:ListItem>
                                                            <asp:ListItem Value="31">Week 31</asp:ListItem>
                                                            <asp:ListItem Value="32">Week 32</asp:ListItem>
                                                            <asp:ListItem Value="33">Week 33</asp:ListItem>
                                                            <asp:ListItem Value="34">Week 34</asp:ListItem>
                                                            <asp:ListItem Value="35">Week 35</asp:ListItem>
                                                            <asp:ListItem Value="36">Week 36</asp:ListItem>
                                                            <asp:ListItem Value="37">Week 37</asp:ListItem>
                                                            <asp:ListItem Value="38">Week 38</asp:ListItem>
                                                            <asp:ListItem Value="39">Week 39</asp:ListItem>
                                                            <asp:ListItem Value="40">Week 40</asp:ListItem>
                                                            <asp:ListItem Value="41">Week 41</asp:ListItem>
                                                            <asp:ListItem Value="42">Week 42</asp:ListItem>
                                                            <asp:ListItem Value="43">Week 43</asp:ListItem>
                                                            <asp:ListItem Value="44">Week 44</asp:ListItem>
                                                            <asp:ListItem Value="45">Week 45</asp:ListItem>
                                                            <asp:ListItem Value="46">Week 46</asp:ListItem>
                                                            <asp:ListItem Value="47">Week 47</asp:ListItem>
                                                            <asp:ListItem Value="48">Week 48</asp:ListItem>
                                                            <asp:ListItem Value="49">Week 49</asp:ListItem>
                                                            <asp:ListItem Value="50">Week 50</asp:ListItem>
                                                            <asp:ListItem Value="51">Week 51</asp:ListItem>
                                                            <asp:ListItem Value="52">Week 52</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="8%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Unit Of Execution" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUnitExe" runat="server"
                                                            Text='<%# Eval("execution_unit") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Schedule Notes">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblScheduleNotes" runat="server"
                                                            Text='<%# Eval("schedule_note") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkComplete" runat="server" Visible="false"
                                                            Checked='<%# Eval("is_complete") %>' />
                                                        <asp:Label ID="lblComplete" runat="server" Text=""></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:ButtonField CommandName="Edit" Text="Edit" Visible="false">
                                                    <HeaderStyle Width="5%" />
                                                </asp:ButtonField>

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
                <tr>
                    <td align="left" valign="top">
                        <table width="100%" border="0" cellspacing="4" cellpadding="4">
                            <tr>
                                <td align="left" valign="top">
                                    <asp:GridView ID="grdGroupingDirect" runat="server" AutoGenerateColumns="False" Width="100%"
                                        CssClass="mGrid" OnRowDataBound="grdGroupingDirect_RowDataBound"
                                        ShowFooter="False" CaptionAlign="Top">
                                        <FooterStyle CssClass="white_text" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" CssClass="grid_header"
                                                        Text='<%# Eval("colName").ToString() %>' />
                                                    <asp:GridView ID="grdSelectedItem2" runat="server" AutoGenerateColumns="False" DataKeyNames="item_id"
                                                        OnRowDataBound="grdSelectedItem2_RowDataBound" ShowFooter="False"
                                                        Width="100%" CssClass="mGrid">
                                                        <Columns>
                                                            <asp:BoundField DataField="item_id" HeaderText="Item Id" Visible="False"></asp:BoundField>
                                                            <asp:BoundField DataField="section_serial" HeaderText="SL" Visible="False"></asp:BoundField>
                                                            <asp:BoundField DataField="section_name" HeaderText="Section">
                                                                <HeaderStyle Width="12%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="location_name" HeaderText="Location" Visible="False"></asp:BoundField>
                                                            <asp:BoundField DataField="item_name" HeaderText="Item Name">
                                                                <HeaderStyle Width="24%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="measure_unit" HeaderText="UoM" Visible="False" NullDisplayText=" "></asp:BoundField>
                                                            <asp:BoundField DataField="item_cost" HeaderText="Unit Price" Visible="false"></asp:BoundField>
                                                            <asp:BoundField DataField="quantity" HeaderText="Code" Visible="False"></asp:BoundField>
                                                            <asp:TemplateField HeaderText="Direct Price" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTotal_price2" runat="server"
                                                                        Text='<%# Eval("total_direct_price") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="labor_rate" HeaderText="Labor Rate"
                                                                Visible="False">
                                                                <HeaderStyle Width="1%" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="short_notes" HeaderText="Short Notes">
                                                                <HeaderStyle Width="18%" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Week Of Execution" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlWeek1" runat="server"
                                                                        SelectedValue='<%# Eval("week_id") %>'
                                                                        Width="90px">
                                                                        <asp:ListItem Value="0">NO Week</asp:ListItem>
                                                                        <asp:ListItem Value="1">Week 1</asp:ListItem>
                                                                        <asp:ListItem Value="2">Week 2</asp:ListItem>
                                                                        <asp:ListItem Value="3">Week 3</asp:ListItem>
                                                                        <asp:ListItem Value="4">Week 4</asp:ListItem>
                                                                        <asp:ListItem Value="5">Week 5</asp:ListItem>
                                                                        <asp:ListItem Value="6">Week 6</asp:ListItem>
                                                                        <asp:ListItem Value="7">Week 7</asp:ListItem>
                                                                        <asp:ListItem Value="8">Week 8</asp:ListItem>
                                                                        <asp:ListItem Value="9">Week 9</asp:ListItem>
                                                                        <asp:ListItem Value="10">Week 10</asp:ListItem>
                                                                        <asp:ListItem Value="11">Week 11</asp:ListItem>
                                                                        <asp:ListItem Value="12">Week 12</asp:ListItem>
                                                                        <asp:ListItem Value="13">Week 13</asp:ListItem>
                                                                        <asp:ListItem Value="14">Week 14</asp:ListItem>
                                                                        <asp:ListItem Value="15">Week 15</asp:ListItem>
                                                                        <asp:ListItem Value="16">Week 16</asp:ListItem>
                                                                        <asp:ListItem Value="17">Week 17</asp:ListItem>
                                                                        <asp:ListItem Value="18">Week 18</asp:ListItem>
                                                                        <asp:ListItem Value="19">Week 19</asp:ListItem>
                                                                        <asp:ListItem Value="20">Week 20</asp:ListItem>
                                                                        <asp:ListItem Value="21">Week 21</asp:ListItem>
                                                                        <asp:ListItem Value="22">Week 22</asp:ListItem>
                                                                        <asp:ListItem Value="23">Week 23</asp:ListItem>
                                                                        <asp:ListItem Value="24">Week 24</asp:ListItem>
                                                                        <asp:ListItem Value="25">Week 25</asp:ListItem>
                                                                        <asp:ListItem Value="26">Week 26</asp:ListItem>
                                                                        <asp:ListItem Value="27">Week 27</asp:ListItem>
                                                                        <asp:ListItem Value="28">Week 28</asp:ListItem>
                                                                        <asp:ListItem Value="29">Week 29</asp:ListItem>
                                                                        <asp:ListItem Value="30">Week 30</asp:ListItem>
                                                                        <asp:ListItem Value="31">Week 31</asp:ListItem>
                                                                        <asp:ListItem Value="32">Week 32</asp:ListItem>
                                                                        <asp:ListItem Value="33">Week 33</asp:ListItem>
                                                                        <asp:ListItem Value="34">Week 34</asp:ListItem>
                                                                        <asp:ListItem Value="35">Week 35</asp:ListItem>
                                                                        <asp:ListItem Value="36">Week 36</asp:ListItem>
                                                                        <asp:ListItem Value="37">Week 37</asp:ListItem>
                                                                        <asp:ListItem Value="38">Week 38</asp:ListItem>
                                                                        <asp:ListItem Value="39">Week 39</asp:ListItem>
                                                                        <asp:ListItem Value="40">Week 40</asp:ListItem>
                                                                        <asp:ListItem Value="41">Week 41</asp:ListItem>
                                                                        <asp:ListItem Value="42">Week 42</asp:ListItem>
                                                                        <asp:ListItem Value="43">Week 43</asp:ListItem>
                                                                        <asp:ListItem Value="44">Week 44</asp:ListItem>
                                                                        <asp:ListItem Value="45">Week 45</asp:ListItem>
                                                                        <asp:ListItem Value="46">Week 46</asp:ListItem>
                                                                        <asp:ListItem Value="47">Week 47</asp:ListItem>
                                                                        <asp:ListItem Value="48">Week 48</asp:ListItem>
                                                                        <asp:ListItem Value="49">Week 49</asp:ListItem>
                                                                        <asp:ListItem Value="50">Week 50</asp:ListItem>
                                                                        <asp:ListItem Value="51">Week 51</asp:ListItem>
                                                                        <asp:ListItem Value="52">Week 52</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Unit Of Execution" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUnitExe1" runat="server"
                                                                        Text='<%# Eval("execution_unit") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Schedule Notes">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblScheduleNotes1" runat="server"
                                                                        Text='<%# Eval("schedule_note") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="12%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Status">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkComplete1" runat="server" Visible="false"
                                                                        Checked='<%# Eval("is_complete") %>' />
                                                                    <asp:Label ID="lblComplete1" runat="server" Text=""></asp:Label>
                                                                </ItemTemplate>
                                                                <HeaderStyle Width="8%" />
                                                            </asp:TemplateField>
                                                            <asp:ButtonField CommandName="Edit" Text="Edit" Visible="false">
                                                                <HeaderStyle Width="5%" />
                                                            </asp:ButtonField>
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
                            <tr>
                                <td align="left" valign="top">
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
    </script>
    
</body>
</html>
