<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportDocumentViewer.aspx.cs" Inherits="Reports_Common_ReportDocumentViewer" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <CR:CrystalReportViewer id="CRViewer" style="Z-INDEX: 101; LEFT: 152px; POSITION: absolute; TOP: 192px"
				runat="server" Height="50px" Width="350px"></CR:CrystalReportViewer>
    </form>
</body>
</html>
