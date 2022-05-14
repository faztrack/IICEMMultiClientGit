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

public partial class change_order_locations : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin017") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            hdnChEstId.Value = "0";
            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();
            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Request.QueryString.Get("coestid") != null)
            {
                hdnChEstId.Value = Convert.ToInt32(Request.QueryString.Get("coestid")).ToString();
                changeorder_estimate cho = new changeorder_estimate();
                cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));
                if(cho.change_order_status_id>2)
                    Response.Redirect("change_order_worksheet_readonly.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
            }
            else
            {
                if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnChEstId.Value) == 0)
                {
                    decimal taxrate = 0;
                    if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                    {
                        taxrate = 0;
                    }
                    else
                    {
                        estimate_payment esp = new estimate_payment();
                        esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        taxrate = Convert.ToDecimal(esp.tax_rate);
                    }
                    
                    
                    int nchoEstId = 0;
                    var result = (from coe in _db.changeorder_estimates
                                  where coe.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && coe.customer_id == Convert.ToInt32(hdnCustomerId.Value) && coe.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select coe.chage_order_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nchoEstId = result.Max();
                    nchoEstId = nchoEstId + 1;
                    hdnChEstId.Value = nchoEstId.ToString();
                    changeorder_estimate cus_est = new changeorder_estimate();
                    cus_est.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    cus_est.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    cus_est.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    cus_est.chage_order_id = Convert.ToInt32(hdnChEstId.Value);
                    cus_est.change_order_status_id = 1;
                    cus_est.change_order_type_id = 1;
                    cus_est.payment_terms = "";
                    cus_est.other_terms = "";
                    cus_est.is_total = 1;
                    cus_est.is_tax = true;
                    cus_est.tax = taxrate;
                    cus_est.total_payment_due = "";
                    cus_est.execute_date = "";
                    cus_est.changeorder_date = "";
                    cus_est.notes1 = "";
                    cus_est.is_close = false;
                    cus_est.comments = "";
                    cus_est.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    string est = "Change Order" + " " + hdnChEstId.Value;
                    cus_est.changeorder_name = est;
                    cus_est.create_date = DateTime.Now;
                    cus_est.last_updated_date = DateTime.Now;
                    cus_est.is_cutomer_viewable = 2;
                    cus_est.IsChangeOrderQtyViewByCust = false;
                    _db.changeorder_estimates.InsertOnSubmit(cus_est);

                    //Check CO Pricing Master Exist.
                    if (_db.co_pricing_masters.Where(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count == 0)
                    {
                        List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                        List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                        List<pricing_detail> Pm_List = _db.pricing_details.Where(pd => pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A").ToList();

                        foreach (customer_location objcl in Cust_LocList)
                        {
                            changeorder_location co_loc = new changeorder_location();
                            co_loc.client_id = objcl.client_id;
                            co_loc.customer_id = objcl.customer_id;
                            co_loc.location_id = objcl.location_id;
                            co_loc.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                            _db.changeorder_locations.InsertOnSubmit(co_loc);
                        }
                        foreach (customer_section objcs in Cust_SecList)
                        {
                            changeorder_section co_sec = new changeorder_section();
                            co_sec.client_id = objcs.client_id;
                            co_sec.customer_id = objcs.customer_id;
                            co_sec.section_id = objcs.section_id;
                            co_sec.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                            co_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            _db.changeorder_sections.InsertOnSubmit(co_sec);
                        }
                        foreach (pricing_detail objCpm in Pm_List)
                        {
                            co_pricing_master cpm = new co_pricing_master();
                            cpm.client_id = objCpm.client_id; ;
                            cpm.customer_id = objCpm.customer_id;
                            cpm.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                            cpm.location_id = objCpm.location_id; ;
                            cpm.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            cpm.section_level = objCpm.section_level;
                            cpm.item_id = objCpm.item_id;
                            cpm.section_name = objCpm.section_name;
                            cpm.item_name = objCpm.item_name;
                            cpm.measure_unit = objCpm.measure_unit;
                            cpm.minimum_qty = objCpm.minimum_qty;
                            cpm.quantity = objCpm.quantity;
                            cpm.retail_multiplier = objCpm.retail_multiplier;
                            cpm.labor_id = objCpm.labor_id;
                            cpm.is_direct = objCpm.is_direct;
                            cpm.item_cost = objCpm.item_cost;
                            cpm.total_direct_price = objCpm.total_direct_price;
                            cpm.total_retail_price = objCpm.total_retail_price;
                            cpm.labor_rate = objCpm.labor_rate;
                            cpm.section_serial = objCpm.section_serial;
                            cpm.item_cnt = objCpm.item_cnt;
                            cpm.item_status_id = 1;
                            cpm.short_notes = objCpm.short_notes;
                            cpm.create_date = DateTime.Today;
                            cpm.last_update_date = DateTime.Today;
                            cpm.prev_total_price = 0;
                            cpm.execution_unit = 0;
                            cpm.week_id = 0;
                            cpm.sort_id = objCpm.sort_id;
                            cpm.is_complete = false;
                            cpm.schedule_note = "";
                            cpm.CalEventId = 0;
                            cpm.is_CommissionExclude = objCpm.is_CommissionExclude;
                            _db.co_pricing_masters.InsertOnSubmit(cpm);
                        }
                    }

                    _db.SubmitChanges();

                }



            }

           // var typelist = new int[] { 0, Convert.ToInt32(hdnCustomerId.Value) };
            //typelist.Contains((int)cl.CustomerId) &&

            chkLocations.DataSource = _db.locations.Where(cl =>  cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && Convert.ToBoolean(cl.is_active) == true).OrderBy(s => s.location_name).ToList();
            chkLocations.DataTextField = "location_name";
            chkLocations.DataValueField = "location_id";
            chkLocations.DataBind();

            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblPhone.Text = cust.phone;
                lblEmail.Text = cust.email;
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            }
            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                lblEstimateName.Text = cus_est.estimate_name;
            }
            // Get Change Order Info
            if (Convert.ToInt32(hdnChEstId.Value) > 0)
            {
                changeorder_estimate cho = new changeorder_estimate();
                cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

                lblChangeOrderName.Text = cho.changeorder_name;
            }

            MarkExistingEstimateLocations(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));

           
            if (Request.QueryString.Get("clid") != null)
            {
                CheckExistingChangeOrderLocations(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
                MarkExistingEstimateLocations(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));

            }
            else
            {
                // Check Change Order Location Exist.
                if (_db.changeorder_locations.Where(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count > 0)
                {
                    Response.Redirect("change_order_sections.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                }
            }
        }
    }

    protected void CheckExistingChangeOrderLocations(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.changeorder_locations.Where(cl => cl.customer_id == nCustomerId && cl.estimate_id == nEstimateId);
        foreach (ListItem li in chkLocations.Items)
        {
            foreach (changeorder_location loc in item)
            {
                if (loc.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Selected = true;
                }
            }
        }
    }

    protected void MarkExistingEstimateLocations(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_locations.Where(cl => cl.customer_id == nCustomerId && cl.estimate_id == nEstimateId);
        foreach (ListItem li in chkLocations.Items)
        {
            foreach (customer_location loc in item)
            {
                if (loc.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Attributes.CssStyle.Add("color", "red");
                    li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
    }
    protected void btnContinueChangeOrder_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContinueChangeOrder.ID, btnContinueChangeOrder.GetType().Name, "Click"); 
        lblMessage.Text = "";

        if (chkLocations.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select change order locations.");
             
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "DELETE changeorder_locations WHERE  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        for (int i = 0; i < chkLocations.Items.Count; i++)
        {
            if (chkLocations.Items[i].Selected == true)
            {
                int nLocationid = Convert.ToInt32(chkLocations.Items[i].Value);
                // Add Customer locations
                changeorder_location co_loc = new changeorder_location();
                co_loc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                co_loc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                co_loc.location_id = nLocationid;
                co_loc.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                _db.changeorder_locations.InsertOnSubmit(co_loc);
               
            }
        }
        _db.SubmitChanges();

        Response.Redirect("change_order_sections.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
}
