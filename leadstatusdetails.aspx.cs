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

public partial class leadstatusdetails : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin040") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nlid = Convert.ToInt32(Request.QueryString.Get("lid"));
            hdnLeadId.Value = nlid.ToString();

            if (Convert.ToInt32(hdnLeadId.Value) > 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                lead_status ls = new lead_status();
                ls = _db.lead_status.Single(l => l.lead_status_id == Convert.ToInt32(hdnLeadId.Value));
                txtLeadName.Text = ls.lead_status_name;
                chkActive.Checked = Convert.ToBoolean(ls.is_active);
                txtDescription.Text = ls.description;
                lblLeadStatus.Text = "Lead Status Details";
            }
            else
            {
                hdnLeadId.Value = "0";               
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("leadstatus_list.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        if (txtLeadName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Lead Status Name.");
            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        // Lead status Name Does Exist
        lead_status ls = new lead_status();
        if (Convert.ToInt32(hdnLeadId.Value) > 0)
            ls = _db.lead_status.Single(l => l.lead_status_id == Convert.ToInt32(hdnLeadId.Value));
        else
            if (_db.lead_status.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.lead_status_name == txtLeadName.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Lead status name '" + txtLeadName.Text.Trim() + "' already exist. Please try another name.");
                
                txtLeadName.Focus();
                return;
            }
        ls.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        ls.lead_status_name = txtLeadName.Text;
        ls.description = txtDescription.Text;
        ls.is_active = Convert.ToBoolean(chkActive.Checked);
        if (Convert.ToInt32(hdnLeadId.Value) == 0)
        {
            _db.lead_status.InsertOnSubmit(ls);
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead status '" + txtLeadName.Text.Trim() + "' has been saved successfully.  You may enter another lead status.");
             
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Lead status '" + txtLeadName.Text.Trim() + "' has been updated successfully.");
             
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
