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

using System.Collections.Generic;
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;


public partial class sales_statistics : System.Web.UI.Page
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
            if (Page.User.IsInRole("rpt002") == false)
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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Start Date");
                
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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid End Date");
                
                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");
            
            return;
        }

       
        DataClassesDataContext _db = new DataClassesDataContext();
        int nclient_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == nclient_id);
        string strCompanyName = oCom.company_name;
          string strQ ="";
          if (ddlSalesPersons.SelectedItem.Text == "All")
          {
              strQ = " SELECT sp.sales_person_id, sp.first_name + ' ' + sp.last_name AS Sales_Person, ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL(t2.NumberOfSales,0) AS NumberOfSales, ISNULL(t3.NumberOfAppt,0) AS NumberOfAppt " +
                      " FROM customer_estimate ce " +
                      " INNER JOIN customers c ON c.customer_id = ce.customer_id AND c.client_id = ce.client_id " +
                      " INNER JOIN sales_person sp ON sp.sales_person_id = c.sales_person_id AND sp.client_id = c.client_id " +
                      " LEFT OUTER JOIN (SELECT pd.client_id,c1.sales_person_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd " +
                      " INNER JOIN customer_estimate ce1 ON ce1.estimate_id = pd.estimate_id AND ce1.customer_id = pd.customer_id  AND  ce1.client_id = pd.client_id AND ce1.status_id = 3 " +
                      " INNER JOIN customers c1 ON c1.customer_id = pd.customer_id AND c1.client_id = pd.client_id " +
                      " WHERE pd.client_id = " + nclient_id + " AND c1.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c1.sales_person_id,pd.client_id) AS t1 ON t1.sales_person_id = c.sales_person_id AND t1.client_id = ce.client_id " +
                      " LEFT OUTER JOIN (select ce2.client_id,c2.sales_person_id,COUNT(ce2.estimate_id) AS NumberOfSales " +
                      "  from customer_estimate ce2 "+
                      "INNER JOIN customers c2 ON c2.customer_id = ce2.customer_id AND c2.client_id = ce2.client_id "+
                      " where ce2.status_id = 3 AND ce2.client_id =" + nclient_id + "  AND c2.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c2.sales_person_id,ce2.client_id) AS t2 ON t2.sales_person_id = c.sales_person_id AND t2.client_id = ce.client_id " +
                      " LEFT OUTER JOIN (select pdm.client_id,c3.sales_person_id,COUNT(distinct pdm.customer_id) AS NumberOfAppt from pricing_details pdm " +
                      " INNER JOIN customers c3 ON c3.customer_id = pdm.customer_id AND c3.client_id = pdm.client_id "+
                      " where  pdm.client_id = " + nclient_id + " AND c3.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c3.sales_person_id,pdm.client_id ) AS t3 ON t3.sales_person_id = c.sales_person_id AND t3.client_id = ce.client_id " +
                      " WHERE  ce.client_id = " + nclient_id + "  AND c.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' GROUP BY sp.sales_person_id,sp.first_name, sp.last_name,Total_Price,NumberOfSales,NumberOfAppt";
          }
          else
          {
              int nSalesPersonId = Convert.ToInt32(ddlSalesPersons.SelectedValue);
              strQ = " SELECT sp.sales_person_id, sp.first_name + ' ' + sp.last_name AS Sales_Person, ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL(t2.NumberOfSales,0) AS NumberOfSales, ISNULL(t3.NumberOfAppt,0) AS NumberOfAppt " +
                     " FROM customer_estimate ce " +
                       " INNER JOIN customers c ON c.customer_id = ce.customer_id AND c.client_id = ce.client_id " +
                     " INNER JOIN sales_person sp ON sp.sales_person_id = c.sales_person_id AND sp.client_id = c.client_id " +
                     " LEFT OUTER JOIN (SELECT pd.client_id,c1.sales_person_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd " +
                     " INNER JOIN customer_estimate ce1 ON ce1.estimate_id = pd.estimate_id AND ce1.customer_id = pd.customer_id  AND  ce1.client_id = pd.client_id AND ce1.status_id = 3 " +
                     " INNER JOIN customers c1 ON c1.customer_id = pd.customer_id AND c1.client_id = pd.client_id " +
                     " WHERE pd.client_id = " + nclient_id + " AND c1.sales_person_id = " + nSalesPersonId + " AND c1.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c1.sales_person_id,pd.client_id) AS t1 ON t1.sales_person_id = c.sales_person_id AND t1.client_id = ce.client_id " +
                     " LEFT OUTER JOIN (select ce2.client_id,c2.sales_person_id,COUNT(ce2.estimate_id) AS NumberOfSales " +
                     "  from customer_estimate ce2 " +
                     "INNER JOIN customers c2 ON c2.customer_id = ce2.customer_id AND c2.client_id = ce2.client_id " +
                     " where ce2.status_id = 3 AND ce2.client_id =" + nclient_id + " AND c2.sales_person_id = " + nSalesPersonId + "  AND c2.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c2.sales_person_id,ce2.client_id) AS t2 ON t2.sales_person_id = c.sales_person_id AND t2.client_id = ce.client_id " +
                     " LEFT OUTER JOIN (select pdm.client_id,c3.sales_person_id,COUNT(distinct pdm.customer_id) AS NumberOfAppt from pricing_details pdm " +
                     " INNER JOIN customers c3 ON c3.customer_id = pdm.customer_id AND c3.client_id = pdm.client_id " +
                     " where  pdm.client_id = " + nclient_id + "  AND c3.sales_person_id = " + nSalesPersonId + " AND c3.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'  GROUP BY c3.sales_person_id,pdm.client_id ) AS t3 ON t3.sales_person_id = c.sales_person_id AND t3.client_id = ce.client_id " +
                     " WHERE  ce.client_id = " + nclient_id + " AND c.sales_person_id = " + nSalesPersonId + "  AND c.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' GROUP BY sp.sales_person_id,sp.first_name, sp.last_name,Total_Price,NumberOfSales,NumberOfAppt";

          }

        List<SalesStatisticsModel> SCList = _db.ExecuteQuery<SalesStatisticsModel>(strQ, string.Empty).ToList();
        
	if (SCList.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("No data exist.");
            
            return;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptSalesStatistics.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(SCList);

        string strDate = "Entry Date Range: " + txtStartDate.Text + " To " + txtEndDate.Text + "";
       
        Hashtable ht = new Hashtable();
        ht.Add("p_date", strDate);
        ht.Add("p_SalesPersonName", ddlSalesPersons.SelectedItem.Text);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        
    }
    
}
