using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Collections;

using System.Web.UI;

using System.Collections.Generic;

using Plivo;


public partial class sendsms : System.Web.UI.Page
{
    private string COName = "";
    private string strCustName = "";
    private string strEstName = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            customer cust = new customer();

            imgCencel.Attributes.Add("onClick", "CloseWindow();");

            DataClassesDataContext _db = new DataClassesDataContext();

            if (Request.QueryString.Get("custId") != null)
            {
                int ncid = Convert.ToInt32(Request.QueryString.Get("custId"));
                int neid = Convert.ToInt32(Request.QueryString.Get("eid"));
                if (ncid > 0)
                {
                    hdnCustomerId.Value = ncid.ToString();
                    hdnEstimateId.Value = neid.ToString();
                    cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                    lblCustomer.Text = cust.first_name1 + " " + cust.last_name1;

                    if (cust.phone.Length > 0)
                        lblTo.Text = cust.phone.Replace("(","").Replace(")", "");
                    else
                        lblTo.Text = cust.mobile.Replace("(", "").Replace(")", "");

                    if(lblTo.Text.Trim().Length==0)
                    {
                        imgSend.Visible = false;
                        lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Customer mobile no is empty.");

                    }
                }

            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "imgSend" });
        }
    }



    protected void imgSend_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgSend.ID, imgSend.GetType().Name, "img Send Click");
        lblMessage.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();

        customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
       if(objCust!=null)
        {
            if(objCust.IsEnableSMS==false)
            {
                lblMessage.Text = csCommonUtility.GetSystemErrorMessage("This customer opted out from the SMS service. Please Opt-in before sending any Text message. ");
                return;
            }
        }
        if (txtBody.Text.Trim().Length == 0)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Message is required");
            return;
        }

        if (txtBody.Text.Trim().Length > 500)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Write your message below:(500 Chars Max)");
            return;
        }

        string auth_id = ConfigurationManager.AppSettings["AuthId"];
        string auth_token = ConfigurationManager.AppSettings["AuthToken"];
        string from_mobile = ConfigurationManager.AppSettings["FromNumber"];
        string AppType = ConfigurationManager.AppSettings["AppType"];

        string strToMobile = lblTo.Text;

        string NoFormat = csCommonUtility.ExtractNumber(strToMobile.Trim());

        if (NoFormat.Length == 10)
        {
            strToMobile = "1" + strToMobile;
        }

        try
        {
          
            ScheduleSM sms = new ScheduleSM();
            var api = new PlivoApi(auth_id, auth_token);// Main Account


            var response = api.Message.Create(

                              src: from_mobile,
                              dst: new List<String> { strToMobile },
                              text: txtBody.Text.Trim()

                          );


            sms.title = txtBody.Text.Trim();
            sms.reference_id = Convert.ToInt32(hdnCustomerId.Value);
            sms.reference_type = 1; // Customer
            sms.estimate_id = 0;
            sms.mobile = strToMobile;
            //obj.reponse = "";
            sms.event_id = 0;
            sms.schedule_date = DateTime.Now;
            sms.create_date = DateTime.Now;
            sms.created_by = User.Identity.Name;
            sms.reponse = response.ToString();
            sms.is_success = true;
            sms.send_date = DateTime.Now;
            sms.error = "";
            sms.Uuid = response.MessageUuid[0].ToString();
            sms.Status = response.Message;
            sms.units = 0;
            sms.total_amount = 0;
            sms.sms_rate = 0;

            _db.ScheduleSMs.InsertOnSubmit(sms);

            _db.SubmitChanges();

            lblMessage.Text = csCommonUtility.GetSystemMessage("Message sent successfully.....");

            if (Request.QueryString.Get("sms") != null)  // Pending SMS send
            {

                try
                {
                    if (Session["pSection"] != null)
                    {

                        PendingPaymentHistory objPP = new PendingPaymentHistory();

                        if (_db.PendingPaymentHistories.Any(p => p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.section.ToLower().Trim() == Convert.ToString(Session["pSection"]).ToLower().Trim()))
                        {
                            objPP = _db.PendingPaymentHistories.FirstOrDefault(p => p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.section.ToLower().Trim() == Convert.ToString(Session["pSection"]).ToLower().Trim());
                            if (objPP != null)
                            {
                                objPP.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                                objPP.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                                objPP.section = Convert.ToString(Session["pSection"]);
                                objPP.createddate = DateTime.Now;
                            }

                        }
                        else
                        {
                            objPP.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            objPP.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                            objPP.section = Convert.ToString(Session["pSection"]);
                            objPP.createddate = DateTime.Now;
                            _db.PendingPaymentHistories.InsertOnSubmit(objPP);
                        }
                        _db.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Unable to send text. Please try again in 30 mins.");

                    Console.WriteLine(ex.Message.ToString());
                }




            }
         }
        catch (Exception ex)
        {
            //lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Unable to send text. Please try again in 30 mins.");
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            Console.WriteLine(ex.Message.ToString());
        }



    }


}