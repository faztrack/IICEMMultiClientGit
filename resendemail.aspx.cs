using System;
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
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using Prabhu;
using System.Text.RegularExpressions;
using System.Text;


public partial class resendemail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            imgCencel.Attributes.Add("onClick", "CloseWindow();");
            int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
            hdnCustomerId.Value = ncid.ToString();
            int nMessId = Convert.ToInt32(Request.QueryString.Get("MessId"));
            hdnMessageId.Value = "0";
            DataClassesDataContext _db = new DataClassesDataContext();
            if (ncid > 0 && nMessId > 0)
            {
                customer_message cust_mess = new customer_message();
                cust_mess = _db.customer_messages.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.message_id == nMessId);

                string strBody = cust_mess.mess_description;
                Regex matchNewLine = new Regex("\r\n|\r|\n", RegexOptions.Compiled | RegexOptions.Singleline);
                string MsgBody = matchNewLine.Replace(strBody, "<br />");


                txtTo.Text = cust_mess.mess_to;
                txtFrom.Text = cust_mess.mess_from;
                txtSubject.Text = cust_mess.mess_subject;
                txtCc.Text = cust_mess.mess_cc;
                //txtBcc.Text = cust_mess.mess_bcc;
                Editor1.Content = MsgBody;
                userinfo obj = (userinfo)Session["oUser"];
                txtFrom.Text = obj.company_email;

                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                if (cust.email2 != null)
                {
                    if (cust.email2.ToString().Length > 4)
                    {
                        txtCc2.Text = cust.email2;
                        Email2.Visible = true;
                    }
                    else
                    {
                        Email2.Visible = false;
                        txtCc2.Text = "";
                    }
                }
                else
                {
                    Email2.Visible = false;
                    txtCc2.Text = "";
                }



                string strQ = "select * from message_upolad_info where customer_id=" + ncid + " and message_id=" + nMessId + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                foreach (message_upolad_info message_upolad in list)
                {
                    string mess_file = message_upolad.mess_file_name.Replace("amp;", "").Trim();

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    HyperLink hyp = new HyperLink();

                    cell.BorderWidth = 0;
                    hyp.Text = mess_file;
                    hyp.NavigateUrl = "Uploads/" + ncid.ToString() + "/" + nMessId.ToString() + "/" + mess_file;
                    hyp.Target = "_blank";
                    cell.Controls.Add(hyp);
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    row.Cells.Add(cell);
                    tdLink.Rows.Add(row);
                }

                string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Test");
                string strpath = Request.PhysicalApplicationPath + "Uploads\\";
                strpath = strpath + "\\" + ncid.ToString() + "\\" + nMessId.ToString();
                if (Directory.Exists(strpath))
                {
                    if (!System.IO.Directory.Exists(NewDir))
                    {
                        System.IO.Directory.CreateDirectory(NewDir);
                    }
                    else
                    {
                        string[] fileEntries1 = Directory.GetFiles(NewDir);
                        foreach (string file1 in fileEntries1)
                        {
                            string FileName1 = Path.GetFileName(file1);
                            File.Delete(Path.Combine(NewDir, FileName1));

                        }
                    }
                    string[] fileEntries = Directory.GetFiles(strpath);
                    foreach (string file in fileEntries)
                    {
                        string FileName = Path.GetFileName(file);
                        File.Copy(file, Path.Combine(NewDir, FileName));

                    }
                }


            }
        }

    }
    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strFromEmail = txtFrom.Text;
        string strToEmail = txtTo.Text;
        string strCCEmail = txtCc.Text;
        string strCC2Email = txtCc2.Text;
        // string strBCCEmail = txtBcc.Text;

        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        if (strFromEmail.Length > 4)
        {

            Match match1 = regex.Match(strFromEmail.Trim());
            if (!match1.Success)
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("From email address " + strFromEmail + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                return;

            }
        }
        else
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("From email address is a required field");
            return;

        }

        if (strToEmail.Length > 4)
        {
            string[] strIds = strToEmail.Split(',');
            foreach (string strId in strIds)
            {
                Match match1 = regex.Match(strId.Trim());
                if (!match1.Success)
                {
                    lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Recipient email address " + strId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                    return;

                }
            }
        }
        else
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Recipient email address is a required field");
            return;

        }

        if (strCCEmail.Length > 4)
        {
            string[] strCCIds = strCCEmail.Split(',');
            foreach (string strCCId in strCCIds)
            {
                Match match1 = regex.Match(strCCId.Trim());
                if (!match1.Success)
                {
                    lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("CC email address " + strCCId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                    return;

                }
            }
        }
        if (strCC2Email.Length > 4)
        {
            string[] strCC2Ids = strCC2Email.Split(',');
            foreach (string strCC2Id in strCC2Ids)
            {
                Match match1 = regex.Match(strCC2Id.Trim());
                if (!match1.Success)
                {
                    lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("CC (Email 2) email address " + strCC2Id + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
                    return;

                }
            }
        }
        //if (strBCCEmail.Length > 4)
        //{
        //    string[] strBCCIds = strBCCEmail.Split(',');
        //    foreach (string strBCCId in strBCCIds)
        //    {
        //        Match match1 = regex.Match(strBCCId.Trim());
        //        if (!match1.Success)
        //        {
        //            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("BCC email address " + strBCCId + " is not in correct format, Please enter valid email address (Ex: john@domain.com)");
        //            return;

        //        }
        //    }
        //}
        string strpath = Request.PhysicalApplicationPath + "Uploads\\";
        strpath = strpath + "\\" + hdnCustomerId.Value + "\\Test";

        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnMessageId.Value) == 0)
        {
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            hdnMessageId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = Convert.ToInt32(hdnMessageId.Value);
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = txtTo.Text;
            cus_ms.mess_from = txtFrom.Text;
            cus_ms.mess_cc = txtCc.Text;
            cus_ms.mess_bcc = "";
            cus_ms.mess_description = Editor1.Content;
            cus_ms.mess_subject = txtSubject.Text;
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;

            _db.customer_messages.InsertOnSubmit(cus_ms);

        }
        _db.SubmitChanges();
        string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnMessageId.Value);
        try
        {

            if (Directory.Exists(strpath))
            {
                // string NewDir = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//" + hdnMessageId.Value);
                if (!System.IO.Directory.Exists(NewDir))
                {
                    System.IO.Directory.CreateDirectory(NewDir);
                }
                string[] fileEntries = Directory.GetFiles(strpath);
                foreach (string file in fileEntries)
                {
                    string FileName = Path.GetFileName(file);
                    File.Move(file, Path.Combine(NewDir, FileName));
                    if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnMessageId.Value) > 0)
                    {
                        message_upolad_info mui = new message_upolad_info();
                        if (_db.message_upolad_infos.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.message_id == Convert.ToInt32(hdnMessageId.Value) && l.mess_file_name == FileName.ToString()).SingleOrDefault() == null)
                        {
                            mui.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                            mui.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            mui.message_id = Convert.ToInt32(hdnMessageId.Value); ;
                            mui.mess_file_name = FileName.ToString();
                            mui.create_date = DateTime.Now;
                            _db.message_upolad_infos.InsertOnSubmit(mui);
                        }

                    }
                }
                _db.SubmitChanges();
            }
        }
        catch (Exception ex)
        {

        }




        MailMessage msg = new MailMessage();
        if (strFromEmail.Length > 4)
            msg.From = new MailAddress(strFromEmail);
        if (strToEmail.Length > 4)
            msg.To.Add(strToEmail);
        if (strCCEmail.Length > 4)
            msg.CC.Add(strCCEmail);
        if (strCC2Email.Length > 4)
            msg.CC.Add(strCC2Email);
        //if (strBCCEmail.Length > 4)
        //    msg.Bcc.Add(strBCCEmail);
        msg.Subject = txtSubject.Text.ToString();
        msg.IsBodyHtml = true;
        msg.Body = Editor1.Content;
        msg.Priority = MailPriority.High;
        try
        {
            if (Directory.Exists(NewDir))
            {
                string[] fileEntries = Directory.GetFiles(NewDir);
                foreach (string fileName in fileEntries)
                {
                    msg.Attachments.Add(new Attachment(fileName));
                }
            }
        }
        catch (Exception ex)
        {

        }

        try
        {
            csCommonUtility.SendByLocalhost(msg);
            //SmtpClient smtp = new SmtpClient();
            //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
            //smtp.Send(msg);
            Session.Add("FromEmailPage", "Yes");
            string url = "customer_details.aspx?cid=" + hdnCustomerId.Value;
            string Script = @"<script language=JavaScript>window.close('" + url + "'); opener.document.forms[0].submit(); </script>";
            if (!IsClientScriptBlockRegistered("OpenFile"))
                this.RegisterClientScriptBlock("OpenFile", Script);
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        HttpFileCollection fileCollection = Request.Files;
        for (int i = 0; i < fileCollection.Count; i++)
        {
            string DestinationPath = Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Test");

            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            HttpPostedFile uploadfile = fileCollection[i];
            // string fileName = Path.GetFileNameWithoutExtension(uploadfile.FileName);
            string fileName = "";
            string fileExt = Path.GetExtension(uploadfile.FileName);
            string originalFileName = Path.GetFileNameWithoutExtension(uploadfile.FileName);
            if (uploadfile.ContentLength > 0)
            {
                fileName = originalFileName + '_' + DateTime.Now.Ticks.ToString() + fileExt;
                uploadfile.SaveAs(Server.MapPath("~/Uploads//" + hdnCustomerId.Value + "//Test//") + fileName);
                lblMessage.Text += csCommonUtility.GetSystemMessage(fileName + "  Attachment(s) uploaded successfully<br>");
            }
            string[] fileEntries = Directory.GetFiles(DestinationPath);
            tdLink.Rows.Clear();
            foreach (string file in fileEntries)
            {
                string FileName = Path.GetFileName(file);
                //File.Delete(Path.Combine(NewDir, FileName));
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink hyp = new HyperLink();

                cell.BorderWidth = 0;
                hyp.Text = FileName;
                hyp.NavigateUrl = "Uploads/" + hdnCustomerId.Value + "/" + "Test" + "/" + FileName;
                hyp.Target = "_blank";
                cell.Controls.Add(hyp);
                cell.HorizontalAlign = HorizontalAlign.Left;
                row.Cells.Add(cell);
                tdLink.Rows.Add(row);

            }
        }
    }
}
