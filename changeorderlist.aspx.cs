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

public partial class changeorderlist : System.Web.UI.Page
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
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            if (Page.User.IsInRole("admin023") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();
            int nestd = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nestd.ToString();

           
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                if (cust != null)
                {
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                    string strAddress = "";
                    strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                    lblAddress.Text = strAddress;
                    lblEmail.Text = cust.email;
                    lblPhone.Text = cust.phone;
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                    //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                    hdnClientId.Value = cust.client_id.ToString();
                }
                
            }
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                lblEstimateName.Text = cus_est.estimate_name;
                if ((cus_est.job_number ?? "").Length > 0)
                {
                    //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                    else
                        lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                }
            }
            GetChangeOrders(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));


        }
    }

    private void GetChangeOrders(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        //int nSalePersonId = Convert.ToInt32(hdnSalesPersonId.Value);
        var item = from co in _db.changeorder_estimates
                   where co.customer_id == nCustId && co.estimate_id == nEstId 
                   orderby co.changeorder_name ascending
                   select co;
        grdChangeOrders.DataSource = item;
        grdChangeOrders.DataBind();
    }
    protected void grdChangeOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        int nSalePersonId = Convert.ToInt32(hdnSalesPersonId.Value);


        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncoeid = Convert.ToInt32(grdChangeOrders.DataKeys[e.Row.RowIndex].Value.ToString());

            e.Row.Cells[6].ToolTip = ncoeid.ToString();

            LinkButton lnkCO = (LinkButton)e.Row.FindControl("lnkCO");
            lnkCO.CommandArgument = ncoeid.ToString();

            //LinkButton lnkHTML = (LinkButton)e.Row.FindControl("lnkHTML");
            //lnkHTML.CommandArgument = ncoeid.ToString();

            changeorder_estimate objCOE = new changeorder_estimate();
            objCOE = _db.changeorder_estimates.Single(coe => coe.chage_order_id == ncoeid && coe.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && coe.customer_id == Convert.ToInt32(hdnCustomerId.Value));

            HyperLink hypTitle = (HyperLink)e.Row.FindControl("hypTitle");
            hypTitle.Text = objCOE.changeorder_name;
            hypTitle.NavigateUrl = "change_order_worksheet.aspx?coestid=" + ncoeid + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value;

            //HyperLink hypHTMLCO = (HyperLink)e.Row.FindControl("hypHTMLCO");
            //hypHTMLCO.NavigateUrl = "htmlco.aspx?coestid=" + ncoeid + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value;
            
            //e.Row.Cells[5].Text = obj.first_name + " " + obj.last_name;
            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            e.Row.Cells[5].Text = sp.first_name + " " + sp.last_name;

            if (e.Row.Cells[1].Text.Trim() != "")
            {
                int nStatusId = Convert.ToInt32(e.Row.Cells[1].Text.Trim());
                if (nStatusId == 1)
                    e.Row.Cells[1].Text = "Draft";
                else if (nStatusId == 2)
                    e.Row.Cells[1].Text = "Pending";
                else if (nStatusId == 3)
                    e.Row.Cells[1].Text = "Executed";
                else if (nStatusId == 4)
                    e.Row.Cells[1].Text = "Declined";
            }
            if (e.Row.Cells[2].Text.Trim() != "")
            {
                int nChangeOrderType = Convert.ToInt32(e.Row.Cells[2].Text.Trim());
                if (nChangeOrderType == 1)
                    e.Row.Cells[2].Text = "Change Order";
                else if (nChangeOrderType == 2)
                    e.Row.Cells[2].Text = "Clarification";
                else if (nChangeOrderType == 3)
                    e.Row.Cells[2].Text = "Internal Use Only";
            }
        }
    }
    protected void GetReport(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        int ncoId = Convert.ToInt32(lnk.CommandArgument);

        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes,quantity,measure_unit FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id AND change_order_pricing_list.client_id=location.client_id " +
                    " WHERE chage_order_id=" + ncoId + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();
        
        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == ncoId);

        decimal COAmount = 0;
        decimal CoTax = 0;
        decimal CoTaxRate = Convert.ToDecimal(cho.tax);
        decimal dEconCost = 0;
        var Coresult = (from chpl in _db.change_order_pricing_lists
                        where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == 1 && chpl.chage_order_id == ncoId
                        select chpl.EconomicsCost);
        int cn = Coresult.Count();
        if (Coresult != null && cn > 0)
            dEconCost = Coresult.Sum();
        else
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Change order items not exists.");
            return;
        }


        if (CoTaxRate > 0)
        {
            CoTax = dEconCost * (CoTaxRate / 100);
            COAmount = dEconCost + CoTax;

        }
        else
        {
            COAmount = dEconCost + CoTax;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
        if (oCom != null)
        {
            if (oCom.ChangeQtyView == 1)
                strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order.rpt");
            else
                strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order2.rpt");
        }
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(rList);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));

        string strSalesPerson = "( " + sp.first_name + " " + sp.last_name + " )";
        string strCustomerName = lblCustomerName.Text;
        string strpayment_terms = "";
        if (cho.payment_terms == "Other")
        {
            strpayment_terms = cho.other_terms.ToString();
        }
        else
        {
            strpayment_terms = cho.payment_terms.ToString();
        }

        
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComFax = oCom.fax;
        string strComEmail = oCom.email;
        string strComAddress = oCom.address;
        string strComCity = oCom.city;
        string strComState = oCom.state;
        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        // CO Payment Term

        string strUponSignValue = string.Empty;
        string strUponCompletionValue = string.Empty;
        string strBalanceDueValue = string.Empty;
        string strOtherValue = string.Empty;

        string strUponSignDate = string.Empty;
        string strUponCompletionDate = string.Empty;
        string strBalanceDueDate = string.Empty;
        string strOtherDate = string.Empty;

        decimal dUponSignAmount = 0;
        decimal dUponCompletionAmount = 0;
        decimal dBalanceDueAmount = 0;
        decimal dOtherAmount = 0;

        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoId && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoId && est_p.client_id == Convert.ToInt32(hdnClientId.Value));

            if (objPayTerm.UponSign_value != null)
            {
                strUponSignValue = objPayTerm.UponSign_value.ToString().Replace("^", "'");
            }
            if (objPayTerm.UponCompletion_value != null)
            {
                strUponCompletionValue = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
            }
            if (objPayTerm.BalanceDue_value != null)
            {
                strBalanceDueValue = objPayTerm.BalanceDue_value.Replace("^", "'");
            }
            if (objPayTerm.other_value != null)
            {
                strOtherValue = objPayTerm.other_value.Replace("^", "'");
            }

            dUponSignAmount = Convert.ToDecimal(objPayTerm.UponSign_amount);

            dUponCompletionAmount = Convert.ToDecimal(objPayTerm.UponCompletion_amount);

            dBalanceDueAmount = Convert.ToDecimal(objPayTerm.BalanceDue_amount);

            dOtherAmount = Convert.ToDecimal(objPayTerm.other_amount);

            strUponSignDate = objPayTerm.UponSign_date;
            strUponCompletionDate = objPayTerm.UponCompletion_date;
            strBalanceDueDate = objPayTerm.BalanceDue_date;
            strOtherDate = objPayTerm.other_date;
        }
        else
        {
            strUponSignValue = "Payment due upon signing";
            dUponSignAmount = COAmount;

        }

        Hashtable ht = new Hashtable();

        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_changeorder_name", cho.changeorder_name);
        ht.Add("p_change_order_status_id", cho.change_order_status_id);
        ht.Add("P_change_order_type_id", cho.change_order_type_id);
        ht.Add("p_comments", cho.comments);
        ht.Add("p_payment_terms", strpayment_terms);
        ht.Add("p_is_total", cho.is_total);
        ht.Add("p_is_tax", cho.is_tax);
        ht.Add("p_tax", cho.tax);
        ht.Add("p_total_payment_due", cho.total_payment_due);
        ht.Add("p_changeorder_date", cho.changeorder_date);
        ht.Add("p_notes1", cho.notes1);
        ht.Add("p_ExecuteDate", cho.last_updated_date);
        ht.Add("p_EstimateName", lblEstimateName.Text);

        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyFax", strComFax);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_CompanyWeb", strComWeb);

        ht.Add("p_UponSignValue", strUponSignValue);
        ht.Add("p_UponCompletionValue", strUponCompletionValue);
        ht.Add("p_BalanceDueValue", strBalanceDueValue);
        ht.Add("p_OtherValue", strOtherValue);

        ht.Add("p_UponSignDate", strUponSignDate);
        ht.Add("p_UponCompletionDate", strUponCompletionDate);
        ht.Add("p_BalanceDueDate", strBalanceDueDate);
        ht.Add("p_OtherDate", strOtherDate);


        ht.Add("p_UponSignAmount", dUponSignAmount);
        ht.Add("p_UponCompletionAmount", dUponCompletionAmount);
        ht.Add("p_BalanceDueAmount", dBalanceDueAmount);
        ht.Add("p_OtherAmount", dOtherAmount);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }
    protected void GetHTML(object sender, EventArgs e)
    {
        //LinkButton lnk = (LinkButton)sender;
        //int ncoId = Convert.ToInt32(lnk.CommandArgument);
        //string url = "htmlco.aspx?coestid=" + ncoId + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value;
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('" + url + "','MyWindow','left=50,top=50,width=930,status=1,toolbar=0,resizable=0,scrollbars=1');", true);

        LinkButton lnk = (LinkButton)sender;
        int ncoId = Convert.ToInt32(lnk.CommandArgument);

        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id AND change_order_pricing_list.client_id=location.client_id " +
                    " WHERE chage_order_id=" + ncoId + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();
        
        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == ncoId);

        decimal COAmount = 0;
        decimal CoTax = 0;
        decimal CoTaxRate = Convert.ToDecimal(cho.tax);
        decimal dEconCost = 0;
        var Coresult = (from chpl in _db.change_order_pricing_lists
                        where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == 1 && chpl.chage_order_id == ncoId
                        select chpl.EconomicsCost);
        int cn = Coresult.Count();
        if (Coresult != null && cn > 0)
            dEconCost = Coresult.Sum();



        if (CoTaxRate > 0)
        {
            CoTax = dEconCost * (CoTaxRate / 100);
            COAmount = dEconCost + CoTax;

        }
        else
        {
            COAmount = dEconCost + CoTax;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(rList);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));

        string strSalesPerson = "( " + sp.first_name + " " + sp.last_name + " )";
        string strCustomerName = lblCustomerName.Text;
        string strpayment_terms = "";
        if (cho.payment_terms == "Other")
        {
            strpayment_terms = cho.other_terms.ToString();
        }
        else
        {
            strpayment_terms = cho.payment_terms.ToString();
        }

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComFax = oCom.fax;
        string strComEmail = oCom.email;
        string strComAddress = oCom.address;
        string strComCity = oCom.city;
        string strComState = oCom.state;
        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;
        // CO Payment Term

        string strUponSignValue = string.Empty;
        string strUponCompletionValue = string.Empty;
        string strBalanceDueValue = string.Empty;
        string strOtherValue = string.Empty;

        string strUponSignDate = string.Empty;
        string strUponCompletionDate = string.Empty;
        string strBalanceDueDate = string.Empty;
        string strOtherDate = string.Empty;

        decimal dUponSignAmount = 0;
        decimal dUponCompletionAmount = 0;
        decimal dBalanceDueAmount = 0;
        decimal dOtherAmount = 0;

        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoId && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoId && est_p.client_id == Convert.ToInt32(hdnClientId.Value));

            if (objPayTerm.UponSign_value != null)
            {
                strUponSignValue = objPayTerm.UponSign_value.ToString().Replace("^", "'");
            }
            if (objPayTerm.UponCompletion_value != null)
            {
                strUponCompletionValue = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
            }
            if (objPayTerm.BalanceDue_value != null)
            {
                strBalanceDueValue = objPayTerm.BalanceDue_value.Replace("^", "'");
            }
            if (objPayTerm.other_value != null)
            {
                strOtherValue = objPayTerm.other_value.Replace("^", "'");
            }

            dUponSignAmount = Convert.ToDecimal(objPayTerm.UponSign_amount);

            dUponCompletionAmount = Convert.ToDecimal(objPayTerm.UponCompletion_amount);

            dBalanceDueAmount = Convert.ToDecimal(objPayTerm.BalanceDue_amount);

            dOtherAmount = Convert.ToDecimal(objPayTerm.other_amount);

            strUponSignDate = objPayTerm.UponSign_date;
            strUponCompletionDate = objPayTerm.UponCompletion_date;
            strBalanceDueDate = objPayTerm.BalanceDue_date;
            strOtherDate = objPayTerm.other_date;
        }
        else
        {
            strUponSignValue = "Payment due upon signing";
            dUponSignAmount = COAmount;

        }

        Hashtable ht = new Hashtable();

        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_changeorder_name", cho.changeorder_name);
        ht.Add("p_change_order_status_id", cho.change_order_status_id);
        ht.Add("P_change_order_type_id", cho.change_order_type_id);
        ht.Add("p_comments", cho.comments);
        ht.Add("p_payment_terms", strpayment_terms);
        ht.Add("p_is_total", cho.is_total);
        ht.Add("p_is_tax", cho.is_tax);
        ht.Add("p_tax", cho.tax);
        ht.Add("p_total_payment_due", cho.total_payment_due);
        ht.Add("p_changeorder_date", cho.changeorder_date);
        ht.Add("p_notes1", cho.notes1);
        ht.Add("p_ExecuteDate", cho.last_updated_date);
        ht.Add("p_EstimateName", lblEstimateName.Text);

        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyFax", strComFax);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_CompanyWeb", strComWeb);

        ht.Add("p_UponSignValue", strUponSignValue);
        ht.Add("p_UponCompletionValue", strUponCompletionValue);
        ht.Add("p_BalanceDueValue", strBalanceDueValue);
        ht.Add("p_OtherValue", strOtherValue);

        ht.Add("p_UponSignDate", strUponSignDate);
        ht.Add("p_UponCompletionDate", strUponCompletionDate);
        ht.Add("p_BalanceDueDate", strBalanceDueDate);
        ht.Add("p_OtherDate", strOtherDate);


        ht.Add("p_UponSignAmount", dUponSignAmount);
        ht.Add("p_UponCompletionAmount", dUponCompletionAmount);
        ht.Add("p_BalanceDueAmount", dBalanceDueAmount);
        ht.Add("p_OtherAmount", dOtherAmount);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportDocumentViewer_CO.aspx');", true);

    }
    protected void btnCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
    protected void btnSoldEstimate_Click(object sender, EventArgs e)
    {
        Response.Redirect("sold_estimate.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value);
    }

}
