<%@ Page Title="Activity Log" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CallLogInfo.aspx.cs" Inherits="CallLogInfo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/ToolsMenu.ascx" TagPrefix="uc1" TagName="ToolsMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <script src="js/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/JavaScript">
        function NewEmailWindow() {
            window.open('sendemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, '_blank', 'left=200,top=100,width=800,height=800,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function NewEmailWindowOutlook() {
            window.open('sendemailoutlook.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, '_blank', 'left=200,top=100,width=800,height=800,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
        function DisplayWindow() {
            window.open('sendemail.aspx?custId=' + document.getElementById('head_hdnCustomerId').value, '_blank', 'left=200,top=100,width=800,height=800,status=0,toolbar=0,resizable=0,scrollbars=1');
        }

        function DisplayWindow(cid) {
            window.open('sendsms.aspx?custId=' + cid, 'MyWindow', 'left=400,top=100,width=550,height=600,status=0,toolbar=0,resizable=0,scrollbars=1');
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table cellpadding="0" cellspacing="0" width="100%">

                <tr>
                    <td align="center" class="cssHeader">
                        <table cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="left"><span class="titleNu">Activity Log</span></td>
                                <td align="right" style="padding-right: 30px; float: right;">
                                    <uc1:ToolsMenu runat="server" ID="ToolsMenu" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table class="wrapper" width="100%">
                            <tr>
                                <td style="width: 260px; border-right: 1px solid #ddd;" align="left" valign="top">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <img src="images/icon-customer-info.png" /></td>
                                            <td align="left">
                                                <h2>Customer Information</h2>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 390px;" align="left" valign="top">
                                    <table style="width: 390px;">
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Customer Name: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblCustomerName" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Phone: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblPhone" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 200px;" align="left" valign="top"><b>Email: </b></td>
                                            <td style="width: auto;">
                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="left" valign="top">
                                    <table style="width: 420px;">
                                        <tr>
                                            <td style="width: 64px;" align="left" valign="top"><b>Address: </b></td>
                                            <td style="width: auto;" align="left" valign="top">
                                                <asp:Label ID="lblAddress" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 64px;" align="left" valign="top"><b>Company: </b></td>

                                            <td style="width: auto;" align="left" valign="top">
                                                <asp:Label ID="lblCompany" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: auto;" align="left" valign="top">&nbsp;</td>
                                            <td align="left" style="width: auto;" valign="top">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="cssHeader" align="center">
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <span class="titleNu">
                                            <img width="25" height="25" src="images/08_call_log.png" alt="Activity Log" title="Activity Log" style="padding: 0px; vertical-align: middle;" />
                                            <span id="head_lblHeaderTitle" class="cssTitleHeader" style="padding: 0px; vertical-align: middle;">Activity Information                                                        
                                            </span>
                                        </span>
                                    </td>
                                    <td align="right"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="wrapper" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td align="center">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr style="padding: 0px; margin: 0px;">
                                            <td align="left">
                                                <asp:HyperLink ID="HyperLink1" runat="server" CssClass="hyp">Compose New Message</asp:HyperLink>
                                            </td>
                                            <td style="padding: 0px; margin: 0px; text-align: right;">
                                                <asp:Panel ID="Panel2" runat="server">
                                                    <span id="PnlCtrlID" runat="server">
                                                        <asp:LinkButton ID="lnkAddNewCall" runat="server" CssClass="button" Width="160px" OnClick="lnkAddNewCall_Click">
                                                            <asp:ImageButton ID="ImageTGI2" runat="server" ImageUrl="~/Images/expand.png" Style="margin: 0px; background: none; border: none; box-shadow: none; padding: 0px; vertical-align: middle;" TabIndex="40" />
                                                            Add New Activity
                                                        </asp:LinkButton>
                                                    </span>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr style="padding: 0px; margin: 0px;">
                                            <td style="padding: 0px; margin: 0px;" align="center">
                                                <asp:Panel ID="pnlTGI2" runat="server" Height="100%">
                                                    <table style="padding: 0px; margin: 0px;" width="100%">
                                                        <tr>
                                                            <td align="right">Type: </td>
                                                            <td align="left">
                                                                <asp:DropDownList ID="ddlCallType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCallType_SelectedIndexChanged" TabIndex="1">
                                                                    <asp:ListItem Value="1">Called</asp:ListItem>
                                                                    <asp:ListItem Value="2">Pitched</asp:ListItem>
                                                                    <asp:ListItem Value="6">Emailed</asp:ListItem>
                                                                    <asp:ListItem Value="3">Booked</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <table id="tblApptDate" runat="server" visible="false">
                                                                    <tr>
                                                                        <td align="right"><span class="required">*</span>Appointment Date: </td>
                                                                        <td align="left">
                                                                            <table cellpadding="0" cellspacing="0" class="tblAppDate">
                                                                                <tr>
                                                                                    <td align="left" width="120px">
                                                                                        <asp:TextBox ID="txtAppointmentDate" runat="server" TabIndex="1" Width="110px"></asp:TextBox>
                                                                                    </td>
                                                                                    <td>&nbsp;</td>
                                                                                    <td align="left">
                                                                                        <asp:ImageButton ID="imgAppointmentDate" runat="server"
                                                                                            CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" /></td>
                                                                                    <td align="left">
                                                                                        <asp:ImageButton ID="btnSalesCalendar" runat="server" CssClass="nostyleCalImg" Height="30" ImageUrl="~/images/sales_calendar.png" OnClick="btnSalesCalendarC_Click" ToolTip="Go to Sales Calendar" Width="27" />
                                                                                    </td>

                                                                                </tr>
                                                                            </table>

                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right"><span class="required">* </span>Subject: </td>
                                                            <td align="left">
                                                                <asp:TextBox ID="txtCallSubject" runat="server" TabIndex="2" Width="285px"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <table id="tblApptTime" runat="server" visible="false">
                                                                    <tr>
                                                                        <td align="right">Appointment Time:</td>
                                                                        <td align="left">
                                                                            <table cellpadding="0" cellspacing="0" style="padding: 0px; margin-left: 2px;">
                                                                                <tr>
                                                                                    <td><b>Start:</b></td>
                                                                                    <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                                                        <div class="cbox">
                                                                                            <cc1:ComboBox ID="cmbStartTime" runat="server" AutoPostBack="true" DropDownStyle="Simple"
                                                                                                CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                                                                OnSelectedIndexChanged="cmbStartTime_SelectedIndexChanged"
                                                                                                AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px" TabIndex="2">
                                                                                                <asp:ListItem></asp:ListItem>
                                                                                            </cc1:ComboBox>
                                                                                        </div>
                                                                                    </td>
                                                                                    <td><b>End:</b></td>
                                                                                    <td align="left" style="vertical-align: top; padding-bottom: 2px;">
                                                                                        <div class="cbox">
                                                                                            <cc1:ComboBox ID="cmbEndTime" runat="server" AutoPostBack="true" DropDownStyle="Simple"
                                                                                                CssClass="ddCombo" CaseSensitive="False" MaxLength="50" AutoCompleteMode="SuggestAppend"
                                                                                                AppendDataBoundItems="false" ItemInsertLocation="Append" Width="48px" TabIndex="2">
                                                                                                <asp:ListItem></asp:ListItem>
                                                                                            </cc1:ComboBox>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>

                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right"><span class="required">* </span>Start Date and Time:</td>
                                                            <td align="left">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtCallStartDate" runat="server" TabIndex="3" Width="68px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:ImageButton ID="imgCallStartDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" TabIndex="4" />
                                                                        </td>
                                                                        <td>
                                                                            <asp:DropDownList ID="ddlCallHour" runat="server" TabIndex="5">
                                                                                <asp:ListItem>00</asp:ListItem>
                                                                                <asp:ListItem>01</asp:ListItem>
                                                                                <asp:ListItem>02</asp:ListItem>
                                                                                <asp:ListItem>03</asp:ListItem>
                                                                                <asp:ListItem>04</asp:ListItem>
                                                                                <asp:ListItem>05</asp:ListItem>
                                                                                <asp:ListItem>06</asp:ListItem>
                                                                                <asp:ListItem>07</asp:ListItem>
                                                                                <asp:ListItem>08</asp:ListItem>
                                                                                <asp:ListItem>09</asp:ListItem>
                                                                                <asp:ListItem>10</asp:ListItem>
                                                                                <asp:ListItem>11</asp:ListItem>
                                                                                <asp:ListItem>12</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>:</td>
                                                                        <td>
                                                                            <asp:DropDownList ID="ddlCallMinutes" runat="server" TabIndex="6">
                                                                                <asp:ListItem>00</asp:ListItem>
                                                                                <asp:ListItem>01</asp:ListItem>
                                                                                <asp:ListItem>02</asp:ListItem>
                                                                                <asp:ListItem>03</asp:ListItem>
                                                                                <asp:ListItem>04</asp:ListItem>
                                                                                <asp:ListItem>05</asp:ListItem>
                                                                                <asp:ListItem>06</asp:ListItem>
                                                                                <asp:ListItem>07</asp:ListItem>
                                                                                <asp:ListItem>08</asp:ListItem>
                                                                                <asp:ListItem>09</asp:ListItem>
                                                                                <asp:ListItem>10</asp:ListItem>
                                                                                <asp:ListItem>11</asp:ListItem>
                                                                                <asp:ListItem>12</asp:ListItem>
                                                                                <asp:ListItem>13</asp:ListItem>
                                                                                <asp:ListItem>14</asp:ListItem>
                                                                                <asp:ListItem>15</asp:ListItem>
                                                                                <asp:ListItem>16</asp:ListItem>
                                                                                <asp:ListItem>17</asp:ListItem>
                                                                                <asp:ListItem>18</asp:ListItem>
                                                                                <asp:ListItem>19</asp:ListItem>
                                                                                <asp:ListItem>20</asp:ListItem>
                                                                                <asp:ListItem>21</asp:ListItem>
                                                                                <asp:ListItem>22</asp:ListItem>
                                                                                <asp:ListItem>23</asp:ListItem>
                                                                                <asp:ListItem>24</asp:ListItem>
                                                                                <asp:ListItem>25</asp:ListItem>
                                                                                <asp:ListItem>26</asp:ListItem>
                                                                                <asp:ListItem>27</asp:ListItem>
                                                                                <asp:ListItem>28</asp:ListItem>
                                                                                <asp:ListItem>29</asp:ListItem>
                                                                                <asp:ListItem>30</asp:ListItem>
                                                                                <asp:ListItem>31</asp:ListItem>
                                                                                <asp:ListItem>32</asp:ListItem>
                                                                                <asp:ListItem>33</asp:ListItem>
                                                                                <asp:ListItem>34</asp:ListItem>
                                                                                <asp:ListItem>35</asp:ListItem>
                                                                                <asp:ListItem>36</asp:ListItem>
                                                                                <asp:ListItem>37</asp:ListItem>
                                                                                <asp:ListItem>38</asp:ListItem>
                                                                                <asp:ListItem>39</asp:ListItem>
                                                                                <asp:ListItem>40</asp:ListItem>
                                                                                <asp:ListItem>41</asp:ListItem>
                                                                                <asp:ListItem>42</asp:ListItem>
                                                                                <asp:ListItem>43</asp:ListItem>
                                                                                <asp:ListItem>44</asp:ListItem>
                                                                                <asp:ListItem>45</asp:ListItem>
                                                                                <asp:ListItem>46</asp:ListItem>
                                                                                <asp:ListItem>47</asp:ListItem>
                                                                                <asp:ListItem>48</asp:ListItem>
                                                                                <asp:ListItem>49</asp:ListItem>
                                                                                <asp:ListItem>50</asp:ListItem>
                                                                                <asp:ListItem>51</asp:ListItem>
                                                                                <asp:ListItem>52</asp:ListItem>
                                                                                <asp:ListItem>53</asp:ListItem>
                                                                                <asp:ListItem>54</asp:ListItem>
                                                                                <asp:ListItem>55</asp:ListItem>
                                                                                <asp:ListItem>56</asp:ListItem>
                                                                                <asp:ListItem>57</asp:ListItem>
                                                                                <asp:ListItem>58</asp:ListItem>
                                                                                <asp:ListItem>59</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </td>
                                                                        <td>

                                                                            <asp:DropDownList ID="ddlCallAMPM" runat="server" TabIndex="7">
                                                                                <asp:ListItem>AM</asp:ListItem>
                                                                                <asp:ListItem>PM</asp:ListItem>
                                                                            </asp:DropDownList>

                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td align="left">
                                                                <table style="width: 400px;">
                                                                    <tr>
                                                                        <td align="left">
                                                                            <asp:CheckBox ID="chkFollowup" runat="server" Text="Followup" TextAlign="Left" AutoPostBack="True" OnCheckedChanged="chkFollowup_CheckedChanged" TabIndex="20" />
                                                                        </td>
                                                                        <td align="left">
                                                                            <table>
                                                                                <tr>
                                                                                    <td align="left">
                                                                                        <table id="tblFollowUp" runat="server" visible="false">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtFollowupDate" runat="server" TabIndex="17" Width="68px"></asp:TextBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:ImageButton ID="imgFollowupDate" runat="server" CssClass="nostyleCalImg" ImageUrl="~/images/calendar.gif" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:DropDownList ID="ddlFollowHour" runat="server">
                                                                                                        <asp:ListItem>00</asp:ListItem>
                                                                                                        <asp:ListItem>01</asp:ListItem>
                                                                                                        <asp:ListItem>02</asp:ListItem>
                                                                                                        <asp:ListItem>03</asp:ListItem>
                                                                                                        <asp:ListItem>04</asp:ListItem>
                                                                                                        <asp:ListItem>05</asp:ListItem>
                                                                                                        <asp:ListItem>06</asp:ListItem>
                                                                                                        <asp:ListItem>07</asp:ListItem>
                                                                                                        <asp:ListItem>08</asp:ListItem>
                                                                                                        <asp:ListItem>09</asp:ListItem>
                                                                                                        <asp:ListItem>10</asp:ListItem>
                                                                                                        <asp:ListItem>11</asp:ListItem>
                                                                                                        <asp:ListItem>12</asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <td>:</td>
                                                                                                <td>
                                                                                                    <asp:DropDownList ID="ddlFollowMin" runat="server">
                                                                                                        <asp:ListItem>00</asp:ListItem>
                                                                                                        <asp:ListItem>01</asp:ListItem>
                                                                                                        <asp:ListItem>02</asp:ListItem>
                                                                                                        <asp:ListItem>03</asp:ListItem>
                                                                                                        <asp:ListItem>04</asp:ListItem>
                                                                                                        <asp:ListItem>05</asp:ListItem>
                                                                                                        <asp:ListItem>06</asp:ListItem>
                                                                                                        <asp:ListItem>07</asp:ListItem>
                                                                                                        <asp:ListItem>08</asp:ListItem>
                                                                                                        <asp:ListItem>09</asp:ListItem>
                                                                                                        <asp:ListItem>10</asp:ListItem>
                                                                                                        <asp:ListItem>11</asp:ListItem>
                                                                                                        <asp:ListItem>12</asp:ListItem>
                                                                                                        <asp:ListItem>13</asp:ListItem>
                                                                                                        <asp:ListItem>14</asp:ListItem>
                                                                                                        <asp:ListItem>15</asp:ListItem>
                                                                                                        <asp:ListItem>16</asp:ListItem>
                                                                                                        <asp:ListItem>17</asp:ListItem>
                                                                                                        <asp:ListItem>18</asp:ListItem>
                                                                                                        <asp:ListItem>19</asp:ListItem>
                                                                                                        <asp:ListItem>20</asp:ListItem>
                                                                                                        <asp:ListItem>21</asp:ListItem>
                                                                                                        <asp:ListItem>22</asp:ListItem>
                                                                                                        <asp:ListItem>23</asp:ListItem>
                                                                                                        <asp:ListItem>24</asp:ListItem>
                                                                                                        <asp:ListItem>25</asp:ListItem>
                                                                                                        <asp:ListItem>26</asp:ListItem>
                                                                                                        <asp:ListItem>27</asp:ListItem>
                                                                                                        <asp:ListItem>28</asp:ListItem>
                                                                                                        <asp:ListItem>29</asp:ListItem>
                                                                                                        <asp:ListItem>30</asp:ListItem>
                                                                                                        <asp:ListItem>31</asp:ListItem>
                                                                                                        <asp:ListItem>32</asp:ListItem>
                                                                                                        <asp:ListItem>33</asp:ListItem>
                                                                                                        <asp:ListItem>34</asp:ListItem>
                                                                                                        <asp:ListItem>35</asp:ListItem>
                                                                                                        <asp:ListItem>36</asp:ListItem>
                                                                                                        <asp:ListItem>37</asp:ListItem>
                                                                                                        <asp:ListItem>38</asp:ListItem>
                                                                                                        <asp:ListItem>39</asp:ListItem>
                                                                                                        <asp:ListItem>40</asp:ListItem>
                                                                                                        <asp:ListItem>41</asp:ListItem>
                                                                                                        <asp:ListItem>42</asp:ListItem>
                                                                                                        <asp:ListItem>43</asp:ListItem>
                                                                                                        <asp:ListItem>44</asp:ListItem>
                                                                                                        <asp:ListItem>45</asp:ListItem>
                                                                                                        <asp:ListItem>46</asp:ListItem>
                                                                                                        <asp:ListItem>47</asp:ListItem>
                                                                                                        <asp:ListItem>48</asp:ListItem>
                                                                                                        <asp:ListItem>49</asp:ListItem>
                                                                                                        <asp:ListItem>50</asp:ListItem>
                                                                                                        <asp:ListItem>51</asp:ListItem>
                                                                                                        <asp:ListItem>52</asp:ListItem>
                                                                                                        <asp:ListItem>53</asp:ListItem>
                                                                                                        <asp:ListItem>54</asp:ListItem>
                                                                                                        <asp:ListItem>55</asp:ListItem>
                                                                                                        <asp:ListItem>56</asp:ListItem>
                                                                                                        <asp:ListItem>57</asp:ListItem>
                                                                                                        <asp:ListItem>58</asp:ListItem>
                                                                                                        <asp:ListItem>59</asp:ListItem>
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                                <td>

                                                                                                    <asp:DropDownList ID="ddlFollowAMPM" runat="server">
                                                                                                        <asp:ListItem>AM</asp:ListItem>
                                                                                                        <asp:ListItem>PM</asp:ListItem>
                                                                                                    </asp:DropDownList>

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
                                                            <td align="right">Duration:</td>
                                                            <td align="left">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TextBox ID="txtDurationH" runat="server" Width="62px" TabIndex="8"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:DropDownList ID="ddlDurationMin" runat="server" TabIndex="9">
                                                                                <asp:ListItem>00</asp:ListItem>
                                                                                <asp:ListItem>01</asp:ListItem>
                                                                                <asp:ListItem>02</asp:ListItem>
                                                                                <asp:ListItem>03</asp:ListItem>
                                                                                <asp:ListItem>04</asp:ListItem>
                                                                                <asp:ListItem>05</asp:ListItem>
                                                                                <asp:ListItem>06</asp:ListItem>
                                                                                <asp:ListItem>07</asp:ListItem>
                                                                                <asp:ListItem>08</asp:ListItem>
                                                                                <asp:ListItem>09</asp:ListItem>
                                                                                <asp:ListItem>10</asp:ListItem>
                                                                                <asp:ListItem>11</asp:ListItem>
                                                                                <asp:ListItem>12</asp:ListItem>
                                                                                <asp:ListItem>13</asp:ListItem>
                                                                                <asp:ListItem>14</asp:ListItem>
                                                                                <asp:ListItem>15</asp:ListItem>
                                                                                <asp:ListItem>16</asp:ListItem>
                                                                                <asp:ListItem>17</asp:ListItem>
                                                                                <asp:ListItem>18</asp:ListItem>
                                                                                <asp:ListItem>19</asp:ListItem>
                                                                                <asp:ListItem>20</asp:ListItem>
                                                                                <asp:ListItem>21</asp:ListItem>
                                                                                <asp:ListItem>22</asp:ListItem>
                                                                                <asp:ListItem>23</asp:ListItem>
                                                                                <asp:ListItem>24</asp:ListItem>
                                                                                <asp:ListItem>25</asp:ListItem>
                                                                                <asp:ListItem>26</asp:ListItem>
                                                                                <asp:ListItem>27</asp:ListItem>
                                                                                <asp:ListItem>28</asp:ListItem>
                                                                                <asp:ListItem>29</asp:ListItem>
                                                                                <asp:ListItem>30</asp:ListItem>
                                                                                <asp:ListItem>31</asp:ListItem>
                                                                                <asp:ListItem>32</asp:ListItem>
                                                                                <asp:ListItem>33</asp:ListItem>
                                                                                <asp:ListItem>34</asp:ListItem>
                                                                                <asp:ListItem>35</asp:ListItem>
                                                                                <asp:ListItem>36</asp:ListItem>
                                                                                <asp:ListItem>37</asp:ListItem>
                                                                                <asp:ListItem>38</asp:ListItem>
                                                                                <asp:ListItem>39</asp:ListItem>
                                                                                <asp:ListItem>40</asp:ListItem>
                                                                                <asp:ListItem>41</asp:ListItem>
                                                                                <asp:ListItem>42</asp:ListItem>
                                                                                <asp:ListItem>43</asp:ListItem>
                                                                                <asp:ListItem>44</asp:ListItem>
                                                                                <asp:ListItem>45</asp:ListItem>
                                                                                <asp:ListItem>46</asp:ListItem>
                                                                                <asp:ListItem>47</asp:ListItem>
                                                                                <asp:ListItem>48</asp:ListItem>
                                                                                <asp:ListItem>49</asp:ListItem>
                                                                                <asp:ListItem>50</asp:ListItem>
                                                                                <asp:ListItem>51</asp:ListItem>
                                                                                <asp:ListItem>52</asp:ListItem>
                                                                                <asp:ListItem>53</asp:ListItem>
                                                                                <asp:ListItem>54</asp:ListItem>
                                                                                <asp:ListItem>55</asp:ListItem>
                                                                                <asp:ListItem>56</asp:ListItem>
                                                                                <asp:ListItem>57</asp:ListItem>
                                                                                <asp:ListItem>58</asp:ListItem>
                                                                                <asp:ListItem>59</asp:ListItem>
                                                                            </asp:DropDownList>

                                                                        </td>
                                                                        <td>(hours/minutes)
                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                            </td>
                                                            <td align="left">
                                                                <table style="width: 400px;">
                                                                    <tr>
                                                                        <td align="left">
                                                                            <asp:CheckBox ID="ChkDoNotCall" runat="server" Text="Do Not Call" TextAlign="Left" TabIndex="20" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <%--  <tr>
                                                            <td align="right">User:</td>
                                                            <td align="left">
                                                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>--%>
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
                                                            <td align="left" colspan="2">
                                                                <asp:TextBox ID="txtCallDescription" runat="server" TabIndex="10" Height="60px" onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'500',document.getElementById('head_txtDisplayCall'));" TextMode="MultiLine" Width="680px"></asp:TextBox>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right" valign="top">&nbsp;</td>
                                                            <td align="left">
                                                                <asp:Label ID="lblResultCallLog" runat="server"></asp:Label>
                                                            </td>
                                                            <td></td>
                                                        </tr>

                                                        <tr>
                                                            <td align="right" valign="top">&nbsp;</td>
                                                            <td align="left" colspan="2">
                                                                <asp:Button ID="btnSaveAndReturn" runat="server" CssClass="button" TabIndex="11" Text=" Save & Return to List " OnClick="btnSaveAndReturn_Click" />
                                                                <asp:Button ID="btnSaveCall" runat="server" CssClass="button" TabIndex="12" Text=" Save Activity " OnClick="btnSaveCall_Click" />
                                                                <asp:Button ID="btnCancel" runat="server" CssClass="button" OnClick="btnCancel_Click" TabIndex="12" Text="CANCEL" />
                                                                <asp:Label ID="lblLoadTime" runat="server" Text="" ForeColor="White"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:GridView ID="grdCallLog" runat="server" AutoGenerateColumns="False" CssClass="mGrid" OnRowCommand="grdCallLog_RowCommand" OnRowDataBound="grdCallLog_RowDataBound" PageSize="50" Width="100%" TabIndex="111">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Subject">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCallSubjectG" runat="server" Text='<%# Eval("CallSubject").ToString() %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="20%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Notes">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCallDescriptionG" runat="server" Text='<%# Eval("Description").ToString() %>' Style="display: inline;"></asp:Label>
                                                                <pre style="height: auto; white-space: pre-wrap; display: inline; font-family: 'Open Sans', Arial, Tahoma, Verdana, sans-serif;"><asp:Label ID="lblCallDescriptionG_r" runat="server" Text='<%# Eval("Description") %>' Visible="false" ></asp:Label></pre>
                                                                <asp:LinkButton ID="lnkOpen" Style="display: inline;" Text="More" Font-Bold="true" ToolTip="Click here to view more" OnClick="lnkOpen_Click" runat="server" ForeColor="Blue"></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="45%" />
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCallType" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="13%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Followup">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFollowup" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="10%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Date Last Called">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblCallStartDateTime" runat="server" Text='<%# Eval("CallDateTime").ToString() %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" Width="8%" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <%-- <asp:TemplateField HeaderText="Duration">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCallDuration" runat="server" Text='<%# Eval("CallDuration").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>--%>
                                                        <asp:ButtonField CommandName="Select" Text="Edit" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="9%"></asp:ButtonField>
                                                    </Columns>
                                                    <PagerStyle CssClass="pgr" />
                                                    <AlternatingRowStyle CssClass="alt" />
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                    </table>

                                </td>
                            </tr>

                            <tr>
                                <td align="center">
                                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnDivisionName" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnClientId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnSalespersonId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnAddress" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnLastName" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnCallLogId" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnEmailType" runat="server" Value="2" />
                                    <asp:HiddenField ID="hdnEstimateId" runat="server" Value="0" />
                                    <cc1:CalendarExtender ID="CallDate" runat="server"
                                        Format="MM/dd/yyyy" PopupButtonID="imgCallStartDate"
                                        PopupPosition="BottomLeft" TargetControlID="txtCallStartDate">
                                    </cc1:CalendarExtender>
                                    <cc1:CalendarExtender ID="appointmentdate" runat="server"
                                        Format="MM/dd/yyyy" PopupButtonID="imgAppointmentDate"
                                        PopupPosition="BottomLeft" TargetControlID="txtAppointmentDate">
                                    </cc1:CalendarExtender>
                                    <cc1:CalendarExtender ID="followDate" runat="server"
                                        Format="MM/dd/yyyy" PopupButtonID="imgFollowupDate"
                                        PopupPosition="BottomLeft" TargetControlID="txtFollowupDate">
                                    </cc1:CalendarExtender>

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>

    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1" AssociatedUpdatePanelID="UpdatePanel1"
        DynamicLayout="False">
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

