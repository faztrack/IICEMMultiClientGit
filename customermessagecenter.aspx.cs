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
using System.Collections.Generic;
using Prabhu;
using System.Net.Mail;
using System.Drawing;

public partial class customermessagecenter : System.Web.UI.Page
{
    protected int sCustId = 0;
    //public const string DisplayEmailLink = "<a href=\"DisplayPop3Email.aspx?emailId={0}\" target=_blank>{1}</a>";
    //public const string Host = "west.exch031.serverdata.net";
    //public const int Port = 995;
    //public string Email;
    //public string Password;
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
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }
            if (Page.User.IsInRole("admin035") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));

            if (Request.QueryString.Get("eid") != null)
            {
                int neid = Convert.ToInt32(Request.QueryString.Get("eid"));
                hdnEstimateId.Value = neid.ToString();
            }

            sCustId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();
            HyperLink1.Attributes.Add("onClick", "DisplayWindow();");
            Session.Add("CustomerId", hdnCustomerId.Value);
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;

                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;

                hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));
                var resultCount = (from ce in _db.customer_estimates
                                   where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == 1 && ce.status_id == 3
                                   select ce.estimate_id);
                int nEstCount = resultCount.Count();
                if (nEstCount == 0)
                {
                    trWelcome.Visible = false;

                    GetEstimate(Convert.ToInt32(hdnCustomerId.Value));
                    //GetEmail(0);
                    if (ddlEstimate.Items.Count > 0)
                    {
                        followup_message followMsg = new followup_message();
                        if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 1 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                        {
                            followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 1 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                            lblInitial.Text = "Initial eMail was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                            lblInitial.CssClass = "imgBtnTxtGreen";
                            btnSendInitailMail.Visible = false;
                            lblfollow2.Text = "2nd eMail with follow-up scheduled on " + Convert.ToDateTime(followMsg.create_date).AddDays(3).ToShortDateString();
                            trfollow2.Visible = true;
                            if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 2 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                            {
                                followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 2 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                                lblfollow2.Text = "2nd eMail with follow-up was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                                lblfollow2.CssClass = "imgBtnTxtGreen";
                                btnSend2FollowUp.Visible = false;
                                trfollow3.Visible = true;
                            }
                            else
                            {
                                trfollow3.Visible = false;

                            }
                            if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 3 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                            {
                                followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 3 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                                lblfollow3.Text = "3rd eMail with Coupon was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                                lblfollow3.CssClass = "imgBtnTxtGreen";
                                btnSend3FollowUp.Visible = false;
                                trfollow3.Visible = true;
                            }

                        }
                        else
                        {
                            trfollow2.Visible = false;
                            trfollow3.Visible = false;
                            //newly added 102117
                            trWelcome.Visible = false;
                            trfollow1.Visible = false;
                            lblPendingEst.Visible = false;
                            ddlEstimate.Visible = false;

                        }
                    }
                    else
                    {
                        trfollow2.Visible = false;
                        trfollow3.Visible = false;
                        //newly added 102117
                        trWelcome.Visible = false;
                        trfollow1.Visible = false;
                        lblPendingEst.Visible = false;
                        ddlEstimate.Visible = false;

                    }
                }
                else
                {
                    followup_message followMsg = new followup_message();
                    trWelcome.Visible = false;
                    trfollow1.Visible = false;
                    trfollow2.Visible = false;
                    trfollow3.Visible = false;
                    lblPendingEst.Visible = false;
                    ddlEstimate.Visible = false;
                    if (_db.followup_messages.Where(ep => ep.mess_type_id == 4 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                    {
                        followMsg = _db.followup_messages.Single(ep => ep.mess_type_id == 4 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                        lblWelcome.Text = "Welcome Email sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                        lblWelcome.CssClass = "imgBtnTxtGreen";
                        btnSendWelcome.Visible = false;
                    }
                }
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] {  "btnSendInitailMail", "HyperLink1", "ddlEstimate", "hypTextMsg", "btnSendWelcome" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "hypMessageDetails" });
        }
        else
        {
            if (Session["FromEmailPage"] != null)
            {
                Session.Remove("FromEmailPage");
                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

            }
        }


    }
    private void GetEstimate(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select * from customer_estimate where customer_id=" + nCustId + " and client_id= 1 and status_id != 3 order by estimate_id desc ";
        IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);
        ddlEstimate.DataSource = clist;
        ddlEstimate.DataTextField = "estimate_name";
        ddlEstimate.DataValueField = "estimate_id";
        ddlEstimate.DataBind();


    }

    private void GetCustomerMessageInfo(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            var mess = from mess_info in _db.customer_messages
                       where mess_info.customer_id == nCustId && mess_info.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                       orderby mess_info.cust_message_id descending
                       select mess_info;
            grdCustomersMessage.DataSource = mess;
            grdCustomersMessage.DataKeyNames = new string[] { "customer_id", "message_id", "AttachmentList", "mess_from", "mess_to", "Type" };
            grdCustomersMessage.DataBind(); 
        }

    }
    protected void grdCustomersMessage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int MessId = Convert.ToInt32(grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int CustId = Convert.ToInt32(grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[0].ToString());

            if (e.Row.Cells[7].Text.Equals("True"))
                e.Row.Cells[7].Text = "Yes";
            else
                e.Row.Cells[7].Text = "";

            System.Web.UI.WebControls.Table tdLink = (System.Web.UI.WebControls.Table)e.Row.FindControl("tdLink");
            string strQ = "select * from message_upolad_info where customer_id=" + CustId + " and message_id=" + MessId + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

            foreach (message_upolad_info message_upolad in list)
            {
                string mess_file = message_upolad.mess_file_name.Replace("amp;", "").Trim();
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink hyp = new HyperLink();

                cell.BorderWidth = 0;
                hyp.Text = mess_file;
                hyp.NavigateUrl = "Uploads/" + CustId.ToString() + "/" + MessId.ToString() + "/" + mess_file;
                hyp.Target = "_blank";
                cell.Controls.Add(hyp);
                cell.HorizontalAlign = HorizontalAlign.Left;
                row.Cells.Add(cell);
                tdLink.Rows.Add(row);
            }
           

            HyperLink hypMessageDetails = (HyperLink)e.Row.FindControl("hypMessageDetails");
            hypMessageDetails.ToolTip = "Click on Message Details to view specific Message Details .";
            //hypMessageDetails.NavigateUrl = "messagedetails.aspx?custId=" + CustId + "&MessId=" + MessId;
            //hypMessageDetails.Target = "MyWindow";
            string script = String.Format("GetdatakeyValue1('{0}')", MessId.ToString());
            hypMessageDetails.Attributes.Add("onclick", script);

            HyperLink hypResend = (HyperLink)e.Row.FindControl("hypResend");
            //hypResend.NavigateUrl = "resendemail.aspx?custId=" + CustId + "&MessId=" + MessId;
            //hypResend.Target = "MyWindow";
            hypResend.ToolTip = "Click on Resend Message to Resend specific Message.";
            string script1 = String.Format("GetdatakeyValue3('{0}')", MessId.ToString());
            hypResend.Attributes.Add("onclick", script1);



        }
    }
    //private void GetEmail(int nPageNo)
    //{
    //    userinfo obj = (userinfo)Session["oUser"];

    //    string strEmail = "";
    //    string strPass = "";
    //    if (obj.is_verify)
    //    {
    //        strEmail = obj.company_email;
    //        strPass = obj.email_password;
    //    }
    //    else
    //    {
    //        strEmail = "info@interiorinnovations.biz";
    //        strPass = "Brody2";
    //    }

    //    if (strEmail != "" && strPass != "" && strEmail.Contains("@interiorinnovations.biz"))
    //    {
    //        Session["email"] = strEmail.ToString();
    //        Session["pwd"] = strPass.ToString();

    //        string strCusEmail = "";
    //        //strCusEmail = "@cox.net";
    //        if (lblEmail.Text.Trim().Length > 3)
    //            strCusEmail = lblEmail.Text;

    //        DataTable dt = new DataTable();
    //        dt.Columns.Add("NoCell", typeof(int));
    //        dt.Columns.Add("From", typeof(string));
    //        dt.Columns.Add("Subject", typeof(string));
    //        dt.Columns.Add("Date", typeof(DateTime));

    //        Email = strEmail.ToString();
    //        Password = strPass.ToString();

    //        int totalEmails;
    //        List<Email> emails;
    //        string emailAddress;
    //        using (Prabhu.Pop3Client client = new Prabhu.Pop3Client(Host, Port, Email, Password, true))
    //        {
    //            emailAddress = client.Email;
    //            client.Connect();
    //            totalEmails = client.GetEmailCount();
    //            emails = client.FetchEmailList(0, totalEmails);
    //            emails.Reverse();
    //        }

    //        for (int i = 0; i < emails.Count; i++)
    //        {
    //            Email email = emails[i];
    //            if (email.From.Contains(strCusEmail))
    //            {
    //                int emailId = i + 1;

    //                DataRow dr = dt.NewRow();
    //                dr["NoCell"] = emailId;
    //                dr["From"] = email.From;
    //                dr["Subject"] = email.Subject;
    //                if (email.UtcDateTime != DateTime.MinValue)
    //                    dr["Date"] = email.UtcDateTime.ToString();
    //                dt.Rows.Add(dr);
    //            }
    //        }

    //        if (dt.Rows.Count > 0)
    //        {

    //            grdEmailData.PageIndex = nPageNo;
    //            grdEmailData.DataKeyNames = new string[] { "NoCell", "Subject" };
    //            grdEmailData.DataSource = dt;
    //            grdEmailData.DataBind();
    //        }
    //    }
    //}

    //protected void grdEmailData_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    GetEmail(e.NewPageIndex);

    //}
    //protected void grdEmailData_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {

    //        HyperLink hypReplyMessage = (HyperLink)e.Row.FindControl("hypReplyMessage");

    //        HyperLink hypSubject = (HyperLink)e.Row.FindControl("hypSubject");
    //        int nNoCell = Convert.ToInt32(grdEmailData.DataKeys[e.Row.RowIndex].Values[0]);
    //        string strSubject = grdEmailData.DataKeys[e.Row.RowIndex].Values[1].ToString();
    //        hypSubject.Text = strSubject;
    //        hypSubject.ToolTip = "Click on a Subject to view specific email details.";
    //        hypSubject.NavigateUrl = "DisplayPop3Email.aspx?emailId=" + nNoCell;
    //        hypSubject.Target = "MyWindow";
    //        string script1 = String.Format("GetdatakeyValue2('{0}')", grdEmailData.DataKeys[e.Row.RowIndex].Values[0].ToString());
    //        hypSubject.Attributes.Add("onclick", script1);

    //        hypReplyMessage.NavigateUrl = "replaymail.aspx?custId=" + Convert.ToInt32(hdnCustomerId.Value) + "&emailId=" + nNoCell;
    //        hypReplyMessage.Target = "MyWindow";
    //        hypSubject.ToolTip = "Click on Reply to Reply specific email.";
    //        string script = String.Format("GetdatakeyValue('{0}')", grdEmailData.DataKeys[e.Row.RowIndex].Values[0].ToString());
    //        hypReplyMessage.Attributes.Add("onclick", script);

    //    }

    //}

    private void SendInitialeMailwithestimateToCustomer(string strEstimate)
    {
        string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                "<tr><td align='left'>Hello, </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>From all of us in Arizona's Interior Innovations Kitchen & Bath Design, we would like to thank you for giving us the opportunity to present you with an estimate. We look forward to providing you with our award winning craftsmanship and exceptional customer service. If you have any questions concerning your estimate, please do not hesitate to call us. </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Arizona's Interior Innovations Kitchen & Bath Design is a family owned and operated, five star rated, licensed, bonded and insured contracting company. We specialize in whole house redesign, bathroom remodeling and kitchen remodeling. With an “A” rating with the BBB, we've been in business in the Valley for more than 14 years with a talented team of tradespeople, designers and project managers - voted year after year as the Valley's Best! </td></tr>" + Environment.NewLine +
                 "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>As a family business, we take pride in building lasting relationships with our clients and helping them build or redesign the homes they’ve always dreamed of. And now we look forward to adding you to our family of customers (more than 5,000 strong) we are so proud to serve. </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>President</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Arizona's Interior Innovations Kitchen & Bath Design</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Phone: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Fax: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr></table>";

        string strToEmail = lblEmail.Text;
        if (strToEmail.Length > 4)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            // hdnMessageId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = nMsId;
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = strToEmail;
            cus_ms.mess_from = "info@interiorinnovations.biz";
            cus_ms.mess_cc = "";
            cus_ms.mess_bcc = "info@interiorinnovations.biz";
            cus_ms.mess_description = strTable;
            cus_ms.mess_subject = "Thank you from Arizona's Interior Innovations Kitchen & Bath Design";
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;
            _db.customer_messages.InsertOnSubmit(cus_ms);

            followup_message followMsg = new followup_message();
            followMsg.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            followMsg.message_id = nMsId;
            followMsg.estimate_id = Convert.ToInt32(ddlEstimate.SelectedValue);
            followMsg.mess_type_id = 1;
            followMsg.sent_by = strUser;
            followMsg.create_date = DateTime.Now;
            followMsg.EstimateName = ddlEstimate.SelectedItem.Text;
            followMsg.NextEmailScheduleDate = Convert.ToDateTime(followMsg.create_date).AddDays(3);
            followMsg.mess_to = strToEmail;
            followMsg.CustomerName = lblCustomerName.Text;


            _db.followup_messages.InsertOnSubmit(followMsg);

            string strQ = "UPDATE customers SET status_id = 2 WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value); //FollowUp
            _db.ExecuteCommand(strQ, string.Empty);
            _db.SubmitChanges();



            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("info@interiorinnovations.biz");
            msg.To.Add(strToEmail.Trim());
            msg.Bcc.Add(" info@interiorinnovations.biz,tislam@faztrack.com");
            msg.Subject = "Thank you from Arizona's Interior Innovations Kitchen & Bath Design";
            msg.IsBodyHtml = true;
            msg.Body = strTable;
            msg.Priority = MailPriority.High;

            try
            {
                csCommonUtility.SendByLocalhost(msg);
                // SmtpClient smtp = new SmtpClient();
                // smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                //smtp.Send(msg);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                return;
            }
        }

    }

    private void Send2ndFollowupeMailToCustomer(string strEstimate)
    {
        string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                "<tr><td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design! </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>We are just following-up to see if you are closer to making a decision on the estimate we sent you. We understand that it can be a challenge to find a competent partner to fulfill your home improvement needs. Your concerns are very important to us and we will do whatever it takes to make your project a success.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Please let us know if you have any questions. We look forward to hearing from you soon. </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>President</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Arizona's Interior Innovations Kitchen & Bath Design</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Phone: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Fax: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr></table>";

        string strToEmail = lblEmail.Text;
        if (strToEmail.Length > 4)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            // hdnMessageId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = nMsId;
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = strToEmail;
            cus_ms.mess_from = "info@interiorinnovations.biz";
            cus_ms.mess_cc = "";
            cus_ms.mess_bcc = "info@interiorinnovations.biz";
            cus_ms.mess_description = strTable;
            cus_ms.mess_subject = "Follow-up from Arizona's Interior Innovations Kitchen & Bath Design";
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;


            _db.customer_messages.InsertOnSubmit(cus_ms);

            followup_message followMsg = new followup_message();
            followMsg.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            followMsg.message_id = nMsId;
            followMsg.estimate_id = Convert.ToInt32(ddlEstimate.SelectedValue);
            followMsg.mess_type_id = 2;
            followMsg.sent_by = strUser;
            followMsg.create_date = DateTime.Now;
            followMsg.EstimateName = ddlEstimate.SelectedItem.Text;
            followMsg.NextEmailScheduleDate = DateTime.Now;
            followMsg.mess_to = strToEmail;
            followMsg.CustomerName = lblCustomerName.Text;

            _db.followup_messages.InsertOnSubmit(followMsg);
            _db.SubmitChanges();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("info@interiorinnovations.biz");
            msg.To.Add(strToEmail.Trim());
            msg.Bcc.Add(" info@interiorinnovations.biz, tislam@faztrack.com");
            msg.Subject = "Follow-up from Arizona's Interior Innovations Kitchen & Bath Design";
            msg.IsBodyHtml = true;
            msg.Body = strTable;
            msg.Priority = MailPriority.High;

            try
            {
                csCommonUtility.SendByLocalhost(msg);
                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                //smtp.Send(msg);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                return;
            }
        }

    }

    private void Send3rdFollowupWithCuponToCustomer(string strEstimate)
    {
        string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                "<tr><td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design!</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Just checking in to see if you're one step closer to working with us. We understand that budget can be a concern in making a decision on which contractor to work with. In order to help ease your concern, we are including a coupon to put towards your estimate. This is a limited time offer valid for 30 days. </td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Please let us know if you'd like to take advantage of this offer and join the 5,000 satisfied customers that have worked with us. We look forward to hearing from you soon.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>President</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Arizona's Interior Innovations Kitchen & Bath Design</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Phone 1: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Fax: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr></table>";

        string strToEmail = lblEmail.Text;
        if (strToEmail.Length > 4)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            // hdnMessageId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = nMsId;
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = strToEmail;
            cus_ms.mess_from = "info@interiorinnovations.biz";
            cus_ms.mess_cc = "";
            cus_ms.mess_bcc = "info@interiorinnovations.biz";
            cus_ms.mess_description = strTable;
            cus_ms.mess_subject = "Follow-up from Arizona's Interior Innovations Kitchen & Bath Design";
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;

            _db.customer_messages.InsertOnSubmit(cus_ms);

            followup_message followMsg = new followup_message();
            followMsg.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            followMsg.message_id = nMsId;
            followMsg.estimate_id = Convert.ToInt32(ddlEstimate.SelectedValue);
            followMsg.mess_type_id = 3;
            followMsg.sent_by = strUser;
            followMsg.create_date = DateTime.Now;
            followMsg.EstimateName = ddlEstimate.SelectedItem.Text;
            followMsg.NextEmailScheduleDate = DateTime.Now;
            followMsg.mess_to = strToEmail;
            followMsg.CustomerName = lblCustomerName.Text;

            _db.followup_messages.InsertOnSubmit(followMsg);
            _db.SubmitChanges();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("info@interiorinnovations.biz");
            msg.To.Add(strToEmail.Trim());
            msg.Bcc.Add(" info@interiorinnovations.biz, tislam@faztrack.com");
            msg.Subject = "Follow-up from Arizona's Interior Innovations Kitchen & Bath Design";
            msg.IsBodyHtml = true;
            msg.Body = strTable;
            msg.Priority = MailPriority.High;

            try
            {
                csCommonUtility.SendByLocalhost(msg);
                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                //smtp.Send(msg);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                return;
            }
        }

    }

    private void SendWelcomeToCustomer()
    {
        string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                "<tr><td align='left'>Hello from Arizona's Interior Innovations Kitchen & Bath Design!</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>We're delighted that you've selected to work with us. We understand the gravity of trust you've imparted in us to help build your dream project and look forward to providing nothing short of exceptional customer service and fantastic craftsmanship.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Should you have any additional questions about your estimate, please reach out. In the meantime, please stay tuned for a phone call from one of our top notch project coordinators to go over next steps.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                 "<tr><td align='left'>We're honored to be a part of your project's process and can't wait to work with you!</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>With gratitude,</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>President</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Arizona's Interior Innovations Kitchen & Bath Design</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Phone : 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Fax: 520- 461-1570</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr></table>";

        string strToEmail = lblEmail.Text;
        if (strToEmail.Length > 4)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nMsId = 0;
            var result = (from cm in _db.customer_messages
                          where cm.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cm.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select cm.message_id);

            int n = result.Count();
            if (result != null && n > 0)
                nMsId = result.Max();
            nMsId = nMsId + 1;
            // hdnMessageId.Value = nMsId.ToString();
            userinfo obj = (userinfo)Session["oUser"];
            string strUser = obj.first_name + " " + obj.last_name;

            customer_message cus_ms = new customer_message();
            cus_ms.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_ms.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_ms.message_id = nMsId;
            cus_ms.sent_by = strUser;
            cus_ms.mess_to = strToEmail;
            cus_ms.mess_from = "info@interiorinnovations.biz";
            cus_ms.mess_cc = "";
            cus_ms.mess_bcc = "info@interiorinnovations.biz";
            cus_ms.mess_description = strTable;
            cus_ms.mess_subject = "Welcome from Arizona's Interior Innovations Kitchen & Bath Design";
            cus_ms.last_view = DateTime.Now;
            cus_ms.create_date = DateTime.Now;

            _db.customer_messages.InsertOnSubmit(cus_ms);

            followup_message followMsg = new followup_message();
            followMsg.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            followMsg.message_id = nMsId;
            followMsg.estimate_id = 0;
            followMsg.mess_type_id = 4;
            followMsg.sent_by = strUser;
            followMsg.create_date = DateTime.Now;
            followMsg.EstimateName = "";
            followMsg.NextEmailScheduleDate = DateTime.Now;
            followMsg.mess_to = strToEmail;
            followMsg.CustomerName = lblCustomerName.Text;

            _db.followup_messages.InsertOnSubmit(followMsg);
            _db.SubmitChanges();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("info@interiorinnovations.biz");
            msg.To.Add(strToEmail.Trim());
            msg.Bcc.Add(" info@interiorinnovations.biz, tislam@faztrack.com");
            msg.Subject = "Welcome from Arizona's Interior Innovations Kitchen & Bath Design";
            msg.IsBodyHtml = true;
            msg.Body = strTable;
            msg.Priority = MailPriority.High;

            try
            {
                csCommonUtility.SendByLocalhost(msg);
                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                //smtp.Send(msg);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                return;
            }
        }

    }


    protected void ddlEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlEstimate.ID, ddlEstimate.GetType().Name, "SelectedIndexChanged"); 

        DataClassesDataContext _db = new DataClassesDataContext();
        if (ddlEstimate.Items.Count > 0)
        {
            followup_message followMsg = new followup_message();
            if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 1 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
            {
                followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 1 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                lblInitial.Text = "Initial eMail was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                lblInitial.CssClass = "imgBtnTxtGreen";
                btnSendInitailMail.Visible = false;
                lblfollow2.Text = "2nd eMail with follow-up scheduled on " + Convert.ToDateTime(followMsg.create_date).AddDays(3).ToShortDateString();
                trfollow2.Visible = true;
                if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 2 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                {
                    followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 2 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    lblfollow2.Text = "2nd eMail with follow-up was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                    lblfollow2.CssClass = "imgBtnTxtGreen";
                    btnSend2FollowUp.Visible = false;
                    trfollow3.Visible = true;
                }
                else
                {
                    trfollow3.Visible = false;

                }
                if (_db.followup_messages.Where(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 3 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
                {
                    followMsg = _db.followup_messages.Single(ep => ep.estimate_id == Convert.ToInt32(ddlEstimate.SelectedValue) && ep.mess_type_id == 3 && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    lblfollow3.Text = "3rd eMail with Coupon was sent on " + Convert.ToDateTime(followMsg.create_date).ToShortDateString();
                    lblfollow3.CssClass = "imgBtnTxtGreen";
                    btnSend3FollowUp.Visible = false;
                    trfollow3.Visible = true;
                }

            }
            else
            {
                trfollow2.Visible = false;
                trfollow3.Visible = false;

            }
        }


    }
    protected void btnSendInitailMail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSendInitailMail.ID, btnSendInitailMail.GetType().Name, "Click"); 
        SendInitialeMailwithestimateToCustomer(ddlEstimate.SelectedItem.Text);
        lblResult.Text = csCommonUtility.GetSystemMessage("Initial eMail sent successfully");
        lblResult1.Text = csCommonUtility.GetSystemMessage("Initial eMail sent successfully");
        lblInitial.Text = "Initial eMail was sent on " + DateTime.Now.ToShortDateString();
        lblInitial.CssClass = "imgBtnTxtGreen";
        btnSendInitailMail.Visible = false;
        trfollow2.Visible = true;
        lblfollow2.Text = "2nd eMail with follow-up scheduled on " + DateTime.Now.AddDays(3).ToShortDateString();
        GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));
    }
    protected void btnSend2FollowUp_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSend2FollowUp.ID, btnSend2FollowUp.GetType().Name, "Click"); 
        Send2ndFollowupeMailToCustomer(ddlEstimate.SelectedItem.Text);
        lblResult.Text = csCommonUtility.GetSystemMessage("2nd eMail with follow-up sent successfully");
        trfollow3.Visible = true;
        lblfollow2.Text = "2nd eMail with follow-up sent on " + DateTime.Now.ToShortDateString();
        lblfollow2.CssClass = "imgBtnTxtGreen";
        btnSend2FollowUp.Visible = false;
        GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

    }
    protected void btnSend3FollowUp_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSend3FollowUp.ID, btnSend3FollowUp.GetType().Name, "Click"); 
        Send3rdFollowupWithCuponToCustomer(ddlEstimate.SelectedItem.Text);
        lblResult.Text = csCommonUtility.GetSystemMessage("3rd eMail with Coupon sent successfully");
        lblResult2.Text = csCommonUtility.GetSystemMessage("3rd eMail with Coupon sent successfully");
        lblfollow3.Text = "3rd eMail with coupon sent on " + DateTime.Now.ToShortDateString();
        lblfollow3.CssClass = "imgBtnTxtGreen";
        btnSend3FollowUp.Visible = false;
        GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

    }
    protected void btnSendWelcome_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSendWelcome.ID, btnSendWelcome.GetType().Name, "Click"); 
        SendWelcomeToCustomer();
        lblResult.Text = csCommonUtility.GetSystemMessage("Welcome Email sent successfully");
        lblWelcomeResult.Text = csCommonUtility.GetSystemMessage("Welcome Email sent successfully");
        lblWelcome.Text = "Welcome Email sent on " + DateTime.Now.ToShortDateString();
        lblWelcome.CssClass = "imgBtnTxtGreen";
        btnSendWelcome.Visible = false;
        GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

    }

}
