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
using System.Drawing;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using CrystalDecisions.Shared;
using System.Net;
using System.Text.RegularExpressions;

public partial class payment_info : System.Web.UI.Page
{
    string sFileName = "test"; 
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

            if (Page.User.IsInRole("admin032") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile oCom = new company_profile();
            oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            if (oCom.CompletionTypeId == 1)
            {
                rdoCompletionType.SelectedValue = "1";

                lblLeadTime.Visible = true;
                txtLeadTime.Visible = true;
                lblStartDate.Visible = false;
                txtStartDate.Visible = false;
                imgStartDate.Visible = false;
                lblCompletionDate.Visible = false;
                txtCompletionDate.Visible = false;
                imgCompletionDate.Visible = false;
            }
            else
            {
                rdoCompletionType.SelectedValue = "2";
                lblLeadTime.Visible = false;
                txtLeadTime.Visible = false;
                lblStartDate.Visible = true;
                txtStartDate.Visible = true;
                imgStartDate.Visible = true;
                lblCompletionDate.Visible = true;
                txtCompletionDate.Visible = true;
                imgCompletionDate.Visible = true;

            }

            // Is Incentive Active/Inactive
            pnlIncentive.Visible = Convert.ToBoolean(oCom.IsIncentiveActive);

            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);



            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                if(cust != null)
                {
                    hdnLastName.Value = cust.last_name1;
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
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                    hdnClientId.Value = cust.client_id.ToString();

                    //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
                }



                if (_db.sales_persons.Any(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

            }
            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                if(cus_est != null)
                {
                    lblEstimateName.Text = cus_est.estimate_name;
                    if (cus_est.tax_rate != null)
                    {
                        txtTax.Text = cus_est.tax_rate.ToString();
                    }
                    if (cus_est.status_id == 3)
                    {
                        btnFinalize.Visible = false;
                    }
                    else
                    {
                        btnFinalize.Visible = true;
                    }

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                }
            }

