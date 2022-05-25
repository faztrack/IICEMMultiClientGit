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

public partial class changeorder_pricing : System.Web.UI.Page
{
    public static string strDetailsFull = "";
    private double subtotal = 0.0;
    private double grandtotal = 0.0;
    string strDetails = "";

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    private double subtotalCost = 0.0;
    private double grandtotalCost = 0.0;
    private double subtotal_diectCost = 0.0;
    private double grandtotal_directCost = 0.0;
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
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            if (Page.User.IsInRole("admin022") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            Session.Remove("Main_list_Direct");
            Session.Remove("Main_list");
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            Session.Remove("gspSearch");

            tdlOther.Visible = false;
            tdlCustomizePrice.Visible = false;
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            int nChEstid = Convert.ToInt32(Request.QueryString.Get("coestid"));
            hdnChEstId.Value = nChEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                hdnClientId.Value = cust.client_id.ToString();

                if (_db.sales_persons.Any(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                    chkCustDisp.Checked = Convert.ToBoolean(cus_est.IsCustDisplay);

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                }
                // Get Change Order Info
                if (Convert.ToInt32(hdnChEstId.Value) > 0)
                {
                    changeorder_estimate cho = new changeorder_estimate();
                    cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

                    lblChangeOrderName.Text = cho.changeorder_name;
                    lblExistingChangeOrderName.Text = cho.changeorder_name;
                    //ddlStatus.SelectedValue = cho.change_order_status_id.ToString();
                    //ddlChangeOrderType.SelectedValue = cho.change_order_type_id.ToString();
                    //txtComments.Text = cho.comments;
                }
                // Get Change Order Locations
                var item = from loc in _db.locations
                           join cl in _db.changeorder_locations on loc.location_id equals cl.location_id
                           where cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)
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

                // Get Change Order Sections

                var section = from sec in _db.sectioninfos
                              join cs in _db.changeorder_sections on sec.section_id equals cs.section_id
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                              orderby sec.section_name
                              select new SectionInfo()
                              {
                                  section_id = (int)cs.section_id,
                                  section_name = sec.section_name
                              };
                DataTable dtSectionId = csCommonUtility.LINQToDataTable(section);
                ddlSections.DataSource = section;
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
                //DataTable dtNew = LoadFullSection();
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

                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {
                    rdoSort.Visible = false;

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
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "lblChangeOrderName", "lnkAddMoreLocation", "ddlCustomerLocations", "lnkAddMoreSections", "ddlSections", "chkPMDisplay", "chkCustDisp", "pnlUpdateCoEstimate", "lnkUpdateCoEstimate" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Delete", "grdGrouping_Label1", "grdGrouping_lnkMove1", "Undo", "Edit", "Update" });
        }

    }



