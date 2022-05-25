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

public partial class customer_locations : System.Web.UI.Page
{
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
            if (Page.User.IsInRole("admin038") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            hdnEstimateId.Value = "0";

            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);



            DataClassesDataContext _db = new DataClassesDataContext();
            customer cust = new customer();
            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
           
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if (cust != null)
                {
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

                    hdnClientId.Value = cust.client_id.ToString();
                }
                               

            }

            if (Request.QueryString.Get("eid") != null)
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();
            else
            {
                if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) == 0)
                {
                    int nEstId = 0;
                    var result = (from ce in _db.customer_estimates
                                  where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value)
                                  select ce.estimate_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nEstId = result.Max();
                    nEstId = nEstId + 1;
                    hdnEstimateId.Value = nEstId.ToString();

                    customer_estimate cus_est = new customer_estimate();
                    cus_est.client_id = cust.client_id;
                    cus_est.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    cus_est.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    cus_est.status_id = 1;
                    cus_est.tax_rate = 0;
                    cus_est.sale_date = "";
                    cus_est.estimate_comments = "";
                    cus_est.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    string est = "Estimate" + " " + hdnEstimateId.Value;
                    cus_est.estimate_name = est;
                    cus_est.IsEstimateActive = true;
                    cus_est.IsCustDisplay = false;
                    cus_est.create_date = DateTime.Now;
                    cus_est.last_update_date = DateTime.Now;
                    cus_est.JobId = 0;
                    _db.customer_estimates.InsertOnSubmit(cus_est);
                    _db.SubmitChanges();
                }
            }

           // var typelist = new int[] { 0, Convert.ToInt32(hdnCustomerId.Value) };
           // typelist.Contains((int)cl.CustomerId) &&

            chkLocations.DataSource = _db.locations.Where(cl =>  cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && Convert.ToBoolean(cl.is_active) == true).OrderBy(s => s.location_name).ToList();
            chkLocations.DataTextField = "location_name";
            chkLocations.DataValueField = "location_id";
            chkLocations.DataBind();


            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                lblEstimateName.Text = cus_est.estimate_name;
            }


            if (Request.QueryString.Get("lid") != null)
            {
                CheckExistingLocations(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
                MarkExistItems(Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
            }
            else
            {
                // Check Customer Location Exist.
                if (_db.customer_locations.Where(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                {
                    if (_db.customer_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                        Response.Redirect("pricing.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                    else
                        Response.Redirect("estimate_sections.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                }
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnContinuetoPricing", "chkLocations", "lnkAddNewLocation", "hypGoogleMap", "pnlAddNewLocation" });
        }
    }

    protected void CheckExistingLocations(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_locations.Where(cl => cl.customer_id == nCustomerId && cl.estimate_id == nEstimateId);
        foreach (ListItem li in chkLocations.Items)
        {
            //location locNa = _db.locations.SingleOrDefault(l => l.location_id == Convert.ToInt32(li.Value));
            //if (locNa.CustomerId > 0)
            //{
            //    li.Attributes.CssStyle.Add("color","red");
            //}

            foreach (customer_location loc in item)
            {
                if (loc.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Selected = true;
                    //li.Attributes.CssStyle.Add("font-weight", "bold");

                }
            }
        }
    }

    private void MarkExistItems(int nEstId, int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.pricing_details.Where(cl => cl.customer_id == nCustId && cl.estimate_id == nEstId);
        foreach (ListItem li in chkLocations.Items)
        {
            foreach (pricing_detail pd in item)
            {
                if (pd.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                     li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        lblResult.Text = "";
        txtLocationName.Text = "";
        txtDescription.Text = "";
        modAddNewLocation.Hide();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        if (txtLocationName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Location Name.");

            return;
        }
        // Location name exist
        DataClassesDataContext _db = new DataClassesDataContext();
        location loc = new location();
       // var typelist = new int[] { 0, Convert.ToInt32(hdnCustomerId.Value) };
        //&& typelist.Contains((int)l.CustomerId)
        if (_db.locations.Where(l => l.location_name == txtLocationName.Text).SingleOrDefault() != null)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Location name already exist. Please try another name.");

            return;
        }
        // Add the Location
        loc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        loc.location_name = txtLocationName.Text;
        loc.loation_desc = txtDescription.Text;
        loc.is_active = Convert.ToBoolean(1);
        //loc.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
        _db.locations.InsertOnSubmit(loc);
        _db.SubmitChanges();

        // Hide Modal Popup
        lblResult.Text = "";
        txtLocationName.Text = "";
        txtDescription.Text = "";
        modAddNewLocation.Hide();

        // Load Locations
        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            Response.Redirect("customer_locations.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
        else
            Response.Redirect("customer_locations.aspx?cid=" + hdnCustomerId.Value);
    }

    protected void btnContinuetoPricing_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContinuetoPricing.ID, btnContinuetoPricing.GetType().Name, "Click"); 
        lblMessage.Text = "";

        if (chkLocations.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select estimate locations.");

            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "DELETE customer_locations WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id = " + Convert.ToInt32(hdnClientId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        for (int i = 0; i < chkLocations.Items.Count; i++)
        {
            if (chkLocations.Items[i].Selected == true)
            {
                int nLocationid = Convert.ToInt32(chkLocations.Items[i].Value);
                // Add Customer locations
                customer_location cus_loc = new customer_location();
                cus_loc.client_id = Convert.ToInt32(hdnClientId.Value);
                cus_loc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                cus_loc.location_id = nLocationid;
                cus_loc.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                _db.customer_locations.InsertOnSubmit(cus_loc);

            }
        }
        _db.SubmitChanges();
        Response.Redirect("estimate_sections.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    protected void chkLocations_DataBound(object sender, EventArgs e)
    {

    }
}
