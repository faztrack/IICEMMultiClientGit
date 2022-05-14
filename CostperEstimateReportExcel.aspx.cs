using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CostperEstimateReportExcel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";
        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                }
                strdate = dt.ToShortDateString();

            }
        }

        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + nEstId + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + nEstId + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  " +
              " UNION " +
              " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + nEstId + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + nEstId + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtCostMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sCostList", dtCostMaster);

        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == nEstId);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        DataTable tmpTable = LoadTmpDataTable();
        if (nTypeId == 1)
        {
            #region Cost by Location

            DataView dvLoc = new DataView(dtCostMaster);
            DataTable dtResults = dvLoc.ToTable(true, "location_id", "location_name");

            DataTable tblCustList = (DataTable)Session["sCostList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Cost by Location";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Customer Name: " + strCustomerName;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Estimate: " + cus_est.estimate_name;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtResults.Rows)
            {
                int nLocationId = Convert.ToInt32(dr["location_id"]);
                string strLocation = dr["location_name"].ToString();

                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "Location: " + strLocation;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "Section Name";
                drNew["Item Name"] = "Item Name";
                drNew["Short Notes"] = "Short Notes";
                drNew["U of M"] = "U of M";
                drNew["Code"] = "Code";
                drNew["Item Cost"] = "Item Cost";
                drNew["Item Total"] = "Item Total";
                drNew["Labor Rate"] = "Labor Rate";
                drNew["Labor Total"] = "Labor Total";
                drNew["Total Cost"] = "Total Cost";
                tmpTable.Rows.Add(drNew);

                bool Iexists = tblCustList.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(nLocationId)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblCustList.Select("location_id =" + nLocationId + "");
                    foreach (var row in rows)
                    {
                        decimal dOrginalCost = 0;
                        decimal dOrginalTotalCost = 0;
                        decimal dLaborTotal = 0;
                        decimal dLineItemTotal = 0;
                        string sItemName = row["item_name"].ToString();

                        decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                        decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                        decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                        decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                        if (dRetail_multiplier > 0)
                        {
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                dOrginalCost = (dItemCost / dRetail_multiplier) / 2;
                            }
                            else
                            {
                                dOrginalCost = (dItemCost / dRetail_multiplier);
                            }
                        }
                        else
                        {
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                dOrginalCost = dItemCost / 2;
                            }
                            else
                            {
                                dOrginalCost = dItemCost;
                            }

                        }
                        dOrginalTotalCost = dOrginalCost * dQuantity;
                        dLaborTotal = dLabor_rate * dQuantity;
                        dLineItemTotal = dOrginalTotalCost + dLaborTotal;

                        drNew = tmpTable.NewRow();
                        drNew["Section Name"] = row["section_name"];
                        drNew["Item Name"] = sItemName;
                        drNew["Short Notes"] = row["short_notes"];
                        drNew["U of M"] = row["measure_unit"];
                        drNew["Code"] = row["quantity"].ToString();
                        drNew["Item Cost"] = dOrginalCost.ToString("0.0000");
                        drNew["Item Total"] = dOrginalTotalCost.ToString("c");
                        drNew["Labor Rate"] = dLabor_rate.ToString("c");
                        drNew["Labor Total"] = dLaborTotal.ToString("c");
                        drNew["Total Cost"] = dLineItemTotal.ToString("c");
                        tmpTable.Rows.Add(drNew);

                    }
                }

            }
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(tmpTable, false);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=Cost_by_Location.csv");
            Response.End();

            #endregion
        }
        else
        {
            #region Cost by Section

            DataView dvSec = new DataView(dtCostMaster);
            DataTable dtResults = dvSec.ToTable(true, "section_level", "section_name");

            DataTable tblCustList = (DataTable)Session["sCostList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Cost by Section";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Customer Name: " + strCustomerName;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Estimate: " + cus_est.estimate_name;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtResults.Rows)
            {
                int nSection_level = Convert.ToInt32(dr["section_level"]);
                string strSection = dr["section_name"].ToString();
               
                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "Section: " + strSection;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["Section Name"] = "Location Name";
                drNew["Item Name"] = "Item Name";
                drNew["Short Notes"] = "Short Notes";
                drNew["U of M"] = "U of M";
                drNew["Code"] = "Code";
                drNew["Item Cost"] = "Item Cost";
                drNew["Item Total"] = "Item Total";
                drNew["Labor Rate"] = "Labor Rate";
                drNew["Labor Total"] = "Labor Total";
                drNew["Total Cost"] = "Total Cost";
                tmpTable.Rows.Add(drNew);

                bool Iexists = tblCustList.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(nSection_level)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblCustList.Select("section_level =" + nSection_level + "");
                    foreach (var row in rows)
                    {
                        decimal dOrginalCost = 0;
                        decimal dOrginalTotalCost = 0;
                        decimal dLaborTotal = 0;
                        decimal dLineItemTotal = 0;
                        string sItemName = row["item_name"].ToString();

                        decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                        decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                        decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                        decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                        if (dRetail_multiplier > 0)
                        {
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                dOrginalCost = (dItemCost / dRetail_multiplier) / 2;
                            }
                            else
                            {
                                dOrginalCost = (dItemCost / dRetail_multiplier);
                            }
                        }
                        else
                        {
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                dOrginalCost = dItemCost / 2;
                            }
                            else
                            {
                                dOrginalCost = dItemCost;
                            }

                        }
                        dOrginalTotalCost = dOrginalCost * dQuantity;
                        dLaborTotal = dLabor_rate * dQuantity;
                        dLineItemTotal = dOrginalTotalCost + dLaborTotal;

                        drNew = tmpTable.NewRow();
                        drNew["Section Name"] = row["location_name"];
                        drNew["Item Name"] = sItemName;
                        drNew["Short Notes"] = row["short_notes"];
                        drNew["U of M"] = row["measure_unit"];
                        drNew["Code"] = row["quantity"].ToString();
                        drNew["Item Cost"] = dOrginalCost.ToString("0.0000");
                        drNew["Item Total"] = dOrginalTotalCost.ToString("c");
                        drNew["Labor Rate"] = dLabor_rate.ToString("c");
                        drNew["Labor Total"] = dLaborTotal.ToString("c");
                        drNew["Total Cost"] = dLineItemTotal.ToString("c");
                        tmpTable.Rows.Add(drNew);

                    }
                }

            }
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(tmpTable, false);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=Cost_by_Section.csv");
            Response.End();

            #endregion
 
        }

       


    }

    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("Section Name", typeof(string));
        table.Columns.Add("Item Name", typeof(string));
        table.Columns.Add("Short Notes", typeof(string));
        table.Columns.Add("U of M", typeof(string));
        table.Columns.Add("Code", typeof(string));
        table.Columns.Add("Item Cost", typeof(string));
        table.Columns.Add("Item Total", typeof(string));
        table.Columns.Add("Labor Rate", typeof(string));
        table.Columns.Add("Labor Total", typeof(string));
        table.Columns.Add("Total Cost", typeof(string));
        
        
        return table;
    }
}