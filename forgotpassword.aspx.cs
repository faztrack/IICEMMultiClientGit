using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Web.Security;
using System.Configuration;
public partial class forgotpassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["ForgotPasword"] != null)
            {
                int nEmpId = Convert.ToInt32(Session["ForgotPasword"]);
                hdnEmployeeId.Value = nEmpId.ToString();

                try
                {
                    DataClassesDataContext _db = new DataClassesDataContext();
                    user_info objEmp = new user_info();
                    objEmp = _db.user_infos.Single(emp => emp.user_id == Convert.ToInt32(hdnEmployeeId.Value));

                    int nQId = Convert.ToInt32(objEmp.QuestionID);
                    Question objQ = new Question();
                    objQ = _db.Questions.Single(q => q.QuestionID == nQId);

                    lblQuestion.Text = objQ.QuestionName;
                }
                catch (Exception ex)
                {

                }

                Session.Remove("ForgotPasword");

            }
            else
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        if (txtAnswer.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Answer.");
            return;
        }
        else
        {
            if (Convert.ToInt32(hdnEmployeeId.Value) > 0)
            {
                user_info objUInfo = new user_info();
                try
                {
                    objUInfo = _db.user_infos.Single(emp => emp.user_id == Convert.ToInt32(hdnEmployeeId.Value));
                }
                catch (Exception ex)
                {

                }

                if (txtAnswer.Text.Trim() != objUInfo.Answer)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Answer.");
                    return;
                }
                if (objUInfo.email.Length > 3)
                {
                    divPasswordReset.Visible = true;
                    btnReset.Visible = true;
                    btnSubmit.Visible = false;
                }
                else
                {
                    divPasswordReset.Visible = false;
                    btnReset.Visible = false;
                    btnSubmit.Visible = true;
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Email address does not exist.");
                }
            }
        }
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnReset.ID, btnReset.GetType().Name, "Click"); 
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        if (txtAnswer.Text.Trim() == "")
        {
           
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Answer.");
            return;
        }
        
        else
        {
            if (Convert.ToInt32(hdnEmployeeId.Value) > 0)
            {
                user_info objUInfo = new user_info();
                try
                {
                    objUInfo = _db.user_infos.Single(emp => emp.user_id == Convert.ToInt32(hdnEmployeeId.Value));
                }
                catch (Exception ex)
                {

                }

                if (txtAnswer.Text.Trim() != objUInfo.Answer)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Answer.");
                    return;
                }
                if (objUInfo.email.Length > 3)
                {
                    if (txtPassword.Text.Trim() == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Password.");
                        return;
                    }
                    else
                    {
                        if (txtPassword.Text.Trim().Length < 6)
                        {
                            lblResult.Visible = true;
                            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Password length should be minimum 6");
                            return;
                        }
                    }
                    if (txtConfirmPass.Text.Trim() == "")
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Confirm Password.");
                        return;
                    }
                    if (txtPassword.Text.Trim() != txtConfirmPass.Text.Trim())
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please confirm password");
                        return;
                    }

                    objUInfo.password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text.Trim(), "SHA1");
                    _db.SubmitChanges();

                    lblQuestion.Visible = false;
                    txtAnswer.Visible = false;
                    divPasswordReset.Visible = false;
                    btnReset.Visible = false;
                    btnSubmit.Visible = false;
                    pVerificationTitle.Visible = false;

                    lblResult.Text = csCommonUtility.GetSystemMessage("Your Password has been Reset Succesfully.");
                }
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Email address does not exist.");
                }
            }
        }
    }

    private void SendForgotPassword(user_info objUInfo)
    {
        string strPassword = objUInfo.password;

        company_profile objCom = csCommonUtility.GetCompanyProfile();

        string strBody = "Dear " + objUInfo.first_name + " " + objUInfo.last_name + "," + Environment.NewLine +
                "Your Password is: " + strPassword + Environment.NewLine +
                "Should you have any question, please do not hesitate to contact us." + Environment.NewLine + Environment.NewLine +
                "Sincerely," + Environment.NewLine +
                objCom.company_name;


        //string strSMTPServer = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];

        MailMessage msg = new MailMessage();
        msg.From = new MailAddress("tislam@faztrack.com");
        msg.To.Add(objUInfo.email);
        msg.Subject = "Forgot Password Retrive";
        msg.IsBodyHtml = true;
        msg.Body = strBody;
        msg.Priority = MailPriority.High;

        try
        {
            csCommonUtility.SendByLocalhost(msg);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            // smtp.Send(msg);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            return;
        }

    }
    protected void btngotoLogin_Click(object sender, EventArgs e)
    {
        Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
    }

}