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

public partial class SalesReportByLeadExcel : System.Web.UI.Page
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
            else
            {
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
                hdnDivisionName.Value = ((userinfo)Session["oUser"]).divisionName;
            }
            if (Page.User.IsInRole("rpt011") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSalesPersons();
        }
    }
    private void BindSalesPersons()
    {
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_id asc";

        DataTable mList = csCommonUtility.GetDataTable(strQ);
        ddlSalesPersons.DataSource = mList;
        ddlSalesPersons.DataTextField = "sales_person_name";
        ddlSalesPersons.DataValueField = "sales_person_id";
        ddlSalesPersons.DataBind();
        ddlSalesPersons.Items.Insert(0, "All");
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
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Date Range");
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
        //string strNewCondition = string.Empty;
        //if (ddlSalesPersons.SelectedItem.Text == "All")
        //{
        //    strNewCondition = " AND customers.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        //}
        //else
        //{
        //    strNewCondition = " AND customer_estimate.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue) + " AND customers.registration_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        //}

        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT customers.customer_id,customer_estimate.estimate_id,customer_estimate.job_number,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,sales_person.first_name +' '+last_name AS SalesRep, customer_estimate.sale_date, " +
                     "   customers.address,  customers.city, customers.state, customers.zip_code, customers.email, customers.phone, customer_estimate.sales_person_id, " +
                     "  customers.status_id, customers.notes, customers.appointment_date, CONVERT(VARCHAR(10), customers.registration_date, 101) AS [Entry Date] ,customers.registration_date, lead_source.lead_source_id,lead_source.lead_name,ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price, '' AS [Sale Amount]  " +
                     " FROM customers  " +
                     " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id " +
                     " INNER JOIN lead_source ON  lead_source.lead_source_id = customers.lead_source_id " +
                     " INNER JOIN sales_person ON  sales_person.sales_person_id = customer_estimate.sales_person_id " +
                     " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id " +
                     " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id " +
                     " WHERE  customer_estimate.status_id=3 " + strCondition+" Order by sales_person.first_name ASC, CONVERT(DATETIME,customer_estimate.sale_date) ASC";//customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 AND

        DataTable dtReport = csCommonUtility.GetDataTable(strQ);

        #region lead 
        //string strQN = " SELECT customers.customer_id,customer_estimate.estimate_id,ISNULL(t1.Total_Price,0) AS Total_Price, " +
        //                 " ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price ,customer_estimate.estimate_name,  " +
        //                 " customers.first_name1+' '+customers.last_name1 AS CustomerName,  customers.address,  customers.city, customers.state, customers.zip_code, " +
        //                 " customers.email, customers.phone, customer_estimate.sales_person_id,  sales_person.first_name +' '+last_name AS SalesRep,  " +
        //                 " customers.status_id, customers.notes, customers.appointment_date, customers.registration_date, lead_source.lead_source_id,lead_source.lead_name  FROM customers   " +
        //                 " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id  " +
        //                 " INNER JOIN lead_source ON  lead_source.lead_source_id = customers.lead_source_id  " +
        //                 " INNER JOIN sales_person ON  sales_person.sales_person_id = customer_estimate.sales_person_id " +
        //                 " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  " +
        //                 " GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id " +
        //                 " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id  " +
        //                 " WHERE  customer_estimate.status_id<>3 " + strNewCondition + " " + //customers.status_id NOT IN(4,5) AND
        //                 " AND customers.customer_id NOT IN(SELECT DISTINCT customers.customer_id FROM customer_estimate " +
        //                 " INNER JOIN  customers ON  customer_estimate.customer_id = customers.customer_id  WHERE  customer_estimate.IsEstimateActive = 1 AND customer_estimate.status_id=3  " + strNewCondition + " ) " +
        //                 " ORDER BY customers.customer_id  ";
        //DataTable dtN = csCommonUtility.GetDataTable(strQN);
        //DataTable dtLead = FormatTaskTable(dtN);


        //DataTable NewLeadTable = dtLead.DefaultView.ToTable(true, "sales_person_id", "SalesRep");
        //DataTable NewSaleTable = dtLead.DefaultView.ToTable(true, "sales_person_id", "SalesRep");
        //foreach (DataRow drm in NewSaleTable.Rows)
        //{
        //    int nSalesId = Convert.ToInt32(drm["sales_person_id"]);
        //    string SalesRep = drm["SalesRep"].ToString();
        //    bool contains = NewLeadTable.AsEnumerable().Any(row => nSalesId == row.Field<int>("sales_person_id"));
        //    if (!contains)
        //    {
        //        DataRow drNew = NewLeadTable.NewRow();
        //        drNew["sales_person_id"] = nSalesId;
        //        drNew["SalesRep"] = SalesRep;
        //    }
        //}
        //DataView dvSales = NewLeadTable.DefaultView;



        //DataTable dtCombine = new DataTable();
        //foreach (DataRow dr in NewLeadTable.Rows)
        //{
        //    int nSalesId = Convert.ToInt32(dr["sales_person_id"]);
        //    bool contains = dtReport.AsEnumerable().Any(row => nSalesId == row.Field<int>("sales_person_id"));
        //    if (!contains)
        //    {
        //        DataRow drNew = dtReport.NewRow();
        //        drNew["customer_id"] = 0;
        //        drNew["estimate_id"] = 0;
        //        drNew["job_number"] = "";
        //        drNew["Total_Price"] = 0;
        //        drNew["NewTotal_Price"] = 0;
        //        drNew["estimate_name"] = "";
        //        drNew["sale_date"] = "01/01/1999";
        //        drNew["CustomerName"] = "No Sales";
        //        drNew["address"] = "";
        //        drNew["city"] = "";
        //        drNew["state"] = "";
        //        drNew["zip_code"] = "";
        //        drNew["email"] = "";
        //        drNew["phone"] = "";
        //        drNew["sales_person_id"] = dr["sales_person_id"];
        //        drNew["SalesRep"] = dr["SalesRep"];
        //        drNew["status_id"] = 0;
        //        drNew["notes"] = "";
        //        drNew["appointment_date"] = Convert.ToDateTime("01/01/1999");
        //        drNew["registration_date"] = Convert.ToDateTime("01/01/1999");
        //        drNew["lead_source_id"] = 0;
        //        drNew["lead_name"] = "";
        //        dtReport.Rows.Add(drNew);

        //    }

        //}
        #endregion

        if (dtReport.Rows.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No data exist.");
            return;
        }
        else
        {
            foreach (DataRow dr in dtReport.Rows)
            {
                decimal nTotal_Price = Convert.ToDecimal(dr["Total_Price"]);
                if (Convert.ToDecimal(dr["NewTotal_Price"]) == 0)
                {
                    dr["NewTotal_Price"] = nTotal_Price;
                    dr["Sale Amount"] = nTotal_Price.ToString("c");


                }
                else
                {
                    dr["Sale Amount"] = Convert.ToDecimal(dr["NewTotal_Price"]).ToString("c"); 
                }
              
            }
        }

       

        dtReport.Columns.Remove("customer_id");
        dtReport.Columns.Remove("estimate_id");
        dtReport.Columns.Remove("address");
        dtReport.Columns.Remove("city");
        dtReport.Columns.Remove("state");
        dtReport.Columns.Remove("zip_code");
        dtReport.Columns.Remove("email");
        dtReport.Columns.Remove("phone");
        dtReport.Columns.Remove("sales_person_id");
        dtReport.Columns.Remove("status_id");
        dtReport.Columns.Remove("notes");
        dtReport.Columns.Remove("appointment_date");
        dtReport.Columns.Remove("lead_source_id");
        dtReport.Columns.Remove("Total_Price");
        dtReport.Columns.Remove("NewTotal_Price");
        dtReport.Columns.Remove("registration_date");

        dtReport.Columns["job_number"].ColumnName = "Job Number";
       // dtReport.Columns["NewTotal_Price"].ColumnName = "Amount";       
        dtReport.Columns["estimate_name"].ColumnName = "Estimate";
        dtReport.Columns["sale_date"].ColumnName = "Sale Date";
        dtReport.Columns["CustomerName"].ColumnName = "Customer Name";
        dtReport.Columns["SalesRep"].ColumnName = "Sales Rep.";
       // dtReport.Columns["registration_date"].ColumnName = "Entry Date";
        dtReport.Columns["lead_name"].ColumnName = "Lead Source";


        Session.Add("sSalesReportExcel", dtReport);

      
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('salesreportexcel.aspx');", true);

    }

    //private static DataTable FormatTaskTable(DataTable dt)
    //{
    //    DataView dv = dt.DefaultView;
    //    DataTable tmpTable = dt.Clone();

    //    int CustId = 0;

    //    ArrayList arr = new ArrayList();
    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        CustId = Convert.ToInt32(dr["customer_id"]);
    //        decimal nTotal_Price = 0;
    //        string strEstimate_name = string.Empty;
    //        dv.RowFilter = "customer_id = " + CustId;
    //        if (dv.Count > 0)
    //        {

    //            for (int i = 0; i < dv.Count; i++)
    //            {
    //                if (Convert.ToDecimal(dv[i]["NewTotal_Price"]) == 0)
    //                {
    //                    nTotal_Price = Convert.ToDecimal(dv[i]["Total_Price"]);
    //                }
    //                else
    //                {
    //                    nTotal_Price = Convert.ToDecimal(dv[i]["NewTotal_Price"]);

    //                }
    //                //if(strEstimate_name.Length == 0)
    //                //   strEstimate_name = dv[i]["estimate_name"].ToString() + " (" + nTotal_Price.ToString("c")+") ";
    //                //else
    //                //    strEstimate_name = strEstimate_name + ", " + dv[i]["estimate_name"].ToString() + " (" + nTotal_Price.ToString("c") + ") ";

    //            }

    //        }

    //        DataRow drNew = tmpTable.NewRow();
    //        drNew["customer_id"] = dr["customer_id"];
    //        drNew["estimate_id"] = dr["estimate_id"];
    //        drNew["Total_Price"] = nTotal_Price; // dr["Total_Price"];
    //        drNew["NewTotal_Price"] = dr["NewTotal_Price"];
    //        drNew["estimate_name"] = dr["estimate_name"]; //strEstimate_name;
    //        drNew["CustomerName"] = dr["CustomerName"];
    //        drNew["address"] = dr["address"];
    //        drNew["city"] = dr["city"];
    //        drNew["state"] = dr["state"];
    //        drNew["zip_code"] = dr["zip_code"];
    //        drNew["email"] = dr["email"];
    //        drNew["phone"] = dr["phone"];
    //        drNew["sales_person_id"] = dr["sales_person_id"];
    //        drNew["SalesRep"] = dr["SalesRep"];
    //        drNew["status_id"] = dr["status_id"];
    //        drNew["notes"] = dr["notes"];
    //        drNew["appointment_date"] = dr["appointment_date"];
    //        drNew["registration_date"] = dr["registration_date"];
    //        drNew["lead_source_id"] = dr["lead_source_id"];
    //        drNew["lead_name"] = dr["lead_name"];

    //        if (!arr.Contains(drNew["customer_id"]))
    //        {
    //            arr.Add(drNew["customer_id"]);
    //            tmpTable.Rows.Add(drNew);
    //        }

    //    }

    //    return tmpTable;
    //}

   

}