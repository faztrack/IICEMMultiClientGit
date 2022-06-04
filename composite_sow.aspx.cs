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

public partial class composite_sow : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    private double subtotalCost = 0.0;
    private double subtotalUnitCost = 0.0;
    private double subtotalLaborCost = 0.0;
    private double grandtotalLaborCost = 0.0;
    private double grandtotalUnitCost = 0.0;
    private double grandtotalCost = 0.0;
    private double subtotal_diectCost = 0.0;
    private double grandtotal_directCost = 0.0;
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
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();
                int nRoleId = oUser.role_id;
                if (nRoleId == 4 || nRoleId == 1)
                {
                    chkPMDisplay.Visible = true;
                    lblPm.Visible = true;
                }
                else
                {
                    chkPMDisplay.Visible = false;
                    lblPm.Visible = false;

                }

            }

            if (Page.User.IsInRole("admin039") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
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

            hyp_SowCost.NavigateUrl = "CompositeSOWCost.aspx?eid=" + nEstid + "&cid=" + nCid;
            hyp_SowCost.ToolTip = "Click here to view Composite SOW Cost";


            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value));
            if (com != null)
                hdnSuperExCostDecrease.Value = com.ExCostPercentage.ToString();
            else
                hdnSuperExCostDecrease.Value = "0";
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                Session.Add("cCustId", hdnCustomerId.Value);
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if(cust != null)
                {
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
                }

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
                   // hdnIsCash.Value = Convert.ToBoolean(cus_est.IsCashTerm).ToString();
                    //hdnJob.Value = cus_est.job_number;
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        hdnJob.Value = cus_est.job_number;
                    else
                        hdnJob.Value = cus_est.alter_job_number;
                    lblEstimateName.Text = cus_est.estimate_name;
                    ddlStatus.SelectedValue = cus_est.status_id.ToString();
                    ddlStatus.Enabled = false;
                    lblSaleDate.Visible = true;
                    txtSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                    txtSaleDate.Visible = true;
                    txtSaleDate.ReadOnly = true;
                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";
                    txtComments.ReadOnly = true;

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                    
                }

                BindProjectNotes();
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
                    userinfo obj = (userinfo)Session["oUser"];

                    if (obj.role_id == 1)
                    {
                        tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;


                    }
                    else if (obj.role_id == 4)
                    {
                        tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;
                    }
                    else if (obj.role_id == 5)
                    {
                        tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;
                    }
                    else
                    {
                        tblTotalProjectPrice.Visible = false;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = false;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = false;

                    }

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


            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkPMDisplay", "ddlStatus", "btnPrintWOCost", "btnSave" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdGrouping_lnkSendEmail1" });
        }
    }

    private void BindProjectNotes()
    {
        string sql = "select * from ProjectNoteInfo where  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and section_id in (-1,1,2) and isSOW=1 order by ProjectDate DESC ";
        DataTable  dt = csCommonUtility.GetDataTable(sql);
        grdGeneralProjectNotes.DataSource = dt;
        grdGeneralProjectNotes.DataKeyNames = new string[] { "ProjectNoteId", "customer_id", "is_complete", "SectionName" };
        grdGeneralProjectNotes.DataBind();
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
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
                                " AND is_direct=1 AND cop.client_id = " + Convert.ToInt32(hdnEstimateId.Value) + " order by colName asc ";
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
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =" + Convert.ToInt32(hdnEstimateId.Value) + "  " +
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
                            " AND is_direct = 2 AND cop.client_id = " + Convert.ToInt32(hdnEstimateId.Value) + " order by colName asc ";
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
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
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

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes
            //lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes

            ntotal_incentives = Convert.ToDecimal(esp.total_incentives);
            bincentives = Convert.ToBoolean(esp.is_incentives);


        }
        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == Convert.ToInt32(hdnClientId.Value)
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecievedAmount.Text = payAmount.ToString("c");

        //decimal COAmount = 0;
        //var Co_result = (from cpi in _db.change_order_pricing_lists
        //                 join cho in _db.changeorder_estimates on new { cpi.chage_order_id, cpi.customer_id, cpi.estimate_id } equals new { chage_order_id = cho.chage_order_id, customer_id = cho.customer_id, estimate_id = cho.estimate_id }
        //                 where cpi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cho.change_order_status_id == 3 && cho.change_order_type_id != 3 && cpi.client_id == 1
        //                 select cpi.EconomicsCost);

        //int co_AM = Co_result.Count();
        //if (result != null && co_AM > 0)
        //    COAmount = Co_result.Sum();

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
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
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
            Label lblshNote = (Label)e.Row.FindControl("lblshNote");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");

            if (lblshNote.Text != "" && lblshNote.Text.Length > 50)
            {
                lblshNote.ToolTip = lblshNote.Text;
                lblshNote.Text = lblshNote.Text.Substring(0, 50) + "...";
                lnkOpen.Visible = true;
            }
            else
            {
                lblshNote.ToolTip = lblshNote.Text;
                lnkOpen.Visible = false;
            }
            userinfo obj = (userinfo)Session["oUser"];
            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblT_price1 = (Label)e.Row.Cells[11].FindControl("lblT_price1");
            Label lblTotal_Cost1 = (Label)e.Row.Cells[11].FindControl("lblTotal_Cost1");
            Label lblTotal_Cost = (Label)e.Row.Cells[11].FindControl("lblTotal_Cost");
            Label lblTotalLabor_Cost1 = (Label)e.Row.Cells[11].FindControl("lblTotalLabor_Cost1");
            Label lblUnit_Cost1 = (Label)e.Row.Cells[11].FindControl("lblUnit_Cost1");
            Label lblTotal_price = (Label)e.Row.Cells[11].FindControl("lblTotal_price");
            Label lblDleted = (Label)e.Row.FindControl("lblDleted");

            if (obj.role_id == 4)
            {
                lblTotal_Cost.Visible = false;
            }
            else
            {
                lblTotal_Cost.Visible = true;
            }

            decimal dTotalPrice = Convert.ToDecimal(gv1.DataKeys[e.Row.RowIndex].Values[2]);
            
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
                e.Row.Cells[10].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[11].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
               // e.Row.Cells[12].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[13].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                e.Row.Cells[13].Text = "Item Deleted" + e.Row.Cells[10].Text;
                lblT_price1.Text = "0.00";
                lblTotal_Cost1.Text = "0.00";
                lblTotalLabor_Cost1.Text = "0.00";
                lblUnit_Cost1.Text = "0.00";

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
                    lblTotalLabor_Cost1.Text = "0.00";
                    lblUnit_Cost1.Text = "0.00";
                    lblTotal_Cost1.Text = "0.00";
                    lblT_price1.Text = "0.00";
                    lblDleted.Visible = true;
                    lblDleted.ForeColor = Color.Red;
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Cells[13].Text = "Item Added" + e.Row.Cells[13].Text;
                
            }
            else
            {
                e.Row.Cells[13].Text = "No Change";
            }

            if (chkPMDisplay.Checked)
            {
                gv1.Columns[6].Visible = true;
                gv1.Columns[9].Visible = true;
                gv1.Columns[10].Visible = true;
            }
            else
            {
                gv1.Columns[6].Visible = false;
                gv1.Columns[9].Visible = false;
                gv1.Columns[10].Visible = false;

            }


        }

    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes = gRow.Cells[2].Controls[0].FindControl("lblshNote") as Label;
        Label lblshort_notes_r = gRow.Cells[2].Controls[1].FindControl("lblshort_notes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblshort_notes.Visible = false;
            lblshort_notes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblshort_notes.Visible = true;
            lblshort_notes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";

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
        decimal TotalwithTax = 0;
        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        TotalwithTax = Convert.ToDecimal(nGrandtotal + tax_amount);

        lblRunning.Text = TotalwithTax.ToString("c");

        return "Total with Tax: " + TotalwithTax.ToString("c");

    }

    protected string GetTotalUnitCost()
    {
        decimal nGrandTotalUnitCost = Convert.ToDecimal(grandtotalUnitCost);
        return nGrandTotalUnitCost.ToString("c");
    }
    protected string GetTotalLaborCost()
    {
        decimal nGrandtotalLaborCost = Convert.ToDecimal(grandtotalLaborCost);
         return nGrandtotalLaborCost.ToString("c");
    }
    protected string GetTotalExtCost()
    {

        decimal nGrandtotalCost = Convert.ToDecimal(grandtotalCost);
        // string Total = string.Empty;
        //Total = "Total Ext. Cost: " + nGrandtotalCost.ToString("c");
        return nGrandtotalCost.ToString("c");
    }
    protected string GetTotalExPrice()
    {

        decimal nGrandtotal = Convert.ToDecimal(grandtotal);
        //string Total = string.Empty;
        //Total = "Total Ext. Price: " + nGrandtotal.ToString("c");
        return nGrandtotal.ToString("c");
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

    protected string GetTotalExtCostDirect()
    {
        decimal nGrandtotalCost = Convert.ToDecimal(grandtotal_directCost);
        return nGrandtotalCost.ToString("c");
    }
    protected string GetTotalExPriceDirect()
    {
        decimal nGrandtotal = Convert.ToDecimal(grandtotal_direct);
        return nGrandtotal.ToString("c");
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            LinkButton lnkSendEmail1 = (LinkButton)e.Row.FindControl("lnkSendEmail1");
            LinkButton inkProjectNotes = (LinkButton)e.Row.FindControl("inkProjectNotes");
            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail1.Visible = false;
                inkProjectNotes.Visible = false;

            }
            else
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                lnkSendEmail1.Visible = true;

                if(_db.ProjectNoteInfos.Any(p=>p.customer_id==Convert.ToInt32(hdnCustomerId.Value)&&p.section_id==colId))
                   inkProjectNotes.Visible = true;
                else
                    inkProjectNotes.Visible = false;
            }
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
                gv.DataBind();
            }
            else
            {
                int nDirectId = 1;
                GetData(colId, gv, nDirectId);
            }
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;

            userinfo obj = (userinfo)Session["oUser"];

            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
               
                subtotal += Double.Parse((row.FindControl("lblT_price1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");

                Label lblItemNameDetail = row.FindControl("lblItemNameDetail") as Label;
                
                if (obj.role_id == 4)
                {
                    if (lblItemNameDetail.Text.ToLower().Contains("allowance"))
                    {
                        subtotalCost -= Double.Parse((row.FindControl("lblTotal_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));  
                    }
                    else
                    {
                        subtotalCost += Double.Parse((row.FindControl("lblTotal_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    }
                   
                }
                else
                {
                    subtotalCost += Double.Parse((row.FindControl("lblTotal_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }

                
               // subtotalCost += Double.Parse((row.FindControl("lblTotal_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                //lblSubTotalCost.Text = subtotalCost.ToString("c");

                Label lblTotalUnitCost = footerRow.FindControl("lblTotalUnitCost") as Label;
                subtotalUnitCost += (Double.Parse((row.FindControl("lblUnit_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", "")) * Double.Parse((row.FindControl("lblQuantity") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", "")));
                lblTotalUnitCost.Text = subtotalUnitCost.ToString("c");

                Label lblSubTotalLaborCost = footerRow.FindControl("lblSubTotalLaborCost") as Label;
                subtotalLaborCost += Double.Parse((row.FindControl("lblTotalLabor_Cost1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                lblSubTotalLaborCost.Text = subtotalLaborCost.ToString("c");

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

            Label lblSubTotalCost = footerRow.FindControl("lblSubTotalCost") as Label;

            if (obj.role_id == 4)
            {
                if (Convert.ToDouble(hdnSuperExCostDecrease.Value) > 0)
                {
                    subtotalCost = (subtotalCost - (subtotalCost * Convert.ToDouble(hdnSuperExCostDecrease.Value)) / 100);
                    lblSubTotalCost.Text = subtotalCost.ToString("c");
                }
                else
                {
                    lblSubTotalCost.Text = subtotalCost.ToString("c");
                }
            }
            else
            {
                lblSubTotalCost.Text = subtotalCost.ToString("c");
            }

           

            grandtotalCost += subtotalCost;
            subtotalCost = 0.0;

            grandtotalLaborCost += subtotalLaborCost;
            subtotalLaborCost = 0.0;

            grandtotalUnitCost += subtotalUnitCost;
            subtotalUnitCost = 0.0;

           

            if (obj.role_id == 1)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 4)
            {
                gv.Columns[6].Visible = false;
                gv.Columns[9].Visible = false;
                gv.Columns[8].Visible = true;
                gv.Columns[11].Visible = false;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 5)
            {
                gv.Columns[11].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else
            {
                gv.Columns[11].Visible = false;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = false;
            }




        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            userinfo obj = (userinfo)Session["oUser"];
            Label lblGtotalExtCost = (Label)e.Row.FindControl("lblGtotalExtCost");
            Label lblGtotalExtPrice = (Label)e.Row.FindControl("lblGtotalExtPrice");
            Label lblGtotalLabel = (Label)e.Row.FindControl("lblGtotalLabel");
            Label lblGtrandTotalLaborCost = (Label)e.Row.FindControl("lblGtrandTotalLaborCost");
            Label lblGrandTotalUnitCost = (Label)e.Row.FindControl("lblGrandTotalUnitCost");
            Panel pnlAdminView = (Panel)e.Row.FindControl("pnlAdminView");
            Panel pnlSuperView = (Panel)e.Row.FindControl("pnlSuperView");
            Panel pnlAdminView2 = (Panel)e.Row.FindControl("pnlAdminView2");
            if (chkPMDisplay.Checked == true)
            {
                if (obj.role_id == 1)
                {
                    lblGtotalExtPrice.Visible = true;
                    lblGtotalExtCost.Visible = true;
                    lblGtotalLabel.Visible = true;
                    lblGtrandTotalLaborCost.Visible = true;
                    lblGrandTotalUnitCost.Visible = true;
                    pnlAdminView.Visible = true;
                    pnlAdminView2.Visible = true;
                    pnlSuperView.Visible = false;
                }
                else
                {
                    lblGtotalExtPrice.Visible = true;
                    lblGtotalExtCost.Visible = true;
                    lblGtotalLabel.Visible = true;
                    pnlAdminView.Visible = false;
                    pnlAdminView2.Visible = false;
                    pnlSuperView.Visible = true;
                   
                }
               

            }
            else
            {
                lblGtotalExtPrice.Visible = false;
                lblGtotalExtCost.Visible = false;
                lblGtotalLabel.Visible = false;
                lblGtrandTotalLaborCost.Visible = false;
                lblGrandTotalUnitCost.Visible = false;

            }

        }

    }

    protected void chkPMDisplay_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkPMDisplay.ID, chkPMDisplay.GetType().Name, "Click"); 
        try
        {
            
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
        catch (Exception ex)
        {
            //lblResult1.Text = ex.Message;
            //lblResult1.ForeColor = System.Drawing.Color.Red;
        }
    }

   
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        userinfo obj = (userinfo)Session["oUser"];
        if (rdoSort.SelectedValue == "1")
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial,0.00 as unit_cost,0.00 as total_labor_cost,0.00 as total_unit_cost, " +
                    " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                    " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = " + Convert.ToInt32(hdnEstimateId.Value) + " ) " +
                   " order by section_level";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;

                decimal dTPrice = 0;

                string sItemName = dr["item_name"].ToString();

                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);

                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                    }

                    dLabor_rate = 0;

                }
                else
                {
                    if (dRetail_multiplier > 0)
                    {

                        dOrginalCost = (dItemCost / dRetail_multiplier);
                    }
                    else
                    {
                        dOrginalCost = dItemCost;
                    }
                }

                dOrginalTotalCost = dOrginalCost * dQuantity;
                dLaborTotal = dLabor_rate * dQuantity;
                dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                dr["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                dr["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                dr["total_unit_cost"] = dLineItemTotal;
                //if (obj.role_id == 4)
                //{
                //    if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                //    {
                //        if (sItemName.Contains("Allowance"))
                //        {
                //            dr["total_unit_cost"] = "0.00";
                //        }
                //        else
                //        {
                //            dr["total_unit_cost"] = (dLineItemTotal - (dLineItemTotal * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100); // Orginal Cost Total
                //        }
                //    }
                //    else
                //    {
                //        dr["total_unit_cost"] = dLineItemTotal;
                //    }
                //}
                //else
                //{
                //    dr["total_unit_cost"] = dLineItemTotal;
                //}


            }
            DataView dv2 = dt.DefaultView;
            dt = dv2.ToTable();

            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial,0.00 as unit_cost,0.00 as total_labor_cost,0.00 as total_unit_cost," +
                           " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,short_notes_new,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name, IsNull(changeorder_estimate.execute_date, '01/01/2000') as execute_date " +
                           " FROM change_order_pricing_list " +
                           " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                           " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level ";
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
               // drNew["item_cost"] = dr["item_cost"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["minimum_qty"] = dr["minimum_qty"];
                drNew["quantity"] = dr["quantity"];
                //drNew["retail_multiplier"] = dr["retail_multiplier"];
               // drNew["labor_rate"] = dr["labor_rate"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["short_notes_new"] = dr["short_notes_new"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;

                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;
                decimal dQuantity = 0;
                decimal dLabor_rate = 0;
                decimal dItemCost = 0;
                decimal dRetail_multiplier = 0;

                decimal dTPrice = 0;
                string sItemName = dr["item_name"].ToString();

                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);
                dQuantity = Convert.ToDecimal(dr["quantity"]);

                try
                {
                    if (Convert.ToInt32(dr["item_status_id"]) == 3)
                    {

                        co_pricing_master objP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                        if (objP != null)
                        {
                            drNew["retail_multiplier"] = objP.retail_multiplier;
                            drNew["labor_rate"] = objP.labor_rate;
                            drNew["item_cost"] = objP.item_cost;
                            dItemCost = Convert.ToDecimal(objP.item_cost);
                            dRetail_multiplier = Convert.ToDecimal(objP.retail_multiplier);
                            dLabor_rate = Convert.ToDecimal(objP.labor_rate);
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                if (dQuantity > 0)
                                {
                                    dOrginalCost = (objP.total_retail_price / dQuantity) / 2;
                                }
                                dLabor_rate = 0;
                            }
                            else
                            {
                                if (dRetail_multiplier > 0)
                                {

                                    dOrginalCost = (dItemCost / dRetail_multiplier);
                                }
                                else
                                {
                                    dOrginalCost = dItemCost;
                                }
                            }
                        }
                        else
                        {
                            //Deleted Later Items
                            var result = (from ppi in _db.co_pricing_masters
                                          where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"])
                                          select ppi.total_direct_price);
                            int n = result.Count();
                            if (result == null || n == 0)
                            {

                                if (sItemName.IndexOf("Other") > 0)
                                {
                                    co_pricing_master objPP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                                    if (dQuantity > 0)
                                    {
                                        dOrginalCost = (objPP.total_retail_price / dQuantity) / 2;
                                    }
                                    dLabor_rate = 0;
                                }
                                else
                                {
                                    item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                                    if (objI != null)
                                    {
                                        dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                        dOrginalCost = Convert.ToDecimal(objI.item_cost);
                                    }
                                }
                            }

                        }
                    }
                    else if (Convert.ToInt32(dr["item_status_id"]) == 2)
                    {

                        if (sItemName.IndexOf("Other") > 0 || sItemName.Contains("Other"))
                        {
                            change_order_pricing_list objPP = _db.change_order_pricing_lists.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                            if (dQuantity > 0)
                            {
                                dOrginalCost = (Convert.ToDecimal(objPP.total_retail_price) / dQuantity) / 2;
                            }
                            dLabor_rate = 0;
                        }
                        else
                        {
                            item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                            if (objI != null)
                            {
                                dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                dOrginalCost = Convert.ToDecimal(objI.item_cost);
                            }
                        }
                    }
                    dOrginalTotalCost = dOrginalCost * dQuantity;
                    dLaborTotal = dLabor_rate * dQuantity;
                    dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                    drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                    drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                    drNew["total_unit_cost"] = dLineItemTotal;
                    //if (obj.role_id == 4)
                    //{
                    //    if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                    //    {
                    //        drNew["total_unit_cost"] = (dLineItemTotal - (dLineItemTotal * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100); // Orginal Cost Total
                    //    }
                    //    else
                    //    {
                    //        drNew["total_unit_cost"] = dLineItemTotal;
                    //    }
                    //}
                    //else
                    //{
                    //    drNew["total_unit_cost"] = dLineItemTotal;
                    //}
                    dt.Rows.Add(drNew);
                }
                catch
                {
                }

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
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
            grd.DataBind();
        }
        else
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial,0.00 as unit_cost,0.00 as total_labor_cost,0.00 as total_unit_cost, " +
                    " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo  " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = " + Convert.ToInt32(hdnEstimateId.Value) + " ) " +
                   " order by location.location_name";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;

                decimal dTPrice = 0;

                string sItemName = dr["item_name"].ToString();

                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);

                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                    }

                    dLabor_rate = 0;

                }
                else
                {
                    if (dRetail_multiplier > 0)
                    {

                        dOrginalCost = (dItemCost / dRetail_multiplier);
                    }
                    else
                    {
                        dOrginalCost = dItemCost;
                    }
                }

                dOrginalTotalCost = dOrginalCost * dQuantity;
                dLaborTotal = dLabor_rate * dQuantity;
                dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                dr["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                dr["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                dr["total_unit_cost"] = dLineItemTotal;
                //if (obj.role_id == 4)
                //{
                //    if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                //    {
                //        dr["total_unit_cost"] = (dLineItemTotal - (dLineItemTotal * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100); // Orginal Cost Total
                //    }
                //    else
                //    {
                //        dr["total_unit_cost"] = dLineItemTotal;
                //    }
                //}
                //else
                //{
                //    dr["total_unit_cost"] = dLineItemTotal;
                //}


            }
            DataView dv2 = dt.DefaultView;
            dt = dv2.ToTable();

            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial,0.00 as unit_cost,0.00 as total_labor_cost,0.00 as total_unit_cost, " +
                           " location.location_name AS section_name,section_name AS location_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,short_notes_new,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,IsNull(changeorder_estimate.execute_date, '01/01/2000') as execute_date " +
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
                // drNew["item_cost"] = dr["item_cost"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["minimum_qty"] = dr["minimum_qty"];
                drNew["quantity"] = dr["quantity"];
                //drNew["retail_multiplier"] = dr["retail_multiplier"];
                // drNew["labor_rate"] = dr["labor_rate"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["short_notes_new"] = dr["short_notes_new"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;

                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;
                decimal dQuantity = 0;
                decimal dLabor_rate = 0;
                decimal dItemCost = 0;
                decimal dRetail_multiplier = 0;

                decimal dTPrice = 0;
                string sItemName = dr["item_name"].ToString();

                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);
                dQuantity = Convert.ToDecimal(dr["quantity"]);

                try
                {
                    if (Convert.ToInt32(dr["item_status_id"]) == 3)
                    {

                        co_pricing_master objP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                        if (objP != null)
                        {
                            drNew["retail_multiplier"] = objP.retail_multiplier;
                            drNew["labor_rate"] = objP.labor_rate;
                            drNew["item_cost"] = objP.item_cost;
                            dItemCost = Convert.ToDecimal(objP.item_cost);
                            dRetail_multiplier = Convert.ToDecimal(objP.retail_multiplier);
                            dLabor_rate = Convert.ToDecimal(objP.labor_rate);
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                if (dQuantity > 0)
                                {
                                    dOrginalCost = (objP.total_retail_price / dQuantity) / 2;
                                }
                                dLabor_rate = 0;
                            }
                            else
                            {
                                if (dRetail_multiplier > 0)
                                {

                                    dOrginalCost = (dItemCost / dRetail_multiplier);
                                }
                                else
                                {
                                    dOrginalCost = dItemCost;
                                }
                            }
                        }
                        else
                        {
                            //Deleted Later Items
                            var result = (from ppi in _db.co_pricing_masters
                                          where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"])
                                          select ppi.total_direct_price);
                            int n = result.Count();
                            if (result == null || n == 0)
                            {

                                if (sItemName.IndexOf("Other") > 0)
                                {
                                    co_pricing_master objPP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                                    if (dQuantity > 0)
                                    {
                                        dOrginalCost = (objPP.total_retail_price / dQuantity) / 2;
                                    }
                                    dLabor_rate = 0;
                                }
                                else
                                {
                                    item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                                    if (objI != null)
                                    {
                                        dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                        dOrginalCost = Convert.ToDecimal(objI.item_cost);
                                    }
                                }
                            }

                        }
                    }
                    else if (Convert.ToInt32(dr["item_status_id"]) == 2)
                    {

                        if (sItemName.IndexOf("Other") > 0 || sItemName.Contains("Other"))
                        {
                            change_order_pricing_list objPP = _db.change_order_pricing_lists.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                            if (dQuantity > 0)
                            {
                                dOrginalCost = (Convert.ToDecimal(objPP.total_retail_price) / dQuantity) / 2;
                            }
                            dLabor_rate = 0;
                        }
                        else
                        {
                            item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                            if (objI != null)
                            {
                                dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                dOrginalCost = Convert.ToDecimal(objI.item_cost);
                            }
                        }
                    }
                    dOrginalTotalCost = dOrginalCost * dQuantity;
                    dLaborTotal = dLabor_rate * dQuantity;
                    dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                    drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                    drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                    drNew["total_unit_cost"] = dLineItemTotal;
                    //if (obj.role_id == 4)
                    //{
                    //    if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                    //    {
                    //        drNew["total_unit_cost"] = (dLineItemTotal - (dLineItemTotal * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100); // Orginal Cost Total
                    //    }
                    //    else
                    //    {
                    //        drNew["total_unit_cost"] = dLineItemTotal;
                    //    }
                    //}
                    //else
                    //{
                    //    drNew["total_unit_cost"] = dLineItemTotal;
                    //}
                    dt.Rows.Add(drNew);
                }
                catch
                {
                }
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
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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
            Label lblDleted2 = (Label)e.Row.FindControl("lblDleted2");

            decimal dDirectPrice = Convert.ToDecimal(gv2.DataKeys[e.Row.RowIndex].Values[3]);
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
                e.Row.Cells[13].Text = "No Change";
            }
            if (chkPMDisplay.Checked)
            {
                gv2.Columns[6].Visible = true;
                gv2.Columns[9].Visible = true;
                gv2.Columns[10].Visible = true;
            }
            else
            {
                gv2.Columns[6].Visible = false;
                gv2.Columns[9].Visible = false;
                gv2.Columns[10].Visible = false;

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
            LinkButton lnkSendEmail2 = (LinkButton)e.Row.FindControl("lnkSendEmail2");
            LinkButton inkProjectNotes2 = (LinkButton)e.Row.FindControl("inkProjectNotes2");
            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail2.Visible = false;
                inkProjectNotes2.Visible = false;
            }
            else
            {
                lnkSendEmail2.Visible = true;
                inkProjectNotes2.Visible = true;
            }
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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

            userinfo obj = (userinfo)Session["oUser"];

            if (obj.role_id == 1)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 4)
            {
               
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 5)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else
            {
                gv.Columns[8].Visible = false;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = false;
            }

        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblGtotalExtCost = (Label)e.Row.FindControl("lblGtotalExtCost2");
            Label lblGtotalExtPrice = (Label)e.Row.FindControl("lblGtotalExtPrice2");
            Label lblGtotalLabel = (Label)e.Row.FindControl("lblGtotalLabel2");
            if (chkPMDisplay.Checked == true)
            {
                lblGtotalExtPrice.Visible = true;
                lblGtotalExtCost.Visible = true;
                lblGtotalLabel.Visible = true;

            }
            else
            {
                lblGtotalExtPrice.Visible = false;
                lblGtotalExtCost.Visible = false;
                lblGtotalLabel.Visible = false;

            }

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
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
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
       
       

        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                       " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                      " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                      " is_direct,section_level,location.location_id,'' as tmpCo,short_notes_new " +
                      " FROM pricing_details " +
                      " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                      " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                      " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                      " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                      " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                       " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = " + Convert.ToInt32(hdnEstimateId.Value) + " ) " +
                      "order by section_level";
        DataTable dtP = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = string.Empty;
      

         strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                         " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                         " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                         " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date,short_notes_new " +
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
            drNew["short_notes_new"] = dr["short_notes_new"];
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
        userinfo obj = (userinfo)Session["oUser"];
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (nTypeId == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbyLocation.rpt");
        else if (nTypeId == 2)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbySection.rpt");
        else if (nTypeId == 3)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbyLocationWOPrice.rpt");
        else if (nTypeId == 4)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWbySectionWOPrice.rpt");
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
        if (cus_est.job_number != null) {
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strJob = cus_est.job_number;
            else
                strJob = cus_est.alter_job_number;
            //strJob = cus_est.job_number;
        }
           

        int IsQty = 0;
        int IsUom = 0;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsUom = 2;

            }
        }

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
        if (obj.role_id == 1)
        {
            ht.Add("p_Role", 1);
        }
        else if (obj.role_id == 4)
        {
            ht.Add("p_Role", 1);
        }
        else if (obj.role_id == 5)
        {
            ht.Add("p_Role", 1);
        }
        else
        {

            ht.Add("p_Role", obj.role_id);
        }
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
    protected void btnPrintWOCost_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintWOCost.ID, btnPrintWOCost.GetType().Name, "Click"); 
        if (rdoSort.SelectedValue == "2")
            GetData(4);
        else
            GetData(3);


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
            userinfo obj = (userinfo)Session["oUser"];

            if (obj.role_id == 1)
            {
                tblTotalProjectPrice.Visible = true;
                if (grdGrouping.FooterRow != null)
                    grdGrouping.FooterRow.Visible = true;
                if (grdGroupingDirect.FooterRow != null)
                    grdGroupingDirect.FooterRow.Visible = true;


            }
            else if (obj.role_id == 4)
            {
                tblTotalProjectPrice.Visible = true;
                if (grdGrouping.FooterRow != null)
                    grdGrouping.FooterRow.Visible = true;
                if (grdGroupingDirect.FooterRow != null)
                    grdGroupingDirect.FooterRow.Visible = true;
            }
            else if (obj.role_id == 5)
            {
                tblTotalProjectPrice.Visible = true;
                if (grdGrouping.FooterRow != null)
                    grdGrouping.FooterRow.Visible = true;
                if (grdGroupingDirect.FooterRow != null)
                    grdGroupingDirect.FooterRow.Visible = true;
            }
            else
            {
                tblTotalProjectPrice.Visible = false;
                if (grdGrouping.FooterRow != null)
                    grdGrouping.FooterRow.Visible = false;
                if (grdGroupingDirect.FooterRow != null)
                    grdGroupingDirect.FooterRow.Visible = false;

            }

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
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + 
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
                                " AND is_direct=1 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + " order by colName asc ";
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
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value)  +
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
                            " AND is_direct = 2 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + " order by colName asc ";
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

    protected void grdGrouping_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "sEmail")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);

            string secName = "";
            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;
            string sSubject = "Request for Bid - " + secName + " (" + hdnLastName.Value + ": " + hdnJob.Value + ")";
            hdnSecName.Value = secName;
            Session.Add("sSubject", sSubject);
            string sFileName = CreateSectionReportForEMail(nSecId);
            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                //strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                //            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

                //string strQ = string.Empty;

                strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                         " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                         " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                         " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                         " AND section_level=" + nSecId + "  AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value) + "  " +
                         " UNION " +
                         " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                         " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                         " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                         " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                         " AND section_level=" + nSecId + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value);

                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                if (CList.Count == 0)
                {
                    strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                     " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                     " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


                    CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                }
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));

            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }
        if (e.CommandName == "projectNotes")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            modProjectNotes.Show();

            string sql = "select * from ProjectNoteInfo where  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and section_id=" + nSecId + " and isSOW=1 order by ProjectDate DESC ";
            dt = csCommonUtility.GetDataTable(sql);
            grdProjectNote.DataSource = dt;
            grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "customer_id", "is_complete", "SectionName" };
            grdProjectNote.DataBind();
           
        }

    }


    protected void grdGroupingDirect_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "sEmail2")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGroupingDirect.DataKeys[index].Values[0]);

            string secName = "";
            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;
            string sSubject = "Request for Bid - " + secName + " (" + hdnLastName.Value + ": " + hdnJob.Value + ")";
            hdnSecName.Value = secName;
            Session.Add("sSubject", sSubject);
            string sFileName = CreateSectionReportForEMail(nSecId);
            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                //strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                //            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

                strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                         " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                         " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                         " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                         " AND section_level=" + nSecId + "  AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value) + "  " +
                         " UNION " +
                         " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                         " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                         " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                         " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                         " AND section_level=" + nSecId + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value);

                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                if (CList.Count == 0)
                {
                    strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                     " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                     " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


                    CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                }

                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }

        if (e.CommandName == "projectNotes2")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGroupingDirect.DataKeys[index].Values[0]);
            modProjectNotes.Show();

            string sql = "select * from ProjectNoteInfo where  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and section_id=" + nSecId + " and isSOW=1 order by ProjectDate DESC ";
            dt = csCommonUtility.GetDataTable(sql);
            grdProjectNote.DataSource = dt;
            grdProjectNote.DataKeyNames = new string[] { "ProjectNoteId", "customer_id", "is_complete", "SectionName" };
            grdProjectNote.DataBind();

        }

    }

    string CreateHtml(DataTable dt, int ncid, string secName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        // Customer Address
        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == ncid);
        string strCustomer = cust.last_name1;
        string strAddress = cust.city + ", " + cust.state + " " + cust.zip_code;
        string address = cust.city + ",+" + cust.state + ",+" + cust.zip_code;
        string strPO = "";
        string strEmailSignature = "";
        string strCompanyName = "";

        company_profile com = new company_profile();
        if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            com = _db.company_profiles.Single(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value));
            strCompanyName = com.company_name;
        }

        userinfo obj = new userinfo();
        if ((userinfo)Session["oUser"] != null)
        {
            obj = (userinfo)Session["oUser"];
        }
        if (obj.user_id > 0)
        {
            user_info objuser = _db.user_infos.Where(u => u.user_id == obj.user_id).SingleOrDefault();

            if (objuser != null)
            {
                strEmailSignature = objuser.EmailSignature ?? "";
            }
        }
        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
        {
            customer_estimate cus_est = new customer_estimate();
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
           // strPO = cus_est.job_number;
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strPO = cus_est.job_number;
            else
                strPO = cus_est.alter_job_number;
        }

        string url = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

        DataView dvFinal = dt.DefaultView;
        //  dvFinal.Sort = "location_name,week_id";
        string strHTML = "Dear Vendor:<br/><br/>";

        strHTML += strCompanyName + " is requesting a bid for the following (see PDF attachment). Please reply to this email with the amount and any comment as soon as possible. Please attach any document if necessary. <br/>  Thank you for your participation.<br/><br/>";

        strHTML += strEmailSignature;

        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='center'> <tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Last Name: " + strCustomer + "</p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'><a style='color:#000;' target='_blank' href='" + url + "'>Job Location: " + strAddress + "</a></p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Job Number: " + strPO + "</p> </td></tr>";
        strHTML += " <tr style='background-color:#333333; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'><td align='left' valign='top'><table width='100%' border='0' cellspacing='1' cellpadding='5' > <tr style='background-color:#333333; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='6%'>SL</td><td  width='12%' >Section</td><td width='12%'>Location</td><td width='40%'>Item Name</td><td width='5%'>UoM</td><td align='right'width='5%'>Code</td><td width='20%'>Short Notes</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string strColor = "";

            if (i % 2 == 0)
                strColor = "background-color:#f0eae8; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";

            strHTML += "<tr style='" + strColor + "'><td>" + dr["section_serial"].ToString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["item_name"].ToString() + "</td><td>" + dr["measure_unit"].ToString() + "</td><td align='right'>" + Convert.ToInt32(dr["quantity"]).ToString() + "</td><td>" + dr["short_notes"].ToString() + "</td></tr>";

        }
        strHTML += "</table> </td></tr>";
        strHTML += "</table><br/><br/><br/>";

        strHTML += "<b>Sub-contractors guidelines:</b><br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1. All sub-contractors are expected to arrive within the scheduled window, if not, a phone call must be made 24 hours in advance to the job superintendent.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2. All sub-contractors are expected to have a clean and professional appearance.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3. Please do not discuss any details with the homeowner directly. Please talk to the job superintendent.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;4. There is no smoking or vaping allowed on homeowner property. Please dispose of cigarettes properly prior to arriving at the job site.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;5. The use of foul language is not permitted on homeowner property.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;6. All sub-contractors are expected to clean (sweep up, pick up all trash and material) at the end of the day.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;7. If there is an issue/problem that will affect the time frame of the project, please notify the job superintendent immediately.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;8. Any finished surfaces must be protected by floor covering or booties must be worn.<br/>";
        strHTML += " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;9. Please do not use a radio/music if the homeowner is home, and volume should be kept down, so neighbors are not disturbed.<br/>";
        strHTML += "&nbsp;&nbsp;&nbsp;&nbsp;10. Cell phone use should be limited to outside use only, and only in an emergency basis. The homeowner should feel that your attention to details will not be second to your phone calls.<br/>";
        strHTML += "&nbsp;&nbsp;&nbsp;&nbsp;11. Vehicles are not allowed on homeowner driveways, please park all vehicles in the street.<br/><br/>";
        strHTML += "We appreciate your cooperation and thank you for your partnership in ensuring that the customer can have the greatest experience during a time that can be stressful for them.";
        return strHTML;
    }

    private string CreateSectionReportForEMail(int nSecId)
    {
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));


        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                       " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                       " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                       " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND section_level=" + nSecId + "  AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value) + "  " +
                       " UNION " +
                       " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
                       " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
                       " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                       " AND section_level=" + nSecId + "  AND  estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
             " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
             " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
             " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
             " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }
        //strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
        //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
        //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


        //List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();


        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptEmailReportBySection.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
       // string strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName2 = "";
        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
            strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        else
            strCustomerName2 = cus_est.alter_job_number;
        string strCustomerName = objCust.last_name1; // objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

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

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        try
        {
            rptFile = (ReportDocument)Session[SessionInfo.Report_File];
            bool bParam = false;
            foreach (string strKey in Session.Keys)
            {
                if (strKey == SessionInfo.Report_Param)
                {
                    bParam = true;
                    break;
                }
            }
            if (bParam)
            {
                Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                ParameterValues param = new ParameterValues();
                ParameterDiscreteValue Val = new ParameterDiscreteValue();
                foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                {
                    if (htable.ContainsKey(obj.Name))
                    {
                        Val.Value = htable[obj.Name].ToString();
                        param.Add(Val);
                        obj.ApplyCurrentValues(param);
                    }
                }
            }
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

            return exportReportForSection(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }


        return "";
    }
    protected string exportReportForSection(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string strFile = "";
        try
        {
            string contentType = "";
            //strFile = "SECTION_" + DateTime.Now.Ticks;
            strFile = hdnLastName.Value + "-" + hdnSecName.Value + "_" + DateTime.Now.Ticks;

            // string tempFileName = Request.PhysicalApplicationPath + "tmp\\ChangeOrder\\";
            string tempFileName = Server.MapPath("tmp\\Contract") + @"\" + strFile;
            switch (eft)
            {
                case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                    tempFileName = tempFileName + ".pdf";
                    contentType = "application/pdf";
                    break;
            }
            CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
            dfo.DiskFileName = tempFileName;
            CrystalDecisions.Shared.ExportOptions eo = selectedReport.ExportOptions;
            eo.DestinationOptions = dfo;
            eo.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
            eo.ExportFormatType = eft;
            selectedReport.Export();

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strFile;
    }


    protected void btnExpCostLocList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCostLocList.ID, btnExpCostLocList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        userinfo obj = (userinfo)Session["oUser"];
        string strP = string.Empty;
        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, 0.0 as unit_cost,0.0 as total_labor_cost,0.0 as total_unit_cost," +
                       " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                      " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
                      " is_direct,section_level,location.location_id,'' as tmpCo " +
                      " FROM pricing_details " +
                      " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                      " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                      " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                      " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                      " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                       " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = " + Convert.ToInt32(hdnClientId.Value)  + " ) "+
                      " order by section_level";
        DataTable dtP = csCommonUtility.GetDataTable(strP);
        foreach (DataRow dr in dtP.Rows)
        {
            decimal dOrginalCost = 0;
            decimal dOrginalTotalCost = 0;
            decimal dLaborTotal = 0;
            decimal dLineItemTotal = 0;

            decimal dTPrice = 0;

            string sItemName = dr["item_name"].ToString();

            decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
            decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
            decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
            decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
           dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
            

            if (sItemName.IndexOf("Other") > 0)
            {
                if (dQuantity > 0)
                {
                    dOrginalCost = (dTPrice / dQuantity) / 2;
                }

                dLabor_rate = 0;

            }
            else
            {
                if (dRetail_multiplier > 0)
                {

                    dOrginalCost = (dItemCost / dRetail_multiplier);
                }
                else
                {
                    dOrginalCost = dItemCost;
                }
            }

            dOrginalTotalCost = dOrginalCost * dQuantity;
            dLaborTotal = dLabor_rate * dQuantity;
            dLineItemTotal = dOrginalTotalCost + dLaborTotal;
            dr["unit_cost"] = dOrginalCost; // Orginal Unit Cost
            dr["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
            dr["total_unit_cost"] = dLineItemTotal;
         


        }
        DataView dv = dtP.DefaultView;
        dtP = dv.ToTable();

        DataRow drNew2 = null;
        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial,0.0 as unit_cost,0.0 as total_labor_cost,0.0 as total_unit_cost, " +
                        " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                        " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,short_notes_new,item_status_id,last_update_date, " +
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

            drNew2 = dtP.NewRow();
            drNew2["co_pricing_list_id"] = dr["co_pricing_list_id"];
            drNew2["item_id"] = dr["item_id"];
            drNew2["labor_id"] = dr["labor_id"];
            drNew2["section_serial"] = dr["section_serial"];
            drNew2["location_name"] = dr["location_name"];
            drNew2["section_name"] = dr["section_name"];
            drNew2["item_name"] = dr["item_name"];
            drNew2["measure_unit"] = dr["measure_unit"];
            drNew2["item_cost"] = dr["item_cost"];
            drNew2["total_retail_price"] = dr["total_retail_price"];
            drNew2["total_direct_price"] = dr["total_direct_price"];
            drNew2["minimum_qty"] = dr["minimum_qty"];
            drNew2["quantity"] = dr["quantity"];
            drNew2["retail_multiplier"] = dr["retail_multiplier"];
            drNew2["labor_rate"] = dr["labor_rate"];
            drNew2["short_notes_new"] = dr["short_notes_new"];
            drNew2["short_notes"] = dr["short_notes"];
            drNew2["item_status_id"] = dr["item_status_id"];
            drNew2["last_update_date"] = dr["last_update_date"];
            drNew2["is_direct"] = dr["is_direct"];
            drNew2["section_level"] = dr["section_level"];
            drNew2["location_id"] = dr["location_id"];
            drNew2["tmpCo"] = strTmp;
            
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;
                decimal dQuantity = 0;
                decimal dLabor_rate = 0;
                decimal dItemCost = 0;
                decimal dRetail_multiplier = 0;

                decimal dTPrice = 0;
                string sItemName = dr["item_name"].ToString();

               dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                
                dQuantity = Convert.ToDecimal(dr["quantity"]);

                try
                {
                    if (Convert.ToInt32(dr["item_status_id"]) == 3)
                    {

                        co_pricing_master objP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                        if (objP != null)
                        {
                            drNew2["retail_multiplier"] = objP.retail_multiplier;
                            drNew2["labor_rate"] = objP.labor_rate;
                            drNew2["item_cost"] = objP.item_cost;
                            dItemCost = Convert.ToDecimal(objP.item_cost);
                            dRetail_multiplier = Convert.ToDecimal(objP.retail_multiplier);
                            dLabor_rate = Convert.ToDecimal(objP.labor_rate);
                            if (sItemName.IndexOf("Other") > 0)
                            {
                                if (dQuantity > 0)
                                {
                                    dOrginalCost = (objP.total_retail_price / dQuantity) / 2;
                                }
                                dLabor_rate = 0;
                            }
                            else
                            {
                                if (dRetail_multiplier > 0)
                                {

                                    dOrginalCost = (dItemCost / dRetail_multiplier);
                                }
                                else
                                {
                                    dOrginalCost = dItemCost;
                                }
                            }
                        }
                        else
                        {
                            //Deleted Later Items
                            var result = (from ppi in _db.co_pricing_masters
                                          where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"])
                                          select ppi.total_direct_price);
                            int n = result.Count();
                            if (result == null || n == 0)
                            {

                                if (sItemName.IndexOf("Other") > 0)
                                {
                                    co_pricing_master objPP = _db.co_pricing_masters.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                                    if (dQuantity > 0)
                                    {
                                        dOrginalCost = (objPP.total_retail_price / dQuantity) / 2;
                                    }
                                    dLabor_rate = 0;
                                }
                                else
                                {
                                    item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                                    if (objI != null)
                                    {
                                        dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                        dOrginalCost = Convert.ToDecimal(objI.item_cost);
                                    }
                                }
                            }

                        }
                    }
                    else if (Convert.ToInt32(dr["item_status_id"]) == 2)
                    {

                        if (sItemName.IndexOf("Other") > 0 || sItemName.Contains("Other"))
                        {
                            change_order_pricing_list objPP = _db.change_order_pricing_lists.SingleOrDefault(c => c.co_pricing_list_id == Convert.ToInt32(dr["co_pricing_list_id"]) && c.item_id == Convert.ToInt32(dr["item_id"]) && c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                            if (dQuantity > 0)
                            {
                                dOrginalCost = (Convert.ToDecimal(objPP.total_retail_price) / dQuantity) / 2;
                            }
                            dLabor_rate = 0;
                        }
                        else
                        {
                            item_price objI = _db.item_prices.SingleOrDefault(c => c.item_id == Convert.ToInt32(dr["item_id"]));
                            if (objI != null)
                            {
                                dLabor_rate = Convert.ToDecimal(objI.labor_rate);
                                dOrginalCost = Convert.ToDecimal(objI.item_cost);
                            }
                        }
                    }
                    dOrginalTotalCost = dOrginalCost * dQuantity;
                    dLaborTotal = dLabor_rate * dQuantity;
                    dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                    drNew2["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                    drNew2["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                    drNew2["total_unit_cost"] = dLineItemTotal;
                    dtP.Rows.Add(drNew2);
                }
                catch
                {
                }

        }

        DataView dv2 = dtP.DefaultView;
        dv2.Sort = "last_update_date asc";
        DataTable dtSoldMaster = dv2.ToTable();
        Session.Add("sSoldList", dtSoldMaster);

        DataTable tmpTable = LoadAsSoldDataTable();

        if (rdoSort.SelectedValue == "1")
        {

            #region Cost by Location
            decimal GUnitCostTotal = 0;
            decimal GLaborTotal = 0;
            decimal GSubTotalCost = 0;
            decimal GIExPriceTotal = 0;


            DataView dvLoc = new DataView(dtSoldMaster);
            DataTable dtResults = dvLoc.ToTable(true, "location_id", "location_name");

            DataTable tblCustList = (DataTable)Session["sSoldList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Composite SOW by Location";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Customer Name: " + lblCustomerName.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Estimate: " + lblEstimateName.Text;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtResults.Rows)
            {
                decimal dSubTotalUnitCost = 0;
                decimal dSubTotalCost = 0;
                decimal dSubExtPriceTotal = 0;
                decimal dSubTotalLaborCost = 0;

                int nLocationId = Convert.ToInt32(dr["location_id"]);
                string strLocation = dr["location_name"].ToString();

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "Location: " + strLocation;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();

                drNew["ITEM ID"] = "ITEM ID";
                drNew["SL"] = "SL";
                drNew["SHORT NOTES"] = "SHORT NOTES";
                drNew["SECTION NAME"] = "SECTION";
                drNew["ITEM NAME"] = "ITEM NAME";
                drNew["UOM"] = "UOM";
                drNew["QTY"] = "QTY";
                if (chkPMDisplay.Checked)
                {
                    if (obj.role_id == 4)
                    {
                        drNew["EXT. COST"] = "EXT. COST";
                    }
                    else
                    {
                        drNew["UNITCOST"] = "UNIT COST";
                        drNew["LABORCOST"] = "LABOR COST";
                        drNew["EXT. COST"] = "EXT. COST";
                        drNew["EXT. PRICE"] = "EXT. PRICE";
                    }

                }
                else
                {
                    if (obj.role_id == 1)
                    {
                        drNew["EXT. PRICE"] = "EXT. PRICE";
                    }
                 }
                
                drNew["CHECKLISTNOTES"] = "CHECKLIST NOTES";
                drNew["ITEMSTATUS"] = "ITEM STATUS";

                tmpTable.Rows.Add(drNew);

                bool Iexists = tblCustList.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(nLocationId)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblCustList.Select("location_id =" + nLocationId + "");
                    foreach (var row in rows)
                    {
                        decimal unitCost = 0;
                        decimal laborCost = 0;
                        decimal extCost = 0;
                        decimal extPrice = 0;

                        unitCost = Convert.ToDecimal(row["unit_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        laborCost = Convert.ToDecimal(row["total_labor_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        extCost = Convert.ToDecimal(row["total_unit_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        extPrice = Convert.ToDecimal(row["total_retail_price"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));

                        if (Convert.ToInt32(row["item_status_id"]) == 2)
                        {
                            dSubTotalUnitCost += 0;
                            dSubTotalLaborCost += 0;
                            dSubTotalCost += 0;
                            dSubExtPriceTotal += 0;
                            GUnitCostTotal += 0;
                            GLaborTotal += 0;
                            GSubTotalCost += 0;
                            GIExPriceTotal += 0;

                        }
                        else
                        {
                            dSubTotalUnitCost += (unitCost * Convert.ToDecimal(row["quantity"]));
                            dSubTotalLaborCost += laborCost;
                            if (obj.role_id == 4)
                            {
                                if (row["item_name"].ToString().ToLower().Contains("allowance"))
                                {
                                    dSubTotalCost -= extCost;
                                }
                                else
                                {
                                    dSubTotalCost += extCost;
                                }
                            }
                            else
                            {
                                dSubTotalCost += extCost;
                            }

                            dSubExtPriceTotal += extPrice;
                        }
                        drNew = tmpTable.NewRow();

                        drNew["ITEM ID"] = row["item_id"].ToString();
                        drNew["SL"] = row["section_serial"].ToString();
                        drNew["SHORT NOTES"] = row["short_notes"];
                        drNew["SECTION NAME"] = row["section_name"];
                        drNew["ITEM NAME"] = row["item_name"];
                        drNew["UOM"] = row["measure_unit"];
                        drNew["QTY"] = row["quantity"].ToString();
                        if (chkPMDisplay.Checked)
                        {
                            if (obj.role_id == 4)
                            {
                                drNew["EXT. COST"] = "";
                            }
                            else
                            {
                                drNew["UNITCOST"] = unitCost.ToString("c");
                                drNew["LABORCOST"] = laborCost.ToString("c");
                                drNew["EXT. COST"] = extCost.ToString("c");
                                drNew["EXT. PRICE"] = extPrice.ToString("c");
                            }


                        }
                        else
                        {
                            if (obj.role_id == 1)
                            {
                                drNew["EXT. PRICE"] = extPrice.ToString("c");
                            }
                        }
                       
                        drNew["CHECKLISTNOTES"] = row["short_notes_new"].ToString();
                        drNew["ITEMSTATUS"] = row["tmpCo"].ToString();
                        tmpTable.Rows.Add(drNew);

                    }
                }

                GUnitCostTotal += dSubTotalUnitCost;
                GLaborTotal += dSubTotalLaborCost;
                GIExPriceTotal += dSubExtPriceTotal;

                drNew = tmpTable.NewRow();

                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["SECTION NAME"] = "";
                drNew["ITEM NAME"] = "";
                
                drNew["QTY"] = "";
                if (chkPMDisplay.Checked)
                {
                    drNew["UOM"] = "Sub Total:";
                    if (obj.role_id == 4)
                    {
                        if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                        {

                            dSubTotalCost = (dSubTotalCost - (dSubTotalCost * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100);
                            GSubTotalCost += dSubTotalCost;
                            drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        }
                        else
                        {
                            GSubTotalCost += dSubTotalCost;
                            drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        }

                    }
                    else
                    {
                        GSubTotalCost += dSubTotalCost;
                        drNew["UNITCOST"] = dSubTotalUnitCost.ToString("c");
                        drNew["LABORCOST"] = dSubTotalLaborCost.ToString("c");
                        drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        drNew["EXT. PRICE"] = dSubExtPriceTotal.ToString("c");
                    }

                }
                else
                {
                  
                    if (obj.role_id == 1)
                    {
                        drNew["UOM"] = "Sub Total:";
                        drNew["EXT. PRICE"] = dSubExtPriceTotal.ToString("c");
                    }
                }
                
                drNew["CHECKLISTNOTES"] = "CHECKLIST NOTES";
                drNew["ITEMSTATUS"] = "ITEM STATUS";
                tmpTable.Rows.Add(drNew);

            }



            drNew = tmpTable.NewRow();

            drNew["ITEM ID"] = "";
            drNew["SL"] = "";
            drNew["SHORT NOTES"] = "";
            drNew["SECTION NAME"] = "";
            drNew["ITEM NAME"] = "";
          
            drNew["QTY"] = "";
            if (chkPMDisplay.Checked)
            {
                drNew["UOM"] = "Grand Total:";
                if (obj.role_id == 4)
                {
                    drNew["EXT. COST"] = GSubTotalCost.ToString("c");
                }
                else
                {
                    drNew["UNITCOST"] = GUnitCostTotal.ToString("c");
                    drNew["LABORCOST"] = GLaborTotal.ToString("c");
                    drNew["EXT. COST"] = GSubTotalCost.ToString("c");
                    drNew["EXT. PRICE"] = GIExPriceTotal.ToString("c");
                }

            }
            else
            {
               
                if (obj.role_id == 1)
                {
                    drNew["UOM"] = "Grand Total:";
                    drNew["EXT. PRICE"] = GIExPriceTotal.ToString("c");
                }
            }
           
            drNew["CHECKLISTNOTES"] = "";
            drNew["ITEMSTATUS"] = "";
            tmpTable.Rows.Add(drNew);

            #endregion


            Session.Add("CostReport", tmpTable);
            string strUrl = "CostReportCsv.aspx?TypeId=10";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
        }
        else
        {
            
               #region Cost by Section
                decimal GUnitCostTotal = 0;
                decimal GLaborTotal = 0;
                decimal GSubTotalCost = 0;
                decimal GIExPriceTotal = 0;

            DataView dvsecsold = new DataView(dtSoldMaster);
            dvsecsold.Sort = "section_level, item_id";
            DataTable dtSoldResults = dvsecsold.ToTable(true, "section_level", "section_name");

            DataTable tblSoldList = (DataTable)Session["sSoldList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Composite SOW by Section";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Customer Name: " + lblCustomerName.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Estimate: " + lblEstimateName.Text;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtSoldResults.Rows)
            {
                decimal dSubTotalUnitCost = 0;
                decimal dSubTotalCost = 0;
                decimal dSubExtPriceTotal = 0;
                decimal dSubTotalLaborCost = 0;

                int nSection_level = Convert.ToInt32(dr["section_level"]);
                string strSection = dr["section_name"].ToString();

                drNew = tmpTable.NewRow();
                drNew["ITEM NAME"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "Section: " + strSection;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();

                drNew["ITEM ID"] = "ITEM ID";
                drNew["SL"] = "SL";
                drNew["SHORT NOTES"] = "SHORT NOTES";
                drNew["LOCATION NAME"] = "LOCATION NAME";
                drNew["ITEM NAME"] = "ITEM NAME";
                drNew["UOM"] = "UOM";
                drNew["QTY"] = "QTY";
                if (chkPMDisplay.Checked)
                {
                    if (obj.role_id == 4)
                    {
                        drNew["EXT. COST"] = "EXT. COST";
                    }
                    else
                    {
                        drNew["UNITCOST"] = "UNIT COST";
                        drNew["LABORCOST"] = "LABOR COST";
                        drNew["EXT. COST"] = "EXT. COST";
                        drNew["EXT. PRICE"] = "EXT. PRICE";
                    }

                }
                else
                {
                    if (obj.role_id == 1)
                    {
                        drNew["EXT. PRICE"] = "EXT. PRICE";
                    }
                }
        
                drNew["CHECKLISTNOTES"] = "CHECKLIST NOTES";
                drNew["ITEMSTATUS"] = "ITEM STATUS";

                tmpTable.Rows.Add(drNew);

                bool Iexists = tblSoldList.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(nSection_level)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblSoldList.Select("section_level =" + nSection_level + "");
                    foreach (var row in rows)
                    {
                        decimal unitCost = 0;
                        decimal laborCost = 0;
                        decimal extCost = 0;
                        decimal extPrice = 0;

                        unitCost = Convert.ToDecimal(row["unit_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        laborCost = Convert.ToDecimal(row["total_labor_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        extCost = Convert.ToDecimal(row["total_unit_cost"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        extPrice = Convert.ToDecimal(row["total_retail_price"].ToString().Replace("$", "").Replace("(", "-").Replace(")", ""));

                        if (Convert.ToInt32(row["item_status_id"]) == 2)
                        {
                            dSubTotalUnitCost += 0;
                            dSubTotalLaborCost += 0;
                            dSubTotalCost += 0;
                            dSubExtPriceTotal += 0;
                            GUnitCostTotal += 0;
                            GLaborTotal += 0;
                            GSubTotalCost += 0;
                            GIExPriceTotal += 0;

                        }
                        else
                        {
                            dSubTotalUnitCost += (unitCost * Convert.ToDecimal(row["quantity"]));
                            dSubTotalLaborCost += laborCost;
                            if (obj.role_id == 4)
                            {
                                if (row["item_name"].ToString().ToLower().Contains("allowance"))
                                {
                                    dSubTotalCost -= extCost;
                                }
                                else
                                {
                                    dSubTotalCost += extCost;
                                }
                            }
                            else
                            {
                                dSubTotalCost += extCost;
                            }

                            dSubExtPriceTotal += extPrice;




                        }



                        drNew = tmpTable.NewRow();

                        drNew["ITEM ID"] = row["item_id"].ToString();
                        drNew["SL"] = row["section_serial"].ToString();
                        drNew["SHORT NOTES"] = row["short_notes"];
                        drNew["LOCATION NAME"] = row["location_name"];
                        drNew["ITEM NAME"] = row["item_name"];
                        drNew["UOM"] = row["measure_unit"];
                        drNew["QTY"] = row["quantity"].ToString();
                        if (chkPMDisplay.Checked)
                        {
                            if (obj.role_id == 4)
                            {
                                drNew["EXT. COST"] = "";
                            }
                            else
                            {
                                drNew["UNITCOST"] = unitCost.ToString("c");
                                drNew["LABORCOST"] = laborCost.ToString("c");
                                drNew["EXT. COST"] = extCost.ToString("c");
                                drNew["EXT. PRICE"] = extPrice.ToString("c");
                            }


                        }
                        else
                        {
                            if (obj.role_id == 1)
                            {
                                drNew["EXT. PRICE"] = extPrice.ToString("c");
                            }
                        }
                       
                        drNew["CHECKLISTNOTES"] = row["short_notes_new"].ToString();
                        drNew["ITEMSTATUS"] = row["tmpCo"].ToString();
                        tmpTable.Rows.Add(drNew);

                    }
                }

                GUnitCostTotal += dSubTotalUnitCost;
                GLaborTotal += dSubTotalLaborCost;
                GIExPriceTotal += dSubExtPriceTotal;

                drNew = tmpTable.NewRow();

                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["LOCATION NAME"] = "";
                drNew["ITEM NAME"] = "";
               
                drNew["QTY"] = "";
                if (chkPMDisplay.Checked)
                {
                    drNew["UOM"] = "Sub Total:";
                    if (obj.role_id == 4)
                    {
                        if (Convert.ToDecimal(hdnSuperExCostDecrease.Value) > 0)
                        {

                            dSubTotalCost = (dSubTotalCost - (dSubTotalCost * Convert.ToDecimal(hdnSuperExCostDecrease.Value)) / 100);
                            GSubTotalCost += dSubTotalCost;
                            drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        }
                        else
                        {
                            GSubTotalCost += dSubTotalCost;
                            drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        }

                    }
                    else
                    {
                        GSubTotalCost += dSubTotalCost;
                        drNew["UNITCOST"] = dSubTotalUnitCost.ToString("c");
                        drNew["LABORCOST"] = dSubTotalLaborCost.ToString("c");
                        drNew["EXT. COST"] = dSubTotalCost.ToString("c");
                        drNew["EXT. PRICE"] = dSubExtPriceTotal.ToString("c");
                    }

                }
                else
                {
                    
                    if (obj.role_id == 1)
                    {
                        drNew["UOM"] = "Sub Total:";
                        drNew["EXT. PRICE"] = dSubExtPriceTotal.ToString("c");
                    }
                }
               
                drNew["CHECKLISTNOTES"] = "CHECKLIST NOTES";
                drNew["ITEMSTATUS"] = "ITEM STATUS";
                tmpTable.Rows.Add(drNew);




             }

            drNew = tmpTable.NewRow();

            drNew["ITEM ID"] = "";
            drNew["SL"] = "";
            drNew["SHORT NOTES"] = "";
            drNew["LOCATION NAME"] = "";
            drNew["ITEM NAME"] = "";
       
            drNew["QTY"] = "";
            if (chkPMDisplay.Checked)
            {
                drNew["UOM"] = "Grand Total:";
                if (obj.role_id == 4)
                {
                    drNew["EXT. COST"] = GSubTotalCost.ToString("c");
                }
                else
                {
                    drNew["UNITCOST"] = GUnitCostTotal.ToString("c");
                    drNew["LABORCOST"] = GLaborTotal.ToString("c");
                    drNew["EXT. COST"] = GSubTotalCost.ToString("c");
                    drNew["EXT. PRICE"] = GIExPriceTotal.ToString("c");
                }

            }
            else
            {
              
                if (obj.role_id == 1)
                {
                    drNew["UOM"] = "Grand Total:";
                    drNew["EXT. PRICE"] = GIExPriceTotal.ToString("c");
                }
            }
       
            drNew["CHECKLISTNOTES"] = "";
            drNew["ITEMSTATUS"] = "";
            tmpTable.Rows.Add(drNew);

            #endregion

            Session.Add("SoldReport", tmpTable);
            string strUrl = "ProjectSoldReport.aspx?TypeId=10";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
        }
        
    }
    private DataTable LoadAsSoldDataTable()
    {
        DataTable table = new DataTable();
        userinfo obj = (userinfo)Session["oUser"];
        table.Columns.Add("ITEM ID", typeof(string));
        table.Columns.Add("SL", typeof(string));
        table.Columns.Add("SHORT NOTES", typeof(string));
        if (rdoSort.SelectedValue == "1")
        {
            table.Columns.Add("SECTION NAME", typeof(string));
           
        }
        else
        {
            table.Columns.Add("LOCATION NAME", typeof(string));
        }
        
        table.Columns.Add("ITEM NAME", typeof(string));
        table.Columns.Add("UOM", typeof(string));
        table.Columns.Add("QTY", typeof(string));
        if (chkPMDisplay.Checked)
        {
            if (obj.role_id == 4)
            {
                table.Columns.Add("EXT. COST", typeof(string));
            }
            else
            {
                table.Columns.Add("UNITCOST", typeof(string));
                table.Columns.Add("LABORCOST", typeof(string));
                table.Columns.Add("EXT. COST", typeof(string));
                table.Columns.Add("EXT. PRICE", typeof(string));
            }
           
        }
        else
        {
            if (obj.role_id == 1)
            {
                table.Columns.Add("EXT. PRICE", typeof(string));
            }
        }
        
        table.Columns.Add("CHECKLISTNOTES", typeof(string));
        table.Columns.Add("ITEMSTATUS", typeof(string));
       
        return table;
    }

    protected void lnkGeneralOpen_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDescription = gRow.Cells[2].Controls[0].FindControl("lblDescription") as Label;
        Label lblDescription_r = gRow.Cells[2].Controls[1].FindControl("lblDescription_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkGeneralOpen") as LinkButton;
        modProjectNotes.Show();
        if (lnkOpen.Text == "More")
        {
            lblDescription.Visible = false;
            lblDescription_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDescription.Visible = true;
            lblDescription_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

    protected void lnkOpenMaterialTrack_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
        modProjectNotes.Show();
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblMaterialTrack = gRow.Cells[2].Controls[0].FindControl("lblMaterialTrack") as Label;
        Label lblMaterialTrack_r = gRow.Cells[2].Controls[1].FindControl("lblMaterialTrack_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenMaterialTrack") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblMaterialTrack.Visible = false;
            lblMaterialTrack_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblMaterialTrack.Visible = true;
            lblMaterialTrack_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenDesignUpdates_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
        modProjectNotes.Show();
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDesignUpdates = gRow.Cells[2].Controls[0].FindControl("lblDesignUpdates") as Label;
        Label lblDesignUpdates_r = gRow.Cells[2].Controls[1].FindControl("lblDesignUpdates_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenDesignUpdates") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblDesignUpdates.Visible = false;
            lblDesignUpdates_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDesignUpdates.Visible = true;
            lblDesignUpdates_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenSuperintendentNotes_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
        modProjectNotes.Show();
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblSuperintendentNotes = gRow.Cells[2].Controls[0].FindControl("lblSuperintendentNotes") as Label;
        Label lblSuperintendentNotes_r = gRow.Cells[2].Controls[1].FindControl("lblSuperintendentNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenSuperintendentNotes") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblSuperintendentNotes.Visible = false;
            lblSuperintendentNotes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblSuperintendentNotes.Visible = true;
            lblSuperintendentNotes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

    protected void grdProjectNote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int section_id = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[1].ToString());
            bool isCompleted= Convert.ToBoolean(grdProjectNote.DataKeys[e.Row.RowIndex].Values[2].ToString());
            string sectionName = grdProjectNote.DataKeys[e.Row.RowIndex].Values[3].ToString();

            LinkButton lnkGeneralOpen = (LinkButton)e.Row.FindControl("lnkGeneralOpen");
            LinkButton lnkOpenMaterialTrack = (LinkButton)e.Row.FindControl("lnkOpenMaterialTrack"); 
            LinkButton lnkOpenDesignUpdates = (LinkButton)e.Row.FindControl("lnkOpenDesignUpdates");
            LinkButton lnkOpenSuperintendentNotes = (LinkButton)e.Row.FindControl("lnkOpenSuperintendentNotes");

            Label lblDescription = (Label)e.Row.FindControl("lblDescription");
            Label lblCompleted = (Label)e.Row.FindControl("lblCompleted");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            Label lblMaterialTrack = (Label)e.Row.FindControl("lblMaterialTrack");
            Label lblDesignUpdates = (Label)e.Row.FindControl("lblDesignUpdates");
            Label lblSuperintendentNotes = (Label)e.Row.FindControl("lblSuperintendentNotes");
            Label lblSection = (Label)e.Row.FindControl("lblSection");
            lblSection.Text = sectionName;

            if (isCompleted)
            {
                lblCompleted.Text = "Yes";
                e.Row.Attributes.CssStyle.Add("color", "green");
            
            }
            else
            {
                lblCompleted.Text = "No";
              
            }


           if (lblDescription.Text != "" && lblDescription.Text.Length > 90)
            {
                lblDescription.Text = lblDescription.Text.Substring(0, 90) + " ...";
                lblDescription.ToolTip = lblDescription.Text;
                lnkGeneralOpen.Visible = true;
            }
            else
            {
                lblDescription.Text = lblDescription.Text;
                lnkGeneralOpen.Visible = false;
            }

           if (lblMaterialTrack.Text != "" && lblMaterialTrack.Text.Length > 90)
            {
                lblMaterialTrack.Text = lblMaterialTrack.Text.Substring(0, 90) + " ...";
                lblMaterialTrack.ToolTip = lblMaterialTrack.Text;
                lnkOpenMaterialTrack.Visible = true;
            }
            else
            {
                lblMaterialTrack.Text = lblMaterialTrack.Text;
                lnkOpenMaterialTrack.Visible = false;
            }

           if (lblDesignUpdates.Text != "" && lblDesignUpdates.Text.Length > 90)
            {
                lblDesignUpdates.Text = lblDesignUpdates.Text.Substring(0, 90) + " ...";
                lblDesignUpdates.ToolTip = lblDesignUpdates.Text;
                lnkOpenDesignUpdates.Visible = true;
            }
            else
            {
                lblDesignUpdates.Text = lblDesignUpdates.Text;
                lnkOpenDesignUpdates.Visible = false;
            }

           if (lblSuperintendentNotes.Text != "" && lblSuperintendentNotes.Text.Length > 90)
            {
                lblSuperintendentNotes.Text = lblSuperintendentNotes.Text.Substring(0, 90) + " ...";
                lblSuperintendentNotes.ToolTip = lblSuperintendentNotes.Text;
                lnkOpenSuperintendentNotes.Visible = true;
            }
            else
            {
                lblSuperintendentNotes.Text = lblSuperintendentNotes.Text;
                lnkOpenSuperintendentNotes.Visible = false;
            }

        }

    }


    protected void grdGeneralProjectNotes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int id = Convert.ToInt32(grdGeneralProjectNotes.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int section_id = Convert.ToInt32(grdGeneralProjectNotes.DataKeys[e.Row.RowIndex].Values[1].ToString());
            bool isCompleted = Convert.ToBoolean(grdGeneralProjectNotes.DataKeys[e.Row.RowIndex].Values[2].ToString());
            string sectionName = grdGeneralProjectNotes.DataKeys[e.Row.RowIndex].Values[3].ToString();

            LinkButton lnkGeneralOpen2 = (LinkButton)e.Row.FindControl("lnkGeneral2Open");
            Label lblDescription2 = (Label)e.Row.FindControl("lblDescription2");
            Label lblCompleted = (Label)e.Row.FindControl("lblCompleted");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
           
           

            if (isCompleted)
            {
                lblCompleted.Text = "Yes";
                e.Row.Attributes.CssStyle.Add("color", "green");

            }
            else
            {
                lblCompleted.Text = "No";

            }


            if (lblDescription2.Text != "" && lblDescription2.Text.Length > 250)
            {
                lblDescription2.Text = lblDescription2.Text.Substring(0, 250) + " ...";
                lblDescription2.ToolTip = lblDescription2.Text;
                lnkGeneralOpen2.Visible = true;
            }
            else
            {
                lblDescription2.Text = lblDescription2.Text;
                lnkGeneralOpen2.Visible = false;
            }

          

        }

    }



    protected void lnkGeneralOpe2_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDescription = gRow.Cells[2].Controls[0].FindControl("lblDescription2") as Label;
        Label lblDescription_r = gRow.Cells[2].Controls[1].FindControl("lblDescription2_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkGeneral2Open") as LinkButton;
    
        if (lnkOpen.Text == "More")
        {
            lblDescription.Visible = false;
            lblDescription_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDescription.Visible = true;
            lblDescription_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

}
