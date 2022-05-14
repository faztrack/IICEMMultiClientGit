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

public partial class companycoverletter : System.Web.UI.Page
{
    

    protected void Page_Load(object sender, EventArgs e)
    {
        hdnClientId.Value = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]).ToString();

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin010") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            company_cover_letter objComcl = new company_cover_letter();
            if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value));

                txtCoverLetter.Text = objComcl.cover_letter;
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit" });
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        if (Convert.ToInt32(txtCoverLetter.Text.Trim().Length) == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing cover letter content.");
            
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        company_cover_letter objComcl = new company_cover_letter();

        objComcl.client_id = Convert.ToInt32(hdnClientId.Value);
        objComcl.cover_letter = txtCoverLetter.Text;

        string strQ = "DELETE company_cover_letter where client_id=" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        _db.company_cover_letters.InsertOnSubmit(objComcl);
        _db.SubmitChanges();

        lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        
    }
}
