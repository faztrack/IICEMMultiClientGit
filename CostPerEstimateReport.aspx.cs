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
using System.Drawing;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;

public partial class CostPerEstimateReport : System.Web.UI.Page
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
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (nTypeId == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\rptCostReport.rpt");
        else if (nTypeId == 2)
            strReportPath = Server.MapPath(@"Reports\rpt\rptCostReportBySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptCostReport.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

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

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);
        ht.Add("p_date", strdate);
        ht.Add("p_customername2", strCustomerName2);
        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        Response.Redirect(@"Reports/Common/ReportViewer.aspx");
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);


    }
}
