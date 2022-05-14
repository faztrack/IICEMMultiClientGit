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
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;
public partial class messagedetailsoutlook : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["EWS_Mess"] != null)
                Session.Remove("EWS_Mess");
            string sMessId = Request.QueryString.Get("MessId");
            string sFrom = Request.QueryString.Get("From");
            string sTo = Request.QueryString.Get("To");
            string sType = Request.QueryString.Get("Type");

            sMessId = sMessId.Replace(" ", "+");

            ExchangeService service = EWSAPI.GetEWSService(sFrom);

            EmailMessage message = EWSAPI.GetEmailDetails(sMessId, service);
            Session.Add("EWS_Mess", message);

            ShowMessageDetails(message);



        }
        LoadAttachment();
    }


    private void ShowMessageDetails(EmailMessage message)
    {
        try
        {


            foreach (EmailAddress add in message.ToRecipients)
            {
                lblTo.Text += add.Address.ToString() + ", ";
            }
            lblTo.Text = lblTo.Text.Trim().TrimEnd(',');

            lblFrom.Text = message.From.Address;
            lblSubject.Text = message.Subject;
            foreach (EmailAddress add in message.CcRecipients)
            {
                lblCc.Text += add.Address.ToString() + ", ";
            }
            lblCc.Text = lblCc.Text.Trim().TrimEnd(',');
            foreach (EmailAddress add in message.BccRecipients)
            {
                lblBcc.Text += add.Address.ToString() + ", ";
            }

            lblBcc.Text = lblBcc.Text.Trim().TrimEnd(',');
            string strBody = message.Body;


            txtBody.Content = strBody;





        }
        catch (Exception ex)
        {
            string ss = ex.Message;

        }


    }
    private void LoadAttachment()
    {
        if (Session["EWS_Mess"] != null)
        {
            EmailMessage message = (EmailMessage)Session["EWS_Mess"];

            AttachmentCollection attCollection = message.Attachments;
            int i = 0;
            foreach (Attachment attachment in attCollection)
            {
                if (attachment is FileAttachment)
                {

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    LinkButton hyp = new LinkButton();

                    cell.BorderWidth = 0;
                    hyp.Text = attachment.Name;
                    hyp.ID = "btnAttachment" + i;
                    hyp.Click += new EventHandler(Attachment_Click);
                    hyp.ToolTip = attachment.Name;
                    cell.Controls.Add(hyp);
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    row.Cells.Add(cell);
                    tdLink.Rows.Add(row);

                    i++;

                }
            }
        }



    }

    protected void Attachment_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;



        try
        {
            EmailMessage message = (EmailMessage)Session["EWS_Mess"];
            string DestinationPath = Server.MapPath("~/Downloads//");
            string sFileName = "";
            foreach (Attachment attachment in message.Attachments)
            {
                if (attachment is FileAttachment)
                {
                    if (attachment.Name.Equals(btn.Text))
                    {
                        FileAttachment fileAttachment = attachment as FileAttachment;
                        sFileName = DateTime.Now.Ticks + attachment.Name;
                        fileAttachment.Load(DestinationPath + sFileName);
                        string url = "Downloads/" + sFileName;
                        string Script = @"<script language=JavaScript>window.open('" + url + "', '_blank'); opener.document.forms[0].submit(); </script>";
                        if (!IsClientScriptBlockRegistered("OpenAttachment"))
                            this.RegisterClientScriptBlock("OpenAttachment", Script);


                    }


                }
            }
        }
        catch (Exception ex)
        {
            string ss = ex.Message;
        }


    }
    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {

    }
}
