using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ActivityLogReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        if (!IsPostBack)
        {
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("rpt008") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSalesPersons();

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnViewReport", "ddlSalesPersons" });
        }
    }
    private void BindSalesPersons()
    {
        int nclient_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "SELECT DISTINCT sp.first_name + ' '+sp.last_name AS sales_person_name,sp.sales_person_id " +
                    " FROM sales_person sp " +
                    " INNER JOIN customer_estimate ce ON ce.sales_person_id = sp.sales_person_id AND ce.client_id = sp.client_id " +
                    " WHERE sp.is_active = 1 AND ce.client_id = " + nclient_id + " AND sp.client_id = " + nclient_id +
                    " ORDER BY sales_person_name ASC";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSalesPersons.DataSource = mList;
        ddlSalesPersons.DataTextField = "sales_person_name";
        ddlSalesPersons.DataValueField = "sales_person_id";
        ddlSalesPersons.DataBind();
        ddlSalesPersons.Items.Insert(0, "All");
        ddlSalesPersons.SelectedIndex = 0;
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
    protected void btnViewReport_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewReport.ID, btnViewReport.GetType().Name, "Click"); 
        lblResult.Text = "";

        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (txtStartDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Start Date is a required field");
            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtStartDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Start Date");
                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("End Date is a required field");
            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtEndDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid End Date");
                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text).AddHours(23).AddMinutes(59);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Date Range");
            return;
        }
        string strCondition = string.Empty;
        if (ddlSalesPersons.SelectedItem.Text == "All")
        {
            strCondition = "  WHERE CONVERT(DATETIME,ccl.CallDate) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'";
        }
        else
        {
            strCondition = "  WHERE ccl.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue) + " AND CONVERT(DATETIME,ccl.CallDate) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'";
        }


        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT DISTINCT CONVERT(DATETIME,ccl.CallDate) as CallDate, t1.NOOfCall, Isnull(t2.NOOfPitched,0) as NOOfPitched,Isnull(t3.NOOfBooked,0) as NOOfBooked " +
                    " FROM CustomerCallLog ccl " +
                    " LEFT OUTER JOIN (select count(CallLogID) as NOOfCall, CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog group by CONVERT(DATETIME,CallDate)) AS t1 on CONVERT(DATETIME, t1.CallDate) = CONVERT(DATETIME, ccl.CallDate) " +
                    " LEFT OUTER JOIN (select count(CallTypeId) as NOOfPitched , CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog where CallTypeId = 2  group by CONVERT(DATETIME,CallDate)) AS t2 on CONVERT(DATETIME, t2.CallDate) = CONVERT(DATETIME, ccl.CallDate) " +
                    " LEFT OUTER JOIN (select count(CallTypeId) as NOOfBooked , CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog where CallTypeId = 3  group by CONVERT(DATETIME,CallDate)) AS t3 on CONVERT(DATETIME, t3.CallDate) = CONVERT(DATETIME, ccl.CallDate) " + strCondition;

        DataTable dtReport = csCommonUtility.GetDataTable(strQ);

        if (dtReport.Rows.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No data exist.");
            return;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptActivitylogReport.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtReport);

        string strDate = "Date Range: " + txtStartDate.Text + " To " + txtEndDate.Text + "";

        Hashtable ht = new Hashtable();
        ht.Add("p_date", strDate);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }
}