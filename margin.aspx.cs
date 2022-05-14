using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class margin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataTable dtTmp = (DataTable)Session["Margin"];
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                //string[] str1 = { "Margin Report" };
                //string[] str2 = { " " };

                //writer.WriteRecord(str1);
                //writer.WriteRecord(str2, true);
                writer.WriteAll(dtTmp, false);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=MarginReport.csv");
            Response.End();
        }

    }
}