    protected void lnkAddMoreLocation_Click(object sender, EventArgs e)
    {
        Response.Redirect("change_order_locations.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&clid=1");
    }
    protected void lnkAddMoreSections_Click(object sender, EventArgs e)
    {
        Response.Redirect("change_order_sections.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&csid=1");
    }




    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    private void LoadTree(int nSectionLevel)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE  section_level=" + nSectionLevel + " AND client_id= "+ hdnClientId.Value + " AND is_disable = 0 and is_active=1 ORDER BY section_name";
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
        string strQ = " SELECT * FROM sectioninfo WHERE  client_id= "+ hdnClientId.Value + " AND is_disable = 0 and is_active=1 AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id =  " + hdnClientId.Value + ") ORDER BY section_name ";
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
    public void BindGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo sinfo = new sectioninfo();
        sinfo = _db.sectioninfos.Single(c => c.section_id == Convert.ToInt32(hdnSectionId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        var item = from it in _db.item_prices
                   join si in _db.sectioninfos on it.item_id equals si.section_id
                   where si.is_active == true && si.is_disable == false && si.parent_id == Convert.ToInt32(trvSection.SelectedValue)
                   select new ItemPriceModel()
                   {
                       item_id = (int)it.item_id,
                       section_name = si.section_name,
                       measure_unit = it.measure_unit,
                       item_cost = (decimal)it.item_cost * (decimal)it.retail_multiplier,
                       minimum_qty = (decimal)it.minimum_qty,
                       retail_multiplier = (decimal)it.retail_multiplier,
                       labor_rate = (decimal)it.labor_rate,
                       section_serial = (decimal)si.section_serial,
                       ext_item_cost = (((decimal)it.item_cost + (decimal)it.labor_rate) * (decimal)it.retail_multiplier) * (decimal)it.minimum_qty,
                       labor_id = (int)it.labor_id,
                       LaborUnitCost = (decimal)it.labor_rate * (decimal)it.retail_multiplier
                   };
        grdItemPrice.DataSource = item;
        grdItemPrice.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier" };
        grdItemPrice.DataBind();
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



    public void BindSelectedItemGrid()
    {

        Session.Remove("Main_list");
        Session.Remove("Item_list");

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id from co_pricing_master  INNER JOIN location on location.location_id = co_pricing_master.location_id where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + " GROUP BY co_pricing_master.location_id,location.location_name order by sort_id asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        DataTable dtMain = SessionInfo.LINQToDataTable(mList);
        Session.Add("Main_list", dtMain);
        grdGrouping.DataSource = mList;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

    }


    public void BindSelectedItemGrid_Direct()
    {
        Session.Remove("Main_list_Direct");
        Session.Remove("Item_list_Direct");
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id from co_pricing_master  INNER JOIN location on location.location_id = co_pricing_master.location_id where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + " GROUP BY co_pricing_master.location_id,location.location_name order by sort_id asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        DataTable dtMainDirect = SessionInfo.LINQToDataTable(mList);
        Session.Add("Main_list_Direct", dtMainDirect);
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }

    protected void ddlSections_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSections.ID, ddlSections.GetType().Name, "SelectedIndexChanged"); 
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

    //protected void ddlSections_SelectedIndexChanged(object sender, EventArgs e)
    //{





    //    if (ddlSections.SelectedItem.Text != "Select Section")
    //    {
    //        lblSelectLocation.Text = "";
    //        lblResult1.Text = "";
    //        lblAdd.Text = "";
    //        lblParent.Text = "";
    //        LoadTree(Convert.ToInt32(ddlSections.SelectedValue));
    //        grdItemPrice.Visible = false;
    //        tdlOther.Visible = false;
    //        GetItemId_other();
    //        tblPricingWrapper.Visible = true;

    //    }
    //    else
    //    {
    //        lblResult1.Text = "";
    //        trvSection.Nodes.Clear();
    //        tblPricingWrapper.Visible = false;

    //    }
    //}
    protected void trvSection_SelectedNodeChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, trvSection.ID, trvSection.GetType().Name, "SelectedNodeChanged"); 
        hdnSectionId.Value = trvSection.SelectedValue;
        grdItemPrice.Visible = true;
        tdlOther.Visible = false;
        tdlCustomizePrice.Visible = true;
        BindGrid();
        lblParent.Text = GetItemDetials_forBreadCome(Convert.ToInt32(trvSection.SelectedValue));

    }



    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        lblMessage.Text = "";
        txtNewChangeOrderName.Text = "";
        modUpdateCoEstimate.Hide();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblMessage.Text = "";
        if (txtNewChangeOrderName.Text.Trim() == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("New Changeorder Name is required field");
            modUpdateCoEstimate.Show();
            return;
        }

        string strNewName = txtNewChangeOrderName.Text.Trim();
        DataClassesDataContext _db = new DataClassesDataContext();
        changeorder_estimate co_est = new changeorder_estimate();
        int ncid = Convert.ToInt32(hdnCustomerId.Value);
        int nestId = Convert.ToInt32(hdnEstimateId.Value);
        int coId = Convert.ToInt32(hdnChEstId.Value);
        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnChEstId.Value) > 0)
            co_est = _db.changeorder_estimates.Single(ce => ce.customer_id == ncid && ce.chage_order_id == coId && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(hdnClientId.Value));

        co_est.changeorder_name = strNewName;

        _db.SubmitChanges();


        lblMessage.Text = "";
        txtNewChangeOrderName.Text = "";
        modUpdateCoEstimate.Hide();

        Response.Redirect("changeorder_pricing.aspx?eid=" + nestId + "&cid=" + ncid + "&coestid=" + coId);
    }

    protected void grdSelectedItem1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        lblResult1.Text = "";
        try
        {
            GridView gv1 = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblshort_notes = (Label)e.Row.FindControl("lblshort_notes");
                TextBox txtshort_notes = (TextBox)e.Row.FindControl("txtshort_notes");
                LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
                string str = lblshort_notes.Text.Replace("&nbsp;", "");

                LinkButton btnEdit = (LinkButton)e.Row.Cells[13].Controls[0];
                btnEdit.Visible = false;

                if (str != "" && str.Length > 25)
                {
                    txtshort_notes.Text = str;
                    lblshort_notes.Text = str.Substring(0, 25) + "...";
                    lblshort_notes.ToolTip = str;
                    lnkOpen.Visible = true;

                }
                else
                {
                    txtshort_notes.Text = str;
                    lblshort_notes.Text = str;
                    lnkOpen.Visible = false;

                }

                int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
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
                    Label lblTotalPrice = (Label)e.Row.Cells[8].FindControl("lblTotal_price");
                    lblTotalPrice.Text = "0.00";
                    LinkButton btn = (LinkButton)e.Row.Cells[14].Controls[0];
                    btn.Text = "Undo";
                    btn.CommandName = "Undo";
                }
                else if (nItemStatusId == 3)
                {
                    e.Row.Attributes.CssStyle.Add("color", "green");
                    btnEdit.Visible = true;
                    //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
                }
                //if (chkCustDisp.Checked)
                //{
                //    gv1.Columns[5].Visible = false;
                //    gv1.Columns[7].Visible = false;
                //    gv1.Columns[8].Visible = false;

                //    gv1.Columns[3].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                //    gv1.Columns[4].ItemStyle.Width = new Unit(40, UnitType.Percentage);
                //}
                if (chkCustDisp.Checked)
                {
                    gv1.Columns[6].Visible = false;
                    gv1.Columns[7].Visible = false;
                    gv1.Columns[8].Visible = false;
                    gv1.Columns[9].Visible = false;
                    gv1.Columns[10].Visible = false;
                    gv1.Columns[11].Visible = false;
                    gv1.Columns[2].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                    gv1.Columns[4].ItemStyle.Width = new Unit(41, UnitType.Percentage);
                }
                if (chkPMDisplay.Checked)
                {
                    gv1.Columns[7].Visible = true;
                    gv1.Columns[9].Visible = true;
                    gv1.Columns[10].Visible = true;


                }
                else
                {
                    gv1.Columns[7].Visible = false;
                    gv1.Columns[9].Visible = false;
                    gv1.Columns[10].Visible = false;

                }
            }
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void grdSelectedItem1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";

        GridView grdSelectedItem = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem.DataKeys[e.RowIndex].Values[0].ToString();
        co_pricing_master cpl = new co_pricing_master();
        cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
        string strQ = "";

        DataTable dtItem = (DataTable)Session["Item_list"];
        DataTable dtMain = (DataTable)Session["Main_list"];
        bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(Convert.ToInt32(hdnPricingId.Value))).Count() > 0;
        if (cpl.item_status_id == 3)
        {
            strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);

            if (Iexists)
            {
                var rows = dtItem.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                foreach (var row in rows)
                    row.Delete();
            }
            Session.Add("Item_list", dtItem);
        }
        else
        {
            strQ = "UPDATE co_pricing_master SET item_status_id=2,prev_total_price=" + cpl.total_retail_price + ",total_retail_price = 0 WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            if (Iexists)
            {
                var rows = dtItem.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                foreach (var row in rows)
                {
                    row["item_status_id"] = 2;
                }

            }
            Session.Add("Item_list", dtItem);
        }

        _db.ExecuteCommand(strQ, string.Empty);

        subtotal = 0.0;
        grandtotal = 0.0;
        subtotal_diect = 0.0;
        grandtotal_direct = 0.0;
        hdnSortDesc.Value = "0";
        grdGrouping.DataSource = dtMain;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

        //BindSelectedItemGrid();
        //BindSelectedItemGrid_Direct();
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
        lblResult1.Text = "Item deleted successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;
        lblAdd.Text = "Item deleted successfully";
        lblAdd.ForeColor = System.Drawing.Color.Green;

    }


    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    protected string GetTotalPrice()
    {
        return "Total: " + grandtotal.ToString("c");
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
        return "Total: " + grandtotal_direct.ToString("c");
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
            if (rdoSort.SelectedValue == "1")
            {
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

            }

            DataTable dtItem = (DataTable)Session["Item_list"];
            DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
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
        if (rdoSort.SelectedValue == "1")
        {
            //&& p.item_status_id != 2
            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level) && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value)
                               orderby p.section_level ascending
                               //&& p.location_id == colId

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = lc.location_name,
                                   section_name = p.section_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   tmpCol = string.Empty,
                                   last_update_date = (DateTime)p.last_update_date,
                                   is_direct = (int)p.is_direct,
                                   section_level = (int)p.section_level,
                                   location_id = (int)p.location_id,
                                   unit_cost = 0,
                                   total_unit_cost = 0,
                                   total_labor_cost = 0
                               };
            DataTable dt = SessionInfo.LINQToDataTable(price_detail);
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
                dr["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total

            }
            if (nDirectId == 1)
                Session.Add("Item_list", dt);
            else
                Session.Add("Item_list_Direct", dt);
            //if (hdnSortDesc.Value == "1")
            //    grd.DataSource = price_detail.ToList().OrderByDescending(c => c.last_update_date);
            //else
            //    grd.DataSource = price_detail.ToList();
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

            //if (hdnSortDesc.Value == "1")
            //    grd.DataSource = price_detail.ToList().OrderByDescending(c => c.last_update_date);
            //else
            //    grd.DataSource = price_detail.ToList();
            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
            grd.DataBind();
        }
        else
        {
            //&& p.item_status_id != 2
            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level)
                                       && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value)
                               orderby lc.location_name ascending
                               //&& p.section_level == colId

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = p.section_name,
                                   section_name = lc.location_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   tmpCol = string.Empty,
                                   last_update_date = (DateTime)p.last_update_date,
                                   is_direct = (int)p.is_direct,
                                   section_level = (int)p.section_level,
                                   location_id = (int)p.location_id,
                                   unit_cost = 0,
                                   total_unit_cost = 0,
                                   total_labor_cost = 0
                               };


            DataTable dt = SessionInfo.LINQToDataTable(price_detail);
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
                dr["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total

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
            if (hdnSortDesc.Value == "1")
                dv.Sort = "last_update_date DESC";

            //if (hdnSortDesc.Value == "1")
            //    grd.DataSource = price_detail.ToList().OrderByDescending(c => c.last_update_date);
            //else
            //    grd.DataSource = price_detail.ToList();
            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
            grd.DataBind();
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged"); 
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

        csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Delete", "grdGrouping_Label1", "grdGrouping_lnkMove1", "Undo", "Edit", "Update" });
    }
    protected void grdSelectedItem2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        lblSelectLocation.Text = "";
        lblAdd.Text = "";
        lblMessage.Text = "";
        lblResult1.Text = "";
        DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

        GridView grdSelectedItem2 = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem2.DataKeys[e.RowIndex].Values[0].ToString();
        co_pricing_master cpl = new co_pricing_master();
        cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
        string strQ = "";
        bool Iexists = dtItemDirect.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(Convert.ToInt32(hdnPricingId.Value))).Count() > 0;
        if (cpl.item_status_id == 3)
        {
            strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            {
                if (Iexists)
                {
                    var rows = dtItemDirect.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                    foreach (var row in rows)
                        row.Delete();
                }
                Session.Add("Item_list_Direct", dtItemDirect);
            }
        }
        else
        {
            strQ = "UPDATE co_pricing_master SET item_status_id=2,prev_total_price=" + cpl.total_direct_price + ",total_direct_price=0 WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            if (Iexists)
            {
                var rows = dtItemDirect.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                foreach (var row in rows)
                {
                    row["item_status_id"] = 2;
                }

            }
            Session.Add("Item_list_Direct", dtItemDirect);
        }

        _db.ExecuteCommand(strQ, string.Empty);

        subtotal = 0.0;
        grandtotal = 0.0;
        subtotal_diect = 0.0;
        grandtotal_direct = 0.0;
        hdnSortDesc.Value = "0";
        grdGroupingDirect.DataSource = dtMainDirect;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

        //BindSelectedItemGrid_Direct();
        //BindSelectedItemGrid();
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
        lblResult1.Text = "Item deleted successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;
        lblAdd.Text = "Item deleted successfully";
        lblAdd.ForeColor = System.Drawing.Color.Green;
    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv2 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            Label lblshort_notes1 = (Label)e.Row.FindControl("lblshort_notes1");
            TextBox txtshort_notes1 = (TextBox)e.Row.FindControl("txtshort_notes1");
            LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
            string str = lblshort_notes1.Text.Replace("&nbsp;", "");
            LinkButton btnEdit = (LinkButton)e.Row.Cells[13].Controls[0];
            btnEdit.Visible = false;
            if (str != "" && str.Length > 25)
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str.Substring(0, 25) + "...";
                lblshort_notes1.ToolTip = str;
                lnkOpen1.Visible = true;

            }
            else
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str;
                lnkOpen1.Visible = false;

            }
            int nItemStatusId = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[1]);
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
                Label lblTotalPrice2 = (Label)e.Row.Cells[8].FindControl("lblTotal_price2");
                lblTotalPrice2.Text = "0.00";

                LinkButton btn = (LinkButton)e.Row.Cells[14].Controls[0];
                btn.Text = "Undo";
                btn.CommandName = "Undo";
            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                btnEdit.Visible = true;

            }
            //if (chkCustDisp.Checked)
            //{
            //    gv2.Columns[5].Visible = false;
            //    gv2.Columns[7].Visible = false;
            //    gv2.Columns[8].Visible = false;

            //    gv2.Columns[3].ItemStyle.Width = new Unit(20, UnitType.Percentage);
            //    gv2.Columns[4].ItemStyle.Width = new Unit(40, UnitType.Percentage);
            //}
            if (chkCustDisp.Checked)
            {
                gv2.Columns[6].Visible = false;
                gv2.Columns[7].Visible = false;
                gv2.Columns[8].Visible = false;
                gv2.Columns[9].Visible = false;
                gv2.Columns[10].Visible = false;
                gv2.Columns[11].Visible = false;
                gv2.Columns[2].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                gv2.Columns[4].ItemStyle.Width = new Unit(41, UnitType.Percentage);
            }
            if (chkPMDisplay.Checked)
            {
                gv2.Columns[7].Visible = true;
                gv2.Columns[9].Visible = true;
                gv2.Columns[10].Visible = true;


            }
            else
            {
                gv2.Columns[7].Visible = false;
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
            LinkButton lnkMove2 = (LinkButton)e.Row.FindControl("lnkMove2");
            if (rdoSort.SelectedValue == "1")
            {
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
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
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
    public string GetItemDetialsForUpdateItem(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.parent_id > 0 && c.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
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
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
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

        si = _db.sectioninfos.SingleOrDefault(c => c.section_level == section_level && c.parent_id == 0 && c.client_id == Convert.ToInt32(hdnClientId.Value));

        if (si != null)
            str = si.section_name;

        return str;
    }
    private int GetSerialMultiple(int sectionLvel)
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.co_pricing_masters
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.section_level == sectionLvel
                      select pd.item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }
    private int GetSerial()
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.co_pricing_masters
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.section_level == Convert.ToInt32(hdnSectionLevel.Value)
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
        var result = (from pd in _db.co_pricing_masters
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.item_id == nOtherId && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.section_level == Convert.ToInt32(hdnSectionLevel.Value)
                      select pd.other_item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }
    private DataTable LoadMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("colId", typeof(int));
        table.Columns.Add("colName", typeof(string));
        return table;
    }

    private DataTable LoadItemTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("co_pricing_list_id", typeof(int));
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
        table.Columns.Add("item_status_id", typeof(int));
        table.Columns.Add("tmpCol", typeof(string));
        table.Columns.Add("last_update_date", typeof(DateTime));
        table.Columns.Add("is_direct", typeof(int));
        table.Columns.Add("section_level", typeof(int));
        table.Columns.Add("location_id", typeof(int));
        table.Columns.Add("unit_cost", typeof(decimal));
        table.Columns.Add("total_unit_cost", typeof(decimal));
        table.Columns.Add("total_labor_cost", typeof(decimal));

        return table;
    }

    protected void grdItemPrice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            lblSelectLocation.Text = "";
            lblAdd.Text = "";
            lblMessage.Text = "";
            lblResult1.Text = "";
            string strPriceId = "0";
            decimal ditem_cost = 0;
            decimal dlabor_rate = 0;
            decimal dretail_multiplier = 0;
            string smeasure_unit = "";
            decimal dminimum_qty = 0;

            DataClassesDataContext _db = new DataClassesDataContext();
            int index = Convert.ToInt32(e.CommandArgument);
            hdnSectionId.Value = grdItemPrice.Rows[index].Cells[1].Text;
            TextBox txtQty = (TextBox)grdItemPrice.Rows[index].FindControl("txtQty");
            TextBox txtShortNote = (TextBox)grdItemPrice.Rows[index].FindControl("txtShortNote");
            DropDownList ddlLabor = (DropDownList)grdItemPrice.Rows[index].FindControl("ddlLabor");
            DropDownList ddlDirect = (DropDownList)grdItemPrice.Rows[index].FindControl("ddlDirect");
            strPriceId = grdItemPrice.DataKeys[index].Values[0].ToString();

            dlabor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[index].Values[1].ToString());
            dretail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[index].Values[2].ToString());
            smeasure_unit = grdItemPrice.Rows[index].Cells[7].Text;
            ditem_cost = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[8].Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
            dminimum_qty = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text);
            hdnSectionId.Value = strPriceId;



            if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
            {
                lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
                lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
                ddlCustomerLocations.Focus();
                return;
            }
            decimal nTotalPrice = 0;
            if (txtQty.Text.Trim() == "")
            {
                lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
                lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
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
                    lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid  quantity.");
                    lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Invalid  quantity.");
                    return;
                }
            }
            if (Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text) > Convert.ToDecimal(txtQty.Text))
            {
                lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
                lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
                return;
            }
            if (Convert.ToDecimal(grdItemPrice.Rows[index].Cells[10].Text) <= Convert.ToDecimal(txtQty.Text))
            {
                decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[index].Cells[8].Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
                decimal nQty = Convert.ToDecimal(txtQty.Text);

                decimal nLaborRate = dlabor_rate * dretail_multiplier;
                nLaborRate = nLaborRate * nQty;

                if (ddlLabor.SelectedValue == "2")
                    nTotalPrice = nCost * nQty + nLaborRate;
                else
                    nTotalPrice = nCost * nQty;
            }
            sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(hdnClientId.Value));
            hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
            hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
            hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
            bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);

            co_pricing_master co_price_detail = new co_pricing_master();

            co_price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString();
            co_price_detail.measure_unit = smeasure_unit;
            co_price_detail.minimum_qty = dminimum_qty;
            co_price_detail.retail_multiplier = dretail_multiplier;
            co_price_detail.labor_rate = dlabor_rate;
            co_price_detail.item_cost = ditem_cost;



            co_price_detail.client_id = Convert.ToInt32(hdnClientId.Value);
            co_price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            co_price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
            //co_price_detail.change_order_id = Convert.ToInt32(hdnChEstId.Value);
            co_price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
            co_price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            co_price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
            co_price_detail.item_id = Convert.ToInt32(hdnSectionId.Value);
            co_price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
            co_price_detail.quantity = Convert.ToDecimal(txtQty.Text);
            co_price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
            co_price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

            if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
            {
                co_price_detail.total_retail_price = nTotalPrice;
                co_price_detail.total_direct_price = 0;
            }
            else
            {
                co_price_detail.total_retail_price = 0;
                co_price_detail.total_direct_price = nTotalPrice;
            }

            if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
                co_price_detail.labor_rate = 0;
            co_price_detail.section_serial = Convert.ToDecimal(hdnSectionSerial.Value);
            hdnItemCnt.Value = GetSerial().ToString();
            co_price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
            co_price_detail.other_item_cnt = 0;
            co_price_detail.item_status_id = 3;
            co_price_detail.short_notes = txtShortNote.Text;
            co_price_detail.create_date = DateTime.Now;
            co_price_detail.last_update_date = DateTime.Now;
            co_price_detail.prev_total_price = nTotalPrice;
            co_price_detail.execution_unit = 0;
            co_price_detail.week_id = 0;
            co_price_detail.is_complete = false;
            co_price_detail.schedule_note = "";
            co_price_detail.sort_id = 0;
            co_price_detail.CalEventId = 0;
            co_price_detail.is_CommissionExclude = IsCommissionExclude;

            _db.co_pricing_masters.InsertOnSubmit(co_price_detail);
            _db.SubmitChanges();
            lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
            lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
            hdnSortDesc.Value = "1";
            // Cost
            decimal dOrginalCost = 0;
            decimal dOrginalTotalCost = 0;
            decimal dLaborTotal = 0;
            decimal dLineItemTotal = 0;

            decimal dTPrice = 0;

            string sItemName = co_price_detail.item_name.ToString();

            decimal dItemCost = Convert.ToDecimal(co_price_detail.item_cost);
            decimal dRetail_multiplier = Convert.ToDecimal(co_price_detail.retail_multiplier);
            decimal dQuantity = Convert.ToDecimal(co_price_detail.quantity);
            decimal dLabor_rate = Convert.ToDecimal(co_price_detail.labor_rate);
            if (co_price_detail.is_direct == 1)
                dTPrice = Convert.ToDecimal(co_price_detail.total_retail_price);
            else
                dTPrice = Convert.ToDecimal(co_price_detail.total_direct_price);

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
            if (rdoSort.SelectedValue == "1")
            {
                //strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name
                DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
                DataTable dtMain = (DataTable)Session["Main_list"];
                DataTable dtItem = (DataTable)Session["Item_list"];
                DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

                if ((dtMain == null) || (dtMain.Rows.Count == 0))
                {
                    dtMain = LoadMasterTable();
                }
                if ((dtMainDirect == null) || (dtMainDirect.Rows.Count == 0))
                {
                    dtMainDirect = LoadMasterTable();
                }
                if (dtItemDirect == null)
                {
                    dtItemDirect = LoadItemTable();
                }
                if (dtItem == null)
                {
                    dtItem = LoadItemTable();
                }
                if (co_price_detail.is_direct == 1)
                {
                    bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                    if (!exists)
                    {
                        DataRow drNew = dtMain.NewRow();
                        drNew["colId"] = co_price_detail.location_id;
                        drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                        dtMain.Rows.Add(drNew);

                    }
                }
                else
                {
                    bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                    if (!exists)
                    {
                        DataRow drNew = dtMainDirect.NewRow();
                        drNew["colId"] = co_price_detail.location_id;
                        drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                        dtMainDirect.Rows.Add(drNew);

                    }

                }
                bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
                if (!Iexists)
                {
                    DataRow drNew = null;
                    if (co_price_detail.is_direct == 1)
                        drNew = dtItem.NewRow();
                    else
                        drNew = dtItemDirect.NewRow();
                    drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                    drNew["item_id"] = co_price_detail.item_id;
                    drNew["labor_id"] = co_price_detail.labor_id;
                    drNew["section_serial"] = co_price_detail.section_serial;
                    drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
                    drNew["section_name"] = co_price_detail.section_name;

                    drNew["item_name"] = co_price_detail.item_name;
                    drNew["measure_unit"] = co_price_detail.measure_unit;
                    drNew["item_cost"] = co_price_detail.item_cost;
                    drNew["total_retail_price"] = co_price_detail.total_retail_price;
                    drNew["total_direct_price"] = co_price_detail.total_direct_price;
                    drNew["minimum_qty"] = co_price_detail.minimum_qty;
                    drNew["quantity"] = co_price_detail.quantity;
                    drNew["labor_rate"] = co_price_detail.labor_rate;
                    drNew["short_notes"] = co_price_detail.short_notes;
                    drNew["item_status_id"] = co_price_detail.item_status_id;
                    drNew["tmpCol"] = "";
                    drNew["last_update_date"] = co_price_detail.last_update_date;
                    drNew["is_direct"] = co_price_detail.is_direct;
                    drNew["section_level"] = co_price_detail.section_level;
                    drNew["location_id"] = co_price_detail.location_id;
                    drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                    drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                    drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                    if (co_price_detail.is_direct == 1)
                        dtItem.Rows.Add(drNew);
                    else
                        dtItemDirect.Rows.Add(drNew);
                    Session.Add("Item_list", dtItem);
                    Session.Add("Item_list_Direct", dtItemDirect);
                }
                grdGrouping.DataSource = dtMain;
                grdGrouping.DataKeyNames = new string[] { "colId" };
                grdGrouping.DataBind();

                grdGroupingDirect.DataSource = dtMainDirect;
                grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                grdGroupingDirect.DataBind();
            }
            else
            {
                DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
                DataTable dtMain = (DataTable)Session["Main_list"];
                DataTable dtItem = (DataTable)Session["Item_list"];
                DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
                if (dtMain == null)
                {
                    dtMain = LoadMasterTable();
                }
                if (dtMainDirect == null)
                {
                    dtMainDirect = LoadMasterTable();
                }
                if (dtItemDirect == null)
                {
                    dtItemDirect = LoadItemTable();
                }
                if (dtItem == null)
                {
                    dtItem = LoadItemTable();
                }
                if (co_price_detail.is_direct == 1)
                {
                    bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                    if (!exists)
                    {
                        DataRow drNew = dtMain.NewRow();
                        drNew["colId"] = co_price_detail.section_level;
                        drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                        dtMain.Rows.Add(drNew);

                    }
                }
                else
                {
                    bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                    if (!exists)
                    {
                        DataRow drNew = dtMainDirect.NewRow();
                        drNew["colId"] = co_price_detail.section_level;
                        drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                        dtMainDirect.Rows.Add(drNew);

                    }

                }
                bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
                if (!Iexists)
                {
                    DataRow drNew = null;
                    if (co_price_detail.is_direct == 1)
                        drNew = dtItem.NewRow();
                    else
                        drNew = dtItemDirect.NewRow();
                    drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                    drNew["item_id"] = co_price_detail.item_id;
                    drNew["labor_id"] = co_price_detail.labor_id;
                    drNew["section_serial"] = co_price_detail.section_serial;

                    drNew["location_name"] = co_price_detail.section_name;//ddlCustomerLocations.SelectedItem.Text;
                    drNew["section_name"] = ddlCustomerLocations.SelectedItem.Text;//co_price_detail.section_name;
                    drNew["item_name"] = co_price_detail.item_name;
                    drNew["measure_unit"] = co_price_detail.measure_unit;
                    drNew["item_cost"] = co_price_detail.item_cost;
                    drNew["total_retail_price"] = co_price_detail.total_retail_price;
                    drNew["total_direct_price"] = co_price_detail.total_direct_price;
                    drNew["minimum_qty"] = co_price_detail.minimum_qty;
                    drNew["quantity"] = co_price_detail.quantity;
                    drNew["labor_rate"] = co_price_detail.labor_rate;
                    drNew["short_notes"] = co_price_detail.short_notes;
                    drNew["item_status_id"] = co_price_detail.item_status_id;
                    drNew["tmpCol"] = "";
                    drNew["last_update_date"] = co_price_detail.last_update_date;
                    drNew["is_direct"] = co_price_detail.is_direct;
                    drNew["section_level"] = co_price_detail.section_level;
                    drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                    drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                    drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                    if (co_price_detail.is_direct == 1)
                        dtItem.Rows.Add(drNew);
                    else
                        dtItemDirect.Rows.Add(drNew);


                    Session.Add("Item_list", dtItem);
                    Session.Add("Item_list_Direct", dtItemDirect);
                }

                grdGrouping.DataSource = dtMain;
                grdGrouping.DataKeyNames = new string[] { "colId" };
                grdGrouping.DataBind();

                grdGroupingDirect.DataSource = dtMainDirect;
                grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                grdGroupingDirect.DataBind();

            }



            //BindSelectedItemGrid();
            //BindSelectedItemGrid_Direct();

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
    protected void grdSelectedItem1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        DataTable dtMain = (DataTable)Session["Main_list"];
        DataTable dtItem = (DataTable)Session["Item_list"];
        GridView grdSelectedItem = (GridView)sender;
        if (e.CommandName == "Undo")
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            hdnPricingId.Value = grdSelectedItem.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0].ToString();
            co_pricing_master cpl = new co_pricing_master();
            cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
            string strQ = "UPDATE co_pricing_master SET item_status_id=1,total_retail_price=" + cpl.prev_total_price + " WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            //string strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(Convert.ToInt32(hdnPricingId.Value))).Count() > 0;
            if (Iexists)
            {
                var rows = dtItem.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                foreach (var row in rows)
                {
                    row["item_status_id"] = 1;
                    row["total_retail_price"] = cpl.prev_total_price;
                }

            }
            Session.Add("Item_list", dtItem);


            subtotal = 0.0;
            grandtotal = 0.0;
            subtotal_diect = 0.0;
            grandtotal_direct = 0.0;
            hdnSortDesc.Value = "0";
            grdGrouping.DataSource = dtMain;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();
            //BindSelectedItemGrid();
            //BindSelectedItemGrid_Direct();
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
            lblResult1.Text = "";
            lblAdd.Text = "";
        }
    }
    protected void grdSelectedItem2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
        GridView grdSelectedItem2 = (GridView)sender;
        if (e.CommandName == "Undo")
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            hdnPricingId.Value = grdSelectedItem2.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0].ToString();
            co_pricing_master cpl = new co_pricing_master();
            cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
            string strQ = "UPDATE co_pricing_master SET item_status_id=1, total_direct_price=" + cpl.prev_total_price + " WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            bool Iexists = dtItemDirect.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(Convert.ToInt32(hdnPricingId.Value))).Count() > 0;
            if (Iexists)
            {
                var rows = dtItemDirect.Select("co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + "");
                foreach (var row in rows)
                {
                    row["item_status_id"] = 1;
                    row["total_direct_price"] = cpl.prev_total_price;
                }

            }
            Session.Add("Item_list_Direct", dtItemDirect);


            subtotal = 0.0;
            grandtotal = 0.0;
            subtotal_diect = 0.0;
            grandtotal_direct = 0.0;
            hdnSortDesc.Value = "0";
            grdGroupingDirect.DataSource = dtMainDirect;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
            //BindSelectedItemGrid();
            //BindSelectedItemGrid_Direct();
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
            lblResult1.Text = "";
            lblAdd.Text = "";
        }

    }

    protected void btnGotoWorkSheet_Click(object sender, EventArgs e)
    {
        Response.Redirect("change_order_worksheet.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
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
        if (txtOther.Text == "")
        {
            lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Item Name is required.");
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

        sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(hdnClientId.Value));
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
        co_pricing_master co_price_detail = new co_pricing_master();

        co_price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString() + "" + txtOther.Text + ">>Other";
        co_price_detail.measure_unit = txtO_Unit.Text;
        co_price_detail.minimum_qty = 1;
        co_price_detail.retail_multiplier = 0;
        co_price_detail.labor_rate = 0;
        co_price_detail.item_cost = nOtherUnitPrice;//Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
        co_price_detail.client_id = Convert.ToInt32(hdnClientId.Value);
        co_price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        co_price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        co_price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
        co_price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
        co_price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
        co_price_detail.item_id = Convert.ToInt32(hdnOtherId.Value);
        co_price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
        co_price_detail.quantity = nOtherQty; //Convert.ToDecimal(txtO_Qty.Text);
        co_price_detail.labor_id = 1;
        co_price_detail.is_direct = Convert.ToInt32(ddlO_Direct.SelectedValue);

        if (Convert.ToInt32(ddlO_Direct.SelectedValue) == 1)
        {
            co_price_detail.total_retail_price = Other_TotalPrice;
            co_price_detail.total_direct_price = 0;
        }
        else
        {
            co_price_detail.total_retail_price = 0;
            co_price_detail.total_direct_price = Other_TotalPrice;
        }
        hdnItemCnt.Value = GetSerial().ToString();
        string strOtherCount = GetSerial_other(Convert.ToInt32(co_price_detail.item_id)).ToString();
        co_price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
        co_price_detail.other_item_cnt = Convert.ToInt32(strOtherCount);
        string str = "0";
        if (Convert.ToInt32(strOtherCount) > 9)
            str = co_price_detail.item_id + "." + strOtherCount;
        else
            str = co_price_detail.item_id + ".0" + strOtherCount;
        co_price_detail.section_serial = Convert.ToDecimal(str);
        co_price_detail.item_status_id = 3;
        co_price_detail.short_notes = txtO_ShortNotes.Text;
        co_price_detail.create_date = DateTime.Now;
        co_price_detail.last_update_date = DateTime.Now;
        co_price_detail.prev_total_price = Other_TotalPrice;
        co_price_detail.execution_unit = 0;
        co_price_detail.week_id = 0;
        co_price_detail.is_complete = false;
        co_price_detail.schedule_note = "";
        co_price_detail.sort_id = 0;
        co_price_detail.CalEventId = 0;
        co_price_detail.is_CommissionExclude = IsCommissionExclude;

        _db.co_pricing_masters.InsertOnSubmit(co_price_detail);
        _db.SubmitChanges();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Other Item added to change order list, select another item or Location/Section");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Other Item added to change order list, select another item or Location/Section");
        txtO_Price.Text = "";
        txtO_Qty.Text = "";
        txtO_Unit.Text = "";
        txtOther.Text = "";
        // txtO_TotalPrice.Text = "";
        txtO_ShortNotes.Text = "";
        hdnSortDesc.Value = "1";
        // Cost
        decimal dOrginalCost = 0;
        decimal dOrginalTotalCost = 0;
        decimal dLaborTotal = 0;
        decimal dLineItemTotal = 0;

        decimal dTPrice = 0;

        string sItemName = co_price_detail.item_name.ToString();

        decimal dItemCost = Convert.ToDecimal(co_price_detail.item_cost);
        decimal dRetail_multiplier = Convert.ToDecimal(co_price_detail.retail_multiplier);
        decimal dQuantity = Convert.ToDecimal(co_price_detail.quantity);
        decimal dLabor_rate = Convert.ToDecimal(co_price_detail.labor_rate);
        if (co_price_detail.is_direct == 1)
            dTPrice = Convert.ToDecimal(co_price_detail.total_retail_price);
        else
            dTPrice = Convert.ToDecimal(co_price_detail.total_direct_price);

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

        if (rdoSort.SelectedValue == "1")
        {
            //strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name
            DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
            DataTable dtMain = (DataTable)Session["Main_list"];
            DataTable dtItem = (DataTable)Session["Item_list"];
            DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

            if ((dtMain == null) || (dtMain.Rows.Count == 0))
            {
                dtMain = LoadMasterTable();
            }
            if ((dtMainDirect == null) || (dtMainDirect.Rows.Count == 0))
            {
                dtMainDirect = LoadMasterTable();
            }
            if (dtItemDirect == null)
            {
                dtItemDirect = LoadItemTable();
            }
            if (dtItem == null)
            {
                dtItem = LoadItemTable();
            }
            if (co_price_detail.is_direct == 1)
            {
                bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMain.NewRow();
                    drNew["colId"] = co_price_detail.location_id;
                    drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                    dtMain.Rows.Add(drNew);

                }
            }
            else
            {
                bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMainDirect.NewRow();
                    drNew["colId"] = co_price_detail.location_id;
                    drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                    dtMainDirect.Rows.Add(drNew);

                }

            }
            bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
            if (!Iexists)
            {
                DataRow drNew = null;
                if (co_price_detail.is_direct == 1)
                    drNew = dtItem.NewRow();
                else
                    drNew = dtItemDirect.NewRow();
                drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                drNew["item_id"] = co_price_detail.item_id;
                drNew["labor_id"] = co_price_detail.labor_id;
                drNew["section_serial"] = co_price_detail.section_serial;
                drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
                drNew["section_name"] = co_price_detail.section_name;

                drNew["item_name"] = co_price_detail.item_name;
                drNew["measure_unit"] = co_price_detail.measure_unit;
                drNew["item_cost"] = co_price_detail.item_cost;
                drNew["total_retail_price"] = co_price_detail.total_retail_price;
                drNew["total_direct_price"] = co_price_detail.total_direct_price;
                drNew["minimum_qty"] = co_price_detail.minimum_qty;
                drNew["quantity"] = co_price_detail.quantity;
                drNew["labor_rate"] = co_price_detail.labor_rate;
                drNew["short_notes"] = co_price_detail.short_notes;
                drNew["item_status_id"] = co_price_detail.item_status_id;
                drNew["tmpCol"] = "";
                drNew["last_update_date"] = co_price_detail.last_update_date;
                drNew["is_direct"] = co_price_detail.is_direct;
                drNew["section_level"] = co_price_detail.section_level;
                drNew["location_id"] = co_price_detail.location_id;
                drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                if (co_price_detail.is_direct == 1)
                    dtItem.Rows.Add(drNew);
                else
                    dtItemDirect.Rows.Add(drNew);
                Session.Add("Item_list", dtItem);
                Session.Add("Item_list_Direct", dtItemDirect);
            }
            grdGrouping.DataSource = dtMain;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();

            grdGroupingDirect.DataSource = dtMainDirect;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
        else
        {
            DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
            DataTable dtMain = (DataTable)Session["Main_list"];
            DataTable dtItem = (DataTable)Session["Item_list"];
            DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
            if (dtMain == null)
            {
                dtMain = LoadMasterTable();
            }
            if (dtMainDirect == null)
            {
                dtMainDirect = LoadMasterTable();
            }
            if (dtItemDirect == null)
            {
                dtItemDirect = LoadItemTable();
            }
            if (dtItem == null)
            {
                dtItem = LoadItemTable();
            }
            if (co_price_detail.is_direct == 1)
            {
                bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMain.NewRow();
                    drNew["colId"] = co_price_detail.section_level;
                    drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                    dtMain.Rows.Add(drNew);

                }
            }
            else
            {
                bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMainDirect.NewRow();
                    drNew["colId"] = co_price_detail.section_level;
                    drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                    dtMainDirect.Rows.Add(drNew);

                }

            }
            bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
            if (!Iexists)
            {
                DataRow drNew = null;
                if (co_price_detail.is_direct == 1)
                    drNew = dtItem.NewRow();
                else
                    drNew = dtItemDirect.NewRow();
                drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                drNew["item_id"] = co_price_detail.item_id;
                drNew["labor_id"] = co_price_detail.labor_id;
                drNew["section_serial"] = co_price_detail.section_serial;

                drNew["location_name"] = co_price_detail.section_name;//ddlCustomerLocations.SelectedItem.Text;
                drNew["section_name"] = ddlCustomerLocations.SelectedItem.Text;//co_price_detail.section_name;
                drNew["item_name"] = co_price_detail.item_name;
                drNew["measure_unit"] = co_price_detail.measure_unit;
                drNew["item_cost"] = co_price_detail.item_cost;
                drNew["total_retail_price"] = co_price_detail.total_retail_price;
                drNew["total_direct_price"] = co_price_detail.total_direct_price;
                drNew["minimum_qty"] = co_price_detail.minimum_qty;
                drNew["quantity"] = co_price_detail.quantity;
                drNew["labor_rate"] = co_price_detail.labor_rate;
                drNew["short_notes"] = co_price_detail.short_notes;
                drNew["item_status_id"] = co_price_detail.item_status_id;
                drNew["tmpCol"] = "";
                drNew["last_update_date"] = co_price_detail.last_update_date;
                drNew["is_direct"] = co_price_detail.is_direct;
                drNew["section_level"] = co_price_detail.section_level;
                drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                if (co_price_detail.is_direct == 1)
                    dtItem.Rows.Add(drNew);
                else
                    dtItemDirect.Rows.Add(drNew);


                Session.Add("Item_list", dtItem);
                Session.Add("Item_list_Direct", dtItemDirect);
            }

            grdGrouping.DataSource = dtMain;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();

            grdGroupingDirect.DataSource = dtMainDirect;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();

        }
        //BindSelectedItemGrid();
        //BindSelectedItemGrid_Direct();

    }
    //protected void txtO_Qty_TextChanged(object sender, EventArgs e)
    //{
    //    if (txtO_Price.Text != "")
    //    {
    //        try
    //        {
    //            Convert.ToDecimal(txtO_Qty.Text.Trim());
    //        }
    //        catch (Exception ex)
    //        {

    //            txtO_Qty.Text = "0";

    //        }
    //        try
    //        {
    //            Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", "").Trim());
    //        }
    //        catch (Exception ex)
    //        {

    //            txtO_Price.Text = "0";

    //        }

    //        decimal Other_TotalPrice = Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", "")) * Convert.ToDecimal(txtO_Qty.Text);
    //        txtO_TotalPrice.Text = Other_TotalPrice.ToString();
    //    }
    //}
    //protected void txtO_Price_TextChanged(object sender, EventArgs e)
    //{


    //    if (txtO_Qty.Text != "")
    //    {

    //        try
    //        {
    //            Convert.ToDecimal(txtO_Qty.Text.Trim());
    //        }
    //        catch (Exception ex)
    //        {

    //            txtO_Qty.Text = "0";

    //        }
    //        try
    //        {
    //            Convert.ToDecimal(txtO_Price.Text.Trim().Replace("(", "-").Replace(")", "").Replace("$", ""));
    //        }
    //        catch (Exception ex)
    //        {

    //            txtO_Price.Text = "0";

    //        }


    //        decimal Other_TotalPrice = Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", "")) * Convert.ToDecimal(txtO_Qty.Text);
    //        txtO_TotalPrice.Text = Other_TotalPrice.ToString();

    //    }
    //}
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
    //    else
    //    {
    //        DropDownList ddlLabor1 = (DropDownList)grdItemPrice.FindControl("ddlLabor");
    //        ddlLabor1 = (DropDownList)sender;
    //        GridViewRow gvr = (GridViewRow)ddlLabor1.NamingContainer;
    //        i = gvr.RowIndex;
    //    }
    //    DropDownList ddlLabor = (DropDownList)grdItemPrice.Rows[i].FindControl("ddlLabor");
    //   // Label lblTotalPrice = (Label)grdItemPrice.Rows[i].FindControl("lblTotalPrice");
    //    TextBox txtQty = (TextBox)grdItemPrice.Rows[i].FindControl("txtQty");
    //    DropDownList ddlDirect = (DropDownList)grdItemPrice.Rows[i].FindControl("ddlDirect");
    //    TextBox txtShortNote = (TextBox)grdItemPrice.Rows[i].FindControl("txtShortNote");
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
    //            lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid  quantity.");                
    //            lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Invalid  quantity.");               
    //            return;
    //        }
    //    }
    //    if (Convert.ToDecimal(grdItemPrice.Rows[i].Cells[10].Text) > Convert.ToDecimal(txtQty.Text))
    //    {
    //        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
    //        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Qty should be greater than minimum value");
    //        return;
    //    }
    //    if (Convert.ToDecimal(grdItemPrice.Rows[i].Cells[10].Text) <= Convert.ToDecimal(txtQty.Text))
    //    {
    //        decimal nCost = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[8].Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
    //        decimal nQty = Convert.ToDecimal(txtQty.Text);
    //        decimal dlabor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[1].ToString());
    //        decimal dretail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[2].ToString());
    //        decimal nLaborRate = dlabor_rate * dretail_multiplier;
    //        nLaborRate = nLaborRate * nQty;

    //        if (ddlLabor.SelectedValue == "2")
    //            nTotalPrice = nCost * nQty + nLaborRate;
    //        else
    //            nTotalPrice = nCost * nQty;
    //    }
    //    string strPriceId = "0";
    //    decimal ditem_cost = 0;
    //    decimal labor_rate = 0;
    //    decimal retail_multiplier = 0;
    //    string smeasure_unit = "";
    //    decimal dminimum_qty = 0;

    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    hdnSectionId.Value = grdItemPrice.Rows[i].Cells[1].Text;


    //    strPriceId = grdItemPrice.DataKeys[i].Values[0].ToString();

    //    labor_rate = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[1].ToString());
    //    retail_multiplier = Convert.ToDecimal(grdItemPrice.DataKeys[i].Values[2].ToString());
    //    smeasure_unit = grdItemPrice.Rows[i].Cells[7].Text;
    //    ditem_cost = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[8].Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
    //    dminimum_qty = Convert.ToDecimal(grdItemPrice.Rows[i].Cells[9].Text);
    //    hdnSectionId.Value = strPriceId;

    //    if (ddlCustomerLocations.SelectedItem.Text == "Select Location")
    //    {
    //        lblAdd.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");
    //        lblSelectLocation.Text = csCommonUtility.GetSystemErrorMessage("Please select location.");

    //        ddlCustomerLocations.Focus();
    //        return;
    //    }
    //    sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(hdnClientId.Value));
    //    hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
    //    hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
    //    hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
    //    bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);

    //    co_pricing_master co_price_detail = new co_pricing_master();

    //    co_price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString();
    //    co_price_detail.measure_unit = smeasure_unit;
    //    co_price_detail.minimum_qty = dminimum_qty;
    //    co_price_detail.retail_multiplier = retail_multiplier;
    //    co_price_detail.labor_rate = labor_rate;
    //    co_price_detail.item_cost = ditem_cost;



    //    co_price_detail.client_id = Convert.ToInt32(hdnClientId.Value);
    //    co_price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
    //    co_price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
    //    //co_price_detail.change_order_id = Convert.ToInt32(hdnChEstId.Value);
    //    co_price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
    //    co_price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
    //    co_price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
    //    co_price_detail.item_id = Convert.ToInt32(hdnSectionId.Value);
    //    co_price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
    //    co_price_detail.quantity = Convert.ToDecimal(txtQty.Text);
    //    co_price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
    //    co_price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

    //    if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
    //    {
    //        co_price_detail.total_retail_price = nTotalPrice;
    //        co_price_detail.total_direct_price = 0;
    //    }
    //    else
    //    {
    //        co_price_detail.total_retail_price = 0;
    //        co_price_detail.total_direct_price = nTotalPrice;
    //    }

    //    if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
    //        co_price_detail.labor_rate = 0;
    //    co_price_detail.section_serial = Convert.ToDecimal(hdnSectionSerial.Value);
    //    hdnItemCnt.Value = GetSerial().ToString();
    //    co_price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
    //    co_price_detail.other_item_cnt = 0;
    //    co_price_detail.item_status_id = 3;
    //    co_price_detail.short_notes = txtShortNote.Text;
    //    co_price_detail.create_date = DateTime.Now;
    //    co_price_detail.last_update_date = DateTime.Now;
    //    co_price_detail.prev_total_price = Convert.ToDecimal(lblTotalPrice.Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
    //    co_price_detail.execution_unit = 0;
    //    co_price_detail.week_id = 0;
    //    co_price_detail.is_complete = false;
    //    co_price_detail.schedule_note = "";
    //    co_price_detail.sort_id = 0;
    //    co_price_detail.CalEventId = 0;
    //    co_price_detail.is_CommissionExclude = IsCommissionExclude;

    //    _db.co_pricing_masters.InsertOnSubmit(co_price_detail);
    //    _db.SubmitChanges();
    //    lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
    //    lblResult1.Text =  csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
    //    hdnSortDesc.Value = "1";
    //    if (rdoSort.SelectedValue == "1")
    //    {
    //        //strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name
    //        DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
    //        DataTable dtMain = (DataTable)Session["Main_list"];
    //        DataTable dtItem = (DataTable)Session["Item_list"];
    //        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

    //        if ((dtMain == null) || (dtMain.Rows.Count == 0))
    //        {
    //            dtMain = LoadMasterTable();
    //        }
    //        if ((dtMainDirect == null) || (dtMainDirect.Rows.Count == 0))
    //        {
    //            dtMainDirect = LoadMasterTable();
    //        }
    //        if (dtItemDirect == null)
    //        {
    //            dtItemDirect = LoadItemTable();
    //        }
    //        if (dtItem == null)
    //        {
    //            dtItem = LoadItemTable();
    //        }
    //        if (co_price_detail.is_direct == 1)
    //        {
    //            bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
    //            if (!exists)
    //            {
    //                DataRow drNew = dtMain.NewRow();
    //                drNew["colId"] = co_price_detail.location_id;
    //                drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
    //                dtMain.Rows.Add(drNew);

    //            }
    //        }
    //        else
    //        {
    //            bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
    //            if (!exists)
    //            {
    //                DataRow drNew = dtMainDirect.NewRow();
    //                drNew["colId"] = co_price_detail.location_id;
    //                drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
    //                dtMainDirect.Rows.Add(drNew);

    //            }

    //        }
    //        bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
    //        if (!Iexists)
    //        {
    //            DataRow drNew = null;
    //            if (co_price_detail.is_direct == 1)
    //                drNew = dtItem.NewRow();
    //            else
    //                drNew = dtItemDirect.NewRow();
    //            drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
    //            drNew["item_id"] = co_price_detail.item_id;
    //            drNew["labor_id"] = co_price_detail.labor_id;
    //            drNew["section_serial"] = co_price_detail.section_serial;
    //            drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
    //            drNew["section_name"] = co_price_detail.section_name;

    //            drNew["item_name"] = co_price_detail.item_name;
    //            drNew["measure_unit"] = co_price_detail.measure_unit;
    //            drNew["item_cost"] = co_price_detail.item_cost;
    //            drNew["total_retail_price"] = co_price_detail.total_retail_price;
    //            drNew["total_direct_price"] = co_price_detail.total_direct_price;
    //            drNew["minimum_qty"] = co_price_detail.minimum_qty;
    //            drNew["quantity"] = co_price_detail.quantity;
    //            drNew["labor_rate"] = co_price_detail.labor_rate;
    //            drNew["short_notes"] = co_price_detail.short_notes;
    //            drNew["item_status_id"] = co_price_detail.item_status_id;
    //            drNew["tmpCol"] = "";
    //            drNew["last_update_date"] = co_price_detail.last_update_date;
    //            drNew["is_direct"] = co_price_detail.is_direct;
    //            drNew["section_level"] = co_price_detail.section_level;
    //            drNew["location_id"] = co_price_detail.location_id;
    //            if (co_price_detail.is_direct == 1)
    //                dtItem.Rows.Add(drNew);
    //            else
    //                dtItemDirect.Rows.Add(drNew);
    //            Session.Add("Item_list", dtItem);
    //            Session.Add("Item_list_Direct", dtItemDirect);
    //        }
    //        grdGrouping.DataSource = dtMain;
    //        grdGrouping.DataKeyNames = new string[] { "colId" };
    //        grdGrouping.DataBind();

    //        grdGroupingDirect.DataSource = dtMainDirect;
    //        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
    //        grdGroupingDirect.DataBind();
    //    }
    //    else
    //    {
    //        DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
    //        DataTable dtMain = (DataTable)Session["Main_list"];
    //        DataTable dtItem = (DataTable)Session["Item_list"];
    //        DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
    //        if (dtMain == null)
    //        {
    //            dtMain = LoadMasterTable();
    //        }
    //        if (dtMainDirect == null)
    //        {
    //            dtMainDirect = LoadMasterTable();
    //        }
    //        if (dtItemDirect == null)
    //        {
    //            dtItemDirect = LoadItemTable();
    //        }
    //        if (dtItem == null)
    //        {
    //            dtItem = LoadItemTable();
    //        }
    //        if (co_price_detail.is_direct == 1)
    //        {
    //            bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
    //            if (!exists)
    //            {
    //                DataRow drNew = dtMain.NewRow();
    //                drNew["colId"] = co_price_detail.section_level;
    //                drNew["colName"] = "SECTION: " + co_price_detail.section_name;
    //                dtMain.Rows.Add(drNew);

    //            }
    //        }
    //        else
    //        {
    //            bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
    //            if (!exists)
    //            {
    //                DataRow drNew = dtMainDirect.NewRow();
    //                drNew["colId"] = co_price_detail.section_level;
    //                drNew["colName"] = "SECTION: " + co_price_detail.section_name;
    //                dtMainDirect.Rows.Add(drNew);

    //            }

    //        }
    //        bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
    //        if (!Iexists)
    //        {
    //            DataRow drNew = null;
    //            if (co_price_detail.is_direct == 1)
    //                drNew = dtItem.NewRow();
    //            else
    //                drNew = dtItemDirect.NewRow();
    //            drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
    //            drNew["item_id"] = co_price_detail.item_id;
    //            drNew["labor_id"] = co_price_detail.labor_id;
    //            drNew["section_serial"] = co_price_detail.section_serial;

    //            drNew["location_name"] = co_price_detail.section_name;//ddlCustomerLocations.SelectedItem.Text;
    //            drNew["section_name"] = ddlCustomerLocations.SelectedItem.Text;//co_price_detail.section_name;
    //            drNew["item_name"] = co_price_detail.item_name;
    //            drNew["measure_unit"] = co_price_detail.measure_unit;
    //            drNew["item_cost"] = co_price_detail.item_cost;
    //            drNew["total_retail_price"] = co_price_detail.total_retail_price;
    //            drNew["total_direct_price"] = co_price_detail.total_direct_price;
    //            drNew["minimum_qty"] = co_price_detail.minimum_qty;
    //            drNew["quantity"] = co_price_detail.quantity;
    //            drNew["labor_rate"] = co_price_detail.labor_rate;
    //            drNew["short_notes"] = co_price_detail.short_notes;
    //            drNew["item_status_id"] = co_price_detail.item_status_id;
    //            drNew["tmpCol"] = "";
    //            drNew["last_update_date"] = co_price_detail.last_update_date;
    //            drNew["is_direct"] = co_price_detail.is_direct;
    //            drNew["section_level"] = co_price_detail.section_level;
    //            if (co_price_detail.is_direct == 1)
    //                dtItem.Rows.Add(drNew);
    //            else
    //                dtItemDirect.Rows.Add(drNew);


    //            Session.Add("Item_list", dtItem);
    //            Session.Add("Item_list_Direct", dtItemDirect);
    //        }

    //        grdGrouping.DataSource = dtMain;
    //        grdGrouping.DataKeyNames = new string[] { "colId" };
    //        grdGrouping.DataBind();

    //        grdGroupingDirect.DataSource = dtMainDirect;
    //        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
    //        grdGroupingDirect.DataBind();

    //    }



    //    //BindSelectedItemGrid();
    //    //BindSelectedItemGrid_Direct();

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
                string strUpdate = " UPDATE co_pricing_master SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=1";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            string Strq = " UPDATE co_pricing_master SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId + " AND is_direct=1";
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
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
                string strUpdate = " UPDATE co_pricing_master SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=2";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(grdGroupingDirect.DataKeys[index].Values[0]);
            string Strq = " UPDATE co_pricing_master SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId + " AND is_direct=2";
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid_Direct();
        }

    }
    protected void ddlCustomerLocations_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlCustomerLocations.ID, ddlCustomerLocations.GetType().Name, "SelectedIndexChanged"); 
        if (ddlCustomerLocations.SelectedItem.Text != "Select Location")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                int serial = dimaster1.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGrouping.DataKeys[dimaster1.RowIndex].Values[0]);
                string strUpdate = " UPDATE co_pricing_master SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=1";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                int serial = dimaster2.RowIndex + 1;
                int LocationId = Convert.ToInt32(grdGroupingDirect.DataKeys[dimaster2.RowIndex].Values[0]);
                string strUpdate = " UPDATE co_pricing_master SET sort_id =" + serial + "  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + LocationId + " AND is_direct=2";
                _db.ExecuteCommand(strUpdate, string.Empty);

            }
            int nLocId = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
            string Strq = " UPDATE co_pricing_master SET sort_id = 0  WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND location_id=" + nLocId;
            _db.ExecuteCommand(Strq, string.Empty);
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
        }
    }

    protected void grdSelectedItem1_RowEditing(object sender, GridViewEditEventArgs e)
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

            LinkButton btn = (LinkButton)grdSelecterdItem.Rows[e.NewEditIndex].Cells[13].Controls[0];
            btn.Text = "Update";
            btn.CommandName = "Update";
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
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
        //string strQ = "UPDATE co_pricing_master SET quantity=" + Convert.ToDecimal(txtquantity.Text) + " ,total_retail_price=" + dTotalPrice + " ,prev_total_price =" + dTotalPrice + " , short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE co_pricing_list_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
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
        //// Calculate_Total();
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
            dTotalPrice = dTotalPrice = Convert.ToDecimal(lblTotal_price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
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
        string strQ = "UPDATE co_pricing_master SET quantity=" + Convert.ToDecimal(txtquantity.Text) + " ,total_retail_price=" + dTotalPrice + " ,prev_total_price =" + dTotalPrice + " , short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE co_pricing_list_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ, string.Empty);

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
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");


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

        LinkButton btn = (LinkButton)grdSelectedItem2.Rows[e.NewEditIndex].Cells[13].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
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
        //string strQ = "UPDATE co_pricing_master SET quantity=" + Convert.ToDecimal(txtquantity1.Text) + " ,total_direct_price=" + dTotalDirectPrice + " ,prev_total_price =" + dTotalDirectPrice + " , short_notes='" + txtshort_notes1.Text.Replace("'", "''") + "' WHERE co_pricing_list_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
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
        //// Calculate_Total();
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
        string strQ = "UPDATE co_pricing_master SET quantity=" + Convert.ToDecimal(txtquantity1.Text) + " ,total_direct_price=" + dTotalDirectPrice + " ,prev_total_price =" + dTotalDirectPrice + " , short_notes='" + txtshort_notes1.Text.Replace("'", "''") + "' WHERE co_pricing_list_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ, string.Empty);

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

        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item updated successfully");


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


                if (txtquantity.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
                    lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
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
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
                        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
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
                    if (_db.co_pricing_masters.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
                    {
                        co_pricing_master pd = _db.co_pricing_masters.Single(p => p.item_id == Convert.ToInt32(diitem.Cells[0].Text) && p.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value));
                        LaborId = Convert.ToInt32(pd.labor_id);
                        nCost1 = Convert.ToDecimal(pd.item_cost);
                    }
                    //decimal nCost1 = Convert.ToDecimal(diitem.Cells[5].Text.Replace("$", ""));
                    decimal nQty1 = Convert.ToDecimal(txtquantity.Text);
                    decimal nLaborRate = 0;
                    decimal nTotalPrice1 = 0;

                    if (LaborId == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem.Cells[0].Text) && it.client_id == Convert.ToInt32(hdnClientId.Value));
                        //nLaborRate = Convert.ToDecimal(itm.labor_rate);
                        nLaborRate = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate = nLaborRate * nQty1;
                    }

                    nTotalPrice1 = nCost1 * nQty1 + nLaborRate;
                    lblTotal_price.Text = nTotalPrice1.ToString("c");
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


                if (txtquantity1.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
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
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
                        lblAdd.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
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
                    if (_db.co_pricing_masters.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
                    {
                        co_pricing_master pd = _db.co_pricing_masters.Single(p => p.item_id == Convert.ToInt32(diitem2.Cells[0].Text) && p.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value));
                        LaborId2 = Convert.ToInt32(pd.labor_id);
                        nCost2 = Convert.ToDecimal(pd.item_cost);
                    }
                    if (LaborId2 == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem2.Cells[0].Text) && it.client_id == Convert.ToInt32(hdnCustomerId.Value ));
                        //nLaborRate2 = Convert.ToDecimal(itm.labor_rate);
                        nLaborRate2 = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate2 = nLaborRate2 * nQty2;
                    }

                    nTotalPrice2 = nCost2 * nQty2 + nLaborRate2;
                    lblTotal_price2.Text = nTotalPrice2.ToString("c");
                }


                if (nIndex > -1)
                    break;

            }
            if (nIndex > -1)
                break;
        }
        GridRowUpdatingDirect(grvFind, nIndex);
    }

    //public static DataTable LoadFullSection()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    int[] ids = (int[])HttpContext.Current.Session["myIds"];

    //    var itemlist = (from c in _db.sectioninfos
    //                    join i in _db.item_prices on c.section_id equals i.item_id
    //                    where ids.Contains((int)c.section_level) && c.client_id == Convert.ToInt32(hdnClientId.Value)
    //                    select new SectionInfo()
    //                    {
    //                        section_id = c.section_id,
    //                        section_name = c.section_name

    //                    }).Distinct().OrderBy(od => od.section_name).ToList();


    //    DataTable dt = csCommonUtility.LINQToDataTable(itemlist);

    //    foreach (DataRow dr in dt.Rows)
    //    {
    //        strDetailsFull = string.Empty;
    //        string strItem = GetItemDetialsFull(Convert.ToInt32(dr["section_id"]));
    //        dr["section_name"] = strItem.ToString();
    //    }
    //    HttpContext.Current.Session["gspSearch"] = dt;

    //    return dt;
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
                       " s1.section_name+'>>'+s2.section_name+isnull('>>'+s3.section_name,'')+isnull('>>'+s4.section_name,'')+isnull('>>'+s5.section_name,'') as section_name " +
                       " from sectioninfo s1 " +
                       " left outer join sectioninfo s2 on s2.parent_id = s1.section_id and s2.client_id =" + nClientId +
                       " left outer join sectioninfo s3 on s3.parent_id = s2.section_id and s3.client_id =" + nClientId +
                       " left outer join sectioninfo s4 on s4.parent_id = s3.section_id and s4.client_id =" + nClientId +
                       " left outer join sectioninfo s5 on s5.parent_id = s4.section_id and s5.client_id =" + nClientId +
                       " where s1.client_id =" + nClientId + " and s1.section_id in( " + strId + ")";

            var result = _db.ExecuteQuery<sectioninfo>(sSQL);

            dt = csCommonUtility.LINQToDataTable(result);

            HttpContext.Current.Session["gspSearch"] = dt;

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




    protected void btnSearchAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearchAll.ID, btnSearchAll.GetType().Name, "Click"); 
        BindGridAllPrice();
    }
    protected void lnkClear_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkClear.ID, lnkClear.GetType().Name, "Click"); 
        txtSearchAll.Text = string.Empty;
       

    }
    protected void lnkResetAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkResetAll.ID, lnkResetAll.GetType().Name, "Click"); 
        txtSearchAll.Text = string.Empty;
        BindGridAllPrice();

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
    public void BindGridAllPrice()
    {

        lblResult1.Text = "";
        lblAdd.Text = "";
        lblSelectLocation.Text = "";
        DataTable dtMainList = new DataTable();

        DataClassesDataContext _db = new DataClassesDataContext();
        string strCondition = string.Empty;
        if (txtSearchAll.Text.Trim() != "")
        {
            string str = txtSearchAll.Text.Trim();

            if (str.IndexOf(">>") != -1)
            {
                var corrected = str.Substring(0, str.Length - 2);
                str = corrected.Substring(corrected.LastIndexOf(">>") + 2);
            }
            strCondition = "  AND si.section_name LIKE '%" + str.Trim() + "%'";

            string StrQ = " SELECT it.client_id,it.item_id,si.section_name,si.is_mandatory,it.measure_unit,it.item_cost*it.retail_multiplier AS item_cost,it.minimum_qty,it.retail_multiplier,it.labor_rate,it.labor_rate* it.retail_multiplier as LaborUnitCost, it.update_time,si.section_serial,si.section_level,si.parent_id,it.labor_id,((it.item_cost + it.labor_rate) * it.retail_multiplier) * it.minimum_qty AS ext_item_cost " +
                         " FROM item_price it " +
                         " INNER JOIN sectioninfo si on si.section_id =  it.item_id " +
                         " WHERE si.is_active = 1   " + strCondition + " AND si.is_disable = 0";
            dtMainList = csCommonUtility.GetDataTable(StrQ);
            grdItemPriceAll.DataSource = dtMainList;
            grdItemPriceAll.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory", "section_level", "parent_id", "section_serial" };
            grdItemPriceAll.DataBind();
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
            grdItemPriceAll.DataSource = dtMainList;
            grdItemPriceAll.DataBind();
            btnAddMultiple.Visible = false;
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

            if (e.Row.Cells[1].Text == "")
                e.Row.Visible = false;

        }

    }
    protected void btnAddMultiple_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddMultiple.ID, btnAddMultiple.GetType().Name, "Click"); 
        lblSelectLocation.Text = "";
        lblResult1.Text = "";
        lblMessage.Text = "";
        lblAdd.Text = "";
        co_pricing_master co_price_detail = new co_pricing_master();

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
                    lblSelectLocation.Font.Bold = true;
                    ddlCustomerLocations.Focus();
                    return;
                }


                sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == nSectionId && s.client_id == Convert.ToInt32(hdnClientId.Value));
                bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
                co_price_detail = new co_pricing_master();

                co_price_detail.item_name = GetItemDetialsForUpdateItem(nSectionId).ToString();
                co_price_detail.measure_unit = smeasure_unit;
                co_price_detail.minimum_qty = dminimum_qty;
                co_price_detail.retail_multiplier = dretail_multiplier;
                co_price_detail.labor_rate = dlabor_rate;
                co_price_detail.item_cost = ditem_cost;



                co_price_detail.client_id = Convert.ToInt32(hdnClientId.Value);
                co_price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                co_price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                //co_price_detail.change_order_id = Convert.ToInt32(hdnChEstId.Value);
                co_price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
                co_price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                co_price_detail.section_level = sinfo.section_level;
                co_price_detail.item_id = nSectionId;
                co_price_detail.section_name = GetSectionName(Convert.ToInt32(sinfo.section_level));
                co_price_detail.quantity = Convert.ToDecimal(txtQty.Text);
                co_price_detail.labor_id = Convert.ToInt32(ddlLabor.SelectedValue);
                co_price_detail.is_direct = Convert.ToInt32(ddlDirect.SelectedValue);

                if (Convert.ToInt32(ddlDirect.SelectedValue) == 1)
                {
                    co_price_detail.total_retail_price = nTotalPrice;
                    co_price_detail.total_direct_price = 0;
                }
                else
                {
                    co_price_detail.total_retail_price = 0;
                    co_price_detail.total_direct_price = nTotalPrice;
                }

                if (Convert.ToInt32(ddlLabor.SelectedValue) == 1)
                    co_price_detail.labor_rate = 0;
                co_price_detail.section_serial = Convert.ToDecimal(sinfo.section_serial); ;
                int serial = GetSerialMultiple(Convert.ToInt32(sinfo.section_level));
                co_price_detail.item_cnt = serial;
                co_price_detail.other_item_cnt = 0;
                co_price_detail.item_status_id = 3;
                co_price_detail.short_notes = txtShortNote.Text;
                co_price_detail.create_date = DateTime.Now;
                co_price_detail.last_update_date = DateTime.Now;
                co_price_detail.prev_total_price = nTotalPrice;
                co_price_detail.execution_unit = 0;
                co_price_detail.week_id = 0;
                co_price_detail.is_complete = false;
                co_price_detail.schedule_note = "";
                co_price_detail.sort_id = 0;
                co_price_detail.CalEventId = 0;
                co_price_detail.is_CommissionExclude = IsCommissionExclude;

                _db.co_pricing_masters.InsertOnSubmit(co_price_detail);
                _db.SubmitChanges();

            }

        }


        lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
        hdnSortDesc.Value = "1";
        Session.Remove("Main_list_Direct");
        Session.Remove("Main_list");
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

        if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
        {
            rdoSort.Visible = false;

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




    protected void chkCustDisp_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCustDisp.ID, chkCustDisp.GetType().Name, "CheckedChanged"); 
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
              " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ, string.Empty);
                //lblResult1.Text = csCommonUtility.GetSystemMessage("Customized display setting of Project activated successfully.");
            }
            else
            {
                strQ = "UPDATE customer_estimate SET IsCustDisplay = 0 " +
             " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
             " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
             " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
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

            if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
            {
                rdoSort.Visible = false;

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
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }

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
            //Calculate_Total();
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
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
        decimal nTotalPrice = 0;
        try
        {
            nTotalPrice = Convert.ToDecimal(txtCustomizeExtPrice.Text.Replace("(", "-").Replace(")", "").Replace("$", "").Trim());
        }
        catch (Exception ex)
        {

            nTotalPrice = 0;

        }


        if (nTotalPrice== nUnitPrice)
        {
            nRetailMul = nTotalPrice / nUnitPrice;
        }
        else if (nTotalPrice > nUnitPrice)
        {
            nRetailMul = nTotalPrice / nUnitPrice / nQty;
        }
        sectioninfo sinfo = _db.sectioninfos.Single(s => s.section_id == Convert.ToInt32(hdnSectionId.Value) && s.client_id == Convert.ToInt32(hdnClientId.Value));
        hdnParentId.Value = Convert.ToInt32(sinfo.parent_id).ToString();
        hdnSectionLevel.Value = Convert.ToInt32(sinfo.section_level).ToString();
        hdnSectionSerial.Value = Convert.ToDecimal(sinfo.section_serial).ToString();
        bool IsCommissionExclude = Convert.ToBoolean(sinfo.is_CommissionExclude);
        co_pricing_master co_price_detail = new co_pricing_master();

        co_price_detail.item_name = GetItemDetialsForUpdateItem(Convert.ToInt32(hdnSectionId.Value)).ToString() + "" + txtCustomizeItemName.Text;//txtOther.Text + ">>Other";
        co_price_detail.measure_unit = txtCustomizeUOM.Text;
        co_price_detail.minimum_qty = 1;
        co_price_detail.retail_multiplier = nRetailMul;
        co_price_detail.labor_rate = 0;
        co_price_detail.item_cost = nUnitPrice * nRetailMul; ;//Convert.ToDecimal(txtO_Price.Text.Replace("(", "-").Replace(")", "").Replace("$", ""));
        co_price_detail.client_id = Convert.ToInt32(hdnClientId.Value);
        co_price_detail.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        co_price_detail.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        co_price_detail.location_id = Convert.ToInt32(ddlCustomerLocations.SelectedValue);
        co_price_detail.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
        co_price_detail.section_level = Convert.ToInt32(hdnSectionLevel.Value);
        co_price_detail.item_id = Convert.ToInt32(hdnOtherId.Value);
        co_price_detail.section_name = GetSectionName(Convert.ToInt32(hdnSectionLevel.Value));
        co_price_detail.quantity = nQty; //Convert.ToDecimal(txtO_Qty.Text);
        co_price_detail.labor_id = 1;
        co_price_detail.is_direct = Convert.ToInt32(ddlO_Direct.SelectedValue);

        if (Convert.ToInt32(ddlO_Direct.SelectedValue) == 1)
        {
            co_price_detail.total_retail_price = nTotalPrice;
            co_price_detail.total_direct_price = 0;
        }
        else
        {
            co_price_detail.total_retail_price = 0;
            co_price_detail.total_direct_price = nTotalPrice;
        }
        hdnItemCnt.Value = GetSerial().ToString();
        string strOtherCount = GetSerial_other(Convert.ToInt32(co_price_detail.item_id)).ToString();
        co_price_detail.item_cnt = Convert.ToInt32(hdnItemCnt.Value);
        co_price_detail.other_item_cnt = Convert.ToInt32(strOtherCount);
        string str = "0";
        if (Convert.ToInt32(strOtherCount) > 9)
            str = co_price_detail.item_id + "." + strOtherCount;
        else
            str = co_price_detail.item_id + ".0" + strOtherCount;
        co_price_detail.section_serial = Convert.ToDecimal(str);
        co_price_detail.item_status_id = 3;
        co_price_detail.short_notes = txtCustomezeNotes.Text;
        co_price_detail.create_date = DateTime.Now;
        co_price_detail.last_update_date = DateTime.Now;
        co_price_detail.prev_total_price = nTotalPrice;
        co_price_detail.execution_unit = 0;
        co_price_detail.week_id = 0;
        co_price_detail.is_complete = false;
        co_price_detail.schedule_note = "";
        co_price_detail.sort_id = 0;
        co_price_detail.CalEventId = 0;
        co_price_detail.is_CommissionExclude = IsCommissionExclude;

        _db.co_pricing_masters.InsertOnSubmit(co_price_detail);
        _db.SubmitChanges();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
        lblAdd.Text = csCommonUtility.GetSystemMessage("Item added to change order list, select another item or Location/Section");
        txtCustomizeItemName.Text = "";
        txtCustomizeUOM.Text = "";
        txtCustomizeCode.Text = "";
        txtCustomizeUnitPrice.Text = "";
        txtCustomizeExtPrice.Text = "";
        txtCustomezeNotes.Text = "";
        hdnSortDesc.Value = "1";
        // Cost
        decimal dOrginalCost = 0;
        decimal dOrginalTotalCost = 0;
        decimal dLaborTotal = 0;
        decimal dLineItemTotal = 0;

        decimal dTPrice = 0;

        string sItemName = co_price_detail.item_name.ToString();

        decimal dItemCost = Convert.ToDecimal(co_price_detail.item_cost);
        decimal dRetail_multiplier = Convert.ToDecimal(co_price_detail.retail_multiplier);
        decimal dQuantity = Convert.ToDecimal(co_price_detail.quantity);
        decimal dLabor_rate = Convert.ToDecimal(co_price_detail.labor_rate);
        if (co_price_detail.is_direct == 1)
            dTPrice = Convert.ToDecimal(co_price_detail.total_retail_price);
        else
            dTPrice = Convert.ToDecimal(co_price_detail.total_direct_price);

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

        if (rdoSort.SelectedValue == "1")
        {
            //strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name
            DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
            DataTable dtMain = (DataTable)Session["Main_list"];
            DataTable dtItem = (DataTable)Session["Item_list"];
            DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];

            if ((dtMain == null) || (dtMain.Rows.Count == 0))
            {
                dtMain = LoadMasterTable();
            }
            if ((dtMainDirect == null) || (dtMainDirect.Rows.Count == 0))
            {
                dtMainDirect = LoadMasterTable();
            }
            if (dtItemDirect == null)
            {
                dtItemDirect = LoadItemTable();
            }
            if (dtItem == null)
            {
                dtItem = LoadItemTable();
            }
            if (co_price_detail.is_direct == 1)
            {
                bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMain.NewRow();
                    drNew["colId"] = co_price_detail.location_id;
                    drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                    dtMain.Rows.Add(drNew);

                }
            }
            else
            {
                bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.location_id)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMainDirect.NewRow();
                    drNew["colId"] = co_price_detail.location_id;
                    drNew["colName"] = "LOCATION:" + ddlCustomerLocations.SelectedItem.Text;
                    dtMainDirect.Rows.Add(drNew);

                }

            }
            bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
            if (!Iexists)
            {
                DataRow drNew = null;
                if (co_price_detail.is_direct == 1)
                    drNew = dtItem.NewRow();
                else
                    drNew = dtItemDirect.NewRow();
                drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                drNew["item_id"] = co_price_detail.item_id;
                drNew["labor_id"] = co_price_detail.labor_id;
                drNew["section_serial"] = co_price_detail.section_serial;
                drNew["location_name"] = ddlCustomerLocations.SelectedItem.Text;
                drNew["section_name"] = co_price_detail.section_name;

                drNew["item_name"] = co_price_detail.item_name;
                drNew["measure_unit"] = co_price_detail.measure_unit;
                drNew["item_cost"] = co_price_detail.item_cost;
                drNew["total_retail_price"] = co_price_detail.total_retail_price;
                drNew["total_direct_price"] = co_price_detail.total_direct_price;
                drNew["minimum_qty"] = co_price_detail.minimum_qty;
                drNew["quantity"] = co_price_detail.quantity;
                drNew["labor_rate"] = co_price_detail.labor_rate;
                drNew["short_notes"] = co_price_detail.short_notes;
                drNew["item_status_id"] = co_price_detail.item_status_id;
                drNew["tmpCol"] = "";
                drNew["last_update_date"] = co_price_detail.last_update_date;
                drNew["is_direct"] = co_price_detail.is_direct;
                drNew["section_level"] = co_price_detail.section_level;
                drNew["location_id"] = co_price_detail.location_id;
                drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                if (co_price_detail.is_direct == 1)
                    dtItem.Rows.Add(drNew);
                else
                    dtItemDirect.Rows.Add(drNew);
                Session.Add("Item_list", dtItem);
                Session.Add("Item_list_Direct", dtItemDirect);
            }
            grdGrouping.DataSource = dtMain;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();

            grdGroupingDirect.DataSource = dtMainDirect;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
        else
        {
            DataTable dtMainDirect = (DataTable)Session["Main_list_Direct"];
            DataTable dtMain = (DataTable)Session["Main_list"];
            DataTable dtItem = (DataTable)Session["Item_list"];
            DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
            if (dtMain == null)
            {
                dtMain = LoadMasterTable();
            }
            if (dtMainDirect == null)
            {
                dtMainDirect = LoadMasterTable();
            }
            if (dtItemDirect == null)
            {
                dtItemDirect = LoadItemTable();
            }
            if (dtItem == null)
            {
                dtItem = LoadItemTable();
            }
            if (co_price_detail.is_direct == 1)
            {
                bool exists = dtMain.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMain.NewRow();
                    drNew["colId"] = co_price_detail.section_level;
                    drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                    dtMain.Rows.Add(drNew);

                }
            }
            else
            {
                bool exists = dtMainDirect.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(co_price_detail.section_level)).Count() > 0;
                if (!exists)
                {
                    DataRow drNew = dtMainDirect.NewRow();
                    drNew["colId"] = co_price_detail.section_level;
                    drNew["colName"] = "SECTION: " + co_price_detail.section_name;
                    dtMainDirect.Rows.Add(drNew);

                }

            }
            bool Iexists = dtItem.AsEnumerable().Where(c => c.Field<Int32>("co_pricing_list_id").Equals(co_price_detail.co_pricing_list_id)).Count() > 0;
            if (!Iexists)
            {
                DataRow drNew = null;
                if (co_price_detail.is_direct == 1)
                    drNew = dtItem.NewRow();
                else
                    drNew = dtItemDirect.NewRow();
                drNew["co_pricing_list_id"] = co_price_detail.co_pricing_list_id;
                drNew["item_id"] = co_price_detail.item_id;
                drNew["labor_id"] = co_price_detail.labor_id;
                drNew["section_serial"] = co_price_detail.section_serial;

                drNew["location_name"] = co_price_detail.section_name;//ddlCustomerLocations.SelectedItem.Text;
                drNew["section_name"] = ddlCustomerLocations.SelectedItem.Text;//co_price_detail.section_name;
                drNew["item_name"] = co_price_detail.item_name;
                drNew["measure_unit"] = co_price_detail.measure_unit;
                drNew["item_cost"] = co_price_detail.item_cost;
                drNew["total_retail_price"] = co_price_detail.total_retail_price;
                drNew["total_direct_price"] = co_price_detail.total_direct_price;
                drNew["minimum_qty"] = co_price_detail.minimum_qty;
                drNew["quantity"] = co_price_detail.quantity;
                drNew["labor_rate"] = co_price_detail.labor_rate;
                drNew["short_notes"] = co_price_detail.short_notes;
                drNew["item_status_id"] = co_price_detail.item_status_id;
                drNew["tmpCol"] = "";
                drNew["last_update_date"] = co_price_detail.last_update_date;
                drNew["is_direct"] = co_price_detail.is_direct;
                drNew["section_level"] = co_price_detail.section_level;
                drNew["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                drNew["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                drNew["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total
                if (co_price_detail.is_direct == 1)
                    dtItem.Rows.Add(drNew);
                else
                    dtItemDirect.Rows.Add(drNew);


                Session.Add("Item_list", dtItem);
                Session.Add("Item_list_Direct", dtItemDirect);
            }

            grdGrouping.DataSource = dtMain;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();

            grdGroupingDirect.DataSource = dtMainDirect;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();

        }

    }
}


