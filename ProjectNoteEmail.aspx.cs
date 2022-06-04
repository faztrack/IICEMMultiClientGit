using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectNoteEmail : System.Web.UI.Page
{
    private string strCustName = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            imgCencel.Attributes.Add("onClick", "CloseWindow();");
            int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
            hdnCustomerId.Value = ncid.ToString();
            DataClassesDataContext _db = new DataClassesDataContext();

            if (ncid > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == ncid);
                sales_person sap = new sales_person();
                sap = _db.sales_persons.SingleOrDefault(c => c.sales_person_id == Convert.ToInt32(cust.sales_person_id) &&c.is_active==true);
                if (sap != null)
                {
                    txtTo.Text = sap.email;
                }

                strCustName = cust.last_name1;
                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == cust.client_id).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == 1);

                    txtCc.Text = com.email;
                    txtBcc.Text = "info@interiorinnovations.biz";
                    userinfo obj = (userinfo)Session["oUser"];
                    txtFrom.Text = obj.company_email;
                }

                txtSubject.Text = "Project Notes for (" + strCustName + ") ";
                if (Session["MessBody"] != null)
                {
                    string str = Convert.ToString(Session["MessBody"]);
                    Editor1.Content = str;

                }

                //Editor1.Content = cust_mess.mess_description;

            }
        }

    }
    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();



        string strTo = txtTo.Text.ToString();
        string strCc = txtCc.Text.ToString();
        string strbcc = txtBcc.Text.ToString();

        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(strTo);
        if (!match.Success)
            strTo = "";

        match = regex.Match(strCc);
        if (!match.Success)
            strCc = "";

        match = regex.Match(strbcc);
        if (!match.Success)
            strbcc = "";


        MailMessage msg = new MailMessage();
        msg.From = new MailAddress(txtFrom.Text.ToString());
        if (strTo != "")
            msg.To.Add(strTo);
        else
        {
            lblMessage.Text = "Please select valid recipient email";
            return;
        }
        if (strCc != "")
            msg.CC.Add(strCc);
        if (strbcc != "")
            msg.Bcc.Add(strbcc);
        msg.Bcc.Add("avijit019@gmail.com");
        msg.Subject = txtSubject.Text.ToString();
        msg.IsBodyHtml = true;
        msg.Body = Editor1.Content;
        msg.Priority = MailPriority.High;
        //try
        //{
        //    if (Directory.Exists(NewDir))
        //    {
        //        string[] fileEntries = Directory.GetFiles(NewDir);
        //        foreach (string fileName in fileEntries)
        //        {
        //            msg.Attachments.Add(new Attachment(fileName));
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{

        //}
        try
        {
            csCommonUtility.SendByLocalhost(msg);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            //smtp.Send(msg);


            string url = "ProjectNotes.aspx?cid=" + hdnCustomerId.Value;
            string Script = @"<script language=JavaScript>window.close('" + url + "'); opener.document.forms[0].submit(); </script>";
            if (!IsClientScriptBlockRegistered("OpenFile"))
                this.RegisterClientScriptBlock("OpenFile", Script);
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

}