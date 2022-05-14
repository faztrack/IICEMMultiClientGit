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
public partial class messagedetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            //if (Session["oUser"] == null)
            //{
            //    Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            //}
            int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
            hdnCustomerId.Value = ncid.ToString();
            int nMessId = Convert.ToInt32(Request.QueryString.Get("MessId"));
            hdnMessageId.Value = nMessId.ToString();
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnMessageId.Value) > 0)
            {
                customer_message cust_mess = new customer_message();
                cust_mess = _db.customer_messages.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.message_id == Convert.ToInt32(hdnMessageId.Value));
                lblTo.Text = cust_mess.mess_to;
                lblFrom.Text = cust_mess.mess_from;
                lblSubject.Text = cust_mess.mess_subject;
                lblCc.Text = cust_mess.mess_cc;
                lblBcc.Text = cust_mess.mess_bcc;

                string str = String.Format("{0:f}", Convert.ToDateTime(cust_mess.create_date));
                lblDate.Text = str;
                string strBody = cust_mess.mess_description;

                Regex matchNewLine = new Regex("\r\n|\r|\n", RegexOptions.Compiled | RegexOptions.Singleline);
                string MsgBody = matchNewLine.Replace(strBody, "<br />");

                Editor1.Content = MsgBody;



                string strQ = "select * from message_upolad_info where customer_id=" + ncid + " and message_id=" + nMessId + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                foreach (message_upolad_info message_upolad in list)
                {
                    string mess_file = message_upolad.mess_file_name.Replace("amp;", "").Trim();
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    HyperLink hyp = new HyperLink();

                    cell.BorderWidth = 0;
                    try
                    {
                        hyp.Text = mess_file.Substring(0, mess_file.IndexOf('_')).Trim() + mess_file.Substring(mess_file.IndexOf('.'));

                    }
                    catch 
                    {
                        hyp.Text = mess_file;
                    }
                    hyp.NavigateUrl = "Uploads/" + ncid.ToString() + "/" + nMessId.ToString() + "/" + mess_file;
                    hyp.Target = "_blank";
                    cell.Controls.Add(hyp);
                    cell.HorizontalAlign = HorizontalAlign.Left;
                    row.Cells.Add(cell);
                    tdLink.Rows.Add(row);
                }
            }
        }

    }
}
