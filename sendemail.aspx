<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sendemail.aspx.cs" Inherits="sendemail" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-

transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <link type="text/css" href="css/layout.css" rel="stylesheet" />
    <script language="javascript" src="commonScript.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsup/jquery-1.8.2.js"></script>
    <script type="text/javascript" src="jsup/jquery.MultiFile.js"></script>
    <title>Compose new Message</title>
</head>
<body>

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="PopUpMsg" align="left" valign="top" colspan="2">
                    <h1>
                        <img src="images/icon_compose_mail.png" width="48px" />Compose new Message </h1>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="5" width="100%">
            <tr>
                <td align="right"><b>To:</b></td>
                <td>
                    <asp:TextBox ID="txtTo"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right"><b>From:</b></td>
                <td>
                    <asp:TextBox ID="txtFrom"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>CC: </b></td>
                <td>
                    <asp:TextBox ID="txtCc"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr id="Email2" runat="server">
                <td align="right">
                    <b>CC (Email 2):</b></td>
                <td align="left">
                    <asp:TextBox ID="txtCc2" runat="server" CssClass="textBox" TabIndex="1"
                        Width="445px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td align="right"><b>BCC: </b></td>
                <td>
                    <asp:TextBox ID="txtBcc"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="100"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>Subject:</b></td>
                <td>
                    <asp:TextBox ID="txtSubject"
                        runat="server" CssClass="textBox" Width="445px" MaxLength="199"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right"><b>Attachments:</b></td>
                <td>
                    <asp:Table ID="tdLink" runat="server">
                    </asp:Table>
                </td>
            </tr>
            <tr>
                <td align="right"><b>Add Attachments:</b></td>
                <td align="left">

                    <table>
                        <tr>
                            <td>

                                <asp:FileUpload ID="file_upload" class="multi" CssClass="blindInput" runat="server" accept=".pdf, .doc, .docx, .xls, .xlsx, .csv, .txt, .jpg, .jpeg, .png, .gif" AllowMultiple="true" />
                            </td>
                            <td>
                                <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Width="180px"
                                    Text="Upload Attachment" CssClass="button" />

                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">

                                <asp:Label ID="lblMessage" runat="server" />

                            </td>
                        </tr>
                    </table>

                </td>
            </tr>
            <tr>
                <td align="right">&nbsp;</td>
                <td>
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
            <tr align="right">
                <td>&nbsp;</td>
                <td align="left">Write your message below:(5000 Chars Max)<asp:TextBox
                    ID="txtDisplay" runat="server" BackColor="Transparent" CssClass="blindInput"
                    BorderColor="Transparent" BorderStyle="None" BorderWidth="0px" Font-Bold="True"
                    Height="16px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="style1">
                    <asp:TextBox ID="txtBody" runat="server" CssClass="textBox"
                        TextMode="MultiLine" Height="220px" Width="637px"
                        onkeydown="checkTextAreaMaxLengthWithDisplay(this,event,'5000',document.getElementById('txtDisplay'));"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:ImageButton ID="imgSend" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/send.gif"
                        OnClick="imgSend_Click" />
                    &nbsp;<asp:ImageButton ID="imgCencel" runat="server" CssClass="blindInput" ImageUrl="~/_scripts/cancelMail.gif" />
                </td>
            </tr>
            <tr>
                <td colspan="2">

                    <asp:HiddenField ID="hdnCustomerId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnMessageId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnCallLogId" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnSalespersonId" runat="server" Value="0" />
                    <cc1:CalendarExtender ID="followDate" runat="server"
                        Format="MM/dd/yyyy" PopupButtonID="imgFollowupDate"
                        PopupPosition="BottomLeft" TargetControlID="txtFollowupDate">
                    </cc1:CalendarExtender>
                </td>
            </tr>
        </table>
        <input type="hidden" name="hdnva" runat="server" id="hdnva" />


    </form>
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            window.close();
        }
    </script>
</body>

</html>
