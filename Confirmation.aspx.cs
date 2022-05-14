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
using System.Net;
using System.IO;
using System.Drawing;

public partial class Confirmation : System.Web.UI.Page
{
    protected string ccnumber = null;
    protected string ccexp = null;
    protected string cvv = null;
    protected decimal amount = 0;
    protected string firstname = null;
    protected string lastname = null;
    protected string fullname = null;
    protected string address = null;
    protected string city = null;
    protected string strstate = null;
    protected string zip = null;
    protected string phone = null;

    protected string comname = null;
    protected string comaddress = null;
    protected string comemail = null;

    protected string paymentterms = null;
    protected Boolean is_different = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        string str = "";
        lblTrans.Text = "";

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            DataClassesDataContext _db = new DataClassesDataContext();

            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();
            }

            int nEstPayId = Convert.ToInt32(Request.QueryString.Get("epid"));
            hdnEstPayId.Value = nEstPayId.ToString();

            Hashtable ht = (Hashtable)Session["ccard"];
            int nPaymentType = Convert.ToInt32(ht["pType"]);
            amount = Convert.ToDecimal(ht["amount"]);
            firstname = ht["firstname"].ToString();
            lastname = ht["lastname"].ToString();
            fullname = ht["fullname"].ToString();
            address = ht["address"].ToString();
            city = ht["city"].ToString();
            strstate = ht["state"].ToString();
            zip = ht["zip"].ToString();
            phone = ht["phone"].ToString();

            comaddress = ht["CompanyAddress"].ToString();
            comname = ht["CompanyName"].ToString();
            comemail = ht["CompanyEmail"].ToString();

            lblFullName.Text = fullname;
            lblAddress.Text = address;
            lblAmount.Text = amount.ToString("c");

            ccnumber = ht["ccnumber"].ToString();

            paymentterms = ht["paymentterms"].ToString();
            is_different = Convert.ToBoolean(ht["is_different"]);

            if (nPaymentType >= 1 && nPaymentType <= 5)
            {
                ccexp = ht["ccexp"].ToString();
                cvv = ht["cvv"].ToString();


                string str1 = ccnumber.Substring(ccnumber.Length - 4);
                string str2 = "";
                int le = ccnumber.Substring(0, ccnumber.Length - 4).Length;
                for (int i = 0; i < le; i++)
                {
                    str2 += "*";
                }
                lblccNumber.Text = str2 + str1;
                lblcvv.Text = cvv;
                if (ccexp.Length == 3)
                {
                    lblExpMonth.Text = ccexp.Substring(0, 1);
                    lblExpYear.Text = ccexp.Substring(1);
                }
                else
                {
                    lblExpMonth.Text = ccexp.Substring(0, 2);
                    lblExpYear.Text = ccexp.Substring(2);
                }
                //str = "username=demo&password=password&ccnumber=" + ccnumber + "&ccexp=" + ccexp + "&cvv=" + cvv + "&amount=" + amount + "&firstname=" + firstname + "&address1=" + address1 + "&zip=" + zip;
                hdn.Value = str;
            }
            else if (nPaymentType == 7)//Check
            {
                lblLCCNo.Text = "Check Number:";
                lblccNumber.Text = ccnumber;
                lblLExpMonth.Text = "Identification Number:";
                lblExpMonth.Text = ht["IdentificationNo"].ToString();

                lblLExpYear.Visible = false;
                lblExpYear.Visible = false;
                lblLcvv.Visible = false;
                lblcvv.Visible = false;
            }
            else if (nPaymentType == 8)//Cash
            {
                lblLCCNo.Visible = false;
                lblccNumber.Visible = false;
                lblLExpMonth.Visible = false;
                lblExpMonth.Visible = false;
                lblLExpYear.Visible = false;
                lblExpYear.Visible = false;
                lblLcvv.Visible = false;
                lblcvv.Visible = false;
            }
        }
    }

    private bool readHtmlPage(string url, string strTransId, string strTransKey)
    {
        Hashtable ht = (Hashtable)Session["ccard"];

        String result = "";
        try
        {
            Hashtable post_values = new Hashtable();
            post_values.Add("x_login", strTransId);
            post_values.Add("x_tran_key", strTransKey);

            post_values.Add("x_delim_data", "TRUE");
            post_values.Add("x_delim_char", '|');
            post_values.Add("x_relay_response", "FALSE");

            post_values.Add("x_type", "AUTH_CAPTURE");
            post_values.Add("x_method", "CC");
            post_values.Add("x_card_num", ht["ccnumber"].ToString());
            post_values.Add("x_exp_date", lblExpMonth.Text.Trim() + lblExpYear.Text.Trim());

            post_values.Add("x_amount", lblAmount.Text.Replace("$", string.Empty));
            post_values.Add("x_description", "Sample Transaction");

            post_values.Add("x_first_name", firstname);
            post_values.Add("x_last_name", lastname);
            post_values.Add("x_address", address);
            post_values.Add("x_state", strstate);
            post_values.Add("x_zip", zip);

            String strPost = "";
            foreach (DictionaryEntry field in post_values)
            {
                strPost += field.Key + "=" + field.Value + "&";
            }
            strPost = strPost.TrimEnd('&');

            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";


            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(strPost);
            myWriter.Close();

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                sr.Close();
            }
            string[] response_array = result.Split('|');

            int nReason = Convert.ToInt32(response_array[2]);
            if (nReason != 1)
            {
                lblReason.Text = GetttingErrorReason(nReason);
                Response.Redirect("unsuccess.aspx?reason=" + lblReason.Text + "&cid=" + hdnCustomerId.Value + "&eid=" + hdnEstId.Value + "&epid=" + hdnEstPayId.Value);
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
        if (result.IndexOf("approved") != -1)
            return true;
        else
            return false;
    }

    private string GetttingErrorReason(int nReasonCode)
    {
        string strReson = null;
        if (nReasonCode == 1)
            strReson = "This transaction has been approved.";
        else if (nReasonCode == 2 || nReasonCode == 2 || nReasonCode == 3 || nReasonCode == 4 || nReasonCode == 41 ||
            nReasonCode == 44 || nReasonCode == 45 || nReasonCode == 65 || nReasonCode == 141 || nReasonCode == 145 ||
            nReasonCode == 165 || nReasonCode == 200 || nReasonCode == 201 || nReasonCode == 202 || nReasonCode == 203 ||
            nReasonCode == 204 || nReasonCode == 205 || nReasonCode == 206 || nReasonCode == 206 || nReasonCode == 207 ||
            nReasonCode == 208 || nReasonCode == 209 || nReasonCode == 210 || nReasonCode == 211 || nReasonCode == 212 ||
            nReasonCode == 213 || nReasonCode == 214 || nReasonCode == 215 || nReasonCode == 216 || nReasonCode == 217 ||
            nReasonCode == 218 || nReasonCode == 219 || nReasonCode == 220 || nReasonCode == 221 || nReasonCode == 222 ||
            nReasonCode == 223 || nReasonCode == 224 || nReasonCode == 250 || nReasonCode == 251 || nReasonCode == 254)
            strReson = "This transaction has been declined.";
        else if (nReasonCode == 5)
            strReson = "A valid amount is required.";
        else if (nReasonCode == 6)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 7)
            strReson = "The credit card expiration date is invalid.";
        else if (nReasonCode == 8)
            strReson = "The credit card has expired.";
        else if (nReasonCode == 9)
            strReson = "The ABA code is invalid.";
        else if (nReasonCode == 10)
            strReson = "The account number is invalid.";
        else if (nReasonCode == 11)
            strReson = "A duplicate transaction has been submitted.";
        else if (nReasonCode == 12)
            strReson = "An authorization code is required but not present.";
        else if (nReasonCode == 13)
            strReson = "The merchant API Login ID is invalid or the account is inactive.";
        else if (nReasonCode == 14)
            strReson = "The Referrer or Relay Response URL is invalid.";
        else if (nReasonCode == 15)
            strReson = "The transaction ID is invalid.";
        else if (nReasonCode == 16)
            strReson = "The transaction was not found.";
        else if (nReasonCode == 17)
            strReson = "The merchant does not accept this type of credit card.";
        else if (nReasonCode == 18)
            strReson = "ACH transactions are not accepted by this merchant.";
        else if (nReasonCode == 19 || nReasonCode == 20 || nReasonCode == 21 || nReasonCode == 22 || nReasonCode == 23)
            strReson = "An error occurred during processing. Please try again in 5 minutes.";
        else if (nReasonCode == 24)
            strReson = "The Nova Bank Number or Terminal ID is incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 25 || nReasonCode == 26 || nReasonCode == 57 || nReasonCode == 58 ||
            nReasonCode == 59 || nReasonCode == 60 || nReasonCode == 61 || nReasonCode == 62 || nReasonCode == 63)
            strReson = "An error occurred during processing. Please try again in 5 minutes.";
        else if (nReasonCode == 27)
            strReson = "The transaction resulted in an AVS mismatch. The address provided does not match billing address of cardholder.";
        else if (nReasonCode == 28)
            strReson = "The merchant does not accept this type of credit card.";
        else if (nReasonCode == 29)
            strReson = "The Paymentech identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 30)
            strReson = "The configuration with the processor is invalid. Call Merchant Service Provider.";
        else if (nReasonCode == 31)
            strReson = "The FDC Merchant ID or Terminal ID is incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 32)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 33)
            strReson = "FIELD cannot be left blank.";
        else if (nReasonCode == 34)
            strReson = "The VITAL identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 35)
            strReson = "An error occurred during processing. Call Merchant Service Provider.";
        else if (nReasonCode == 36)
            strReson = "The authorization was approved, but settlement failed.";
        else if (nReasonCode == 33)
            strReson = "FIELD cannot be left blank.";
        else if (nReasonCode == 34)
            strReson = "The VITAL identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 35)
            strReson = "An error occurred during processing. Call Merchant Service Provider.";
        else if (nReasonCode == 37)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 38)
            strReson = "The Global Payment System identification numbers are incorrect. Call Merchant Service Provider.";
        else if (nReasonCode == 40)
            strReson = "This transaction must be encrypted.";
        else if (nReasonCode == 43)
            strReson = "The merchant was incorrectly set up at the processor. Call your Merchant Service Provider.";
        else if (nReasonCode == 46)
            strReson = "Your session has expired or does not exist. You must log in to continue working.";
        else if (nReasonCode == 47)
            strReson = "The amount requested for settlement may not be greater than the original amount authorized.";
        else if (nReasonCode == 48)
            strReson = "This processor does not accept partial reversals.";
        else if (nReasonCode == 49)
            strReson = "A transaction amount greater than $[amount] will not be accepted.";
        else if (nReasonCode == 50)
            strReson = "This transaction is awaiting settlement and cannot be refunded.";
        else if (nReasonCode == 51)
            strReson = "The sum of all credits against this transaction is greater than the original transaction amount.";
        else if (nReasonCode == 52)
            strReson = "The transaction was authorized, but the client could not be notified; the transaction will not be settled.";
        else if (nReasonCode == 53)
            strReson = "The transaction type was invalid for ACH transactions.";
        else if (nReasonCode == 54)
            strReson = "The referenced transaction does not meet the criteria for issuing a credit.";
        else if (nReasonCode == 55)
            strReson = "The sum of credits against the referenced transaction would exceed the original debit amount.";
        else if (nReasonCode == 56)
            strReson = "This merchant accepts ACH transactions only; no credit card transactions are accepted.";
        else if (nReasonCode == 66)
            strReson = "This transaction cannot be accepted for processing.";
        else if (nReasonCode == 68)
            strReson = "The version parameter is invalid.";
        else if (nReasonCode == 69)
            strReson = "The transaction type is invalid.";
        else if (nReasonCode == 70)
            strReson = "The transaction method is invalid.";
        else if (nReasonCode == 71)
            strReson = "The bank account type is invalid.";
        else if (nReasonCode == 72)
            strReson = "The authorization code is invalid.";
        else if (nReasonCode == 73)
            strReson = "The drivers license date of birth is invalid.";
        else if (nReasonCode == 74)
            strReson = "The duty amount is invalid.";
        else if (nReasonCode == 75)
            strReson = "The freight amount is invalid.";
        else if (nReasonCode == 76)
            strReson = "The tax amount is invalid.";
        else if (nReasonCode == 77)
            strReson = "The SSN or tax ID is invalid.";
        else if (nReasonCode == 78)
            strReson = "The Card Code (CVV2/CVC2/CID) is invalid.";
        else if (nReasonCode == 79)
            strReson = "The drivers license number is invalid.";
        else if (nReasonCode == 80)
            strReson = "The drivers license state is invalid.";
        else if (nReasonCode == 81)
            strReson = "The requested form type is invalid.";
        else if (nReasonCode == 82)
            strReson = "Scripts are only supported in version 2.5.";
        else if (nReasonCode == 83)
            strReson = "The requested script is either invalid or no longer supported.";
        else if (nReasonCode == 84 || nReasonCode == 85 || nReasonCode == 86 || nReasonCode == 87 || nReasonCode == 88 || nReasonCode == 89 || nReasonCode == 90)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 91)
            strReson = "Version 2.5 is no longer supported.";
        else if (nReasonCode == 92)
            strReson = "The gateway no longer supports the requested method of integration.";
        else if (nReasonCode == 97 || nReasonCode == 98 || nReasonCode == 99)
            strReson = "This transaction cannot be accepted.";
        else if (nReasonCode == 100)
            strReson = "The eCheck.Net type is invalid.";
        else if (nReasonCode == 101)
            strReson = "The given name on the account and/or the account type does not match the actual account.";
        else if (nReasonCode == 102)
            strReson = "This request cannot be accepted.";
        else if (nReasonCode == 103)
            strReson = "This transaction cannot be accepted.";
        else if (nReasonCode == 104 || nReasonCode == 105 || nReasonCode == 106 || nReasonCode == 107 || nReasonCode == 108 || nReasonCode == 109 || nReasonCode == 110)
            strReson = "This transaction is currently under review.";
        else if (nReasonCode == 116)
            strReson = "The authentication indicator is invalid.";
        else if (nReasonCode == 117)
            strReson = "The cardholder authentication value is invalid.";
        else if (nReasonCode == 118)
            strReson = "The combination of authentication indicator and cardholder authentication value is invalid.";
        else if (nReasonCode == 119)
            strReson = "Transactions having cardholder authentication values cannot be marked as recurring.";
        else if (nReasonCode == 120 || nReasonCode == 121 || nReasonCode == 122)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 123)
            strReson = "This account has not been given the permission(s) required for this request.";
        else if (nReasonCode == 127)
            strReson = "The transaction resulted in an AVS mismatch. The address provided does not match billing address of cardholder.";
        else if (nReasonCode == 128)
            strReson = "This transaction cannot be processed.";
        else if (nReasonCode == 130)
            strReson = "This payment gateway account has been closed.";
        else if (nReasonCode == 131 || nReasonCode == 132)
            strReson = "This transaction cannot be accepted at this time.";
        else if (nReasonCode == 152)
            strReson = "The transaction was authorized, but the client could not be notified; the transaction will not be settled.";
        else if (nReasonCode == 170 || nReasonCode == 171 || nReasonCode == 172 || nReasonCode == 173)
            strReson = "An error occurred during processing. Please contact the merchant.";
        else if (nReasonCode == 174)
            strReson = "The transaction type is invalid. Please contact the merchant.";
        else if (nReasonCode == 175)
            strReson = "The processor does not allow voiding of credits.";
        else if (nReasonCode == 180 || nReasonCode == 181)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 185)
            strReson = "This reason code is reserved or not applicable to this API.";
        else if (nReasonCode == 193)
            strReson = "The transaction is currently under review.";
        else if (nReasonCode == 243)
            strReson = "Recurring billing is not allowed for this eCheck.Net type.";
        else if (nReasonCode == 244)
            strReson = "This eCheck.Net type is not allowed for this Bank Account Type.";
        else if (nReasonCode == 245)
            strReson = "This eCheck.Net type is not allowed when using the payment gateway hosted payment form.";
        else if (nReasonCode == 247 || nReasonCode == 246)
            strReson = "This eCheck.Net type is not allowed.";
        else if (nReasonCode == 252 || nReasonCode == 253)
            strReson = "Your order has been received. Thank you for your business!";
        else if (nReasonCode == 261)
            strReson = "An error occurred during processing. Please try again.";
        else if (nReasonCode == 270)
            strReson = "The line item [item number] is invalid.";
        else if (nReasonCode == 271)
            strReson = "The number of line items submitted is not allowed. A maximum of 30 line items can be submitted.";
        else if (nReasonCode == 315)
            strReson = "The credit card number is invalid.";
        else if (nReasonCode == 316)
            strReson = "The credit card expiration date is invalid.";
        else if (nReasonCode == 317)
            strReson = "The credit card has expired.";
        else if (nReasonCode == 318)
            strReson = "A duplicate transaction has been submitted.";
        else if (nReasonCode == 318)
            strReson = "The transaction cannot be found.";
        return strReson;

    }
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnConfirm.ID, btnConfirm.GetType().Name, "Click"); 
        client_info oCompanyProfile = (client_info)Session["MyCompany"];

        string strTransId = oCompanyProfile.transaction_id;
        string strTransKey = oCompanyProfile.transaction_key;

        Hashtable ht = (Hashtable)Session["ccard"];
        int nPaymentType = Convert.ToInt32(ht["pType"]);

        amount = Convert.ToDecimal(ht["amount"]);
        firstname = ht["firstname"].ToString();
        lastname = ht["lastname"].ToString();
        fullname = ht["fullname"].ToString();
        address = ht["address"].ToString();
        city = ht["city"].ToString();
        strstate = ht["state"].ToString();
        zip = ht["zip"].ToString();
        phone = ht["phone"].ToString();
        comaddress = ht["CompanyAddress"].ToString();
        comname = ht["CompanyName"].ToString();
        comemail = ht["CompanyEmail"].ToString();
        ccnumber = ht["ccnumber"].ToString();
        paymentterms = ht["paymentterms"].ToString();
        is_different = Convert.ToBoolean(ht["is_different"]);

        DateTime dtDate;
        string strCC = ccnumber;
        if (nPaymentType >= 1 && nPaymentType <= 5)
        {
            dtDate = Convert.ToDateTime(lblExpMonth.Text + "-1-" + lblExpYear.Text);
        }
        else
            dtDate = Convert.ToDateTime("2010-01-01");


        bool cCheck = false;
        if (nPaymentType >= 1 && nPaymentType <= 5)
        {
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseCreditCard"]))
            {
                if (oCompanyProfile.transactiontype == "authorize.net")
                    cCheck = readHtmlPage("https://secure.authorize.net/gateway/transact.dll", strTransId, strTransKey);
            }
            else
                cCheck = true;
        }
        else
            cCheck = true;

        if (cCheck)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            

            accept_payment obj = new accept_payment();
            obj.est_payment_id = Convert.ToInt32(hdnEstPayId.Value);
            obj.payment_method_id = Convert.ToInt32(ht["pType"]);
            obj.status_id = 1;
            obj.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            obj.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            obj.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            obj.date = DateTime.Now;
            obj.credit_card_number = strCC;
            obj.card_holder_name = fullname;
            obj.credit_card_exp_date = dtDate;
            obj.credit_card_auth_num = fullname;
            obj.amount = amount;
            obj.notes = ht["Notes"].ToString();

            obj.is_different = is_different;
            obj.payment_term = paymentterms;
            obj.card_holder_name = fullname;
            obj.card_holder_address = address;
            obj.card_holder_city = city;
            obj.card_holder_state = strstate;
            obj.card_holder_zip = zip;
            obj.card_holder_phone = phone;

            _db.accept_payments.InsertOnSubmit(obj);
            _db.SubmitChanges();

            //lblTrans.Text = "Payment processed successfully.";
            //lblTrans.ForeColor 
            //btnConfirm.Enabled = false;
            Response.Redirect("success.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstId.Value + "&epid=" + hdnEstPayId.Value);
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPayId.Value + "&eid=" + hdnEstId.Value);
    }
}
