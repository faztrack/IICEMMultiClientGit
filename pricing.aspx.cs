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
using System.Web.Services;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class pricing : System.Web.UI.Page
{
    string strDetails = "";
    public static string strDetailsFull = "";
    private double subtotal = 0.0;
    private double grandtotal = 0.0;
    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);
    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    private double subtotalCost = 0.0;
    private double grandtotalCost = 0.0;
    private double subtotal_diectCost = 0.0;
    private double grandtotal_directCost = 0.0;


    [WebMethod]
    public static string[] GetItemName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (HttpContext.Current.Session["cParent"] != null)
        {

            int nPId = Convert.ToInt32(HttpContext.Current.Session["cParent"]);
            return (from c in _db.sectioninfos
                    where c.section_name.Contains(prefixText) && c.parent_id == nPId
                    select c.section_name).Take<String>(count).ToArray();
        }
        else
        {

            return (from c in _db.sectioninfos
                    where c.section_name.Contains(prefixText)
                    select c.section_name).Take<String>(count).ToArray();
        }

    }
    [WebMethod]
    public static string[] GetItemNameAll(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (HttpContext.Current.Session["gspSearch"] != null)
        {
            DataTable dt = (DataTable)(HttpContext.Current.Session["gspSearch"]);
            return (from c in dt.AsEnumerable()
                    where c.Field<string>("section_name").ToLower().Contains(prefixText.ToLower())
                    select c.Field<string>("section_name")).ToArray();

        }
        else
        {

            DataTable dt = LoadFullSection();
            return (from c in dt.AsEnumerable()
                    where c.Field<string>("section_name").ToLower().Contains(prefixText.ToLower())
                    select c.Field<string>("section_name")).ToArray();
        }

    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo obj = (userinfo)Session["oUser"];
                hdnEmailType.Value = obj.EmailIntegrationType.ToString();
   
                int nRoleId = obj.role_id;
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
            if (Page.User.IsInRole("admin016") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSuperintendent();
            Session.Remove("gspSearch");
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            Session.Remove("TmpList");
            Session.Remove("RecentList");
            Session.Remove("myIds");
            Session.Remove("sNewAddedItem");

            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();
            hdnSortDesc.Value = "0";
            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();
            tdlOther.Visible = false;
            tdlCustomizePrice.Visible = false;
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                hdnLastName.Value = cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                if (_db.sales_persons.Any(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

                if (cust.SuperintendentId != null && cust.SuperintendentId != 0)
                {
                    string strSuperintendent = "";
                    ListItem dditem = ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString());
                    if (dditem != null)
                        this.ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString()).Selected = true;
                    else
                    {
                        user_info uinfo = _db.user_infos.Single(u => u.user_id == cust.SuperintendentId);
                        if (uinfo != null)
                        {
                            strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                            ddlSuperintendent.Items.Insert(0, new ListItem(strSuperintendent, cust.SuperintendentId.ToString()));
                        }
                    }
                }


                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                    lblExistingEstimateName.Text = cus_est.estimate_name;
                    // ddlStatus.SelectedValue = cus_est.status_id.ToString();

                    if (cus_est.status_id == 1)
                        lblStatus.Text = "Pending";
                    else if (cus_est.status_id == 2)
                        lblStatus.Text = "Sit";
                    else
                        lblStatus.Text = "Sold";



                    if (Convert.ToBoolean(cus_est.IsEstimateActive) == false)
                    {
                        rdbEstimateIsActive.SelectedValue = "0";
                    }
                    else
                    {
                        rdbEstimateIsActive.SelectedValue = "1";

                    }
                    chkCustDisp.Checked = Convert.ToBoolean(cus_est.IsCustDisplay);

                    estimate_payment est_pay = new estimate_payment();

                    est_pay = _db.estimate_payments.Where(ep => ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).FirstOrDefault();
                    if (est_pay != null)
                    {
                        if (est_pay.tax_rate != null)
                        {
                            txtTaxPer.Text = est_pay.tax_rate.ToString();
                        }
                    }
                    else
                    {
                        if (cus_est.tax_rate != null)
                        {
                            txtTaxPer.Text = cus_est.tax_rate.ToString();
                        }

                    }
                    if (txtTaxPer.Text != "")
                    {
                        if (Convert.ToDecimal(txtTaxPer.Text) == 0)
                        {
                            txtTaxPer.Visible = false;
                            lblTax_label.Visible = false;

                        }
                        else
                        {
                            txtTaxPer.Visible = true;
                            lblTax_label.Visible = true;
                        }
                    }
                    else
                    {
                        txtTaxPer.Visible = false;
                        lblTax_label.Visible = false;
                    }

                    //if (ddlStatus.SelectedValue == "3")
                    //{
                    //    Response.Redirect("sold_estimate.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                    //}

                    if (cus_est.status_id == 3)
                    {
                        Response.Redirect("sold_estimate.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                    }


                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";

                }


                var item = from loc in _db.locations
                           join cl in _db.customer_locations on loc.location_id equals cl.location_id
                           where cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                           orderby loc.location_name
                           select new LocationModel()
                           {
                               location_id = (int)cl.location_id,
                               location_name = loc.location_name
                           };



                ddlCustomerLocations.DataSource = item;
                ddlCustomerLocations.DataTextField = "location_name";
                ddlCustomerLocations.DataValueField = "location_id";
                ddlCustomerLocations.DataBind();
                ddlCustomerLocations.Items.Insert(0, "Select Location");
                ddlCustomerLocations.SelectedValue = "0";
                ddlNewLocations.DataSource = item;
                ddlNewLocations.DataTextField = "location_name";
                ddlNewLocations.DataValueField = "location_id";
                ddlNewLocations.DataBind();
                ddlNewLocations.Items.Insert(0, "Select New Location");
                ddlNewLocations.SelectedValue = "0";


                ddCopyLocation.DataSource = item;
                ddCopyLocation.DataTextField = "location_name";
                ddCopyLocation.DataValueField = "location_id";
                ddCopyLocation.DataBind();
                ddCopyLocation.Items.Insert(0, "Select Location");
                ddCopyLocation.SelectedValue = "0";

                var section = from sec in _db.sectioninfos
                              join cs in _db.customer_sections on sec.section_id equals cs.section_id
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              orderby sec.section_name
                              select new SectionInfo()
                              {
                                  section_id = (int)cs.section_id,
                                  section_name = sec.section_name
                              };

                DataTable dtSectionId = csCommonUtility.LINQToDataTable(section);
                ddlSections.DataSource = section; //_db.customer_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                ddlSections.DataTextField = "section_name";
                ddlSections.DataValueField = "section_id";
                ddlSections.DataBind();
                ddlSections.Items.Insert(0, "Select Section");
                ddlSections.SelectedValue = "0";
                ddlSections.Items.Insert(1, "All Selected Section");
                ddlSections.SelectedValue = "-1";
                int[] terms = new int[dtSectionId.Rows.Count]; ;
                for (int i = 0; i < dtSectionId.Rows.Count; i++)
                {
                    terms[i] = Convert.ToInt32(dtSectionId.Rows[i]["section_id"]);
                }
                Session["myIds"] = terms;
                // DataTable dtNew = LoadFullSection();
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
                Calculate_Total();
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
                if (ddlSections.SelectedItem.Text == "Select Section")
                {
                    trvSection.Nodes.Clear();
                }

                if (Session["sSytemMassage"] != null)
                {
                    lblResult.Visible = true;
                    lblResult.Text = csCommonUtility.GetSystemMessage(Session["sSytemMassage"].ToString());

                    Session.Add("sSytemMassage", null);
                }
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "lnkUpdateEstimate", "btnTemplateEstimate", "ddlSuperintendent", "ddlCustomerLocations", "ddlSuperintendent", "lnkAddMoreLocation", "rdbEstimateIsActive", "rdbEstimateIsActive", "chkCashPay", "chkCustDisp", "chkPMDisplay", "ddlSections", "btnPricingPrint", "btnPricingPrintWOPrice", "lnkAddMoreSections", "btnRefreshLatestPricing",  "btnSave",  "btnDuplicate", "pnlRefreshPricing", "hypGoogleMap", "lnkUpdateLocation", "pnlChangeLocation", "pnlUpdateEstimate", "rdbDiscount" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Copy Selected Item(s)", "Edit Selected Item(s)", "Delete Selected Item(s)", "Add Notes", "Move to Top", "grdSelectedItem1_0_chkSingle", "grdSelectedItem1_0_chkAll" });
        }

    }
    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE role_id = 4";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSuperintendent.DataSource = mList;
        ddlSuperintendent.DataTextField = "Superintendent_name";
        ddlSuperintendent.DataValueField = "user_id";
        ddlSuperintendent.DataBind();
        ddlSuperintendent.Items.Insert(0, "Select");
    }


    protected void lnkAddMoreLocation_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_locations.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&lid=1");
    }
    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    private void LoadTree(int nSectionLevel)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE is_disable = 0 and is_active=1 AND section_level=" + nSectionLevel + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "ORDER BY section_name";
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        trvSection.Nodes.Clear();
        foreach (sectioninfo sec in list)
        {
            string name = sec.section_name;
            if (sec.parent_id == 0)
            {
                TreeNode node = new TreeNode(sec.section_name, sec.section_id.ToString());
                trvSection.Nodes.Add(node);
                AddChildMenu(node, sec);
            }
        }
    }
    private void AddChildMenu(TreeNode parentNode, sectioninfo sec)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE is_disable = 0 and is_active=1 AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + ")  ORDER BY section_name";
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        //List<sectioninfo> list = _db.sectioninfos.Where(c => c.client_id == 1).ToList();
        foreach (sectioninfo subsec in list)
        {
            if (subsec.parent_id.ToString() == parentNode.Value)
            {
                TreeNode node = new TreeNode(subsec.section_name, subsec.section_id.ToString());
                parentNode.ChildNodes.Add(node);
                AddChildMenu(node, subsec);
            }
        }
    }
    //public void BindGrid()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    sectioninfo sinfo = new sectioninfo();
    //    sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnSectionId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
    //    hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
    //    hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
    //    hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
    //    var item = from it in _db.item_prices
    //               join si in _db.sectioninfos on it.item_id equals si.section_id
    //               where si.parent_id == Convert.ToInt32(trvSection.SelectedValue)
    //               select new ItemPriceModel()
    //               {
    //                   item_id = (int)it.item_id,
    //                   section_name = si.section_name,
    //                   measure_unit = it.measure_unit,
    //                   item_cost = (decimal)it.item_cost * (decimal)it.retail_multiplier,
    //                   minimum_qty = (decimal)it.minimum_qty,
    //                   retail_multiplier = (decimal)it.retail_multiplier,
    //                   labor_rate = (decimal)it.labor_rate,
    //                   section_serial = (decimal)si.section_serial,
    //                   ext_item_cost = (((decimal)it.item_cost + (decimal)it.labor_rate) * (decimal)it.retail_multiplier) * (decimal)it.minimum_qty,
    //                   labor_id = (int)it.labor_id
    //               };
    //    grdItemPrice.DataSource = item;
    //    grdItemPrice.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier" };
    //    grdItemPrice.DataBind();
    //}

    public void BindGrid()
    {
        try
        {
            if (Convert.ToInt32(hdnSectionId.Value) != 0)
            {
                lblResult1.Text = "";
                lblAdd.Text = "";
                lblSelectLocation.Text = "";

                DataClassesDataContext _db = new DataClassesDataContext();
                string strCondition = string.Empty;
                if (txtSearchItemName.Text.Trim() != "")
                {
                    strCondition = "  AND si.section_name LIKE '%" + txtSearchItemName.Text.Trim() + "%'";
                }
                sectioninfo sinfo = new sectioninfo();
                sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnSectionId.Value) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
                hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
                hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();

                string StrQ = " SELECT it.client_id,it.item_id,si.section_name,si.is_mandatory,it.measure_unit,it.item_cost*it.retail_multiplier AS item_cost,it.minimum_qty,it.retail_multiplier,it.labor_rate,it.labor_rate* it.retail_multiplier as LaborUnitCost,it.update_time,si.section_serial,it.labor_id,((it.item_cost + it.labor_rate) * it.retail_multiplier) * it.minimum_qty AS ext_item_cost " +
                             " FROM item_price it " +
                             " INNER JOIN sectioninfo si on si.section_id =  it.item_id " +
                             " WHERE si.is_active = 1 AND si.is_disable = 0 AND si.parent_id = " + Convert.ToInt32(trvSection.SelectedValue) + "  " + strCondition + " ";
                List<ItemPriceModel> ItemList = _db.ExecuteQuery<ItemPriceModel>(StrQ, string.Empty).ToList();
                DataTable dt = csCommonUtility.LINQToDataTable(ItemList);
                grdItemPrice.DataSource = ItemList;
                grdItemPrice.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory" };
                grdItemPrice.DataBind();

                string strId = string.Empty;

                DataTable dtMainList = csCommonUtility.LINQToDataTable(ItemList);
                if (dtMainList.Rows.Count > 0)
                {
                    DataView dv = dtMainList.DefaultView;
                    dv.RowFilter = "is_mandatory = 1";
                    if (dv.Count > 0)
                    {

                        for (int i = 0; i < dv.Count; i++)
                        {
                            if (strId.Length == 0)
                            {
                                strId = Convert.ToInt32(dv[i]["item_id"]).ToString();
                            }
                            else
                            {
                                strId = strId + ", " + Convert.ToInt32(dv[i]["item_id"]).ToString();
                            }
                        }
                    }

                    hdnMandatoryId.Value = strId;
                }
            }
        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void GetItemId_other()
    {
        int nItemId = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from it in _db.item_prices
                      join si in _db.sectioninfos on it.item_id equals si.section_id
                      where si.section_level == Convert.ToInt32(ddlSections.SelectedValue)
                      select it.item_id);
        int n = result.Count();
        if (result != null && n > 0)
            nItemId = result.Max();

        nItemId = nItemId + 1;
        hdnOtherId.Value = nItemId.ToString();
    }
    //public static string[] GetItemFullName(string strName)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //int[] ids = (int[])HttpContext.Current.Session["myIds"];
    //List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_name.Contains(strName) && ids.Contains((int)c.section_level) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();

    //}

    public static DataTable LoadFullSection()
    {
        DataTable dt = new DataTable();
        try
        {
            int nClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            DataClassesDataContext _db = new DataClassesDataContext();
            int[] ids = (int[])HttpContext.Current.Session["myIds"];

            string strId = "";


            for (int i = 0; i < ids.Length; i++)
                strId += ids[i] + ",";

            strId = strId.TrimEnd(',');

            string sSQL = "select isnull(s5.section_id,isnull(s4.section_id,isnull(s3.section_id,isnull(s2.section_id,0)))) as [section_auto_id], isnull(s5.section_id,isnull(s4.section_id,isnull(s3.section_id,isnull(s2.section_id,0)))) as section_id, " +
                       " isnull(s1.section_name,'')+isnull('>>'+s2.section_name,'')+isnull('>>'+s3.section_name,'')+isnull('>>'+s4.section_name,'')+isnull('>>'+s5.section_name,'') as section_name " +
                       " from sectioninfo s1 " +
                       " left outer join sectioninfo s2 on s2.parent_id = s1.section_id and s2.client_id =" + nClientId + " and s2.is_active = 1  and s2.is_disable = 0 " +
                       " left outer join sectioninfo s3 on s3.parent_id = s2.section_id and s3.client_id =" + nClientId + " and s3.is_active = 1  and s3.is_disable = 0 " +
                       " left outer join sectioninfo s4 on s4.parent_id = s3.section_id and s4.client_id =" + nClientId + " and s4.is_active = 1  and s4.is_disable = 0 " +
                       " left outer join sectioninfo s5 on s5.parent_id = s4.section_id and s5.client_id =" + nClientId + " and s5.is_active = 1  and s5.is_disable = 0 " +
                       " where s1.client_id =" + nClientId + " and s1.section_id in( " + strId + ")";

            var result = _db.ExecuteQuery<sectioninfo>(sSQL);

            dt = csCommonUtility.LINQToDataTable(result);

            HttpContext.Current.Session["gspSearch"] = dt;
            //var itemlist = (from c in _db.sectioninfos
            //                 join i in _db.item_prices on c.section_id equals i.item_id
            //                where ids.Contains((int)c.section_level) && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //                select new SectionInfo()
            //                {
            //                    section_id = c.section_id,
            //                    section_name = c.section_name

            //                }).Distinct().OrderBy(od => od.section_name).ToList();


            //DataTable dt = csCommonUtility.LINQToDataTable(itemlist);

            //foreach (DataRow dr in dt.Rows)
            //{
            //    strDetailsFull = string.Empty;
            //    string strItem = GetItemDetialsFull(Convert.ToInt32(dr["section_id"]));
            //    dr["section_name"] = strItem.ToString();
            //}
            //HttpContext.Current.Session["gspSearch"] = dt;
        }
        catch (Exception ex)
        {
            string ss = ex.Message;
        }

        return dt;
    }

    public static string GetItemDetialsFull(int SectionId)
    {
        string strSection = string.Empty;

        //&& c.parent_id > 0 
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetailsFull = sec1.section_name + " >> " + strDetailsFull;
            GetItemDetialsFull(Convert.ToInt32(sec1.parent_id));
        }
        return strDetailsFull;

    }


    public string GetItemDetialsForUpdateItem(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.parent_id > 0 && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetails = sec1.section_name + " >> " + strDetails;
            GetItemDetialsForUpdateItem(Convert.ToInt32(sec1.parent_id));
        }
        return strDetails;
    }
    public string GetItemDetials_forBreadCome(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetails = sec1.section_name + " >> " + strDetails;
            GetItemDetials_forBreadCome(Convert.ToInt32(sec1.parent_id));
        }
        return strDetails;
    }
    public string GetSectionName(int section_level)
    {
        string str = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo si = new sectioninfo();
        si = _db.sectioninfos.SingleOrDefault(c => c.section_level == section_level && c.parent_id == 0 && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        if (si != null)
            str = si.section_name;
        return str;
    }
    private int GetSerial()
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A" && pd.section_level == Convert.ToInt32(hdnSectionLevel.Value)
                      select pd.item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }
    private int GetSerialMultiple(int sectionLvel)
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A" && pd.section_level == sectionLvel
                      select pd.item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }
    private int GetSerial_other(int nOtherId)
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.item_id == nOtherId && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.section_level == Convert.ToInt32(hdnSectionLevel.Value)
                      select pd.other_item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }
    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where (from clc in _db.customer_locations
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                      select pd.total_retail_price);
        int n = result.Count();
        if (result != null && n > 0)
            dRetail = result.Sum();

        return dRetail;
    }
    private decimal GetDirctTotal()
    {
        decimal dDirect = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where (from clc in _db.customer_locations
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                      select pd.total_direct_price);
        int n = result.Count();
        if (result != null && n > 0)
            dDirect = result.Sum();

        return dDirect;
    }
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id  from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " GROUP BY pricing_details.location_id,location.location_name order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGrouping.DataSource = mList;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " GROUP BY pricing_details.location_id,location.location_name order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }
    public void Calculate_Total()
    {
        decimal direct = 0;
        decimal retail = 0;
        decimal grandtotal = 0;
        direct = GetDirctTotal();
        retail = GetRetailTotal();
        grandtotal = direct + retail;

        lblDirctTotalCost.Text = direct.ToString("c");
        lblRetailTotalCost.Text = retail.ToString("c");
        lblGrandTotalCost.Text = grandtotal.ToString("c");

    }


    protected void ddlSections_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlSections.SelectedItem.Text == "All Selected Section")
        {
            lblResult1.Text = "";
            trvSection.Nodes.Clear();
            lblSelectLocation.Text = "";
            lblResult1.Text = "";
            lblAdd.Text = "";
            lblParent.Text = "";


            tblMultiPricing.Visible = true;


            tblPricingWrapper.Visible = false;

        }
        else
        {
            tblMultiPricing.Visible = false;
            txtSearchAll.Text = string.Empty;
            BindGridAllPrice();
            if (ddlSections.SelectedItem.Text != "Select Section")
            {
                lblSelectLocation.Text = "";
                lblResult1.Text = "";
                lblAdd.Text = "";
                lblParent.Text = "";
                grdItemPrice.Visible = false;
                txtSearchItemName.Visible = false;
                btnSearch.Visible = false;
                LinkButton1.Visible = false;
                LoadTree(Convert.ToInt32(ddlSections.SelectedValue));
                tdlOther.Visible = false;
                tdlCustomizePrice.Visible = false;
                GetItemId_other();
                tblPricingWrapper.Visible = true;
            }
            else
            {
                lblResult1.Text = "";
                trvSection.Nodes.Clear();
                tblPricingWrapper.Visible = false;
            }
        }
    }
    protected void trvSection_SelectedNodeChanged(object sender, EventArgs e)
    {
        hdnSectionId.Value = trvSection.SelectedValue;
        Session.Add("cParent", hdnSectionId.Value);
        txtSearchItemName.Text = string.Empty;
        txtSearchItemName.Visible = true;
        btnSearch.Visible = true;
        LinkButton1.Visible = true;
        grdItemPrice.Visible = true;
        tdlOther.Visible = false;
        tdlCustomizePrice.Visible = true;
        BindGrid();
        lblParent.Text = GetItemDetials_forBreadCome(Convert.ToInt32(trvSection.SelectedValue));

    }

    protected void grdItemPrice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            string NewAddedItem = "";
            lblSelectLocation.Text = "";
            lblResult1.Text = "";
            lblMessage.Text = "";
            lblAdd.Text = "";
            string strPriceId = "0";
            decimal ditem_cost = 0;
            decimal dlabor_rate = 0;
            decimal dretail_multiplier = 0;
            string smeasure_unit = "";
            decimal dminimum_qty = 0;

            DataClassesDataContext _db = new DataClassesDataContext();
            int index = Convert.ToInt32(e.CommandArgument);
            hdnSectionId.Value = grdItemPrice.Rows[index].Cells[1].Text.Trim();
            TextBox txtQty = (TextBox)grdItemPrice.Rows[index].FindControl("txtQty");
            TextBox txtShortNote = (TextBox)grdItemPrice.Rows[index].FindControl("txtShortNote");
            //Label lblTotalPrice = (Label)grdItemPrice.Rows[index].FindControl("lblTotalPrice");
            DropDownList ddlLabor = (DropDownList)grdItemPrice.Rows[index].FindControl("ddlLabor");
            DropDownList ddlDirect = (DropDownList)grdItemPrice.Rows[index].FindControl("ddlDirect");
            strPriceId = grdItemPrice.DataKeys[index].Values[0].ToString();
            bool isMandatory = Convert.ToBoolean(grdItemPrice.DataKeys[index].Values[3]);
            int nItemId = Convert.ToInt32(grdItemPrice.DataKeys[index].Values[0]);

            dlabor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[index].Values[1].ToString());
            dretail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[index].Values[2].ToString());
            smeasure_unit = grdItemPrice.Rows[index].Cells[7].Text.Trim();
            ditem_cost = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            dminimum_qty = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text.Trim());
            decimal nTotalPrice = 0;


            if (txtQty.Text.Trim() == "")
            {
                lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Code.");
                lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Code.");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtQty.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                    return;
                }
            }
            if (isMandatory == false)
            {

                if (Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text) > Convert.ToDecimal(txtQty.Text))
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Code should be greater than minimum value");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Code should be greater than minimum value");
                    return;
                }
                if (Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text) <= Convert.ToDecimal(txtQty.Text))
                {
                    decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    decimal nQty = Convert.ToDecimal(txtQty.Text);

                    decimal nLaborRate = dlabor_rate * dretail_multiplier;
                    nLaborRate = nLaborRate * nQty;

                    if (ddlLabor.SelectedValue == "2")
                        nTotalPrice = nCost * nQty + nLaborRate;
                    else
                        nTotalPrice = nCost * nQty;
                    //lblTotalPrice.Text = nTotalPrice.ToString("c");
                }
            }
            else
            {
                if (Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text) > Convert.ToDecimal(txtQty.Text))
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Code should be greater than minimum value");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Code should be greater than minimum value");
                    return;
                }
                decimal nQty = Convert.ToDecimal(txtQty.Text);
                decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                if (nQty > 0)
                {
                    decimal nLaborRate = dlabor_rate * dretail_multiplier;
                    nLaborRate = nLaborRate * nQty;

                    if (ddlLabor.SelectedValue == "2")
                        nTotalPrice = nCost * nQty + nLaborRate;
                    else
                        nTotalPrice = nCost * nQty;
                    // lblTotalPrice.Text = nTotalPrice.ToString("c");
                }
                else
                {
                    nQty = Convert.ToDecimal(0.01);
                    nCost = Convert.ToDecimal(0);
                    nTotalPrice = nCost * nQty;
                    // lblTotalPrice.Text = nTotalPrice.ToString("c");

                }

            }








            if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
            {
                lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");

                lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");

                ddlCustomerLocations.Focus();
                return;
            }
            if (isMandatory == false)
            {
                if (!IsMandatoryItemCheck())
                {
                    ModalPopupExtender2.Show();
                    return;
                }
            }

            sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
            hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
            hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
            hdnSectionId.Value = strPriceId;
            bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
            pricing_detail price_detail = new pricing_detail();
            price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString();
            price_detail.measure_unit = smeasure_unit;
            price_detail.minimum_qty = dminimum_qty;
            price_detail.retail_multiplier = dretail_multiplier;
            price_detail.labor_rate = dlabor_rate;
            price_detail.item_cost = ditem_cost;

            price_detail.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
            //co_price_detail.change_order_id = Convert.ToInt32(hdnChEstId.Value);
            price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
            price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
            price_detail.item_id = Convert.ToInt32(hdnSectionId.Value);
            price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
            if (isMandatory)
            {
                if (Convert.ToDecimal(txtQty.Text) > 0)
                {
                    price_detail.quantity = Convert.ToDecimal(txtQty.Text);
                }
                else
                {
                    price_detail.quantity = Convert.ToDecimal(0.01);
                }
            }
            else
            {
                price_detail.quantity = Convert.ToDecimal(txtQty.Text);
            }
            price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
            price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

            if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
            {
                price_detail.total_retail_price = nTotalPrice;//Convert.ToDecimal(lblTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                price_detail.total_direct_price = 0;
            }
            else
            {
                price_detail.total_retail_price = 0;
                price_detail.total_direct_price = nTotalPrice;//Convert.ToDecimal(lblTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }

            if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
                price_detail.labor_rate = 0;
            price_detail.section_serial = Convert.ToDecimal(hdnSectionSerial.Value);
            hdnItemCnt.Value = GetSerial().ToString();
            price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
            price_detail.pricing_type = "A";
            price_detail.short_notes = txtShortNote.Text;
            price_detail.create_date = DateTime.Now;
            price_detail.last_update_date = DateTime.Now;
            price_detail.execution_unit = 0;
            price_detail.sort_id = 0;
            price_detail.is_mandatory = isMandatory;
            price_detail.is_CommissionExclude = IsCommissionExclude;

            _db.pricing_details.InsertOnSubmit(price_detail);

            _db.SubmitChanges();
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to estimate list, select another item or Location/Section");
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to estimate list, select another item or Location/Section");

            if (Session["sNewAddedItem"] == null)
            {
                Session.Add("sNewAddedItem", price_detail.item_id.ToString());
            }
            else
            {
                NewAddedItem = Session["sNewAddedItem"].ToString();

                NewAddedItem += "," + price_detail.item_id.ToString();

                Session.Add("sNewAddedItem", NewAddedItem);

            }

            DataTable dtItem = new DataTable();
            DataTable dtItemDirect = new DataTable();
            if (Session["Item_list_Direct"] != null)
                dtItemDirect = (DataTable)Session["Item_list_Direct"];
            else
                dtItemDirect = LoadItemTable();

            if (Session["Item_list"] != null)
                dtItem = (DataTable)Session["Item_list"];
            else
                dtItem = LoadItemTable();

            // Cost
            decimal dOrginalCost = 0;
            decimal dOrginalTotalCost = 0;
            decimal dLaborTotal = 0;
            decimal dLineItemTotal = 0;

            decimal dTPrice = 0;

            string sItemName = price_detail.item_name.ToString();

            decimal dItemCost = Convert.ToDecimal(price_detail.item_cost);
            decimal dRetail_multiplier = Convert.ToDecimal(price_detail.retail_multiplier);
            decimal dQuantity = Convert.ToDecimal(price_detail.quantity);
            decimal dLabor_rate = Convert.ToDecimal(price_detail.labor_rate);
            if (price_detail.is_direct == 1)
                dTPrice = Convert.ToDecimal(price_detail.total_retail_price);
            else
                dTPrice = Convert.ToDecimal(price_detail.total_direct_price);

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


            DataRow drNew = null;
            if (price_detail.is_direct == 1)
                drNew = dtItem.NewRow();
            else
                drNew = dtItemDirect.NewRow();
            drNew["pricing_id"] = price_detail.pricing_id;
            drNew["item_id"] = price_detail.item_id;
            drNew["labor_id"] = price_detail.labor_id;
            drNew["section_serial"] = price_detail.section_serial;
            drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
            drNew["section_name"] = price_detail.section_name;
            drNew["item_name"] = price_detail.item_name;
            drNew["measure_unit"] = price_detail.measure_unit;
            drNew["item_cost"] = price_detail.item_cost;
            drNew["total_retail_price"] = price_detail.total_retail_price;
            drNew["total_direct_price"] = price_detail.total_direct_price;
            drNew["minimum_qty"] = price_detail.minimum_qty;
            drNew["quantity"] = price_detail.quantity;
            drNew["retail_multiplier"] = price_detail.retail_multiplier;
            drNew["labor_rate"] = price_detail.labor_rate;
            drNew["short_notes"] = price_detail.short_notes;
            drNew["tmpCol"] = "";
            drNew["pricing_type"] = "A";
            drNew["is_mandatory"] = price_detail.is_mandatory;
            drNew["create_date"] = price_detail.create_date;
            drNew["last_update_date"] = price_detail.last_update_date;
            drNew["customer_id"] = price_detail.customer_id;
            drNew["section_level"] = price_detail.section_level;
            drNew["location_id"] = price_detail.location_id;
            drNew["sales_person_id"] = price_detail.sales_person_id;
            drNew["client_id"] = price_detail.client_id;

            drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
            drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
            drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total

            if (price_detail.is_direct == 1)
                dtItem.Rows.Add(drNew);
            else
                dtItemDirect.Rows.Add(drNew);
            Session.Add("Item_list", dtItem);
            Session.Add("Item_list_Direct", dtItemDirect);


            hdnSortDesc.Value = "1";
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
            Calculate_Total();
            //Item updated successfully to estimate list, select another item or Location/Section

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

            if (_db.pricing_details.Any(p => p.item_id == nItemId && p.location_id == Convert.ToInt32(ddlCustomerLocations.SelectedValue) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
            {
                grdItemPrice.Rows[index].BackColor = System.Drawing.ColorTranslator.FromHtml("#E0F8E0");
                grdItemPrice.Rows[index].BorderColor = System.Drawing.ColorTranslator.FromHtml("#fff");
                grdItemPrice.Rows[index].BorderStyle = BorderStyle.Solid;
                grdItemPrice.Rows[index].BorderWidth = 2;
            }
        }
    }
    protected void grdSelectedItem_RowEditing(object sender, GridViewEditEventArgs e)
    {
        try
        {
            GridView grdSelecterdItem = (GridView)sender;
            TextBox txtquantity = (TextBox)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("txtquantity");
            Label lblquantity = (Label)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("lblquantity");
            TextBox txtshort_notes = (TextBox)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("txtshort_notes");
            Label lblshort_notes = (Label)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("lblshort_notes");
            Label lblshort_notes_r = (Label)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("lblshort_notes_r");
            LinkButton lnkOpen = (LinkButton)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("lnkOpen");


            txtquantity.Visible = true;
            lblquantity.Visible = false;
            txtshort_notes.Visible = true;
            lblshort_notes.Visible = false;
            lblshort_notes_r.Visible = false;
            lnkOpen.Visible = false;

            LinkButton btn = (LinkButton)grdSelecterdItem.Rows[e.NewEditIndex].Cells[11].Controls[0];
            btn.Text = "Update";
            btn.CommandName = "Update";
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        lblMessage.Text = "";
        txtNewEstimateName.Text = "";
        modUpdateEstimate.Hide();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblMessage.Text = "";
        if (txtNewEstimateName.Text.Trim() == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("New Estimate Name is required field");

            modUpdateEstimate.Show();
            return;
        }

        string strNewName = txtNewEstimateName.Text.Trim();
        DataClassesDataContext _db = new DataClassesDataContext();
        customer_estimate cus_est = new customer_estimate();
        int ncid = Convert.ToInt32(hdnCustomerId.Value);
        int nestId = Convert.ToInt32(hdnEstimateId.Value);
        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0)
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == ncid && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

        cus_est.estimate_name = strNewName;

        _db.SubmitChanges();


        lblMessage.Text = "";
        txtNewEstimateName.Text = "";
        modUpdateEstimate.Hide();

        Response.Redirect("pricing.aspx?eid=" + nestId + "&cid=" + ncid);
    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelecterdItem = (GridView)sender;
            Label lblshort_notes = (Label)e.Row.FindControl("lblshort_notes");
            TextBox txtshort_notes = (TextBox)e.Row.FindControl("txtshort_notes");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            string str = lblshort_notes.Text.Replace("&nbsp;", "");
            Label lblItemName = (Label)e.Row.FindControl("lblItemName");

            string strLocation = grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strSection = grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[3].ToString();

            bool IsMandatory = Convert.ToBoolean(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[1]);

            decimal dTotalPrice = Convert.ToDecimal(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[4]);

            DateTime dtCreateDate = Convert.ToDateTime(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[6]);
            int item_id = Convert.ToInt32(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[7]);

            if (rdoSort.SelectedValue == "1")
            {
                lblItemName.Text = strSection;

            }
            else
            {
                lblItemName.Text = strLocation;
            }
            if (str != "" && str.Length > 50)
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str.Substring(0, 50) + "...";
                lblshort_notes.ToolTip = str;
                lnkOpen.Visible = true;

            }
            else
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str;
                lnkOpen.Visible = false;

            }
            if (IsMandatory)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");

            }

            if (Session["sNewAddedItem"] != null)
            {
                string NewAddedItem = Session["sNewAddedItem"].ToString();
                if (NewAddedItem.Contains(item_id.ToString()) && strLocation.Trim() == ddlCustomerLocations.SelectedItem.Text.Trim())
                {
                    e.Row.Attributes.CssStyle.Add("background", "#E0F8E0");
                    e.Row.Attributes.CssStyle.Add("border", "2px solid #fff");
                    //  e.Row.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }

            if (chkCustDisp.Checked)
            {
                grdSelecterdItem.Columns[5].Visible = false;
                grdSelecterdItem.Columns[6].Visible = false;
                grdSelecterdItem.Columns[7].Visible = false;
                grdSelecterdItem.Columns[8].Visible = false;
                grdSelecterdItem.Columns[9].Visible = false;
                grdSelecterdItem.Columns[10].Visible = false;
                grdSelecterdItem.Columns[11].Visible = false;
                grdSelecterdItem.Columns[2].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                grdSelecterdItem.Columns[4].ItemStyle.Width = new Unit(41, UnitType.Percentage);
            }
            if (chkPMDisplay.Checked)
            {
                grdSelecterdItem.Columns[7].Visible = true;
                grdSelecterdItem.Columns[9].Visible = true;
                grdSelecterdItem.Columns[10].Visible = true;


            }
            else
            {
                grdSelecterdItem.Columns[7].Visible = false;
                grdSelecterdItem.Columns[9].Visible = false;
                grdSelecterdItem.Columns[10].Visible = false;

            }

        }

    }
    //protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    //{

    //    if (ddlStatus.SelectedValue == "3")
    //    {
    //        lblSaleDate.Visible = true;
    //        txtSaleDate.Visible = true;
    //        imgSaleDate.Visible = true;

    //        btnSavePopUp.Visible = true;

    //        btnSave.Visible = false;

    //    }
    //    else
    //    {
    //        lblSaleDate.Visible = false;
    //        txtSaleDate.Visible = false;
    //        imgSaleDate.Visible = false;
    //        txtSaleDate.Text = "";

    //        btnSavePopUp.Visible = false;

    //        btnSave.Visible = true;
    //    }
    //}
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        try
        {
            lblSelectLocation.Text = "";
            lblAdd.Text = "";
            lblMessage.Text = "";
            lblResult1.Text = "";

            string strNewName = lblEstimateName.Text.Trim();
            DataClassesDataContext _db = new DataClassesDataContext();
            customer_estimate cus_est = new customer_estimate();

            int ncid = Convert.ToInt32(hdnCustomerId.Value);
            int nestId = Convert.ToInt32(hdnEstimateId.Value);
            if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0)
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == ncid && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            cus_est.estimate_name = strNewName;

            cus_est.status_id = 1; //Convert.ToInt32(ddlStatus.SelectedValue);

            cus_est.sale_date = "";

            cus_est.estimate_comments = txtComments.Text;
            decimal TaxPer = 0;
            try
            {
                TaxPer = Convert.ToDecimal(txtTaxPer.Text.Trim());
            }
            catch (Exception ex)
            {
                TaxPer = 0;
            }
            cus_est.tax_rate = TaxPer;



            if (rdbEstimateIsActive.SelectedValue == "1")
                cus_est.IsEstimateActive = true;
            else
                cus_est.IsEstimateActive = false;

            cus_est.IsCustDisplay = chkCustDisp.Checked;

            string strDisplay = "0";
            if (chkCustDisp.Checked)
            {
                strDisplay = "1";
            }

            int nSuperintendentId = 0;
            if (ddlSuperintendent.SelectedItem.Text != "Select")
            {
                nSuperintendentId = Convert.ToInt32(ddlSuperintendent.SelectedValue);
            }
            string strQ = "UPDATE customer_estimate SET estimate_name='" + cus_est.estimate_name.Replace("'", "''") +
                "', sale_date='" + cus_est.sale_date + "',estimate_comments='" + cus_est.estimate_comments.Replace("'", "''") +
                "',status_id=" + cus_est.status_id +
                ",sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " , tax_rate=" + TaxPer +
                 ", IsEstimateActive =" + rdbEstimateIsActive.SelectedValue + ",  IsCustDisplay = " + strDisplay +
                " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
                " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
                " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ, string.Empty);

            string strCustQ = "UPDATE customers SET  SuperintendentId =" + nSuperintendentId + "  WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strCustQ, string.Empty);
            estimate_payment est_pay = new estimate_payment();
            if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
            {
                string sSql = "UPDATE estimate_payments SET tax_rate=" + TaxPer + " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(sSql, string.Empty);
            }
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
            lblResult1.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
            // lblResult1.ForeColor = System.Drawing.Color.Green;

            // Need to Show confirmation Popup for Save the Sold Estimate.

            if (cus_est.status_id == 3)
            {
                Response.Redirect("sold_estimate.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
            }
        }
        catch (Exception ex)
        {
            lblResult1.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected void btnDuplicate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDuplicate.ID, btnDuplicate.GetType().Name, "Click"); 
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";
        string est = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {

            List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
            List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
            List<pricing_detail> Pd_List = _db.pricing_details.Where(pd => pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A").ToList();

            int nEstId = 0;
            var result = (from ce in _db.customer_estimates
                          where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select ce.estimate_id);

            int n = result.Count();
            if (result != null && n > 0)
                nEstId = result.Max();
            nEstId = nEstId + 1;
            hdnEstimateId.Value = nEstId.ToString();
            customer_estimate cus_est = new customer_estimate();
            cus_est.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_est.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_est.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
            cus_est.status_id = 1;
            cus_est.sale_date = "";
            cus_est.estimate_comments = txtComments.Text;
            cus_est.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
             est = "Duplicate of" + " " + lblEstimateName.Text + " #" + hdnEstimateId.Value;
            cus_est.estimate_name = est;
            cus_est.create_date = DateTime.Now;
            cus_est.last_update_date = DateTime.Now;
            cus_est.IsEstimateActive = true;
            cus_est.IsCustDisplay = false;
            cus_est.JobId = 0;
            _db.customer_estimates.InsertOnSubmit(cus_est);
            foreach (customer_location objcl in Cust_LocList)
            {
                customer_location cus_loc = new customer_location();
                cus_loc.client_id = objcl.client_id;
                cus_loc.customer_id = objcl.customer_id;
                cus_loc.location_id = objcl.location_id;
                cus_loc.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                _db.customer_locations.InsertOnSubmit(cus_loc);
            }
            foreach (customer_section objcs in Cust_SecList)
            {
                customer_section cus_sec = new customer_section();
                cus_sec.client_id = objcs.client_id;
                cus_sec.customer_id = objcs.customer_id;
                cus_sec.section_id = objcs.section_id;
                cus_sec.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                cus_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                _db.customer_sections.InsertOnSubmit(cus_sec);
            }
            foreach (pricing_detail objPd in Pd_List)
            {
                pricing_detail pd = new pricing_detail();
                pd.client_id = objPd.client_id; ;
                pd.customer_id = objPd.customer_id;
                pd.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                pd.location_id = objPd.location_id; ;
                pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                pd.section_level = objPd.section_level;
                pd.item_id = objPd.item_id;
                pd.section_name = objPd.section_name;
                pd.item_name = objPd.item_name;
                pd.measure_unit = objPd.measure_unit;
                pd.minimum_qty = objPd.minimum_qty;
                pd.quantity = objPd.quantity;
                pd.retail_multiplier = objPd.retail_multiplier;
                pd.labor_id = objPd.labor_id;
                pd.is_direct = objPd.is_direct;
                pd.item_cost = objPd.item_cost;
                pd.total_direct_price = objPd.total_direct_price;
                pd.total_retail_price = objPd.total_retail_price;
                pd.labor_rate = objPd.labor_rate;
                pd.section_serial = objPd.section_serial;
                pd.item_cnt = objPd.item_cnt;
                pd.short_notes = objPd.short_notes;
                pd.pricing_type = objPd.pricing_type;
                pd.create_date = DateTime.Now;
                pd.last_update_date = DateTime.Now;
                pd.sort_id = objPd.sort_id;
                pd.is_mandatory = objPd.is_mandatory;
                pd.is_CommissionExclude = objPd.is_CommissionExclude;

                _db.pricing_details.InsertOnSubmit(pd);
            }

            _db.SubmitChanges();

        }
        Session.Add("sSytemMassage", "" + est + " has been successfully.");
        Response.Redirect("pricing.aspx?eid=" + Convert.ToInt32(hdnEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
    }


    protected void lnkAddMoreSections_Click(object sender, EventArgs e)
    {
        Response.Redirect("estimate_sections.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&sid=1");
    }
    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    protected string GetTotalPrice()
    {

        decimal nGrandtotal = Convert.ToDecimal(grandtotal);
        decimal nGrandtotalCost = Convert.ToDecimal(grandtotalCost);

        if (txtTaxPer.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(txtTaxPer.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);
        //  string Total = string.Empty;
        //if (chkPMDisplay.Checked)
        //    Total = "Total Ext. Cost: " + nGrandtotalCost.ToString("c")+ "         Total Ext. Price: "+nGrandtotal.ToString("c")+"      Total with Tax: " + grandtotal.ToString("c");
        //else
        //    Total = "Total with Tax: " + grandtotal.ToString("c");

        return "Total with Tax: " + grandtotal.ToString("c");
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
        decimal nGrandtotal_directCost = Convert.ToDecimal(grandtotal_directCost);

        if (txtTaxPer.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(txtTaxPer.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);
        // string Total = string.Empty;
        //if (chkPMDisplay.Checked)
        //    Total = "Total Ext. Cost: " + nGrandtotal_directCost.ToString("c") + "         Total Ext. Price: " + nGrandtotal_direct.ToString("c") + "      Total with Tax: " + grandtotal_direct.ToString("c");
        //else
        //    Total = "Total with tax " + grandtotal_direct.ToString("c");
        return "Total with tax " + grandtotal_direct.ToString("c");
    }
    protected string GetTotalExtCostDirect()
    {

        decimal nGrandtotalCost = Convert.ToDecimal(grandtotal_directCost);
        // string Total = string.Empty;
        //Total = "Total Ext. Cost: " + nGrandtotalCost.ToString("c");
        return nGrandtotalCost.ToString("c");
    }
    protected string GetTotalExPriceDirect()
    {

        decimal nGrandtotal = Convert.ToDecimal(grandtotal_direct);
        //string Total = string.Empty;
        //Total = "Total Ext. Price: " + nGrandtotal.ToString("c");
        return nGrandtotal.ToString("c");
    }

    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            LinkButton lnkMove1 = (LinkButton)e.Row.FindControl("lnkMove1");
            LinkButton lnkSendEmail1 = (LinkButton)e.Row.FindControl("lnkSendEmail1");
            Label lblMSub = (Label)e.Row.FindControl("lblMSub");



            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail1.Visible = false;
                if (e.Row.RowIndex == 0)
                {
                    lnkMove1.Visible = false;
                }
                else
                {
                    lnkMove1.Visible = true;
                }
            }
            else
            {
                lnkMove1.Visible = false;
                if (chkPMDisplay.Checked)
                {
                    lnkSendEmail1.Visible = true;
                }
                else
                {
                    lnkSendEmail1.Visible = false;
                }
            }

            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            if (Session["Item_list"] != null)
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
                dv.Sort = "last_update_date DESC";


                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "pricing_id", "is_mandatory", "location_name", "section_name", "total_retail_price", "total_direct_price", "create_date", "item_id" };
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
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;


                subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");

                Label lblSubTotalCost = footerRow.FindControl("lblSubTotalCost") as Label;
                subtotalCost += Double.Parse((row.FindControl("lblTotal_Cost") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

                lblSubTotalCost.Text = subtotalCost.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                if (chkCustDisp.Checked)
                    lblSubTotalLabel.Text = "Sub Total: " + subtotal.ToString("c");
                else
                    lblSubTotalLabel.Text = "Sub Total:";



            }
            grandtotal += subtotal;
            lblMSub.Text = " (" + subtotal.ToString("c") + ")";
            subtotal = 0.0;
            grandtotalCost += subtotalCost;
            subtotalCost = 0.0;
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblGtotalExtCost = (Label)e.Row.FindControl("lblGtotalExtCost");
            Label lblGtotalExtPrice = (Label)e.Row.FindControl("lblGtotalExtPrice");
            Label lblGtotalLabel = (Label)e.Row.FindControl("lblGtotalLabel");
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
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strCondition = "";
        string strOrderby = "";

        if (rdoSort.SelectedValue == "1")
        {
            strOrderby = " ORDER BY p.section_level ASC ";
        }
        else
        {
            strOrderby = " ORDER BY lc.location_name ASC ";

        }

        int nCustId = Convert.ToInt32(hdnCustomerId.Value);
        int nEstId = Convert.ToInt32(hdnEstimateId.Value);
        int nClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        string strQ = "SELECT p.pricing_id, p.item_id, p.labor_id, p.section_serial, lc.location_name, p.section_name, p.item_name, " +
                    " p.measure_unit, p.item_cost, p.total_retail_price, p.total_direct_price, p.minimum_qty, p.quantity, " +
                    " p.retail_multiplier, p.labor_rate, p.short_notes, '' AS tmpCol, 'A' AS pricing_type, p.is_mandatory,  " +
                    " p.create_date, p.last_update_date, p.section_level, p.location_id,p.customer_id, p.sales_person_id, p.client_id, " +

                      " ISNULL(CAST(CASE WHEN p.item_name LIKE '%Other%' THEN(CASE WHEN p.is_direct = 1 THEN CAST(p.total_retail_price AS decimal(10, 2))" +
                    " ELSE CAST(p.total_direct_price AS decimal(10, 2)) END / NULLIF(p.quantity, 0) / 2) ELSE(CASE WHEN p.retail_multiplier > 0 THEN CAST(p.item_cost / NULLIF(p.retail_multiplier, 0) AS decimal(10, 2)) ELSE(CAST(p.item_cost AS decimal(10, 2))) END) END AS decimal(10, 2)), 0) AS unit_cost," +

                    " ISNULL(CAST(CASE WHEN p.item_name LIKE '%Other%' THEN(CASE WHEN p.is_direct = 1 THEN CAST(p.total_retail_price AS decimal(10, 2)) ELSE CAST(p.total_direct_price AS decimal(10, 2)) END / NULLIF(p.quantity, 0) / 2)" +

                    "  ELSE(CASE WHEN p.retail_multiplier > 0 THEN CAST(p.item_cost / NULLIF(p.retail_multiplier, 0) AS decimal(10, 2)) ELSE(p.item_cost) END) END * p.quantity" +
                    " + CASE WHEN p.item_name LIKE '%Other%' THEN 0 ELSE CAST(p.labor_rate AS decimal(10, 2)) * p.quantity END AS decimal(10, 2)), 0) AS total_unit_cost," +
                    " cast(CASE WHEN p.item_name LIKE '%Other%' THEN 0 ELSE cast(p.labor_rate as decimal(10, 2)) * cast(p.quantity as decimal(10, 2)) END as decimal(10, 2)) AS total_labor_cost" +

                    " FROM pricing_details AS p " +
                    " INNER JOIN location AS lc ON p.location_id = lc.location_id " +
                    " WHERE(p.location_id IN(SELECT location_id FROM customer_locations WHERE estimate_id = " + nEstId + " AND customer_id = " + nCustId + " AND client_id = " + nClientId + ")) " +
                    " AND(p.section_level IN(SELECT section_id FROM customer_sections WHERE estimate_id = " + nEstId + " AND customer_id = " + nCustId + " AND client_id = " + nClientId + ")) " +
                    " AND p.is_direct = " + nDirectId + " AND p.estimate_id = " + nEstId + " AND p.customer_id = " + nCustId + " AND p.client_id = " + nClientId + " AND p.pricing_type = 'A' " +
                    " " + strCondition + strOrderby;

        List<PricingDetailModel> mList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();

        DataTable dt = SessionInfo.LINQToDataTable(mList);


        if (nDirectId == 1)
            Session.Add("Item_list", dt);
        else
            Session.Add("Item_list_Direct", dt);

        //-----------
        DataView dv = dt.DefaultView;
        if (rdoSort.SelectedValue == "1")
        {
            dv.RowFilter = "location_id =" + colId;
        }
        else
        {
            dv.RowFilter = "section_level =" + colId;
        }
        if (hdnSortDesc.Value == "1")
            dv.Sort = "last_update_date DESC";
        else
            dv.Sort = "item_id ASC";
        //-------------

        grd.DataSource = dv;
        grd.DataKeyNames = new string[] { "pricing_id", "is_mandatory", "location_name", "section_name", "total_retail_price", "total_direct_price", "create_date", "item_id" };
        grd.DataBind();
    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";
        hdnSortDesc.Value = "0";
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
        Calculate_Total();


        csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Copy Selected Item(s)", "Edit Selected Item(s)", "Delete Selected Item(s)", "Add Notes", "Move to Top", "grdSelectedItem1_0_chkSingle", "grdSelectedItem1_0_chkAll" });
    }
    protected void grdSelectedItem2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridView grdSelectedItem2 = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem2.DataKeys[e.RowIndex].Values[0].ToString();
        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

        bool Iexists = dtItemDirect.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(Convert.ToInt32(hdnPricingId.Value))).Count() > 0;
        string strQ = "Delete pricing_details WHERE pricing_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);
        if (Iexists)
        {
            var rows = dtItemDirect.Select("pricing_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
            foreach (var row in rows)
                row.Delete();
        }
        Session.Add("Item_list_Direct", dtItemDirect);
        hdnSortDesc.Value = "0";
        BindSelectedItemGrid_Direct();
        BindSelectedItemGrid();
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
        Calculate_Total();
        hdnPricingId.Value = "0";
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");
    }
    protected void grdSelectedItem2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView grdSelectedItem2 = (GridView)sender;
        TextBox txtquantity1 = (TextBox)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("txtquantity1");
        Label lblquantity1 = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblquantity1");
        TextBox txtshort_notes1 = (TextBox)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("txtshort_notes1");
        Label lblshort_notes1 = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblshort_notes1");
        Label lblshort_notes1_r = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblshort_notes1_r");
        LinkButton lnkOpen1 = (LinkButton)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lnkOpen1");

        txtquantity1.Visible = true;
        lblquantity1.Visible = false;
        txtshort_notes1.Visible = true;
        lblshort_notes1.Visible = false;
        lblshort_notes1_r.Visible = false;
        lnkOpen1.Visible = false;

        LinkButton btn = (LinkButton)grdSelectedItem2.Rows[e.NewEditIndex].Cells[11].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelectedItem2 = (GridView)sender;
            Label lblshort_notes1 = (Label)e.Row.FindControl("lblshort_notes1");
            TextBox txtshort_notes1 = (TextBox)e.Row.FindControl("txtshort_notes1");
            LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
            bool IsMandatory = Convert.ToBoolean(grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[1]);

            Label lblItemName2 = (Label)e.Row.FindControl("lblItemName2");

            string strLocation = grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strSection = grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[3].ToString();

            if (rdoSort.SelectedValue == "1")
            {
                lblItemName2.Text = strSection;

            }
            else
            {
                lblItemName2.Text = strLocation;
            }
            string str = lblshort_notes1.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 50)
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str.Substring(0, 50) + "...";
                lblshort_notes1.ToolTip = str;
                lnkOpen1.Visible = true;

            }
            else
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str;
                lnkOpen1.Visible = false;

            }
            if (IsMandatory)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");

            }
            if (chkCustDisp.Checked)
            {
                grdSelectedItem2.Columns[5].Visible = false;
                grdSelectedItem2.Columns[6].Visible = false;
                grdSelectedItem2.Columns[7].Visible = false;
                grdSelectedItem2.Columns[8].Visible = false;
                grdSelectedItem2.Columns[9].Visible = false;
                grdSelectedItem2.Columns[10].Visible = false;
                grdSelectedItem2.Columns[11].Visible = false;
                grdSelectedItem2.Columns[2].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                grdSelectedItem2.Columns[4].ItemStyle.Width = new Unit(41, UnitType.Percentage);
            }
            if (chkPMDisplay.Checked)
            {
                grdSelectedItem2.Columns[7].Visible = true;
                grdSelectedItem2.Columns[9].Visible = true;
                grdSelectedItem2.Columns[10].Visible = true;


            }
            else
            {
                grdSelectedItem2.Columns[7].Visible = false;
                grdSelectedItem2.Columns[9].Visible = false;
                grdSelectedItem2.Columns[10].Visible = false;

            }

        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            int nDirectId = 2;
            LinkButton lnkMove2 = (LinkButton)e.Row.FindControl("lnkMove2");
            LinkButton lnkSendEmail2 = (LinkButton)e.Row.FindControl("lnkSendEmail2");
            Label lblMSub2 = (Label)e.Row.FindControl("lblMSub2");
            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail2.Visible = false;
                if (e.Row.RowIndex == 0)
                {
                    lnkMove2.Visible = false;
                }
                else
                {
                    lnkMove2.Visible = true;

                }
            }
            else
            {
                lnkMove2.Visible = false;
                if (chkPMDisplay.Checked)
                {
                    lnkSendEmail2.Visible = true;
                }
                else
                {
                    lnkSendEmail2.Visible = false;
                }

            }


            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;

            if (Session["Item_list_Direct"] != null)
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
                gv.DataKeyNames = new string[] { "pricing_id", "is_mandatory", "location_name", "section_name", "create_date", "item_id" };
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
                //HyperLink hypView1 = row.FindControl("hypView1") as HyperLink;
                //if (hypView1.NavigateUrl == "" || hypView1.NavigateUrl == "Document/")
                //    hypView1.Text = "";
                Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal2.Text = subtotal_diect.ToString("c");

                Label lblSubTotalCost3 = footerRow.FindControl("lblSubTotalCost3") as Label;
                subtotal_diectCost += Double.Parse((row.FindControl("lblTotal_Cost3") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                lblSubTotalCost3.Text = subtotal_diectCost.ToString("c");

                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader2.Text = "Section";

                }
                else
                {
                    lblHeader2.Text = "Location";

                }
                if (chkCustDisp.Checked)
                    lblSubTotalLabel2.Text = "Sub Total:" + subtotal_diect.ToString("c");
                else
                    lblSubTotalLabel2.Text = "Sub Total:";
            }
            grandtotal_direct += subtotal_diect;
            lblMSub2.Text = " (" + subtotal_diect.ToString("c") + ")";
            subtotal_diect = 0.0;

            grandtotal_directCost += subtotal_diectCost;
            subtotal_diectCost = 0.0;
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



    protected void grdSelectedItem1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gvr = (GridView)sender;
        GridRowUpdatingNonDirect(gvr, e.RowIndex);

        #region BlockCode
        //lblSelectLocation.Text = "";
        //lblAdd.Text = "";
        //lblMessage.Text = "";
        //lblResult1.Text = "";

        //GridView grdSelectedItem = (GridView)sender;
        //DataClassesDataContext _db = new DataClassesDataContext();

        //TextBox txtquantity = (TextBox)grdSelectedItem.Rows[e.RowIndex].FindControl("txtquantity");
        //Label lblTotal_price = (Label)grdSelectedItem.Rows[e.RowIndex].FindControl("lblTotal_price");
        //TextBox txtshort_notes = (TextBox)grdSelectedItem.Rows[e.RowIndex].FindControl("txtshort_notes");
        //int nPricingId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        //decimal dTotalPrice = Convert.ToDecimal(lblTotal_price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        //string strQ = "UPDATE pricing_details SET quantity=" + Convert.ToDecimal(txtquantity.Text) + " ,total_retail_price=" + dTotalPrice + " ,short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        //_db.ExecuteCommand(strQ, string.Empty);

        //subtotal = 0.0;
        //subtotal_diect = 0.0;
        //hdnSortDesc.Value = "0";
        //BindSelectedItemGrid();
        //BindSelectedItemGrid_Direct();
        //if (grdGrouping.Rows.Count == 0)
        //{
        //    lblRetailPricingHeader.Visible = false;
        //}
        //else
        //{
        //    lblRetailPricingHeader.Visible = true;
        //}
        //if (grdGroupingDirect.Rows.Count == 0)
        //{
        //    lblDirectPricingHeader.Visible = false;
        //}
        //else
        //{
        //    lblDirectPricingHeader.Visible = true;
        //}
        //hdnPricingId.Value = "0";
        //Calculate_Total();
        //lblResult1.Text = "Item updated successfully";
        //lblResult1.ForeColor = System.Drawing.Color.Green;
        //lblAdd.Text = "Item updated successfully";
        //lblAdd.ForeColor = System.Drawing.Color.Green;
        #endregion

    }

    private void GridRowUpdatingNonDirect(GridView gvr, int nIndex)
    {
        if (nIndex < 0) return;

        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        TextBox txtquantity = (TextBox)gvr.Rows[nIndex].FindControl("txtquantity");
        Label lblTotal_price = (Label)gvr.Rows[nIndex].FindControl("lblTotal_price");
        TextBox txtshort_notes = (TextBox)gvr.Rows[nIndex].FindControl("txtshort_notes");




        int nPricingId = Convert.ToInt32(gvr.DataKeys[Convert.ToInt32(nIndex)].Values[0]);
        decimal dTotalPrice = 0;
        try
        {
            dTotalPrice = Convert.ToDecimal(lblTotal_price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        }
        catch
        {
            dTotalPrice = 0;

        }

        decimal nQty = 0;
        try
        {
            nQty = Convert.ToDecimal(txtquantity.Text);
        }
        catch
        {
            nQty = 1;

        }
        string strQ = "UPDATE pricing_details SET quantity=" + nQty + " ,total_retail_price=" + dTotalPrice + " ,short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);
        DataTable dtItem = (DataTable)Session["Item_list"];

        bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
        if (Iexists)
        {
            var rows = dtItem.Select("pricing_id =" + nPricingId + "");
            foreach (var row in rows)
            {
                row["quantity"] = nQty;
                row["total_retail_price"] = dTotalPrice;
                row["short_notes"] = txtshort_notes.Text.Replace("'", "''");
            }
        }
        Session.Add("Item_list", dtItem);

        subtotal = 0.0;
        subtotal_diect = 0.0;
        hdnSortDesc.Value = "0";
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
        hdnPricingId.Value = "0";
        Calculate_Total();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");


    }
    protected void grdSelectedItem2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gvr = (GridView)sender;
        GridRowUpdatingDirect(gvr, e.RowIndex);

        #region BlockCode
        //lblSelectLocation.Text = "";
        //lblAdd.Text = "";
        //lblMessage.Text = "";
        //lblResult1.Text = "";

        //GridView grdSelectedItem2 = (GridView)sender;
        //DataClassesDataContext _db = new DataClassesDataContext();
        //TextBox txtquantity1 = (TextBox)grdSelectedItem2.Rows[e.RowIndex].FindControl("txtquantity1");
        //Label lblTotal_price2 = (Label)grdSelectedItem2.Rows[e.RowIndex].FindControl("lblTotal_price2");
        //TextBox txtshort_notes1 = (TextBox)grdSelectedItem2.Rows[e.RowIndex].FindControl("txtshort_notes1");
        //int nPricingId = Convert.ToInt32(grdSelectedItem2.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        //decimal dTotalDirectPrice = Convert.ToDecimal(lblTotal_price2.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        //string strQ = "UPDATE pricing_details SET quantity=" + Convert.ToDecimal(txtquantity1.Text) + " ,total_direct_price=" + dTotalDirectPrice + " ,short_notes='" + txtshort_notes1.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        //_db.ExecuteCommand(strQ, string.Empty);

        //subtotal = 0.0;
        //subtotal_diect = 0.0;
        //hdnSortDesc.Value = "0";
        //BindSelectedItemGrid();
        //BindSelectedItemGrid_Direct();
        //if (grdGrouping.Rows.Count == 0)
        //{
        //    lblRetailPricingHeader.Visible = false;
        //}
        //else
        //{
        //    lblRetailPricingHeader.Visible = true;
        //}
        //if (grdGroupingDirect.Rows.Count == 0)
        //{
        //    lblDirectPricingHeader.Visible = false;
        //}
        //else
        //{
        //    lblDirectPricingHeader.Visible = true;
        //}
        //hdnPricingId.Value = "0";
        //Calculate_Total();
        //lblResult1.Text = "Item updated successfully";
        //lblResult1.ForeColor = System.Drawing.Color.Green;
        //lblAdd.Text = "Item updated successfully";
        //lblAdd.ForeColor = System.Drawing.Color.Green;

        #endregion

    }
    private void GridRowUpdatingDirect(GridView gvr, int nIndex)
    {
        if (nIndex < 0) return;

        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        TextBox txtquantity1 = (TextBox)gvr.Rows[nIndex].FindControl("txtquantity1");
        Label lblTotal_price2 = (Label)gvr.Rows[nIndex].FindControl("lblTotal_price2");
        TextBox txtshort_notes1 = (TextBox)gvr.Rows[nIndex].FindControl("txtshort_notes1");
        int nPricingId = Convert.ToInt32(gvr.DataKeys[Convert.ToInt32(nIndex)].Values[0]);

        decimal dTotalDirectPrice = 0;
        try
        {
            dTotalDirectPrice = Convert.ToDecimal(lblTotal_price2.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        }
        catch
        {
            dTotalDirectPrice = 0;

        }

        decimal nQty = 0;
        try
        {
            nQty = Convert.ToDecimal(txtquantity1.Text);
        }
        catch
        {
            nQty = 1;

        }
        string strQ = "UPDATE pricing_details SET quantity=" + nQty + " ,total_direct_price=" + dTotalDirectPrice + " ,short_notes='" + txtshort_notes1.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);

        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

        bool Iexists = dtItemDirect.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
        if (Iexists)
        {
            var rows = dtItemDirect.Select("pricing_id =" + nPricingId + "");
            foreach (var row in rows)
            {
                row["quantity"] = nQty;
                row["total_direct_price"] = dTotalDirectPrice;
                row["short_notes"] = txtshort_notes1.Text.Replace("'", "''");
            }
        }
        Session.Add("Item_list_Direct", dtItemDirect);


        subtotal = 0.0;
        subtotal_diect = 0.0;
        hdnSortDesc.Value = "0";
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
        hdnPricingId.Value = "0";
        Calculate_Total();

        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");


    }

    protected void btnAddOthers_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddOthers.ID, btnAddOthers.GetType().Name, "Click"); 
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
            lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
            ddlCustomerLocations.Focus();
            return;
        }

        decimal nOtherQty = 1;
        decimal nOtherUnitPrice = 0;

        try
        {
            nOtherQty = Convert.ToDecimal(txtO_Qty.Text.Trim());
        }
        catch (Exception ex)
        {

            nOtherQty = 1;

        }
        if (nOtherQty < 0)
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Negative numbers are not allowed in Code.");
            txtO_Qty.Focus();
            return;
        }
        if (nOtherQty == 0)
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Code should be greater than zero.");
            txtO_Qty.Focus();
            return;
        }
        try
        {
            nOtherUnitPrice = Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", "").Trim());
        }
        catch (Exception ex)
        {

            nOtherUnitPrice = 0;

        }

        decimal Other_TotalPrice = nOtherUnitPrice * nOtherQty;

        sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
        pricing_detail price_detail = new pricing_detail();
      ///  string test = lblParent.Text.Trim().Replace(ddlSections.SelectedItem.Text+" >>", "") + "" + txtOther.Text + ">>Other";
        price_detail.item_name = lblParent.Text.Trim().Replace(ddlSections.SelectedItem.Text+" >>", "") + "" + txtOther.Text + ">>Other";//GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString() + "" + txtOther.Text + ">>Other";
        price_detail.measure_unit = txtO_Unit.Text;
        price_detail.minimum_qty = 1;
        price_detail.retail_multiplier = 0;
        price_detail.labor_rate = 0;
        price_detail.item_cost = nOtherUnitPrice; //Convert.ToDecimal(txtO_Price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        price_detail.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
        price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
        price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
        price_detail.item_id = Convert.ToInt32(hdnOtherId.Value);
        price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
        price_detail.quantity = nOtherQty;//Convert.ToDecimal(txtO_Qty.Text);
        price_detail.labor_id = 1;
        price_detail.is_direct = Convert.ToInt32(ddlO_Direct.SelectedValue);

        if (Convert.ToInt32(ddlO_Direct.SelectedValue) == 1)
        {
            price_detail.total_retail_price = Other_TotalPrice;
            price_detail.total_direct_price = 0;
        }
        else
        {
            price_detail.total_retail_price = 0;
            price_detail.total_direct_price = Other_TotalPrice;
        }
        hdnItemCnt.Value = GetSerial().ToString();
        string strOtherCount = GetSerial_other(Convert.ToInt32(price_detail.item_id)).ToString();
        price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
        price_detail.other_item_cnt = Convert.ToInt32(strOtherCount);
        string str = "0";
        if (Convert.ToInt32(strOtherCount) > 9)
            str = price_detail.item_id + "." + strOtherCount;
        else
            str = price_detail.item_id + ".0" + strOtherCount;
        price_detail.section_serial = Convert.ToDecimal(str);
        price_detail.short_notes = txtO_ShortNotes.Text;
        price_detail.create_date = DateTime.Now;
        price_detail.last_update_date = DateTime.Now;
        price_detail.pricing_type = "A";
        //string strFileName = "";
        //if (Session["FileName"] != null)
        //    strFileName = Session["FileName"].ToString();
        //price_detail.upload_file_path = strFileName;
        price_detail.execution_unit = 0;
        price_detail.sort_id = 0;
        price_detail.is_mandatory = false;
        price_detail.is_CommissionExclude = IsCommissionExclude;

        _db.pricing_details.InsertOnSubmit(price_detail);
        _db.SubmitChanges();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Other Item added to the Estimate, select another item or Location/Section");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Other Item added to Estimate, select another item or Location/Section");
        txtO_Price.Text = "";
        txtO_Qty.Text = "";
        txtO_Unit.Text = "";
        txtOther.Text = "";
        //txtO_TotalPrice.Text = "";
        txtO_ShortNotes.Text = "";
        DataTable dtItem = new DataTable();
        DataTable dtItemDirect = new DataTable();
        if (Session["Item_list_Direct"] != null)
            dtItemDirect = (DataTable)Session["Item_list_Direct"];
        else
            dtItemDirect = LoadItemTable();

        if (Session["Item_list"] != null)
            dtItem = (DataTable)Session["Item_list"];
        else
            dtItem = LoadItemTable();

        DataRow drNew = null;

        // Cost
        decimal dOrginalCost = 0;
        decimal dOrginalTotalCost = 0;
        decimal dLaborTotal = 0;
        decimal dLineItemTotal = 0;

        decimal dTPrice = 0;

        string sItemName = price_detail.item_name.ToString();

        decimal dItemCost = Convert.ToDecimal(price_detail.item_cost);
        decimal dRetail_multiplier = Convert.ToDecimal(price_detail.retail_multiplier);
        decimal dQuantity = Convert.ToDecimal(price_detail.quantity);
        decimal dLabor_rate = Convert.ToDecimal(price_detail.labor_rate);
        if (price_detail.is_direct == 1)
            dTPrice = Convert.ToDecimal(price_detail.total_retail_price);
        else
            dTPrice = Convert.ToDecimal(price_detail.total_direct_price);

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

        if (price_detail.is_direct == 1)
            drNew = dtItem.NewRow();
        else
            drNew = dtItemDirect.NewRow();
        drNew["pricing_id"] = price_detail.pricing_id;
        drNew["item_id"] = price_detail.item_id;
        drNew["labor_id"] = price_detail.labor_id;
        drNew["section_serial"] = price_detail.section_serial;
        drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
        drNew["section_name"] = price_detail.section_name;
        drNew["item_name"] = price_detail.item_name;
        drNew["measure_unit"] = price_detail.measure_unit;
        drNew["item_cost"] = price_detail.item_cost;
        drNew["total_retail_price"] = price_detail.total_retail_price;
        drNew["total_direct_price"] = price_detail.total_direct_price;
        drNew["minimum_qty"] = price_detail.minimum_qty;
        drNew["quantity"] = price_detail.quantity;
        drNew["retail_multiplier"] = price_detail.retail_multiplier;
        drNew["labor_rate"] = price_detail.labor_rate;
        drNew["short_notes"] = price_detail.short_notes;
        drNew["tmpCol"] = "";
        drNew["pricing_type"] = "A";
        drNew["is_mandatory"] = price_detail.is_mandatory;
        drNew["create_date"] = price_detail.create_date;
        drNew["last_update_date"] = price_detail.last_update_date;
        drNew["customer_id"] = price_detail.customer_id;
        drNew["section_level"] = price_detail.section_level;
        drNew["location_id"] = price_detail.location_id;
        drNew["sales_person_id"] = price_detail.sales_person_id;
        drNew["client_id"] = price_detail.client_id;

        drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
        drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
        drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total


        if (price_detail.is_direct == 1)
            dtItem.Rows.Add(drNew);
        else
            dtItemDirect.Rows.Add(drNew);
        Session.Add("Item_list", dtItem);
        Session.Add("Item_list_Direct", dtItemDirect);
        hdnSortDesc.Value = "1";
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        // Session.Remove("FileName");

    }
    protected void btnTemplateEstimate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnTemplateEstimate.ID, btnTemplateEstimate.GetType().Name, "Click"); 
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        try
        {
            string est = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {

                List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                List<pricing_detail> Pd_List = _db.pricing_details.Where(pd => pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A").ToList();
                estimate_payment objes = _db.estimate_payments.SingleOrDefault(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                int nEstId = 0;
                var result = (from mest in _db.model_estimates
                              where mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select mest.model_estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
                nEstId = nEstId + 1;
                hdnModelEstimateId.Value = nEstId.ToString();
                model_estimate me = new model_estimate();
                me.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                me.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                me.status_id = 1;
                me.estimate_comments = "";
                me.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                est = "Estimate Template" + " " + hdnModelEstimateId.Value;
                me.model_estimate_name = est;
                me.create_date = DateTime.Now;
                me.last_udated_date = DateTime.Now;
                me.IsPublic = false;
                _db.model_estimates.InsertOnSubmit(me);
                _db.SubmitChanges();
                foreach (customer_location objcl in Cust_LocList)
                {
                    model_estimate_location mod_loc = new model_estimate_location();
                    mod_loc.client_id = objcl.client_id;
                    mod_loc.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    mod_loc.location_id = objcl.location_id;
                    mod_loc.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    _db.model_estimate_locations.InsertOnSubmit(mod_loc);
                }
                foreach (customer_section objcs in Cust_SecList)
                {
                    model_estimate_section mod_sec = new model_estimate_section();
                    mod_sec.client_id = objcs.client_id;
                    mod_sec.section_id = objcs.section_id;
                    mod_sec.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    mod_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    _db.model_estimate_sections.InsertOnSubmit(mod_sec);
                }
                foreach (pricing_detail objPd in Pd_List)
                {
                    model_estimate_pricing pd = new model_estimate_pricing();
                    pd.client_id = objPd.client_id; ;
                    pd.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    pd.location_id = objPd.location_id; ;
                    pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    pd.section_level = objPd.section_level;
                    pd.item_id = objPd.item_id;
                    pd.section_name = objPd.section_name;
                    pd.item_name = objPd.item_name;
                    pd.measure_unit = objPd.measure_unit;
                    pd.minimum_qty = objPd.minimum_qty;
                    pd.quantity = objPd.quantity;
                    pd.retail_multiplier = objPd.retail_multiplier;
                    pd.labor_id = objPd.labor_id;
                    pd.is_direct = objPd.is_direct;
                    pd.item_cost = objPd.item_cost;
                    pd.total_direct_price = objPd.total_direct_price;
                    pd.total_retail_price = objPd.total_retail_price;
                    pd.labor_rate = objPd.labor_rate;
                    pd.section_serial = objPd.section_serial;
                    pd.item_cnt = objPd.item_cnt;
                    pd.short_notes = objPd.short_notes;
                    pd.pricing_type = objPd.pricing_type;
                    pd.create_date = DateTime.Now;
                    pd.last_updated_date = DateTime.Now;
                    pd.sort_id = objPd.sort_id;
                    _db.model_estimate_pricings.InsertOnSubmit(pd);
                }
                _db.SubmitChanges();
              // Estimate Payment 
                if(objes!=null)
                 {
                     model_estimate_payment mod_es = new model_estimate_payment();
                     mod_es.est_payment_id = objes.est_payment_id;
                     mod_es.client_id = objes.client_id;
                     mod_es.estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                     mod_es.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                     mod_es.project_subtotal = objes.project_subtotal;
                     mod_es.tax_rate = objes.tax_rate;
                     mod_es.tax_amount = objes.tax_amount;
                     mod_es.total_with_tax = objes.total_with_tax;
                     mod_es.adjusted_price = objes.adjusted_price;
                     mod_es.adjusted_tax_amount = objes.adjusted_tax_amount;
                     mod_es.adjusted_tax_rate = objes.adjusted_tax_rate;
                     mod_es.new_total_with_tax = objes.new_total_with_tax;
                     mod_es.is_incentives = objes.is_incentives;
                     mod_es.total_incentives = objes.total_incentives;
                     mod_es.incentive_ids = objes.incentive_ids;
                     mod_es.deposit_percent = objes.deposit_percent;
                     mod_es.deposit_amount = objes.deposit_amount;
                     mod_es.countertop_percent = objes.countertop_percent;
                     mod_es.countertop_amount = objes.countertop_amount;
                     mod_es.start_job_percent = objes.start_job_percent;
                     mod_es.start_job_amount = objes.start_job_amount;
                     mod_es.due_completion_percent = objes.due_completion_percent;
                     mod_es.due_completion_amount = objes.due_completion_amount;
                     mod_es.final_measure_percent = objes.final_measure_percent;
                     mod_es.final_measure_amount = objes.final_measure_amount;
                     mod_es.deliver_caninet_percent = objes.deliver_caninet_percent;
                     mod_es.deliver_cabinet_amount = objes.deliver_cabinet_amount;
                     mod_es.substantial_percent = objes.substantial_percent;
                     mod_es.substantial_amount = objes.substantial_amount;
                     mod_es.other_value = objes.other_value;
                     mod_es.other_percent = objes.other_percent;
                     mod_es.other_amount = objes.other_amount;
                     mod_es.based_on_percent = objes.based_on_percent;
                     mod_es.based_on_dollar = objes.based_on_dollar;
                     mod_es.create_date = objes.create_date;
                     mod_es.updated_date = objes.updated_date;

                     mod_es.deposit_date = objes.deposit_date;
                     mod_es.countertop_date = objes.countertop_date;
                     mod_es.startof_job_date = objes.startof_job_date;
                     mod_es.due_completion_date = objes.due_completion_date;
                     mod_es.measure_date = objes.measure_date;
                     mod_es.delivery_date = objes.deposit_date;
                     mod_es.substantial_date = objes.substantial_date;
                     mod_es.other_date = objes.other_date;
                     mod_es.lead_time = objes.lead_time;
                     mod_es.contract_date = objes.contract_date;
                     mod_es.deposit_value = objes.deposit_value;
                     mod_es.countertop_value = objes.countertop_value;
                     mod_es.start_job_value = objes.start_job_value;
                     mod_es.due_completion_value = objes.due_completion_value;
                     mod_es.final_measure_value = objes.final_measure_value;
                     mod_es.deliver_caninet_value = objes.deliver_caninet_value;
                     mod_es.substantial_value = objes.substantial_value;
                     mod_es.special_note = objes.special_note;
                     mod_es.start_date = objes.start_date;
                     mod_es.completion_date = objes.completion_date;
                     mod_es.is_KithenSheet = objes.is_KithenSheet;
                     mod_es.is_BathSheet = objes.is_BathSheet;
                     mod_es.is_ShowerSheet = objes.is_ShowerSheet;
                     mod_es.is_TubSheet = objes.is_TubSheet;
                     mod_es.drywall_percent = objes.drywall_percent;
                     mod_es.drywall_amount = objes.drywall_amount;
                     mod_es.drywall_date = objes.drywall_date;
                     mod_es.drywall_value = objes.drywall_value;
                     mod_es.flooring_percent = objes.flooring_percent;
                     mod_es.flooring_amount = objes.flooring_amount;
                     mod_es.flooring_date = objes.flooring_date;
                     mod_es.flooring_value = objes.flooring_value;
                     _db.model_estimate_payments.InsertOnSubmit(mod_es);
                     _db.SubmitChanges();
                }

                

            }

            Session.Add("sSytemMassage", "Estimate has been saved as a Template (" + est + ") successfully.");
            Response.Redirect("pricing.aspx?eid=" + Convert.ToInt32(hdnEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    //protected void ItemPrice_calculation(object sender, EventArgs e)
    //{

    //    string strSender = sender.ToString();
    //    int i = 0;
    //    if (strSender.IndexOf("TextBox") != -1)
    //    {
    //        TextBox txtQty1 = (TextBox)grdItemPrice.FindControl("txtQty");
    //        txtQty1 = (TextBox)sender;
    //        GridViewRow gvr = (GridViewRow)txtQty1.NamingContainer;
    //        i = gvr.RowIndex;
    //    }

    //    DropDownList ddlLabor = (DropDownList)grdItemPrice.Rows[i].FindControl("ddlLabor");
    //    //Label lblTotalPrice = (Label)grdItemPrice.Rows[i].FindControl("lblTotalPrice");
    //    TextBox txtQty = (TextBox)grdItemPrice.Rows[i].FindControl("txtQty");
    //    DropDownList ddlDirect = (DropDownList)grdItemPrice.Rows[i].FindControl("ddlDirect");
    //    TextBox txtShortNote = (TextBox)grdItemPrice.Rows[i].FindControl("txtShortNote");
    //    bool IsMandatory = Convert.ToBoolean(grdItemPrice.DataKeys[i].Values[3]);
    //    decimal nTotalPrice = 0;
    //    if (txtQty.Text.Trim() == "")
    //    {
    //        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
    //        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
    //        return;
    //    }
    //    else
    //    {
    //        try
    //        {
    //            Convert.ToDecimal(txtQty.Text.Trim());
    //        }
    //        catch (Exception ex)
    //        {
    //            lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
    //            lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
    //            return;
    //        }
    //    }
    //    if (IsMandatory == false)
    //    {

    //        if (Convert.ToDecimal(grdItemPrice.Rows[i].Cells[10].Text) > Convert.ToDecimal(txtQty.Text))
    //        {
    //            lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
    //            lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
    //            return;
    //        }
    //        if (Convert.ToDecimal(grdItemPrice.Rows[i].Cells[10].Text) <= Convert.ToDecimal(txtQty.Text))
    //        {
    //            decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
    //            decimal nQty = Convert.ToDecimal(txtQty.Text);
    //            decimal dlabor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[1].ToString());
    //            decimal dretail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[2].ToString());
    //            decimal nLaborRate = dlabor_rate * dretail_multiplier;
    //            nLaborRate = nLaborRate * nQty;

    //            if (ddlLabor.SelectedValue == "2")
    //                nTotalPrice = nCost * nQty + nLaborRate;
    //            else
    //                nTotalPrice = nCost * nQty;
    //            //lblTotalPrice.Text = nTotalPrice.ToString("c");
    //        }
    //    }
    //    else
    //    {
    //        decimal nQty = Convert.ToDecimal(txtQty.Text);
    //        decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
    //        if (nQty > 0)
    //        {
    //            decimal dlabor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[1].ToString());
    //            decimal dretail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[2].ToString());
    //            decimal nLaborRate = dlabor_rate * dretail_multiplier;
    //            nLaborRate = nLaborRate * nQty;

    //            if (ddlLabor.SelectedValue == "2")
    //                nTotalPrice = nCost * nQty + nLaborRate;
    //            else
    //                nTotalPrice = nCost * nQty;
    //           // lblTotalPrice.Text = nTotalPrice.ToString("c");
    //        }
    //        else
    //        {
    //            nQty = Convert.ToDecimal(0.01);
    //            nCost = Convert.ToDecimal(0);
    //            nTotalPrice = nCost * nQty;
    //           // lblTotalPrice.Text = nTotalPrice.ToString("c");

    //        }

    //    }

    //    //Data Save


    //    string strPriceId = "0";
    //    decimal ditem_cost = 0;
    //    string smeasure_unit = "";
    //    decimal dminimum_qty = 0;
    //    decimal labor_rate = 0;
    //    decimal retail_multiplier = 0;

    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    hdnSectionId.Value = grdItemPrice.Rows[i].Cells[1].Text;

    //    strPriceId = grdItemPrice.DataKeys[i].Values[0].ToString();


    //    labor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[1].ToString());
    //    retail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[2].ToString());
    //    smeasure_unit = grdItemPrice.Rows[i].Cells[7].Text;
    //    ditem_cost = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[8].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
    //    dminimum_qty = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[10].Text);

    //    if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
    //    {
    //        lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
    //        lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
    //        ddlCustomerLocations.Focus();
    //        return;
    //    }
    //    if (IsMandatory == false)
    //    {
    //        if (!IsMandatoryItemCheck())
    //        {
    //            ModalPopupExtender2.Show();
    //            return;
    //        }
    //    }

    //    sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
    //    hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
    //    hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
    //    hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
    //    hdnSectionId.Value = strPriceId;
    //    bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
    //    pricing_detail price_detail = new pricing_detail();
    //    price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString();
    //    price_detail.measure_unit = smeasure_unit;
    //    price_detail.minimum_qty = dminimum_qty;
    //    price_detail.retail_multiplier = retail_multiplier;
    //    price_detail.labor_rate = labor_rate;
    //    price_detail.item_cost = ditem_cost;

    //    price_detail.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
    //    price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
    //    price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
    //    //co_price_detail.change_order_id = Convert.ToInt32(hdnChEstId.Value);
    //    price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
    //    price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
    //    price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
    //    price_detail.item_id = Convert.ToInt32(hdnSectionId.Value);
    //    price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
    //    if (IsMandatory)
    //    {
    //        if (Convert.ToDecimal(txtQty.Text) > 0)
    //        {
    //            price_detail.quantity = Convert.ToDecimal(txtQty.Text);
    //        }
    //        else
    //        {
    //            price_detail.quantity = Convert.ToDecimal(0.01);
    //        }
    //    }
    //    else
    //    {
    //        price_detail.quantity = Convert.ToDecimal(txtQty.Text);
    //    }
    //    price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
    //    price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

    //    if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
    //    {
    //        price_detail.total_retail_price = nTotalPrice; //Convert.ToDecimal(lblTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
    //        price_detail.total_direct_price = 0;
    //    }
    //    else
    //    {
    //        price_detail.total_retail_price = 0;
    //        price_detail.total_direct_price = nTotalPrice;//Convert.ToDecimal(lblTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
    //    }

    //    if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
    //        price_detail.labor_rate = 0;
    //    price_detail.section_serial = Convert.ToDecimal(hdnSectionSerial.Value);
    //    hdnItemCnt.Value = GetSerial().ToString();
    //    price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
    //    price_detail.pricing_type = "A";
    //    price_detail.short_notes = txtShortNote.Text;
    //    price_detail.create_date = DateTime.Now;
    //    price_detail.last_update_date = DateTime.Now;
    //    price_detail.execution_unit = 0;
    //    price_detail.sort_id = 0;
    //    price_detail.is_mandatory = IsMandatory;
    //    price_detail.is_CommissionExclude = IsCommissionExclude;

    //    _db.pricing_details.InsertOnSubmit(price_detail);

    //    _db.SubmitChanges();
    //    lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to estimate list, select another item or Location/Section");
    //    lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to estimate list, select another item or Location/Section");
    //    hdnSortDesc.Value = "1";
    //    BindSelectedItemGrid();
    //    BindSelectedItemGrid_Direct();
    //    Calculate_Total();
    //    //Item updated successfully to estimate list, select another item or Location/Section

    //    if (grdGrouping.Rows.Count == 0)
    //    {
    //        lblRetailPricingHeader.Visible = false;
    //    }
    //    else
    //    {
    //        lblRetailPricingHeader.Visible = true;
    //    }
    //    if (grdGroupingDirect.Rows.Count == 0)
    //    {
    //        lblDirectPricingHeader.Visible = false;
    //    }
    //    else
    //    {
    //        lblDirectPricingHeader.Visible = true;
    //    }


    //}

    protected void NonDirect_calculation(object sender, EventArgs e)
    {
        GridView grvFind = null;
        TextBox txt = (TextBox)sender;
        int nIndex = -1;
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            int i = -1;
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow diitem in grdSelectedItem1.Rows)
            {
                i++;
                TextBox txtquantity = (TextBox)diitem.FindControl("txtquantity");
                Label lblquantity = (Label)diitem.FindControl("lblquantity");
                Label lblTotal_price = (Label)diitem.FindControl("lblTotal_price");

                Label lblUnit_Cost = (Label)diitem.FindControl("lblUnit_Cost");
                Label lblTotalLabor_Cost = (Label)diitem.FindControl("lblTotalLabor_Cost");
                Label lblTotal_Cost = (Label)diitem.FindControl("lblTotal_Cost");


                if (txtquantity.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Code.");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Code.");
                    return;
                }
                else
                {
                    try
                    {
                        Convert.ToDecimal(txtquantity.Text.Trim());
                    }
                    catch (Exception ex)
                    {
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                        return;
                    }
                }
                if (txtquantity == txt)
                {
                    nIndex = i;
                    grvFind = grdSelectedItem1;
                    DataClassesDataContext _db = new DataClassesDataContext();
                    hdnPricingId.Value = grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0].ToString();
                    int LaborId = 1;
                    decimal nCost1 = 0;
                    if (_db.pricing_details.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.pricing_id == Convert.ToInt32(hdnPricingId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
                    {
                        pricing_detail pd = _db.pricing_details.Single(p => p.item_id == Convert.ToInt32(diitem.Cells[1].Text) && p.pricing_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        LaborId = Convert.ToInt32(pd.labor_id);
                        nCost1 = Convert.ToDecimal(pd.item_cost);
                    }
                    //decimal nCost1 = Convert.ToDecimal(diitem.Cells[5].Text.Replace("$", ""));
                    decimal nQty1 = Convert.ToDecimal(txtquantity.Text);
                    decimal nLaborRate = 0;
                    decimal nTotalPrice1 = 0;

                    if (LaborId == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem.Cells[1].Text) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        //nLaborRate = Convert.ToDecimal(itm.labor_rate);
                        nLaborRate = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate = nLaborRate * nQty1;
                    }

                    decimal nUCost = Convert.ToDecimal(lblUnit_Cost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    decimal nTotalUcost = (nUCost * nQty1) + nLaborRate;
                    nTotalPrice1 = nCost1 * nQty1 + nLaborRate;
                    lblTotal_price.Text = nTotalPrice1.ToString("c");

                    lblTotalLabor_Cost.Text = nLaborRate.ToString("c");
                    lblTotal_Cost.Text = nTotalUcost.ToString("c");
                }

                if (nIndex > -1)
                    break;
            }
            if (nIndex > -1)
                break;
        }
        GridRowUpdatingNonDirect(grvFind, nIndex);
    }

    protected void Direct_calculation(object sender, EventArgs e)
    {
        GridView grvFind = null;
        TextBox txt = (TextBox)sender;
        int nIndex = -1;
        foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
        {
            int i = -1;
            GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
            foreach (GridViewRow diitem2 in grdSelectedItem2.Rows)
            {
                i++;
                TextBox txtquantity1 = (TextBox)diitem2.FindControl("txtquantity1");
                Label lblquantity1 = (Label)diitem2.FindControl("lblquantity1");
                Label lblTotal_price2 = (Label)diitem2.FindControl("lblTotal_price2");

                Label lblUnit_Cost3 = (Label)diitem2.FindControl("lblUnit_Cost3");
                Label lblTotalLabor_Cost3 = (Label)diitem2.FindControl("lblTotalLabor_Cost3");
                Label lblTotal_Cost3 = (Label)diitem2.FindControl("lblTotal_Cost3");

                if (txtquantity1.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Code.");
                    return;
                }
                else
                {
                    try
                    {
                        Convert.ToDecimal(txtquantity1.Text.Trim());
                    }
                    catch (Exception ex)
                    {
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  Code.");
                        return;
                    }
                }

                if (txtquantity1 == txt)
                {
                    nIndex = i;
                    grvFind = grdSelectedItem2;
                    DataClassesDataContext _db = new DataClassesDataContext();
                    hdnPricingId.Value = grdSelectedItem2.DataKeys[diitem2.RowIndex].Values[0].ToString();
                    int LaborId2 = 1;
                    decimal nLaborRate2 = 0;
                    //decimal nCost2 = Convert.ToDecimal(diitem2.Cells[5].Text.Replace("$", ""));
                    decimal nCost2 = 0;
                    decimal nQty2 = Convert.ToDecimal(txtquantity1.Text);
                    decimal nTotalPrice2 = 0;
                    if (_db.pricing_details.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.pricing_id == Convert.ToInt32(hdnPricingId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
                    {
                        pricing_detail pd = _db.pricing_details.Single(p => p.item_id == Convert.ToInt32(diitem2.Cells[1].Text) && p.pricing_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        LaborId2 = Convert.ToInt32(pd.labor_id);
                        nCost2 = Convert.ToDecimal(pd.item_cost);
                    }
                    if (LaborId2 == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem2.Cells[1].Text) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        //nLaborRate2 = Convert.ToDecimal(itm.labor_rate);
                        nLaborRate2 = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate2 = nLaborRate2 * nQty2;
                    }
                    decimal nUCost = Convert.ToDecimal(lblUnit_Cost3.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    decimal nTotalUcost = (nUCost * nQty2) + nLaborRate2;

                    nTotalPrice2 = nCost2 * nQty2 + nLaborRate2;
                    lblTotal_price2.Text = nTotalPrice2.ToString("c");

                    lblTotalLabor_Cost3.Text = nLaborRate2.ToString("c");
                    lblTotal_Cost3.Text = nTotalUcost.ToString("c");
                }

                if (nIndex > -1)
                    break;
            }
            if (nIndex > -1)
                break;
        }
        GridRowUpdatingDirect(grvFind, nIndex);
    }

    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_schedule.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    //protected void btnCalculate_Click(object sender, EventArgs e)
    //{
    //    decimal nGrandtotal = Convert.ToDecimal(lblGrandTotalCost.Text.ToString().Trim().Replace("$", ""));
    //    decimal nTaxRate = 0;
    //    if (txtTaxPer.Text.Trim() == "")
    //        nTaxRate = 0;
    //    else
    //        nTaxRate = Convert.ToDecimal(txtTaxPer.Text.Trim());
    //    decimal nTax = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);
    //    //lblTax.Text = nTax.ToString("c");
    //    lblTotalWithTax.Text = Convert.ToDecimal(nGrandtotal + nTax).ToString("c");
    //}
    protected void btnChangeLoc_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnChangeLoc.ID, btnChangeLoc.GetType().Name, "Click"); 
        lblMessLoc.Text = "";
        if (ddlNewLocations.SelectedItem.Text == "Select New Location")
        {
            lblMessLoc.Text = csCommonUtility.GetSystemErrorMessage("Please select new location for change existing loaction");

            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string Ids = string.Empty;
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow diitem in grdSelectedItem1.Rows)
            {
                CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingle");
                if (chkSingle.Checked)
                {
                    int id = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);
                    Ids += id + ",";
                }
            }
        }
        foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
        {
            GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
            foreach (GridViewRow diitem1 in grdSelectedItem2.Rows)
            {
                CheckBox chkSingleD = (CheckBox)diitem1.FindControl("chkSingleD");
                if (chkSingleD.Checked)
                {
                    int id = Convert.ToInt32(grdSelectedItem2.DataKeys[diitem1.RowIndex].Values[0]);
                    Ids += id + ",";
                }
            }
        }
        Ids = Ids.Trim().TrimEnd(',');
        if (Ids != "")
        {

            string strUpdate = " UPDATE pricing_details SET location_id =" + Convert.ToInt32(ddlNewLocations.SelectedValue) + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_id IN (" + Ids + ")";
            _db.ExecuteCommand(strUpdate, string.Empty);
            string Strq = " UPDATE pricing_details SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + Convert.ToInt32(ddlNewLocations.SelectedValue);
            _db.ExecuteCommand(Strq, string.Empty);
            lblResult1.Text = csCommonUtility.GetSystemMessage("Existing location changed successfully to " + ddlNewLocations.SelectedItem.Text + "");
            hdnSortDesc.Value = "0";
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();

        }
    }
    protected void btnCloseLoc_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCloseLoc.ID, btnCloseLoc.GetType().Name, "Click"); 
        lblMessage.Text = "";
        txtNewEstimateName.Text = "";
        modUpdateEstimate.Hide();
    }
    protected void grdGrouping_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Move")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                int serial = dimaster1.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGrouping.DataKeys[dimaster1.RowIndex].Values[0]);
                string strUpdate = " UPDATE pricing_details SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=1";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            string Strq = " UPDATE pricing_details SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId + " AND is_direct=1";
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
        }
        else if (e.CommandName == "sEmail")
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
            // string sSubject = "(" + hdnLastName.Value + ")Request for Bid -" + secName;
            string sSubject = "Request for Bid - " + secName + " (" + hdnLastName.Value + ")";
            hdnSecName.Value = secName;
            string sFileName = CreateSectionReportForEMail(nSecId);
            Session.Add("sSubject", sSubject);

            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));

            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }
        else if (e.CommandName == "Del")
        {
            string Ids = string.Empty;
            // int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                foreach (GridViewRow diitem in grdSelectedItem1.Rows)
                {
                    CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingle");
                    if (chkSingle.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("DeleteID", Ids);

                DataTable dtItem = new DataTable();

                if (Session["Item_list"] != null)
                    dtItem = (DataTable)Session["Item_list"];
                else
                    dtItem = LoadItemTable();

                DataTable dtDeleteList = LoadItemTable();
                DataView dv = dtItem.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtDeleteList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];

                    dtDeleteList.Rows.Add(drNew);

                }
                Session.Add("DeleteList", dtDeleteList);
                BindDeletedGrid();

                ModalPopupExtender4.Show();
                return;
            }
        }

        else if (e.CommandName == "EditItem")
        {
            lblMSG.Text = string.Empty;
            if (chkPMDisplay.Checked)
            {


                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    if (objUser.IsPriceChange == true || objUser.role_id == 1)
                    {
                        rdbmulExprice.Visible = true;
                    }
                    else
                    {
                        rdbmulExprice.Visible = false;
                    }
                }
            }
            else
            {
                rdbmulExprice.Visible = false;
            }
            string Ids = string.Empty;
            // int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                foreach (GridViewRow diitem in grdSelectedItem1.Rows)
                {
                    CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingle");
                    if (chkSingle.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("EditID", Ids);

                DataTable dtItem = new DataTable();

                if (Session["Item_list"] != null)
                    dtItem = (DataTable)Session["Item_list"];
                else
                    dtItem = LoadItemTable();

                DataTable dtEditList = LoadItemTable();
                DataView dv = dtItem.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtEditList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];
                    drNew["unit_cost"] = dv[i]["unit_cost"];
                    drNew["total_unit_cost"] = dv[i]["total_unit_cost"];
                    drNew["total_labor_cost"] = dv[i]["total_labor_cost"];

                    dtEditList.Rows.Add(drNew);

                }
                Session.Add("ItemEditList", dtEditList);
                BindEditGrid();

                ModalPopupExtender5.Show();
                return;
            }
        }

        else if (e.CommandName == "sEmail")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            int ncid = Convert.ToInt32(hdnCustomerId.Value);
            int nEstId = Convert.ToInt32(hdnEstimateId.Value);


            string secName = "";

            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;

            string sSubject = "Request for Bid - " + secName + " (" + hdnLastName.Value + ")";
            hdnSecName.Value = secName;
            string sFileName = CreateSectionReportForEMail(nSecId);
            Session.Add("sSubject", sSubject);


            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, ncid, secName));

            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }

        else if (e.CommandName == "CopyItem")
        {
            lblMSG.Text = string.Empty;
            lblLocationCopy.Text = string.Empty;
            lblResult1.Text = string.Empty;
            lblAdd.Text = string.Empty;
            string Ids = string.Empty;

            // int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                foreach (GridViewRow diitem in grdSelectedItem1.Rows)
                {
                    CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingle");
                    if (chkSingle.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("CopyItemID", Ids);

                DataTable dtItem = new DataTable();

                if (Session["Item_list"] != null)
                    dtItem = (DataTable)Session["Item_list"];
                else
                    dtItem = LoadItemTable();

                DataTable dtCopyList = LoadItemTable();
                DataView dv = dtItem.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtCopyList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];
                    drNew["unit_cost"] = dv[i]["unit_cost"];
                    drNew["total_unit_cost"] = dv[i]["total_unit_cost"];
                    drNew["total_labor_cost"] = dv[i]["total_labor_cost"];

                    dtCopyList.Rows.Add(drNew);

                }
                Session.Add("ItemCopyList", dtCopyList);
                BindCopyItemGrid();

                modCopyItems.Show();
                return;
            }
        }
    }

    public void BindDeletedGrid()
    {
        DataTable dtDelete = new DataTable();

        if (Session["DeleteList"] != null)
        {
            dtDelete = (DataTable)Session["DeleteList"];
        }
        if (dtDelete.Rows.Count > 0)
        {
            grdDeletedItem.DataSource = dtDelete;
            grdDeletedItem.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grdDeletedItem.DataBind();
        }
        else
        {
            grdDeletedItem.DataSource = dtDelete;
            grdDeletedItem.DataBind();

        }

        if (grdDeletedItem.Rows.Count == 0)
        {
            grdDeletedItem.Visible = false;
        }
        else
        {
            grdDeletedItem.Visible = true;
        }
    }

    public void BindEditGrid()
    {
        DataTable dtEdit = new DataTable();

        if (Session["ItemEditList"] != null)
        {
            dtEdit = (DataTable)Session["ItemEditList"];
        }
        if (dtEdit.Rows.Count > 0)
        {
            grdEditItem.DataSource = dtEdit;
            grdEditItem.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grdEditItem.DataBind();
        }
        else
        {
            grdEditItem.DataSource = dtEdit;
            grdEditItem.DataBind();

        }

        if (grdEditItem.Rows.Count == 0)
        {
            grdEditItem.Visible = false;
        }
        else
        {
            grdEditItem.Visible = true;
        }
    }

    protected void btnDeleteItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDeleteItem.ID, btnDeleteItem.GetType().Name, "Click"); 

        string strID = string.Empty;
        if (Session["DeleteID"] == null)
        {
            return;
        }
        else
        {
            strID = Session["DeleteID"].ToString();
            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = "Delete pricing_details WHERE pricing_id IN (" + strID + ") AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ, string.Empty);

            hdnSortDesc.Value = "0";
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
            Calculate_Total();
            hdnPricingId.Value = "0";
            Session.Remove("DeleteList");
            Session.Remove("DeleteID");
            BindDeletedGrid();
            BindGrid();
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item(s) deleted successfully");
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item(s) deleted successfully");
        }

    }

    protected void btnSaveItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveItem.ID, btnSaveItem.GetType().Name, "Click"); 
        string strID = string.Empty;
        if (Session["EditID"] == null)
        {
            return;
        }
        else
        {
            strID = Session["EditID"].ToString();
            DataClassesDataContext _db = new DataClassesDataContext();

            lblSelectLocation.Text = "";
            lblAdd.Text = "";
            lblMessage.Text = "";
            lblResult1.Text = "";

            foreach (GridViewRow gvr in grdEditItem.Rows)
            {
                decimal nLaborRate = 0;
                decimal nTotalPrice = 0;
                //int directId = 1;
                decimal nUnitCost = 0;
                decimal nExtTotalCost = 0;
                decimal nTotallaborRate = 0;
                decimal nRetailMultiplier = 0;
                decimal nTotalRetailPrice = 0;
             

                decimal nItemCost = 0;
                decimal nQty = 0;
                TextBox txtquantity = (TextBox)gvr.FindControl("txtquantity");
                Label lblTotal_price = (Label)gvr.FindControl("lblTotal_price");
                Label lblitem_name = (Label)gvr.FindControl("lblitem_name");
                TextBox txtshort_notes = (TextBox)gvr.FindControl("txtshort_notes");
                int nPricingId = Convert.ToInt32(grdEditItem.DataKeys[gvr.RowIndex].Values[0]);
                int nItemId = Convert.ToInt32(grdEditItem.DataKeys[gvr.RowIndex].Values[1]);

                //Shohidullah 
                TextBox txtUnit_Cost = (TextBox)gvr.FindControl("txtUnit_Cost");
                TextBox txtMultiplier = (TextBox)gvr.FindControl("txtretail_multiplier");
                TextBox txtTotalLabor_Rate = (TextBox)gvr.FindControl("txtTotalLabor_Rate");
                Label lblTotalLabor_Rate = (Label)gvr.FindControl("lblTotalLabor_Rate");
                Label lblTotalLabor_Cost = (Label)gvr.FindControl("lblTotalLabor_Cost");
                TextBox txtTotalPrice = (TextBox)gvr.FindControl("txtTotalPrice");
                DropDownList ddlDirect = (DropDownList)gvr.FindControl("ddlDirect");
                try
                {
                    nUnitCost = Convert.ToDecimal(txtUnit_Cost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nUnitCost = 0;
                }

                try
                {
                    nTotallaborRate = Convert.ToDecimal(txtTotalLabor_Rate.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nTotallaborRate = 0;
                }

                try
                {
                    nRetailMultiplier = Convert.ToDecimal(txtMultiplier.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nRetailMultiplier = 0;
                }
                try
                {
                    nQty = Convert.ToDecimal(txtquantity.Text);
                }
                catch
                {
                    nQty = 0;
                }
                try
                {
                    nTotalRetailPrice = Convert.ToDecimal(txtTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nTotalRetailPrice = 0;
                }
                try
                {

                    if (rdbmulExprice.SelectedValue == "0")
                    {

                        if (lblitem_name.Text.IndexOf("Other") > 0)
                        {

                            if (nQty > 0)
                            {
                                nTotalPrice = Math.Round((nUnitCost * nQty * 2), 2);
                                nItemCost = nUnitCost;
                            }
                            nLaborRate = 0;
                            nRetailMultiplier = 0;

                        }
                        else
                        {
                            if (nRetailMultiplier > 0)
                            {
                                nItemCost = nUnitCost * (decimal)nRetailMultiplier;
                                nLaborRate = nTotallaborRate;// (decimal)itm.labor_rate * (decimal)itm.retail_multiplier; 
                                // nItemCost = nCost1;
                                nTotalPrice = Math.Round(((nUnitCost + nTotallaborRate) * (decimal)nRetailMultiplier * (decimal)nQty), 2);
                                lblTotal_price.Text = nTotalPrice.ToString("c");
                            }
                            else
                            {
                                nItemCost = nUnitCost;
                                nLaborRate = nTotallaborRate;// (decimal)itm.labor_rate * (decimal)itm.retail_multiplier; 
                                // nItemCost = nCost1;
                                nTotalPrice = Math.Round(((nUnitCost + nTotallaborRate) * (decimal)nQty), 2);
                                lblTotal_price.Text = nTotalPrice.ToString("c");
                            }
                        }
                    }
                    else
                    {

                        if (lblitem_name.Text.IndexOf("Other") > 0)
                        {

                            if (nQty > 0)
                            {
                                // nTotalPrice = nUnitCost * nQty * 2;
                                //nItemCost = nUnitCost;
                                nUnitCost = nTotalRetailPrice / nQty / 2;
                                nItemCost = nUnitCost;
                                nTotalPrice = nTotalRetailPrice;
                              
                            }
                            nLaborRate = 0;
                            nRetailMultiplier = 0;

                        }
                        else
                        {
                            nExtTotalCost = Math.Round(((nUnitCost + nTotallaborRate) * nQty), 2);//(nTotalRetailPrice - (nUnitCost +nTotallaborRate)*nQty);
                            nRetailMultiplier = Math.Round((nTotalRetailPrice / nExtTotalCost), 6);
                            nItemCost = nUnitCost * nRetailMultiplier;
                            nLaborRate = nTotallaborRate;
                            nTotalPrice = Math.Round((nExtTotalCost * (decimal)nRetailMultiplier), 2);
                            lblTotal_price.Text = nTotalPrice.ToString("c");

                         }

                    }

                }
                catch
                {
                    nTotalPrice = 0;
                }


                if (Convert.ToInt32(ddlDirect.SelectedValue) == 1 || Convert.ToInt32(ddlDirect.SelectedValue) == 2)
                {
                    pricing_detail pd = _db.pricing_details.Single(p => p.item_id == nItemId && p.pricing_id == nPricingId && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                    pd.labor_rate = nLaborRate;
                    pd.retail_multiplier = nRetailMultiplier;
                    pd.item_cost = nItemCost;
                    pd.short_notes = txtshort_notes.Text.Replace("'", "''");
                    pd.quantity = nQty;
                    pd.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);
                  

                    if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
                    {
                        pd.total_retail_price = nTotalPrice;
                        pd.total_direct_price = 0;
                    }
                    else
                    {
                        pd.total_retail_price = 0;
                        pd.total_direct_price = nTotalPrice;
                    }
                    _db.SubmitChanges();

                    DataTable dtItem = (DataTable)Session["Item_list"];

                    bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
                    if (Iexists)
                    {
                        var rows = dtItem.Select("pricing_id =" + nPricingId + "");
                        foreach (var row in rows)
                        {
                            // Cost
                            decimal dOrginalCost = 0;
                            decimal dOrginalTotalCost = 0;
                            decimal dLaborTotal = 0;
                            decimal dLineItemTotal = 0;




                            string sItemName = row["item_name"].ToString();


                            if (sItemName.IndexOf("Other") > 0)
                            {

                                if (nQty > 0)
                                {
                                    dOrginalCost = nUnitCost;
                                }
                                nLaborRate = 0;
                                nRetailMultiplier = 0;

                            }
                            else
                            {
                                if (nRetailMultiplier > 0)
                                {

                                    dOrginalCost = (nItemCost / nRetailMultiplier);
                                }
                                else
                                {
                                    dOrginalCost = nItemCost;
                                }
                            }

                            dOrginalTotalCost = dOrginalCost * nQty;
                            dLaborTotal = nLaborRate * nQty;
                            dLineItemTotal = (dOrginalCost + nLaborRate) * nQty;

                            row["quantity"] = nQty;
                            row["total_retail_price"] = nTotalPrice;
                            row["short_notes"] = txtshort_notes.Text.Replace("'", "''");

                            row["unit_cost"] = dOrginalCost;
                            row["retail_multiplier"] = nRetailMultiplier;

                            // Orginal Unit Cost
                            row["labor_rate"] = nLaborRate;
                            row["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                            row["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                        }
                    }
                    Session.Add("Item_list", dtItem);
                }
                else
                {
                    string strQ = "UPDATE pricing_details SET quantity=" + nQty + " ,total_direct_price=" + nTotalPrice + " ,short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    _db.ExecuteCommand(strQ, string.Empty);
                    DataTable dtItemDiret = (DataTable)Session["Item_list_Direct"];

                    bool Iexists = dtItemDiret.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
                    if (Iexists)
                    {
                        var rows = dtItemDiret.Select("pricing_id =" + nPricingId + "");
                        foreach (var row in rows)
                        {
                            // Cost
                            decimal dOrginalCost = 0;
                            decimal dOrginalTotalCost = 0;
                            decimal dLaborTotal = 0;
                            decimal dLineItemTotal = 0;


                            string sItemName = row["item_name"].ToString();

                            decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                            decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                            decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                            decimal dTPrice = Convert.ToDecimal(row["total_direct_price"]);

                            if (sItemName.IndexOf("Other") > 0)
                            {
                                if (nQty > 0)
                                {
                                    dOrginalCost = (dTPrice / nQty) / 2;
                                    dLabor_rate = 0;
                                }
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
                            dOrginalTotalCost = dOrginalCost * nQty;
                            dLaborTotal = dLabor_rate * nQty;
                            dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                            row["quantity"] = nQty;
                            row["total_direct_price"] = nTotalPrice;
                            row["short_notes"] = txtshort_notes.Text.Replace("'", "''");
                            row["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                            row["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                            row["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                        }
                    }
                    Session.Add("Item_list_Direct", dtItemDiret);

                }


            }



            subtotal = 0.0;
            subtotal_diect = 0.0;
            hdnSortDesc.Value = "0";
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
            hdnPricingId.Value = "0";
            Calculate_Total();
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");



        }

    }

    protected void grdGroupingDirect_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Move1")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                int serial = dimaster2.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGroupingDirect.DataKeys[dimaster2.RowIndex].Values[0]);
                string strUpdate = " UPDATE pricing_details SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=2";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(grdGroupingDirect.DataKeys[index].Values[0]);
            string Strq = " UPDATE pricing_details SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId + " AND is_direct=2";
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid_Direct();
        }
        else if (e.CommandName == "sEmail2")
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGroupingDirect.DataKeys[index].Values[0]);
            string sFileName = CreateSectionReportForEMail(nSecId);
            string secName = "";
            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;
            string sSubject = "(" + hdnLastName.Value + ")Request for Bid -" + secName;
            Session.Add("sSubject", sSubject);

            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }
        else if (e.CommandName == "Del1")
        {
            string Ids = string.Empty;
            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
                foreach (GridViewRow diitem2 in grdSelectedItem2.Rows)
                {
                    CheckBox chkSingleD = (CheckBox)diitem2.FindControl("chkSingleD");
                    if (chkSingleD.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem2.DataKeys[diitem2.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }

            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("DeleteID", Ids);
                DataTable dtItemDirect = new DataTable();

                if (Session["Item_list_Direct"] != null)
                    dtItemDirect = (DataTable)Session["Item_list_Direct"];
                else
                    dtItemDirect = LoadItemTable();

                DataTable dtDeleteList = LoadItemTable();
                DataView dv = dtItemDirect.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtDeleteList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];

                    dtDeleteList.Rows.Add(drNew);

                }
                Session.Add("DeleteList", dtDeleteList);
                BindDeletedGrid();

                ModalPopupExtender4.Show();
                return;
            }
        }

        else if (e.CommandName == "EditItem1")
        {
            lblMSG.Text = string.Empty;
            if (chkPMDisplay.Checked)
            {


                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    if (objUser.IsPriceChange == true || objUser.role_id == 1)
                    {
                        rdbmulExprice.Visible = true;
                    }
                    else
                    {
                        rdbmulExprice.Visible = false;
                    }
                }
            }
            else
            {
                rdbmulExprice.Visible = false;
            }
            string Ids = string.Empty;
            // int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGroupingDirect.Rows)
            {
                GridView grdSelectedItem2 = (GridView)dimaster1.FindControl("grdSelectedItem2");
                foreach (GridViewRow diitem in grdSelectedItem2.Rows)
                {
                    CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingleD");
                    if (chkSingle.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem2.DataKeys[diitem.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("EditID", Ids);

                DataTable dtItem = new DataTable();

                if (Session["Item_list_Direct"] != null)
                    dtItem = (DataTable)Session["Item_list_Direct"];
                else
                    dtItem = LoadItemTable();

                DataTable dtEditList = LoadItemTable();
                DataView dv = dtItem.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtEditList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_direct_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];
                    drNew["unit_cost"] = dv[i]["unit_cost"];
                    drNew["total_unit_cost"] = dv[i]["total_unit_cost"];
                    drNew["total_labor_cost"] = dv[i]["total_labor_cost"];
                    dtEditList.Rows.Add(drNew);

                }
                Session.Add("ItemEditList", dtEditList);
                BindEditGrid();

                ModalPopupExtender5.Show();
                return;
            }
        }
        else if (e.CommandName == "CopyItem1")
        {
            lblMSG.Text = string.Empty;
            lblLocationCopy.Text = string.Empty;
            lblResult1.Text = string.Empty;
            lblAdd.Text = string.Empty;
            string Ids = string.Empty;

            // int index = Convert.ToInt32(e.CommandArgument);
            foreach (GridViewRow dimaster1 in grdGroupingDirect.Rows)
            {
                GridView grdSelectedItem2 = (GridView)dimaster1.FindControl("grdSelectedItem2");
                foreach (GridViewRow diitem in grdSelectedItem2.Rows)
                {
                    CheckBox chkSingle = (CheckBox)diitem.FindControl("chkSingleD");
                    if (chkSingle.Checked)
                    {
                        int id = Convert.ToInt32(grdSelectedItem2.DataKeys[diitem.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
            }
            Ids = Ids.Trim().TrimEnd(',');
            if (Ids != "")
            {
                Session.Add("CopyItemID", Ids);

                DataTable dtItem = new DataTable();

                if (Session["Item_list"] != null)
                    dtItem = (DataTable)Session["Item_list"];
                else
                    dtItem = LoadItemTable();

                DataTable dtCopyList = LoadItemTable();
                DataView dv = dtItem.DefaultView;

                dv.RowFilter = "pricing_id IN (" + Ids + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtCopyList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];
                    drNew["unit_cost"] = dv[i]["unit_cost"];
                    drNew["total_unit_cost"] = dv[i]["total_unit_cost"];
                    drNew["total_labor_cost"] = dv[i]["total_labor_cost"];

                    dtCopyList.Rows.Add(drNew);

                }
                Session.Add("ItemCopyList", dtCopyList);
                BindCopyItemGrid();

                modCopyItems.Show();
                return;
            }
        }

    }

    protected void ddlCustomerLocations_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session.Remove("sNewAddedItem");
        if (ddlCustomerLocations.SelectedItem.Text != "Select Location")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                int serial = dimaster1.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGrouping.DataKeys[dimaster1.RowIndex].Values[0]);
                string strUpdate = " UPDATE pricing_details SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=1";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                int serial = dimaster2.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGroupingDirect.DataKeys[dimaster2.RowIndex].Values[0]);
                string strUpdate = " UPDATE pricing_details SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=2";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
            string Strq = " UPDATE pricing_details SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId;
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
            BindGrid();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        BindGrid();

    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearchItemName.Text = string.Empty;
        BindGrid();

    }
    protected void grdItemPrice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nItemId = Convert.ToInt32(grdItemPrice.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]);

            bool IsMandatory = Convert.ToBoolean(grdItemPrice.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[3]);
            DropDownList ddlLabor = (DropDownList)e.Row.FindControl("ddlLabor");
            DropDownList ddlDirect = (DropDownList)e.Row.FindControl("ddlDirect");
            TextBox txtQty = (TextBox)e.Row.FindControl("txtQty");

            if (IsMandatory)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");
                ddlLabor.Attributes.CssStyle.Add("color", "Violet");
                ddlDirect.Attributes.CssStyle.Add("color", "Violet");
                txtQty.Attributes.CssStyle.Add("color", "Violet");

            }
            if (ddlCustomerLocations.SelectedItem.Text != "Select Location")
            {
                if (_db.pricing_details.Any(p => p.item_id == nItemId && p.location_id == Convert.ToInt32(ddlCustomerLocations.SelectedValue) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
                {

                    e.Row.Attributes.CssStyle.Add("background", "#E0F8E0");
                    e.Row.Attributes.CssStyle.Add("border", "2px solid #fff");
                    //  e.Row.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }

    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes = gRow.Cells[3].Controls[0].FindControl("lblshort_notes") as Label;
        Label lblshort_notes_r = gRow.Cells[3].Controls[1].FindControl("lblshort_notes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[3].Controls[2].FindControl("lnkOpen") as LinkButton;

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
    protected void lnkOpen1_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes1 = gRow.Cells[3].Controls[0].FindControl("lblshort_notes1") as Label;
        Label lblshort_notes1_r = gRow.Cells[3].Controls[1].FindControl("lblshort_notes1_r") as Label;
        LinkButton lnkOpen1 = gRow.Cells[3].Controls[2].FindControl("lnkOpen1") as LinkButton;

        if (lnkOpen1.Text == "More")
        {
            lblshort_notes1.Visible = false;
            lblshort_notes1_r.Visible = true;
            lnkOpen1.Text = " Less";
            lnkOpen1.ToolTip = "Click here to view less";
        }
        else
        {
            lblshort_notes1.Visible = true;
            lblshort_notes1_r.Visible = false;
            lnkOpen1.Text = "More";
            lnkOpen1.ToolTip = "Click here to view more";

        }

    }
    private bool IsMandatoryItemCheck()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        lblResult1.Text = "";
        lblAdd.Text = "";
        lblSelectLocation.Text = "";

        bool bflag = true;

        string strRequired = string.Empty;
        string str = hdnMandatoryId.Value;
        if (str.Length > 1)
        {
            string[] strIds = str.Split(',');


            foreach (string strId in strIds)
            {
                if (_db.pricing_details.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.item_id == Convert.ToInt32(strId) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).Count() == 0)
                {
                    if (strRequired.Length == 0)
                        strRequired = "Mandatory Items in Pink Color: " + strId;
                    else
                        strRequired += ", " + strId;
                }

            }


            if (strRequired.Length > 0)
            {

                lblReqmsg1.Text = strRequired;
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                lblAdd.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                bflag = false;
            }
        }

        return bflag;
    }
    protected void btnSearchAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearchAll.ID, btnSearchAll.GetType().Name, "Click"); 
        BindGridAllPrice();
    }

    protected void lnkClear_Click(object sender, EventArgs e)
    {
        txtSearchAll.Text = string.Empty;
    }
    protected void lnkResetAll_Click(object sender, EventArgs e)
    {
        txtSearchAll.Text = string.Empty;
        Session.Remove("TmpList");
        Session.Remove("RecentList");
        BindGridAllPrice();
        BindRecentlyGrid();

    }
    public void BindGridAllPrice()
    {

        lblResult1.Text = "";
        lblAdd.Text = "";
        lblSelectLocation.Text = "";
        DataTable dtMainList = new DataTable();
        DataTable dtTmp = new DataTable();

        DataClassesDataContext _db = new DataClassesDataContext();
        int[] ids = (int[])HttpContext.Current.Session["myIds"];

        string strId = "";
        for (int i = 0; i < ids.Length; i++)
            strId += ids[i] + ",";

        strId = strId.TrimEnd(',');
        string strCondition = string.Empty;
        if (txtSearchAll.Text.Trim() != "")
        {
            string str = txtSearchAll.Text.Trim();

            if (str.IndexOf(">>") != -1)
            {
                // var corrected = str.Substring(0, str.Length - 2);
                str = str.Substring(str.LastIndexOf(">>") + 2);
            }
            if (strId.Length > 0)
            {
                strCondition = "  AND si.section_level IN(" + strId + ")  AND si.section_name LIKE '%" + str.Trim() + "%'";
            }
            else
            {
                strCondition = "  AND si.section_name LIKE '%" + str.Trim() + "%'";
            }



            string StrQ = " SELECT it.client_id,it.item_id,si.section_name,si.is_mandatory,it.measure_unit,it.item_cost*it.retail_multiplier AS item_cost,it.minimum_qty,it.retail_multiplier,it.labor_rate,it.labor_rate* it.retail_multiplier as LaborUnitCost, it.update_time,si.section_serial,si.section_level,si.parent_id,it.labor_id,((it.item_cost + it.labor_rate) * it.retail_multiplier) * it.minimum_qty AS ext_item_cost " +
                         " FROM item_price it " +
                         " INNER JOIN sectioninfo si on si.section_id =  it.item_id " +
                         " WHERE si.is_active = 1   " + strCondition + " AND si.is_disable = 0";
            dtMainList = csCommonUtility.GetDataTable(StrQ);

            if (Session["TmpList"] == null)
            {
                if (dtMainList.Rows.Count > 0)
                {
                    Session.Add("TmpList", dtMainList);
                }

            }
            else
            {
                dtTmp = (DataTable)Session["TmpList"];
                foreach (DataRow dr in dtMainList.Rows)
                {
                    int nItemId = Convert.ToInt32(dr["item_id"]);
                    bool Iexists = dtTmp.AsEnumerable().Where(c => c.Field<Int32>("item_id").Equals(nItemId)).Count() > 0;
                    DataRow drNew = null;
                    if (!Iexists)
                    {

                        drNew = dtTmp.NewRow();
                        drNew["client_id"] = dr["client_id"];
                        drNew["item_id"] = dr["item_id"];
                        drNew["section_name"] = dr["section_name"];
                        drNew["is_mandatory"] = dr["is_mandatory"];
                        drNew["measure_unit"] = dr["measure_unit"];
                        drNew["item_cost"] = dr["item_cost"];
                        drNew["minimum_qty"] = dr["minimum_qty"];
                        drNew["retail_multiplier"] = dr["retail_multiplier"];
                        drNew["labor_rate"] = dr["labor_rate"];
                        drNew["LaborUnitCost"] = dr["LaborUnitCost"];
                        drNew["update_time"] = dr["update_time"];
                        drNew["section_serial"] = dr["section_serial"];
                        drNew["section_level"] = dr["section_level"];
                        drNew["parent_id"] = dr["parent_id"];
                        drNew["labor_id"] = dr["labor_id"];
                        drNew["ext_item_cost"] = dr["ext_item_cost"];
                        dtTmp.Rows.Add(drNew);
                    }
                }
            }
            if (Session["TmpList"] != null)
                dtTmp = (DataTable)Session["TmpList"];
            if (dtTmp.Rows.Count > 0)
            {
                Session.Add("TmpList", dtTmp);
                grdItemPriceAll.DataSource = dtTmp;
                grdItemPriceAll.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory", "section_level", "parent_id", "section_serial" };
                grdItemPriceAll.DataBind();
            }
            if (grdItemPriceAll.Rows.Count == 0)
            {
                btnAddMultiple.Visible = false;
                lblAdd.Text = csCommonUtility.GetSystemErrorMessage("No Record Found");
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("No Record Found");
            }
            else
            {
                btnAddMultiple.Visible = true;
            }
        }
        else
        {
            if (Session["TmpList"] != null)
                dtTmp = (DataTable)Session["TmpList"];
            if (dtTmp.Rows.Count > 0)
            {
                Session.Add("TmpList", dtTmp);
                grdItemPriceAll.DataSource = dtTmp;
                grdItemPriceAll.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory", "section_level", "parent_id", "section_serial" };
                grdItemPriceAll.DataBind();
            }
            else
            {
                grdItemPriceAll.DataSource = dtTmp;
                grdItemPriceAll.DataBind();
                btnAddMultiple.Visible = false;
            }
        }



    }



    protected void grdItemPriceAll_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            strDetails = "";

            int nsectionId = Convert.ToInt32(grdItemPriceAll.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]);
            int nsectionLevelId = Convert.ToInt32(grdItemPriceAll.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[4]);
            string strItem = GetItemDetialsForUpdateItem(nsectionId).ToString();
            e.Row.Cells[2].Text = strItem;
            string sectionName = GetSectionName(nsectionLevelId);
            e.Row.Cells[1].Text = sectionName;

        }

    }
    protected void btnAddMultiple_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddMultiple.ID, btnAddMultiple.GetType().Name, "Click"); 
        lblSelectLocation.Text = "";
        lblResult1.Text = "";
        lblMessage.Text = "";
        lblAdd.Text = "";
        Session.Remove("TmpColList");

        DataTable dtCloection = (DataTable)Session["TmpList"];
        Session.Add("TmpColList", dtCloection);

        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow di in grdItemPriceAll.Rows)
        {
            CheckBox chkIsSelected = (CheckBox)di.FindControl("chkIsSelected");
            int index = Convert.ToInt32(di.RowIndex);
            if (chkIsSelected.Checked)
            {
                strDetails = string.Empty;

                decimal ditem_cost = 0;
                decimal dlabor_rate = 0;
                decimal dretail_multiplier = 0;
                string smeasure_unit = "";
                decimal dminimum_qty = 0;
                decimal nQty = 0;


                int nSectionId = Convert.ToInt32(grdItemPriceAll.Rows[index].Cells[0].Text);
                TextBox txtQty = (TextBox)di.FindControl("txtQty");
                TextBox txtShortNote = (TextBox)di.FindControl("txtShortNote");
                //Label lblTotalPrice = (Label)di.FindControl("lblTotalPrice");
                DropDownList ddlLabor = (DropDownList)di.FindControl("ddlLabor");
                DropDownList ddlDirect = (DropDownList)di.FindControl("ddlDirect");


                bool isMandatory = Convert.ToBoolean(grdItemPriceAll.DataKeys[index].Values[3]);

                dlabor_rate = Convert.ToDecimal(grdItemPriceAll.DataKeys[index].Values[1].ToString());
                dretail_multiplier = Convert.ToDecimal(grdItemPriceAll.DataKeys[index].Values[2].ToString());
                smeasure_unit = di.Cells[6].Text;
                ditem_cost = Convert.ToDecimal(di.Cells[7].Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                dminimum_qty = Convert.ToDecimal(di.Cells[9].Text);
                nQty = Convert.ToDecimal(txtQty.Text);
                if (dminimum_qty > nQty)
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value for Item Id=" + nSectionId + "");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value for Item Id=" + nSectionId + "");
                    return;
                }

                decimal nLaborRate = dlabor_rate * dretail_multiplier;
                nLaborRate = nLaborRate * nQty;
                decimal nTotalPrice = 0;
                if (ddlLabor.SelectedValue == "2")
                    nTotalPrice = ditem_cost * nQty + nLaborRate;
                else
                    nTotalPrice = ditem_cost * nQty;

                if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
                {
                    lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
                    lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
                    ddlCustomerLocations.Focus();
                    return;
                }


                sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == nSectionId && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
                pricing_detail price_detail = new pricing_detail();
                price_detail.item_name = GetItemDetialsForUpdateItem(nSectionId).ToString();
                price_detail.measure_unit = smeasure_unit;
                price_detail.minimum_qty = dminimum_qty;
                price_detail.retail_multiplier = dretail_multiplier;
                price_detail.labor_rate = dlabor_rate;
                price_detail.item_cost = ditem_cost;

                price_detail.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
                price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                price_detail.section_level = Convert.ToInt32(sinfo.section_level);
                price_detail.item_id = nSectionId;
                price_detail.section_name = GetSectionName(Convert.ToInt32(sinfo.section_level));
                if (isMandatory)
                {
                    if (Convert.ToDecimal(txtQty.Text) > 0)
                    {
                        price_detail.quantity = Convert.ToDecimal(txtQty.Text);
                    }
                    else
                    {
                        price_detail.quantity = Convert.ToDecimal(0.01);
                    }
                }
                else
                {
                    price_detail.quantity = Convert.ToDecimal(txtQty.Text);
                }
                price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
                price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

                if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
                {
                    price_detail.total_retail_price = nTotalPrice;
                    price_detail.total_direct_price = 0;
                }
                else
                {
                    price_detail.total_retail_price = 0;
                    price_detail.total_direct_price = nTotalPrice;
                }
                if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
                    price_detail.labor_rate = 0;
                price_detail.section_serial = Convert.ToDecimal(sinfo.section_serial);
                //hdnItemCnt.Value = GetSerial().ToString();

                int serial = GetSerialMultiple(Convert.ToInt32(sinfo.section_level));
                price_detail.item_cnt = serial;
                price_detail.pricing_type = "A";
                price_detail.short_notes = txtShortNote.Text;
                price_detail.create_date = DateTime.Now;
                price_detail.last_update_date = DateTime.Now;
                price_detail.execution_unit = 0;
                price_detail.sort_id = 0;
                price_detail.is_mandatory = isMandatory;
                price_detail.is_CommissionExclude = IsCommissionExclude;

                _db.pricing_details.InsertOnSubmit(price_detail);
                _db.SubmitChanges();

                DataTable dtItem = new DataTable();
                DataTable dtItemDirect = new DataTable();
                DataTable dtRecentList = new DataTable();


                if (Session["RecentList"] != null)
                    dtRecentList = (DataTable)Session["RecentList"];
                else
                    dtRecentList = LoadItemTable();

                if (Session["Item_list_Direct"] != null)
                    dtItemDirect = (DataTable)Session["Item_list_Direct"];
                else
                    dtItemDirect = LoadItemTable();

                if (Session["Item_list"] != null)
                    dtItem = (DataTable)Session["Item_list"];
                else
                    dtItem = LoadItemTable();
                // Cost
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;

                decimal dTPrice = 0;

                string sItemName = price_detail.item_name.ToString();

                decimal dItemCost = Convert.ToDecimal(price_detail.item_cost);
                decimal dRetail_multiplier = Convert.ToDecimal(price_detail.retail_multiplier);
                decimal dQuantity = Convert.ToDecimal(price_detail.quantity);
                decimal dLabor_rate = Convert.ToDecimal(price_detail.labor_rate);
                if (price_detail.is_direct == 1)
                    dTPrice = Convert.ToDecimal(price_detail.total_retail_price);
                else
                    dTPrice = Convert.ToDecimal(price_detail.total_direct_price);

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

                DataRow drNew = null;
                drNew = dtRecentList.NewRow();
                if (price_detail.is_direct == 1)
                    drNew = dtItem.NewRow();
                else
                    drNew = dtItemDirect.NewRow();
                drNew["pricing_id"] = price_detail.pricing_id;
                drNew["item_id"] = price_detail.item_id;
                drNew["labor_id"] = price_detail.labor_id;
                drNew["section_serial"] = price_detail.section_serial;
                drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
                drNew["section_name"] = price_detail.section_name;
                drNew["item_name"] = price_detail.item_name;
                drNew["measure_unit"] = price_detail.measure_unit;
                drNew["item_cost"] = price_detail.item_cost;
                drNew["total_retail_price"] = price_detail.total_retail_price;
                drNew["total_direct_price"] = price_detail.total_direct_price;
                drNew["minimum_qty"] = price_detail.minimum_qty;
                drNew["quantity"] = price_detail.quantity;
                drNew["retail_multiplier"] = price_detail.retail_multiplier;
                drNew["labor_rate"] = price_detail.labor_rate;
                drNew["short_notes"] = price_detail.short_notes;
                drNew["tmpCol"] = "";
                drNew["pricing_type"] = "A";
                drNew["is_mandatory"] = price_detail.is_mandatory;
                drNew["create_date"] = price_detail.create_date;
                drNew["last_update_date"] = price_detail.last_update_date;
                drNew["customer_id"] = price_detail.customer_id;
                drNew["section_level"] = price_detail.section_level;
                drNew["location_id"] = price_detail.location_id;
                drNew["sales_person_id"] = price_detail.sales_person_id;
                drNew["client_id"] = price_detail.client_id;
                drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total


                if (price_detail.is_direct == 1)
                    dtItem.Rows.Add(drNew);
                else
                    dtItemDirect.Rows.Add(drNew);

                dtRecentList.ImportRow(drNew);
                Session.Add("Item_list", dtItem);
                Session.Add("Item_list_Direct", dtItemDirect);
                Session.Add("RecentList", dtRecentList);


                int nItemId = Convert.ToInt32(price_detail.item_id);

                dtCloection = (DataTable)Session["TmpColList"];

                DataRow[] drr = dtCloection.Select("item_id=" + nItemId + "");
                for (int i = 0; i < drr.Length; i++)
                    drr[i].Delete();
                dtCloection.AcceptChanges();

                Session.Add("TmpColList", dtCloection);
            }

        }



        hdnSortDesc.Value = "1";

        if (Session["TmpColList"] != null)
        {
            txtSearchAll.Text = string.Empty;
            DataTable dtTmpColList = (DataTable)Session["TmpColList"];
            Session.Add("TmpList", dtTmpColList);
            BindGridAllPrice();

        }
        BindRecentlyGrid();
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        Calculate_Total();

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
        lblAdd.Text = csCommonUtility.GetSystemMessage("Selected Item(s) added to estimate pricing list");
        lblResult1.Text = csCommonUtility.GetSystemMessage("Selected Item(s) added to estimate pricing list");
    }
    public void BindRecentlyGrid()
    {
        DataTable dtRecent = new DataTable();

        if (Session["RecentList"] != null)
        {
            dtRecent = (DataTable)Session["RecentList"];
        }
        if (dtRecent.Rows.Count > 0)
        {
            grdRecentlyAdded.DataSource = dtRecent;
            grdRecentlyAdded.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grdRecentlyAdded.DataBind();
        }
        else
        {
            grdRecentlyAdded.DataSource = dtRecent;
            grdRecentlyAdded.DataBind();

        }

        if (grdRecentlyAdded.Rows.Count == 0)
        {
            grdRecentlyAdded.Visible = false;
        }
        else
        {
            grdRecentlyAdded.Visible = true;
        }





    }

    private DataTable LoadItemTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("pricing_id", typeof(int));
        table.Columns.Add("item_id", typeof(int));
        table.Columns.Add("labor_id", typeof(int));
        table.Columns.Add("section_serial", typeof(decimal));
        table.Columns.Add("location_name", typeof(string));
        table.Columns.Add("section_name", typeof(string));
        table.Columns.Add("item_name", typeof(string));
        table.Columns.Add("measure_unit", typeof(string));
        table.Columns.Add("item_cost", typeof(decimal));
        table.Columns.Add("total_retail_price", typeof(decimal));
        table.Columns.Add("total_direct_price", typeof(decimal));
        table.Columns.Add("minimum_qty", typeof(decimal));
        table.Columns.Add("quantity", typeof(decimal));
        table.Columns.Add("retail_multiplier", typeof(decimal));
        table.Columns.Add("labor_rate", typeof(decimal));
        table.Columns.Add("short_notes", typeof(string));
        table.Columns.Add("tmpCol", typeof(string));
        table.Columns.Add("pricing_type", typeof(string));
        table.Columns.Add("is_mandatory", typeof(bool));
        table.Columns.Add("create_date", typeof(DateTime));
        table.Columns.Add("last_update_date", typeof(DateTime));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("section_level", typeof(int));
        table.Columns.Add("location_id", typeof(int));
        table.Columns.Add("sales_person_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("unit_cost", typeof(decimal));
        table.Columns.Add("total_unit_cost", typeof(decimal));
        table.Columns.Add("total_labor_cost", typeof(decimal));


        return table;
    }



    protected void grdRecentlyAdded_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nPricingId = Convert.ToInt32(grdRecentlyAdded.DataKeys[e.RowIndex].Values[0]);
        int nItemId = Convert.ToInt32(grdRecentlyAdded.DataKeys[e.RowIndex].Values[1]);

        DataTable dtItem = (DataTable)Session["Item_list"];
        bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
        string strQ = "Delete pricing_details WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);
        if (Iexists)
        {
            var rows = dtItem.Select("pricing_id =" + nPricingId + "");
            foreach (var row in rows)
                row.Delete();
        }
        dtItem.AcceptChanges();
        Session.Add("Item_list", dtItem);

        hdnSortDesc.Value = "0";
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
        Calculate_Total();
        hdnPricingId.Value = "0";

        DataTable dtRecent = new DataTable();

        if (Session["RecentList"] != null)
        {
            dtRecent = (DataTable)Session["RecentList"];
        }

        DataRow[] drr = dtRecent.Select("item_id=" + nItemId + "");
        for (int i = 0; i < drr.Length; i++)
            drr[i].Delete();
        dtRecent.AcceptChanges();
        Session.Add("RecentList", dtRecent);


        if (dtRecent.Rows.Count > 0)
        {
            grdRecentlyAdded.DataSource = dtRecent;
            grdRecentlyAdded.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grdRecentlyAdded.DataBind();
        }
        else
        {
            grdRecentlyAdded.DataSource = dtRecent;
            grdRecentlyAdded.DataBind();

        }

        if (nItemId > 0)
        {
            DataTable dtTmp = new DataTable();

            string StrQ = " SELECT it.client_id,it.item_id,si.section_name,si.is_mandatory,it.measure_unit,it.item_cost*it.retail_multiplier AS item_cost,it.minimum_qty,it.retail_multiplier,it.labor_rate,it.labor_rate* it.retail_multiplier as LaborUnitCost, it.update_time,si.section_serial,si.section_level,si.parent_id,it.labor_id,((it.item_cost + it.labor_rate) * it.retail_multiplier) * it.minimum_qty AS ext_item_cost " +
                         " FROM item_price it " +
                         " INNER JOIN sectioninfo si on si.section_id =  it.item_id " +
                         " WHERE si.is_active = 1 AND it.item_id =" + nItemId;
            DataTable dtMainList = csCommonUtility.GetDataTable(StrQ);

            if (Session["TmpList"] == null)
            {
                if (dtMainList.Rows.Count > 0)
                {
                    Session.Add("TmpList", dtMainList);
                }

            }
            else
            {
                dtTmp = (DataTable)Session["TmpList"];
                foreach (DataRow dr in dtMainList.Rows)
                {
                    bool Iexists2 = dtTmp.AsEnumerable().Where(c => c.Field<Int32>("item_id").Equals(nItemId)).Count() > 0;
                    DataRow drNew = null;
                    if (!Iexists2)
                    {

                        drNew = dtTmp.NewRow();
                        drNew["client_id"] = dr["client_id"];
                        drNew["item_id"] = dr["item_id"];
                        drNew["section_name"] = dr["section_name"];
                        drNew["is_mandatory"] = dr["is_mandatory"];
                        drNew["measure_unit"] = dr["measure_unit"];
                        drNew["item_cost"] = dr["item_cost"];
                        drNew["minimum_qty"] = dr["minimum_qty"];
                        drNew["retail_multiplier"] = dr["retail_multiplier"];
                        drNew["labor_rate"] = dr["labor_rate"];
                        drNew["LaborUnitCost"] = dr["LaborUnitCost"];
                        drNew["update_time"] = dr["update_time"];
                        drNew["section_serial"] = dr["section_serial"];
                        drNew["section_level"] = dr["section_level"];
                        drNew["parent_id"] = dr["parent_id"];
                        drNew["labor_id"] = dr["labor_id"];
                        drNew["ext_item_cost"] = dr["ext_item_cost"];
                        dtTmp.Rows.Add(drNew);
                    }
                }
            }
            if (Session["TmpList"] != null)
                dtTmp = (DataTable)Session["TmpList"];
            if (dtTmp.Rows.Count > 0)
            {
                Session.Add("TmpList", dtTmp);
                grdItemPriceAll.DataSource = dtTmp;
                grdItemPriceAll.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory", "section_level", "parent_id", "section_serial" };
                grdItemPriceAll.DataBind();
            }
            if (grdItemPriceAll.Rows.Count == 0)
            {
                btnAddMultiple.Visible = false;
            }
            else
            {
                btnAddMultiple.Visible = true;
            }
        }
        lblResult1.Text = csCommonUtility.GetSystemMessage("Undo successful, item deleted from pricing and back in Collector Grid");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Undo successful, item deleted from pricing and back in Collector Grid");

    }
    protected void imgHelp_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgHelp.ID, imgHelp.GetType().Name, "Click"); 
        string strPopup = "<script language='javascript' ID='script1'>"

         // Passing intId to popup window.
         + "window.open('AddingItemInPricing.aspx"

         + "','new window', 'top=140, left=180, width=950, height=630, dependant=no, location=0, alwaysRaised=no, menubar=no, resizeable=no, scrollbars=yes, toolbar=no, status=no, center=yes')"

         + "</script>";

        ScriptManager.RegisterStartupScript((Page)HttpContext.Current.Handler, typeof(Page), "Script1", strPopup, false);
    }

    protected void chkCustDisp_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = string.Empty;

            chkPMDisplay.Checked = false;

            if (chkCustDisp.Checked)
            {
                strQ = "UPDATE customer_estimate SET IsCustDisplay = 1 " +
              " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
              " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
              " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
                //lblResult1.Text = csCommonUtility.GetSystemMessage("Customized display setting of Project activated successfully.");

            }
            else
            {
                strQ = "UPDATE customer_estimate SET IsCustDisplay = 0 " +
             " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
             " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
             " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                _db.ExecuteCommand(strQ, string.Empty);
                //lblResult1.Text = csCommonUtility.GetSystemMessage("Customized display setting of Project deactivated successfully.");
            }
            hdnSortDesc.Value = "0";
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
            Calculate_Total();
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }

    }

    protected void btnExpCostLocList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCostLocList.ID, btnExpCostLocList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  " +
              " UNION " +
              " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtCostMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sCostList", dtCostMaster);

        DataTable tmpTable = LoadTmpDataTable();
        #region Cost by Location
        decimal GItemTotal = 0;
        decimal GLaborTotal = 0;
        decimal GSubTotalCost = 0;

        DataView dvLoc = new DataView(dtCostMaster);
        DataTable dtResults = dvLoc.ToTable(true, "location_id", "location_name");

        DataTable tblCustList = (DataTable)Session["sCostList"];
        DataRow drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Cost by Location";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Customer Name: " + lblCustomerName.Text;
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Estimate: " + lblEstimateName.Text;
        tmpTable.Rows.Add(drNew);

        foreach (DataRow dr in dtResults.Rows)
        {
            decimal dSubItemTotal = 0;
            decimal dSubLaborTotal = 0;
            decimal dSubTotalCost = 0;
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

                    dSubItemTotal += dOrginalTotalCost;
                    dSubLaborTotal += dLaborTotal;
                    dSubTotalCost += dLineItemTotal;

                    GItemTotal += dOrginalTotalCost;
                    GLaborTotal += dLaborTotal;
                    GSubTotalCost += dLineItemTotal;

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

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            drNew["Item Name"] = "";
            drNew["Short Notes"] = "";
            drNew["U of M"] = "";
            drNew["Code"] = "";
            drNew["Item Cost"] = "Sub Total:";
            drNew["Item Total"] = dSubItemTotal.ToString("c");
            drNew["Labor Rate"] = "";
            drNew["Labor Total"] = dSubLaborTotal.ToString("c");
            drNew["Total Cost"] = dSubTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

        }

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "";
        drNew["Item Name"] = "";
        drNew["Short Notes"] = "";
        drNew["U of M"] = "";
        drNew["Code"] = "";
        drNew["Item Cost"] = "Grand Total:";
        drNew["Item Total"] = GItemTotal.ToString("c");
        drNew["Labor Rate"] = "";
        drNew["Labor Total"] = GLaborTotal.ToString("c");
        drNew["Total Cost"] = GSubTotalCost.ToString("c");
        tmpTable.Rows.Add(drNew);

        #endregion

        Session.Add("CostReport", tmpTable);
        string strUrl = "CostReportCsv.aspx?TypeId=1";
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
    }
    protected void btnExpCostSecList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCostSecList.ID, btnExpCostSecList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  " +
              " UNION " +
              " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtCostMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sCostList", dtCostMaster);

        DataTable tmpTable = LoadTmpDataTable();
        #region Cost by Section
        decimal GItemTotal = 0;
        decimal GLaborTotal = 0;
        decimal GSubTotalCost = 0;
        DataView dvSec = new DataView(dtCostMaster);
        DataTable dtResults = dvSec.ToTable(true, "section_level", "section_name");

        DataTable tblCustList = (DataTable)Session["sCostList"];
        DataRow drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Cost by Section";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Customer Name: " + lblCustomerName.Text;
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Estimate: " + lblEstimateName.Text;
        tmpTable.Rows.Add(drNew);

        foreach (DataRow dr in dtResults.Rows)
        {
            decimal dSubItemTotal = 0;
            decimal dSubLaborTotal = 0;
            decimal dSubTotalCost = 0;
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

                    dSubItemTotal += dOrginalTotalCost;
                    dSubLaborTotal += dLaborTotal;
                    dSubTotalCost += dLineItemTotal;

                    GItemTotal += dOrginalTotalCost;
                    GLaborTotal += dLaborTotal;
                    GSubTotalCost += dLineItemTotal;

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
            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            drNew["Item Name"] = "";
            drNew["Short Notes"] = "";
            drNew["U of M"] = "";
            drNew["Code"] = "";
            drNew["Item Cost"] = "Sub Total:";
            drNew["Item Total"] = dSubItemTotal.ToString("c");
            drNew["Labor Rate"] = "";
            drNew["Labor Total"] = dSubLaborTotal.ToString("c");
            drNew["Total Cost"] = dSubTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

        }
        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "";
        drNew["Item Name"] = "";
        drNew["Short Notes"] = "";
        drNew["U of M"] = "";
        drNew["Code"] = "";
        drNew["Item Cost"] = "Grand Total:";
        drNew["Item Total"] = GItemTotal.ToString("c");
        drNew["Labor Rate"] = "";
        drNew["Labor Total"] = GLaborTotal.ToString("c");
        drNew["Total Cost"] = GSubTotalCost.ToString("c");
        tmpTable.Rows.Add(drNew);
        #endregion
        Session.Add("CostReport", tmpTable);
        string strUrl = "CostReportCsv.aspx?TypeId=2";
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
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
    protected void chkPMDisplay_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkPMDisplay.ID, chkPMDisplay.GetType().Name, "CheckedChanged"); 
        try
        {
            chkCustDisp.Checked = false;
            hdnSortDesc.Value = "0";
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
            Calculate_Total();
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
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
        if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        {
            com = _db.company_profiles.Single(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
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
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            //strPO = cus_est.job_number;
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
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        string strQ = string.Empty;

        strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();


        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptEmailReportBySection.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = "";//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.last_name1; // objCust.first_name1 + " " + objCust.last_name1;
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

    protected void btnPricingPrint_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPricingPrint.ID, btnPricingPrint.GetType().Name, "Click"); 
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        string strQ = string.Empty;

        strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();

        string strPsumm = string.Empty;
        if (rdoSort.SelectedValue == "1")
        {
            strPsumm = " SELECT  t1.location_id as MainID,lc.location_name  AS MainName,t3.ProjectCost, max(t1.sort_id) as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + t2.section_name  " +
                    " FROM pricing_details t2  " +
                    " WHERE t2.location_id = t1.location_id AND t2.estimate_id = " + nEstId + " AND t2.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  " +
                    "FOR XML PATH (''))  " +
                    "  ,1,2,'') AS SummaryName  " +
                    " FROM pricing_details t1   " +
                    " INNER JOIN location lc ON lc.location_id = t1.location_id  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,location_id from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id= " + nEstId + " and  " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 )   " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 )  " +
                    " GROUP BY location_id ) AS t3 on t3.location_id = t1.location_id  " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and t1.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id = " + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1)  " +
                    " GROUP BY t1.location_id,lc.location_name,t3.ProjectCost; ";
        }
        else
        {
            strPsumm = " SELECT  t1.section_level as MainID,t1.section_name AS MainName,t3.ProjectCost,1 as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + lc.location_name  " +
                    " FROM pricing_details t2 " +
                    " INNER JOIN location lc ON lc.location_id = t2.location_id " +
                    " WHERE t2.section_level = t1.section_level AND t2.estimate_id =" + nEstId + " AND t2.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " " +
                    " FOR XML PATH ('')) " +
                    " ,1,2,'') AS SummaryName " +
                    " FROM pricing_details t1  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,section_level from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + nEstId + " and " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 )  " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 ) " +
                    " GROUP BY section_level ) AS t3 on t3.section_level = t1.section_level " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and t1.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 )" +
                    " GROUP BY t1.section_level,t1.section_name,t3.ProjectCost;";


        }

        List<PMainSummary> PSummList = _db.ExecuteQuery<PMainSummary>(strPsumm, string.Empty).ToList();

        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = string.Empty;

        if (rdoSort.SelectedValue == "1")
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportByLocation.rpt");
        else if (rdoSort.SelectedValue == "2")
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportBySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportByLocation.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        ReportDocument subReport = rptFile.OpenSubreport("rptSummaryPricing.rpt");
        subReport.SetDataSource(PSummList);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = "";
        string strCustomerName = objCust.last_name1;
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


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }

    protected void grdEditItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            TextBox txtUnit_Cost = (TextBox)e.Row.FindControl("txtUnit_Cost");
            Label lblUnit_Cost = (Label)e.Row.FindControl("lblUnit_Cost");
            TextBox txtTotalLabor_Rate = (TextBox)e.Row.FindControl("txtTotalLabor_Rate");

            TextBox txtretail_multiplier = (TextBox)e.Row.FindControl("txtretail_multiplier");
            Label lblretail_multiplier = (Label)e.Row.FindControl("lblretail_multiplier");
            TextBox txtTotalPrice = (TextBox)e.Row.FindControl("txtTotalPrice");
            Label lblTotal_price = (Label)e.Row.FindControl("lblTotal_price");

            if (chkPMDisplay.Checked)
            {


                if (Session["oUser"] != null)
                {
                    userinfo objUser = (userinfo)Session["oUser"];
                    if (objUser.IsPriceChange == true || objUser.role_id == 1)
                    {
                        txtUnit_Cost.Visible = true;
                        lblUnit_Cost.Visible = false;
                        txtTotalLabor_Rate.Visible = true;

                        txtretail_multiplier.Visible = true;
                        lblretail_multiplier.Visible = false;

                        if (rdbmulExprice.SelectedValue == "0")
                        {
                            txtretail_multiplier.Visible = true;
                            lblretail_multiplier.Visible = false;
                            txtTotalPrice.Visible = false;
                            lblTotal_price.Visible = true;
                        }
                        else
                        {
                            txtTotalPrice.Visible = true;
                            lblTotal_price.Visible = false;
                            txtretail_multiplier.Visible = false;
                            lblretail_multiplier.Visible = true;
                        }
                    }
                    else
                    {
                        txtUnit_Cost.Visible = false;
                        lblUnit_Cost.Visible = true;
                        txtTotalLabor_Rate.Visible = false;
                        txtTotalPrice.Visible = false;
                        lblTotal_price.Visible = true;
                        txtretail_multiplier.Visible = false;
                        lblretail_multiplier.Visible = true;
                    }
                }

                grdEditItem.Columns[8].Visible = true;
                grdEditItem.Columns[9].Visible = true;
                grdEditItem.Columns[10].Visible = true;
                grdEditItem.Columns[11].Visible = true;
                grdEditItem.Columns[12].Visible = true;
            }
            else
            {
                txtTotalPrice.Visible = false;
                lblTotal_price.Visible = true;
                grdEditItem.Columns[8].Visible = false;
                grdEditItem.Columns[9].Visible = false;
                grdEditItem.Columns[10].Visible = false;
                grdEditItem.Columns[11].Visible = false;
                grdEditItem.Columns[12].Visible = false;
            }
        }

    }


    protected void rdbmulExprice_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindEditGrid();
        ModalPopupExtender5.Show();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdate.ID, btnUpdate.GetType().Name, "Click"); 
        string strID = string.Empty;
        DataTable dtItem2 = new DataTable();
        DataTable dtEditList = LoadItemTable();
        if (Session["EditID"] == null)
        {
            return;
        }
        else
        {
            strID = Session["EditID"].ToString();
            DataClassesDataContext _db = new DataClassesDataContext();

            lblSelectLocation.Text = "";
            lblAdd.Text = "";
            lblMessage.Text = "";
            lblResult1.Text = "";

            foreach (GridViewRow gvr in grdEditItem.Rows)
            {
                decimal nLaborRate = 0;
                decimal nTotalPrice = 0;
                //int directId = 1;
                decimal nUnitCost = 0;
                decimal nExtTotalCost = 0;
                decimal nTotallaborRate = 0;
                decimal nRetailMultiplier = 0;
                decimal nTotalRetailPrice = 0;
                decimal nItemCost = 0;
                decimal nQty = 0;
                TextBox txtquantity = (TextBox)gvr.FindControl("txtquantity");
                Label lblTotal_price = (Label)gvr.FindControl("lblTotal_price");
                Label lblitem_name = (Label)gvr.FindControl("lblitem_name");
                TextBox txtshort_notes = (TextBox)gvr.FindControl("txtshort_notes");
                int nPricingId = Convert.ToInt32(grdEditItem.DataKeys[gvr.RowIndex].Values[0]);
                int nItemId = Convert.ToInt32(grdEditItem.DataKeys[gvr.RowIndex].Values[1]);



                //Shohidullah  
                TextBox txtUnit_Cost = (TextBox)gvr.FindControl("txtUnit_Cost");
                TextBox txtMultiplier = (TextBox)gvr.FindControl("txtretail_multiplier");
                TextBox txtTotalLabor_Rate = (TextBox)gvr.FindControl("txtTotalLabor_Rate");
                Label lblTotalLabor_Rate = (Label)gvr.FindControl("lblTotalLabor_Rate");
                Label lblTotalLabor_Cost = (Label)gvr.FindControl("lblTotalLabor_Cost");
                DropDownList ddlDirect = (DropDownList)gvr.FindControl("ddlDirect");
                TextBox txtTotalPrice = (TextBox)gvr.FindControl("txtTotalPrice");


                try
                {
                    nUnitCost = Convert.ToDecimal(txtUnit_Cost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nUnitCost = 0;
                }

                try
                {
                    nTotallaborRate = Convert.ToDecimal(txtTotalLabor_Rate.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nTotallaborRate = 0;
                }

                try
                {
                    nRetailMultiplier = Convert.ToDecimal(txtMultiplier.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nRetailMultiplier = 0;
                }
                try
                {
                    nQty = Convert.ToDecimal(txtquantity.Text);
                }
                catch
                {
                    nQty = 0;
                }

                try
                {
                    nTotalRetailPrice = Convert.ToDecimal(txtTotalPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                catch
                {
                    nTotalRetailPrice = 0;
                }

                try
                {
                    if (rdbmulExprice.SelectedValue == "0")
                    {

                        if (lblitem_name.Text.IndexOf("Other") > 0)
                        {

                            if (nQty > 0)
                            {
                                nTotalPrice = Math.Round((nUnitCost * nQty * 2), 2);
                                nItemCost = nUnitCost;
                            }
                            nLaborRate = 0;
                            nRetailMultiplier = 0;

                        }
                        else
                        {
                            if (nRetailMultiplier > 0)
                            {
                                nItemCost = nUnitCost * (decimal)nRetailMultiplier;
                                nLaborRate = nTotallaborRate;// (decimal)itm.labor_rate * (decimal)itm.retail_multiplier; 
                                // nItemCost = nCost1;
                                nTotalPrice = Math.Round(((nUnitCost + nTotallaborRate) * (decimal)nRetailMultiplier * (decimal)nQty), 2);
                                lblTotal_price.Text = nTotalPrice.ToString("c");
                            }
                            else
                            {
                                nItemCost = nUnitCost;
                                nLaborRate = nTotallaborRate;// (decimal)itm.labor_rate * (decimal)itm.retail_multiplier; 
                                // nItemCost = nCost1;
                                nTotalPrice = Math.Round(((nUnitCost + nTotallaborRate) * (decimal)nQty), 2);
                                lblTotal_price.Text = nTotalPrice.ToString("c");
                            }
                        }
                    }
                    else
                    {

                        if (lblitem_name.Text.IndexOf("Other") > 0)
                        {

                            if (nQty > 0)
                            {
                                // nTotalPrice = nUnitCost * nQty * 2;
                                //nItemCost = nUnitCost;
                                nUnitCost = nTotalRetailPrice / nQty / 2;
                                nItemCost = nUnitCost;
                                nTotalPrice = nTotalRetailPrice;
                              
                            }
                            nLaborRate = 0;
                            nRetailMultiplier = 0;

                        }
                        else
                        {
                            nExtTotalCost = Math.Round(((nUnitCost + nTotallaborRate) * nQty), 2);//(nTotalRetailPrice - (nUnitCost +nTotallaborRate)*nQty);
                            nRetailMultiplier = Math.Round((nTotalRetailPrice / nExtTotalCost), 6);
                            nItemCost = nUnitCost * nRetailMultiplier;
                            nLaborRate = nTotallaborRate;
                            nTotalPrice = Math.Round((nExtTotalCost * (decimal)nRetailMultiplier), 2);
                            lblTotal_price.Text = nTotalPrice.ToString("c");

                        }

                    }

                }
                catch
                {
                    nTotalPrice = 0;
                }


                if (Convert.ToInt32(ddlDirect.SelectedValue) == 1 || Convert.ToInt32(ddlDirect.SelectedValue) == 2)
                {
                    pricing_detail pd = _db.pricing_details.Single(p => p.item_id == nItemId && p.pricing_id == nPricingId && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                    pd.labor_rate = nLaborRate;
                    pd.retail_multiplier = nRetailMultiplier;
                    pd.item_cost = nItemCost;
                    pd.short_notes = txtshort_notes.Text.Replace("'", "''");
                    pd.quantity = nQty;
                    if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
                    {
                        pd.total_retail_price = nTotalPrice;
                        pd.total_direct_price = 0;
                    }
                    else
                    {
                        pd.total_retail_price = 0;
                        pd.total_direct_price = nTotalPrice;
                    }
                    pd.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);
                    _db.SubmitChanges();

                    DataTable dtItem = (DataTable)Session["Item_list"];

                    bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
                    if (Iexists)
                    {
                        var rows = dtItem.Select("pricing_id =" + nPricingId + "");
                        foreach (var row in rows)
                        {
                            // Cost
                            decimal dOrginalCost = 0;
                            decimal dOrginalTotalCost = 0;
                            decimal dLaborTotal = 0;
                            decimal dLineItemTotal = 0;




                            string sItemName = row["item_name"].ToString();


                            if (sItemName.IndexOf("Other") > 0)
                            {

                                if (nQty > 0)
                                {
                                    dOrginalCost = nUnitCost;
                                }
                                nLaborRate = 0;
                                nRetailMultiplier = 0;

                            }
                            else
                            {
                                if (nRetailMultiplier > 0)
                                {

                                    dOrginalCost = (nItemCost / nRetailMultiplier);
                                }
                                else
                                {
                                    dOrginalCost = nItemCost;
                                }
                            }

                            dOrginalTotalCost = dOrginalCost * nQty;
                            dLaborTotal = nLaborRate * nQty;
                            dLineItemTotal = (dOrginalCost + nLaborRate) * nQty;

                            row["quantity"] = nQty;
                            row["total_retail_price"] = nTotalPrice;
                            row["short_notes"] = txtshort_notes.Text.Replace("'", "''");

                            row["unit_cost"] = dOrginalCost;
                            row["retail_multiplier"] = nRetailMultiplier;

                            // Orginal Unit Cost
                            row["labor_rate"] = nLaborRate;
                            row["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                            row["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                        }
                    }
                    Session.Add("Item_list", dtItem);

                }
                else
                {
                    string strQ = "UPDATE pricing_details SET quantity=" + nQty + " ,total_direct_price=" + nTotalPrice + " ,short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    _db.ExecuteCommand(strQ, string.Empty);
                    DataTable dtItemDiret = (DataTable)Session["Item_list_Direct"];
                    if (Session["Item_list_Direct"] != null)
                    {
                        bool Iexists = dtItemDiret.AsEnumerable().Where(c => c.Field<Int32>("pricing_id").Equals(nPricingId)).Count() > 0;
                        if (Iexists)
                        {
                            var rows = dtItemDiret.Select("pricing_id =" + nPricingId + "");
                            foreach (var row in rows)
                            {
                                // Cost
                                decimal dOrginalCost = 0;
                                decimal dOrginalTotalCost = 0;
                                decimal dLaborTotal = 0;
                                decimal dLineItemTotal = 0;


                                string sItemName = row["item_name"].ToString();

                                decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                                decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                                decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                                decimal dTPrice = Convert.ToDecimal(row["total_direct_price"]);

                                if (sItemName.IndexOf("Other") > 0)
                                {
                                    if (nQty > 0)
                                    {
                                        dOrginalCost = (dTPrice / nQty) / 2;
                                        dLabor_rate = 0;
                                    }
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
                                dOrginalTotalCost = dOrginalCost * nQty;
                                dLaborTotal = dLabor_rate * nQty;
                                dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                                row["quantity"] = nQty;
                                row["total_direct_price"] = nTotalPrice;
                                row["short_notes"] = txtshort_notes.Text.Replace("'", "''");
                                row["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                                row["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                                row["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                            }
                        }
                    }
                    Session.Add("Item_list_Direct", dtItemDiret);

                }




                if (Session["Item_list"] != null)
                    dtItem2 = (DataTable)Session["Item_list"];
                else
                    dtItem2 = LoadItemTable();


                DataView dv = dtItem2.DefaultView;

                dv.RowFilter = "pricing_id IN (" + nPricingId + ")";


                DataRow drNew = null;
                for (int i = 0; i < dv.Count; i++)
                {
                    drNew = dtEditList.NewRow();

                    drNew["pricing_id"] = dv[i]["pricing_id"];
                    drNew["item_id"] = dv[i]["item_id"];
                    drNew["labor_id"] = dv[i]["labor_id"];
                    drNew["section_serial"] = dv[i]["section_serial"];
                    drNew["location_name"] = dv[i]["location_name"];
                    drNew["section_name"] = dv[i]["section_name"];
                    drNew["item_name"] = dv[i]["item_name"];
                    drNew["measure_unit"] = dv[i]["measure_unit"];
                    drNew["item_cost"] = dv[i]["item_cost"];
                    drNew["total_retail_price"] = dv[i]["total_retail_price"];
                    drNew["total_direct_price"] = dv[i]["total_direct_price"];
                    drNew["minimum_qty"] = dv[i]["minimum_qty"];
                    drNew["quantity"] = dv[i]["quantity"];
                    drNew["retail_multiplier"] = dv[i]["retail_multiplier"];
                    drNew["labor_rate"] = dv[i]["labor_rate"];
                    drNew["short_notes"] = dv[i]["short_notes"];
                    drNew["tmpCol"] = "";
                    drNew["pricing_type"] = "A";
                    drNew["is_mandatory"] = dv[i]["is_mandatory"];
                    drNew["create_date"] = dv[i]["create_date"];
                    drNew["last_update_date"] = dv[i]["last_update_date"];
                    drNew["customer_id"] = dv[i]["customer_id"];
                    drNew["section_level"] = dv[i]["section_level"];
                    drNew["location_id"] = dv[i]["location_id"];
                    drNew["sales_person_id"] = dv[i]["sales_person_id"];
                    drNew["client_id"] = dv[i]["client_id"];
                    drNew["unit_cost"] = dv[i]["unit_cost"];
                    drNew["total_unit_cost"] = dv[i]["total_unit_cost"];
                    drNew["total_labor_cost"] = dv[i]["total_labor_cost"];
                    dtEditList.Rows.Add(drNew);

                }



            }

            Session.Add("ItemEditList", dtEditList);

            subtotal = 0.0;
            subtotal_diect = 0.0;
            hdnSortDesc.Value = "0";
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
            //hdnPricingId.Value = "0";
            Calculate_Total();
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
            lblMSG.Text = csCommonUtility.GetSystemMessage("Item updated successfully");

            BindEditGrid();
            ModalPopupExtender5.Show();


        }
    }


    public void BindCopyItemGrid()
    {
        DataTable dtCopyItems = new DataTable();

        if (Session["ItemCopyList"] != null)
        {
            dtCopyItems = (DataTable)Session["ItemCopyList"];
        }
        if (dtCopyItems.Rows.Count > 0)
        {
            grdCopyItem.DataSource = dtCopyItems;
            grdCopyItem.DataKeyNames = new string[] { "pricing_id", "item_id", "total_retail_price" };
            grdCopyItem.DataBind();
        }
        else
        {
            grdCopyItem.DataSource = dtCopyItems;
            grdCopyItem.DataBind();

        }

        if (grdCopyItem.Rows.Count == 0)
        {
            grdCopyItem.Visible = false;
        }
        else
        {
            grdCopyItem.Visible = true;
        }
    }

    protected void btnCopy_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCopy.ID, btnCopy.GetType().Name, "Click"); 
        try
        {

            lblLocationCopy.Text = "";
            string strID = string.Empty;
            if (Session["CopyItemID"] == null)
            {
                return;
            }
            else
            {
                if (ddCopyLocation.SelectedItem.Text == "Select Location")
                {
                    lblLocationCopy.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
                    ddCopyLocation.Focus();
                    modCopyItems.Show();
                    return;
                }
                strID = Session["CopyItemID"].ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                lblSelectLocation.Text = "";
                lblAdd.Text = "";
                lblMessage.Text = "";
                lblResult1.Text = "";

                foreach (GridViewRow gvr in grdCopyItem.Rows)
                {
                    decimal newQty = 0;
                    TextBox txtquantity = (TextBox)gvr.FindControl("txtquantity");
                    if (txtquantity.Text != "")
                        newQty = Convert.ToDecimal(txtquantity.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    else
                        newQty = 1;

                    TextBox txtshort_notes = (TextBox)gvr.FindControl("txtshort_notes");
                    int nPricingId = Convert.ToInt32(grdCopyItem.DataKeys[gvr.RowIndex].Values[0]);
                    int nItemId = Convert.ToInt32(grdCopyItem.DataKeys[gvr.RowIndex].Values[1]);
                    pricing_detail objPD = new pricing_detail();

                    pricing_detail pd = _db.pricing_details.SingleOrDefault(p => p.item_id == nItemId && p.pricing_id == nPricingId && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                    //arefin 102719
                    int nSortId = _db.pricing_details.Where(p => p.customer_id == pd.customer_id && p.estimate_id == pd.estimate_id && p.location_id == Convert.ToInt32(ddCopyLocation.SelectedValue)).DefaultIfEmpty().Max(x => x == null ? 0 : x.sort_id) ?? 0;
                    
                    if (pd != null)
                    {
                        objPD.client_id = pd.client_id;
                        objPD.customer_id = pd.customer_id;
                        objPD.estimate_id = pd.estimate_id;
                        objPD.location_id = Convert.ToInt32(ddCopyLocation.SelectedValue);
                        objPD.sales_person_id = pd.sales_person_id;
                        objPD.section_level = pd.section_level;
                        objPD.item_id = pd.item_id;
                        objPD.section_name = pd.section_name;
                        objPD.item_name = pd.item_name;
                        objPD.measure_unit = pd.measure_unit;
                        objPD.item_cost = pd.item_cost;
                        objPD.minimum_qty = pd.minimum_qty;
                        objPD.quantity = newQty;
                        objPD.retail_multiplier = pd.retail_multiplier;
                        objPD.labor_rate = pd.labor_rate;
                        objPD.labor_id = pd.location_id;
                        objPD.section_serial = pd.section_serial;
                        objPD.item_cnt = pd.item_cnt;
                        // Total direct
                        if (pd.quantity == newQty)
                            objPD.total_direct_price = pd.total_direct_price;
                        else if (newQty > pd.quantity)
                            objPD.total_direct_price = Convert.ToDecimal(pd.total_direct_price) * newQty;
                        else if (newQty < pd.quantity)
                            objPD.total_direct_price = Convert.ToDecimal(pd.total_direct_price) / newQty;
                        // Total retail
                        if (pd.quantity == newQty)
                            objPD.total_retail_price = pd.total_retail_price;
                        else if (newQty > pd.quantity)
                            objPD.total_retail_price = Convert.ToDecimal(pd.total_retail_price) * newQty;
                        else if (newQty < pd.quantity)
                            objPD.total_retail_price = Convert.ToDecimal(pd.total_retail_price) / newQty;


                        objPD.is_direct = pd.is_direct;
                        objPD.pricing_type = pd.pricing_type;
                        objPD.short_notes = txtshort_notes.Text;
                        objPD.create_date = DateTime.Now;
                        objPD.last_update_date = DateTime.Now;
                        objPD.other_item_cnt = pd.other_item_cnt;
                        objPD.upload_file_path = pd.upload_file_path;
                        objPD.execution_unit = pd.execution_unit;
                        objPD.sort_id = nSortId;
                        objPD.is_mandatory = pd.is_mandatory;
                        objPD.is_CommissionExclude = pd.is_CommissionExclude;
                        objPD.short_notes_new = pd.short_notes_new;
                      
                        _db.pricing_details.InsertOnSubmit(objPD);
                    }

                }
                _db.SubmitChanges();
            }
            subtotal = 0.0;
            subtotal_diect = 0.0;
            hdnSortDesc.Value = "0";
            Session["Item_list"] = null;
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
            hdnPricingId.Value = "0";
            Calculate_Total();
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item  has been copied & saved successfully");
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item has been copied & saved successfully");

        }
        catch (Exception ex)
        {
            lblResult1.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void grdCopyItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            TextBox txtUnit_Cost = (TextBox)e.Row.FindControl("txtUnit_Cost");
            Label lblUnit_Cost = (Label)e.Row.FindControl("lblUnit_Cost");
            TextBox txtTotalLabor_Rate = (TextBox)e.Row.FindControl("txtTotalLabor_Rate");

            TextBox txtretail_multiplier = (TextBox)e.Row.FindControl("txtretail_multiplier");
            Label lblretail_multiplier = (Label)e.Row.FindControl("lblretail_multiplier");
            TextBox txtTotalPrice = (TextBox)e.Row.FindControl("txtTotalPrice");
            Label lblTotal_price = (Label)e.Row.FindControl("lblTotal_price");
            Label lblTotal_priceORG = (Label)e.Row.FindControl("lblTotal_priceORG");
            if (chkPMDisplay.Checked)
            {

                txtUnit_Cost.Visible = false;
                lblUnit_Cost.Visible = true;
                txtTotalLabor_Rate.Visible = false;

                txtretail_multiplier.Visible = false;
                lblretail_multiplier.Visible = true;

                txtTotalPrice.Visible = false;
                lblTotal_price.Visible = true;
                grdCopyItem.Columns[8].Visible = true;
                grdCopyItem.Columns[9].Visible = true;
                grdCopyItem.Columns[10].Visible = true;
                grdCopyItem.Columns[11].Visible = true;
                grdCopyItem.Columns[12].Visible = true;
                grdCopyItem.Columns[14].Visible = true;
            }
            else
            {

                grdCopyItem.Columns[8].Visible = false;
                grdCopyItem.Columns[9].Visible = false;
                grdCopyItem.Columns[10].Visible = false;
                grdCopyItem.Columns[11].Visible = false;
                grdCopyItem.Columns[12].Visible = false;
                grdCopyItem.Columns[14].Visible = false;

            }
           

        }
    }


    protected void btnCustomizeItemAdd_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddOthers.ID, btnAddOthers.GetType().Name, "Click");
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
            lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
            ddlCustomerLocations.Focus();
            return;
        }

        decimal nQty = 1;
        decimal nUnitPrice = 0;
        decimal nRetailMul = 0;

        if (txtCustomizeItemName.Text == "")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Item Name is required filed.");
            txtCustomizeItemName.Focus();
            return;
        }

        if (txtCustomizeUnitPrice.Text == "")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Unit Cost is required filed.");
            txtCustomizeUnitPrice.Focus();
            return;
        }
        if (txtCustomizeCode.Text == "")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Code is required filed.");
            txtCustomizeCode.Focus();
            return;
        }

        if (txtCustomizeExtPrice.Text == "")
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Ex.Price is required filed.");
            txtCustomizeExtPrice.Focus();
            return;
        }

        try
        {
            nQty = Convert.ToDecimal(txtCustomizeCode.Text.Trim());
        }
        catch (Exception ex)
        {

            nQty = 0;

        }
        if (nQty < 0)
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Negative numbers are not allowed in Code.");
            txtCustomizeCode.Focus();
            return;
        }
        if (nQty == 0)
        {
            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Code should be greater than zero.");
            txtCustomizeCode.Focus();
            return;
        }
        try
        {
            nUnitPrice = Convert.ToDecimal(txtCustomizeUnitPrice.Text.Replace("(", "-").Replace(")", "").Replace("$", "").Trim());
        }
        catch (Exception ex)
        {

            nUnitPrice = 0;

        }
        decimal nTotalPrice =0;
        try
        {
            nTotalPrice = Convert.ToDecimal(txtCustomizeExtPrice.Text.Replace("(", "-").Replace(")", "").Replace("$", "").Trim());
        }
        catch (Exception ex)
        {

            nTotalPrice = 0;

        }

      if(nTotalPrice==nUnitPrice)
        {
            nRetailMul = nTotalPrice /nUnitPrice;
        }
       else if (nTotalPrice > nUnitPrice)
       {
          nRetailMul = nTotalPrice / nUnitPrice / nQty;
       }
        
        sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
        pricing_detail price_detail = new pricing_detail();
        ///  string test = lblParent.Text.Trim().Replace(ddlSections.SelectedItem.Text+" >>", "") + "" + txtOther.Text + ">>Other";
        price_detail.item_name = lblParent.Text.Trim().Replace(ddlSections.SelectedItem.Text + " >>", "") + "" + txtCustomizeItemName.Text;//GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString() + "" + txtOther.Text + ">>Other";
        price_detail.measure_unit = txtCustomizeUOM.Text;
        price_detail.minimum_qty = 1;
        price_detail.retail_multiplier = nRetailMul;
        price_detail.labor_rate = 0;
        price_detail.item_cost = nUnitPrice * nRetailMul; //Convert.ToDecimal(txtO_Price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        price_detail.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
        price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
        price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
        price_detail.item_id = Convert.ToInt32(hdnOtherId.Value);
        price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
        price_detail.quantity = nQty;//Convert.ToDecimal(txtO_Qty.Text);
        price_detail.labor_id = 1;
        price_detail.is_direct = Convert.ToInt32(ddlCustomizeDirect.SelectedValue);

        if (Convert.ToInt32(ddlO_Direct.SelectedValue) == 1)
        {
            price_detail.total_retail_price = nTotalPrice;
            price_detail.total_direct_price = 0;
        }
        else
        {
            price_detail.total_retail_price = 0;
            price_detail.total_direct_price = nTotalPrice;
        }
        hdnItemCnt.Value = GetSerial().ToString();
        string strOtherCount = GetSerial_other(Convert.ToInt32(price_detail.item_id)).ToString();
        price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
        price_detail.other_item_cnt = Convert.ToInt32(strOtherCount);
        string str = "0";
        if (Convert.ToInt32(strOtherCount) > 9)
            str = price_detail.item_id + "." + strOtherCount;
        else
            str = price_detail.item_id + ".0" + strOtherCount;
        price_detail.section_serial = Convert.ToDecimal(str);
        price_detail.short_notes = txtCustomezeNotes.Text;
        price_detail.create_date = DateTime.Now;
        price_detail.last_update_date = DateTime.Now;
        price_detail.pricing_type = "A";
        //string strFileName = "";
        //if (Session["FileName"] != null)
        //    strFileName = Session["FileName"].ToString();
        //price_detail.upload_file_path = strFileName;
        price_detail.execution_unit = 0;
        price_detail.sort_id = 0;
        price_detail.is_mandatory = false;
        price_detail.is_CommissionExclude = IsCommissionExclude;

        _db.pricing_details.InsertOnSubmit(price_detail);
        _db.SubmitChanges();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to the Estimate, select another item or Location/Section");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to Estimate, select another item or Location/Section");
        txtCustomizeItemName.Text = "";
        txtCustomizeUOM.Text = "";
        txtCustomizeCode.Text = "";
        txtCustomizeUnitPrice.Text = "";
        txtCustomizeExtPrice.Text = "";
        txtCustomezeNotes.Text = "";
        DataTable dtItem = new DataTable();
        DataTable dtItemDirect = new DataTable();
        if (Session["Item_list_Direct"] != null)
            dtItemDirect = (DataTable)Session["Item_list_Direct"];
        else
            dtItemDirect = LoadItemTable();

        if (Session["Item_list"] != null)
            dtItem = (DataTable)Session["Item_list"];
        else
            dtItem = LoadItemTable();

        DataRow drNew = null;

        // Cost
        decimal dOrginalCost = 0;
        decimal dOrginalTotalCost = 0;
        decimal dLaborTotal = 0;
        decimal dLineItemTotal = 0;

        decimal dTPrice = 0;

        string sItemName = price_detail.item_name.ToString();

        decimal dItemCost = Convert.ToDecimal(price_detail.item_cost);
        decimal dRetail_multiplier = Convert.ToDecimal(price_detail.retail_multiplier);
        decimal dQuantity = Convert.ToDecimal(price_detail.quantity);
        decimal dLabor_rate = Convert.ToDecimal(price_detail.labor_rate);
        if (price_detail.is_direct == 1)
            dTPrice = Convert.ToDecimal(price_detail.total_retail_price);
        else
            dTPrice = Convert.ToDecimal(price_detail.total_direct_price);

       
            if (dRetail_multiplier > 0)
            {

                dOrginalCost = (dItemCost / dRetail_multiplier);
            }
            else
            {
                dOrginalCost = dItemCost;
            }
        

        dOrginalTotalCost = dOrginalCost * dQuantity;
        dLaborTotal = dLabor_rate * dQuantity;
        dLineItemTotal = dOrginalTotalCost + dLaborTotal;

        if (price_detail.is_direct == 1)
            drNew = dtItem.NewRow();
        else
            drNew = dtItemDirect.NewRow();
        drNew["pricing_id"] = price_detail.pricing_id;
        drNew["item_id"] = price_detail.item_id;
        drNew["labor_id"] = price_detail.labor_id;
        drNew["section_serial"] = price_detail.section_serial;
        drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
        drNew["section_name"] = price_detail.section_name;
        drNew["item_name"] = price_detail.item_name;
        drNew["measure_unit"] = price_detail.measure_unit;
        drNew["item_cost"] = price_detail.item_cost;
        drNew["total_retail_price"] = price_detail.total_retail_price;
        drNew["total_direct_price"] = price_detail.total_direct_price;
        drNew["minimum_qty"] = price_detail.minimum_qty;
        drNew["quantity"] = price_detail.quantity;
        drNew["retail_multiplier"] = price_detail.retail_multiplier;
        drNew["labor_rate"] = price_detail.labor_rate;
        drNew["short_notes"] = price_detail.short_notes;
        drNew["tmpCol"] = "";
        drNew["pricing_type"] = "A";
        drNew["is_mandatory"] = price_detail.is_mandatory;
        drNew["create_date"] = price_detail.create_date;
        drNew["last_update_date"] = price_detail.last_update_date;
        drNew["customer_id"] = price_detail.customer_id;
        drNew["section_level"] = price_detail.section_level;
        drNew["location_id"] = price_detail.location_id;
        drNew["sales_person_id"] = price_detail.sales_person_id;
        drNew["client_id"] = price_detail.client_id;

        drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
        drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
        drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total


        if (price_detail.is_direct == 1)
            dtItem.Rows.Add(drNew);
        else
            dtItemDirect.Rows.Add(drNew);
        Session.Add("Item_list", dtItem);
        Session.Add("Item_list_Direct", dtItemDirect);
        hdnSortDesc.Value = "1";
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        // Session.Remove("FileName");

    }

}


