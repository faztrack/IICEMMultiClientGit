using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MarginReport : System.Web.UI.Page
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
            if (Page.User.IsInRole("rpt007") == false)
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
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Job #", typeof(string));
        table.Columns.Add("Last Name", typeof(string));
        table.Columns.Add("First Name", typeof(string));
        table.Columns.Add("Estimate", typeof(string));
        table.Columns.Add("Sale Date", typeof(string));
        table.Columns.Add("Sales Person", typeof(string));
        table.Columns.Add("Initial Sales", typeof(string));
        table.Columns.Add("C/O Total", typeof(string));
        table.Columns.Add("Total Project Revenue", typeof(string));
        table.Columns.Add("Collected $", typeof(string));
        table.Columns.Add("Collected %", typeof(string));
        table.Columns.Add("Project Base Cost", typeof(string));
        table.Columns.Add("Labor Cost", typeof(string));
        table.Columns.Add("Material Cost", typeof(string));
        table.Columns.Add("Commission", typeof(string));
        table.Columns.Add("Total Project Cost", typeof(string));
        table.Columns.Add("Margin $", typeof(string));
        table.Columns.Add("Projected Margin", typeof(string));
        table.Columns.Add("Actual Margin", typeof(string));
       
        return table;
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
       
        string strQ = " SELECT customers.customer_id,customer_estimate.estimate_id,customer_estimate.job_number, " +
                     " customers.last_name1,  customers.first_name1,customer_estimate.estimate_name,customer_estimate.sale_date, sales_person.first_name +' '+last_name AS SalesPerson,customer_estimate.sales_person_id, " +
                     " ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price,ISNULL (estimate_payments.total_incentives,0) AS total_incentives  " +
                     " FROM customers  "+
                     " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id "+
                     " INNER JOIN sales_person ON  sales_person.sales_person_id = customer_estimate.sales_person_id "+
                     " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id "+
                     " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id "+
                     " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 AND customer_estimate.status_id=3 " + strCondition;

       DataTable dtReport = csCommonUtility.GetDataTable(strQ);
       DataTable tmpTable = LoadDataTable();

       DataRow drNew = tmpTable.NewRow();
       drNew["Job #"] = "Margin Report";
       drNew["Last Name"] = "";
       drNew["First Name"] = "";
       drNew["Estimate"] = "";
       drNew["Sale Date"] = "Projected Margin:";
       drNew["Sales Person"] = "((TotalProjectRevenue - ProjectBaseCost) / TotalProjectRevenue) * 100";
       //drNew["Initial Sales"] = drNew["Initial Sales"];
       //drNew["C/O Total"] = drNew["C/O Total"];
       //drNew["Total Project Revenue"] = drNew["Total Project Revenue"];
       //drNew["Collected $"] = drNew["Collected $"];
       //drNew["Collected %"] = drNew["Collected %"];
       //drNew["Project Base Cost"] = drNew["Project Base Cost"];
       //drNew["Labor Cost"] = drNew["Labor Cost"];
       //drNew["Material Cost"] = drNew["Material Cost"];
       //drNew["Commission"] = drNew["Commission"];
       //drNew["Total Project Cost"] = drNew["Total Project Cost"];
       //drNew["Margin $"] = drNew["Margin $"];
       //drNew["Projected Margin"] = drNew["Projected Margin"];
       //drNew["Actual Margin"] = drNew["Actual Margin"];
       //tmpTable.Rows.Add(drNew);
       //drNew = tmpTable.NewRow();
       //drNew["Job #"] = "Margin Report";
       //drNew["Last Name"] = drNew["Last Name"];
       //drNew["First Name"] = drNew["First Name"];
       //drNew["Estimate"] = drNew["Estimate"];
       //drNew["Sale Date"] = drNew["Sale Date"];
       //drNew["Sales Person"] = drNew["Sales Person"];
       //drNew["Initial Sales"] = drNew["Initial Sales"];
       //drNew["C/O Total"] = drNew["C/O Total"];
       //drNew["Total Project Revenue"] = drNew["Total Project Revenue"];
       //drNew["Collected $"] = drNew["Collected $"];
       //drNew["Collected %"] = drNew["Collected %"];
       //drNew["Project Base Cost"] = drNew["Project Base Cost"];
       //drNew["Labor Cost"] = drNew["Labor Cost"];
       //drNew["Material Cost"] = drNew["Material Cost"];
       //drNew["Commission"] = drNew["Commission"];
       //drNew["Total Project Cost"] = drNew["Total Project Cost"];
       //drNew["Margin $"] = drNew["Margin $"];
       //drNew["Projected Margin"] = drNew["Projected Margin"];
       //drNew["Actual Margin"] = drNew["Actual Margin"];
       tmpTable.Rows.Add(drNew);
       drNew = tmpTable.NewRow();
       drNew["Job #"] = "Sales Person:";
       drNew["Last Name"] = ddlSalesPersons.SelectedItem.Text;
       drNew["First Name"] = "";
       drNew["Estimate"] = "";
       drNew["Sale Date"] = "Actual Margin:";
       drNew["Sales Person"] = "((TotalProjectRevenue - TotalProjectCost) / TotalProjectRevenue) * 100";
       tmpTable.Rows.Add(drNew);
       drNew = tmpTable.NewRow();
       drNew["Job #"] = "Date Range:";
       drNew["Last Name"] = txtStartDate.Text+" to "+txtEndDate.Text;
       drNew["First Name"] = "";
       drNew["Estimate"] = "";
       drNew["Sale Date"] = "Project Base Cost:";
       drNew["Sales Person"] = "Price Book without Retail Multiplier and Other Items divided by 2";
       tmpTable.Rows.Add(drNew);
       drNew = tmpTable.NewRow();
       drNew["Job #"] = "";
       drNew["Last Name"] = "";
       drNew["First Name"] = "";
       drNew["Estimate"] = "";
       drNew["Sale Date"] = "Total Project Cost:";
       drNew["Sales Person"] = "Labor + Material + Commission";
       tmpTable.Rows.Add(drNew);
       drNew = tmpTable.NewRow();
       tmpTable.Rows.Add(drNew);

       drNew = tmpTable.NewRow();
       drNew["Job #"] = "Job #";
       drNew["Last Name"] = "Last Name";
       drNew["First Name"] = "First Name";
       drNew["Estimate"] = "Estimate";
       drNew["Sale Date"] = "Sale Date";
       drNew["Sales Person"] = "Sales Person";
       drNew["Initial Sales"] = "Initial Sales";
       drNew["C/O Total"] = "C/O Total";
       drNew["Total Project Revenue"] = "Total Project Revenue";
       drNew["Collected $"] = "Collected $";
       drNew["Collected %"] = "Collected %";
       drNew["Project Base Cost"] = "Project Base Cost";
       drNew["Labor Cost"] = "Labor Cost";
       drNew["Material Cost"] = "Material Cost";
       drNew["Commission"] = "Commission";
       drNew["Total Project Cost"] = "Total Project Cost";
       drNew["Margin $"] = "Margin $";
       drNew["Projected Margin"] = "Projected Margin";
       drNew["Actual Margin"] = "Actual Margin";
       tmpTable.Rows.Add(drNew);


       if (dtReport.Rows.Count == 0)
       {
           lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No data exist.");
           return;
       }
       else
       {
           foreach (DataRow dr in dtReport.Rows)
           {
               int nCustomerId = Convert.ToInt32(dr["customer_id"]);
               int nEstimateId = Convert.ToInt32(dr["estimate_id"]);
               int nSalesPersonId = Convert.ToInt32(dr["sales_person_id"]);
               decimal Intensive = Convert.ToDecimal(dr["total_incentives"]);
               decimal dRetail = 0;
               decimal Comper = 0;
               var result1 = (from pd in _db.pricing_details
                             where (from clc in _db.customer_locations
                                    where clc.estimate_id == nEstimateId && clc.customer_id == nCustomerId && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                    select clc.location_id).Contains(pd.location_id) &&
                                    (from cs in _db.customer_sections
                                     where cs.estimate_id == nEstimateId && cs.customer_id == nCustomerId && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                     select cs.section_id).Contains(pd.section_level) && pd.estimate_id == nEstimateId && pd.customer_id == nCustomerId && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A" && pd.is_CommissionExclude == false
                             select pd.total_retail_price);
               int n1 = result1.Count();
               if (result1 != null && n1 > 0)
                   dRetail = result1.Sum();

               decimal TotalPriceExCom = dRetail - Intensive;
               decimal ComAmount = 0;
               var result3 = (from ppi in _db.co_estimate_commissions
                              where ppi.estimate_id == nEstimateId && ppi.customer_id == nCustomerId && ppi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select ppi.comission_amount);
               int n3 = result3.Count();
               if (result3 != null && n3 > 0)
                   ComAmount = (decimal)result3.Sum();

               if (ComAmount == 0)
               {

                   decimal sCommission = 0;
                   if (nSalesPersonId > 0)
                   {
                       sales_person sp_info = new sales_person();
                       sp_info = _db.sales_persons.Single(c => c.sales_person_id == nSalesPersonId);
                       if (sp_info.com_per != null)
                           Comper = Convert.ToDecimal(sp_info.com_per);

                       if (Comper > 0 && TotalPriceExCom > 0)
                       {
                           sCommission = TotalPriceExCom * (Comper / 100);
                       }


                   }
                   ComAmount = sCommission;
               }


               decimal TotalCOAmount = 0;
                 decimal TotalCoAmount_Base = 0;
                 decimal TotalCOExCom = 0;
             
               var COitem = from co in _db.changeorder_estimates
                            where co.customer_id == nCustomerId && co.estimate_id == nEstimateId && co.change_order_status_id == 3
                            orderby co.changeorder_name ascending
                            select co;
               foreach (changeorder_estimate cho in COitem)
               {
                   int ncoeid = cho.chage_order_id;
                   decimal CoTaxRate = 0;
                   decimal CoPrice = 0;
                   decimal CoTax = 0;
                   decimal CoPriceExCom = 0;
                 
                   CoTaxRate = Convert.ToDecimal(cho.tax);
                   decimal dEconCost = 0;
                   decimal dECOCostExCom = 0;
                   var Coresult = (from chpl in _db.change_order_pricing_lists
                                   where chpl.estimate_id == nEstimateId && chpl.customer_id == nCustomerId && chpl.client_id == 1 && chpl.chage_order_id == ncoeid
                                   select chpl.EconomicsCost);
                   int cn = Coresult.Count();
                   if (Coresult != null && cn > 0)
                       dEconCost = Coresult.Sum();


                   var CoresultExCom = (from chpl in _db.change_order_pricing_lists
                                        where chpl.estimate_id == nEstimateId  && chpl.customer_id == nCustomerId && chpl.client_id == 1 && chpl.chage_order_id == ncoeid
                                        select chpl.EconomicsCost);

                   int cnExCom = CoresultExCom.Count();
                   if (CoresultExCom != null && cnExCom > 0)
                       dECOCostExCom = CoresultExCom.Sum();


                   if (CoTaxRate > 0)
                   {
                       CoTax = dEconCost * (CoTaxRate / 100);
                       CoPrice = dEconCost + CoTax;
                       CoPriceExCom = dECOCostExCom;

                   }
                   else
                   {
                       CoPrice = dEconCost;
                       CoPriceExCom = dECOCostExCom;
                   }
                   TotalCOAmount += CoPrice;
                   TotalCOExCom += CoPriceExCom;


                   decimal TotalCoCostAmount = 0;

                   string strCOCostQ = " SELECT item_id, total_retail_price FROM change_order_pricing_list  where estimate_id =" + nEstimateId + " AND customer_id = " + nCustomerId + " AND client_id = 1 AND chage_order_id ="+ ncoeid;

                   DataTable dtCOCost = csCommonUtility.GetDataTable(strCOCostQ);
                   foreach (DataRow drCOCost in dtCOCost.Rows)
                   {
                       decimal nCOCostAmt = 0;
                       // int nLaborId = 1;
                       decimal nRetailMulti = 0;
                       if (_db.item_prices.Where(it => it.item_id == Convert.ToInt32(drCOCost["item_id"]) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count > 0)
                       {
                           item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(drCOCost["item_id"]) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                         
                           //decimal nLaborRate = 0;
                           //decimal nItemCost = 0;

                           if (itm != null)
                           {

                               nRetailMulti = Convert.ToDecimal(itm.retail_multiplier);
                               //nLaborRate = Convert.ToDecimal(itm.labor_rate);
                               //nLaborRate = Convert.ToDecimal(itm.labor_rate);
                               //nLaborId = Convert.ToInt32(itm.labor_id);
                           }
                       }
                       if (nRetailMulti > 0)
                       {
                           nCOCostAmt = Convert.ToDecimal(drCOCost["total_retail_price"]) / nRetailMulti;
                       }
                       else
                       {
                           nCOCostAmt = Convert.ToDecimal(drCOCost["total_retail_price"])/2;
 
                       }

                       TotalCoCostAmount += nCOCostAmt;

                   }
                    TotalCoAmount_Base += TotalCoCostAmount;

               }

               decimal COComAmount = 0;
               var result2 = (from ppi in _db.co_estimate_commissions
                             where ppi.estimate_id == nEstimateId && ppi.customer_id == nCustomerId && ppi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select ppi.comission_amount);
               int n2 = result2.Count();
               if (result2 != null && n2 > 0)
                   COComAmount = (decimal)result2.Sum();

               if (COComAmount == 0)
               {

                   decimal CoComper = 0;
                   decimal sCOCommission = 0;
                   if (nSalesPersonId > 0)
                   {
                       sales_person sp_info = new sales_person();
                       sp_info = _db.sales_persons.Single(c => c.sales_person_id == nSalesPersonId);
                       if (sp_info.co_com_per != null)
                           CoComper = Convert.ToDecimal(sp_info.co_com_per);


                       if (CoComper > 0 && TotalCOExCom > 0)
                       {
                           sCOCommission = TotalCOExCom * (CoComper / 100);
                       }


                   }
                   COComAmount = sCOCommission;

               }
              

               decimal payAmount = 0;
               var result = (from ppi in _db.New_partial_payments
                             where ppi.estimate_id == nEstimateId && ppi.customer_id == nCustomerId && ppi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select ppi.pay_amount);
               int n = result.Count();
               if (result != null && n > 0)
                   payAmount = result.Sum();
             

               decimal dSaleAmount = Convert.ToDecimal(dr["NewTotal_Price"]);
               decimal TotalProjectRevenue = dSaleAmount + TotalCOAmount;

               decimal CollectedPer = 0;
               if (payAmount > 0)
               {
                   if(TotalProjectRevenue > 0) // Divided by zero error solve
                   CollectedPer = (payAmount / TotalProjectRevenue) * 100;
 
               }

               decimal TotalCostAmountasSold = 0;

               string strCostQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                   " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                   " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstimateId + " AND customer_locations.customer_id =" + nCustomerId + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstimateId + " AND customer_sections.customer_id =" + nCustomerId+ " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND estimate_id=" + nEstimateId + " AND customer_id=" + nCustomerId + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

               DataTable dtCost = csCommonUtility.GetDataTable(strCostQ);
               foreach (DataRow drCost in dtCost.Rows)
               {
                   decimal nCostAmt = 0;
                   if (Convert.ToDecimal(drCost["retail_multiplier"])>0) 
                   {
                        if (Convert.ToInt32(drCost["labor_id"]) == 2) 
                         {
                           nCostAmt = (Convert.ToDecimal(drCost["item_cost"])/Convert.ToDecimal(drCost["retail_multiplier"]) + Convert.ToDecimal(drCost["labor_rate"]))* Convert.ToDecimal(drCost["quantity"]);
                         }
                        else
                        {
                             nCostAmt = (Convert.ToDecimal(drCost["item_cost"])/Convert.ToDecimal(drCost["retail_multiplier"]))* Convert.ToDecimal(drCost["quantity"]);

                        }

                   }
                   else
                   {
                        if (Convert.ToInt32(drCost["labor_id"]) == 2) 
                         {
                           nCostAmt = (Convert.ToDecimal(drCost["item_cost"]) + Convert.ToDecimal(drCost["labor_rate"]))* Convert.ToDecimal(drCost["quantity"]);
                         }
                        else
                        {
                             nCostAmt = (Convert.ToDecimal(drCost["item_cost"])/2)* Convert.ToDecimal(drCost["quantity"]);

                        }

                   }

                   TotalCostAmountasSold += nCostAmt;
 
               }

               decimal nLabor = 0;
               var result4 = (from ppi in _db.vendor_costs
                              where ppi.estimate_id == nEstimateId && ppi.customer_id == nCustomerId && ppi.category_id == 2 && ppi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select ppi.cost_amount);
               int n4 = result4.Count();
               if (result4 != null && n4 > 0)
                   nLabor = (decimal) result4.Sum();

               decimal nMaterial = 0;
               var result5 = (from ppi in _db.vendor_costs
                              where ppi.estimate_id == nEstimateId && ppi.customer_id == nCustomerId && ppi.category_id == 1 && ppi.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select ppi.cost_amount);
               int n5 = result5.Count();
               if (result5 != null && n5 > 0)
                   nMaterial = (decimal)result5.Sum();

               decimal ProjectBaseCost = TotalCostAmountasSold + TotalCoAmount_Base;


               decimal tCommission = ComAmount + COComAmount;
               decimal TotalProjectCost = nLabor + nMaterial + tCommission;

               decimal dMargin = TotalProjectRevenue - TotalProjectCost;
               decimal ProjectedMarginper = 0;
               decimal ActualMarginper = 0;
               if (TotalProjectRevenue > 0) // Divided by zero error solve
               {
                   ProjectedMarginper = ((TotalProjectRevenue - ProjectBaseCost) / TotalProjectRevenue) * 100;
                   ActualMarginper = ((TotalProjectRevenue - TotalProjectCost) / TotalProjectRevenue) * 100;
               }

               

               DataRow drNew1 = tmpTable.NewRow();
               drNew1["Job #"] = dr["job_number"];
               drNew1["Last Name"] = dr["last_name1"];
               drNew1["First Name"] = dr["first_name1"];
               drNew1["Estimate"] = dr["estimate_name"];
               drNew1["Sale Date"] = dr["sale_date"];
               drNew1["Sales Person"] = dr["SalesPerson"];
               drNew1["Initial Sales"] = dSaleAmount.ToString("c");
               drNew1["C/O Total"] = TotalCOAmount.ToString("c"); ;
               drNew1["Total Project Revenue"] = TotalProjectRevenue.ToString("c");
               drNew1["Collected $"] = payAmount.ToString("c");
               drNew1["Collected %"] = CollectedPer.ToString() + " %";
               drNew1["Project Base Cost"] = ProjectBaseCost.ToString("c");
               drNew1["Labor Cost"] = nLabor.ToString("c"); ;
               drNew1["Material Cost"] = nMaterial.ToString("c");
               drNew1["Commission"] = tCommission.ToString("c");
               drNew1["Total Project Cost"] = TotalProjectCost.ToString("c");
               drNew1["Margin $"] = dMargin.ToString("c");
               drNew1["Projected Margin"] = ProjectedMarginper.ToString() + "%";
               drNew1["Actual Margin"] = ActualMarginper.ToString() + "%";
               tmpTable.Rows.Add(drNew1);
 
           }
 
       }

       Session.Add("Margin", tmpTable);

       //Response.Redirect("margin.aspx");
       ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('margin.aspx');", true);
    
       //Response.Clear();
       //Response.ClearHeaders();

       //using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
       //{
       //    string[] str1 = { "Margin Report" };
       //    string[] str2 = { " " };
       //    writer.WriteRecord(str1, true);
       //    writer.WriteRecord(str2, true);
       //    writer.WriteAll(tmpTable, true);
       //}
       //Response.ContentType = "application/vnd.ms-excel";
       //Response.AddHeader("Content-Disposition", "attachment; filename=MarginReport.csv");
       //Response.End();

       
        
    }
    
}