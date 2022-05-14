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

public partial class leadsourcedetails : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin009") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nlid = Convert.ToInt32(Request.QueryString.Get("lid"));
            hdnLeadId.Value = nlid.ToString();

            if (Convert.ToInt32(hdnLeadId.Value) > 0)
            {
                lblHeaderTitle.Text = "Lead Source Details";
                DataClassesDataContext _db = new DataClassesDataContext();
                lead_source ls = new lead_source();
                ls = _db.lead_sources.Single(l => l.lead_source_id == Convert.ToInt32(hdnLeadId.Value));
                txtLeadName.Text = ls.lead_name;
                chkActive.Checked = Convert.ToBoolean(ls.is_active);
                txtDescription.Text = ls.description;
            }
            else
            {
                lblHeaderTitle.Text = "Add New Lead Source";
                hdnLeadId.Value = "0";
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkActive", "btnSubmit" });
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("leadsource_list.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (txtLeadName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Lead Source Name.");
            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        // Lead Source Name Does Exist
        lead_source ls = new lead_source();
        if (Convert.ToInt32(hdnLeadId.Value) > 0)
            ls = _db.lead_sources.Single(l => l.lead_source_id == Convert.ToInt32(hdnLeadId.Value));
        else
            if (_db.lead_sources.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.lead_name == txtLeadName.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Lead source name '" + txtLeadName.Text.Trim() + "' already exist. Please try another name.");
                
                txtLeadName.Focus();
                return;
            }
        ls.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        ls.lead_name = txtLeadName.Text;
        ls.description = txtDescription.Text;
        ls.is_active = Convert.ToBoolean(chkActive.Checked);
        if (Convert.ToInt32(hdnLeadId.Value) == 0)
        {
            _db.lead_sources.InsertOnSubmit(ls);
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead source '" + txtLeadName.Text.Trim() + "' has been saved successfully.  You may enter another lead source.");
             
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead source '" + txtLeadName.Text.Trim() + "' has been updated successfully.");
             
        }

        Reset();

        _db.SubmitChanges();
    }
    private void Reset()
    {
        txtLeadName.Text = "";
        txtDescription.Text = "";
        chkActive.Checked = false;
        hdnLeadId.Value = "0";
    }
}
