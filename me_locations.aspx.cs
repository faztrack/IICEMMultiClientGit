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

public partial class me_locations : System.Web.UI.Page
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
            hdnEstimateId.Value = "0";
            DataClassesDataContext _db = new DataClassesDataContext();
            model_estimate me = new model_estimate();
            if (Request.QueryString.Get("spid") != null)
            {
                hdnSalesPersonId.Value = Convert.ToInt32(Request.QueryString.Get("spid")).ToString();
            }
            else
            {
                userinfo obj = (userinfo)Session["oUser"];
                hdnSalesPersonId.Value = obj.sales_person_id.ToString();
            }
            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            lblSalesPersonName.Text = sp.first_name + " " + sp.last_name;
            lblAddress.Text = sp.address;
            lblPhone.Text = sp.phone;
            lblEmail.Text = sp.email;
             string strModelEstimateName = "";

             chkLocations.DataSource = _db.locations.Where(cl => cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && Convert.ToBoolean(cl.is_active) == true).OrderBy(s => s.location_name).ToList();
             chkLocations.DataTextField = "location_name";
             chkLocations.DataValueField = "location_id";
             chkLocations.DataBind();
           
            if (Request.QueryString.Get("meid") != null)
            {
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("meid")).ToString();
            if (_db.model_estimates.Where(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count > 0)
            {
                me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                strModelEstimateName = me.model_estimate_name;
                hdnEstimateId.Value = me.model_estimate_id.ToString();

                if (Session["AddMoreMELocation"] != null)
                {
                    CheckExistingLocations(Convert.ToInt32(hdnSalesPersonId.Value), Convert.ToInt32(hdnEstimateId.Value));
                }
                else
                {
                    if (_db.model_estimate_locations.Where(mel => mel.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mel.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mel.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count > 0)
                    {
                        Response.Redirect("me_sections.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
                    }                    
                }   
            }
            }
            else
            {
                if (Convert.ToInt32(hdnEstimateId.Value) == 0)
                {
                    int nEstId = 0;
                    var result = (from mest in _db.model_estimates
                                  where mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select mest.model_estimate_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nEstId = result.Max();
                    nEstId = nEstId + 1;
                    hdnEstimateId.Value = nEstId.ToString();

                    me.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    me.model_estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    me.status_id = 1;
                    me.estimate_comments = "";
                    me.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    string est = "Estimate Template" + " " + hdnEstimateId.Value;
                    me.model_estimate_name = est;
                    me.create_date = DateTime.Now;
                    me.last_udated_date = DateTime.Now;
                    me.IsPublic = false;
                    _db.model_estimates.InsertOnSubmit(me);
                    _db.SubmitChanges();
                    strModelEstimateName=est;
                }
            }
           

           
            lblModelEstimateName.Text = strModelEstimateName;
        }
    }

    protected void CheckExistingLocations(int nSalesPersonId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.model_estimate_locations.Where(mel => mel.model_estimate_id == nEstimateId && mel.sales_person_id == nSalesPersonId && mel.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

        foreach (ListItem li in chkLocations.Items)
        {
            foreach (model_estimate_location loc in item)
            {
                if (loc.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Selected = true;
                    //li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
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
        //l.CustomerId == 0 &&
        if (_db.locations.Where(l =>  l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.location_name == txtLocationName.Text).SingleOrDefault() != null)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Location name already exist. Please try another name.");
            
            return;
        }
        // Add the Location
        loc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        loc.location_name = txtLocationName.Text;
        loc.loation_desc = txtDescription.Text;
        loc.is_active = Convert.ToBoolean(1);
       // loc.CustomerId = 0;
        _db.locations.InsertOnSubmit(loc);
        _db.SubmitChanges();

        // Hide Modal Popup
        lblResult.Text = "";
        txtLocationName.Text = "";
        txtDescription.Text = "";
        modAddNewLocation.Hide();

        Response.Redirect("me_locations.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        lblResult.Text = "";
        txtLocationName.Text = "";
        txtDescription.Text = "";
        modAddNewLocation.Hide();
    }
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContinue.ID, btnContinue.GetType().Name, "Click"); 
        lblMessage.Text = "";


        if (chkLocations.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select estimate locations.");
             
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "DELETE model_estimate_locations WHERE sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + "  AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        for (int i = 0; i < chkLocations.Items.Count; i++)
        {
            if (chkLocations.Items[i].Selected == true)
            {
                int nLocationid = Convert.ToInt32(chkLocations.Items[i].Value);
                // Add Customer locations
                model_estimate_location me_loc = new model_estimate_location();
                me_loc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                me_loc.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                me_loc.model_estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                me_loc.location_id = nLocationid;
                _db.model_estimate_locations.InsertOnSubmit(me_loc);

            }
        }
        _db.SubmitChanges();
        Response.Redirect("me_sections.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
    }
}
