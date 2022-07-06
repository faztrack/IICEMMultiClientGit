using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Configuration;
using System.Data;

public partial class customerbalancereport : System.Web.UI.Page
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
                userinfo oUser = (userinfo)Session["oUser"];
                hdnClientId.Value = oUser.client_id.ToString();
                hdnDivisionName.Value = oUser.divisionName;
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
            }
            if (Page.User.IsInRole("br01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindDivision();

            if (hdnDivisionName.Value != "" && hdnDivisionName.Value.Contains(","))
            {
                pnlDivision.Visible = true;
            }
            else
            {
                pnlDivision.Visible = false;
            }
        }

       
    }

    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            ddlDivision.DataSource = dt;
            ddlDivision.DataTextField = "division_name";
            ddlDivision.DataValueField = "id";
            ddlDivision.DataBind();
            ddlDivision.Items.Insert(0, "All");
            ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
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
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sale Start Date is a required field");
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
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Sale Start Date");
                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sale End Date is a required field");
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
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Sale End Date");
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
         strCondition = " CONVERT(DATETIME,ce.sale_date) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "' ";

        

        if (ddlDivision.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 2)
            {
                strCondition += " AND c.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }
            else
            {
                strCondition += " WHERE c.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }
        }

        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = "select c.customer_id,ce.estimate_id,c.first_name1 + ' ' + c.last_name1 as customername,u.first_name + ' ' + u.last_name as supername,s.first_name + ' ' + s.last_name as salesperson,ce.sale_date,ce.job_number, " +
            " CASE  WHEN c.status_id = 2  THEN 'Active' "+
            " WHEN c.status_id = 4 THEN 'Archived' " +
            " WHEN c.status_id  = 5 THEN 'InActive' " +
            " ELSE 'Warranty Only' END as status" + 
            " from customers as c" +
              " LEFT OUTER JOIN customer_estimate as ce on ce.customer_id = c.customer_id" +
              " LEFT OUTER JOIN sales_person as s on s.sales_person_id = c.sales_person_id" +
              " LEFT OUTER JOIN user_info as u on u.user_id = c.SuperintendentId" +
              " where "+ strCondition + " order by CONVERT(DATETIME,ce.sale_date) asc ";

           DataTable dtReport = csCommonUtility.GetDataTable(strQ);
        if (dtReport.Rows.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No records found.");
            return;
        }
        string SourceFilePath = Server.MapPath(@"Reports") + @"\";
        string FilePath = Server.MapPath(@"Reports\excel_report") + @"\";
        string sFileName = "CustomerBalanceReport" + DateTime.Now.Ticks.ToString() + ".xlsx";

        FileInfo tempFile = new FileInfo(SourceFilePath + "CustomerBalanceReport.xlsx");

        tempFile.CopyTo(FilePath + sFileName);

        FileInfo newFile = new FileInfo(FilePath + sFileName);

        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            // Add a worksheet to the empty workbook for Pickup Orders
            ExcelWorksheet worksheet0 = package.Workbook.Worksheets["Customer Balance Report"];

            worksheet0.Cells["B2"].Value = "Sale Date Range: " + txtStartDate.Text + " to " + txtEndDate.Text;
            worksheet0.Cells["B2"].Style.Font.Bold = true;
            worksheet0.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            int nRowCount = 8;

            foreach (DataRow dr in dtReport.Rows)
            {
                decimal TotalContactAmount = 0;
                decimal TotalCoAmount = 0;
                decimal TotalPaidAmount = 0;
                TotalContactAmount = getTotalContactAmount(Convert.ToInt32(dr["customer_id"].ToString()), Convert.ToInt32(dr["estimate_id"].ToString()));
                TotalCoAmount= getTotalCOAmount(Convert.ToInt32(dr["customer_id"].ToString()), Convert.ToInt32(dr["estimate_id"].ToString()));
                TotalPaidAmount=getTotalPaidAmount(Convert.ToInt32(dr["customer_id"].ToString()), Convert.ToInt32(dr["estimate_id"].ToString()));

                worksheet0.Cells[nRowCount, 2].Value = dr["customername"].ToString();
                worksheet0.Cells[nRowCount, 3].Value = dr["job_number"].ToString();
                worksheet0.Cells[nRowCount, 4].Value = dr["sale_date"].ToString();
                worksheet0.Cells[nRowCount, 5].Value = dr["salesperson"].ToString();
                worksheet0.Cells[nRowCount, 6].Value = dr["supername"].ToString();
                worksheet0.Cells[nRowCount, 7].Value = TotalContactAmount; //Sold Amount
                worksheet0.Cells[nRowCount, 8].Value = TotalCoAmount;  // Change Order Amount
                worksheet0.Cells[nRowCount, 9].Value = TotalContactAmount+TotalCoAmount; //Total Amount
                worksheet0.Cells[nRowCount, 10].Value = TotalPaidAmount; //Paid Amount
                worksheet0.Cells[nRowCount, 11].Value = TotalContactAmount + TotalCoAmount-TotalPaidAmount; //Due Balance 
                worksheet0.Cells[nRowCount, 12].Value = dr["status"].ToString();
                nRowCount++;

            }
            worksheet0.Cells["A:L"].AutoFitColumns();
            package.Save();
          

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/excel_report/" + sFileName + "');", true);
        }
    }

    private static decimal getTotalContactAmount(int custId, int estId)
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string sSql = "SELECT new_total_with_tax,adjusted_price,project_subtotal,adjusted_tax_amount,tax_amount FROM estimate_payments " +
            " WHERE customer_id = " + custId + " AND estimate_id = " + estId;

        DataTable table =csCommonUtility.GetDataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
        {
            totalwithtax = Convert.ToDecimal(table.Rows[0]["new_total_with_tax"]);
            if (Convert.ToDecimal(table.Rows[0]["adjusted_price"]) > 0)
                project_subtotal = Convert.ToDecimal(table.Rows[0]["adjusted_price"]);
            else
                project_subtotal = Convert.ToDecimal(table.Rows[0]["project_subtotal"]);
            if (Convert.ToDecimal(table.Rows[0]["adjusted_tax_amount"]) > 0)
                tax_amount = Convert.ToDecimal(table.Rows[0]["adjusted_tax_amount"]);
            else
                tax_amount = Convert.ToDecimal(table.Rows[0]["tax_amount"]);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            return totalwithtax;
        }

    }
    private static decimal getTotalCOAmount(int custId, int estId)
    {
        decimal TotalCOAmount = 0;
        string sSql = "SELECT  chage_order_id, tax FROM changeorder_estimate " +
        " WHERE customer_id = " + custId + " AND estimate_id = " + estId + " AND change_order_status_id = 3 ";

        DataTable table = csCommonUtility.GetDataTable(sSql);
        if (table.Rows.Count > 0)
        {
            int chage_order_id = 0;
            decimal taxRate = 0;
            foreach (DataRow dr in table.Rows)
            {
                chage_order_id = Convert.ToInt32(dr["chage_order_id"]);
                taxRate = Convert.ToDecimal(dr["tax"]);
                decimal COAmount = GetCOAmount(custId, estId, chage_order_id);
                decimal CoTax = COAmount * (taxRate / 100);
                decimal CoPrice = 0;

                if (CoTax > 0)
                {
                    CoPrice = COAmount + CoTax;

                }
                else
                {
                    CoPrice = COAmount;
                }
                TotalCOAmount += CoPrice;
            }

        }
        return TotalCOAmount;
    }
    private static decimal GetCOAmount(int custId, int estId, int coId)
    {
        string sSql = "SELECT Isnull(sum(EconomicsCost),0) as CoTotal  FROM change_order_pricing_list " +
                      " WHERE customer_id = " + custId + " AND estimate_id = " + estId + " AND chage_order_id = " + coId;

        DataTable table = csCommonUtility.GetDataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
            return Convert.ToDecimal(table.Rows[0]["CoTotal"]);
    }
    private static decimal getTotalPaidAmount(int custId, int estId)
    {
        string sSql = "SELECT Isnull(sum(pay_amount),0) as RecivedAmount  FROM New_partial_payment " +
                       " WHERE customer_id = " + custId + " AND estimate_id = " + estId;
        DataTable table = csCommonUtility.GetDataTable(sSql);
        if (table.Rows.Count == 0)
            return 0;
        else
            return Convert.ToDecimal(table.Rows[0]["RecivedAmount"]);
    }





    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}