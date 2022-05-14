<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DisplayPop3Email.aspx.cs" Inherits="DisplayPop3Email" Title="Message Details" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
		.emails-table { width: 600px; border: solid 1px #444444; }
		.emails-table-header { font-family: "Trebuchet MS"; font-size: 9pt;
			background-color: #0099B9; font-weight: bold; color: white;
			text-align: center; border: solid 1px #444444; }
		.emails-table-header-cell { font-family: "Georgia"; font-size: 9pt;
			font-weight: bold; border: solid 1px #666666; padding: 6px; }
		.emails-table-cell { font-family: "Georgia"; font-size: 9pt;
			border: solid 1px #666666; padding: 6px; }
		.emails-table-footer { border: solid 1px #666666; padding: 3px;
			width: 50%; }
		.email-datetime { float: right; color: #666666; }
		
		a { font-family: "Lucida Sans Unicode", "Trebuchet MS"; font-size: 9pt;
			color: #005B7F; }
		a:hover { color:red; }
		pre { font-family: "Georgia"; font-size: 9pt; }
    * {
	padding:0;
	margin-left: 0;
	margin-right: 0;
	margin-top: 0;
}

.ajax__htmleditor_editor_default .ajax__htmleditor_editor_container
{
	border: 1px solid #C2C2C2;
}
.ajax__htmleditor_editor_base .ajax__htmleditor_editor_container
{
	border-collapse: separate;
	empty-cells: show;
	width:100%;
	height:100%;
}
.ajax__htmleditor_editor_default .ajax__htmleditor_editor_toptoolbar
{
	background-color:#F0F0F0; padding: 0px 2px 2px 2px;
}
.ajax__htmleditor_editor_base .ajax__htmleditor_editor_toptoolbar
{
	cursor:text;
}
.ajax__htmleditor_editor_default .ajax__htmleditor_editor_toptoolbar .ajax__htmleditor_toolbar_button
{
	background-color:#C2C2C2; margin:2px 0px 0px 0px;
}
.ajax__htmleditor_editor_base .ajax__htmleditor_toolbar_button
{
	height: 21px; white-space: nowrap; border-width: 0px; padding:0px; cursor:pointer; float:left;
}
.ajax__htmleditor_editor_default .ajax__htmleditor_editor_toptoolbar div.ajax__htmleditor_toolbar_button span.ajax__htmleditor_toolbar_selectlable
{
    font-family:Arial; font-size:12px; font-weight:bold;
}
.ajax__htmleditor_editor_base div.ajax__htmleditor_toolbar_button span.ajax__htmleditor_toolbar_selectlable
{
    padding:0px 2px; vertical-align:middle; cursor:text;
}
.ajax__htmleditor_editor_default .ajax__htmleditor_editor_toptoolbar div.ajax__htmleditor_toolbar_button select.ajax__htmleditor_toolbar_selectbutton
{
    font-size:12px; font-family:arial; cursor:pointer;
}
.ajax__htmleditor_editor_base div.ajax__htmleditor_toolbar_button select.ajax__htmleditor_toolbar_selectbutton
{
    vertical-align:middle; padding:0px; margin:0px; height: 20px; width:120px;
}

select option {
	padding-right:15px;
}

.ajax__htmleditor_editor_default .ajax__htmleditor_editor_editpanel
{
	border-width: 0px;
	border-top: 1px solid #C2C2C2;
	border-bottom: 1px solid #C2C2C2;
}
.ajax__htmleditor_editor_base .ajax__htmleditor_editor_editpanel
{
	height:100%;
}
.ajax__htmleditor_editor_default .ajax__htmleditor_editor_bottomtoolbar
{
	background-color:#F0F0F0; padding: 0px 0px 2px 2px;
}
.ajax__htmleditor_editor_base .ajax__htmleditor_editor_bottomtoolbar
{
	cursor:text;
}

.ajax__htmleditor_editor_default .ajax__htmleditor_editor_bottomtoolbar .ajax__htmleditor_toolbar_button
{
	background-color:#C2C2C2; margin:0px 4px 0px 0px;
}
        .style6
        {
            font-family: "Georgia";
            font-size: 9pt;
            font-weight: bold;
            border: solid 1px #666666;
            padding: 6px;
            height: 30px;
        }
        .style7
        {
            font-family: "Georgia";
            font-size: 9pt;
            border: solid 1px #666666;
            padding: 6px;
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="DebugLiteral" runat="server" />
    
    <table class="emails-table">
    <tr>
		<td class="emails-table-header" colspan="2">
		Email #<asp:Literal ID="EmailIdLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="style6">Date &amp; Time</td>
		<td class="style7">
			<asp:Literal ID="DateLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-header-cell">From</td>
		<td class="emails-table-cell">
			<asp:Literal ID="FromLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-header-cell">Subject</td>
		<td class="emails-table-cell">
			<asp:Literal ID="SubjectLiteral" runat="server" /></td>
    </tr>
    <tr id="AttachementsRow" runat="server">
		<td class="emails-table-header-cell">Attachments</td>
		<td class="emails-table-cell">
			<asp:Literal ID="AttachmentsLiteral" runat="server" /></td>
    </tr>
     <tr>
		<td class="emails-table-cell" colspan="2">
			<asp:Literal ID="HeadersLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-cell" colspan="2">
			<asp:Literal ID="BodyLiteral" runat="server" />
            </td>
    </tr>
        <tr>
            <td class="emails-table-cell" colspan="2">
                        &nbsp;</td>
        </tr>
    </table>
    </form>
</body>
</html>
