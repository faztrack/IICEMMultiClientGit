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


public partial class replaymail : System.Web.UI.Page
{

    public const string Host = "pop.gmail.com";
    public const int Port = 995;
    public string Email;
    public string Password;
    protected static Regex CharsetRegex = new Regex("charset=\"?(?<charset>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex QuotedPrintableRegex = new Regex("=(?<hexchars>[0-9a-fA-F]{2,2})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex UrlRegex = new Regex("(?<url>https?://[^\\s\"]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex FilenameRegex = new Regex("filename=\"?(?<filename>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    protected static Regex NameRegex = new Regex("name=\"?(?<filename>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected void Page_Load(object sender, EventArgs e)
    {



        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            #region Original send eamil
            if (Request.QueryString.Get("custId") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
                hdnCustomerId.Value = ncid.ToString();
            }
            imgCencel.Attributes.Add("onClick", "CloseWindow();");
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                txtTo.Text = cust.email;
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
                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == 1).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == 1);
                    userinfo obj = (userinfo)Session["oUser"];
                    txtFrom.Text = obj.company_email;
                    txtCc.Text = com.email;
                }

            }
            #endregion

            # region Replay Mail Page Load
            int emailId = -1;
            if (Request.QueryString["emailId"] == null)
            {
                Response.Redirect("replaymail.aspx");
                Response.Flush();
                Response.End();
            }
            else
            {
                Email = Session["email"].ToString();
                Password = Session["pwd"].ToString();
                emailId = Convert.ToInt32(Request.QueryString["emailId"]);
            }
            Email email = null;
            List<MessagePart> msgParts = null;
            using (Prabhu.Pop3Client client = new Prabhu.Pop3Client(Host, Port, Email, Password, true))
            {

                client.Connect();
                email = client.FetchEmail(emailId);
                msgParts = client.FetchMessageParts(emailId);
            }
            if (email == null || msgParts == null)
            {
                Response.Redirect("Pop3Client.aspx");
                Response.Flush();
                Response.End();
            }
            MessagePart preferredMsgPart = FindMessagePart(msgParts, "text/html");
            if (preferredMsgPart == null)
                preferredMsgPart = FindMessagePart(msgParts, "text/plain");
            else if (preferredMsgPart == null && msgParts.Count > 0)
                preferredMsgPart = msgParts[0];
            string contentType, charset, contentTransferEncoding, body = null;
            if (preferredMsgPart != null)
            {
                contentType = preferredMsgPart.Headers["Content-Type"];
                charset = "us-ascii";
                contentTransferEncoding = preferredMsgPart.Headers["Content-Transfer-Encoding"];
                Match m = CharsetRegex.Match(contentType);
                if (m.Success)
                    charset = m.Groups["charset"].Value;
                HeadersLiteral.Text = contentType != null ? "Content-Type: " + contentType + "<br />" : string.Empty;
                HeadersLiteral.Text += contentTransferEncoding != null ? "Content-Transfer-Encoding: " + contentTransferEncoding : string.Empty;
                if (contentTransferEncoding != null)
                {
                    if (contentTransferEncoding.ToLower() == "base64")
                        body = DecodeBase64String(charset, preferredMsgPart.MessageText);
                    else if (contentTransferEncoding.ToLower() == "quoted-printable")
                        body = DecodeQuotedPrintableString(preferredMsgPart.MessageText);
                    else
                        body = preferredMsgPart.MessageText;
                }
                else
                    body = preferredMsgPart.MessageText;
            }
            EmailIdLiteral.Text = Convert.ToString(emailId);
            DateLiteral.Text = email.UtcDateTime.ToString(); ;
            FromLiteral.Text = email.From;
            SubjectLiteral.Text = email.Subject;
            txtSubject.Text = "RE: " + email.Subject;
            BodyLiteral.Text = preferredMsgPart != null ? (preferredMsgPart.Headers["Content-Type"].IndexOf("text/plain") != -1 ? "<pre>" + FormatUrls(body) + "</pre>" : body) : null;
            //string str = HtmlToPlainText(BodyLiteral.Text);

            //TextBox1.Text = str;
            //StreamReader rdr = new StreamReader(sFileName);
            string strHtml = preferredMsgPart != null ? (preferredMsgPart.Headers["Content-Type"].IndexOf("text/plain") != -1 ? "<pre>" + FormatUrls(body) + "</pre>" : body) : null;
            Editor1.Content = "</br></br></br></br></br>" + "--------------Original message--------------" + "</br>" + "Date & Time:" + email.UtcDateTime.ToString() + "</br>" + "From:" + email.From + "</br>" + "</br>" + BodyLiteral.Text;
            ListAttachments(msgParts);
            #endregion
        }



    }
    #region send email main
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

    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "Click"); 
        string strFromEmail = txtFrom.Text;
        string strToEmail = txtTo.Text;
        string strCCEmail = txtCc.Text;
        string strCC2Email = txtCc2.Text;
        //  string strBCCEmail = txtbCc2.Text;

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
        DataClassesDataContext _db = new DataClassesDataContext();
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
    #endregion

    # region Replay Mail Main
    private static string HtmlToPlainText(string html)
    {
        const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
        const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
        const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
        var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
        var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

        var text = html;
        //Decode html specific characters
        //text = System.Net.WebUtility.HtmlDecode(text);
        text = HttpUtility.HtmlDecode(text);
        //Remove tag whitespace/line breaks
        text = tagWhiteSpaceRegex.Replace(text, "><");
        //Replace <br /> with line breaks
        text = lineBreakRegex.Replace(text, Environment.NewLine);
        //Strip formatting
        text = stripFormattingRegex.Replace(text, string.Empty);

        return text;
    }

    protected Decoder GetDecoder(string charset)
    {
        Decoder decoder;
        switch (charset.ToLower())
        {
            case "utf-7":
                decoder = Encoding.UTF7.GetDecoder();
                break;
            case "utf-8":
                decoder = Encoding.UTF8.GetDecoder();
                break;
            case "us-ascii":
                decoder = Encoding.ASCII.GetDecoder();
                break;
            case "iso-8859-1":
                decoder = Encoding.ASCII.GetDecoder();
                break;
            default:
                decoder = Encoding.ASCII.GetDecoder();
                break;
        }
        return decoder;
    }
    protected string DecodeBase64String(string charset, string encodedString)
    {
        Decoder decoder = GetDecoder(charset);
        byte[] buffer = Convert.FromBase64String(encodedString);
        char[] chararr = new char[decoder.GetCharCount(buffer, 0, buffer.Length)];
        decoder.GetChars(buffer, 0, buffer.Length, chararr, 0);
        return new string(chararr);
    }
    protected string DecodeQuotedPrintableString(string encodedString)
    {
        StringBuilder b = new StringBuilder();
        int startIndx = 0;
        MatchCollection matches = QuotedPrintableRegex.Matches(encodedString);
        for (int i = 0; i < matches.Count; i++)
        {
            Match m = matches[i];
            string hexchars = m.Groups["hexchars"].Value;
            int charcode = Convert.ToInt32(hexchars, 16);
            char c = (char)charcode;
            if (m.Index > 0)
                b.Append(encodedString.Substring(startIndx, (m.Index - startIndx)));
            b.Append(c);
            startIndx = m.Index + 3;
        }
        if (startIndx < encodedString.Length)
            b.Append(encodedString.Substring(startIndx));
        return Regex.Replace(b.ToString(), "=\r\n", "");
    }
    protected void ListAttachments(List<MessagePart> msgParts)
    {
        bool attachmentsFound = false;
        StringBuilder b = new StringBuilder();
        b.Append("<ol>");
        foreach (MessagePart p in msgParts)
        {
            string contentType = p.Headers["Content-Type"];
            string contentDisposition = p.Headers["Content-Disposition"];
            Match m;
            if (contentDisposition != null)
            {
                m = FilenameRegex.Match(contentDisposition);
                if (m.Success)
                {
                    attachmentsFound = true;
                    b.Append("<li>").Append(m.Groups["filename"].Value).Append("</li>");

                }
            }
            else if (contentType != null)
            {
                m = NameRegex.Match(contentType);

                if (m.Success)
                {
                    attachmentsFound = true;
                    b.Append("<li>").Append(m.Groups["filename"].Value).Append("</li>");
                }
            }
        }
        b.Append("</ol>");
        if (attachmentsFound)
            AttachmentsLiteral.Text = b.ToString();

    }
    protected MessagePart FindMessagePart(List<MessagePart> msgParts, string contentType)
    {
        foreach (MessagePart p in msgParts)
            if (p.ContentType != null && p.ContentType.IndexOf(contentType) != -1)
                return p;
        return null;
    }
    protected string FormatUrls(string plainText)
    {
        string replacementLink = "<a href=\"${url}\">${url}</a>";
        return UrlRegex.Replace(plainText, replacementLink);
    }
    #endregion

    protected void imgCencel_Click(object sender, ImageClickEventArgs e)
    {

    }
}
