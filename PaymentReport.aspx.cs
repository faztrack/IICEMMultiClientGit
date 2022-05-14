using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PaymentReport : System.Web.UI.Page
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
            if (Page.User.IsInRole("rpt010") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            //BindSalesPersons();
            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnViewReport" });
        }
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
        strCondition = "  pay_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        //if (ddlSalesPersons.SelectedItem.Text == "All")
        //{
        //    strCondition = "  pay_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        //}
        //else
        //{
        //    strCondition = " AND customer_estimate.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue) + " AND pay_date BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";
        //}

        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT CONVERT(VarChar(50), pp.pay_date, 101) AS [Payment Date],c.first_name1+' '+c.last_name1 as [Customer Name],ce.estimate_name as Estimate, pp.pay_term_desc as [Payment Term], CASE pp.pay_type_id WHEN 1 THEN 'Cash' WHEN 2 THEN 'Check' ELSE 'Credit Card' END AS [Payment Type],pp.reference as Reference,'$' + CONVERT(varchar(12),  pp.pay_amount, 1) AS Amount " +
                     " FROM New_partial_payment pp " +
                     " INNER JOIN customers c ON pp.customer_id = c.customer_id " +
                     " INNER JOIN customer_estimate ce  ON pp.customer_id = ce.customer_id and pp.estimate_id = ce.estimate_id " +
                     " WHERE "+strCondition+" ORDER BY pay_date desc ";

        DataTable dtReport = csCommonUtility.GetDataTable(strQ);


        //Session.Add("PayReport", dtReport);

        // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('margin.aspx');", true);

        Response.Clear();
        Response.ClearHeaders();

        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
        {
            string[] str1 = { "Payment Report" };
            string[] str2 = { "Date Range: " + txtStartDate.Text + " to " + txtEndDate.Text };
            writer.WriteRecord(str1, true);
            writer.WriteRecord(str2, true);
            writer.WriteAll(dtReport, true);
        }
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment; filename=PaymentReport.csv");
        Response.End();



    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}