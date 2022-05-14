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
using System.Web.SessionState;

public partial class Reports_Common_ReportDocumentViewer_CO : System.Web.UI.Page
{
    ReportDocument rptFile = new ReportDocument();

    private void Page_Load(object sender, System.EventArgs e)
    {
        // Put user code to initialize the page here
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

                if (ExportReport(rptFile, "htm", ConfigurationManager.AppSettings["ReportPath"], Session.SessionID.ToString()))
                {

                    string tempFileNameUsed = "tmp/rptchange_order/" + Session.SessionID.ToString();
                    tempFileNameUsed += ".htm";
                    Response.Redirect(tempFileNameUsed);
                    File.Delete(tempFileNameUsed);

                }
            }
            catch (Exception ex)
            {
                ExportReport(rptFile, "htm", ConfigurationManager.AppSettings["ReportPath"], Session.SessionID.ToString());
            }
        }
    }


    public static bool ExportReport(ReportDocument crReportDocument,
 string ExpType, string ExportPath, string filename)
    {
        //creating full report file name 

        //for example if the filename was "MyReport1"

        //and ExpType was "pdf", full file name will be "MyReport1.pdf"

        filename = filename + "." + ExpType;

        //creating storage directory if not exists

        if (!Directory.Exists(ExportPath))
            Directory.CreateDirectory(ExportPath);

        //creating new instance representing disk file destination 

        //options such as filename, export type etc.

        DiskFileDestinationOptions crDiskFileDestinationOptions =
         new DiskFileDestinationOptions();
        ExportOptions crExportOptions = crReportDocument.ExportOptions;


        switch (ExpType)
        {
            case "rtf":
                {
                    //setting disk file name 

                    crDiskFileDestinationOptions.DiskFileName =
                     ExportPath + filename;
                    //setting destination type in our case disk file

                    crExportOptions.ExportDestinationType =
                     ExportDestinationType.DiskFile;
                    //setuing export format type

                    crExportOptions.ExportFormatType = ExportFormatType.RichText;
                    //setting previously defined destination 

                    //opions to our input report document

                    crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
                    break;
                }
            //NOTE following code is similar to previous, so I want comment it again

            case "pdf":
                {
                    crDiskFileDestinationOptions.DiskFileName =
                      ExportPath + filename;
                    crExportOptions.DestinationOptions =
                     crDiskFileDestinationOptions;
                    crExportOptions.ExportDestinationType =
                     ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType =
                     ExportFormatType.PortableDocFormat;
                    break;
                }
            case "doc":
                {
                    crDiskFileDestinationOptions.DiskFileName = ExportPath + filename;
                    crExportOptions.ExportDestinationType =
                            ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType = ExportFormatType.WordForWindows;
                    crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
                    break;
                }
            case "xls":
                {
                    crDiskFileDestinationOptions.DiskFileName = ExportPath + filename;
                    crExportOptions.ExportDestinationType =
                          ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType = ExportFormatType.Excel;
                    crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
                    break;
                }
            case "rpt":
                {
                    crDiskFileDestinationOptions.DiskFileName = ExportPath + filename;
                    crExportOptions.ExportDestinationType =
                         ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType = ExportFormatType.CrystalReport;
                    crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
                    break;
                }
            case "htm":
                {
                    HTMLFormatOptions HTML40Formatopts = new HTMLFormatOptions();
                    crExportOptions.ExportDestinationType =
                     ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType = ExportFormatType.HTML40;
                    // HTML40Formatopts.HTMLBaseFolderName = ExportPath + filename;
                    HTML40Formatopts.HTMLBaseFolderName = ExportPath;
                    HTML40Formatopts.HTMLFileName = filename;
                    HTML40Formatopts.HTMLEnableSeparatedPages = false;
                    HTML40Formatopts.HTMLHasPageNavigator = false;
                    //HTML40Formatopts.FirstPageNumber = 1;
                    //HTML40Formatopts.LastPageNumber = 3;
                    crExportOptions.FormatOptions = HTML40Formatopts;

                    break;
                }

        }
        try
        {
            //trying to export input report document, 

            //and if success returns true

            crReportDocument.Export();

            return true;
        }
        catch (Exception err)
        {
            return false;
        }
    }

    /// <summary>

    /// Export report to byte array

    /// </summary>

    /// <param name="crReportDocument">

    /// ReportDocument< /param >

    /// <param name="exptype">

    /// CrystalDecisions.Shared.ExportFormatType< /param >

    /// < returns>byte array representing current report< /returns >

    public static byte[] ExportReportToStream(ReportDocument
      crReportDocument, ExportFormatType exptype)
    {//this code exports input report document into stream, 

        //and returns array of bytes


        Stream st;
        st = crReportDocument.ExportToStream(exptype);

        byte[] arr = new byte[st.Length];
        st.Read(arr, 0, (int)st.Length);

        return arr;

    }
}

