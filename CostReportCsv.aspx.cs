using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CostReportCsv : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            DataTable dtTmp = (DataTable)Session["CostReport"];
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(dtTmp, false);
            }
            Response.ContentType = "application/vnd.ms-excel";
            if (nTypeId == 1)
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=Cost_by_Location.csv");
            }
            else if (nTypeId == 10)
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=CompositeSOW.csv");
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=Cost_by_Section.csv");
 
            }
            Response.End();
        }

    }
}