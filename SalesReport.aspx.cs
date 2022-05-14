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

public partial class SalesReport : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("rpt005") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSalesPersons();
        }
    }
    private void BindSalesPersons()
    {
        int nclient_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "SELECT DISTINCT sp.first_name + ' '+sp.last_name AS sales_person_name,sp.sales_person_id " +
                    " FROM sales_person sp " +
                    " INNER JOIN customer_estimate ce ON ce.sales_person_id = sp.sales_person_id AND ce.client_id = sp.client_id " +
                    " WHERE  sp.is_active = 1 AND ce.client_id = " + nclient_id + " AND sp.client_id = " + nclient_id +
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
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage ("Invalid Date Range");
            return;
        }

        string strCondition = string.Empty;
        if (ddlSalesPersons.SelectedItem.Text == "All")
        {
            strCondition = " AND CONVERT(DATETIME,customer_estimate.sale_date) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        }
        else
        {
            strCondition = " AND customer_estimate.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue) + " AND CONVERT(DATETIME,customer_estimate.sale_date) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        }
       
        DataClassesDataContext _db = new DataClassesDataContext();
       
        string strQ = " SELECT customers.customer_id,customer_estimate.estimate_id,customer_estimate.job_number,ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price ,customer_estimate.estimate_name,customer_estimate.sale_date, "+
                     " customers.first_name1+' '+customers.last_name1 AS CustomerName,  customers.address,  customers.city, customers.state, customers.zip_code, customers.email, customers.phone, customer_estimate.sales_person_id, "+
                     " sales_person.first_name +' '+last_name AS SalesRep,  customers.status_id, customers.notes, customers.appointment_date, lead_source.lead_source_id,lead_source.lead_name "+
                     " FROM customers  "+
                     " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id "+
                     " INNER JOIN lead_source ON  lead_source.lead_source_id = customers.lead_source_id "+
                     " INNER JOIN sales_person ON  sales_person.sales_person_id = customers.sales_person_id " +
                     " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id "+
                     " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id "+
                     " WHERE  customer_estimate.status_id=3 " + strCondition + " Order by sales_person.first_name ASC, CONVERT(DATETIME,customer_estimate.sale_date) ASC"; //customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1  AND

       DataTable dtReport = csCommonUtility.GetDataTable(strQ);

       if (dtReport.Rows.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No data exist.");
            return;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptSalesReport.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtReport);

        string strDate = "Sale Date Range: "+ txtStartDate.Text + " To " + txtEndDate.Text+ "";
       
        Hashtable ht = new Hashtable();
        ht.Add("p_date", strDate);
        ht.Add("p_SalesPersonName", ddlSalesPersons.SelectedItem.Text);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        
    }
    
}