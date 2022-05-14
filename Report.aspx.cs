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
using DataStreams.Csv;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

public partial class Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable CusTable = LoadDataTable();
            DataTable EstTable = LoadDataTable();
            DataTable PayTable = LoadDataTable();

            DataRow drNewCus = CusTable.NewRow();
            drNewCus["ColumnID"] = 1;
            drNewCus["ColumnName"] = "Customer Name";
            CusTable.Rows.Add(drNewCus);
            drNewCus = CusTable.NewRow();
            drNewCus["ColumnID"] = 2;
            drNewCus["ColumnName"] = "Customer Name2";
            CusTable.Rows.Add(drNewCus);
            drNewCus = CusTable.NewRow();
            drNewCus["ColumnID"] = 3;
            drNewCus["ColumnName"] = "Phone";
            CusTable.Rows.Add(drNewCus);
            drNewCus = CusTable.NewRow();
            drNewCus["ColumnID"] = 9;
            drNewCus["ColumnName"] = "Status Note";
            CusTable.Rows.Add(drNewCus);
            drNewCus = CusTable.NewRow();
            drNewCus["ColumnID"] = 5;
            drNewCus["ColumnName"] = "Sales Person";
            CusTable.Rows.Add(drNewCus);

            DataRow drNewEst = CusTable.NewRow();
            drNewEst = EstTable.NewRow();
            drNewEst["ColumnID"] = 4;
            drNewEst["ColumnName"] = "Estimate Name";
            EstTable.Rows.Add(drNewEst);
            drNewEst = EstTable.NewRow();
            drNewEst["ColumnID"] = 6;
            drNewEst["ColumnName"] = "Job Start Date";
            EstTable.Rows.Add(drNewEst);
            drNewEst = EstTable.NewRow();
            drNewEst["ColumnID"] = 8;
            drNewEst["ColumnName"] = "Sale Date";
            EstTable.Rows.Add(drNewEst);
            drNewEst = EstTable.NewRow();
            drNewEst["ColumnID"] = 7;
            drNewEst["ColumnName"] = "Status";
            EstTable.Rows.Add(drNewEst);
            drNewEst = EstTable.NewRow();
            drNewEst["ColumnID"] = 10;
            drNewEst["ColumnName"] = "Estimate's Note";
            EstTable.Rows.Add(drNewEst);

            DataRow drNewPay = CusTable.NewRow();
            drNewPay = PayTable.NewRow();
            drNewPay["ColumnID"] = 11;
            drNewPay["ColumnName"] = "Project Total With Tax";
            PayTable.Rows.Add(drNewPay);
            drNewPay = PayTable.NewRow();
            drNewPay["ColumnID"] = 12;
            drNewPay["ColumnName"] = "Special Note";
            PayTable.Rows.Add(drNewPay);
            drNewPay = PayTable.NewRow();
            drNewPay["ColumnID"] = 13;
            drNewPay["ColumnName"] = "Lead Time";
            PayTable.Rows.Add(drNewPay);
            drNewPay = PayTable.NewRow();
            drNewPay["ColumnID"] = 14;
            drNewPay["ColumnName"] = "Anticipated Start Date of project";
            PayTable.Rows.Add(drNewPay);
            drNewPay = PayTable.NewRow();
            drNewPay["ColumnID"] = 15;
            drNewPay["ColumnName"] = "Project Completion Date";
            PayTable.Rows.Add(drNewPay);

            chkCustomer.DataSource = CusTable;
            chkCustomer.DataTextField = "ColumnName";
            chkCustomer.DataValueField = "ColumnID";
            chkCustomer.DataBind();

            chkEstimate.DataSource = EstTable;
            chkEstimate.DataTextField = "ColumnName";
            chkEstimate.DataValueField = "ColumnID";
            chkEstimate.DataBind();

            chkPayments.DataSource = PayTable;
            chkPayments.DataTextField = "ColumnName";
            chkPayments.DataValueField = "ColumnID";
            chkPayments.DataBind();
            lblMessage.Text = "";
        }
    }

    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ColumnID", typeof(int));
        table.Columns.Add("ColumnName", typeof(string));

        return table;
    }

    protected void btnViewReport_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewReport.ID, btnViewReport.GetType().Name, "Click"); 
        if (chkCustomer.SelectedItem == null && chkEstimate.SelectedItem == null && chkPayments.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select Item.");
             
            return;
        }
        else
        {
            lblMessage.Text = "";
        }
        //if (txtSearch.Text == null)
        //{
        //    lblMessage.Text = "Please select Item.";
        //     
        //    return;
        //}

        string searchBy = "";
        string nSearch = txtSearch.Text.ToString();

        if (ddlSearchBy.SelectedItem.Value == "1" && nSearch != "")
        {
            searchBy = "WHERE first_name1 = '" + nSearch + "'";
        }
        if (ddlSearchBy.SelectedItem.Value == "2" && nSearch != "")
        {
            searchBy = "WHERE last_name1 ='" + nSearch + "'";
        }

        //try
        //{
        string sSql = @"SELECT customers.customer_id AS [Customer ID], first_name1+' '+ last_name1 AS [Customer Name], 
                        first_name2+' '+ last_name2 AS [Customer Name2], customers.phone AS [Phone], estimate_name AS [Estimate Name] , 
                        sales_person.first_name+' '+sales_person.last_name AS [Sales Person], ISNULL(job_start_date,'') AS [Job Start Date] , 
                        CASE customer_estimate.status_id WHEN 1 THEN 'Pending' 
                        WHEN 2 THEN 'Sit' WHEN 3 THEN 'Sold' WHEN 4 THEN 'Price Presented' WHEN 5 THEN 'In Design' WHEN 6 THEN 'Rejected'  
                        WHEN 7 THEN 'Closed out' WHEN 8 THEN 'Appointment Cancelled' END AS [Status] , sale_date AS [Sale Date], 
                        status_note AS [Status Note], estimate_comments AS [Estimate's Note], 
                        estimate_payments.new_total_with_tax AS [Project Total With Tax],ISNULL(estimate_payments.special_note,'') AS [Special Note], 
                        estimate_payments.lead_time AS [Lead Time],estimate_payments.[start_date] AS [Anticipated Start Date of project],
                        estimate_payments.completion_date AS [Project Completion Date]                           
                        FROM customers
                        INNER JOIN customer_estimate ON customers.customer_id=customer_estimate.customer_id
                        INNER JOIN sales_person ON customer_estimate.sales_person_id=sales_person.sales_person_id
                        INNER JOIN estimate_payments ON customer_estimate.customer_id=estimate_payments.customer_id AND customer_estimate.estimate_id=estimate_payments.estimate_id
                        " + searchBy +
                    " ORDER BY first_name1";

        DataTable table = GetDataTable(sSql);
        if (table.Rows.Count > 0)
        {

            string CusId = table.Rows[0][0].ToString().Trim();
            string CusName = table.Rows[0][1].ToString().Trim();

            table.Columns.Remove("Customer ID");

            //Remove Unselected Item
            for (int i = 0; i < chkCustomer.Items.Count; i++)
            {
                if (chkCustomer.Items[i].Selected == false)
                {
                    string nColItem = chkCustomer.Items[i].ToString();
                    table.Columns.Remove(nColItem);
                }
            }

            for (int i = 0; i < chkEstimate.Items.Count; i++)
            {
                if (chkEstimate.Items[i].Selected == false)
                {
                    string nColItem = chkEstimate.Items[i].ToString();
                    table.Columns.Remove(nColItem);
                }
            }

            for (int i = 0; i < chkPayments.Items.Count; i++)
            {
                if (chkPayments.Items[i].Selected == false)
                {
                    string nColItem = chkPayments.Items[i].ToString();
                    table.Columns.Remove(nColItem);
                }
            }

            //Excell Report
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                //Customer Name
                if (txtSearch.Text != null && txtSearch.Text != "")
                {
                    string[] str1 = { " " };
                    string[] str2 = { "Customer Name: " + CusName };
                    writer.WriteRecord(str1, true);
                    writer.WriteRecord(str2, true);
                }

                string[] str3 = { " " };
                writer.WriteRecord(str3, true);
                writer.WriteAll(table, true);
            }

            //Report Name By Search
            string rpt = "Report";
            if (txtSearch.Text != null && txtSearch.Text != "")
            {
                rpt = "Report" + CusId;
            }

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + rpt + ".csv");
            lblMessage.Text = "";
            Response.End();
        }
        else
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("No Data Found.");
             
            return;
        }
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }

    public DataTable GetDataTable(string strQ)
    {
        DataTable table = new DataTable();
        SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CRMDBConnectionString"].ConnectionString);

        try
        {
            SqlCommand cmd = new SqlCommand(strQ, con);
            con.Open();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            table = ds.Tables[0];
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

            con.Close();
            con.Dispose();
        }
        return table;
    }
    #region
   
    #endregion

}
