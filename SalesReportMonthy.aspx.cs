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


using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;


public partial class SalesReportMonthy : System.Web.UI.Page
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
                hdnDivisionName.Value = oUser.divisionName.ToString();
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();                
            }
            if (Page.User.IsInRole("rpt006") == false)
            {
                // No Permission Page.
                //Response.Redirect("nopermission.aspx");
            }

            BindDivision();
            BindSalesPersons();

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
   
   
    private void BindSalesPersons()
    {
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_name asc";

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
            ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }



    protected void btnViewExcel_Click(object sender, EventArgs e)
    {
        
        lblResult.Text = "";

        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (rdbReportType.SelectedValue == "1")
        {

            if (txtStartDate.Text == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sold Start Date is a required field");
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
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Sold Start Date");
                    return;
                }
                strStartDate = Convert.ToDateTime(txtStartDate.Text);
            }

            if (txtEndDate.Text == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sold End Date is a required field");
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
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Sold End Date");
                    return;
                }
                strEndDate = Convert.ToDateTime(txtEndDate.Text);
            }
            if (strStartDate > strEndDate)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Sold Date Range");
                return;
            }

            string strCondition = string.Empty;

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition += " and CONVERT(DATETIME,customer_estimate.sale_date) >= '" + dt1 + "' and CONVERT(DATETIME,customer_estimate.sale_date) <'" + dt2.AddDays(1) + "'";
            }

            if (ddlSalesPersons.SelectedItem.Text != "All")
            {
                strCondition += " AND customers.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue);
            }

           
            
            if (ddlEstimateStatus.SelectedItem.Text != "All")
            {
                strCondition += " AND customer_estimate.IsEstimateActive =" + Convert.ToInt32(ddlEstimateStatus.SelectedValue);
            }
            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {

                    strCondition += " AND customers.status_id NOT IN(4,5,7)  ";
                }
               else
                {
                   strCondition += " AND customers.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                }

            }


            if (strCondition.Length > 2)
            {
                strCondition += " AND customers.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }
            else
            {
                strCondition += " WHERE customers.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }



            DataClassesDataContext _db = new DataClassesDataContext();


            string strQ = @"SELECT Month=Datename(M,customer_estimate.sale_date),  Year=Datepart(Year,customer_estimate.sale_date)
                        FROM customers INNER JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id LEFT OUTER JOIN
                        (SELECT  customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                        FROM   pricing_details AS pd
                         GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                         LEFT OUTER JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id

                        
                         WHERE(customer_estimate.status_id = 3) " + strCondition + "  order by CONVERT(DATETIME, customer_estimate.sale_date)";




            DataTable dtMaster = csCommonUtility.GetDataTable(strQ);

            if (dtMaster.Rows.Count == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found.");
                return;
            }
            int Total=0;
            strQ = @"SELECT Month=Datename(M,customer_estimate.sale_date),  Year=Datepart(Year,customer_estimate.sale_date),customers.sales_person_id
                        FROM customers INNER JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id LEFT OUTER JOIN
                             (SELECT  customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                               FROM            pricing_details AS pd
                               GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                            LEFT OUTER JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id

                         WHERE(customer_estimate.status_id = 3) " + strCondition + " group by Datename(M, customer_estimate.sale_date),Datepart(Year, customer_estimate.sale_date),customers.sales_person_id ";




            DataTable dtSubMaster = csCommonUtility.GetDataTable(strQ);

            strQ = @"SELECT Month=Datename(M,customer_estimate.sale_date),  Year=Datepart(Year,customer_estimate.sale_date), ISNULL(t1.Total_Price, 0) AS Total_Price,  
                         customer_estimate.estimate_name, customer_estimate.sale_date, customers.last_name1 + ' ' + customers.first_name1 AS CustomerName
                         ,lead_source.lead_name,customers.sales_person_id,sales_person.first_name + ' ' + last_name AS SalesRep,
    				     case when customer_estimate.alter_job_number != '' then customer_estimate.alter_job_number
                         else   customer_estimate.job_number end as job_number,
                         ISNULL(project_subtotal,0) as project_subtotal,  ISNULL(estimate_payments.tax_rate,0) as tax_rate, ISNULL(tax_amount,0) as tax_amount, 
						 ISNULL(total_with_tax, 0) as  total_with_tax,ISNULL(adjusted_price,0) as adjusted_price, ISNULL(adjusted_tax_rate,0) as adjusted_tax_rate , 
						 ISNULL(adjusted_tax_amount,0) as adjusted_tax_amount, ISNULL( new_total_with_tax, 0) as new_total_with_tax, 
						  ISNULL(total_incentives,0) as total_incentives 
                          
                         FROM customers INNER JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id LEFT OUTER JOIN
                             (SELECT        customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                               FROM            pricing_details AS pd
                               GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                          LEFT OUTER JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id
                       
                        WHERE(customer_estimate.status_id = 3) " + strCondition + "  order by CONVERT(DATETIME, customer_estimate.sale_date)";


            DataTable dtReport = csCommonUtility.GetDataTable(strQ);
            Total = dtReport.Rows.Count;
            string SourceFilePath = Server.MapPath(@"Reports") + @"\";
            string FilePath = Server.MapPath(@"Reports\excel_report") + @"\";
            string sFileName = "SalesReportSoldDateBased" + DateTime.Now.Ticks.ToString() + ".xlsx";

            FileInfo tempFile = new FileInfo(SourceFilePath + "SalesReportSoldDateBased.xlsx");
            
            tempFile.CopyTo(FilePath + sFileName);

            FileInfo newFile = new FileInfo(FilePath + sFileName);





            using (ExcelPackage package = new ExcelPackage(newFile))
            {

                ExcelWorksheet worksheet0 = package.Workbook.Worksheets["Sales Report"];

                worksheet0.Cells["B3"].Value = "Sold Date Range: " + txtStartDate.Text + " to " + txtEndDate.Text;
                worksheet0.Cells["B3"].Style.Font.Bold = true;
                worksheet0.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet0.Cells["E3"].Value = "Total: "+ Total;
                worksheet0.Cells["E3"].Style.Font.Bold = true;
                worksheet0.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //var SalesRep = (from r in dtMaster.AsEnumerable()
                //                  orderby r.Field<int>("Year") ascending
                //                    select r);
                var sMonth = (from r in dtMaster.AsEnumerable()
                              select new
                              {
                                  Month = r.Field<string>("Month"),
                                  Year = r.Field<int>("Year")
                              }).Distinct();

                var SalesRep = (from r in dtSubMaster.AsEnumerable()
                                select new
                                {
                                    sales_person_id = r.Field<int>("sales_person_id")
                                }).Distinct();

                int nRowCount = 6;

                decimal nGrandTotalProject = 0;
                decimal nGrandTotalWithTax = 0;
                decimal nGrandTaxAmount = 0;

                foreach (var p in sMonth)
                {
                    string nMonth = p.Month;
                    int nYear = p.Year;
                    DataView dv = dtSubMaster.DefaultView;
                    dv.RowFilter = "Month='" + nMonth + "' and Year=" + nYear;
                    worksheet0.Cells[nRowCount, 1].Value = nMonth.ToString() + "   " + nYear.ToString();
                    worksheet0.Cells[nRowCount, 1].Style.Font.Bold = true;
                    decimal nTotalProject = 0;
                    decimal nTotalWithTax = 0;
                    decimal nTotalTaxAmount = 0;
                    foreach (var sp in SalesRep)
                    {
                        decimal nSubProjecttotal = 0;
                        decimal nSubtotalwithtax = 0;
                        decimal nSubTaxAmount = 0;

                        DataView dv2 = dv.ToTable().DefaultView;
                        dv2.RowFilter = "sales_person_id=" + sp.sales_person_id;

                        foreach (DataRow dr2 in dv2.ToTable().Rows)
                        {

                            foreach (DataRow dr in dtReport.Rows)
                            {
                                string Month = dr["Month"].ToString();
                                int sales_person_id = Convert.ToInt32(dr["sales_person_id"].ToString());
                                int Year = Convert.ToInt32(dr["Year"].ToString());
                                if (Month == dr2["Month"].ToString() && Year == Convert.ToInt32(dr2["Year"].ToString()) && sales_person_id == Convert.ToInt32(dr2["sales_person_id"].ToString()))
                                {

                                    decimal totalwithtax = 0;
                                    decimal project_subtotal = 0;
                                    decimal tax_amount = 0;
                                    decimal tax_rate = 0;
                                    decimal total_incentives = 0;
                                    totalwithtax = Convert.ToDecimal(dr["new_total_with_tax"]);
                                    total_incentives = Convert.ToDecimal(dr["total_incentives"]);
                                    if (Convert.ToDecimal(dr["adjusted_price"]) > 0)
                                        project_subtotal = Convert.ToDecimal(dr["adjusted_price"]);
                                    else
                                        project_subtotal = Convert.ToDecimal(dr["project_subtotal"]);
                                    if (Convert.ToDecimal(dr["adjusted_tax_amount"]) > 0)
                                        tax_amount = Convert.ToDecimal(dr["adjusted_tax_amount"]);
                                    else
                                        tax_amount = Convert.ToDecimal(dr["tax_amount"]);

                                    if (Convert.ToDecimal(dr["adjusted_tax_rate"]) > 0)
                                        tax_rate = Convert.ToDecimal(dr["adjusted_tax_rate"]);
                                    else
                                        tax_rate = Convert.ToDecimal(dr["tax_rate"]);

                                    if (totalwithtax == 0)
                                        totalwithtax = project_subtotal + tax_amount;

                                    worksheet0.Cells[nRowCount, 2].Value = dr["SalesRep"].ToString();
                                    worksheet0.Cells[nRowCount, 3].Value = dr["job_number"].ToString();
                                    worksheet0.Cells[nRowCount, 4].Value = dr["CustomerName"].ToString();
                                    worksheet0.Cells[nRowCount, 5].Value = dr["estimate_name"].ToString();
                                    


                                    worksheet0.Cells[nRowCount, 6].Value = dr["sale_date"].ToString();
                                    worksheet0.Cells[nRowCount, 7].Value = dr["lead_name"].ToString();
                                    worksheet0.Cells[nRowCount, 8].Value = Convert.ToDecimal(project_subtotal).ToString("c"); //;Convert.ToDecimal(dr["NewTotal_Price"]).ToString("c");
                                    worksheet0.Cells[nRowCount, 9].Value = tax_rate+" %";
                                    worksheet0.Cells[nRowCount, 10].Value = Convert.ToDecimal(tax_amount).ToString("c");
                                    worksheet0.Cells[nRowCount, 11].Value = Convert.ToDecimal(totalwithtax).ToString("c");

                                    nSubProjecttotal += project_subtotal;
                                    nSubTaxAmount += tax_amount;
                                    nSubtotalwithtax += totalwithtax;
                                  
                                    nRowCount++;
                                }
                            }


                            nGrandTotalProject += nSubProjecttotal;
                            nTotalProject += nSubProjecttotal;
                            nGrandTaxAmount += nSubTaxAmount;
                            nTotalTaxAmount += nSubTaxAmount;
                            nGrandTotalWithTax += nSubtotalwithtax;
                            nTotalWithTax += nSubtotalwithtax;

                            worksheet0.Cells[nRowCount, 2].Value = "";
                            worksheet0.Cells[nRowCount, 3].Value = "";
                            worksheet0.Cells[nRowCount, 4].Value = "";
                            worksheet0.Cells[nRowCount, 5].Value = "";
                            worksheet0.Cells[nRowCount, 6].Value = "";
                            
                            worksheet0.Cells[nRowCount, 7].Value = "Sub Total:";
                            worksheet0.Cells[nRowCount, 7].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet0.Cells[nRowCount, 8].Value = nSubProjecttotal.ToString("c");
                            worksheet0.Cells[nRowCount, 9].Value = "";
                            worksheet0.Cells[nRowCount, 10].Value = "";
                            worksheet0.Cells[nRowCount, 10].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 10].Value = nSubTaxAmount.ToString("c");
                            worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 11].Value = nSubtotalwithtax.ToString("c");
                          
                            nRowCount++;
                        }




                    }

                    // nRowCount++;
                    worksheet0.Cells[nRowCount, 2].Value = "";
                    worksheet0.Cells[nRowCount, 3].Value = "";
                    worksheet0.Cells[nRowCount, 4].Value = "";
                    worksheet0.Cells[nRowCount, 5].Value = "";
                    
                    worksheet0.Cells[nRowCount, 6].Value = "";
                    worksheet0.Cells[nRowCount, 7].Value = nMonth + "  Total:";
                    worksheet0.Cells[nRowCount, 7].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet0.Cells[nRowCount, 8].Value = nTotalProject.ToString("c");
                    worksheet0.Cells[nRowCount, 8].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 9].Value = "";
                    worksheet0.Cells[nRowCount, 10].Value = nTotalTaxAmount.ToString("c");
                    worksheet0.Cells[nRowCount, 10].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 11].Value = nTotalWithTax.ToString("c");
                    worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                    nRowCount++;

                }
                nRowCount++;
                nRowCount++;
                worksheet0.Cells[nRowCount, 2].Value = "";
                worksheet0.Cells[nRowCount, 3].Value = "";
                worksheet0.Cells[nRowCount, 4].Value = "";
                worksheet0.Cells[nRowCount, 5].Value = "";
                
                worksheet0.Cells[nRowCount, 6].Value = "";
               
                worksheet0.Cells[nRowCount, 7].Value = "Grand Total:";
                worksheet0.Cells[nRowCount, 7].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet0.Cells[nRowCount, 8].Value = nGrandTotalProject.ToString("c");
                worksheet0.Cells[nRowCount, 9].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 9].Value = "";
                worksheet0.Cells[nRowCount, 10].Value = nGrandTaxAmount.ToString("c");
                worksheet0.Cells[nRowCount, 10].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 11].Value = nGrandTotalWithTax.ToString("c");
                worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                //nRowCount++;
                package.Save();

                lblResult.Text = csCommonUtility.GetSystemMessage("Your report is being downloaded.  Please check your browser's download folder.");

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/excel_report/" + sFileName + "');", true);
            }
        }
        else
        {

            // Lend Entry Date Base
            if (txtStartDate.Text == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Lead Entry Start Date is a required field");
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
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Lead Entry Start Date");
                    return;
                }
                strStartDate = Convert.ToDateTime(txtStartDate.Text);
            }

            if (txtEndDate.Text == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Lead Entry End Date is a required field");
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
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Lead Entry End Date");
                    return;
                }
                strEndDate = Convert.ToDateTime(txtEndDate.Text);
            }
            if (strStartDate > strEndDate)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Lead Entry Date Range");
                return;
            }

            string strCondition = string.Empty;

            if (txtStartDate.Text.Trim() != "" && txtEndDate.Text.Trim() != "")
            {

                DateTime dt1 = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition= "  CONVERT(DATETIME,customers.registration_date) >= '" + dt1 + "' and CONVERT(DATETIME,customers.registration_date) <'" + dt2.AddDays(1) + "'";
            }

            if (ddlSalesPersons.SelectedItem.Text != "All")
            {
                if(strCondition.Length>0)
                   strCondition += " AND customers.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue);
                else
                    strCondition= " customers.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue);
            }

          
            if (ddlEstimateStatus.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 0)
                    strCondition += " AND customer_estimate.IsEstimateActive =" + Convert.ToInt32(ddlEstimateStatus.SelectedValue);
                else
                    strCondition = " customer_estimate.IsEstimateActive =" + Convert.ToInt32(ddlEstimateStatus.SelectedValue);
            }

            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND customers.status_id NOT IN(4,5,7) ";
                    else
                        strCondition += " customers.status_id NOT IN(4,5,7) ";
                }
               
                else
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND customers.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                   else
                        strCondition += " customers.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);

                }

            }

            if (strCondition.Length>0)
            {
                strCondition = " WHERE " + strCondition;
            }


            if (strCondition.Length > 2)
            {
                strCondition += " AND customers.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }
            else
            {
                strCondition += " WHERE customers.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
            }

            DataClassesDataContext _db = new DataClassesDataContext();


            string strQ = @"SELECT Month=Datename(M,customers.registration_date),  Year=Datepart(Year,customers.registration_date)
                        FROM customers 
                         LEFT JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id
                         INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id
                         INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id LEFT OUTER JOIN
                        (SELECT  customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                        FROM   pricing_details AS pd
                         GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                         LEFT OUTER JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id

                       
                          " + strCondition + "  order by CONVERT(DATETIME, customers.registration_date)";




            DataTable dtMaster = csCommonUtility.GetDataTable(strQ);

            if (dtMaster.Rows.Count == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found.");
                return;
            }

            int Total = 0;

            strQ = @"SELECT Month=Datename(M,customers.registration_date),  Year=Datepart(Year,customers.registration_date),customers.sales_person_id
                        FROM customers
                        LEFT JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id 
                         INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id 
                        INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id LEFT OUTER JOIN
                             (SELECT  customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                               FROM            pricing_details AS pd
                               GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                            LEFT JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id

                        
                          " + strCondition + " group by Datename(M, customers.registration_date),Datepart(Year, customers.registration_date),customers.sales_person_id ";




            DataTable dtSubMaster = csCommonUtility.GetDataTable(strQ);

            strQ = @"SELECT Month=Datename(M,customers.registration_date),  Year=Datepart(Year,customers.registration_date), ISNULL(t1.Total_Price, 0) AS Total_Price, ISNULL(estimate_payments.new_total_with_tax, 0) AS NewTotal_Price, 
                         customer_estimate.estimate_name, customer_estimate.sale_date, customers.last_name1 + ' ' + customers.first_name1 AS CustomerName,
                         lead_source.lead_name,customers.sales_person_id,sales_person.first_name + ' ' + last_name AS SalesRep,
    				     case when customer_estimate.alter_job_number != '' then customer_estimate.alter_job_number
                         else   customer_estimate.job_number end as job_number,customers.registration_date,
                          ISNULL(project_subtotal,0) as project_subtotal,  ISNULL(estimate_payments.tax_rate,0) as tax_rate, ISNULL(tax_amount,0) as tax_amount, 
						 ISNULL(total_with_tax, 0) as  total_with_tax,ISNULL(adjusted_price,0) as adjusted_price, ISNULL(adjusted_tax_rate,0) as adjusted_tax_rate , 
						 ISNULL(adjusted_tax_amount,0) as adjusted_tax_amount, ISNULL( new_total_with_tax, 0) as new_total_with_tax, 
						  ISNULL(total_incentives,0) as total_incentives, isnull(customer_estimate.status_id,0) as  eststatus_id
                         FROM customers
                         LEFT JOIN
                         customer_estimate ON customer_estimate.customer_id = customers.customer_id 
                        INNER JOIN
                         lead_source ON lead_source.lead_source_id = customers.lead_source_id 
                         INNER JOIN
                         sales_person ON sales_person.sales_person_id = customers.sales_person_id
                          LEFT OUTER JOIN
                             (SELECT        customer_id, estimate_id, ISNULL(SUM(total_retail_price), 0) AS Total_Price
                               FROM            pricing_details AS pd
                               GROUP BY customer_id, estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id
                          LEFT JOIN
                         estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id
                      
                         " + strCondition + " order by customers.registration_date ";


            DataTable dtReport = csCommonUtility.GetDataTable(strQ);
            Total = dtReport.Rows.Count;
            string SourceFilePath = Server.MapPath(@"Reports") + @"\";
            string FilePath = Server.MapPath(@"Reports\excel_report") + @"\";
            string sFileName = "SalesReportLeadEntryBased" + DateTime.Now.Ticks.ToString() + ".xlsx";

            FileInfo tempFile = new FileInfo(SourceFilePath + "SalesReportLeadEntryBased.xlsx");

            tempFile.CopyTo(FilePath + sFileName);

            FileInfo newFile = new FileInfo(FilePath + sFileName);





            using (ExcelPackage package = new ExcelPackage(newFile))
            {

                ExcelWorksheet worksheet0 = package.Workbook.Worksheets["Sales Report"];

                worksheet0.Cells["B3"].Value = "Lead Enty Date Range: " + txtStartDate.Text + " to " + txtEndDate.Text;
                worksheet0.Cells["B3"].Style.Font.Bold = true;
                worksheet0.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet0.Cells["E3"].Value = "Total: " + Total;
                worksheet0.Cells["E3"].Style.Font.Bold = true;
                worksheet0.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Sold  Count
                int soldCount = 0;
                 soldCount = (from r in dtReport.AsEnumerable()  where r.Field<int>("eststatus_id") ==3  select r).ToList().Count();
                        

                worksheet0.Cells["F3"].Value = "Sold: " + soldCount;
                worksheet0.Cells["F3"].Style.Font.Bold = true;
                worksheet0.Cells["F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // UnSold  Count
                worksheet0.Cells["G3"].Value = "Unsold: " + (Total- soldCount);
                worksheet0.Cells["G3"].Style.Font.Bold = true;
                worksheet0.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


               
                var sMonth = (from r in dtMaster.AsEnumerable()
                              select new
                              {
                                  Month = r.Field<string>("Month"),
                                  Year = r.Field<int>("Year")
                              }).Distinct();

                var SalesRep = (from r in dtSubMaster.AsEnumerable()
                                select new
                                {
                                    sales_person_id = r.Field<int>("sales_person_id")
                                }).Distinct();

                int nRowCount = 6;

                decimal nGrandTotalProject = 0;
                decimal nGrandTotalWithTax = 0;
                decimal nGrandTaxAmount = 0;

                foreach (var p in sMonth)
                {
                    string nMonth = p.Month;
                    int nYear = p.Year;
                    DataView dv = dtSubMaster.DefaultView;
                    dv.RowFilter = "Month='" + nMonth + "' and Year=" + nYear;
                    worksheet0.Cells[nRowCount, 1].Value = nMonth.ToString() + "   " + nYear.ToString();
                    worksheet0.Cells[nRowCount, 1].Style.Font.Bold = true;
                    decimal nTotalProject = 0;
                    decimal nTotalWithTax = 0;
                    decimal nTotalTaxAmount = 0;
                    foreach (var sp in SalesRep)
                    {
                        decimal nSubProjecttotal = 0;
                        decimal nSubtotalwithtax = 0;
                        decimal nSubTaxAmount = 0;

                        DataView dv2 = dv.ToTable().DefaultView;
                        dv2.RowFilter = "sales_person_id=" + sp.sales_person_id;

                        foreach (DataRow dr2 in dv2.ToTable().Rows)
                        {

                            foreach (DataRow dr in dtReport.Rows)
                            {
                                string Month = dr["Month"].ToString();
                                int sales_person_id = Convert.ToInt32(dr["sales_person_id"].ToString());
                                int Year = Convert.ToInt32(dr["Year"].ToString());
                                if (Month == dr2["Month"].ToString() && Year == Convert.ToInt32(dr2["Year"].ToString()) && sales_person_id == Convert.ToInt32(dr2["sales_person_id"].ToString()))
                                {

                                    decimal totalwithtax = 0;
                                    decimal project_subtotal = 0;
                                    decimal tax_amount = 0;
                                    decimal tax_rate = 0;
                                    decimal total_incentives = 0;
                                    totalwithtax = Convert.ToDecimal(dr["new_total_with_tax"]);
                                    total_incentives = Convert.ToDecimal(dr["total_incentives"]);
                                    if (Convert.ToDecimal(dr["adjusted_price"]) > 0)
                                        project_subtotal = Convert.ToDecimal(dr["adjusted_price"]);
                                    else
                                        project_subtotal = Convert.ToDecimal(dr["project_subtotal"]);
                                    if (Convert.ToDecimal(dr["adjusted_tax_amount"]) > 0)
                                        tax_amount = Convert.ToDecimal(dr["adjusted_tax_amount"]);
                                    else
                                        tax_amount = Convert.ToDecimal(dr["tax_amount"]);

                                    if (Convert.ToDecimal(dr["adjusted_tax_rate"]) > 0)
                                        tax_rate = Convert.ToDecimal(dr["adjusted_tax_rate"]);
                                    else
                                        tax_rate = Convert.ToDecimal(dr["tax_rate"]);

                                    if (totalwithtax == 0)
                                        totalwithtax = project_subtotal + tax_amount;






                                    worksheet0.Cells[nRowCount, 2].Value = dr["SalesRep"].ToString();
                                    worksheet0.Cells[nRowCount, 3].Value = dr["job_number"].ToString();
                                    worksheet0.Cells[nRowCount, 4].Value = dr["CustomerName"].ToString();
                                    worksheet0.Cells[nRowCount, 5].Value = dr["estimate_name"].ToString();
                                

                                    
                                    worksheet0.Cells[nRowCount, 6].Value =Convert.ToDateTime(dr["registration_date"]).ToString("MM/dd/yyyy");
                                    if(dr["sale_date"].ToString().Length>3)
                                      worksheet0.Cells[nRowCount, 7].Value =Convert.ToDateTime(dr["sale_date"]).ToString("MM/dd/yyyy");

                                    worksheet0.Cells[nRowCount, 8].Value = dr["lead_name"].ToString();
                                    worksheet0.Cells[nRowCount, 9].Value = Convert.ToDecimal(project_subtotal).ToString("c"); //;Convert.ToDecimal(dr["NewTotal_Price"]).ToString("c");
                                    worksheet0.Cells[nRowCount, 10].Value = tax_rate + " %";
                                    worksheet0.Cells[nRowCount, 11].Value = Convert.ToDecimal(tax_amount).ToString("c");
                                    worksheet0.Cells[nRowCount, 12].Value = Convert.ToDecimal(totalwithtax).ToString("c");

                                    nSubProjecttotal += project_subtotal;
                                    nSubTaxAmount += tax_amount;
                                    nSubtotalwithtax += totalwithtax;
                                    nRowCount++;
                                }
                            }


                            nGrandTotalProject += nSubProjecttotal;
                            nTotalProject += nSubProjecttotal;
                            nGrandTaxAmount += nSubTaxAmount;
                            nTotalTaxAmount += nSubTaxAmount;
                            nGrandTotalWithTax += nSubtotalwithtax;
                            nTotalWithTax += nSubtotalwithtax;

                            worksheet0.Cells[nRowCount, 2].Value = "";
                            worksheet0.Cells[nRowCount, 3].Value = "";
                            worksheet0.Cells[nRowCount, 4].Value = "";
                            worksheet0.Cells[nRowCount, 5].Value = "";
                            
                            worksheet0.Cells[nRowCount, 6].Value = "";
                            worksheet0.Cells[nRowCount, 7].Value = "";
                            worksheet0.Cells[nRowCount, 8].Value = "Sub Total:";
                            worksheet0.Cells[nRowCount, 8].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet0.Cells[nRowCount, 9].Value = nSubProjecttotal.ToString("c");
                            worksheet0.Cells[nRowCount, 10].Value = "";
                            worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 11].Value = nSubTaxAmount.ToString("c");
                            worksheet0.Cells[nRowCount, 12].Style.Font.Bold = true;
                            worksheet0.Cells[nRowCount, 12].Value = nSubtotalwithtax.ToString("c");
                            nRowCount++;
                        }




                    }

                    // nRowCount++;
                    worksheet0.Cells[nRowCount, 2].Value = "";
                    worksheet0.Cells[nRowCount, 3].Value = "";
                    worksheet0.Cells[nRowCount, 4].Value = "";
                    worksheet0.Cells[nRowCount, 5].Value = "";
                   
                    worksheet0.Cells[nRowCount, 6].Value = "";
                    worksheet0.Cells[nRowCount, 7].Value = "";
                   

                    worksheet0.Cells[nRowCount, 8].Value = nMonth + "  Total:";
                    worksheet0.Cells[nRowCount, 8].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet0.Cells[nRowCount, 9].Value = nTotalProject.ToString("c");
                    worksheet0.Cells[nRowCount, 9].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 10].Value = "";
                    worksheet0.Cells[nRowCount, 11].Value = nTotalTaxAmount.ToString("c");
                    worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                    worksheet0.Cells[nRowCount, 12].Value = nTotalWithTax.ToString("c");
                    worksheet0.Cells[nRowCount, 12].Style.Font.Bold = true;
                    nRowCount++;

                }
                nRowCount++;
                nRowCount++;
                worksheet0.Cells[nRowCount, 2].Value = "";
                worksheet0.Cells[nRowCount, 3].Value = "";
                worksheet0.Cells[nRowCount, 4].Value = "";
                worksheet0.Cells[nRowCount, 5].Value = "";
               
                worksheet0.Cells[nRowCount, 6].Value = "";
                worksheet0.Cells[nRowCount, 7].Value = "";
                

                worksheet0.Cells[nRowCount, 8].Value = "Grand Total:";
                worksheet0.Cells[nRowCount, 8].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet0.Cells[nRowCount, 9].Value = nGrandTotalProject.ToString("c");
                worksheet0.Cells[nRowCount, 9].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 10].Value = "";
                worksheet0.Cells[nRowCount, 11].Value = nGrandTaxAmount.ToString("c");
                worksheet0.Cells[nRowCount, 11].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 12].Value = nGrandTotalWithTax.ToString("c");
                worksheet0.Cells[nRowCount, 12].Style.Font.Bold = true;

                package.Save();

                lblResult.Text = csCommonUtility.GetSystemMessage("Your report is being downloaded.  Please check your browser's download folder.");

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/excel_report/" + sFileName + "');", true);
            }
        }
    }

    protected void rdbReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
        if(rdbReportType.SelectedValue=="1")
        {
            lblStartDate.Text = "Sold Start Date:";
            lblEndDate.Text = "Sold End Date:";
        }
        else
        {
            lblStartDate.Text = "Lead Entry Start Date:";
            lblEndDate.Text = "Lead Entry End Date:";
        }
    }
}