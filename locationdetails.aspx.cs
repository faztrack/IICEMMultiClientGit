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

public partial class locationdetails : System.Web.UI.Page
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
            if (Page.User.IsInRole("loc002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nlid = Convert.ToInt32(Request.QueryString.Get("lid"));
            hdnLocationId.Value = nlid.ToString();

            if (Convert.ToInt32(hdnLocationId.Value) > 0)
            {
                lblHeaderTitle.Text = "Location Details";
                DataClassesDataContext _db = new DataClassesDataContext();
                location loc = new location();
                loc = _db.locations.Single(l => l.location_id == Convert.ToInt32(hdnLocationId.Value));
                txtLocationName.Text = loc.location_name;
                chkActive.Checked = Convert.ToBoolean(loc.is_active);
                txtDescription.Text = loc.loation_desc;
            }
            else
            {
                lblHeaderTitle.Text = "Add New Location";
                hdnLocationId.Value = "0";
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkActive", "btnSubmit" });

        }

    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("locationlist.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (txtLocationName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Location Name.");
            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        // Location Name Does Exist
       // l.CustomerId == 0 &&
        location loc = new location();
        if (Convert.ToInt32(hdnLocationId.Value) > 0)
            loc = _db.locations.Single(l => l.location_id == Convert.ToInt32(hdnLocationId.Value));
        else
            if (_db.locations.Where(l =>  l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.location_name == txtLocationName.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Location name already exist. Please try another name.");
                
                return;
            }
        loc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        loc.location_name = txtLocationName.Text;
        loc.loation_desc = txtDescription.Text;
        loc.is_active = Convert.ToBoolean(chkActive.Checked);
       // loc.CustomerId = 0;
        if (Convert.ToInt32(hdnLocationId.Value) == 0)
        {
            _db.locations.InsertOnSubmit(loc);
            lblResult.Text = csCommonUtility.GetSystemMessage("Location '" + txtLocationName.Text.Trim() + "' has been saved successfully.  You may enter another location.");
             
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Location '" + txtLocationName.Text.Trim() + "' has been updated successfully.");
             
        }

        Reset();

        _db.SubmitChanges();
    }
    private void Reset()
    {
        txtLocationName.Text = "";
        txtDescription.Text = "";
        chkActive.Checked = false;
        hdnLocationId.Value = "0";
    }
}
