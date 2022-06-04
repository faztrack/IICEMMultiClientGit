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
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;

public partial class payment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin031") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();
            decimal nDeposit = Convert.ToDecimal(Request.QueryString.Get("nDeposit"));
            hdnDepositTotal.Value = nDeposit.ToString();
            int nEstPayId = Convert.ToInt32(Request.QueryString.Get("epid"));
            hdnEstPaymentId.Value = nEstPayId.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            BindStates();

            DataClassesDataContext _db = new DataClassesDataContext();

            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                string strSecondName = cust.first_name2 + " " + cust.last_name2;
                if (strSecondName.Trim() == "")
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                else
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1 + " & " + strSecondName;

                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblPhone.Text = cust.phone;
                lblEmail.Text = cust.email;

                hdnClientId.Value = cust.client_id.ToString();

                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                //string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                //hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                    lblEstimateName.Text = cus_est.estimate_name;
                }
                if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
                {
                    GetPaymentInfo(Convert.ToInt32(hdnEstPaymentId.Value), Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnSalesPersonId.Value));
                }

                rdlPaymentType.SelectedValue = "0";
                tabCard.Visible = true;
                tabCash.Visible = false;
                tabCheck.Visible = false;
            }
            accept_payment obj = new accept_payment();
            if (_db.accept_payments.Where(pay => pay.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value)).ToList().Count > 0)
            {
                pnlPaymentHistory.Visible = true;
                GetPaymentData();
            }
            if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
            {
                chkFinancedProjects.Checked = false;
                pnlFinanceProjects.Visible = false;
                hdnfpId.Value = "0";
            }
            else
            {
                chkFinancedProjects.Checked = true;
                pnlFinanceProjects.Visible = true;

                finance_project objfp = new finance_project();
                objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));
                txtLendingInst.Text = objfp.lending_inst;
                txtApprovalCode.Text = objfp.approval_code;
                txtAmountApproved.Text = Convert.ToDecimal(objfp.amount_approved).ToString();
                hdnfpId.Value = objfp.finance_id.ToString();
            }
        }
    }

    private void GetPaymentData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from ap in _db.accept_payments
                   where ap.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value)
                   select ap;
        grdPaymentHistory.DataSource = item;
        grdPaymentHistory.DataBind();

        if (grdPaymentHistory.Rows.Count > 0)
            pnlPaymentHistory.Visible = true;
        else
            pnlPaymentHistory.Visible = false;

    }
    protected void grdPaymentHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nPaymentId = Convert.ToInt32(grdPaymentHistory.DataKeys[e.Row.RowIndex].Value.ToString());

            LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
            lnkDelete.CommandArgument = nPaymentId.ToString();

            if (e.Row.Cells[1].Text == "1")
            {
                string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
                if (str != "" && str.Length > 12)
                {
                    e.Row.Cells[4].Text = str.Remove(0, 12);

                }

                e.Row.Cells[1].Text = "Master Card";

            }
            else if (e.Row.Cells[1].Text == "2")
            {
                string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
                if (str != "" && str.Length > 12)
                {
                    e.Row.Cells[4].Text = str.Remove(0, 12);
                }

                e.Row.Cells[1].Text = "Visa";
            }
            else if (e.Row.Cells[1].Text == "3")
            {
                string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
                if (str != "" && str.Length > 12)
                {
                    e.Row.Cells[4].Text = str.Remove(0, 12);
                }
                e.Row.Cells[1].Text = "Discover";
            }
            else if (e.Row.Cells[1].Text == "4")
            {
                string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
                if (str != "" && str.Length > 12)
                {
                    e.Row.Cells[4].Text = str.Remove(0, 12);
                }
                e.Row.Cells[1].Text = "American Express";
            }
            else if (e.Row.Cells[1].Text == "7")
                e.Row.Cells[1].Text = "Check";
            else if (e.Row.Cells[1].Text == "8")
                e.Row.Cells[1].Text = "Cash";

        }
    }
    private void BindStates()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var states = from st in _db.states
                     orderby st.abbreviation
                     select st;
        ddlState.DataSource = states;
        ddlState.DataTextField = "abbreviation";
        ddlState.DataValueField = "abbreviation";
        ddlState.DataBind();
    }
    private void GetPaymentInfo(int nPaymentId, int nEstimateId, int nCustomerId, int nSalesPersonId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();


        objEstPay = _db.estimate_payments.Single(pay => pay.est_payment_id == nPaymentId && pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId && pay.sales_person_id == nSalesPersonId && pay.client_id == Convert.ToInt32(hdnClientId.Value));


        txtnDeposit.Text = Convert.ToDecimal(objEstPay.deposit_amount).ToString("c");
        txtnCountertop.Text = Convert.ToDecimal(objEstPay.countertop_amount).ToString("c");
        txtnJob.Text = Convert.ToDecimal(objEstPay.start_job_amount).ToString("c");
        txtnBalance.Text = Convert.ToDecimal(objEstPay.due_completion_amount).ToString("c");
        txtnMeasure.Text = Convert.ToDecimal(objEstPay.final_measure_amount).ToString("c");
        txtnDelivery.Text = Convert.ToDecimal(objEstPay.deliver_cabinet_amount).ToString("c");
        txtnSubstantial.Text = Convert.ToDecimal(objEstPay.substantial_amount).ToString("c");
        txtOthers.Text = objEstPay.other_value.ToString();
        txtnOthers.Text = Convert.ToDecimal(objEstPay.other_amount).ToString("c");
        if (objEstPay.deposit_date != null)
            lblDepositDate.Text = objEstPay.deposit_date.ToString();
        if (objEstPay.countertop_date != null)
            lblCountertopDate.Text = objEstPay.countertop_date.ToString();
        if (objEstPay.startof_job_date != null)
            lblStartJobDate.Text = objEstPay.startof_job_date.ToString();
        if (objEstPay.due_completion_date != null)
            lblBalanceDueDate.Text = objEstPay.due_completion_date.ToString();
        if (objEstPay.measure_date != null)
            lblMeasureDate.Text = objEstPay.measure_date.ToString();
        if (objEstPay.delivery_date != null)
            lblDeliveryDate.Text = objEstPay.delivery_date.ToString();
        if (objEstPay.substantial_date != null)
            lblSubstantialDate.Text = objEstPay.substantial_date.ToString();
        if (objEstPay.other_date != null)
            lblOtherDate.Text = objEstPay.other_date.ToString();

        if (objEstPay.deposit_value != null)
        {
            lblDepositValue.ToolTip = objEstPay.deposit_value.Replace("^", "'").ToString();
            lblDepositValue.Text = objEstPay.deposit_value.Replace("^", "'").ToString();
        }
        if (objEstPay.countertop_value != null)
        {
            lblCountertopValue.ToolTip = objEstPay.countertop_value.Replace("^", "'").ToString();
            lblCountertopValue.Text = objEstPay.countertop_value.Replace("^", "'").ToString();
        }
        if (objEstPay.start_job_value != null)
        {
            lblStartJobValue.ToolTip = objEstPay.start_job_value.Replace("^", "'").ToString();
            lblStartJobValue.Text = objEstPay.start_job_value.Replace("^", "'").ToString();
        }
        if (objEstPay.due_completion_value != null)
        {
            lblBalanceDueValue.ToolTip = objEstPay.due_completion_value.Replace("^", "'").ToString();
            lblBalanceDueValue.Text = objEstPay.due_completion_value.Replace("^", "'").ToString();
        }
        if (objEstPay.final_measure_value != null)
        {
            lblMeasureValue.ToolTip = objEstPay.final_measure_value.Replace("^", "'").ToString();
            lblMeasureValue.Text = objEstPay.final_measure_value.Replace("^", "'").ToString();
        }
        if (objEstPay.deliver_caninet_value != null)
        {
            lblDeliveryValue.ToolTip = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
            lblDeliveryValue.Text = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
        }
        if (objEstPay.substantial_value != null)
        {
            lblSubstantialValue.ToolTip = objEstPay.substantial_value.Replace("^", "'").ToString();
            lblSubstantialValue.Text = objEstPay.substantial_value.Replace("^", "'").ToString();
        }

    }
    protected void rdlPaymentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdlPaymentType.ID, rdlPaymentType.GetType().Name, "SelectedIndexChanged"); 
        lblResult.Text = "";
        lblMessage.Text = "";

        if (rdlPaymentType.SelectedValue == "0")// Card
        {
            tabCard.Visible = true;
            tabCash.Visible = false;
            tabCheck.Visible = false;

        }
        else if (rdlPaymentType.SelectedValue == "7")//Check
        {
            tabCard.Visible = false;
            tabCash.Visible = false;
            tabCheck.Visible = true;
            //txtCheckAmount.Text = Convert.ToDecimal(hdnDepositTotal.Value).ToString("c"); // lblGrandTotal.Text.Substring(1);

        }
        else if (rdlPaymentType.SelectedValue == "8")//cash
        {
            tabCard.Visible = false;
            tabCash.Visible = true;
            tabCheck.Visible = false;
            //txtCashAmount.Text = Convert.ToDecimal(hdnDepositTotal.Value).ToString("c"); //lblGrandTotal.Text.Substring(1);
        }
    }
    private bool _checkCreditCardInformation()
    {
        lblResult.Text = "";
        
        if (rdlPaymentType.SelectedValue == "0")
        {
            if (txtCreditCardNumber.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Credit Card Number is epmty.");
                return false;

            }
            if (ddlCardType.SelectedIndex < 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Select Credit Card type.");
                return false;
            }
            if (ddlMonth.SelectedIndex == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry month.");
                return false;
            }

            if (ddlYear.SelectedIndex == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry year.");
                return false;
            }

            if (txtCVV.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("CVV is epmty.");
                return false;
            }
            if (txtCardAmount.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount is missing.");
                return false;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtCardAmount.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid amount.");
                    return false;
                }
            }
        }
        else if (rdlPaymentType.SelectedValue == "7")// Check
        {
            if (txtCheckNo.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Check Number is epmty.");
                return false;

            }
            if (txtIdentificationNo.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Identification Number is epmty.");
                return false;
            }
            if (txtCheckAmount.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount is epmty.");
                return false;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtCheckAmount.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid amount.");
                    return false;
                }
            }

        }
        else if (rdlPaymentType.SelectedValue == "8")// Cash
        {
            if (txtCashAmount.Text.Trim().Length == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Amount is epmty.");
                return false;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtCashAmount.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid amount.");
                    return false;
                }
            }

        }
        return true;
    }
    private void CreditCardInformation()
    {
        Hashtable ht = new Hashtable();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));

        string strCity = oCom.city;
        if (strCity.Trim().Length > 0)
            strCity += ", ";
        string strState = oCom.state;
        if (strState.Trim().Length > 0)
            strState += ", ";

        string strAddress = oCom.address + Environment.NewLine + strCity + " " + strState + " " + oCom.zip_code;
        //string strCityState = dr["City"].ToString() + " " + dr["State"].ToString() + " " + dr["Zip"].ToString();
        string strPhone = oCom.phone;
        string strEmail = oCom.email;

        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

        //estimate_payment objEstPay = new estimate_payment();
        //objEstPay = _db.estimate_payments.Single(est_pay => est_pay.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value));
        if (chkDifferentName.Checked)
        {
            string strFullName = txtFirstName.Text + " " + txtLastName.Text;
            ht.Add("firstname", txtFirstName.Text);
            ht.Add("lastname", txtLastName.Text);
            ht.Add("fullname", strFullName);
            ht.Add("address", txtAddress.Text);
            ht.Add("city", txtCity.Text);
            ht.Add("state", ddlState.SelectedItem.Text);
            ht.Add("zip", txtZip.Text);
            ht.Add("phone", txtPhone.Text);
            ht.Add("is_different", true);
        }
        else
        {
            string strName = cust.first_name1 + " " + cust.last_name1;
            ht.Add("firstname", cust.first_name1);
            ht.Add("lastname", cust.last_name1);
            ht.Add("fullname", strName);
            ht.Add("address", cust.address);
            ht.Add("city", cust.city);
            ht.Add("state", cust.state);
            ht.Add("zip", cust.zip_code);
            ht.Add("phone", cust.phone);
            ht.Add("is_different", false);
        }

        ht.Add("email", cust.email);
        ht.Add("estpayid", Convert.ToInt32(hdnEstPaymentId.Value));
        ht.Add("CompanyName", oCom.company_name);
        ht.Add("CompanyAddress", strAddress);
        ht.Add("CompanyEmail", oCom.email);


        if (rdlPaymentType.SelectedValue == "0")// Card 0
        {
            ht.Add("pMode", rdlPaymentType.SelectedItem.Text);
            ht.Add("ccnumber", txtCreditCardNumber.Text.Trim());
            ht.Add("ccexp", ddlMonth.SelectedItem.Text.Trim() + ddlYear.SelectedValue.Trim());
            ht.Add("pType", Convert.ToInt32(ddlCardType.SelectedValue));
            ht.Add("Notes", txtCardNotes.Text);
            ht.Add("amount", txtCardAmount.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            ht.Add("paymentterms", ddlCardPaymentTerms.SelectedItem.Text);
            ht.Add("cvv", txtCVV.Text.Trim());
        }
        else
        {
            ht.Add("pMode", rdlPaymentType.SelectedItem.Text);
            ht.Add("pType", Convert.ToInt32(rdlPaymentType.SelectedValue));
            ht.Add("ccexp", "012000");

            if (rdlPaymentType.SelectedValue == "7")// Check 7
            {
                ht.Add("ccnumber", txtCheckNo.Text.Trim());
                ht.Add("IdentificationNo", txtIdentificationNo.Text.Trim());
                ht.Add("Notes", txtCheckNotes.Text);
                ht.Add("amount", txtCheckAmount.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
                ht.Add("paymentterms", ddlCheckPaymentTerms.SelectedItem.Text);
            }
            else // Cash 8
            {
                ht.Add("ccnumber", "");
                ht.Add("Notes", txtCashNotes.Text);
                ht.Add("amount", txtCashAmount.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
                ht.Add("paymentterms", ddlCashPaymentTerms.SelectedItem.Text);
            }
        }
        Session["ccard"] = ht;
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblMessage.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();

        if (chkDifferentName.Checked)
        {
            if (txtFirstName.Text.Trim() == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("First name is missing.");
                 
                txtFirstName.Focus();
                return;
            }
            if (txtLastName.Text.Trim() == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Last name is missing.");
                 
                txtLastName.Focus();
                return;
            }
            if (txtAddress.Text.Trim() == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Address is missing.");
                 
                txtAddress.Focus();
                return;
            }
            if (txtCity.Text.Trim() == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("City is missing.");
                 
                txtCity.Focus();
                return;
            }
            if (txtPhone.Text.Trim() == "")
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Phone is missing.");
                 
                txtPhone.Focus();
                return;
            }
        }

        if (chkFinancedProjects.Checked)
        {
            if (txtLendingInst.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Lending Institution.");
                
                txtLendingInst.Focus();
                return;
            }
            if (txtApprovalCode.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing Approval Code.");
                
                txtApprovalCode.Focus();
                return;
            }
            try
            {
                Convert.ToDecimal(txtAmountApproved.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid amount approved.");
                
                txtAmountApproved.Focus();
                return;
            }
        }


        if (!_checkCreditCardInformation())
            return;
        string strExp = ddlMonth.SelectedItem.Text.Trim() + ddlYear.SelectedValue.Trim();

        CreditCardInformation();

        //accept_payment obj = new accept_payment();
        //if (_db.accept_payments.Where(pay => pay.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value)).ToList().Count>0)
        //{
        //    lblResult.Text = "Transaction already submitted";
        //    
        //    return;
        //}

        finance_project objfp = new finance_project();
        if (chkFinancedProjects.Checked)
        {
            if (Convert.ToInt32(hdnfpId.Value) > 0)
                objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            objfp.lending_inst = txtLendingInst.Text;
            objfp.approval_code = txtApprovalCode.Text;
            objfp.amount_approved = Convert.ToDecimal(txtAmountApproved.Text);
            objfp.client_id = Convert.ToInt32(hdnClientId.Value);
            objfp.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            objfp.estimate_id = Convert.ToInt32(hdnEstimateId.Value);

            if (Convert.ToInt32(hdnfpId.Value) > 0)
            {
                _db.SubmitChanges();
            }
            else
            {
                _db.finance_projects.InsertOnSubmit(objfp);
                _db.SubmitChanges();
            }
        }

        Response.Redirect("Confirmation.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);

    }
    protected void btnBackToPaymentInfo_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value);
    }
    protected void chkDifferentName_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkDifferentName.ID, chkDifferentName.GetType().Name, "CheckedChanged"); 
        if (chkDifferentName.Checked)
            pnlDifferentName.Visible = true;
        else
            pnlDifferentName.Visible = false;
    }

    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
    protected void btnCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void DeletePayment(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        LinkButton lnkDelete = (LinkButton)sender;
        int nPaymentId = Convert.ToInt32(lnkDelete.CommandArgument);
        string strQ = "DELETE accept_payments WHERE payment_id =" + nPaymentId;
        _db.ExecuteCommand(strQ, string.Empty);

        GetPaymentData();
    }
    protected void chkFinancedProjects_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkFinancedProjects.ID, chkFinancedProjects.GetType().Name, "Click"); 
        if (chkFinancedProjects.Checked)
        {
            pnlFinanceProjects.Visible = true;
            txtLendingInst.Focus();
        }
        else
            pnlFinanceProjects.Visible = false;
    }
}
