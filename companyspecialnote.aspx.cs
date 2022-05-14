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

public partial class companyspecialnote : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        hdnClientId.Value = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]).ToString();

        if (!IsPostBack)
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin012") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            Company_special_note objCSN = new Company_special_note();
            if (_db.Company_special_notes.Where(csn => csn.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                objCSN = _db.Company_special_notes.Single(csn => csn.client_id == Convert.ToInt32(hdnClientId.Value));

                txtSpecialNote.Text = objCSN.special_note;
            }
            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit" });
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        if (Convert.ToInt32(txtSpecialNote.Text.Trim().Length) == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing special note content.");
            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        Company_special_note objCSN = new Company_special_note();

        objCSN.client_id = Convert.ToInt32(hdnClientId.Value);
        objCSN.special_note = txtSpecialNote.Text;

        string strQ = "DELETE Company_special_note where client_id=" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        _db.Company_special_notes.InsertOnSubmit(objCSN);
        _db.SubmitChanges();

        lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
}
