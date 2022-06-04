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

public partial class ProjectSummaryReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        customer cust = new customer();
        if(Convert.ToInt32(hdnCustomerId.Value) > 0) 
        {
            cust = _db.customers.FirstOrDefault(x => x.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            hdnClientId.Value = cust.client_id.ToString();
        }

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));


        //string strQ = string.Empty;

        //strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
        //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
        //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


        ////List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        //DataTable dt = csCommonUtility.GetDataTable(strQ);
        //foreach (DataRow dr in dt.Rows)
        //{
        //    string itemName = dr["item_name"].ToString().Trim().TrimEnd('>');
        //    dr["item_name"] = itemName.TrimEnd('>');

        //    if (dr["short_notes"].ToString().Length > 0)
        //    {
        //        dr["short_notes"] = Environment.NewLine + "NOTES: " + dr["short_notes"].ToString();
        //    }
        //}



        //Shohidullah
        string strP = string.Empty;
        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                     " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                    " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
                    " is_direct,section_level,location.location_id,'' as tmpCo " +
                    " FROM pricing_details " +
                    " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                    " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                    " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                     " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = 1 ) " +
                    "order by section_level";
        DataTable dtP = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = string.Empty;
        strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                                " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                                " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,short_notes_new,item_status_id,last_update_date, " +
                                " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,IsNull(changeorder_estimate.execute_date, '01/01/2000') as execute_date " +
                                " FROM change_order_pricing_list " +
                                " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                                " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                                " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        DataTable dtcol = csCommonUtility.GetDataTable(strQ);
        foreach (DataRow dr in dtcol.Rows)
        {
            int nItemStatusId = Convert.ToInt32(dr["item_status_id"]);
            int ncoPricingUd = Convert.ToInt32(dr["co_pricing_list_id"]);

            string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

            drNew = dtP.NewRow();
            drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
            drNew["item_id"] = dr["item_id"];
            drNew["labor_id"] = dr["labor_id"];
            drNew["section_serial"] = dr["section_serial"];
            drNew["location_name"] = dr["location_name"];
            drNew["section_name"] = dr["section_name"];
            drNew["item_name"] = dr["item_name"];
            drNew["measure_unit"] = dr["measure_unit"];
            drNew["item_cost"] = dr["item_cost"];
            drNew["total_retail_price"] = dr["total_retail_price"];
            drNew["total_direct_price"] = dr["total_direct_price"];
            drNew["minimum_qty"] = dr["minimum_qty"];
            drNew["quantity"] = dr["quantity"];
            drNew["retail_multiplier"] = dr["retail_multiplier"];
            drNew["labor_rate"] = dr["labor_rate"];
            drNew["short_notes"] = dr["short_notes"];
            drNew["short_notes_new"] = dr["short_notes_new"];
            drNew["item_status_id"] = dr["item_status_id"];
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["tmpCo"] = strTmp;
            if (nItemStatusId == 2)
            {
                drNew["total_retail_price"] = 0;
            }
            if (nItemStatusId == 3)
            {
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    drNew["item_status_id"] = 4;
                }

            }
            dtP.Rows.Add(drNew);

        }

        DataView dv = dtP.DefaultView;
        dv.Sort = "last_update_date asc";
        DataTable dtMaster = dv.ToTable();

        //DataTable dt = csCommonUtility.GetDataTable(strQ);
        foreach (DataRow dr in dtMaster.Rows)
        {
            string itemName = dr["item_name"].ToString().Trim().TrimEnd('>');
            dr["item_name"] = itemName.TrimEnd('>');

            if (dr["short_notes"].ToString().Length > 0)
            {
                dr["short_notes"] = Environment.NewLine + "NOTES: " + dr["short_notes"].ToString();
            }
            if (dr["short_notes_new"].ToString().Length > 0)
            {
                dr["short_notes_new"] = Environment.NewLine + "Pre-Con Notes: " + dr["short_notes_new"].ToString();
            }
            if (Convert.ToInt32(dr["item_status_id"]) == 3)
            {
                dr["tmpCo"] = Environment.NewLine + "Item Added " + dr["tmpCo"].ToString();
            }
            else if (Convert.ToInt32(dr["item_status_id"]) == 2)
            {
                dr["tmpCo"] = Environment.NewLine + "Item Deleted " + dr["tmpCo"].ToString();
            }
        }




        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = string.Empty;
        strReportPath = Server.MapPath(@"Reports\rpt\rptProjectSummaryReportByLocation.rpt");

        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtMaster);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        //string strCustomerName2 = cus_est.job_number ?? "";
        string strCustomerName2 = "";
        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
            strCustomerName2 = cus_est.job_number ?? "";
        else
            strCustomerName2 = cus_est.alter_job_number;
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;
        sales_person objsales = new sales_person();
        objsales = _db.sales_persons.FirstOrDefault(s => s.sales_person_id == Convert.ToInt32(objCust.sales_person_id));

        string strSalesPerson = objsales.first_name + ' ' + objsales.last_name;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_LeadTime", "");
        ht.Add("p_Contractdate", "");
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);
        ht.Add("p_date", "");
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_SalesRep", strSalesPerson);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);


        Response.Redirect(@"Reports/Common/ReportViewer.aspx");
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);


    }
}
