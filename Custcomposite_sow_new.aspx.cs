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
using System.Web.Services;
using CrystalDecisions.Shared;

public partial class Custcomposite_sow_new : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;
    [WebMethod]
    public static string[] GetItemName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nCustID = Convert.ToInt32(HttpContext.Current.Session["cCustId"]);
        int nEstID = Convert.ToInt32(HttpContext.Current.Session["cEstId"]);
        if (prefixText.IndexOf(",") > 0)
        {
            string strCondition = string.Empty;
            string[] strIds = prefixText.Split(',');
            foreach (string strId in strIds)
            {
                if (strCondition.Length == 0)
                    strCondition = " section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";
                else
                    strCondition += "or section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";

            }
            string sSql = string.Empty;
            if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustID && cl.estimate_id == nEstID).ToList().Count == 0)
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM pricing_details WHERE customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND (" + strCondition + ")";

            }
            else
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM co_pricing_master WHERE customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND item_status_id <>3 AND (" + strCondition + ")";
            }

            return _db.ExecuteQuery<string>(sSql).Take<String>(count).ToArray();

        }
        else
        {
            if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustID && cl.estimate_id == nEstID).ToList().Count == 0)
            {
                return (from c in _db.pricing_details
                        where (c.section_name.Contains(prefixText) || c.item_name.Contains(prefixText)) && c.customer_id == nCustID && c.estimate_id == nEstID
                        select c.section_name + ">>" + c.item_name).Take<String>(count).ToArray();
            }
            else
            {
                return (from c in _db.co_pricing_masters
                        where (c.section_name.Contains(prefixText) || c.item_name.Contains(prefixText)) && c.customer_id == nCustID && c.estimate_id == nEstID && c.item_status_id != 3
                        select c.section_name + ">>" + c.item_name).Take<String>(count).ToArray();

            }

        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oCustomerUser"] == null)
            {
                Response.Redirect("customerlogin.aspx");
            }

            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            Session.Remove("sItem_list");
            Session.Remove("sItem_list_Direct");
            lblResult.Text = "";

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();
            GetEstimate(nCid);
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                Session.Add("cCustId", hdnCustomerId.Value);
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                hdnLastName.Value = cust.last_name1;

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                hdnClientId.Value = cust.client_id.ToString();

                estimate_payment est_pay = new estimate_payment();

                est_pay = _db.estimate_payments.Where(ep => ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).FirstOrDefault();
                if (est_pay != null)
                {
                    if (est_pay.tax_rate != null)
                    {
                        lblTax.Text = est_pay.tax_rate.ToString();
                    }
                }


                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    Session.Add("cEstId", hdnEstimateId.Value);
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    // hdnFinanceValue.Value = Convert.ToDecimal(cus_est.FinancePer).ToString();
                    //  hdnIsCash.Value = Convert.ToBoolean(cus_est.IsCashTerm).ToString();
                    //hdnJob.Value = cus_est.job_number;
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        hdnJob.Value = cus_est.job_number;
                    else
                        hdnJob.Value = cus_est.alter_job_number;
                    lblEstimateName.Text = cus_est.estimate_name;

                    string strStatus = "";
                    if (cus_est.status_id == 1)
                    {
                        strStatus = "Pending";
                    }
                    else if (cus_est.status_id == 2)
                    {
                        strStatus = "Sit";
                    }
                    else if (cus_est.status_id == 3)
                    {
                        strStatus = "Sold";
                    }
                    lblStatus.Text = strStatus;

                    lblSaleDate.Visible = true;
                    lblSaleDateValue.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                    lblSaleDateValue.Visible = true;

                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";
                    txtComments.ReadOnly = true;

                }


                BindSelectedItemGrid();
                BindSelectedItemGrid_Direct();
                if (grdGrouping.Rows.Count == 0)
                {
                    lblRetailPricingHeader.Visible = false;
                }
                else
                {
                    lblRetailPricingHeader.Visible = true;
                }
                if (grdGroupingDirect.Rows.Count == 0)
                {
                    lblDirectPricingHeader.Visible = false;
                }
                else
                {
                    lblDirectPricingHeader.Visible = true;
                }
                Calculate();
                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {
                    rdoSort.Visible = false;
                    tblTotalProjectPrice.Visible = false;
                    if (grdGrouping.Rows.Count == 0)
                    {
                        lblRetailPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblRetailPricingHeader.Visible = true;
                    }
                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }
                else
                {
                    rdoSort.Visible = true;
                    tblTotalProjectPrice.Visible = true;
                    if (grdGrouping.FooterRow != null)
                        grdGrouping.FooterRow.Visible = true;
                    if (grdGroupingDirect.FooterRow != null)
                        grdGroupingDirect.FooterRow.Visible = true;

                    if (grdGrouping.Rows.Count == 0)
                    {
                        lblRetailPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblRetailPricingHeader.Visible = true;
                    }
                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }
            }
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerdashboard.aspx");
    }


    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            if (rdoSort.SelectedValue == "2")
            {
                string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
                DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
                if ((dtsec == null) || (dtsec.Rows.Count == 0))
                {
                    dtsec = LoadMasterTable();
                }
                string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                               " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =1  " +
                                " order by section_level asc";
                DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
                DataRow drNew = null;
                if (dtsec2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtsec2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();
                        bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists)  // if (!ContainDataRowInDataTable(dtsec, dr))
                        {
                            drNew = dtsec.NewRow();
                            drNew["colId"] = ncolId;
                            drNew["colName"] = strColName;
                            dtsec.Rows.Add(drNew);
                        }
                    }
                }
                Session.Add("MainDTSec", dtsec);
                grdGrouping.DataSource = dtsec;
                grdGrouping.DataKeyNames = new string[] { "colId" };
                grdGrouping.DataBind();
            }
            else
            {
                string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
                DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
                if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
                {
                    dtLoc = LoadMasterTable();
                }
                string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                                " INNER JOIN location on location.location_id = cop.location_id " +
                                " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                                " AND is_direct=1 AND cop.client_id =1 order by colName asc ";
                DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
                DataRow drNew = null;
                if (dtLoc2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtLoc2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();
                        bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists)  //  if (!ContainDataRowInDataTable(dtLoc, dr))
                        {
                            drNew = dtLoc.NewRow();
                            drNew["colId"] = ncolId;
                            drNew["colName"] = strColName;
                            dtLoc.Rows.Add(drNew);
                        }
                    }
                }
                Session.Add("MainDTLoc", dtLoc);
                grdGrouping.DataSource = dtLoc;
                grdGrouping.DataKeyNames = new string[] { "colId" };
                grdGrouping.DataBind();
            }
        }
        catch (Exception ex)
        {
            string ext = ex.StackTrace;
            throw ex;
        }


    }
    bool ContainDataRowInDataTable(DataTable T, DataRow R)
    {
        foreach (DataRow item in T.Rows)
        {
            for (int i = 0; i < item.ItemArray.Length; i++)
            {
                if (Enumerable.SequenceEqual(item.ItemArray, R.ItemArray))
                    return true;
            }
        }
        return false;
    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (rdoSort.SelectedValue == "2")
        {
            string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
            if ((dtsec == null) || (dtsec.Rows.Count == 0))
            {
                dtsec = LoadMasterTable();
            }
            string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =1  " +
                            " order by section_level asc";
            DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
            DataRow drNew = null;
            if (dtsec2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtsec2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists)  //if (!ContainDataRowInDataTable(dtsec, dr))
                    {
                        drNew = dtsec.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtsec.Rows.Add(drNew);
                    }
                }
            }
            Session.Add("MainDTSecDirect", dtsec);
            grdGroupingDirect.DataSource = dtsec;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
        else
        {
            string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
            if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
            {
                dtLoc = LoadMasterTable();
            }
            string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                            " INNER JOIN location on location.location_id = cop.location_id " +
                            " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                            " AND is_direct = 2 AND cop.client_id =1 order by colName asc ";
            DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
            DataRow drNew = null;
            if (dtLoc2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtLoc2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists)  //if (!ContainDataRowInDataTable(dtLoc, dr))
                    {
                        drNew = dtLoc.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtLoc.Rows.Add(drNew);
                    }
                }
            }
            Session.Add("MainDTLocDirect", dtLoc);
            grdGroupingDirect.DataSource = dtLoc;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
    }
    private void Calculate()
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal ntotal_incentives = 0;
        bool bincentives = false;
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == 1).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == 1);
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes
            //lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes

            ntotal_incentives = Convert.ToDecimal(esp.total_incentives);
            bincentives = Convert.ToBoolean(esp.is_incentives);


        }
        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == 1
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecievedAmount.Text = payAmount.ToString("c");

        decimal TotalCOAmount = 0;
        var COitem = from co in _db.changeorder_estimates
                     where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                     orderby co.changeorder_name ascending
                     select co;
        foreach (changeorder_estimate cho in COitem)
        {
            int ncoeid = cho.chage_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            CoTaxRate = Convert.ToDecimal(cho.tax);
            decimal dEconCost = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == 1 && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int cn = Coresult.Count();
            if (Coresult != null && cn > 0)
                dEconCost = Coresult.Sum();

            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;

            }
            else
            {
                CoPrice = dEconCost;
            }
            TotalCOAmount += CoPrice;

        }
        lblTotalCOAmount.Text = TotalCOAmount.ToString("c");

        decimal TotalCostAmount = 0;
        TotalCostAmount = totalwithtax + TotalCOAmount;
        lblTotalAmount.Text = TotalCostAmount.ToString("c");

        decimal TotalBalanceAmount = 0;
        TotalBalanceAmount = TotalCostAmount - payAmount;
        lblTotalBalanceAmount.Text = TotalBalanceAmount.ToString("c");

        lblIncentive.Text = ntotal_incentives.ToString("c");
        if (bincentives)
            trIncentive.Visible = true;
        else
            trIncentive.Visible = false;
    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string str = e.Row.Cells[2].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblT_price1 = (Label)e.Row.Cells[8].FindControl("lblT_price1");
            Label lblTotal_price = (Label)e.Row.Cells[8].FindControl("lblTotal_price");
            Label lblItem_price = (Label)e.Row.Cells[6].FindControl("lblItem_price");
            Label lblDleted = (Label)e.Row.FindControl("lblDleted");


            decimal dTotalPrice = Convert.ToDecimal(gv1.DataKeys[e.Row.RowIndex].Values[2]);
            decimal dQty = Convert.ToDecimal(gv1.DataKeys[e.Row.RowIndex].Values[4]);
            decimal nItemPrice = 0;
            if (dQty != 0)
            {
                nItemPrice = dTotalPrice / dQty;

            }
            lblItem_price.Text = nItemPrice.ToString("c");
            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFintRetail = 0;
            //    decimal nFinanceAmountRetail = 0;

            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dTotalPrice * Convert.ToDecimal(hdnFinanceValue.Value) / 100);

            //    }
            //    nFintRetail = dTotalPrice + nFinanceAmountRetail;
            //    lblTotal_price.Text = nFintRetail.ToString("c");
            //    lblT_price1.Text = nFintRetail.ToString();
            //    if (dQty != 0)
            //    {
            //        nItemPrice = nFintRetail / dQty;

            //    }
            //    lblItem_price.Text = nItemPrice.ToString("c");

            //}
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[8].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[9].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[10].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                e.Row.Cells[10].Text = "Item Deleted" + e.Row.Cells[10].Text;
                //Label lblT_price1 = (Label)e.Row.Cells[8].FindControl("lblT_price1");
                lblT_price1.Text = "0.00";

            }
            else if (nItemStatusId == 3)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {

                    lblT_price1.Text = "0.00";
                    lblDleted.Visible = true;
                    lblDleted.ForeColor = Color.Red;
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Cells[10].Text = "Item Added" + e.Row.Cells[10].Text;
                //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }
            else
            {
                e.Row.Cells[10].Text = "No Change";
            }




        }

    }
    private DataTable LoadMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("colId", typeof(int));
        table.Columns.Add("colName", typeof(string));
        return table;
    }

    protected string GetTotalPrice()
    {
        decimal nGrandtotal = Convert.ToDecimal(grandtotal);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);

        lblRunning.Text = grandtotal.ToString("c");

        return "Total with Tax: " + grandtotal.ToString("c");

    }
    protected string GetTotalPriceDirect()
    {
        decimal nGrandtotal_direct = Convert.ToDecimal(grandtotal_direct);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);

        return "Total with Tax: " + grandtotal_direct.ToString("c");
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            if (Session["sItem_list"] != null)
            {
                DataTable dtItemdata = (DataTable)Session["sItem_list"];
                DataView dv = dtItemdata.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date ASC";


                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
                gv.DataBind();
            }
            else if (Session["Item_list"] != null)
            {
                DataTable dtItemdata = (DataTable)Session["Item_list"];
                DataView dv = dtItemdata.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date ASC";


                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
                gv.DataBind();
            }
            else
            {
                int nDirectId = 1;
                GetData(colId, gv, nDirectId);
            }
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;

            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                //subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text);
                subtotal += Double.Parse((row.FindControl("lblT_price1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            grandtotal += subtotal;
            subtotal = 0.0;

        }

    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (rdoSort.SelectedValue == "1")
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                    " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                    " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
                   " order by section_level";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                           " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                           " FROM change_order_pricing_list " +
                           " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                           " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {

                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

                drNew = dt.NewRow();
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
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                dt.Rows.Add(drNew);
                //}

            }
            if (nDirectId == 1)
                Session.Add("Item_list", dt);
            else
                Session.Add("Item_list_Direct", dt);
            DataView dv = dt.DefaultView;
            if (rdoSort.SelectedValue == "1")
            {
                dv.RowFilter = "location_id =" + colId;
            }
            else
            {
                dv.RowFilter = "section_level =" + colId;
            }
            dv.Sort = "last_update_date asc";

            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
            grd.DataBind();
        }
        else
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                    " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo  " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
                   " order by location.location_name";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                           " location.location_name AS section_name,section_name AS location_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                           " FROM change_order_pricing_list " +
                           " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                           " INNER JOIN changeorder_estimate  on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by location.location_name";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {

                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

                drNew = dt.NewRow();
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
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                dt.Rows.Add(drNew);

            }
            if (nDirectId == 1)
                Session.Add("Item_list", dt);
            else
                Session.Add("Item_list_Direct", dt);
            DataView dv = dt.DefaultView;
            if (rdoSort.SelectedValue == "1")
            {
                dv.RowFilter = "location_id =" + colId;
            }
            else
            {
                dv.RowFilter = "section_level =" + colId;
            }

            dv.Sort = "last_update_date asc";
            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
            grd.DataBind();
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "Click"); 
        if (txtSearchItemName.Text.Trim() != "")
        {
            BindSelectedSecORLoc();
            BindSelectedSecORLoc_Direct();
            if (txtSearchItemName.Text.Trim() != "")
            {
                lblResult.Text = "";
                Session.Remove("sItem_list");
                Session.Remove("sItem_list_Direct");
                DataTable dtMainDTSec = new DataTable();
                DataTable dtMainDTLoc = new DataTable();
                DataTable dtMainDTSecDirect = new DataTable();
                DataTable dtMainDTLocDirect = new DataTable();

                DataTable dtItemdata = new DataTable();
                DataTable dtItemDirect = new DataTable();
                if (Session["Item_list"] != null)
                {
                    dtItemdata = (DataTable)Session["Item_list"];
                }
                if (Session["Item_list_Direct"] != null)
                {
                    dtItemDirect = (DataTable)Session["Item_list_Direct"];
                }
                if (Session["MainDTSec"] != null)
                {
                    dtMainDTSec = (DataTable)Session["MainDTSec"];
                }
                if (Session["MainDTLoc"] != null)
                {
                    dtMainDTLoc = (DataTable)Session["MainDTLoc"];
                }
                if (Session["MainDTSecDirect"] != null)
                {
                    dtMainDTSecDirect = (DataTable)Session["MainDTSecDirect"];
                }
                if (Session["MainDTLocDirect"] != null)
                {
                    dtMainDTLocDirect = (DataTable)Session["MainDTLocDirect"];
                }

                DataTable dtItemNew = new DataTable();
                DataTable dtItemNewDirect = new DataTable();

                string prefixText = txtSearchItemName.Text.Trim();
                if (prefixText.IndexOf(">>") != -1)
                {
                    var corrected = prefixText.Substring(0, prefixText.Length - 2);
                    if (prefixText.IndexOf(">>") != -1)
                    {
                        prefixText = corrected.Substring(corrected.LastIndexOf(">>") + 2);
                    }
                    else
                    {
                        prefixText = corrected;

                    }
                }
                int nCustID = Convert.ToInt32(hdnCustomerId.Value);
                int nEstID = Convert.ToInt32(hdnEstimateId.Value);
                if (prefixText.IndexOf(",") > 0)
                {
                    string strCondition = string.Empty;
                    string[] strIds = prefixText.Split(',');
                    foreach (string strId in strIds)
                    {
                        if (strCondition.Length == 0)
                            strCondition = " section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";
                        else
                            strCondition += "or section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";

                    }
                    if (dtItemdata.Rows.Count > 0)
                    {
                        DataRow[] filteredRows = dtItemdata.Select(strCondition);
                        if (filteredRows.Length != 0)
                        {
                            dtItemNew = filteredRows.CopyToDataTable();
                            Session.Add("sItem_list", dtItemNew);

                        }
                    }
                    if (dtItemDirect.Rows.Count > 0)
                    {
                        DataRow[] filteredRows = dtItemDirect.Select(strCondition);
                        if (filteredRows.Length != 0)
                        {
                            dtItemNewDirect = filteredRows.CopyToDataTable();
                            Session.Add("sItem_listDirect", dtItemNewDirect);

                        }
                    }

                }
                else
                {

                    string strquery = "section_name LIKE '%" + prefixText.Trim() + "%'" + " or item_name LIKE '%" + prefixText.Trim() + "%'";
                    if (dtItemdata.Rows.Count > 0)
                    {
                        DataRow[] filteredRows = dtItemdata.Select(strquery);
                        if (filteredRows.Length != 0)
                        {
                            dtItemNew = filteredRows.CopyToDataTable();
                            Session.Add("sItem_list", dtItemNew);

                        }
                    }
                    if (dtItemDirect.Rows.Count > 0)
                    {
                        DataRow[] filteredRows = dtItemDirect.Select(strquery);
                        if (filteredRows.Length != 0)
                        {
                            dtItemNewDirect = filteredRows.CopyToDataTable();
                            Session.Add("sItem_listDirect", dtItemNewDirect);

                        }
                    }

                }

                if (rdoSort.SelectedValue == "1")
                {
                    if (dtItemNew.Rows.Count > 0)
                    {
                        DataTable dtTemp = dtMainDTLoc.Copy();
                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            int ncolId = Convert.ToInt32(dr["colId"]);
                            bool Iexists = dtItemNew.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(ncolId)).Count() > 0;
                            if (!Iexists)
                            {
                                var rows = dtMainDTLoc.Select("colId =" + ncolId + "");
                                foreach (var row in rows)
                                {
                                    row.Delete();
                                    dtMainDTLoc.AcceptChanges();
                                }
                            }
                        }
                    }
                    if (dtItemNewDirect.Rows.Count > 0)
                    {
                        DataTable dtTemp = dtMainDTLocDirect.Copy();
                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            int ncolId = Convert.ToInt32(dr["colId"]);
                            bool Iexists = dtItemNewDirect.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(ncolId)).Count() > 0;
                            if (!Iexists)
                            {
                                var rows = dtMainDTLocDirect.Select("colId =" + ncolId + "");
                                foreach (var row in rows)
                                {
                                    row.Delete();
                                    dtMainDTLocDirect.AcceptChanges();
                                }
                            }
                        }
                    }
                    if (Session["sItem_list"] == null && Session["sItem_listDirect"] == null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found");
                        return;
                    }

                    if (dtMainDTLoc.Rows.Count > 0)
                    {
                        Session.Add("sMainDTLoc", dtMainDTLoc);
                        grdGrouping.DataSource = dtMainDTLoc;
                        grdGrouping.DataKeyNames = new string[] { "colId" };
                        grdGrouping.DataBind();
                    }
                    if (dtMainDTLocDirect.Rows.Count > 0)
                    {
                        Session.Add("sMainDTLocDirect", dtMainDTLocDirect);
                        grdGroupingDirect.DataSource = dtMainDTLocDirect;
                        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                        grdGroupingDirect.DataBind();
                    }
                }
                else
                {
                    if (Session["sItem_list"] == null && Session["sItem_listDirect"] == null)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found");
                        return;
                    }
                    if (dtItemNew.Rows.Count > 0)
                    {
                        DataTable dtTemp = dtMainDTSec.Copy();
                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            int ncolId = Convert.ToInt32(dr["colId"]);
                            bool Iexists = dtItemNew.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                            if (!Iexists)
                            {
                                var rows = dtMainDTSec.Select("colId =" + ncolId + "");
                                foreach (var row in rows)
                                {
                                    row.Delete();
                                    dtMainDTSec.AcceptChanges();
                                }
                            }
                        }
                    }
                    if (dtItemNewDirect.Rows.Count > 0)
                    {
                        DataTable dtTemp = dtMainDTSecDirect.Copy();
                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            int ncolId = Convert.ToInt32(dr["colId"]);
                            bool Iexists = dtItemNewDirect.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                            if (!Iexists)
                            {
                                var rows = dtMainDTSecDirect.Select("colId =" + ncolId + "");
                                foreach (var row in rows)
                                {
                                    row.Delete();
                                    dtMainDTSecDirect.AcceptChanges();
                                }
                            }
                        }
                    }

                    if (dtMainDTSec.Rows.Count > 0)
                    {
                        Session.Add("sMainDTSec", dtMainDTSec);
                        grdGrouping.DataSource = dtMainDTSec;
                        grdGrouping.DataKeyNames = new string[] { "colId" };
                        grdGrouping.DataBind();
                    }
                    if (dtMainDTSecDirect.Rows.Count > 0)
                    {
                        Session.Add("sMainDTSecDirect", dtMainDTSecDirect);
                        grdGroupingDirect.DataSource = dtMainDTSecDirect;
                        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                        grdGroupingDirect.DataBind();
                    }
                }
            }
        }
        else
        {
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
            if (grdGrouping.Rows.Count == 0)
            {
                lblRetailPricingHeader.Visible = false;
            }
            else
            {
                lblRetailPricingHeader.Visible = true;
            }
            if (grdGroupingDirect.Rows.Count == 0)
            {
                lblDirectPricingHeader.Visible = false;
            }
            else
            {
                lblDirectPricingHeader.Visible = true;
            }
        }
        //Calculate_Total();
    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv2 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string str = e.Row.Cells[2].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblT_price2 = (Label)e.Row.Cells[8].FindControl("lblT_price2");
            Label lblTotal_price2 = (Label)e.Row.Cells[8].FindControl("lblTotal_price2");

            Label lblItem_price2 = (Label)e.Row.Cells[6].FindControl("lblItem_price2");
            Label lblDleted2 = (Label)e.Row.FindControl("lblDleted2");

            decimal dQty = Convert.ToDecimal(gv2.DataKeys[e.Row.RowIndex].Values[4]);
            decimal dDirectPrice = Convert.ToDecimal(gv2.DataKeys[e.Row.RowIndex].Values[3]);
            decimal nItemPrice = 0;
            if (dQty != 0)
            {
                nItemPrice = dDirectPrice / dQty;

            }
            lblItem_price2.Text = nItemPrice.ToString("c");

            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFintRetail = 0;
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dDirectPrice * Convert.ToDecimal(hdnFinanceValue.Value) / 100);

            //    }
            //    nFintRetail = dDirectPrice + nFinanceAmountRetail;
            //    lblTotal_price2.Text = nFintRetail.ToString("c");
            //    lblT_price2.Text = nFintRetail.ToString();
            //    if (dQty != 0)
            //    {
            //        nItemPrice = nFintRetail / dQty;

            //    }
            //    lblItem_price2.Text = nItemPrice.ToString("c");
            //}
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[8].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[9].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[10].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");

                lblT_price2.Text = "0.00";

            }
            else if (nItemStatusId == 3)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_direct_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    lblT_price2.Text = "0.00";
                    lblDleted2.Visible = true;
                    lblDleted2.ForeColor = Color.Red;
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }
            else
            {
                e.Row.Cells[10].Text = "No Change";
            }



        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            int nDirectId = 2;
            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;

            if (Session["sItem_listDirect"] != null)
            {
                DataTable dtItemDirect = (DataTable)Session["sItem_listDirect"];
                DataView dv = dtItemDirect.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date DESC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
                gv.DataBind();
            }
            else if (Session["Item_list_Direct"] != null)
            {
                DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
                DataView dv = dtItemDirect.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date DESC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price", "quantity" };
                gv.DataBind();
            }
            else
            {
                GetData(colId, gv, nDirectId);
            }
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                //subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text);
                subtotal_diect += Double.Parse((row.FindControl("lblT_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

                labelTotal2.Text = subtotal_diect.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader2.Text = "Section";
                }
                else
                {
                    lblHeader2.Text = "Location";
                }
                lblSubTotalLabel2.Text = "Sub Total:";
            }
            grandtotal_direct += subtotal_diect;
            subtotal_diect = 0.0;

        }
    }


    protected void btnPrintByLoc_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintByLoc.ID, btnPrintByLoc.GetType().Name, "Click"); 
        GetData(1);
    }
    private void GetData(int nTypeId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
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
        string strP = string.Empty;

        ////Cash vs Fin
        //if (Convert.ToBoolean(hdnIsCash.Value))
        //{
        //    strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
        //               " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
        //              " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
        //              " is_direct,section_level,location.location_id,'' as tmpCo " +
        //              " FROM pricing_details " +
        //              " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //              " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //              " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
        //              " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
        //              " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
        //               " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = 1 ) " +
        //              "order by section_level";
        //}
        //else
        //{
        //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
        //    {

        //        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
        //              " location.location_name,section_name,item_name,measure_unit,item_cost,total_direct_price +(total_direct_price * " + Convert.ToDecimal(hdnFinanceValue.Value) + " / 100) as total_direct_price ,total_retail_price +(total_retail_price * " + Convert.ToDecimal(hdnFinanceValue.Value) + " / 100) as total_retail_price , " +
        //             " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
        //             " is_direct,section_level,location.location_id,'' as tmpCo " +
        //             " FROM pricing_details " +
        //             " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //             " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //             " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
        //             " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
        //             " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
        //              " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = 1 ) " +
        //             "order by section_level";
        //    }
        //    else
        //    {
        //        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
        //               " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
        //              " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
        //              " is_direct,section_level,location.location_id,'' as tmpCo " +
        //              " FROM pricing_details " +
        //              " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //              " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //              " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
        //              " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
        //              " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
        //               " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = 1 ) " +
        //              "order by section_level";

        //    }

        //}


        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                     " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                     " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
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

        //Cash vs Fin
        //if (Convert.ToBoolean(hdnIsCash.Value))
        //{
        //    strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
        //                  " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
        //                  " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
        //                  " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
        //                  " FROM change_order_pricing_list " +
        //                  " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
        //                  " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
        //                  " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        //}
        //else
        //{
        //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
        //    {

        //        strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
        //                 " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_direct_price +(total_direct_price * " + Convert.ToDecimal(hdnFinanceValue.Value) + " / 100) as total_direct_price ,total_retail_price +(total_retail_price * " + Convert.ToDecimal(hdnFinanceValue.Value) + " / 100) as total_retail_price , " +
        //                 " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
        //                 " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
        //                 " FROM change_order_pricing_list " +
        //                 " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
        //                 " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
        //                 " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        //    }
        //    else
        //    {
        //        strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
        //                 " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
        //                 " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
        //                 " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
        //                 " FROM change_order_pricing_list " +
        //                 " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
        //                 " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
        //                 " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";

        //    }

        //}

        strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                        " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                        " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                        " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
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
            drNew["item_status_id"] = dr["item_status_id"];
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["tmpCo"] = strTmp;
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
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (nTypeId == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbyLocation.rpt");
        else if (nTypeId == 2)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbyLocation.rpt");

        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dv);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;
        string strJob = string.Empty;
        //if (cus_est.job_number != null)
        //    strJob = cus_est.job_number;

        if (cus_est.job_number != null)
        {
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strJob = cus_est.job_number;
            else
                strJob = cus_est.alter_job_number;
        }

        int IsQty = 1;
        int IsUom = 2;

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
        ht.Add("p_Role", 1);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsUom", IsUom);
        ht.Add("p_Job", strJob);
        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        //Response.Redirect(@"Reports/Common/ReportViewer.aspx");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);


    }

    protected void btnPrintBySec_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintBySec.ID, btnPrintBySec.GetType().Name, "Click"); 
        GetData(2);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearchItemName.Text = "";
        lblResult.Text = "";
        Session.Remove("sItem_list");
        Session.Remove("sItem_list_Direct");
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        Calculate();
        if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
        {
            rdoSort.Visible = false;
            tblTotalProjectPrice.Visible = false;
            if (grdGrouping.Rows.Count == 0)
            {
                lblRetailPricingHeader.Visible = false;
            }
            else
            {
                lblRetailPricingHeader.Visible = true;
            }
            if (grdGroupingDirect.Rows.Count == 0)
            {
                lblDirectPricingHeader.Visible = false;
            }
            else
            {
                lblDirectPricingHeader.Visible = true;
            }
        }
        else
        {
            rdoSort.Visible = true;
            tblTotalProjectPrice.Visible = true;
            if (grdGrouping.FooterRow != null)
                grdGrouping.FooterRow.Visible = true;
            if (grdGroupingDirect.FooterRow != null)
                grdGroupingDirect.FooterRow.Visible = true;

            if (grdGrouping.Rows.Count == 0)
            {
                lblRetailPricingHeader.Visible = false;
            }
            else
            {
                lblRetailPricingHeader.Visible = true;
            }
            if (grdGroupingDirect.Rows.Count == 0)
            {
                lblDirectPricingHeader.Visible = false;
            }
            else
            {
                lblDirectPricingHeader.Visible = true;
            }
        }

    }
    public void BindSelectedSecORLoc()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            if (rdoSort.SelectedValue == "2")
            {
                string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
                DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
                if ((dtsec == null) || (dtsec.Rows.Count == 0))
                {
                    dtsec = LoadMasterTable();
                }
                string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                               " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =1  " +
                                " order by section_level asc";
                DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
                DataRow drNew = null;
                if (dtsec2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtsec2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();
                        bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists)  // if (!ContainDataRowInDataTable(dtsec, dr))
                        {
                            drNew = dtsec.NewRow();
                            drNew["colId"] = ncolId;
                            drNew["colName"] = strColName;
                            dtsec.Rows.Add(drNew);
                        }
                    }
                }
                Session.Add("MainDTSec", dtsec);

            }
            else
            {
                string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
                DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
                if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
                {
                    dtLoc = LoadMasterTable();
                }
                string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                                " INNER JOIN location on location.location_id = cop.location_id " +
                                " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                                " AND is_direct=1 AND cop.client_id =1 order by colName asc ";
                DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
                DataRow drNew = null;
                if (dtLoc2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtLoc2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();
                        bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists)  //  if (!ContainDataRowInDataTable(dtLoc, dr))
                        {
                            drNew = dtLoc.NewRow();
                            drNew["colId"] = ncolId;
                            drNew["colName"] = strColName;
                            dtLoc.Rows.Add(drNew);
                        }
                    }
                }
                Session.Add("MainDTLoc", dtLoc);

            }
        }
        catch (Exception ex)
        {
            string ext = ex.StackTrace;
            throw ex;
        }


    }
    public void BindSelectedSecORLoc_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (rdoSort.SelectedValue == "2")
        {
            string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
            if ((dtsec == null) || (dtsec.Rows.Count == 0))
            {
                dtsec = LoadMasterTable();
            }
            string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =1  " +
                            " order by section_level asc";
            DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
            DataRow drNew = null;
            if (dtsec2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtsec2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists)  //if (!ContainDataRowInDataTable(dtsec, dr))
                    {
                        drNew = dtsec.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtsec.Rows.Add(drNew);
                    }
                }
            }
            Session.Add("MainDTSecDirect", dtsec);

        }
        else
        {
            string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
            if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
            {
                dtLoc = LoadMasterTable();
            }
            string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                            " INNER JOIN location on location.location_id = cop.location_id " +
                            " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                            " AND is_direct = 2 AND cop.client_id =1 order by colName asc ";
            DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
            DataRow drNew = null;
            if (dtLoc2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtLoc2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists)  //if (!ContainDataRowInDataTable(dtLoc, dr))
                    {
                        drNew = dtLoc.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtLoc.Rows.Add(drNew);
                    }
                }
            }
            Session.Add("MainDTLocDirect", dtLoc);

        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        if (txtSearchItemName.Text.Trim() != "")
        {
            BindSelectedSecORLoc();
            BindSelectedSecORLoc_Direct();
            lblResult.Text = "";
            Session.Remove("sItem_list");
            Session.Remove("sItem_list_Direct");
            DataTable dtMainDTSec = new DataTable();
            DataTable dtMainDTLoc = new DataTable();
            DataTable dtMainDTSecDirect = new DataTable();
            DataTable dtMainDTLocDirect = new DataTable();

            DataTable dtItemdata = new DataTable();
            DataTable dtItemDirect = new DataTable();
            if (Session["Item_list"] != null)
            {
                dtItemdata = (DataTable)Session["Item_list"];
            }
            if (Session["Item_list_Direct"] != null)
            {
                dtItemDirect = (DataTable)Session["Item_list_Direct"];
            }
            if (Session["MainDTSec"] != null)
            {
                dtMainDTSec = (DataTable)Session["MainDTSec"];
            }
            if (Session["MainDTLoc"] != null)
            {
                dtMainDTLoc = (DataTable)Session["MainDTLoc"];
            }
            if (Session["MainDTSecDirect"] != null)
            {
                dtMainDTSecDirect = (DataTable)Session["MainDTSecDirect"];
            }
            if (Session["MainDTLocDirect"] != null)
            {
                dtMainDTLocDirect = (DataTable)Session["MainDTLocDirect"];
            }

            DataTable dtItemNew = new DataTable();
            DataTable dtItemNewDirect = new DataTable();

            string prefixText = txtSearchItemName.Text.Trim();
            if (prefixText.IndexOf(">>") != -1)
            {
                var corrected = prefixText.Substring(0, prefixText.Length - 2);
                if (prefixText.IndexOf(">>") != -1)
                {
                    prefixText = corrected.Substring(corrected.LastIndexOf(">>") + 2);
                }
                else
                {
                    prefixText = corrected;

                }
            }
            int nCustID = Convert.ToInt32(hdnCustomerId.Value);
            int nEstID = Convert.ToInt32(hdnEstimateId.Value);
            if (prefixText.IndexOf(",") > 0)
            {
                string strCondition = string.Empty;
                string[] strIds = prefixText.Split(',');
                foreach (string strId in strIds)
                {
                    if (strCondition.Length == 0)
                        strCondition = " section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";
                    else
                        strCondition += "or section_name LIKE '%" + strId.Trim() + "%'" + " or item_name LIKE '%" + strId.Trim() + "%'";

                }
                if (dtItemdata.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemdata.Select(strCondition);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNew = filteredRows.CopyToDataTable();
                        Session.Add("sItem_list", dtItemNew);

                    }
                }
                if (dtItemDirect.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemDirect.Select(strCondition);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNewDirect = filteredRows.CopyToDataTable();
                        Session.Add("sItem_listDirect", dtItemNewDirect);

                    }
                }

            }
            else
            {

                string strquery = "section_name LIKE '%" + prefixText.Trim() + "%'" + " or item_name LIKE '%" + prefixText.Trim() + "%'";
                if (dtItemdata.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemdata.Select(strquery);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNew = filteredRows.CopyToDataTable();
                        Session.Add("sItem_list", dtItemNew);

                    }
                }
                if (dtItemDirect.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemDirect.Select(strquery);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNewDirect = filteredRows.CopyToDataTable();
                        Session.Add("sItem_listDirect", dtItemNewDirect);

                    }
                }

            }

            if (rdoSort.SelectedValue == "1")
            {
                if (dtItemNew.Rows.Count > 0)
                {
                    DataTable dtTemp = dtMainDTLoc.Copy();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        bool Iexists = dtItemNew.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(ncolId)).Count() > 0;
                        if (!Iexists)
                        {
                            var rows = dtMainDTLoc.Select("colId =" + ncolId + "");
                            foreach (var row in rows)
                            {
                                row.Delete();
                                dtMainDTLoc.AcceptChanges();
                            }
                        }
                    }
                }
                if (dtItemNewDirect.Rows.Count > 0)
                {
                    DataTable dtTemp = dtMainDTLocDirect.Copy();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        bool Iexists = dtItemNewDirect.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(ncolId)).Count() > 0;
                        if (!Iexists)
                        {
                            var rows = dtMainDTLocDirect.Select("colId =" + ncolId + "");
                            foreach (var row in rows)
                            {
                                row.Delete();
                                dtMainDTLocDirect.AcceptChanges();
                            }
                        }
                    }
                }
                if (Session["sItem_list"] == null && Session["sItem_listDirect"] == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found");
                    return;
                }

                if (dtMainDTLoc.Rows.Count > 0)
                {
                    Session.Add("sMainDTLoc", dtMainDTLoc);
                    grdGrouping.DataSource = dtMainDTLoc;
                    grdGrouping.DataKeyNames = new string[] { "colId" };
                    grdGrouping.DataBind();
                }
                if (dtMainDTLocDirect.Rows.Count > 0)
                {
                    Session.Add("sMainDTLocDirect", dtMainDTLocDirect);
                    grdGroupingDirect.DataSource = dtMainDTLocDirect;
                    grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                    grdGroupingDirect.DataBind();
                }
            }
            else
            {
                if (Session["sItem_list"] == null && Session["sItem_listDirect"] == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("No record found");
                    return;
                }
                if (dtItemNew.Rows.Count > 0)
                {
                    DataTable dtTemp = dtMainDTSec.Copy();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        bool Iexists = dtItemNew.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                        if (!Iexists)
                        {
                            var rows = dtMainDTSec.Select("colId =" + ncolId + "");
                            foreach (var row in rows)
                            {
                                row.Delete();
                                dtMainDTSec.AcceptChanges();
                            }
                        }
                    }
                }
                if (dtItemNewDirect.Rows.Count > 0)
                {
                    DataTable dtTemp = dtMainDTSecDirect.Copy();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        bool Iexists = dtItemNewDirect.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                        if (!Iexists)
                        {
                            var rows = dtMainDTSecDirect.Select("colId =" + ncolId + "");
                            foreach (var row in rows)
                            {
                                row.Delete();
                                dtMainDTSecDirect.AcceptChanges();
                            }
                        }
                    }
                }

                if (dtMainDTSec.Rows.Count > 0)
                {
                    Session.Add("sMainDTSec", dtMainDTSec);
                    grdGrouping.DataSource = dtMainDTSec;
                    grdGrouping.DataKeyNames = new string[] { "colId" };
                    grdGrouping.DataBind();
                }
                if (dtMainDTSecDirect.Rows.Count > 0)
                {
                    Session.Add("sMainDTSecDirect", dtMainDTSecDirect);
                    grdGroupingDirect.DataSource = dtMainDTSecDirect;
                    grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                    grdGroupingDirect.DataBind();
                }
            }
        }

    }

    private void GetEstimate(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select * from customer_estimate where customer_id=" + nCustId + " and client_id= 1 and status_id = 3 order by estimate_id desc ";
        IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

        if (clist != null)
        {
            ddlEstimate.DataSource = clist;
            ddlEstimate.DataTextField = "estimate_name";
            ddlEstimate.DataValueField = "estimate_id";
            ddlEstimate.DataBind();

            if (Request.QueryString.Get("eid") != null)
            {
                ddlEstimate.SelectedValue = Request.QueryString.Get("eid").ToString();
            }

            lblCurrentEstimate.Visible = false;
            ddlEstimate.Visible = true;
        }
        else
        {
            lblCurrentEstimate.Text = clist.FirstOrDefault().estimate_name;
            lblCurrentEstimate.Visible = true;
            ddlEstimate.Visible = false;

        }


    }

    protected void ddlEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlEstimate.ID, ddlEstimate.GetType().Name, "SelectedIndexChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();


        int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
        int nEstimateId = Convert.ToInt32(ddlEstimate.SelectedItem.Value);

        Response.Redirect("customer_sow.aspx?eid=" + nEstimateId + "&cid=" + nCustomerId);

    }
}