            // Pull Project Subtotal            
            lblTotalWithTax.Text = lblProjectSubtotal.Text.Trim();
            Company_special_note objCSN = new Company_special_note();
            if (_db.Company_special_notes.Where(csn => csn.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                objCSN = _db.Company_special_notes.Single(csn => csn.client_id == Convert.ToInt32(hdnClientId.Value));

                txtSpecialNote.Text = objCSN.special_note;
            }
            if (_db.estimate_payments.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
            {
                hdnEstPaymentId.Value = "0";
                GetIncentiveData();
                Calculate_Total_price();
                //if (Convert.ToDecimal(txtTax.Text) == 0)
                //{
                //    tblAdjTax.Visible = false;
                //    tblTax.Visible = false;
                //}
                //else
                //{
                //    tblAdjTax.Visible = true;
                //    tblTax.Visible = true;
                //}
            }
            else
            {
                //estimate_payment objEstPay = new estimate_payment();
                estimate_payment objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId  && pay.client_id == Convert.ToInt32(hdnClientId.Value));
                hdnEstPaymentId.Value = objEstPay.est_payment_id.ToString();

                GetPaymentInfo(Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
            }
            CalculateTotal();
            Calculate();
        }



        csCommonUtility.SetPagePermission(this.Page, new string[] { "hypGoogleMap", "imgCompletionDate", "imgStartDate", "imgContractDate", "btnCalculate", "btnGotoPricing", "btnSave", "btnCancelPayment", "chkCVOptions", "btnPrintSummary", "btnContact", "btnEmailSummary", "btnContactMail", "btnEmailSummary", "btnQuickMail", "btnDocuSign", "btnHTML", "pnlDocuConfirmation", "pnlConfirmation", "chkBath", "chkKitchen", "chkShower", "chkTub", "btnFinalize" });

    }
    private void GetPaymentInfo(int nEstimateId, int nCustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();


        objEstPay = _db.estimate_payments.SingleOrDefault(pay => pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId  && pay.client_id == Convert.ToInt32(hdnClientId.Value));

        hdnEstPaymentId.Value = objEstPay.est_payment_id.ToString();

        txtTax.Text = Convert.ToDecimal(objEstPay.tax_rate).ToString();

        if (Convert.ToDecimal(hdnProjectTotal.Value) != Convert.ToDecimal(objEstPay.project_subtotal))
        {
            Calculate_Total_price();


        }
        else
        {
            if (Convert.ToDecimal(objEstPay.project_subtotal) == 0)
            {
                Calculate_Total_price();
            }

            else
            {

                lblProjectSubtotal.Text = Convert.ToDecimal(objEstPay.project_subtotal).ToString("c");
            }

        }
        //txtTax.Text = Convert.ToDecimal(objEstPay.tax_rate).ToString();
        //lblTax.Text = Convert.ToDecimal(objEstPay.tax_amount).ToString("c");
        //lblTotalWithTax.Text = Convert.ToDecimal(objEstPay.total_with_tax).ToString("c");

        string IncIds = objEstPay.incentive_ids;
        if (IncIds != null)
        {
            string strQ = "SELECT * FROM incentives WHERE is_active = 1 AND end_date >= '" + DateTime.Now + "'OR incentive_id IN (" + IncIds + ")";
            IEnumerable<incentive> list = _db.ExecuteQuery<incentive>(strQ, string.Empty);
            grdIncentives.DataSource = list;
            grdIncentives.DataKeyNames = new string[] { "incentive_id", "discount", "amount", "incentive_type" };
            grdIncentives.DataBind();
            if (grdIncentives.Rows.Count > 0)
            {
                tblIncentives.Visible = true;

                lblAdjustedTaxRate.Text = Convert.ToDecimal(objEstPay.adjusted_tax_rate).ToString();

                string[] strIds = IncIds.Split(',');


                decimal inc_discount = 0;
                foreach (GridViewRow dr in grdIncentives.Rows)
                {


                    DateTime dtEsp = Convert.ToDateTime(dr.Cells[4].Text);
                    if (dtEsp <= DateTime.Now)
                        dr.ForeColor = Color.Red;
                    CheckBox chk = (CheckBox)dr.FindControl("chk");
                    int id = Convert.ToInt32(grdIncentives.DataKeys[dr.RowIndex].Values[0]);
                    decimal IncPer = Convert.ToDecimal(grdIncentives.DataKeys[dr.RowIndex].Values[1]);
                    decimal IncAmount = Convert.ToDecimal(grdIncentives.DataKeys[dr.RowIndex].Values[2]);
                    int ntype = Convert.ToInt32(grdIncentives.DataKeys[dr.RowIndex].Values[3]);
                    foreach (string str in strIds)
                    {
                        if (id == Convert.ToInt32(str))
                            chk.Checked = true;
                    }

                    decimal num = 0;
                    if (chk.Checked)
                    {
                        decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
                        if (ntype == 1)
                            num = Convert.ToDecimal(nSubtotal * IncPer / 100); //CalculateIncentives(id);
                        else
                            num = IncAmount;
                        dr.Cells[3].Text = num.ToString("c");
                    }
                    else
                    {
                        dr.Cells[3].Text = "$0.00";
                    }
                    inc_discount += num;

                }
                lblTotalIncentives.Text = inc_discount.ToString("c");

                if (lblTotalIncentives.Text.Trim() != "$0.00")
                {
                    tblAdjustment.Visible = true;
                    CalculateAdjustedPrice();
                }
                else
                {
                    tblAdjustment.Visible = false;
                }


                CalculateAdjustedPrice();
            }
            else
            {
                tblIncentives.Visible = false;
            }
        }
        else
        {
            GetIncentiveData();
        }
        if (objEstPay.lead_time == "" && objEstPay.completion_date == "")
        {
            rdoCompletionType.SelectedValue = "1";

            lblLeadTime.Visible = true;
            txtLeadTime.Visible = true;
            lblStartDate.Visible = false;
            txtStartDate.Visible = false;
            imgStartDate.Visible = false;
            lblCompletionDate.Visible = false;
            txtCompletionDate.Visible = false;
            imgCompletionDate.Visible = false;
        }
        else if (objEstPay.lead_time == "")
        {
            rdoCompletionType.SelectedValue = "2";

            lblLeadTime.Visible = false;
            txtLeadTime.Visible = false;
            lblStartDate.Visible = true;
            txtStartDate.Visible = true;
            imgStartDate.Visible = true;
            lblCompletionDate.Visible = true;
            txtCompletionDate.Visible = true;
            imgCompletionDate.Visible = true;
        }
        else
        {
            rdoCompletionType.SelectedValue = "1";

            lblLeadTime.Visible = true;
            txtLeadTime.Visible = true;
            lblStartDate.Visible = false;
            txtStartDate.Visible = false;
            imgStartDate.Visible = false;
            lblCompletionDate.Visible = false;
            txtCompletionDate.Visible = false;
            imgCompletionDate.Visible = false;
        }

        txtLeadTime.Text = objEstPay.lead_time;
        txtContractDate.Text = objEstPay.contract_date;

        txtpDeposit.Text = Convert.ToDecimal(objEstPay.deposit_percent).ToString();
        txtnDeposit.Text = Convert.ToDecimal(objEstPay.deposit_amount).ToString("c");
        txtpCountertop.Text = Convert.ToDecimal(objEstPay.countertop_percent).ToString();
        txtnCountertop.Text = Convert.ToDecimal(objEstPay.countertop_amount).ToString("c");
        txtpJob.Text = Convert.ToDecimal(objEstPay.start_job_percent).ToString();
        txtnJob.Text = Convert.ToDecimal(objEstPay.start_job_amount).ToString("c");
        txtpBalance.Text = Convert.ToDecimal(objEstPay.due_completion_percent).ToString();
        txtnBalance.Text = Convert.ToDecimal(objEstPay.due_completion_amount).ToString("c");
        txtpMeasure.Text = Convert.ToDecimal(objEstPay.final_measure_percent).ToString();
        txtnMeasure.Text = Convert.ToDecimal(objEstPay.final_measure_amount).ToString("c");
        txtpDelivery.Text = Convert.ToDecimal(objEstPay.deliver_caninet_percent).ToString();
        txtnDelivery.Text = Convert.ToDecimal(objEstPay.deliver_cabinet_amount).ToString("c");
        txtpSubstantial.Text = Convert.ToDecimal(objEstPay.substantial_percent).ToString();
        txtnSubstantial.Text = Convert.ToDecimal(objEstPay.substantial_amount).ToString("c");
        txtOthers.Text = objEstPay.other_value.ToString();
        txtpOthers.Text = Convert.ToDecimal(objEstPay.other_percent).ToString();
        txtnOthers.Text = Convert.ToDecimal(objEstPay.other_amount).ToString("c");

        txtpDrywall.Text = Convert.ToDecimal(objEstPay.drywall_percent).ToString();
        txtnDrywall.Text = Convert.ToDecimal(objEstPay.drywall_amount).ToString("c");
        txtpFlooring.Text = Convert.ToDecimal(objEstPay.flooring_percent).ToString();
        txtnFlooring.Text = Convert.ToDecimal(objEstPay.flooring_amount).ToString("c");

        if (objEstPay.start_date != null)
        {
            txtStartDate.Text = objEstPay.start_date.ToString();
        }
        if (objEstPay.completion_date != null)
        {
            txtCompletionDate.Text = objEstPay.completion_date.ToString();
        }
        if (objEstPay.special_note != null)
        {
            txtSpecialNote.ToolTip = objEstPay.special_note.Replace("^", "'").ToString();
            txtSpecialNote.Text = objEstPay.special_note.Replace("^", "'").ToString();
        }
        
        if (objEstPay.deposit_value != null)
        {
            txtDepositValue.ToolTip = objEstPay.deposit_value.Replace("^", "'").ToString();
            txtDepositValue.Text = objEstPay.deposit_value.Replace("^", "'").ToString();
        }
        if (objEstPay.countertop_value != null)
        {
            txtCountertopValue.ToolTip = objEstPay.countertop_value.Replace("^", "'").ToString();
            txtCountertopValue.Text = objEstPay.countertop_value.Replace("^", "'").ToString();
        }
        if (objEstPay.start_job_value != null)
        {
            txtStartOfJobValue.ToolTip = objEstPay.start_job_value.Replace("^", "'").ToString();
            txtStartOfJobValue.Text = objEstPay.start_job_value.Replace("^", "'").ToString();
        }
        if (objEstPay.due_completion_value != null)
        {
            txtDueCompletionValue.ToolTip = objEstPay.due_completion_value.Replace("^", "'").ToString();
            txtDueCompletionValue.Text = objEstPay.due_completion_value.Replace("^", "'").ToString();
        }
        if (objEstPay.final_measure_value != null)
        {
            txtMeasureValue.ToolTip = objEstPay.final_measure_value.Replace("^", "'").ToString();
            txtMeasureValue.Text = objEstPay.final_measure_value.Replace("^", "'").ToString();
        }
        if (objEstPay.deliver_caninet_value != null)
        {
            txtDeliveryValue.ToolTip = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
            txtDeliveryValue.Text = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
        }
        if (objEstPay.substantial_value != null)
        {
            txtSubstantialValue.ToolTip = objEstPay.substantial_value.Replace("^", "'").ToString();
            txtSubstantialValue.Text = objEstPay.substantial_value.Replace("^", "'").ToString();
        }

        if (objEstPay.drywall_value != null)
        {
            txtStartofDrywallValue.ToolTip = objEstPay.drywall_value.Replace("^", "'").ToString();
            txtStartofDrywallValue.Text = objEstPay.drywall_value.Replace("^", "'").ToString();
        }
        if (objEstPay.flooring_value != null)
        {
            txtStartofFlooringValue.ToolTip = objEstPay.flooring_value.Replace("^", "'").ToString();
            txtStartofFlooringValue.Text = objEstPay.flooring_value.Replace("^", "'").ToString();
        }


        if (objEstPay.deposit_date != null)
            txtDepositDate.Text = objEstPay.deposit_date.ToString();
        if (objEstPay.countertop_date != null)
            txtCountertopDate.Text = objEstPay.countertop_date.ToString();
        if (objEstPay.startof_job_date != null)
            txtStartOfJobDate.Text = objEstPay.startof_job_date.ToString();
        if (objEstPay.due_completion_date != null)
            txtDueCompletionDate.Text = objEstPay.due_completion_date.ToString();
        if (objEstPay.measure_date != null)
            txtMeasureDate.Text = objEstPay.measure_date.ToString();
        if (objEstPay.delivery_date != null)
            txtDeliveryDate.Text = objEstPay.delivery_date.ToString();
        if (objEstPay.substantial_date != null)
            txtSubstantialDate.Text = objEstPay.substantial_date.ToString();
        if (objEstPay.other_date != null)
            txtOtherDate.Text = objEstPay.other_date.ToString();
        if (objEstPay.drywall_date != null)
            txtStartofDrywallDate.Text = objEstPay.drywall_date.ToString();
        if (objEstPay.flooring_date != null)
            txtStartofFlooringDate.Text = objEstPay.flooring_date.ToString();

        if (Convert.ToBoolean(objEstPay.based_on_percent))
            rdoCalc.SelectedValue = "1";
        else
            rdoCalc.SelectedValue = "2";

        if (objEstPay.is_KithenSheet != null)
        {
            chkKitchen.Checked = Convert.ToBoolean(objEstPay.is_KithenSheet);
        }
        if (objEstPay.is_BathSheet != null)
        {
            chkBath.Checked = Convert.ToBoolean(objEstPay.is_BathSheet);
        }
        if (objEstPay.is_ShowerSheet != null)
        {
            chkShower.Checked = Convert.ToBoolean(objEstPay.is_ShowerSheet);
        }
        if (objEstPay.is_TubSheet != null)
        {
            chkTub.Checked = Convert.ToBoolean(objEstPay.is_TubSheet);
        }

        CalculateTotal();

        Calculate();

    }
    private void GetIncentiveData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from inc in _db.incentives
                   where inc.client_id == Convert.ToInt32(hdnClientId.Value) && inc.is_active == Convert.ToBoolean(1) && inc.start_date <= DateTime.Now && inc.end_date >= DateTime.Now
                   orderby inc.incentive_name
                   select inc;
        if (item.Count() > 0)
        {
            tblIncentives.Visible = true;

            grdIncentives.DataSource = item;
            grdIncentives.DataKeyNames = new string[] { "incentive_id", "discount", "amount", "incentive_type" };
            grdIncentives.DataBind();

            CalculateAdjustedPrice();
        }
        else
        {
            tblIncentives.Visible = false;
        }
    }
    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where (from clc in _db.customer_locations
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A"
                      select pd.total_retail_price);
        int n = result.Count();
        if (result != null && n > 0)
            dRetail = result.Sum();

        return dRetail;
    }
    private decimal GetDirctTotal()
    {
        decimal dDirect = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where (from clc in _db.customer_locations
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A"
                      select pd.total_direct_price);
        int n = result.Count();
        if (result != null && n > 0)
            dDirect = result.Sum();

        return dDirect;
    }
    public void Calculate_Total_price()
    {
        decimal direct = 0;
        decimal retail = 0;
        decimal grandtotal = 0;
        direct = GetDirctTotal();
        retail = GetRetailTotal();
        grandtotal = direct + retail;
        lblProjectSubtotal.Text = retail.ToString("c");

        hdnProjectTotal.Value = retail.ToString();
        //lblDirctTotalCost.Text = direct.ToString("c");
        //lblRetailTotalCost.Text = retail.ToString("c");
        //lblGrandTotalCost.Text = grandtotal.ToString("c");

    }
    protected void btnReCalculate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnReCalculate.ID, btnReCalculate.GetType().Name, "Click"); 
        CalculateTotal();
    }
    protected void txtTax_TextChanged(object sender, EventArgs e)
    {
        CalculateTotal();
    }
    private void CalculateTotal()
    {
        lblMessage.Text = "";

        decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nTaxRate = 0;
        if (txtTax.Text.Trim() == "")
            nTaxRate = 0;
        else
            nTaxRate = Convert.ToDecimal(txtTax.Text.Trim());

        //if (nTaxRate == 0)
        //{
        //    tblAdjTax.Visible = false;
        //    tblTax.Visible = false;
        //}
        //else
        //{
        //    tblAdjTax.Visible = true;
        //    tblTax.Visible = true;
        //}

        decimal nTax = Convert.ToDecimal(nSubtotal * nTaxRate / 100);
        lblTax.Text = nTax.ToString("c");
        lblTotalWithTax.Text = Convert.ToDecimal(nSubtotal + nTax).ToString("c");

        lblAdjustedPrice.Text = nSubtotal.ToString("c");
        lblAdjustedTaxRate.Text = txtTax.Text.Trim();
        lblAdjustedTax.Text = lblTax.Text;

        decimal nAdjustedPrice = Convert.ToDecimal(lblAdjustedPrice.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));

        lblNewTotalWithTax.Text = Convert.ToDecimal(nAdjustedPrice + nTax).ToString("c");


        if (lblTotalIncentives.Text.Trim() != "$0.00")
        {
            CalculateAdjustedPrice();
        }
    }
    private void CalculateAdjustedPrice()
    {
        decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nIncentives = Convert.ToDecimal(lblTotalIncentives.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nAdjustedPrice = Convert.ToDecimal(nSubtotal - nIncentives);
        decimal nAdjTaxRate = Convert.ToDecimal(txtTax.Text.Trim());
        decimal nAdjTax = Convert.ToDecimal(nAdjustedPrice * nAdjTaxRate / 100);
        decimal nAdjTotalWithTax = Convert.ToDecimal(nAdjustedPrice + nAdjTax);

        lblAdjustedPrice.Text = nAdjustedPrice.ToString("c");
        lblAdjustedTaxRate.Text = nAdjTaxRate.ToString();
        lblAdjustedTax.Text = nAdjTax.ToString("c");
        lblNewTotalWithTax.Text = nAdjTotalWithTax.ToString("c");
    }
    private bool Calculate()
    {
        try
        {
            bool bFlag = true;
            lblResult.Text = "";
            lblPr.ForeColor = Color.Black;

            double nTotal = 0.0;
            if (tblAdjustment.Visible == true)
            {
                nTotal = Convert.ToDouble(lblNewTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            else
            {
                nTotal = Convert.ToDouble(lblTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            #region Calculate Percentage
            double pDeposit = 0;
            double pMeasure = 0;
            double pStartJob = 0;
            double pDelivery = 0;
            double pCountertop = 0;
            double pSubstantial = 0;
            double pBalance = 0;
            double pDrywall = 0;
            double pFlooring = 0;
            double pOthers = 0;

            double nDeposit = 0;
            double nMeasure = 0;
            double nStartJob = 0;
            double nDelivery = 0;
            double nCountertop = 0;
            double nSubstantial = 0;
            double nBalance = 0;
            double nDrywall = 0;
            double nFlooring = 0;
            double nOthers = 0;


            if (rdoCalc.SelectedIndex == 0)
            {
                #region Calculate Based on Percentage

                txtnDeposit.Text = "";
                txtnMeasure.Text = "";
                txtnJob.Text = "";
                txtnDelivery.Text = "";
                txtnCountertop.Text = "";
                txtnBalance.Text = "";
                txtnDrywall.Text = "";
                txtnFlooring.Text = "";
                txtnOthers.Text = "";
                txtnSubstantial.Text = "";

                if (txtpDeposit.Text.Trim().Length > 0)
                {
                    pDeposit = Convert.ToDouble(txtpDeposit.Text);
                    nDeposit = (nTotal * pDeposit) / 100;
                    txtnDeposit.Text = nDeposit.ToString("c");
                }
                if (txtpMeasure.Text.Trim().Length > 0)
                {
                    pMeasure = Convert.ToDouble(txtpMeasure.Text);
                    nMeasure = (nTotal * pMeasure) / 100;
                    txtnMeasure.Text = nMeasure.ToString("c");
                }
                if (txtpJob.Text.Trim().Length > 0)
                {
                    pStartJob = Convert.ToDouble(txtpJob.Text);
                    nStartJob = (nTotal * pStartJob) / 100;
                    txtnJob.Text = nStartJob.ToString("c");
                }

                if (txtpDelivery.Text.Trim().Length > 0)
                {
                    pDelivery = Convert.ToDouble(txtpDelivery.Text);
                    nDelivery = (nTotal * pDelivery) / 100;
                    txtnDelivery.Text = nDelivery.ToString("c");
                }
                if (txtpCountertop.Text.Trim().Length > 0)
                {
                    pCountertop = Convert.ToDouble(txtpCountertop.Text);
                    nCountertop = (nTotal * pCountertop) / 100;
                    txtnCountertop.Text = nCountertop.ToString("c");
                }
                if (txtpSubstantial.Text.Trim().Length > 0)
                {
                    pSubstantial = Convert.ToDouble(txtpSubstantial.Text);
                    nSubstantial = (nTotal * pSubstantial) / 100;
                    txtnSubstantial.Text = nSubstantial.ToString("c");
                }
                if (txtpBalance.Text.Trim().Length > 0)
                {
                    pBalance = Convert.ToDouble(txtpBalance.Text);
                    nBalance = (nTotal * pBalance) / 100;
                    txtnBalance.Text = nBalance.ToString("c");
                }
                if (txtpDrywall.Text.Trim().Length > 0)
                {
                    pDrywall = Convert.ToDouble(txtpDrywall.Text);
                    nDrywall = (nTotal * pDrywall) / 100;
                    txtnDrywall.Text = nDrywall.ToString("c");
                }

                if (txtpFlooring.Text.Trim().Length > 0)
                {
                    pFlooring = Convert.ToDouble(txtpFlooring.Text);
                    nFlooring = (nTotal * pFlooring) / 100;
                    txtnFlooring.Text = nFlooring.ToString("c");
                }

                if (txtpOthers.Text.Trim().Length > 0)
                {
                    pOthers = Convert.ToDouble(txtpOthers.Text);
                    nOthers = (nTotal * pOthers) / 100;
                    txtnOthers.Text = nOthers.ToString("c");
                }
                double totalPercentage = pDeposit + pMeasure + pStartJob + pDelivery + pCountertop + pSubstantial + pBalance + pDrywall + pFlooring + pOthers;
                lblPr.Text = totalPercentage.ToString("0.00");
                if (totalPercentage == 100)
                    btnSave.Enabled = true;
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage(" % total should be 100%.");

                    bFlag = false;
                }
                #endregion
            }
            else
            {
                #region Calculate Based on Doller Amount
                txtpDeposit.Text = "";
                txtpMeasure.Text = "";
                txtpJob.Text = "";
                txtpDelivery.Text = "";
                txtpCountertop.Text = "";
                txtpBalance.Text = "";
                txtpDrywall.Text = "";
                txtpFlooring.Text = "";
                txtpOthers.Text = "";
                txtpSubstantial.Text = "";

                if (txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", "").Trim().Length > 0)
                {
                    nDeposit = Convert.ToDouble(txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pDeposit = (100 * nDeposit) / nTotal;
                    txtpDeposit.Text = pDeposit.ToString("0.00");
                }
                if (txtnMeasure.Text.Replace("$", "").Trim().Length > 0)
                {
                    nMeasure = Convert.ToDouble(txtnMeasure.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pMeasure = (100 * nMeasure) / nTotal;
                    txtpMeasure.Text = pMeasure.ToString("0.00");
                }
                if (txtnJob.Text.Replace("$", "").Trim().Length > 0)
                {
                    nStartJob = Convert.ToDouble(txtnJob.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pStartJob = (100 * nStartJob) / nTotal;
                    txtpJob.Text = pStartJob.ToString("0.00");
                }
                if (txtnDelivery.Text.Replace("$", "").Trim().Length > 0)
                {
                    nDelivery = Convert.ToDouble(txtnDelivery.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pDelivery = (100 * nDelivery) / nTotal;
                    txtpDelivery.Text = pDelivery.ToString("0.00");
                }
                if (txtnCountertop.Text.Replace("$", "").Trim().Length > 0)
                {
                    nCountertop = Convert.ToDouble(txtnCountertop.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pCountertop = (100 * nCountertop) / nTotal;
                    txtpCountertop.Text = pCountertop.ToString("0.00");
                }
                if (txtnSubstantial.Text.Replace("$", "").Trim().Length > 0)
                {
                    nSubstantial = Convert.ToDouble(txtnSubstantial.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pSubstantial = (100 * nSubstantial) / nTotal;
                    txtpSubstantial.Text = pSubstantial.ToString("0.00");
                }
                if (txtnBalance.Text.Replace("$", "").Trim().Length > 0)
                {
                    nBalance = Convert.ToDouble(txtnBalance.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pBalance = (100 * nBalance) / nTotal;
                    txtpBalance.Text = pBalance.ToString("0.00");
                }

                if (txtnDrywall.Text.Replace("$", "").Trim().Length > 0)
                {
                    nDrywall = Convert.ToDouble(txtnDrywall.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pDrywall = (100 * nDrywall) / nTotal;
                    txtpDrywall.Text = pDrywall.ToString("0.00");
                }

                if (txtnFlooring.Text.Replace("$", "").Trim().Length > 0)
                {
                    nFlooring = Convert.ToDouble(txtnFlooring.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pFlooring = (100 * nFlooring) / nTotal;
                    txtpFlooring.Text = pFlooring.ToString("0.00");
                }

                if (txtnOthers.Text.Replace("$", "").Trim().Length > 0)
                {
                    nOthers = Convert.ToDouble(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    pOthers = (100 * nOthers) / nTotal;
                    txtpOthers.Text = pOthers.ToString("0.00");
                }
                double totalPercentage = pDeposit + pMeasure + pStartJob + pDelivery + pCountertop + pSubstantial + pBalance + pDrywall + pFlooring + pOthers;
                lblPr.Text = totalPercentage.ToString("0.00");
                totalPercentage = Convert.ToDouble(lblPr.Text);
                if (totalPercentage == 100)
                    btnSave.Enabled = true;
                else
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage(" % total should be 100%.");


                    bFlag = false;
                }
                #endregion
            }
            return bFlag;

            #endregion
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            return false;
        }
    }

    protected void grdIncentives_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nIncId = Convert.ToInt32(grdIncentives.DataKeys[e.Row.RowIndex].Values[0]);
            decimal IncPer = Convert.ToDecimal(grdIncentives.DataKeys[e.Row.RowIndex].Values[1]);
            decimal IncAmount = Convert.ToDecimal(grdIncentives.DataKeys[e.Row.RowIndex].Values[2]);
            int ntype = Convert.ToInt32(grdIncentives.DataKeys[e.Row.RowIndex].Values[3]);
            decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));


            CheckBox chk = (CheckBox)e.Row.Cells[0].FindControl("chk");
            if (chk.Checked)
            {
                chk.Checked = true;
                if (ntype == 1)
                    e.Row.Cells[3].Text = Convert.ToDecimal(nSubtotal * IncPer / 100).ToString("c"); //CalculateIncentives(nIncId).ToString("c"); 
                else
                    e.Row.Cells[3].Text = IncAmount.ToString("c");

            }
            else
            {
                chk.Checked = false;
                e.Row.Cells[3].Text = "$0.00";
            }
        }
    }
    //private decimal CalculateIncentives(int nIncId)
    //{
    //    decimal nIncentive = 0; ;
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    incentive inc = new incentive();
    //    inc = _db.incentives.Single(i => i.incentive_id == nIncId);
    //    decimal inc_discount = Convert.ToDecimal(inc.discount);
    //    decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));

    //    nIncentive = Convert.ToDecimal(nSubtotal * inc_discount / 100);

    //    return nIncentive;
    //}
    protected void chk_CheckedChanged(object sender, EventArgs e)
    {
        decimal nSubtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal inc_discount = 0;
        foreach (GridViewRow dr in grdIncentives.Rows)
        {
            CheckBox chk = (CheckBox)dr.FindControl("chk");

            int id = Convert.ToInt32(grdIncentives.DataKeys[dr.RowIndex].Values[0]);
            decimal IncPer = Convert.ToDecimal(grdIncentives.DataKeys[dr.RowIndex].Values[1]);
            decimal IncAmount = Convert.ToDecimal(grdIncentives.DataKeys[dr.RowIndex].Values[2]);
            int ntype = Convert.ToInt32(grdIncentives.DataKeys[dr.RowIndex].Values[3]);

            decimal num = 0;
            if (chk.Checked)
            {
                if (ntype == 1)
                    num = Convert.ToDecimal(nSubtotal * IncPer / 100); //CalculateIncentives(id);
                else
                    num = IncAmount;

                dr.Cells[3].Text = num.ToString("c");
            }
            else
            {
                dr.Cells[3].Text = "$0.00";
            }
            inc_discount += num;

        }
        lblTotalIncentives.Text = inc_discount.ToString("c");

        if (lblTotalIncentives.Text.Trim() != "$0.00")
        {
            tblAdjustment.Visible = true;
            CalculateAdjustedPrice();
        }
        else
        {
            tblAdjustment.Visible = false;
        }
    }
    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCalculate.ID, btnCalculate.GetType().Name, "Click"); 
        Calculate();
    }
    protected void btnGotoPricing_Click(object sender, EventArgs e)
    {
        Response.Redirect("pricing.aspx?eid=" + Convert.ToInt32(hdnEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult.Text = "";
        try
        {
            string strRequired = string.Empty;
            if (txtTax.Text.Trim() == "")
            {
                strRequired = "Missing Tax Rate.<br/>";
            }
            else
            {

                try
                {
                    Convert.ToDecimal(txtTax.Text);
                }
                catch (Exception ex)
                {
                    strRequired += "Invalid Tax Rate.<br/>";

                }
            }

            if (txtContractDate.Text.Trim() != "")
            {
                try
                {
                    Convert.ToDateTime(txtContractDate.Text);
                }
                catch (Exception ex)
                {
                    strRequired += "Contract Date: Invalid date format.<br/>";

                }
            }

            if (rdoCompletionType.SelectedValue == "2")
            {
                if (txtStartDate.Text.Trim() != "")
                {
                    try
                    {
                        Convert.ToDateTime(txtStartDate.Text);
                    }
                    catch (Exception ex)
                    {
                        strRequired += "Start Date: Invalid date format.<br/>";

                    }
                }
                if (txtCompletionDate.Text.Trim() != "")
                {
                    try
                    {
                        Convert.ToDateTime(txtCompletionDate.Text);
                    }
                    catch (Exception ex)
                    {
                        strRequired += "Completion Date: Invalid date format.<br/>";

                    }
                }
            }

            if (strRequired.Length > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                return;
            }


            if (Convert.ToBoolean(Calculate()) == false)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(" % total should be 100%.");

                return;
            }

            DataClassesDataContext _db = new DataClassesDataContext();

            estimate_payment obj = new estimate_payment();
            if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
                obj = _db.estimate_payments.Single(ep => ep.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value));

            obj.client_id = Convert.ToInt32(hdnClientId.Value);
            obj.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            obj.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            obj.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
            obj.project_subtotal = Convert.ToDecimal(lblProjectSubtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            obj.tax_rate = Convert.ToDecimal(txtTax.Text);
            obj.tax_amount = Convert.ToDecimal(lblTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            obj.total_with_tax = Convert.ToDecimal(lblTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            if (rdoCompletionType.SelectedValue == "1")
            {
                obj.lead_time = txtLeadTime.Text;
                obj.start_date = "";
                obj.completion_date = "";
            }
            else
            {
                obj.lead_time = "";
                obj.start_date = txtStartDate.Text;
                obj.completion_date = txtCompletionDate.Text;
            }
            //obj.lead_time = txtLeadTime.Text;
            obj.contract_date = txtContractDate.Text;
            //obj.start_date = txtStartDate.Text;
            //obj.completion_date = txtCompletionDate.Text;

            if (tblAdjustment.Visible == true)
            {
                string Ids = string.Empty;
                foreach (GridViewRow dr in grdIncentives.Rows)
                {
                    CheckBox chk = (CheckBox)dr.FindControl("chk");
                    if (chk.Checked)
                    {
                        int id = Convert.ToInt32(grdIncentives.DataKeys[dr.RowIndex].Values[0]);
                        Ids += id + ",";
                    }
                }
                Ids = Ids.Trim().TrimEnd(',');
                if (Ids != "")
                {
                    obj.is_incentives = true;
                    obj.incentive_ids = Ids;
                    obj.adjusted_price = Convert.ToDecimal(lblAdjustedPrice.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    obj.adjusted_tax_rate = Convert.ToDecimal(lblAdjustedTaxRate.Text);
                    obj.adjusted_tax_amount = Convert.ToDecimal(lblAdjustedTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    obj.new_total_with_tax = Convert.ToDecimal(lblNewTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    obj.total_incentives = Convert.ToDecimal(lblTotalIncentives.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                }
                else
                {
                    obj.is_incentives = false;
                    obj.incentive_ids = null;
                    obj.adjusted_price = Convert.ToDecimal(0.00);
                    obj.adjusted_tax_rate = Convert.ToDecimal(0.00);
                    obj.adjusted_tax_amount = Convert.ToDecimal(0.00);
                    obj.new_total_with_tax = Convert.ToDecimal(lblTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    obj.total_incentives = Convert.ToDecimal(0.00);

                }

            }
            else
            {
                obj.is_incentives = false;
                obj.incentive_ids = null;
                obj.adjusted_price = Convert.ToDecimal(0.00);
                obj.adjusted_tax_rate = Convert.ToDecimal(0.00);
                obj.adjusted_tax_amount = Convert.ToDecimal(0.00);
                obj.new_total_with_tax = Convert.ToDecimal(lblTotalWithTax.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                obj.total_incentives = Convert.ToDecimal(0.00);
            }
            if (txtSpecialNote.Text.Trim() != "")
                obj.special_note = txtSpecialNote.Text.Replace("'", "^");
            if (txtDepositValue.Text.Trim() != "")
                obj.deposit_value = txtDepositValue.Text.Replace("'", "^");
            if (txtCountertopValue.Text.Trim() != "")
                obj.countertop_value = txtCountertopValue.Text.Replace("'", "^");
            if (txtStartOfJobValue.Text.Trim() != "")
                obj.start_job_value = txtStartOfJobValue.Text.Replace("'", "^");
            if (txtDueCompletionValue.Text.Trim() != "")
                obj.due_completion_value = txtDueCompletionValue.Text.Replace("'", "^");
            if (txtMeasureValue.Text.Trim() != "")
                obj.final_measure_value = txtMeasureValue.Text.Replace("'", "^");
            if (txtDeliveryValue.Text.Trim() != "")
                obj.deliver_caninet_value = txtDeliveryValue.Text.Replace("'", "^");
            if (txtSubstantialValue.Text.Trim() != "")
                obj.substantial_value = txtSubstantialValue.Text.Replace("'", "^");
            if (txtStartofDrywallValue.Text.Trim() != "")
                obj.drywall_value = txtStartofDrywallValue.Text.Replace("'", "^");
            if (txtStartofFlooringValue.Text.Trim() != "")
                obj.flooring_value = txtStartofFlooringValue.Text.Replace("'", "^");

            if (txtpDeposit.Text.Trim() != "")
                obj.deposit_percent = Convert.ToDecimal(txtpDeposit.Text);
            else
                obj.deposit_percent = 0;
            if (txtnDeposit.Text.Trim() != "")
                obj.deposit_amount = Convert.ToDecimal(txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.deposit_amount = 0;
            if (txtpCountertop.Text.Trim() != "")
                obj.countertop_percent = Convert.ToDecimal(txtpCountertop.Text);
            else
                obj.countertop_percent = 0;
            if (txtnCountertop.Text != "")
                obj.countertop_amount = Convert.ToDecimal(txtnCountertop.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.countertop_amount = 0;
            if (txtpJob.Text.Trim() != "")
                obj.start_job_percent = Convert.ToDecimal(txtpJob.Text);
            else
                obj.start_job_percent = 0;
            if (txtnJob.Text.Trim() != "")
                obj.start_job_amount = Convert.ToDecimal(txtnJob.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.start_job_amount = 0;
            if (txtpBalance.Text.Trim() != "")
                obj.due_completion_percent = Convert.ToDecimal(txtpBalance.Text);
            else
                obj.due_completion_percent = 0;
            if (txtnBalance.Text.Trim() != "")
                obj.due_completion_amount = Convert.ToDecimal(txtnBalance.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.due_completion_amount = 0;
            if (txtpMeasure.Text.Trim() != "")
                obj.final_measure_percent = Convert.ToDecimal(txtpMeasure.Text);
            else
                obj.final_measure_percent = 0;
            if (txtnMeasure.Text.Trim() != "")
                obj.final_measure_amount = Convert.ToDecimal(txtnMeasure.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.final_measure_amount = 0;
            if (txtpDelivery.Text.Trim() != "")
                obj.deliver_caninet_percent = Convert.ToDecimal(txtpDelivery.Text);
            else
                obj.deliver_caninet_percent = 0;
            if (txtnDelivery.Text.Trim() != "")
                obj.deliver_cabinet_amount = Convert.ToDecimal(txtnDelivery.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.deliver_cabinet_amount = 0;
            if (txtpSubstantial.Text.Trim() != "")
                obj.substantial_percent = Convert.ToDecimal(txtpSubstantial.Text);
            else
                obj.substantial_percent = 0;
            if (txtnSubstantial.Text.Trim() != "")
                obj.substantial_amount = Convert.ToDecimal(txtnSubstantial.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.substantial_amount = 0;

            if (txtpDrywall.Text.Trim() != "")
                obj.drywall_percent = Convert.ToDecimal(txtpDrywall.Text);
            else
                obj.drywall_percent = 0;
            if (txtnDrywall.Text.Trim() != "")
                obj.drywall_amount = Convert.ToDecimal(txtnDrywall.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.drywall_amount = 0;

            if (txtpFlooring.Text.Trim() != "")
                obj.flooring_percent = Convert.ToDecimal(txtpFlooring.Text);
            else
                obj.flooring_percent = 0;
            if (txtnFlooring.Text.Trim() != "")
                obj.flooring_amount = Convert.ToDecimal(txtnFlooring.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.flooring_amount = 0;

            if (txtpOthers.Text.Trim() != "")
                obj.other_percent = Convert.ToDecimal(txtpOthers.Text);
            else
                obj.other_percent = 0;
            if (txtnOthers.Text.Trim() != "")
                obj.other_amount = Convert.ToDecimal(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            else
                obj.other_amount = 0;
            obj.other_value = txtOthers.Text.Replace("&nbsp;", "");

            obj.deposit_date = txtDepositDate.Text;
            obj.countertop_date = txtCountertopDate.Text;
            obj.startof_job_date = txtStartOfJobDate.Text;
            obj.due_completion_date = txtDueCompletionDate.Text;
            obj.measure_date = txtMeasureDate.Text;
            obj.delivery_date = txtDeliveryDate.Text;
            obj.substantial_date = txtSubstantialDate.Text;
            obj.drywall_date = txtStartofDrywallDate.Text;
            obj.flooring_date = txtStartofFlooringDate.Text;
            obj.other_date = txtOtherDate.Text;
            obj.is_KithenSheet = Convert.ToBoolean(chkKitchen.Checked);
            obj.is_BathSheet = Convert.ToBoolean(chkBath.Checked);
            obj.is_ShowerSheet = Convert.ToBoolean(chkShower.Checked);
            obj.is_TubSheet = Convert.ToBoolean(chkTub.Checked);

            if (rdoCalc.SelectedValue == "1")
            {
                obj.based_on_percent = true;
                obj.based_on_dollar = false;
            }
            else
            {
                obj.based_on_percent = false;
                obj.based_on_dollar = true;
            }

            if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
            {
                obj.updated_date = DateTime.Now;

                _db.SubmitChanges();

                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");


            }
            else
            {
                obj.create_date = DateTime.Now;
                obj.updated_date = DateTime.Now;
                string strQ = "DELETE estimate_payments WHERE  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND client_id=" + Convert.ToInt32(hdnClientId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
                _db.ExecuteCommand(strQ, string.Empty);
                _db.estimate_payments.InsertOnSubmit(obj);
                _db.SubmitChanges();

                lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");

            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnContact_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContact.ID, btnContact.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal total_incentives = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";

        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";

        int IsQty = 0;
        int IsSubtotal = 0;
        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsSubtotal = 2;

            }
        }

        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax); //new_total_with_tax
            //if (esp.is_KithenSheet != null)
            //{
            //    is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            //}
            //if (esp.is_BathSheet != null)
            //{
            //    is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            //}
            is_KithenSheet = Convert.ToBoolean(chkKitchen.Checked);
            is_BathSheet = Convert.ToBoolean(chkBath.Checked);
            is_ShowerSheet = Convert.ToBoolean(chkShower.Checked);
            is_TubSheet = Convert.ToBoolean(chkTub.Checked);

            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                //int num = 0;
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                    //num++;
                    //dt = dt.AddDays(num);
                    //if (dt.DayOfWeek == DayOfWeek.Saturday)
                    //{
                    //    dt = dt.AddDays(2);
                    //    num = num + 2;
                    //}
                    //else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    dt = dt.AddDays(1);
                    //    num++;
                    //}


                }
                //dt = dt.AddDays(num);
                strdate = dt.ToShortDateString();
            }
            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();

            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }
        }

        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        } 
        string strCoverLetter = "";
        company_cover_letter objComcl = new company_cover_letter();
        if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value));

            strCoverLetter = objComcl.cover_letter;
        }


        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();
        // DataTable dtCabinet = new DataTable();


        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id  " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value) + " order by item_id asc";

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " )  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value) + " " +
                        "  Union " +
                        " SELECT  * from disclaimers WHERE disclaimers.section_level IN (410001,420001)  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(hdnClientId.Value);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();


        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";

            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;



            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }

        //string strQCabinet = "select * from CabinetSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        //DataTable dtCabi = csCommonUtility.GetDataTable(strQCabinet);
        //if (dtCabi.Rows.Count > 0)
        //{
        //    dtCabinet = dtCabi;
        //}
        //else
        //{
        //    DataTable tmpTable = LoadSectionTable();
        //    DataRow drNew1 = tmpTable.NewRow();

        //    drNew1["CabinetSheetID"] = 0;
        //    drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        //    drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        //    drNew1["UpperWallDoor"] = "";
        //    drNew1["UpperWallWood"] = "";
        //    drNew1["UpperWallStain"] = "";
        //    drNew1["UpperWallExterior"] = "";
        //    drNew1["UpperWallInterior"] = "";
        //    drNew1["UpperWallOther"] = "";
        //    drNew1["BaseDoor"] = "";
        //    drNew1["BaseWood"] = "";
        //    drNew1["BaseStain"] = "";
        //    drNew1["BaseExterior"] = "";
        //    drNew1["BaseInterior"] = "";
        //    drNew1["BaseOther"] = "";
        //    drNew1["MiscDoor"] = "";
        //    drNew1["MiscWood"] = "";
        //    drNew1["MiscStain"] = "";
        //    drNew1["MiscExterior"] = "";
        //    drNew1["MiscInterior"] = "";
        //    drNew1["MiscOther"] = "";
        //    drNew1["LastUpdateDate"] = DateTime.Now;
        //    drNew1["UpdateBy"] = User.Identity.Name;

        //    tmpTable.Rows.InsertAt(drNew1, 0);
        //    dtCabinet = tmpTable;

        //}


        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (rdoSort.SelectedValue == "1")
            strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptContactSection.rpt");
        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);


        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(hdnClientId.Value));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"] == "true")
        {
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
            else
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
        }
        else
        {
            ContactAddress = ContactAddress.Replace("/IICEM", "http://localhost:7854/IICEM");
        }

        //Cover Page Shohid
        try
        {
            string sImagePath = Server.MapPath("Reports\\Common\\pdf_report") + @"\" + DateTime.Now.Ticks.ToString() + ".png";
            csCommonUtility.CreateContactAddressImage(ContactAddress, sImagePath);

            rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sImagePath + "'";
        }
        catch (Exception ex)
        {
            throw ex;
        }

        //ReportDocument subCabinetSheet = rptFile.OpenSubreport("rptCabinetSelection.rpt");
        //subCabinetSheet.SetDataSource(dtCabinet);
        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        string CustPortalUrl = ConfigurationManager.AppSettings["CustPortalUrl"];

        CustPortalUrl = CustPortalUrl + "?cid=" + hdnCustomerId.Value;


        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_CoverLettter", strCoverLetter);

        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);
        ht.Add("p_CustPortalUrl", CustPortalUrl);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("CabinetSheetID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("UpperWallDoor", typeof(string));
        table.Columns.Add("UpperWallWood", typeof(string));
        table.Columns.Add("UpperWallStain", typeof(string));
        table.Columns.Add("UpperWallExterior", typeof(string));
        table.Columns.Add("UpperWallInterior", typeof(string));
        table.Columns.Add("UpperWallOther", typeof(string));
        table.Columns.Add("BaseDoor", typeof(string));
        table.Columns.Add("BaseWood", typeof(string));
        table.Columns.Add("BaseStain", typeof(string));
        table.Columns.Add("BaseExterior", typeof(string));
        table.Columns.Add("BaseInterior", typeof(string));
        table.Columns.Add("BaseOther", typeof(string));
        table.Columns.Add("MiscDoor", typeof(string));
        table.Columns.Add("MiscWood", typeof(string));
        table.Columns.Add("MiscStain", typeof(string));
        table.Columns.Add("MiscExterior", typeof(string));
        table.Columns.Add("MiscInterior", typeof(string));
        table.Columns.Add("MiscOther", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }
    private DataTable LoadBathRoomTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("BathroomID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("BathroomSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Valve_Qty", typeof(string));
        table.Columns.Add("Sink_Valve_Style", typeof(string));
        table.Columns.Add("Sink_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Bathtub_Qty", typeof(string));
        table.Columns.Add("Bathtub_Style", typeof(string));
        table.Columns.Add("Bathtub_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Faucet_Qty", typeof(string));
        table.Columns.Add("Tub_Faucet_Style", typeof(string));
        table.Columns.Add("Tub_Faucet_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Valve_Qty", typeof(string));
        table.Columns.Add("Tub_Valve_Style", typeof(string));
        table.Columns.Add("Tub_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Drain_Qty", typeof(string));
        table.Columns.Add("Tub_Drain_Style", typeof(string));
        table.Columns.Add("Tub_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Tollet_Qty", typeof(string));
        table.Columns.Add("Tollet_Style", typeof(string));
        table.Columns.Add("Tollet_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_TubSystem_Qty", typeof(string));
        table.Columns.Add("Shower_TubSystem_Style", typeof(string));
        table.Columns.Add("Shower_TubSystem_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Value_Qty", typeof(string));
        table.Columns.Add("Shower_Value_Style", typeof(string));
        table.Columns.Add("Shower_Value_WhereToOrder", typeof(string));
        table.Columns.Add("Handheld_Shower_Qty", typeof(string));
        table.Columns.Add("Handheld_Shower_Style", typeof(string));
        table.Columns.Add("Handheld_Shower_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Style", typeof(string));
        table.Columns.Add("Body_Spray_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Style", typeof(string));
        table.Columns.Add("Body_Spray_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Style", typeof(string));
        table.Columns.Add("Shower_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Qty", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Style", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Qty", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Style", typeof(string));
        table.Columns.Add("Tub_skirt_tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Qty", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Style", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_Style", typeof(string));
        table.Columns.Add("Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Style", typeof(string));
        table.Columns.Add("Floor_Tile_layout_WhereToOrder", typeof(string));
        table.Columns.Add("BullnoseTile_Qty", typeof(string));
        table.Columns.Add("BullnoseTile_Style", typeof(string));
        table.Columns.Add("BullnoseTile_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Style", typeof(string));
        table.Columns.Add("Deco_Band_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Height_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Height_Style", typeof(string));
        table.Columns.Add("Deco_Band_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Tile_Baseboard_Qty", typeof(string));
        table.Columns.Add("Tile_Baseboard_Style", typeof(string));
        table.Columns.Add("Tile_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Grout_Selection_Qty", typeof(string));
        table.Columns.Add("Grout_Selection_Style", typeof(string));
        table.Columns.Add("Grout_Selection_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Location_Qty", typeof(string));
        table.Columns.Add("Niche_Location_Style", typeof(string));
        table.Columns.Add("Niche_Location_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Size_Qty", typeof(string));
        table.Columns.Add("Niche_Size_Style", typeof(string));
        table.Columns.Add("Niche_Size_WhereToOrder", typeof(string));
        table.Columns.Add("Glass_Qty", typeof(string));
        table.Columns.Add("Glass_Style", typeof(string));
        table.Columns.Add("Glass_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Grab_Bar_Qty", typeof(string));
        table.Columns.Add("Grab_Bar_Style", typeof(string));
        table.Columns.Add("Grab_Bar_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Qty", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Style", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Qty", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Style", typeof(string));
        table.Columns.Add("Medicine_Cabinet_WhereToOrder", typeof(string));
        table.Columns.Add("Mirror_Qty", typeof(string));
        table.Columns.Add("Mirror_Style", typeof(string));
        table.Columns.Add("Mirror_WhereToOrder", typeof(string));
        table.Columns.Add("Wood_Baseboard_Qty", typeof(string));
        table.Columns.Add("Wood_Baseboard_Style", typeof(string));
        table.Columns.Add("Wood_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Paint_Color_Qty", typeof(string));
        table.Columns.Add("Paint_Color_Style", typeof(string));
        table.Columns.Add("Paint_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("TowelRing_Qty", typeof(string));
        table.Columns.Add("TowelRing_Style", typeof(string));
        table.Columns.Add("TowelRing_WhereToOrder", typeof(string));
        table.Columns.Add("TowelBar_Qty", typeof(string));
        table.Columns.Add("TowelBar_Style", typeof(string));
        table.Columns.Add("TowelBar_WhereToOrder", typeof(string));
        table.Columns.Add("TissueHolder_Qty", typeof(string));
        table.Columns.Add("TissueHolder_Style", typeof(string));
        table.Columns.Add("TissueHolder_WhereToOrder", typeof(string));
        table.Columns.Add("ClosetDoorSeries", typeof(string));
        table.Columns.Add("ClosetDoorOpeningSize", typeof(string));
        table.Columns.Add("ClosetDoorNumberOfPanels", typeof(string));
        table.Columns.Add("ClosetDoorFinish", typeof(string));
        table.Columns.Add("ClosetDoorInsert", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("KitchenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinets_Qty", typeof(string));
        table.Columns.Add("Cabinets_Style", typeof(string));
        table.Columns.Add("Cabinets_WhereToOrder", typeof(string));
        table.Columns.Add("Disposal_Qty", typeof(string));
        table.Columns.Add("Disposal_Style", typeof(string));
        table.Columns.Add("Disposal_WhereToOrder", typeof(string));
        table.Columns.Add("Baseboard_Qty", typeof(string));
        table.Columns.Add("Baseboard_Style", typeof(string));
        table.Columns.Add("Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("AutoKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenTileSheetName", typeof(string));
        table.Columns.Add("BacksplashQTY", typeof(string));
        table.Columns.Add("BacksplashMOU", typeof(string));
        table.Columns.Add("BacksplashStyle", typeof(string));
        table.Columns.Add("BacksplashColor", typeof(string));
        table.Columns.Add("BacksplashSize", typeof(string));
        table.Columns.Add("BacksplashVendor", typeof(string));
        table.Columns.Add("BacksplashPattern", typeof(string));
        table.Columns.Add("BacksplashGroutColor", typeof(string));
        table.Columns.Add("BBullnoseQTY", typeof(string));
        table.Columns.Add("BBullnoseMOU", typeof(string));
        table.Columns.Add("BBullnoseStyle", typeof(string));
        table.Columns.Add("BBullnoseColor", typeof(string));
        table.Columns.Add("BBullnoseSize", typeof(string));
        table.Columns.Add("BBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));


        return table;
    }

    private DataTable LoadTubTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("TubID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("TubTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("ShelfLocation", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }

    private DataTable LoadShowerTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ShowerKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("ShowerTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("ShowerPanQTY", typeof(string));
        table.Columns.Add("ShowerPanMOU", typeof(string));
        table.Columns.Add("ShowerPanStyle", typeof(string));
        table.Columns.Add("ShowerPanColor", typeof(string));
        table.Columns.Add("ShowerPanSize", typeof(string));
        table.Columns.Add("ShowerPanVendor", typeof(string));
        table.Columns.Add("ShowerPanPattern", typeof(string));
        table.Columns.Add("ShowerPanGroutColor", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("BenchTileQTY", typeof(string));
        table.Columns.Add("BenchTileMOU", typeof(string));
        table.Columns.Add("BenchTileStyle", typeof(string));
        table.Columns.Add("BenchTileColor", typeof(string));
        table.Columns.Add("BenchTileSize", typeof(string));
        table.Columns.Add("BenchTileVendor", typeof(string));
        table.Columns.Add("BenchLocation", typeof(string));
        table.Columns.Add("BenchSize", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }
    protected void btnAcceptPayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAcceptPayment.ID, btnAcceptPayment.GetType().Name, "Click"); 
        //if (txtnDeposit.Text.Trim() != "")
        //{
        //    decimal nDeposit = Convert.ToDecimal(txtnDeposit.Text.Trim().Replace("$", ""));
        //    Response.Redirect("payment.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
        //}
        //if (txtnDeposit.Text.Trim() != "")
        //{
        //    decimal nDeposit = Convert.ToDecimal(txtnDeposit.Text.Trim().Replace("$", ""));
        //    Response.Redirect("payment_withco.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
        //}
        if (txtnDeposit.Text.Trim() != "")
        {
            decimal nDeposit = Convert.ToDecimal(txtnDeposit.Text.Trim().Replace("$", ""));
            Response.Redirect("payment_recieved.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
        }

    }
    protected void btnHTML_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnHTML.ID, btnHTML.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal total_incentives = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";

        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";

        int IsQty = 0;
        int IsSubtotal = 0;
        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsSubtotal = 2;

            }
        }

        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.SingleOrDefault(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax);//new_total_with_tax
            is_KithenSheet = Convert.ToBoolean(chkKitchen.Checked);
            is_BathSheet = Convert.ToBoolean(chkBath.Checked);
            is_ShowerSheet = Convert.ToBoolean(chkShower.Checked);
            is_TubSheet = Convert.ToBoolean(chkTub.Checked);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);

            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                //int num = 0;
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                    //num++;
                    //dt = dt.AddDays(num);
                    //if (dt.DayOfWeek == DayOfWeek.Saturday)
                    //{
                    //    dt = dt.AddDays(2);
                    //    num = num + 2;
                    //}
                    //else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    dt = dt.AddDays(1);
                    //    num++;
                    //}


                }
                //dt = dt.AddDays(num);
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();

            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();
            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";
            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();

            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }
        }

        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }


        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name," +
                    " CASE WHEN item_name LIKE '%>>'  THEN LEFT(item_name, LEN(item_name)-2) ELSE item_name END AS item_name, " +
                    " measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, "+
                    " CASE WHEN LEN(short_notes)>0 THEN '' + CHAR(13) + 'NOTES: '+ short_notes ELSE short_notes END AS short_notes, " +                  
                    " location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id  " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value) + " order by location_name asc";

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();

        


        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();

        #region
       
        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";

            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;



            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }
        #endregion

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (rdoSort.SelectedValue == "1")
            strReportPath = Server.MapPath(@"Reports\rpt\rptShortContact.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptShortContactSection.rpt");
        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);

        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }
    protected void rdoCompletionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoCompletionType.ID, rdoCompletionType.GetType().Name, "SelectedIndexChanged"); 
        if (rdoCompletionType.SelectedValue == "1")
        {
            lblLeadTime.Visible = true;
            txtLeadTime.Visible = true;
            lblStartDate.Visible = false;
            txtStartDate.Visible = false;
            imgStartDate.Visible = false;
            lblCompletionDate.Visible = false;
            txtCompletionDate.Visible = false;
            imgCompletionDate.Visible = false;
        }
        else
        {
            lblLeadTime.Visible = false;
            txtLeadTime.Visible = false;
            lblStartDate.Visible = true;
            txtStartDate.Visible = true;
            imgStartDate.Visible = true;
            lblCompletionDate.Visible = true;
            txtCompletionDate.Visible = true;
            imgCompletionDate.Visible = true;
        }
    }
    protected void btnFinalize_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnFinalize.ID, btnFinalize.GetType().Name, "Click"); 
        try
        {
            btnSave_Click(sender, e);
            lblMessage.Text = "";
            lblResult.Text = "";

            string strNewName = lblEstimateName.Text.Trim();
            DataClassesDataContext _db = new DataClassesDataContext();
            customer_estimate cus_est = new customer_estimate();
            int ncid = Convert.ToInt32(hdnCustomerId.Value);
            int nestId = Convert.ToInt32(hdnEstimateId.Value);

            if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0)
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == ncid && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(hdnClientId.Value));

            if (txtContractDate.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Contract Date is a required field.");
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Contract Date is a required field.");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDateTime(txtContractDate.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Contract Date.");
                    lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Contract Date.");
                    return;
                }

                cus_est.sale_date = txtContractDate.Text;
            }

            cus_est.estimate_name = strNewName;
            cus_est.status_id = 3;


            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            string strEmp = sp.first_name.Substring(0, 1) + "" + sp.last_name.Substring(0, 1);

            int nJId = 0;
            try
            {
                var result = (from ce in _db.customer_estimates
                              where ce.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value)  && Convert.ToDateTime(ce.sale_date)>=Convert.ToDateTime("2020-12-20")
                              select (int)ce.JobId);

                int n = result.Count();
                if (result != null && n > 0)
                    nJId = n;// result.Max();
            }
            catch
            {
            }
            nJId = nJId + 1;

            string sJobNumber = string.Empty;
            //if (nJId.ToString().Length == 1)
            //{
            //    sJobNumber = strEmp + "0000" + nJId.ToString();
            //}
            //else if (nJId.ToString().Length == 2)
            //{
            //    sJobNumber = strEmp + "000" + nJId.ToString();
            //}
            //else if (nJId.ToString().Length == 3)
            //{
            //    sJobNumber = strEmp + "00" + nJId.ToString();
            //}
            //else if (nJId.ToString().Length == 4)
            //{
            //    sJobNumber = strEmp + "0" + nJId.ToString();
            //}
            //else
            //{
            //    sJobNumber = strEmp + nJId.ToString();
            //}

            if (nJId<10)
            {
                sJobNumber = strEmp + "500" + nJId.ToString();
            }
            else if  (nJId<100)
            {
                sJobNumber = strEmp + "50" + nJId.ToString();
            }
            else
            {
                sJobNumber = strEmp + "5" + nJId.ToString();
            }
          
            


            // string sJobNumber = strEmp + "1" + cus_est.customer_id.ToString() + cus_est.estimate_id.ToString() + System.DateTime.Now.Year.ToString().Substring(3, 1);

            //string sJobNumber = "1" + cus_est.customer_id.ToString() + cus_est.estimate_id.ToString() + System.DateTime.Now.Year.ToString().Substring(3, 1) + strEmp;

            string strQ = "UPDATE customer_estimate SET job_number = '" + sJobNumber + "', JobId = " + nJId + ", estimate_name='" + cus_est.estimate_name.Replace("'", "''") + "',sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " , sale_date='" + cus_est.sale_date + "',estimate_comments='" + cus_est.estimate_comments.Replace("'", "''") + "',status_id=" + cus_est.status_id + "  WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            string strCustQ = "UPDATE customers SET  isCustomer = 1, islead = 0  WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strCustQ, string.Empty);

            string strSrwNQ = "UPDATE SiteReviewNotes SET  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
            _db.ExecuteCommand(strSrwNQ, string.Empty);

            string strSrwFQ = "UPDATE SiteReview_upolad_info SET  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
            _db.ExecuteCommand(strSrwFQ, string.Empty);
            
            lblResult.Text = csCommonUtility.GetSystemMessage("Data update successfully.");
            Response.Redirect("sold_estimate.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnCancelPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void btnDocuSign_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDocuSign.ID, btnDocuSign.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";

        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";
        int IsQty = 0;
        int IsSubtotal = 0;
        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsSubtotal = 2;

            }
        }

        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax);
            //if (esp.is_KithenSheet != null)
            //{
            //    is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            //}
            //if (esp.is_BathSheet != null)
            //{
            //    is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            //}
            is_KithenSheet = Convert.ToBoolean(chkKitchen.Checked);
            is_BathSheet = Convert.ToBoolean(chkBath.Checked);
            is_ShowerSheet = Convert.ToBoolean(chkShower.Checked);
            is_TubSheet = Convert.ToBoolean(chkTub.Checked);

            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                //int num = 0;
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                    //num++;
                    //dt = dt.AddDays(num);
                    //if (dt.DayOfWeek == DayOfWeek.Saturday)
                    //{
                    //    dt = dt.AddDays(2);
                    //    num = num + 2;
                    //}
                    //else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    dt = dt.AddDays(1);
                    //    num++;
                    //}


                }
                //dt = dt.AddDays(num);
                strdate = dt.ToShortDateString();
            }
            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();


            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }
        }

        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }
        string strCoverLetter = "";
        company_cover_letter objComcl = new company_cover_letter();
        if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value));

            strCoverLetter = objComcl.cover_letter;
        }


        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = oCom.address;
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();
        // DataTable dtCabinet = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id  " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value) + " order by item_id asc";

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " )  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value) + " " +
                        "  Union " +
                        " SELECT  * from disclaimers WHERE disclaimers.section_level IN (410001,420001)  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(hdnClientId.Value);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();



        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }

        //string strQCabinet = "select * from CabinetSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        //DataTable dtCabi = csCommonUtility.GetDataTable(strQCabinet);
        //if (dtCabi.Rows.Count > 0)
        //{
        //    dtCabinet = dtCabi;
        //}
        //else
        //{
        //    DataTable tmpTable = LoadSectionTable();
        //    DataRow drNew1 = tmpTable.NewRow();

        //    drNew1["CabinetSheetID"] = 0;
        //    drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        //    drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        //    drNew1["UpperWallDoor"] = "";
        //    drNew1["UpperWallWood"] = "";
        //    drNew1["UpperWallStain"] = "";
        //    drNew1["UpperWallExterior"] = "";
        //    drNew1["UpperWallInterior"] = "";
        //    drNew1["UpperWallOther"] = "";
        //    drNew1["BaseDoor"] = "";
        //    drNew1["BaseWood"] = "";
        //    drNew1["BaseStain"] = "";
        //    drNew1["BaseExterior"] = "";
        //    drNew1["BaseInterior"] = "";
        //    drNew1["BaseOther"] = "";
        //    drNew1["MiscDoor"] = "";
        //    drNew1["MiscWood"] = "";
        //    drNew1["MiscStain"] = "";
        //    drNew1["MiscExterior"] = "";
        //    drNew1["MiscInterior"] = "";
        //    drNew1["MiscOther"] = "";
        //    drNew1["LastUpdateDate"] = DateTime.Now;
        //    drNew1["UpdateBy"] = User.Identity.Name;

        //    tmpTable.Rows.InsertAt(drNew1, 0);
        //    dtCabinet = tmpTable;

        //}

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (rdoSort.SelectedValue == "1")
            strReportPath = Server.MapPath(@"Reports\rpt\rptDocuSignContact.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptDocuSignContactSection.rpt");
        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);
        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

        //ReportDocument subCabinetSheet = rptFile.OpenSubreport("rptCabinetSelection.rpt");
        //subCabinetSheet.SetDataSource(dtCabinet);

        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(hdnClientId.Value));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"] == "true")
        {
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
            else
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
        }
        else
        {
            ContactAddress = ContactAddress.Replace("/IICEM", "http://localhost:7854/IICEM");
        }

        //Cover Page Shohid
        try
        {
            string sImagePath = Server.MapPath("Reports\\Common\\pdf_report") + @"\" + DateTime.Now.Ticks.ToString() + ".png";
            csCommonUtility.CreateContactAddressImage(ContactAddress, sImagePath);

            rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sImagePath + "'";
        }
        catch (Exception ex)
        {
            throw ex;
        }

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        string strId = hdnCustomerId.Value + "_" + hdnEstimateId.Value + "_" + Convert.ToInt32(hdnClientId.Value);

        string CustPortalUrl = ConfigurationManager.AppSettings["CustPortalUrl"];
        CustPortalUrl = CustPortalUrl + "?cid=" + hdnCustomerId.Value;
        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_CoverLettter", strCoverLetter);

        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);
        ht.Add("p_CustomerEstimateId", strId);
        ht.Add("p_CustPortalUrl", CustPortalUrl);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);


        rptFile = (ReportDocument)Session[SessionInfo.Report_File];
        bool bParam = false;
        foreach (string strKey in Session.Keys)
        {
            if (strKey == SessionInfo.Report_Param)
            {
                bParam = true;
                break;
            }
        }
        if (bParam)
        {
            Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
            ParameterValues param = new ParameterValues();
            ParameterDiscreteValue Val = new ParameterDiscreteValue();
            foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
            {
                if (htable.ContainsKey(obj.Name))
                {
                    Val.Value = htable[obj.Name].ToString();
                    param.Add(Val);
                    obj.ApplyCurrentValues(param);
                    if (obj.Name.ToString() == "p_CustomerEstimateId")
                        sFileName = Val.Value.ToString();

                }
            }
        }
        CrystalDecisions.Web.CrystalReportViewer CRViewer = new CrystalDecisions.Web.CrystalReportViewer();
        CRViewer.ReportSource = rptFile;

        exportReport(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
    }

    protected void exportReport(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {
        selectedReport.ExportOptions.ExportFormatType = eft;

        string contentType = "";
        // Make sure asp.net has create and delete permissions in the directory
        //string tempDir = System.Configuration.ConfigurationSettings.AppSettings["TempDir"];
        string tempDir = ConfigurationManager.AppSettings["ReportPath"];
        //string tempFileName = Session.SessionID.ToString() + ".";
        string tempFileName = sFileName + ".";

        switch (eft)
        {
            case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                tempFileName += "pdf";
                contentType = "application/pdf";
                break;
            case CrystalDecisions.Shared.ExportFormatType.WordForWindows:
                tempFileName += "doc";
                contentType = "application/msword";
                break;
            case CrystalDecisions.Shared.ExportFormatType.Excel:
                tempFileName += "xls";
                contentType = "application/vnd.ms-excel";
                break;
            case CrystalDecisions.Shared.ExportFormatType.HTML32:
            case CrystalDecisions.Shared.ExportFormatType.HTML40:
                tempFileName += "htm";
                contentType = "text/html";
                CrystalDecisions.Shared.HTMLFormatOptions hop = new CrystalDecisions.Shared.HTMLFormatOptions();
                hop.HTMLBaseFolderName = tempDir;
                hop.HTMLFileName = tempFileName;
                selectedReport.ExportOptions.FormatOptions = hop;
                break;
        }

        CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
        dfo.DiskFileName = tempDir + tempFileName;
        selectedReport.ExportOptions.DestinationOptions = dfo;
        selectedReport.ExportOptions.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
        selectedReport.Export();
        selectedReport.Close();

        string FilePath = Server.MapPath("Reports\\Common\\tmp") + @"\" + sFileName + ".pdf";

        if (lblEmail.Text.Length > 0)
        {
            try
            {
                if (CreateSendEnvelope(FilePath, lblEmail.Text, lblCustomerName.Text))
                    lblResult.Text = csCommonUtility.GetSystemMessage("Contract Document has been sent to Customer's email for electronic signature by using DocuSign");
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("DocuSign Error: " + ex.Message);
            }
        }



    }

    public bool CreateSendEnvelope(string pdfPath, string CustomerEmail, string CustomerName)
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string username = ConfigurationManager.AppSettings["APIUserEmail"];
            string password = ConfigurationManager.AppSettings["Password"];
            string integratorKey = ConfigurationManager.AppSettings["IntegratorsKey"];
            string apiURL = ConfigurationManager.AppSettings["APIUrl"];

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            //string designerName = "John Smith";
            //string designerEmail = "avijit@faztrack.com";

            string designerName = "Ann Lyons";
            string designerEmail = "alyons@interiorinnov.com"; //"alyons@interiorinnov.com";


            ApiClient apiClient = new ApiClient(apiURL);

            DocuSign.eSign.Client.Configuration cfi = new DocuSign.eSign.Client.Configuration(apiClient);

            // configure 'X-DocuSign-Authentication' header

            string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";

            cfi.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login API call

            AuthenticationApi authApi = new AuthenticationApi(cfi);

            LoginInformation loginInfo = authApi.Login();

            string accountID = loginInfo.LoginAccounts[0].AccountId;


            if (!string.IsNullOrEmpty(accountID))
            {

                if (System.IO.File.Exists(pdfPath))
                {

                    byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath);



                    EnvelopeDefinition envDef = new EnvelopeDefinition();

                    envDef.EmailSubject = "[DocuSign] - Please sign this doc";



                    Document doc = new Document();

                    doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);

                    doc.Name = "Contract Documents";

                    doc.DocumentId = "1";



                    envDef.Documents = new List<Document>();

                    envDef.Documents.Add(doc);

                    #region Customer

                    Signer signerCust = new Signer();

                    signerCust.Email = CustomerEmail;

                    signerCust.Name = CustomerName;

                    signerCust.RecipientId = "1";
                    signerCust.RoutingOrder = "1";

                    signerCust.Tabs = new Tabs();


                    // SignHere 1
                    signerCust.Tabs.SignHereTabs = new List<SignHere>();

                    SignHere signHere1 = new SignHere();

                    signHere1.DocumentId = "1";

                    signHere1.AnchorString = "Owner / Purchaser";


                    signHere1.AnchorXOffset = "-10";

                    signHere1.AnchorYOffset = "-40";

                    SignHere signHere2 = new SignHere();

                    signHere2.DocumentId = "1";

                    signHere2.AnchorString = "Client:";


                    signHere2.AnchorXOffset = "50";

                    signHere2.AnchorYOffset = "0";

                    signerCust.Tabs.SignHereTabs.Add(signHere2);

                    signerCust.Tabs.SignHereTabs.Add(signHere1);




                    signerCust.Tabs.DateSignedTabs = new List<DateSigned>();
                    DateSigned signHereDate1 = new DateSigned();

                    signHereDate1.DocumentId = "1";

                    signHereDate1.AnchorString = "Owner / Purchaser";
                    signHereDate1.AnchorXOffset = "-10";

                    signHereDate1.AnchorYOffset = "-18";

                    DateSigned signHereDate2 = new DateSigned();

                    signHereDate2.DocumentId = "1";

                    signHereDate2.AnchorString = "Date :";
                    signHereDate2.AnchorXOffset = "50";

                    signHereDate2.AnchorYOffset = "-8";

                    signerCust.Tabs.DateSignedTabs.Add(signHereDate2);

                    signerCust.Tabs.DateSignedTabs.Add(signHereDate1);

                    //Signhere for Cabinet

                    if (_db.CabinetSheetSelections.Where(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                    {
                        // SignHere 3

                        SignHere signHereCabinet = new SignHere();

                        signHereCabinet.DocumentId = "1";

                        signHereCabinet.AnchorString = "Accepted By Owner:";


                        signHereCabinet.AnchorXOffset = "10";

                        signHereCabinet.AnchorYOffset = "40";

                        signerCust.Tabs.SignHereTabs.Add(signHereCabinet);

                        DateSigned signHereDateCabinet = new DateSigned();

                        signHereDateCabinet.DocumentId = "1";

                        signHereDateCabinet.AnchorString = "Dated :";
                        signHereDateCabinet.AnchorXOffset = "50";

                        signHereDateCabinet.AnchorYOffset = "-8";

                        signerCust.Tabs.DateSignedTabs.Add(signHereDateCabinet);

                    }


                    // InitialHere 1


                    signerCust.Tabs.InitialHereTabs = new List<InitialHere>();
                    InitialHere signHere3 = new InitialHere();

                    signHere3.DocumentId = "1";

                    //signHere2.PageNumber = "1";

                    //signHere2.RecipientId = "1";

                    signHere3.AnchorString = "_ ________";
                    signHere3.AnchorXOffset = "-2";

                    signHere3.AnchorYOffset = "-1";

                    signerCust.Tabs.InitialHereTabs.Add(signHere3);

                    #endregion


                    #region Designer/Owner


                    Signer signerDesigner = new Signer();

                    signerDesigner.Email = designerEmail;

                    signerDesigner.Name = designerName;

                    signerDesigner.RecipientId = "2";
                    signerDesigner.RoutingOrder = "2";

                    signerDesigner.Tabs = new Tabs();


                    // SignHere 1
                    signerDesigner.Tabs.SignHereTabs = new List<SignHere>();

                    SignHere signHereDesigner1 = new SignHere();

                    signHereDesigner1.DocumentId = "1";

                    signHereDesigner1.AnchorString = "Design Consultant";


                    signHereDesigner1.AnchorXOffset = "-15";

                    signHereDesigner1.AnchorYOffset = "-40";

                    SignHere signHereDesigner2 = new SignHere();

                    signHereDesigner2.DocumentId = "1";

                    signHereDesigner2.AnchorString = "Designer: _____________________________";


                    signHereDesigner2.AnchorXOffset = "67";

                    signHereDesigner2.AnchorYOffset = "0";

                    signerDesigner.Tabs.SignHereTabs.Add(signHereDesigner1);

                    signerDesigner.Tabs.SignHereTabs.Add(signHereDesigner2);


                    signerDesigner.Tabs.DateSignedTabs = new List<DateSigned>();
                    DateSigned signHereDesignerDate1 = new DateSigned();

                    signHereDesignerDate1.DocumentId = "1";

                    signHereDesignerDate1.AnchorString = "Design Consultant";
                    signHereDesignerDate1.AnchorXOffset = "-15";

                    signHereDesignerDate1.AnchorYOffset = "-18";

                    DateSigned signHereDesignerDate2 = new DateSigned();

                    signHereDesignerDate2.DocumentId = "1";

                    signHereDesignerDate2.AnchorString = "Designer: _____________________________";
                    signHereDesignerDate2.AnchorXOffset = "67";

                    signHereDesignerDate2.AnchorYOffset = "25";

                    signerDesigner.Tabs.DateSignedTabs.Add(signHereDesignerDate1);

                    signerDesigner.Tabs.DateSignedTabs.Add(signHereDesignerDate2);

                    #endregion



                    envDef.Recipients = new Recipients();

                    envDef.Recipients.Signers = new List<Signer>();

                    envDef.Recipients.Signers.Add(signerCust);
                    envDef.Recipients.Signers.Add(signerDesigner);




                    // set envelope status to "sent" to immediately send the signature request 

                    envDef.Status = "sent";



                    // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests) 



                    EnvelopesApi envelopesApi = new EnvelopesApi(cfi);

                    EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountID, envDef);

                    return true;



                    // print the JSON response 

                    // Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));



                    //APIServiceSoapClient apiService = new APIServiceSoapClient();

                    //return envelopeSummary; 


                    //  Console.ReadKey();



                }



            }
        }
        catch (Exception ex)
        {
            throw ex;

        }
        return false;



    }

  
    protected void btnQuickMail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnQuickMail.ID, btnQuickMail.GetType().Name, "Click"); 
        //int nTypeID = Convert.ToInt32(rdoSort.SelectedValue);
        //string url = "Quickcontract_email.aspx?custId=" + hdnCustomerId.Value + "&Typeid=" + nTypeID + "&sid=" + hdnSalesPersonId.Value + "&eid=" + hdnEstimateId.Value;
        //string Script = @"<script language=JavaScript>window.open('" + url + "'); opener.document.forms[0].submit(); </script>";
        //if (!IsClientScriptBlockRegistered("OpenFile"))
        //    this.RegisterClientScriptBlock("OpenFile", Script);

        string sFileName = CreateQuickContacReportForEMail();
        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);


    }
    protected void btnContactMail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContactMail.ID, btnContactMail.GetType().Name, "Click"); 

        string sFileName = CreateContacReportForEMail();
        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
    }
    // New Code

    private string CreateContacReportForEMail()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal total_incentives = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";


        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";

     

        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;

        int IsQty = 0;
        int IsSubtotal = 0;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsSubtotal = 2;

            }
        }

        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax); //esp.new_total_with_tax

            if (esp.is_KithenSheet != null)
            {
                is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            }
            if (esp.is_BathSheet != null)
            {
                is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            }
            if (esp.is_ShowerSheet != null)
            {
                is_ShowerSheet = Convert.ToBoolean(esp.is_ShowerSheet);
            }
            if (esp.is_TubSheet != null)
            {
                is_TubSheet = Convert.ToBoolean(esp.is_TubSheet);
            }
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                }
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();

            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }

        }


        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }
        string strCoverLetter = "";
        company_cover_letter objComcl = new company_cover_letter();
        if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(hdnClientId.Value));

            strCoverLetter = objComcl.cover_letter;
        }
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " "); 
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();
        // DataTable dtCabinet = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " )  AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value) + " " +
                        "  Union " +
                          " SELECT  * from disclaimers WHERE disclaimers.section_level IN (410001,420001) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(hdnClientId.Value);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();

        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }



        ReportDocument rptFile = new ReportDocument();

        string strReportPath = "";
        if (Convert.ToInt32(rdoSort.SelectedValue) == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptContactSection.rpt");

        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(hdnClientId.Value));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"] == "true")
        {
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
            else
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "C:/Faztimate/IICEM/IICEM/logouploads/");
            }
        }
        else
        {
            ContactAddress = ContactAddress.Replace("/IICEM", "http://localhost:7854/IICEM");
        }

        //Cover Page Shohid
        try
        {
            string sImagePath = Server.MapPath("Reports\\Common\\pdf_report") + @"\" + DateTime.Now.Ticks.ToString() + ".png";
            csCommonUtility.CreateContactAddressImage(ContactAddress, sImagePath);

            rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sImagePath + "'";
        }
        catch (Exception ex)
        {
            throw ex;
        }
        //ReportDocument subCabinetSheet = rptFile.OpenSubreport("rptCabinetSelection.rpt");
        //subCabinetSheet.SetDataSource(dtCabinet);
        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;
        string CustPortalUrl = ConfigurationManager.AppSettings["CustPortalUrl"];
        CustPortalUrl = CustPortalUrl + "?cid=" + hdnCustomerId.Value;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_CoverLettter", strCoverLetter);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);
        ht.Add("p_CustPortalUrl", CustPortalUrl);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        try
        {
            rptFile = (ReportDocument)Session[SessionInfo.Report_File];
            bool bParam = false;
            foreach (string strKey in Session.Keys)
            {
                if (strKey == SessionInfo.Report_Param)
                {
                    bParam = true;
                    break;
                }
            }
            if (bParam)
            {
                Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                ParameterValues param = new ParameterValues();
                ParameterDiscreteValue Val = new ParameterDiscreteValue();
                foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                {
                    if (htable.ContainsKey(obj.Name))
                    {
                        Val.Value = htable[obj.Name].ToString();
                        param.Add(Val);
                        obj.ApplyCurrentValues(param);
                    }
                }
            }
            //  CRViewer.ReportSource = rptFile;

            return exportReportForContract(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }

        return "";
    }


    protected string exportReportForContract(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string strFile = "";
        try
        {
            string contentType = "";
            strFile = "CONTRACT_" + DateTime.Now.Ticks;

            // string tempFileName = Request.PhysicalApplicationPath + "tmp\\ChangeOrder\\";
            string tempFileName = Server.MapPath("tmp\\Contract") + @"\" + strFile;
            switch (eft)
            {
                case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                    tempFileName = tempFileName + ".pdf";
                    contentType = "application/pdf";
                    break;
            }
            CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
            dfo.DiskFileName = tempFileName;
            CrystalDecisions.Shared.ExportOptions eo = selectedReport.ExportOptions;
            eo.DestinationOptions = dfo;
            eo.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
            eo.ExportFormatType = eft;
            selectedReport.Export();
            //hdnTempfile.Value = tempFileName;
            //TableRow row = new TableRow();
            //TableCell cell = new TableCell();
            //HyperLink hyp = new HyperLink();

            //cell.BorderWidth = 0;

            //hyp.Text = strEstName;
            //hyp.NavigateUrl = "tmp/Contract/" + strFile + ".pdf";
            //hyp.Target = "_blank";
            //cell.Controls.Add(hyp);
            //cell.HorizontalAlign = HorizontalAlign.Left;
            //row.Cells.Add(cell);
            //tdLink.Rows.Add(row);

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strFile;
    }


    private string CreateQuickContacReportForEMail()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal total_incentives = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";

        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";

        int IsQty = 0;
        int IsSubtotal = 0;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsSubtotal = 2;

            }
        }

        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;

        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax);
            if (esp.is_KithenSheet != null)
            {
                is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            }
            if (esp.is_BathSheet != null)
            {
                is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            }
            if (esp.is_ShowerSheet != null)
            {
                is_ShowerSheet = Convert.ToBoolean(esp.is_ShowerSheet);
            }
            if (esp.is_TubSheet != null)
            {
                is_TubSheet = Convert.ToBoolean(esp.is_TubSheet);
            }
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                //int num = 0;
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                    //num++;
                    //dt = dt.AddDays(num);
                    //if (dt.DayOfWeek == DayOfWeek.Saturday)
                    //{
                    //    dt = dt.AddDays(2);
                    //    num = num + 2;
                    //}
                    //else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    //{
                    //    dt = dt.AddDays(1);
                    //    num++;
                    //}


                }
                //dt = dt.AddDays(num);
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();

            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();
            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";
            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();

            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }
        }

        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.SingleOrDefault(fip => fip.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(hdnClientId.Value));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }


        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = oCom.address;
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id  " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value) + " order by item_id asc";

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) AND disclaimers.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";

            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;



            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }
        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (Convert.ToInt32(rdoSort.SelectedValue) == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\rptShortContact.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptShortContactSection.rpt");
        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);
        //ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        //subReport.SetDataSource(des_List);
        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);

        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);

        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);


        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);


        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        try
        {
            rptFile = (ReportDocument)Session[SessionInfo.Report_File];
            bool bParam = false;
            foreach (string strKey in Session.Keys)
            {
                if (strKey == SessionInfo.Report_Param)
                {
                    bParam = true;
                    break;
                }
            }
            if (bParam)
            {
                Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                ParameterValues param = new ParameterValues();
                ParameterDiscreteValue Val = new ParameterDiscreteValue();
                foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                {
                    if (htable.ContainsKey(obj.Name))
                    {
                        Val.Value = htable[obj.Name].ToString();
                        param.Add(Val);
                        obj.ApplyCurrentValues(param);
                    }
                }
            }


            return exportReportForContract(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }


        return "";
    }



    protected void btnPrintSummary_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintSummary.ID, btnPrintSummary.GetType().Name, "Click"); 
         int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        string strPsumm = string.Empty;
        if (rdoSort.SelectedValue == "1")
        {
            strPsumm = " SELECT  t1.location_id as MainID,lc.location_name  AS MainName,t3.ProjectCost, max(t1.sort_id) as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + t2.section_name  " +
                    " FROM pricing_details t2  " +
                    " WHERE t2.location_id = t1.location_id AND t2.estimate_id = " + nEstId + " AND t2.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND " +
                    " t2.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND t2.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000)  " +
                    "FOR XML PATH (''))  " +
                    "  ,1,2,'') AS SummaryName  " +
                    " FROM pricing_details t1   " +
                    " INNER JOIN location lc ON lc.location_id = t1.location_id  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,location_id from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id= " + nEstId + " and  " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000)  " +
                    " GROUP BY location_id ) AS t3 on t3.location_id = t1.location_id  " +
                    " WHERE  t1.estimate_id = " + nEstId + " AND t1.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and t1.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id = " + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1)  " +
                    " GROUP BY t1.location_id,lc.location_name,t3.ProjectCost; ";
        }
        else
        {
            strPsumm = " SELECT  t1.section_level as MainID,t1.section_name AS MainName,t3.ProjectCost,1 as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + lc.location_name  " +
                    " FROM pricing_details t2 " +
                    " INNER JOIN location lc ON lc.location_id = t2.location_id " +
                    " WHERE t2.section_level = t1.section_level AND t2.estimate_id =" + nEstId + " AND t2.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND " +
                     " t2.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND t2.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000)  " + 
                    " FOR XML PATH ('')) " +
                    " ,1,2,'') AS SummaryName " +
                    " FROM pricing_details t1  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,section_level from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + nEstId + " and " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)  " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000 ) " +
                    " GROUP BY section_level ) AS t3 on t3.section_level = t1.section_level " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and t1.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 )" +
                    " GROUP BY t1.section_level,t1.section_name,t3.ProjectCost;";


        }

        List<PMainSummary> PSummList = _db.ExecuteQuery<PMainSummary>(strPsumm, string.Empty).ToList();

        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = string.Empty;
        strReportPath = Server.MapPath(@"Reports\rpt\rptSummaryReportPricing.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(PSummList);


        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName =objCust.first_name1+" "+ objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        string deposit_value = "";
        string countertop_value = "";
        string start_job_value = "";
        string due_completion_value = "";
        string final_measure_value = "";
        string deliver_caninet_value = "";
        string substantial_value = "";
        string drywall_value = "";
        string flooring_value = "";
        string other_value = "";

        decimal deposit_amount = 0;
        decimal countertop_amount = 0;
        decimal start_job_amount = 0;
        decimal due_completion_amount = 0;
        decimal final_measure_amount = 0;
        decimal deliver_cabinet_amount = 0;
        decimal substantial_amount = 0;
        decimal drywall_amount = 0;
        decimal flooring_amount = 0;
        decimal other_amount = 0;


        if (txtDepositValue.Text.Trim() != "")
            deposit_value = txtDepositValue.Text.Replace("'", "^");
        if (txtCountertopValue.Text.Trim() != "")
            countertop_value = txtCountertopValue.Text.Replace("'", "^");
        if (txtStartOfJobValue.Text.Trim() != "")
            start_job_value = txtStartOfJobValue.Text.Replace("'", "^");
        if (txtDueCompletionValue.Text.Trim() != "")
            due_completion_value = txtDueCompletionValue.Text.Replace("'", "^");
        if (txtMeasureValue.Text.Trim() != "")
            final_measure_value = txtMeasureValue.Text.Replace("'", "^");
        if (txtDeliveryValue.Text.Trim() != "")
           deliver_caninet_value = txtDeliveryValue.Text.Replace("'", "^");
        if (txtSubstantialValue.Text.Trim() != "")
            substantial_value = txtSubstantialValue.Text.Replace("'", "^");
        if (txtStartofDrywallValue.Text.Trim() != "")
            drywall_value = txtStartofDrywallValue.Text.Replace("'", "^");
        if (txtStartofFlooringValue.Text.Trim() != "")
            flooring_value = txtStartofFlooringValue.Text.Replace("'", "^");

        other_value = txtOthers.Text.Replace("&nbsp;", "");

       
        if (txtnDeposit.Text.Trim() != "")
            deposit_amount = Convert.ToDecimal(txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            deposit_amount = 0;
    
        if (txtnCountertop.Text != "")
           countertop_amount = Convert.ToDecimal(txtnCountertop.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           countertop_amount = 0;
      
        if (txtnJob.Text.Trim() != "")
           start_job_amount = Convert.ToDecimal(txtnJob.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           start_job_amount = 0;
       
        if (txtnBalance.Text.Trim() != "")
           due_completion_amount = Convert.ToDecimal(txtnBalance.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           due_completion_amount = 0;
       
        if (txtnMeasure.Text.Trim() != "")
           final_measure_amount = Convert.ToDecimal(txtnMeasure.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           final_measure_amount = 0;
      
        if (txtnDelivery.Text.Trim() != "")
           deliver_cabinet_amount = Convert.ToDecimal(txtnDelivery.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           deliver_cabinet_amount = 0;
       
        if (txtnSubstantial.Text.Trim() != "")
           substantial_amount = Convert.ToDecimal(txtnSubstantial.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           substantial_amount = 0;

       
        if (txtnDrywall.Text.Trim() != "")
           drywall_amount = Convert.ToDecimal(txtnDrywall.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           drywall_amount = 0;

       
        if (txtnFlooring.Text.Trim() != "")
           flooring_amount = Convert.ToDecimal(txtnFlooring.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           flooring_amount = 0;

       
        if (txtnOthers.Text.Trim() != "")
           other_amount = Convert.ToDecimal(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
           other_amount = 0;
        

        decimal TotalwithTax = 0;
        try
        {

            if (Convert.ToDecimal(lblNewTotalWithTax.Text.Replace("$", "")) == 0)
            {
                TotalwithTax = Convert.ToDecimal(lblTotalWithTax.Text.Replace("$", ""));
            }
            else
            {
                TotalwithTax = Convert.ToDecimal(lblNewTotalWithTax.Text.Replace("$", ""));
 
            }

        }
        catch 
        {
        }
        string strSummaryNote = cus_est.estimate_comments;
        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_SalesRep", lblSalesPerson.Text);
        ht.Add("p_SummaryNote", strSummaryNote);
        ht.Add("p_TotalwithTax", TotalwithTax);

        ht.Add("p_DepositValue", deposit_value);
        ht.Add("p_DepositAmont", deposit_amount);
        ht.Add("p_DepositDate", txtDepositDate.Text);

        ht.Add("p_CountertopValue", countertop_value);
        ht.Add("p_countertop_amount", countertop_amount);
        ht.Add("p_CountertopDate", txtCountertopDate.Text);

        ht.Add("p_StartOfJobValue", start_job_value);
        ht.Add("p_start_job_amount", start_job_amount);
        ht.Add("p_StartOfJobDate", txtStartOfJobDate.Text);

        ht.Add("p_StartofFlooringValue", flooring_value);
        ht.Add("p_flooring_amount", flooring_amount);
        ht.Add("p_StartofFlooringDate", txtStartofFlooringDate.Text);

        ht.Add("p_StartofDrywallValue", drywall_value);
        ht.Add("p_drywall_amount", drywall_amount);
        ht.Add("p_StartofDrywallDate", txtStartofDrywallDate.Text);

        ht.Add("p_MeasureValue", final_measure_value);
        ht.Add("p_final_measure_amount", final_measure_amount);
        ht.Add("p_MeasureDate", txtMeasureDate.Text);

        ht.Add("p_DeliveryValue", deliver_caninet_value);
        ht.Add("p_deliver_cabinet_amount", deliver_cabinet_amount);
        ht.Add("p_DeliveryDate", txtDeliveryDate.Text);

        ht.Add("p_SubstantialValue", substantial_value);
        ht.Add("p_substantial_amount", substantial_amount);
        ht.Add("p_SubstantialDate", txtSubstantialDate.Text);

        ht.Add("p_other_value", other_value);
        ht.Add("p_other_amount", other_amount);
        ht.Add("p_OtherDate", txtOtherDate.Text);

        ht.Add("p_DueCompletionValue", due_completion_value);
        ht.Add("p_due_completion_amount", due_completion_amount);
        ht.Add("p_DueCompletionDate", txtDueCompletionDate.Text);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    
    }
    protected void btnEmailSummary_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnEmailSummary.ID, btnEmailSummary.GetType().Name, "Click"); 
        string sFileName = CreateSummaryReportForEMail();
        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
    }
    private string CreateSummaryReportForEMail()
    {
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        string strPsumm = string.Empty;
        if (rdoSort.SelectedValue == "1")
        {
            strPsumm = " SELECT  t1.location_id as MainID,lc.location_name  AS MainName,t3.ProjectCost, max(t1.sort_id) as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + t2.section_name  " +
                    " FROM pricing_details t2  " +
                    " WHERE t2.location_id = t1.location_id AND t2.estimate_id = " + nEstId + " AND t2.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND  " +
                    " t2.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND t2.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000)  " + 
                    "FOR XML PATH (''))  " +
                    "  ,1,2,'') AS SummaryName  " +
                    " FROM pricing_details t1   " +
                    " INNER JOIN location lc ON lc.location_id = t1.location_id  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,location_id from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id= " + nEstId + " and  " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000 )  " +
                    " GROUP BY location_id ) AS t3 on t3.location_id = t1.location_id  " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and t1.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id = " + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1)  " +
                    " GROUP BY t1.location_id,lc.location_name,t3.ProjectCost; ";
        }
        else
        {
            strPsumm = " SELECT  t1.section_level as MainID,t1.section_name AS MainName,t3.ProjectCost,1 as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + lc.location_name  " +
                    " FROM pricing_details t2 " +
                    " INNER JOIN location lc ON lc.location_id = t2.location_id " +
                    " WHERE t2.section_level = t1.section_level AND t2.estimate_id =" + nEstId + " AND t2.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND " +
                    " t2.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91)   " +
                    " AND t2.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000)  " + 
                    " FOR XML PATH ('')) " +
                    " ,1,2,'') AS SummaryName " +
                    " FROM pricing_details t1  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,section_level from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + nEstId + " and " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 AND location_id != 91 )  " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 AND section_id != 44000 ) " +
                    " GROUP BY section_level ) AS t3 on t3.section_level = t1.section_level " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and t1.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 )" +
                    " GROUP BY t1.section_level,t1.section_name,t3.ProjectCost;";


        }

        List<PMainSummary> PSummList = _db.ExecuteQuery<PMainSummary>(strPsumm, string.Empty).ToList();

        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = string.Empty;
        strReportPath = Server.MapPath(@"Reports\rpt\rptSummaryReportPricing.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(PSummList);


        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        string deposit_value = "";
        string countertop_value = "";
        string start_job_value = "";
        string due_completion_value = "";
        string final_measure_value = "";
        string deliver_caninet_value = "";
        string substantial_value = "";
        string drywall_value = "";
        string flooring_value = "";
        string other_value = "";

        decimal deposit_amount = 0;
        decimal countertop_amount = 0;
        decimal start_job_amount = 0;
        decimal due_completion_amount = 0;
        decimal final_measure_amount = 0;
        decimal deliver_cabinet_amount = 0;
        decimal substantial_amount = 0;
        decimal drywall_amount = 0;
        decimal flooring_amount = 0;
        decimal other_amount = 0;


        if (txtDepositValue.Text.Trim() != "")
            deposit_value = txtDepositValue.Text.Replace("'", "^");
        if (txtCountertopValue.Text.Trim() != "")
            countertop_value = txtCountertopValue.Text.Replace("'", "^");
        if (txtStartOfJobValue.Text.Trim() != "")
            start_job_value = txtStartOfJobValue.Text.Replace("'", "^");
        if (txtDueCompletionValue.Text.Trim() != "")
            due_completion_value = txtDueCompletionValue.Text.Replace("'", "^");
        if (txtMeasureValue.Text.Trim() != "")
            final_measure_value = txtMeasureValue.Text.Replace("'", "^");
        if (txtDeliveryValue.Text.Trim() != "")
            deliver_caninet_value = txtDeliveryValue.Text.Replace("'", "^");
        if (txtSubstantialValue.Text.Trim() != "")
            substantial_value = txtSubstantialValue.Text.Replace("'", "^");
        if (txtStartofDrywallValue.Text.Trim() != "")
            drywall_value = txtStartofDrywallValue.Text.Replace("'", "^");
        if (txtStartofFlooringValue.Text.Trim() != "")
            flooring_value = txtStartofFlooringValue.Text.Replace("'", "^");

        other_value = txtOthers.Text.Replace("&nbsp;", "");


        if (txtnDeposit.Text.Trim() != "")
            deposit_amount = Convert.ToDecimal(txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            deposit_amount = 0;

        if (txtnCountertop.Text != "")
            countertop_amount = Convert.ToDecimal(txtnCountertop.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            countertop_amount = 0;

        if (txtnJob.Text.Trim() != "")
            start_job_amount = Convert.ToDecimal(txtnJob.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            start_job_amount = 0;

        if (txtnBalance.Text.Trim() != "")
            due_completion_amount = Convert.ToDecimal(txtnBalance.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            due_completion_amount = 0;

        if (txtnMeasure.Text.Trim() != "")
            final_measure_amount = Convert.ToDecimal(txtnMeasure.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            final_measure_amount = 0;

        if (txtnDelivery.Text.Trim() != "")
            deliver_cabinet_amount = Convert.ToDecimal(txtnDelivery.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            deliver_cabinet_amount = 0;

        if (txtnSubstantial.Text.Trim() != "")
            substantial_amount = Convert.ToDecimal(txtnSubstantial.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            substantial_amount = 0;


        if (txtnDrywall.Text.Trim() != "")
            drywall_amount = Convert.ToDecimal(txtnDrywall.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            drywall_amount = 0;


        if (txtnFlooring.Text.Trim() != "")
            flooring_amount = Convert.ToDecimal(txtnFlooring.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            flooring_amount = 0;


        if (txtnOthers.Text.Trim() != "")
            other_amount = Convert.ToDecimal(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            other_amount = 0;


        decimal TotalwithTax = 0;
        try
        {

            if (Convert.ToDecimal(lblNewTotalWithTax.Text.Replace("$", "")) == 0)
            {
                TotalwithTax = Convert.ToDecimal(lblTotalWithTax.Text.Replace("$", ""));
            }
            else
            {
                TotalwithTax = Convert.ToDecimal(lblNewTotalWithTax.Text.Replace("$", ""));

            }

        }
        catch
        {
        }
        string strSummaryNote = cus_est.estimate_comments;
        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_SalesRep", lblSalesPerson.Text);
        ht.Add("p_SummaryNote", strSummaryNote);
        ht.Add("p_TotalwithTax", TotalwithTax);

        ht.Add("p_DepositValue", deposit_value);
        ht.Add("p_DepositAmont", deposit_amount);
        ht.Add("p_DepositDate", txtDepositDate.Text);

        ht.Add("p_CountertopValue", countertop_value);
        ht.Add("p_countertop_amount", countertop_amount);
        ht.Add("p_CountertopDate", txtCountertopDate.Text);

        ht.Add("p_StartOfJobValue", start_job_value);
        ht.Add("p_start_job_amount", start_job_amount);
        ht.Add("p_StartOfJobDate", txtStartOfJobDate.Text);

        ht.Add("p_StartofFlooringValue", flooring_value);
        ht.Add("p_flooring_amount", flooring_amount);
        ht.Add("p_StartofFlooringDate", txtStartofFlooringDate.Text);

        ht.Add("p_StartofDrywallValue", drywall_value);
        ht.Add("p_drywall_amount", drywall_amount);
        ht.Add("p_StartofDrywallDate", txtStartofDrywallDate.Text);

        ht.Add("p_MeasureValue", final_measure_value);
        ht.Add("p_final_measure_amount", final_measure_amount);
        ht.Add("p_MeasureDate", txtMeasureDate.Text);

        ht.Add("p_DeliveryValue", deliver_caninet_value);
        ht.Add("p_deliver_cabinet_amount", deliver_cabinet_amount);
        ht.Add("p_DeliveryDate", txtDeliveryDate.Text);

        ht.Add("p_SubstantialValue", substantial_value);
        ht.Add("p_substantial_amount", substantial_amount);
        ht.Add("p_SubstantialDate", txtSubstantialDate.Text);

        ht.Add("p_other_value", other_value);
        ht.Add("p_other_amount", other_amount);
        ht.Add("p_OtherDate", txtOtherDate.Text);

        ht.Add("p_DueCompletionValue", due_completion_value);
        ht.Add("p_due_completion_amount", due_completion_amount);
        ht.Add("p_DueCompletionDate", txtDueCompletionDate.Text);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        try
        {
            rptFile = (ReportDocument)Session[SessionInfo.Report_File];
            bool bParam = false;
            foreach (string strKey in Session.Keys)
            {
                if (strKey == SessionInfo.Report_Param)
                {
                    bParam = true;
                    break;
                }
            }
            if (bParam)
            {
                Hashtable htable = (Hashtable)Session[SessionInfo.Report_Param];
                ParameterValues param = new ParameterValues();
                ParameterDiscreteValue Val = new ParameterDiscreteValue();
                foreach (ParameterFieldDefinition obj in rptFile.DataDefinition.ParameterFields)
                {
                    if (htable.ContainsKey(obj.Name))
                    {
                        Val.Value = htable[obj.Name].ToString();
                        param.Add(Val);
                        obj.ApplyCurrentValues(param);
                    }
                }
            }
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

            return exportReportForSummary(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }


        return "";
    }
    protected string exportReportForSummary(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string strFile = "";
        try
        {
            string contentType = "";
            //strFile = "SECTION_" + DateTime.Now.Ticks;
            strFile = hdnLastName.Value + "-"+"EstimateSummary"+"_" + DateTime.Now.Ticks;

            // string tempFileName = Request.PhysicalApplicationPath + "tmp\\ChangeOrder\\";
            string tempFileName = Server.MapPath("tmp\\Contract") + @"\" + strFile;
            switch (eft)
            {
                case CrystalDecisions.Shared.ExportFormatType.PortableDocFormat:
                    tempFileName = tempFileName + ".pdf";
                    contentType = "application/pdf";
                    break;
            }
            CrystalDecisions.Shared.DiskFileDestinationOptions dfo = new CrystalDecisions.Shared.DiskFileDestinationOptions();
            dfo.DiskFileName = tempFileName;
            CrystalDecisions.Shared.ExportOptions eo = selectedReport.ExportOptions;
            eo.DestinationOptions = dfo;
            eo.ExportDestinationType = CrystalDecisions.Shared.ExportDestinationType.DiskFile;
            eo.ExportFormatType = eft;
            selectedReport.Export();

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strFile;
    }

}
