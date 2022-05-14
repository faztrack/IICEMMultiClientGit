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

public partial class emailtemplate : System.Web.UI.Page
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
            else
            {
                userinfo objUser = (userinfo)Session["oUser"];
            }
            if (Page.User.IsInRole("admin010") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            if ((Request.QueryString.Get("id") != null) && (Convert.ToInt32(Request.QueryString.Get("id")) > 0))
            {
                EditEmailTemplate(Convert.ToInt32(Request.QueryString.Get("id")));
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSave" });
        }

    }

    private void EditEmailTemplate(int EmailTemplateId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            EmailTemplateInfo emTemplate = _db.EmailTemplateInfos.FirstOrDefault(x => x.EmailTemplateId == EmailTemplateId);

            if (emTemplate == null)
            {
                lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Data not found");
                return;
            }

            txtName.Text = emTemplate.Name;
            txtTo.Text = emTemplate.ToEmailAddress;
            txtFrom.Text = emTemplate.FromAddress;
            txtCc.Text = emTemplate.CcAddress;
            txtBcc.Text = emTemplate.BccAddress;
            txtSubject.Text = emTemplate.Subject;
            txtBody.Content = emTemplate.Message;

            hdnEmailTemplateId.Value = emTemplate.EmailTemplateId.ToString();

            btnSave.Text = "UPDATE";
        }
        catch (Exception ex)
        {
            lblMsg.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }
       
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
           
            if (txtName.Text == "")
            {
                lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("Name box empty");
                return;
            }
            if(txtTo.Text == "")
            {
                lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("To email not found");
                return;
            }
            if (txtFrom.Text == "")
            {
                lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("From email not found");
                return;
            }
           
            if (txtSubject.Text == "")
            {
                lblMsg.Text = csCommonUtility.GetSystemRequiredMessage("Subject not found");
                return;
            }

            

            userinfo objUser = (userinfo)Session["oUser"];

            DataClassesDataContext _db = new DataClassesDataContext();
            EmailTemplateInfo emTemplate = new EmailTemplateInfo();

            if (Convert.ToInt32(hdnEmailTemplateId.Value) > 0)
            {
                emTemplate = _db.EmailTemplateInfos.FirstOrDefault(x => x.EmailTemplateId == Convert.ToInt32(hdnEmailTemplateId.Value));

                //if (emTemplate == null)
                //{
                //    lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Data not found");
                //    return;
                //}
            }

            emTemplate.Name = txtName.Text;
            emTemplate.ToEmailAddress = txtTo.Text;
            emTemplate.FromAddress = txtFrom.Text;
            emTemplate.CcAddress = txtCc.Text;
            emTemplate.BccAddress = txtBcc.Text;
            emTemplate.Subject = txtSubject.Text;
            emTemplate.Message = txtBody.Content;

            emTemplate.LastUpdatedDate = DateTime.Now;
            emTemplate.LastUpdatedUserId = objUser.user_id;

            if (Convert.ToInt32(hdnEmailTemplateId.Value) == 0)
            {
                emTemplate.CreatedDate = DateTime.Now;
                emTemplate.CreatedUserId = objUser.user_id;
                _db.EmailTemplateInfos.InsertOnSubmit(emTemplate);
               
                lblMsg.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");
                ClearAll();
            }
            else 
            {               
                lblMsg.Text = csCommonUtility.GetSystemMessage("Data has been update successfully");
            }
            _db.SubmitChanges();


       
        }
        catch (Exception ex)
        {
            lblMsg.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

        }
    }


    protected void ClearAll()
    {
        txtName.Text = "";
        txtTo.Text = "";
        txtFrom.Text = "";
        txtCc.Text = "";
        txtBcc.Text = "";
        txtSubject.Text = "";
        txtBody.Content = "";

        hdnEmailTemplateId.Value = "0";
        btnSave.Text = "SAVE";

    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("emailtemplatelist.aspx");
    }
}
