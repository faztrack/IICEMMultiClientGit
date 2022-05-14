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

public partial class emailTemplateList : System.Web.UI.Page
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

            BindGridTemplate();
        }
    }

    protected void BindGridTemplate()
    {
        string sql = @"select * from EmailTemplateInfo";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        //  grdEmailTemplate.PageSize = 200;
        //  grdEmailTemplate.PageIndex = pageNo;
        grdEmailTemplate.DataKeyNames = new string[] { "EmailTemplateId" };
        grdEmailTemplate.DataSource = dt;
        grdEmailTemplate.DataBind();
    }


    //protected void btnSave_Click(object sender, EventArgs e)
    //{
    //    try
    //    {

    //        if (txtName.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("Name box empty");
    //            return;
    //        }
    //        if(txtTo.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("To email not found");
    //            return;
    //        }
    //        if (txtFrom.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("From email not found");
    //            return;
    //        }
    //        if (txtCc.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("CC not found");
    //            return;
    //        }
    //        if (txtBcc.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("BCC not found");
    //            return;
    //        }
    //        if (txtSubject.Text == "")
    //        {
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("Subject not found");
    //            return;
    //        }



    //        userinfo objUser = (userinfo)Session["oUser"];

    //        DataClassesDataContext _db = new DataClassesDataContext();
    //        EmailTemplate emTemplate = new EmailTemplate();

    //        if (Convert.ToInt32(hdnEmailTemplateId.Value) > 0)
    //        {
    //            emTemplate = _db.EmailTemplates.FirstOrDefault(x => x.EmailTemplateId == Convert.ToInt32(hdnEmailTemplateId.Value));

    //            if(emTemplate == null)
    //            {
    //                lblMsg.Text = csCommonUtility.GetSystemErrorMessage("Data not found");
    //                return;
    //            }
    //        }

    //        emTemplate.Name = txtName.Text;
    //        emTemplate.ToEmailAddress = txtTo.Text;
    //        emTemplate.FromAddress = txtFrom.Text;
    //        emTemplate.CcAddress = txtCc.Text;
    //        emTemplate.BccAddress = txtBcc.Text;
    //        emTemplate.Subject = txtSubject.Text;
    //        emTemplate.Message = txtBody.Content;

    //        emTemplate.LastUpdatedDate = DateTime.Now;
    //        emTemplate.LastUpdatedUserId = objUser.user_id;

    //        if (Convert.ToInt32(hdnEmailTemplateId.Value) == 0)
    //        {
    //            emTemplate.CreatedDate = DateTime.Now;
    //            emTemplate.CreatedUserId = objUser.user_id;
    //            _db.EmailTemplates.InsertOnSubmit(emTemplate);
    //            _db.SubmitChanges();
    //            lblMsg.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");
    //        }           

    //        _db.SubmitChanges();
    //        lblMsg.Text = csCommonUtility.GetSystemMessage("Data has been update successfully");


    //        ClearAll();
    //        BindGridTemplate();
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMsg.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

    //    }
    //}




    protected void imgEditBtn_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton imgBtn = (ImageButton)sender;
        int id = Convert.ToInt32(imgBtn.CommandArgument);
        Response.Redirect("emailtemplate.aspx?id=" + id);
    }

    protected void btnAddNewEmail_Click(object sender, EventArgs e)
    {
        Response.Redirect("emailtemplate.aspx");
    }
}
