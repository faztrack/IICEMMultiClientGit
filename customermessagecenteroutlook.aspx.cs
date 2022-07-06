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
using System.Net.Mail;
using System.Drawing;
using Microsoft.Exchange.WebServices.Data;

public partial class customermessagecenteroutlook : System.Web.UI.Page
{
    protected int sCustId = 0;
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


                //userinfo objUser = (userinfo)HttpContext.Current.Session["oUser"];

                //if (objUser.role_id == 1 || objUser.role_id == 2)
                //{

                //    var user = (from u in _db.user_infos
                //                join s in _db.sales_persons on u.sales_person_id equals s.sales_person_id
                //                join c in _db.customers on s.sales_person_id equals c.sales_person_id
                //                where c.customer_id == Convert.ToInt32(hdnCustomerId.Value) && c.sales_person_id > 0
                //                select u).SingleOrDefault();

                //    if (user != null)
                //    {

                //        Session["EWS"] = EWSAPI.GetEWSService(user.company_email, user.email_password);
                //    }

                //}

                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if(cust != null)
                {
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;

                    string strAddress = "";
                    strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                    lblAddress.Text = strAddress;
                    lblEmail.Text = cust.email;
                    lblPhone.Text = cust.phone;

                    hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
                    hdnClientId.Value = cust.client_id.ToString();
                }



                GetCustomerMessageInfo(Convert.ToInt32(hdnCustomerId.Value));

            }

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


    private void GetCustomerMessageInfo(int nCustId)
    {
        if (nCustId > 0)
        {

            try
            {
                //  ExchangeService service = EWSAPI.GetEWSService("alyons@interiorinnovations.biz", "");

                // ExchangeService service = EWSAPI.GetEWSServiceByCustomer(Convert.ToInt32(hdnCustomerId.Value));
                //  DSMessage dsMessageSent = EWSAPI.GetEmailList(lblEmail.Text, nCustId, service);

                DSMessage dsMessageSent = new DSMessage();

                DataClassesDataContext _db = new DataClassesDataContext();
                var messList = (from mess_info in _db.customer_messages
                                where mess_info.customer_id == nCustId && mess_info.client_id == Convert.ToInt32(hdnClientId.Value)
                                orderby mess_info.cust_message_id descending
                                select mess_info).ToList();

                foreach (customer_message msg in messList)
                {
                    DSMessage.MessageRow mes = dsMessageSent.Message.NewMessageRow();

                   

                    if (msg.HasAttachments == null)
                    {
                        string strQ = "select * from message_upolad_info where customer_id=" + nCustId + " and message_id=" + msg.message_id + " and client_id=" + Convert.ToInt32(hdnClientId.Value);
                        IEnumerable<message_upolad_info> list = _db.ExecuteQuery<message_upolad_info>(strQ, string.Empty);

                        string mess_file = "";
                        foreach (message_upolad_info message_upolad in list)
                        {
                            mess_file += message_upolad.mess_file_name.Replace("amp;", "").Trim() + ", "; ;
                        }
                        mess_file = mess_file.Trim().TrimEnd(',');

                        if (mess_file.Length > 0)
                        {
                            mes.HasAttachments = true;
                            mes.AttachmentList = mess_file.Trim().TrimEnd(',');


                        }
                        else
                        {
                            mes.AttachmentList = "";
                            mes.HasAttachments = false;// msg.HasAttachments;
                        }

                        msg.HasAttachments = mes.HasAttachments;
                        msg.AttachmentList = mes.AttachmentList;

                    }
                    else if (Convert.ToBoolean(msg.HasAttachments))
                    {

                        mes.HasAttachments = true;
                        mes.AttachmentList = msg.AttachmentList;


                    }
                    else
                    {
                        mes.HasAttachments = false;
                        mes.AttachmentList = "";
                    }

                    mes.From = msg.mess_from;
                    mes.To = msg.mess_to;
                    mes.IsRead = (bool)(msg.IsView ?? false);
                    mes.customer_id = nCustId.ToString();
                    mes.message_id = msg.message_id.ToString();
                    mes.create_date = (DateTime)msg.create_date;
                    if (msg.mess_subject != null)
                        mes.mess_subject = msg.mess_subject.ToString();
                    else
                        mes.mess_subject = "";
                    mes.last_view = (DateTime)msg.last_view;
                    mes.Protocol = msg.Protocol;
                    mes.Type = msg.Type;
                    mes.sent_by = msg.sent_by;
                    dsMessageSent.Message.AddMessageRow(mes);

                }

                _db.SubmitChanges();

                dsMessageSent.AcceptChanges();

                DataView dv = dsMessageSent.Tables[0].DefaultView;
                dv.Sort = "create_date DESC";
                grdCustomersMessage.DataSource = dv;
                grdCustomersMessage.DataKeyNames = new string[] { "customer_id", "message_id", "AttachmentList", "From", "To", "Type" };
                grdCustomersMessage.DataBind();




            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

            }

        }

    }


    protected void grdCustomersMessage_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                string script = "";

                string Attacheent = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[2].ToString();
                string MessId = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[1].ToString();
                int CustId = Convert.ToInt32(grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[0].ToString());

                string From = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[3].ToString();
                string To = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[4].ToString();
                string Type = grdCustomersMessage.DataKeys[e.Row.RowIndex].Values[5].ToString();




                if (e.Row.Cells[7].Text.Equals("True"))
                    e.Row.Cells[7].Text = "Yes";
                else
                    e.Row.Cells[7].Text = "";


                HyperLink hypMessageDetails = (HyperLink)e.Row.FindControl("hypMessageDetails");
                hypMessageDetails.ToolTip = "Click on Message Details to view specific Message Details .";
                hypMessageDetails.Target = "MyWindow";

                //if (e.Row.Cells[8].Text.Equals("Outlook"))
                //{
                //    script = String.Format("GetdatakeyValue1('{0}','{1}','{2}','{3}')", MessId.ToString(), From, To, Type);
                //}
                //else
                //{
                //    script = String.Format("GetdatakeyValue1Old('{0}')", MessId.ToString());
                //}

                script = String.Format("GetdatakeyValue1Old('{0}')", MessId.ToString());

                hypMessageDetails.Attributes.Add("onclick", script);
                if (Attacheent.Length > 0)
                {
                    HyperLink hypAttachment = (HyperLink)e.Row.FindControl("hypAttachment");
                    hypAttachment.Text = Attacheent;
                    hypAttachment.ToolTip = "Click on Message Details to view specific Message Details .";
                    hypAttachment.Target = "MyWindow";

                    hypAttachment.Attributes.Add("onclick", script);
                }





            }
            catch { }

        }
    }

}
