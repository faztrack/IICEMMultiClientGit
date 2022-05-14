using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;

public partial class Reports_Common_ReportViewer : System.Web.UI.Page
{
    ReportDocument rptFile = new ReportDocument();

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                rptFile = (ReportDocument)Session[SessionInfo.Report_File];
                bool bParam = false;
                foreach (string strKey in Session.Keys)
                {
                    if (strKey == SessionInfo.Report_Param)
                    {
                        bParam = true;
                        break;
                    }
                }
                if (bParam)
                {
                    Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                    ParameterValues param = new ParameterValues();
                    ParameterDiscreteValue Val = new ParameterDiscreteValue();
                    foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                    {
                        if (htable.ContainsKey(obj.Name))
                        {
                            Val.Value = htable[obj.Name].ToString();
                            param.Add(Val);
                            obj.ApplyCurrentValues(param);
                        }
                    }
                }
                if (Request.QueryString.Count == 1)
                {
                    if (Request.QueryString.Keys[0] == "print")
                        rptFile.PrintToPrinter(1, false, 0, 0);
                }
                else
                    CRViewer.ReportSource = rptFile;
                //ShowPdfReport();
                exportReport(rptFile,CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {
                exportReport(rptFile,CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //ShowPdfReport();
            }
        }
    }

    private void ShowPdfReport()
    {
        MemoryStream oStream = (MemoryStream)rptFile.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/pdf";

        Response.BinaryWrite(oStream.ToArray());
        Response.End();
        oStream.Close();

    }
    protected void exportReport(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string contentType = "";
        // Make sure asp.net has create and delete permissions in the directory
        string tempFileName = Server.MapPath("pdf_report") + @"\";//System.Configuration.ConfigurationSettings.AppSettings["TempDir"] + Session.SessionID.ToString() + ".";
        string sPage = string.Empty;
        switch (eft)
        {
            case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                tempFileName += DateTime.Now.Ticks.ToString() + ".pdf";
                sPage = "pdf_report/" + DateTime.Now.Ticks.ToString() + ".pdf";
                contentType = "application/pdf";
                break;

            case CrystalDecisions.Shared.ExportFormatType.WordForWindows:
                tempFileName += "test.doc";
                contentType = "application/msword";
                break;

        }

        CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
        dfo.DiskFileName = tempFileName;

        CrystalDecisions.Shared.ExportOptions eo = selectedReport.ExportOptions;
        eo.DestinationOptions = dfo;
        eo.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
        eo.ExportFormatType = eft;

        selectedReport.Export();
        Response.ClearContent();
        Response.ClearHeaders();
        Response.ContentType = contentType;
        Response.WriteFile(tempFileName);
        Response.Redirect(sPage, false);
        Response.Flush();
        Response.Close();


    }



}
