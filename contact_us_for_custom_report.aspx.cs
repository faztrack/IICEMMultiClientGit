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
using System.Web.Mail;

public partial class contact_us_for_custom_report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        if (!IsPostBack)
        {
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("rpt001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit" });

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        if (txtName.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Name.");
            
            return;
        }
        if (txtEmail.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: eMail.");
            
            return;
        }
        if (txtPhone.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Phone.");
            
            return;
        }
        if (txtInstruction.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Instruction.");
            
            return;
        }
        else
        {
            int nCount = Convert.ToInt32(txtInstruction.Text.Length);
            if (nCount > 500)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Instruction max character 500.");
                
                return;
            }
        }
        if (txtEmail.Text.Trim() != "")
        {
            SendMailContentToUs();
            SendMailContentToCompany();
        }

        lblResult.Text = csCommonUtility.GetSystemMessage("Custom report instruction has submitted successfully.");
         

    }
    private void SendMailContentToCompany()
    {
        string strMailBody = "Dear " + txtName.Text + ": " + Environment.NewLine +
                            "Thank you for your custom report instruction. " + Environment.NewLine +
                            "One of our company associates will contact you soon." + Environment.NewLine +
                            "Should you have any question please do not hesitate to contact us." + Environment.NewLine + Environment.NewLine +
                            "Sincerely," + Environment.NewLine +
                            "FazTimate Developing Team";

        MailMessage msgMail = new MailMessage();
        msgMail.To = txtEmail.Text.Trim();
        msgMail.From = "faztrackbd@gmail.com";
        msgMail.Subject = "Custom Report Instruction";
        msgMail.BodyFormat = MailFormat.Text;
        msgMail.Body = strMailBody;
        try
        {
            SmtpMail.SmtpServer.Insert(0, "127.0.0.1");
            SmtpMail.Send(msgMail);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private void SendMailContentToUs()
    {
        client_info oCompanyProfile = (client_info)Session["MyCompany"];
       
        string strMailBody = "One of our company has send a Custom Report Instruction." + Environment.NewLine +
                            "Company Name: " + oCompanyProfile.company_name + Environment.NewLine +
                            "Contact Person Name: " + txtName.Text + Environment.NewLine +
                            "Email: " + txtEmail.Text + Environment.NewLine +
                            "Phone: " + txtPhone.Text + Environment.NewLine +
                            "Report output format: " + ddlReportFormat.SelectedItem.Text + Environment.NewLine +
                            "Instruction: " + txtInstruction.Text;

        MailMessage msgMail = new MailMessage();
        msgMail.To = "faztrackbd@gmail.com";
        msgMail.From = txtEmail.Text.Trim();
        msgMail.Subject = "Custom Report Instruction";
        msgMail.BodyFormat = MailFormat.Text;
        msgMail.Body = strMailBody;
        try
        {
            SmtpMail.SmtpServer.Insert(0, "127.0.0.1");
            SmtpMail.Send(msgMail);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
}
