using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

public partial class customerprofile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oCustomerUser"] == null)
            {
                Response.Redirect("customerlogin.aspx");
            }
            if (Session["oCustomerUser"] != null)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
                int nCustomerId = Convert.ToInt32(obj.customerid);

                hdnCustomerId.Value = nCustomerId.ToString();

                customeruserinfo objCU = new customeruserinfo();
                objCU = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));
                ddlSecurityQuestion.SelectedValue = objCU.securityquestion;
                txtAnswer.Text = objCU.answer;

                pnlChangePassword.Visible = true;
                pnlSecurityQuestion.Visible = false;

            }
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        if (txtCurrentPassword.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: Current Password.");
            
            return;
        }
        else
        {
            customeruserinfo objCU = new customeruserinfo();
            objCU = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));

            if (txtCurrentPassword.Text.Trim() != objCU.customerpassword)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Current Password.");
                
                return;
            }
        }
        if (txtNewPassword.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: New Password.");
            
            return;
        }
        else
        {
            if (Convert.ToInt32(txtNewPassword.Text.Length) < 6)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Password length should be minimum 6");
                
                return;
            }
            if (txtConfirmPassword.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required file: Confirm Password.");
                
                return;
            }
            if (txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please confirm password.");
                
                return;
            }
        }

        string strNewPassword = txtNewPassword.Text.Trim();

        string strQ = "UPDATE customeruserinfo SET customerpassword = '" + strNewPassword + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        lblResult.Text = csCommonUtility.GetSystemMessage("Password Saved Successfully.");
         


    }
    protected void lnkChangePassword_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkChangePassword.ID, lnkChangePassword.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMessage.Text = "";
        pnlChangePassword.Visible = true;
        pnlSecurityQuestion.Visible = false;
    }
    protected void lnkSecurityQuestion_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkSecurityQuestion.ID, lnkSecurityQuestion.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMessage.Text = "";
        pnlChangePassword.Visible = false;
        pnlSecurityQuestion.Visible = true;
    }
    protected void btnSaveAnswer_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveAnswer.ID, btnSaveAnswer.GetType().Name, "Click"); 
        lblMessage.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        if (txtAnswer.Text == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Answer.");
             
            return;
        }

        string strQuestion = ddlSecurityQuestion.SelectedItem.Text;
        string strAnswer = txtAnswer.Text.Trim();


        string strQ = "UPDATE customeruserinfo SET answer = '" + strAnswer.Replace("'", "''") + "', securityquestion = '" + strQuestion + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        lblMessage.Text = csCommonUtility.GetSystemMessage("Answer Saved Successfully.");
        
        customeruserinfo obj = new customeruserinfo();
        obj = _db.customeruserinfos.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value));
        ddlSecurityQuestion.SelectedValue = obj.securityquestion;
        txtAnswer.Text = obj.answer;
    }
}
