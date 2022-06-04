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
using CrystalDecisions.Shared;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;



public partial class change_order_worksheet : System.Web.UI.Page
{
    private double subtotal = 0.0;
    //private double grandtotal = 0.0;
    string strDetails = "";
    string sFileName = "test";
    private double subtotal_diect = 0.0;
    //private double grandtotal_direct = 0.0;
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

            if (Page.User.IsInRole("admin019") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            //foreach (System.Web.UI.WebControls.ListItem li in rdoconfirm.Items)
            //{
            //    if (Convert.ToInt32(li.Value) == 1)
            //    {
            //        li.Attributes.Add("onClick", "DisplayWindow();");

            //    }
            //}


           


            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            int nChEstid = Convert.ToInt32(Request.QueryString.Get("coestid"));
            hdnChEstId.Value = nChEstid.ToString();
            rdoconfirm.Attributes.Add("onchange", "return confirmOperation();");

            DataClassesDataContext _db = new DataClassesDataContext();
            
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                GetTools();
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
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();
                    sales_person sp = new sales_person();
                    sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    lblSalesRep.Text = sp.first_name + " " + sp.last_name;

                    //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                    hdnClientId.Value = cust.client_id.ToString();
                }

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "")
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                }
                // Get Change Order Info
                if (Convert.ToInt32(hdnChEstId.Value) > 0)
                {
                    changeorder_estimate cho = new changeorder_estimate();
                    cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

                    lblChangeOrderName.Text = cho.changeorder_name;
                    lblExistingChangeOrderName.Text = cho.changeorder_name;
                    ddlStatus.SelectedValue = cho.change_order_status_id.ToString();
                    ddlChangeOrderType.SelectedValue = cho.change_order_type_id.ToString();
                    chkChangeOrderQtyshow.Checked = Convert.ToBoolean(cho.IsChangeOrderQtyViewByCust);
                    txtComments.Text = cho.comments;
                    if (cho.payment_terms != "")
                    {
                        try
                        {
                            //Convert.ToInt32(cho.payment_terms);
                            ddlTerms.Items.FindByText(cho.payment_terms.ToString()).Selected = true;
                            //ddlTerms.SelectedItem.Text = cho.payment_terms.ToString();
                        }
                        catch 
                        {
                        }
                    }
                    if (cho.payment_terms == "Other")
                    {
                        txtOtherTerms.Text = cho.other_terms.ToString();
                        lblOther.Visible = true;
                        txtOtherTerms.Visible = true;
                    }
                    rdoList.SelectedValue = cho.is_total.ToString();
                    ChkIsTax.Checked = Convert.ToBoolean(cho.is_tax);
                    txtTaxPer.Text = cho.tax.ToString();
                    txtTaxPer.ToolTip = cho.tax.ToString();
                    if (cho.is_cutomer_viewable != null)
                    {
                        rdoconfirm.SelectedValue = cho.is_cutomer_viewable.ToString();
                        if (Convert.ToInt32(rdoconfirm.SelectedValue) == 1)
                        {
                            rdoconfirm.Enabled = false;

                        }
                        else
                        {
                            lnkResend.Visible = false;
                        }
                    }
                    else
                    {
                        lnkResend.Visible = false;
                    }
                    if (Convert.ToInt32(ddlChangeOrderType.SelectedValue) == 3)
                    {
                        rdoconfirm.SelectedValue = "2";
                        tblReview.Visible = false;
                        rdoconfirm.Enabled = false;
                        lnkResend.Visible = false;
                    }
                    decimal taxrate = 0;
                    if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
                    {
                        taxrate = 0;
                    }
                    else
                    {
                        estimate_payment esp = new estimate_payment();
                        esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
                        taxrate = Convert.ToDecimal(esp.tax_rate);
                    }
                    if (cho.tax == 0)
                    {
                        txtTaxPer.Text = taxrate.ToString();
                        txtTaxPer.ToolTip = taxrate.ToString();

                    }
                    //else
                    //{
                    //    pnltax.Visible = true;
                    //}
                    if (cho.total_payment_due != "")
                        ddlTotalHeader.SelectedItem.Text = cho.total_payment_due.ToString();
                    txtChangeOrderDate.Text = cho.changeorder_date.ToString();
                    txtNotes1.Text = cho.notes1.ToString();

                    // Executed or decliened Change Order Worksheet
                    if (ddlStatus.SelectedValue == "3" || ddlStatus.SelectedValue == "4")
                    {
                        Response.Redirect("change_order_worksheet_readonly.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                    }
                   

                }

                BindSelectedItemGrid();
                BindSelectedItemGrid_Direct();

                if (grdGrouping.Rows.Count == 0)
                {
                    lblRetailPricingHeader.Visible = false;
                }
                else
                {
                    lblRetailPricingHeader.Visible = true;
                }
                if (grdGroupingDirect.Rows.Count == 0)
                {
                    lblDirectPricingHeader.Visible = false;
                }
                else
                {
                    lblDirectPricingHeader.Visible = true;
                }

                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {
                    rdoSort.Visible = false;

                    if (grdGrouping.Rows.Count == 0)
                    {
                        lblRetailPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblRetailPricingHeader.Visible = true;
                    }
                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }
                else
                {
                    rdoSort.Visible = true;
                    if (grdGrouping.Rows.Count == 0)
                    {
                        lblRetailPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblRetailPricingHeader.Visible = true;
                    }
                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }

            }

            company_profile com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value));
            if (com != null)
            {
                hdnChangeOrderView.Value = com.ChangeQtyView.ToString();
            }


            Main_calculation();
            customer_review();

            // CO Payment Term
            //if (_db.Co_PaymentTerms.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
            if (!_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
            {
                hdnChPaymentId.Value = "0";
                if (ddlTerms.SelectedItem.Text == "Other")
                {
                    txtOthers.Text = txtOtherTerms.Text;
                    txtnOthers.Text = lblGtotal.Text;

                    txtpOthers.Text = "100";
                    txtpUponSign.Text = "";
                    txtpBalanceDue.Text = "";
                    txtpUponCompletion.Text = "";

                }
                else if (ddlTerms.SelectedItem.Text == "Payment due upon signing of this Change Order")
                {
                    txtnUponSign.Text = lblGtotal.Text;

                    txtpOthers.Text = "";
                    txtpUponSign.Text = "100";
                    txtpBalanceDue.Text = "";
                    txtpUponCompletion.Text = "";

                }
                else if (ddlTerms.SelectedItem.Text == "Balance due at Completion")
                {
                    txtpOthers.Text = "";
                    txtpUponSign.Text = "";
                    txtpBalanceDue.Text = "100";
                    txtpUponCompletion.Text = "";
                    txtnBalanceDue.Text = lblGtotal.Text;

                }
                else if (ddlTerms.SelectedItem.Text == "Payment due upon completion of work described on this Change Order")
                {
                    txtpOthers.Text = "";
                    txtpUponSign.Text = "";
                    txtpBalanceDue.Text = "";
                    txtpUponCompletion.Text = "100";
                    txtnUponCompletion.Text = lblGtotal.Text;

                }
                else
                {
                    txtUponSignValue.Text = ddlTerms.SelectedItem.Text;
                    txtpOthers.Text = "";
                    txtpUponSign.Text = "100";
                    txtpBalanceDue.Text = "";
                    txtpUponCompletion.Text = "";
                    txtnUponSign.Text = lblGtotal.Text;

                }
            }
            else
            {
                Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
                hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


                if (objPayTerm.UponSign_value != null)
                {
                    txtUponSignValue.Text = objPayTerm.UponSign_value.ToString().Replace("^", "'");
                    txtUponSignValue.ToolTip = objPayTerm.UponSign_value.ToString().Replace("^", "'");
                }
                if (objPayTerm.UponCompletion_value != null)
                {
                    txtUponCompletionValue.Text = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
                    txtUponCompletionValue.ToolTip = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
                }
                if (objPayTerm.BalanceDue_value != null)
                {
                    txtBalanceDue.Text = objPayTerm.BalanceDue_value.Replace("^", "'");
                    txtBalanceDue.ToolTip = objPayTerm.BalanceDue_value.Replace("^", "'");
                }
                if (objPayTerm.other_value != null)
                {
                    txtOthers.Text = objPayTerm.other_value.Replace("^", "'");
                    txtOthers.ToolTip = objPayTerm.other_value.Replace("^", "'");
                }

                txtpUponSign.Text = Convert.ToDecimal(objPayTerm.UponSign_percent).ToString();
                txtnUponSign.Text = Convert.ToDecimal(objPayTerm.UponSign_amount).ToString("c");

                txtpUponCompletion.Text = Convert.ToDecimal(objPayTerm.UponCompletion_percent).ToString();
                txtnUponCompletion.Text = Convert.ToDecimal(objPayTerm.UponCompletion_amount).ToString("c");

                txtpBalanceDue.Text = Convert.ToDecimal(objPayTerm.BalanceDue_percent).ToString();
                txtnBalanceDue.Text = Convert.ToDecimal(objPayTerm.BalanceDue_amount).ToString("c");

                txtpOthers.Text = Convert.ToDecimal(objPayTerm.other_percent).ToString();
                txtnOthers.Text = Convert.ToDecimal(objPayTerm.other_amount).ToString("c");

                txtUponSignDate.Text = objPayTerm.UponSign_date;
                txtUponCompletionDate.Text = objPayTerm.UponCompletion_date;
                txtBalanceDueDate.Text = objPayTerm.BalanceDue_date;
                txtOtherDate.Text = objPayTerm.other_date;
            }
            Calculate();
               csCommonUtility.SetPagePermission(this.Page, new string[] { "chkChangeOrderQtyshow", "pnlUpdateCoEstimate", "lnkUpdateCoEstimate", "imgCODate", "ddlChangeOrderType", "rdoStatus", "btnCalculate", "ChkIsTax", "btnSave", "btnApproveSubmit", "btnChangeOrder", "rdoconfirm", "ddlStatus", "ddlTotalHeader", "lnkResend" });
              csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "ddlEconomics", "ddlVendor", "Delete", "Undo" });
        }

    }
    private void customer_review()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customerchangeorderstatus objccos = new customerchangeorderstatus();
        // Data not exist
        if (_db.customerchangeorderstatus.Where(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value) && c.estimateid == Convert.ToInt32(hdnEstimateId.Value) && c.changeorderid == Convert.ToInt32(hdnChEstId.Value)).SingleOrDefault() == null)
        {

            tblReview.Visible = false;
        }
        else
        {
            // Data exist
            rdoStatus.Enabled = false;
            objccos = _db.customerchangeorderstatus.Single(c => c.customerid == Convert.ToInt32(hdnCustomerId.Value) && c.estimateid == Convert.ToInt32(hdnEstimateId.Value) && c.changeorderid == Convert.ToInt32(hdnChEstId.Value));
            rdoStatus.SelectedValue = objccos.status.ToString();
            if (objccos.status == 2)
            {
                lblAcceptReason.Text = "Accepted at " + Convert.ToDateTime(objccos.accepteddate).ToShortDateString();
                //lblNameReason.Text = "Accepted Name: " + objccos.acceptedby;
                lblNameReason.Text = "";
            }
            else if (objccos.status == 3)
            {
                lblAcceptReason.Text = "Rejected at " + Convert.ToDateTime(objccos.rejectdate).ToShortDateString();
                lblNameReason.Text = "Reason for reject: " + objccos.causeforreject;
            }
            else
            {
                lblAcceptReason.Visible = false;
                lblNameReason.Visible = false;
            }
        }
    }



    protected void lnkAddMoreLocation_Click(object sender, EventArgs e)
    {

        Response.Redirect("change_order_locations.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&clid=1");
    }
    protected void lnkAddMoreSections_Click(object sender, EventArgs e)
    {
        Response.Redirect("change_order_sections.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&csid=1");
    }




    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("changeorder_pricing.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }

    private void AddChildMenu(TreeNode parentNode, sectioninfo sec)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT * FROM sectioninfo WHERE  client_id=" + Convert.ToInt32(hdnClientId.Value) + " AND section_id NOT IN (SELECT item_id FROM item_price WHERE client_id=" + Convert.ToInt32(hdnClientId.Value) + ")";
        IEnumerable<sectioninfo> list = _db.ExecuteQuery<sectioninfo>(strQ, string.Empty);
        foreach (sectioninfo subsec in list)
        {
            if (subsec.parent_id.ToString() == parentNode.Value)
            {
                TreeNode node = new TreeNode(subsec.section_name, subsec.section_id.ToString());
                parentNode.ChildNodes.Add(node);
                AddChildMenu(node, subsec);
            }
        }
    }


    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.item_status_id!=1 AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";

        }
        else
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName from co_pricing_master  INNER JOIN location on location.location_id = co_pricing_master.location_id where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND   co_pricing_master.item_status_id!=1 AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        grdGrouping.DataSource = mList;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

    }

    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND  co_pricing_master.item_status_id!=1 AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName from co_pricing_master  INNER JOIN location on location.location_id = co_pricing_master.location_id where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND  co_pricing_master.item_status_id!=1 AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }



    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Click"); 
        lblMessage.Text = "";
        txtNewChangeOrderName.Text = "";
        modUpdateCoEstimate.Hide();
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblMessage.Text = "";
        if (txtNewChangeOrderName.Text.Trim() == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("New Changeorder Name is required field");

            modUpdateCoEstimate.Show();
            return;
        }

        string strNewName = txtNewChangeOrderName.Text.Trim();
        DataClassesDataContext _db = new DataClassesDataContext();
        changeorder_estimate co_est = new changeorder_estimate();
        int ncid = Convert.ToInt32(hdnCustomerId.Value);
        int nestId = Convert.ToInt32(hdnEstimateId.Value);
        int coId = Convert.ToInt32(hdnChEstId.Value);
        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnChEstId.Value) > 0)
            co_est = _db.changeorder_estimates.Single(ce => ce.customer_id == ncid && ce.chage_order_id == coId && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(hdnClientId.Value));

        co_est.changeorder_name = strNewName;

        _db.SubmitChanges();


        lblMessage.Text = "";
        txtNewChangeOrderName.Text = "";
        modUpdateCoEstimate.Hide();

        Response.Redirect("changeorder_pricing.aspx?eid=" + nestId + "&cid=" + ncid + "&coestid=" + coId);
    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        lblResult1.Text = "";
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (hdnChangeOrderView.Value == "1")
            {
                gv1.Columns[5].Visible = false;
                gv1.Columns[6].Visible = false;
            }
            else
            {
                gv1.Columns[5].Visible = true;
                gv1.Columns[6].Visible = true;
            }

            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                LinkButton btn = (LinkButton)e.Row.Cells[10].Controls[0];
                btn.Text = "Undo";
                btn.CommandName = "Undo";



            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }


        }

    }

    protected void grdSelectedItem_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridView grdSelectedItem = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem.DataKeys[e.RowIndex].Values[0].ToString();
        co_pricing_master cpl = new co_pricing_master();
        cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
        string strQ = "";
        if (cpl.item_status_id == 3)
            strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        else
            strQ = "UPDATE co_pricing_master SET item_status_id=2,prev_total_price=" + cpl.total_retail_price + ",total_retail_price=0 WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);

        _db.ExecuteCommand(strQ, string.Empty);

        subtotal = 0.0;
        subtotal_diect = 0.0;

        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        CalculateTotal();

    }


    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    //protected string GetTotalPrice()
    //{
    //    return "Total: " + grandtotal.ToString("c");
    //}
    //protected string GetTotalPriceDirect()
    //{
    //    return "Total: " + grandtotal_direct.ToString("c");
    //}
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);

            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("txtViewEcon") as TextBox).Text);
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            subtotal = 0.0;
        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (rdoSort.SelectedValue == "1")
        {

            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.item_status_id != 1 && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value)
                               orderby p.section_level ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   is_direct = (int)p.is_direct,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = lc.location_name,
                                   section_name = p.section_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   prev_total_price = (decimal)p.prev_total_price,
                                   tmpCol = string.Empty,
                               };
            List<CO_PricingDeatilModel> Pm_List1 = price_detail.ToList();
            List<ChangeOrderPricingListModel> objlist1 = new List<ChangeOrderPricingListModel>();
            foreach (CO_PricingDeatilModel objcl1 in Pm_List1)
            {
                ChangeOrderPricingListModel co_obj1 = new ChangeOrderPricingListModel();
                co_obj1.co_pricing_list_id = objcl1.co_pricing_list_id;
                co_obj1.client_id = objcl1.client_id;
                co_obj1.customer_id = objcl1.customer_id;
                co_obj1.estimate_id = objcl1.estimate_id;
                co_obj1.chage_order_id = Convert.ToInt32(hdnChEstId.Value);
                co_obj1.location_id = objcl1.location_id;
                co_obj1.sales_person_id = objcl1.sales_person_id;
                co_obj1.section_level = objcl1.section_level;
                co_obj1.item_id = objcl1.item_id;
                co_obj1.section_name = objcl1.section_name;
                co_obj1.item_name = objcl1.item_name;
                co_obj1.is_direct = objcl1.is_direct;
                co_obj1.item_status_id = objcl1.item_status_id;
                co_obj1.section_serial = objcl1.section_serial;
                co_obj1.short_notes = objcl1.short_notes;
                co_obj1.measure_unit = objcl1.measure_unit;
                co_obj1.quantity = objcl1.quantity;
                if (objcl1.is_direct == 1)
                {
                    if (objcl1.item_status_id == 2)
                    {
                        co_obj1.total_direct_price = 0;
                        co_obj1.total_retail_price = objcl1.prev_total_price * -1;
                    }
                    else
                    {
                        co_obj1.total_direct_price = 0;
                        co_obj1.total_retail_price = objcl1.prev_total_price;
                    }
                }
                else
                {
                    if (objcl1.item_status_id == 2)
                    {
                        co_obj1.total_retail_price = 0;
                        co_obj1.total_direct_price = objcl1.prev_total_price * -1;
                    }
                    else
                    {
                        co_obj1.total_retail_price = 0;
                        co_obj1.total_direct_price = objcl1.prev_total_price;
                    }
                }
                if (_db.change_order_pricing_lists.Where(pd => pd.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && pd.co_pricing_list_id == objcl1.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value)).ToList().Count > 0)
                {
                    change_order_pricing_list copl = new change_order_pricing_list();
                    copl = _db.change_order_pricing_lists.Single(pd => pd.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && pd.co_pricing_list_id == objcl1.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value));
                    co_obj1.EconomicsId = (int)copl.EconomicsId;
                    co_obj1.EconomicsCost = (decimal)copl.EconomicsCost;
                }
                else
                {
                    co_obj1.EconomicsId = 2;
                    co_obj1.EconomicsCost = co_obj1.total_retail_price;
                }
                objlist1.Add(co_obj1);
            }

            //grd.DataSource = price_detail.ToList();

            grd.DataSource = objlist1;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
            grd.DataBind();

        }
        else
        {
            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level)
                                      && p.section_level == colId && p.item_status_id != 1 && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value)
                               orderby lc.location_name ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   is_direct = (int)p.is_direct,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = p.section_name,
                                   section_name = lc.location_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   item_status_id = (int)p.item_status_id,
                                   prev_total_price = (decimal)p.prev_total_price,
                                   tmpCol = string.Empty,
                               };
            List<CO_PricingDeatilModel> Pm_List = price_detail.ToList();
            List<ChangeOrderPricingListModel> objlist = new List<ChangeOrderPricingListModel>();
            foreach (CO_PricingDeatilModel objcl in Pm_List)
            {
                ChangeOrderPricingListModel co_obj = new ChangeOrderPricingListModel();
                co_obj.co_pricing_list_id = objcl.co_pricing_list_id;
                co_obj.client_id = objcl.client_id;
                co_obj.customer_id = objcl.customer_id;
                co_obj.estimate_id = objcl.estimate_id;
                co_obj.chage_order_id = Convert.ToInt32(hdnChEstId.Value);
                co_obj.location_id = objcl.location_id;
                co_obj.sales_person_id = objcl.sales_person_id;
                co_obj.section_level = objcl.section_level;
                co_obj.item_id = objcl.item_id;
                co_obj.section_name = objcl.section_name;
                co_obj.item_name = objcl.item_name;
                co_obj.is_direct = objcl.is_direct;
                co_obj.item_status_id = objcl.item_status_id;
                co_obj.short_notes = objcl.short_notes;
                co_obj.measure_unit = objcl.measure_unit;
                co_obj.quantity = objcl.quantity;
                if (objcl.is_direct == 1)
                {
                    if (objcl.item_status_id == 2)
                    {
                        co_obj.total_direct_price = 0;
                        co_obj.total_retail_price = objcl.prev_total_price * -1;
                    }
                    else
                    {
                        co_obj.total_direct_price = 0;
                        co_obj.total_retail_price = objcl.prev_total_price;
                    }
                }
                else
                {
                    if (objcl.item_status_id == 2)
                    {
                        co_obj.total_retail_price = 0;
                        co_obj.total_direct_price = objcl.prev_total_price * -1;
                    }
                    else
                    {
                        co_obj.total_retail_price = 0;
                        co_obj.total_direct_price = objcl.prev_total_price;
                    }
                }
                if (_db.change_order_pricing_lists.Where(pd => pd.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && pd.co_pricing_list_id == objcl.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value)).ToList().Count > 0)
                {
                    change_order_pricing_list copl = new change_order_pricing_list();
                    copl = _db.change_order_pricing_lists.Single(pd => pd.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && pd.co_pricing_list_id == objcl.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value));
                    co_obj.EconomicsId = (int)copl.EconomicsId;
                    co_obj.EconomicsCost = (decimal)copl.EconomicsCost;
                }
                else
                {
                    co_obj.EconomicsId = 0;
                    co_obj.EconomicsCost = 0;
                }
                objlist.Add(co_obj);
            }



            //grd.DataSource = price_detail.ToList();
            grd.DataSource = objlist;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
            grd.DataBind();

        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged"); 
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
    }
    protected void grdSelectedItem2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridView grdSelectedItem2 = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem2.DataKeys[e.RowIndex].Values[0].ToString();
        co_pricing_master cpl = new co_pricing_master();
        cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
        string strQ = "";
        if (cpl.item_status_id == 3)
            strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        else
            strQ = "UPDATE co_pricing_master SET item_status_id=2,prev_total_price=" + cpl.total_direct_price + ",total_direct_price=0 WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);

        _db.ExecuteCommand(strQ, string.Empty);

        subtotal = 0.0;
        subtotal_diect = 0.0;

        BindSelectedItemGrid_Direct();
        BindSelectedItemGrid();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        CalculateTotal();
    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv2 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (hdnChangeOrderView.Value == "1")
            {
                gv2.Columns[5].Visible = false;
                gv2.Columns[6].Visible = false;
            }
            else
            {
                gv2.Columns[5].Visible = true;
                gv2.Columns[6].Visible = true;
            }

            int nItemStatusId = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[1]);
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                LinkButton btn = (LinkButton)e.Row.Cells[10].Controls[0];
                btn.Text = "Undo";
                btn.CommandName = "Undo";
            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }


        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);

            int nDirectId = 2;

            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;

            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {

                Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                subtotal_diect += Double.Parse((row.FindControl("txtViewEcon1") as TextBox).Text);
                labelTotal2.Text = subtotal_diect.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader2.Text = "Section";
                }
                else
                {
                    lblHeader2.Text = "Location";
                }
                lblSubTotalLabel2.Text = "Sub Total:";
            }
            subtotal_diect = 0.0;
        }
    }
    public string GetItemDetialsForUpdateItem(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.parent_id > 0 && c.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
        foreach (sectioninfo sec1 in list)
        {
            strDetails = sec1.section_name + " >> " + strDetails;
            GetItemDetialsForUpdateItem(Convert.ToInt32(sec1.parent_id));
        }
        return strDetails;
    }
    public string GetSectionName(int section_level)
    {
        string str = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        sectioninfo si = new sectioninfo();
        si = _db.sectioninfos.Single(c => c.section_level == section_level && c.parent_id == 0 && c.client_id == Convert.ToInt32(hdnClientId.Value));
        str = si.section_name;
        return str;
    }
    private int GetSerial()
    {
        int nSerial = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.co_pricing_masters
                      where pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.section_level == Convert.ToInt32(hdnSectionLevel.Value)
                      select pd.item_cnt);
        int n = result.Count();
        if (result != null && n > 0)
            nSerial = result.Max();

        return nSerial + 1;
    }

    protected void grdSelectedItem1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView grdSelectedItem = (GridView)sender;
        if (e.CommandName == "Undo")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            hdnPricingId.Value = grdSelectedItem.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0].ToString();
            co_pricing_master cpl = new co_pricing_master();
            cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
            string strQ = "UPDATE co_pricing_master SET item_status_id=1,total_retail_price=" + cpl.prev_total_price + " WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            //string strQ = "Delete co_pricing_master WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

        }
        subtotal = 0.0;
        subtotal_diect = 0.0;
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        CalculateTotal();

    }
    protected void grdSelectedItem2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView grdSelectedItem2 = (GridView)sender;
        if (e.CommandName == "Undo")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            hdnPricingId.Value = grdSelectedItem2.DataKeys[Convert.ToInt32(e.CommandArgument)].Values[0].ToString();
            co_pricing_master cpl = new co_pricing_master();
            cpl = _db.co_pricing_masters.Single(c => c.co_pricing_list_id == Convert.ToInt32(hdnPricingId.Value) && c.client_id == Convert.ToInt32(hdnClientId.Value));
            string strQ = "UPDATE co_pricing_master SET item_status_id=1, total_direct_price=" + cpl.prev_total_price + " WHERE co_pricing_list_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

        }
        subtotal = 0.0;
        subtotal_diect = 0.0;
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        CalculateTotal();
    }
    protected bool CheckDate(String date)
    {

        try
        {

            DateTime dt = DateTime.Parse(date);

            return true;

        }
        catch
        {

            return false;

        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult1.Text = "";
        int itemCount = 0;
        Calculate();
        bool IsMarkUp = false;
        if (txtChangeOrderDate.Text == "")
        {
            lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Please insert Change Order Date.");
            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtChangeOrderDate.Text);
            }
            catch (Exception ex)
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Change Order Date.");
                return;
            }
        }
        if (txtUponCompletionDate.Text != "")
        {
            if (!CheckDate(txtUponCompletionDate.Text))
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Format.");
                return;
            }
        }
        if (txtBalanceDueDate.Text != "")
        {
            if (!CheckDate(txtBalanceDueDate.Text))
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Format.");
                return;
            }
        }

        if (txtUponSignDate.Text != "")
        {
            if (!CheckDate(txtUponSignDate.Text))
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Format.");
                return;
            }
        }

        if (txtOtherDate.Text != "")
        {
            if (!CheckDate(txtOtherDate.Text))
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Format.");
                return;
            }
        }


        if (Convert.ToInt32(ddlStatus.SelectedValue) == 3)
        {
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                foreach (GridViewRow di in grdSelectedItem1.Rows)
                {
                    DropDownList ddlEconomics = (DropDownList)di.FindControl("ddlEconomics");
                    itemCount++;
                    if (Convert.ToInt32(ddlEconomics.SelectedValue) == 0)
                    {
                        ddlEconomics.ForeColor = System.Drawing.Color.Red;
                        IsMarkUp = true;
                    }
                }
            }

            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
                foreach (GridViewRow di in grdSelectedItem2.Rows)
                {
                    DropDownList ddlEconomics1 = (DropDownList)di.FindControl("ddlEconomics1");
                    if (Convert.ToInt32(ddlEconomics1.SelectedValue) == 0)
                    {
                        ddlEconomics1.ForeColor = System.Drawing.Color.Red;
                        IsMarkUp = true;
                    }
                }
            }

            if(itemCount==0)
            {
                lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Without any change order item, this chnage order will not execute.");
                return;
            }
        }

        if (IsMarkUp)
        {
            lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Markup");
            return;
        }

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ_Delete = "Delete change_order_pricing_list WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        _db.ExecuteCommand(strQ_Delete, string.Empty);

        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                Label lblTotal_price = (Label)di.FindControl("lblTotal_price");
                DropDownList ddlEconomics = (DropDownList)di.FindControl("ddlEconomics");
                ddlEconomics.ForeColor = System.Drawing.Color.Black;
                TextBox txtViewEcon = (TextBox)di.FindControl("txtViewEcon");
                co_pricing_master cpm = new co_pricing_master();
                cpm = _db.co_pricing_masters.Single(ce => ce.co_pricing_list_id == Convert.ToInt32(grdSelectedItem1.DataKeys[di.RowIndex].Values[0]) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                change_order_pricing_list co_obj = new change_order_pricing_list();
                co_obj.co_pricing_list_id = Convert.ToInt32(grdSelectedItem1.DataKeys[di.RowIndex].Values[0]);
                co_obj.client_id = Convert.ToInt32(hdnClientId.Value);
                co_obj.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                co_obj.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                co_obj.chage_order_id = Convert.ToInt32(hdnChEstId.Value);
                co_obj.location_id = cpm.location_id;
                co_obj.sales_person_id = cpm.sales_person_id;
                co_obj.section_level = cpm.section_level;
                co_obj.item_id = cpm.item_id;
                co_obj.section_serial = cpm.section_serial;
                co_obj.section_name = cpm.section_name;
                co_obj.item_name = cpm.item_name;
                co_obj.is_direct = cpm.is_direct;
                co_obj.item_status_id = cpm.item_status_id;
                co_obj.measure_unit = cpm.measure_unit;
                co_obj.short_notes = cpm.short_notes;
                co_obj.quantity = cpm.quantity;
                if (cpm.is_direct == 1)
                {
                    co_obj.total_direct_price = 0;
                    co_obj.total_retail_price = cpm.prev_total_price;
                }
                else
                {
                    co_obj.total_retail_price = 0;
                    co_obj.total_direct_price = cpm.prev_total_price;
                }
                co_obj.EconomicsId = Convert.ToInt32(ddlEconomics.SelectedValue);
                co_obj.EconomicsCost = Convert.ToDecimal(txtViewEcon.Text);
                co_obj.create_date = DateTime.Now;
                co_obj.last_update_date = DateTime.Now;
                _db.change_order_pricing_lists.InsertOnSubmit(co_obj);

            }
        }
        foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
        {
            GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
            foreach (GridViewRow di in grdSelectedItem2.Rows)
            {
                Label lblTotal_price1 = (Label)di.FindControl("lblTotal_price2");
                DropDownList ddlEconomics1 = (DropDownList)di.FindControl("ddlEconomics1");
                ddlEconomics1.ForeColor = System.Drawing.Color.Black;
                TextBox txtViewEcon1 = (TextBox)di.FindControl("txtViewEcon1");
                co_pricing_master cpm = new co_pricing_master();
                cpm = _db.co_pricing_masters.Single(ce => ce.co_pricing_list_id == Convert.ToInt32(grdSelectedItem2.DataKeys[di.RowIndex].Values[0]) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                change_order_pricing_list co_obj = new change_order_pricing_list();
                co_obj.co_pricing_list_id = Convert.ToInt32(grdSelectedItem2.DataKeys[di.RowIndex].Values[0]);
                co_obj.client_id = Convert.ToInt32(hdnClientId.Value);
                co_obj.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                co_obj.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                co_obj.chage_order_id = Convert.ToInt32(hdnChEstId.Value);
                co_obj.location_id = cpm.location_id;
                co_obj.sales_person_id = cpm.sales_person_id;
                co_obj.section_level = cpm.section_level;
                co_obj.item_id = cpm.item_id;
                co_obj.section_serial = cpm.section_serial;
                co_obj.section_name = cpm.section_name;
                co_obj.item_name = cpm.item_name;
                co_obj.is_direct = cpm.is_direct;
                co_obj.item_status_id = cpm.item_status_id;
                co_obj.measure_unit = cpm.measure_unit;
                co_obj.short_notes = cpm.short_notes;
                co_obj.quantity = cpm.quantity;
                if (cpm.is_direct == 1)
                {
                    co_obj.total_direct_price = 0;
                    co_obj.total_retail_price = cpm.prev_total_price;
                }
                else
                {
                    co_obj.total_retail_price = 0;
                    co_obj.total_direct_price = cpm.prev_total_price;
                }
                co_obj.EconomicsId = Convert.ToInt32(ddlEconomics1.SelectedValue);
                co_obj.EconomicsCost = Convert.ToDecimal(txtViewEcon1.Text);
                co_obj.create_date = DateTime.Now;
                co_obj.last_update_date = DateTime.Now;

                _db.change_order_pricing_lists.InsertOnSubmit(co_obj);

            }
        }
        string strQ = "";
        string MasterAdd_strQ = "";
        string MasterDel_strQ = "";
        int ChangeOrderQtyViewByCust = 0;
        bool btax = true;
        bool IsClose = false;
        if (chkChangeOrderQtyshow.Checked)
        {
            ChangeOrderQtyViewByCust = 1;
        }
        else
        {
            ChangeOrderQtyViewByCust = 0;
        }
        if (ChkIsTax.Checked == false)
            btax = false;
        if (Convert.ToInt32(ddlStatus.SelectedValue) == 3)
        {
            IsClose = true;
            strQ = "UPDATE changeorder_estimate SET IsChangeOrderQtyViewByCust= " + ChangeOrderQtyViewByCust + ",is_cutomer_viewable = " + Convert.ToInt32(rdoconfirm.SelectedValue) + ", changeorder_name='" + lblChangeOrderName.Text.Replace("'", "''") + "', change_order_status_id=" + Convert.ToInt32(ddlStatus.SelectedValue) + ",comments='" + txtComments.Text.Replace("'", "''") + "',change_order_type_id=" + Convert.ToInt32(ddlChangeOrderType.SelectedValue) + ",payment_terms='" + ddlTerms.SelectedItem.Text.Replace("'", "''") + "', other_terms='" + txtOtherTerms.Text.Replace("'", "''") + "',is_total=" + Convert.ToInt32(rdoList.SelectedValue) + ",is_tax='" + btax + "',tax=" + Convert.ToDecimal(txtTaxPer.Text.Replace("%", "").Replace("$", "")) + ",total_payment_due='" + ddlTotalHeader.SelectedItem.Text.Replace("'", "''") + "',changeorder_date='" + txtChangeOrderDate.Text + "',execute_date='" + DateTime.Today + "',notes1='" + txtNotes1.Text.Replace("'", "''") + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Now + "' WHERE chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id=1 WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
            _db.ExecuteCommand(MasterDel_strQ, string.Empty);
        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) == 4)
        {
            IsClose = true;
            strQ = "UPDATE changeorder_estimate SET IsChangeOrderQtyViewByCust= " + ChangeOrderQtyViewByCust + ",is_cutomer_viewable = " + Convert.ToInt32(rdoconfirm.SelectedValue) + ", changeorder_name='" + lblChangeOrderName.Text.Replace("'", "''") + "', change_order_status_id=" + Convert.ToInt32(ddlStatus.SelectedValue) + ",comments='" + txtComments.Text.Replace("'", "''") + "',change_order_type_id=" + Convert.ToInt32(ddlChangeOrderType.SelectedValue) + ",payment_terms='" + ddlTerms.SelectedItem.Text.Replace("'", "''") + "', other_terms='" + txtOtherTerms.Text + "',is_total=" + Convert.ToInt32(rdoList.SelectedValue) + ",is_tax='" + btax + "',tax=" + Convert.ToDecimal(txtTaxPer.Text.Replace("%", "").Replace("$", "")) + ",total_payment_due='" + ddlTotalHeader.SelectedItem.Text.Replace("'", "''") + "',changeorder_date='" + txtChangeOrderDate.Text + "',execute_date='" + DateTime.Today + "',notes1='" + txtNotes1.Text.Replace("'", "''") + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Today + "' WHERE chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id=1,total_retail_price=prev_total_price WHERE item_status_id = 2 AND is_direct=1 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            string MasterAdd_direct_strQ = "UPDATE co_pricing_master SET item_status_id=1,total_direct_price=prev_total_price WHERE item_status_id = 2 AND is_direct=2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
            _db.ExecuteCommand(MasterAdd_direct_strQ, string.Empty);
            _db.ExecuteCommand(MasterDel_strQ, string.Empty);
        }
        else
        {
            IsClose = false;
            strQ = "UPDATE changeorder_estimate SET IsChangeOrderQtyViewByCust= " + ChangeOrderQtyViewByCust + ",is_cutomer_viewable = " + Convert.ToInt32(rdoconfirm.SelectedValue) + ", changeorder_name='" + lblChangeOrderName.Text.Replace("'", "''") + "', change_order_status_id=" + Convert.ToInt32(ddlStatus.SelectedValue) + ",comments='" + txtComments.Text.Replace("'", "''") + "',change_order_type_id=" + Convert.ToInt32(ddlChangeOrderType.SelectedValue) + ",payment_terms='" + ddlTerms.SelectedItem.Text.Replace("'", "''") + "', other_terms='" + txtOtherTerms.Text.Replace("'", "''") + "',is_total=" + Convert.ToInt32(rdoList.SelectedValue) + ",is_tax='" + btax + "',tax=" + Convert.ToDecimal(txtTaxPer.Text.Replace("%", "").Replace("$", "")) + ",total_payment_due='" + ddlTotalHeader.SelectedItem.Text.Replace("'", "''") + "',changeorder_date='" + txtChangeOrderDate.Text + "',notes1='" + txtNotes1.Text.Replace("'", "''") + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Today + "' WHERE chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
        }

        _db.ExecuteCommand(strQ, string.Empty);
        Co_PaymentTerm objPayTerm = new Co_PaymentTerm();
        if (Convert.ToInt32(hdnChPaymentId.Value) > 0)
            objPayTerm = _db.Co_PaymentTerms.SingleOrDefault(ep => ep.co_payment_id == Convert.ToInt32(hdnChPaymentId.Value));

        objPayTerm.client_id = Convert.ToInt32(hdnClientId.Value);
        objPayTerm.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        objPayTerm.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        objPayTerm.ChangeOrderId = Convert.ToInt32(hdnChEstId.Value);

        
        if (txtUponSignValue.Text.Trim() != "")
            objPayTerm.UponSign_value = txtUponSignValue.Text.Replace("'", "^");
        if (txtUponCompletionValue.Text.Trim() != "")
            objPayTerm.UponCompletion_value = txtUponCompletionValue.Text.Replace("'", "^");
        if (txtBalanceDue.Text.Trim() != "")
            objPayTerm.BalanceDue_value = txtBalanceDue.Text.Replace("'", "^");
        if (txtOthers.Text.Trim() != "")
            objPayTerm.other_value = txtOthers.Text.Replace("'", "^");

        if (txtpUponSign.Text.Trim() != "")
            objPayTerm.UponSign_percent = Convert.ToDecimal(txtpUponSign.Text);
        else
            objPayTerm.UponSign_percent = 0;

        if (txtnUponSign.Text.Trim() != "")
            objPayTerm.UponSign_amount = Convert.ToDecimal(txtnUponSign.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            objPayTerm.UponSign_amount = 0;

        if (txtpUponCompletion.Text.Trim() != "")
            objPayTerm.UponCompletion_percent = Convert.ToDecimal(txtpUponCompletion.Text);
        else
            objPayTerm.UponCompletion_percent = 0;
        if (txtnUponCompletion.Text != "")
            objPayTerm.UponCompletion_amount = Convert.ToDecimal(txtnUponCompletion.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            objPayTerm.UponCompletion_amount = 0;

        if (txtpBalanceDue.Text.Trim() != "")
            objPayTerm.BalanceDue_percent = Convert.ToDecimal(txtpBalanceDue.Text);
        else
            objPayTerm.BalanceDue_percent = 0;

        if (txtnBalanceDue.Text.Trim() != "")
            objPayTerm.BalanceDue_amount = Convert.ToDecimal(txtnBalanceDue.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            objPayTerm.BalanceDue_amount = 0;

        if (txtpOthers.Text.Trim() != "")
            objPayTerm.other_percent = Convert.ToDecimal(txtpOthers.Text);
        else
            objPayTerm.other_percent = 0;
        if (txtnOthers.Text != "")
            objPayTerm.other_amount = Convert.ToDecimal(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        else
            objPayTerm.other_amount = 0;

        objPayTerm.UponSign_date = txtUponSignDate.Text;
        objPayTerm.UponCompletion_date = txtUponCompletionDate.Text;
        objPayTerm.BalanceDue_date = txtBalanceDueDate.Text;
        objPayTerm.other_date = txtOtherDate.Text;
        if(hdnChPaymentId.Value == "0")
             _db.Co_PaymentTerms.InsertOnSubmit(objPayTerm);
        
        _db.SubmitChanges();
       hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();
        lblResult1.Text = "Data saved successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;

        // Executed or decliened Change Order Worksheet
        if (ddlStatus.SelectedValue == "3" || ddlStatus.SelectedValue == "4")
        {
            Response.Redirect("change_order_worksheet_readonly.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
        }

    }
    protected void ddlTerms_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlTerms.ID, ddlTerms.GetType().Name, "SelectedIndexChanged"); 
        if (ddlTerms.SelectedItem.Text.Trim() == "Other")
        {
            lblOther.Visible = true;
            txtOtherTerms.Visible = true;
        }
        else
        {
            lblOther.Visible = false;
            txtOtherTerms.Visible = false;
        }
    }
    private bool Calculate()
    {
        bool bFlag = true;
        lblResult1.Text = "";
        lblPr.ForeColor = Color.Black;

        double nTotal = Convert.ToDouble(lblGtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
       
        #region Calculate Percentage
        double pUponSign = 0;
        double pUponCompletion = 0;
        double pBalanceDue = 0;
        double pOthers = 0;

        double nUponSign = 0;
        double nUponCompletion = 0;
        double nBalanceDue = 0;
        double nOthers = 0;


        if (rdoCalc.SelectedIndex == 0)
        {
            #region Calculate Based on Percentage

            txtnUponSign.Text = "";
            txtnUponCompletion.Text = "";
            txtnBalanceDue.Text = "";
            txtnOthers.Text = "";
            if (txtpUponSign.Text.Trim().Length > 0)
            {
                pUponSign = Convert.ToDouble(txtpUponSign.Text);
                nUponSign = (nTotal * pUponSign) / 100;
                txtnUponSign.Text = nUponSign.ToString("c");
            }
            if (txtpUponCompletion.Text.Trim().Length > 0)
            {
                pUponCompletion = Convert.ToDouble(txtpUponCompletion.Text);
                nUponCompletion = (nTotal * pUponCompletion) / 100;
                txtnUponCompletion.Text = nUponCompletion.ToString("c");
            }
            if (txtpBalanceDue.Text.Trim().Length > 0)
            {
                pBalanceDue = Convert.ToDouble(txtpBalanceDue.Text);
                nBalanceDue = (nTotal * pBalanceDue) / 100;
                txtnBalanceDue.Text = nBalanceDue.ToString("c");
            }

            if (txtpOthers.Text.Trim().Length > 0)
            {
                pOthers = Convert.ToDouble(txtpOthers.Text);
                nOthers = (nTotal * pOthers) / 100;
                txtnOthers.Text = nOthers.ToString("c");
            }
            double totalPercentage = pUponSign + pUponCompletion + pBalanceDue + pOthers;
            lblPr.Text = totalPercentage.ToString("0.00");
            if (totalPercentage == 100)
                btnSave.Enabled = true;
            else
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage(" % total should be 100%.");

                bFlag = false;
            }
            #endregion
        }
        else
        {
            #region Calculate Based on Doller Amount
            txtpUponSign.Text = "";
            txtpUponCompletion.Text = "";
            txtpBalanceDue.Text = "";
            txtpOthers.Text = "";

            if (txtnUponSign.Text.Replace("$", "").Replace("(", "-").Replace(")", "").Trim().Length > 0)
            {
                nUponSign = Convert.ToDouble(txtnUponSign.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                pUponSign = (100 * nUponSign) / nTotal;
                txtpUponSign.Text = pUponSign.ToString("0.00");
            }
            if (txtnUponCompletion.Text.Replace("$", "").Trim().Length > 0)
            {
                nUponCompletion = Convert.ToDouble(txtnUponCompletion.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                pUponCompletion = (100 * nUponCompletion) / nTotal;
                txtpUponCompletion.Text = pUponCompletion.ToString("0.00");
            }
            if (txtnBalanceDue.Text.Replace("$", "").Trim().Length > 0)
            {
                nBalanceDue = Convert.ToDouble(txtnBalanceDue.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                pBalanceDue = (100 * nBalanceDue) / nTotal;
                txtpBalanceDue.Text = pBalanceDue.ToString("0.00");
            }

            if (txtnOthers.Text.Replace("$", "").Trim().Length > 0)
            {
                nOthers = Convert.ToDouble(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                pOthers = (100 * nOthers) / nTotal;
                txtpOthers.Text = pOthers.ToString("0.00");
            }
            double totalPercentage = pUponSign + pUponCompletion + pBalanceDue + pOthers;
            lblPr.Text = totalPercentage.ToString("0.00");
            totalPercentage = Convert.ToDouble(lblPr.Text);
            if (totalPercentage == 100)
                btnSave.Enabled = true;
            else
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage(" % total should be 100%.");


                bFlag = false;
            }
            #endregion
        }
        return bFlag;

        #endregion
    }
    private void CalculateTotal()
    {
        decimal totalPrice = 0;
        decimal tax = 0;
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                string strNewPrice = ((TextBox)di.FindControl("txtViewEcon")).Text.Replace("$", "").Replace("(", "-").Replace(")", "");
                decimal price = 0;

                if (strNewPrice.Length > 0)
                {
                    price = Convert.ToDecimal(strNewPrice);
                    totalPrice += price;
                }


            }
        }
        lblProjectEcon.Text = totalPrice.ToString("c");
        if (!ChkIsTax.Checked)
        {
            txtTaxPer.Text = "0";

        }
        else
        {
            txtTaxPer.Text = txtTaxPer.ToolTip;
        }

        decimal taxRate = Convert.ToDecimal(txtTaxPer.Text);
        tax = totalPrice * (taxRate / 100);

        lblGtotal.Text = Convert.ToDecimal(totalPrice + tax).ToString("c");



    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        if (ddlStatus.SelectedValue == "3" || ddlStatus.SelectedValue == "4")
        {
            btnSavePopUp.Visible = true;

            btnSave.Visible = false;
        }
        else
        {
            btnSavePopUp.Visible = false;

            btnSave.Visible = true;
        }
    }
    protected void btnChangeOrder_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnChangeOrder.ID, btnChangeOrder.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes,quantity,measure_unit FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id AND change_order_pricing_list.client_id=location.client_id " +
                    " WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();

        if (rList.Count == 0)
        {
            lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Without any change order item, this report will not print");
            return;
        }
        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

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

        string strUponSignDate= string.Empty;
        string strUponCompletionDate= string.Empty;
        string strBalanceDueDate = string.Empty;
        string strOtherDate = string.Empty;

        decimal dUponSignAmount = 0;
        decimal dUponCompletionAmount = 0;
        decimal dBalanceDueAmount = 0;
        decimal dOtherAmount = 0;


        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
            hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();

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
            strUponSignValue = txtUponSignValue.Text;
            dUponSignAmount = Convert.ToDecimal(lblGtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

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
    protected void btnGotoChangeOrderList_Click(object sender, EventArgs e)
    {
        Response.Redirect("changeorderlist.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value);
    }

    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
    protected void btnViewHTML_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewHTML.ID, btnViewHTML.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id " +
                    " WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();

        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

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

        //if (_db.Co_PaymentTerms.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
            hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


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
            strUponSignValue = txtUponSignValue.Text;
            dUponSignAmount = Convert.ToDecimal(lblGtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

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

    protected void rdoconfirm_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoconfirm.ID, rdoconfirm.GetType().Name, "SelectedIndexChanged"); 
        foreach (System.Web.UI.WebControls.ListItem li in rdoconfirm.Items)
        {
            if (li.Selected && Convert.ToInt32(li.Value) == 1)
            {
                string sFileName = CreateReportfor_Mail();
                hdnFileName.Value = sFileName;
              
                  ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow()", true);

           //     li.Attributes.Add("onClick", "DisplayWindow();");

            }
        }


    }




    private string CreateReportfor_Mail()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id  " +
                    " WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();

        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(rList);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));

        string strSalesPerson = "( " + sp.first_name + " " + sp.last_name + " )";

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
        string COName = cho.changeorder_name;


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

        //if (_db.Co_PaymentTerms.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
            hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


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
            strUponSignValue = txtUponSignValue.Text;
            dUponSignAmount = Convert.ToDecimal(lblGtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

        }

        Hashtable ht = new Hashtable();

        ht.Add("p_CustomerName", lblCustomerName.Text);
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
            //    CRViewer.ReportSource = rptFile;

            return exportChangeOrderReport(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }

        return "";
    }

    protected string exportChangeOrderReport(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string strFile = "";
        try
        {

            string contentType = "";
            strFile = "CO_" + DateTime.Now.Ticks;

            // string tempFileName = Request.PhysicalApplicationPath + "tmp\\ChangeOrder\\";
            string tempFileName = Server.MapPath("tmp\\ChangeOrder") + @"\" + strFile;
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


            //System.IO.File.Delete(tempFileName);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strFile;
    }

    protected void Main_calculation()
    {
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                Label lblTotal_price = (Label)di.FindControl("lblTotal_price");
                DropDownList ddlEconomics = (DropDownList)di.FindControl("ddlEconomics");
                TextBox txtViewEcon = (TextBox)di.FindControl("txtViewEcon");

                if (Convert.ToInt32(ddlEconomics.SelectedValue) == 0)
                {
                    ddlEconomics.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    ddlEconomics.ForeColor = System.Drawing.Color.Black;
                }

                if (ddlEconomics.SelectedValue.Trim().Length > 0)
                {
                    decimal Cost = Convert.ToDecimal(lblTotal_price.Text);

                    if (Convert.ToInt32(ddlEconomics.SelectedValue) == 5)
                    {
                        txtViewEcon.Enabled = true;

                    }
                    else
                    {
                        txtViewEcon.Enabled = false;

                        if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 1)
                        {
                            txtViewEcon.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 0)
                        {
                            txtViewEcon.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 2)
                        {
                            txtViewEcon.Text = Cost.ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 3)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 20 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 4)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost * 2).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 6)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 30 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 7)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 50 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 8)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 70 / 100).ToString("0.00");
                        }

                    }
                }

            }
            GridViewRow footerRow = grdSelectedItem1.FooterRow;
            GridViewRow headerRow = grdSelectedItem1.HeaderRow;
            foreach (GridViewRow row in grdSelectedItem1.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("txtViewEcon") as TextBox).Text);
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            subtotal = 0.0;
        }
        CalculateTotal();
    }

    protected void NonDirect_calculation(object sender, EventArgs e)
    {
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                Label lblTotal_price = (Label)di.FindControl("lblTotal_price");
                DropDownList ddlEconomics = (DropDownList)di.FindControl("ddlEconomics");
                TextBox txtViewEcon = (TextBox)di.FindControl("txtViewEcon");

                if (Convert.ToInt32(ddlEconomics.SelectedValue) == 0)
                {
                    ddlEconomics.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    ddlEconomics.ForeColor = System.Drawing.Color.Black;
                }

                if (ddlEconomics.SelectedValue.Trim().Length > 0)
                {
                    decimal Cost = Convert.ToDecimal(lblTotal_price.Text);

                    if (Convert.ToInt32(ddlEconomics.SelectedValue) == 5)
                    {
                        txtViewEcon.Enabled = true;

                    }
                    else
                    {
                        txtViewEcon.Enabled = false;

                        if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 1)
                        {
                            txtViewEcon.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 0)
                        {
                            txtViewEcon.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 2)
                        {
                            txtViewEcon.Text = Cost.ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 3)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 20 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 4)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost * 2).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 6)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 30 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 7)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 50 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 8)
                        {
                            txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 70 / 100).ToString("0.00");
                        }

                    }
                }

            }
            GridViewRow footerRow = grdSelectedItem1.FooterRow;
            GridViewRow headerRow = grdSelectedItem1.HeaderRow;
            foreach (GridViewRow row in grdSelectedItem1.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("txtViewEcon") as TextBox).Text);
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            subtotal = 0.0;
        }
        CalculateTotal();
    }

    protected void Direct_calculation(object sender, EventArgs e)
    {
        foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
        {
            GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
            foreach (GridViewRow di in grdSelectedItem2.Rows)
            {
                Label lblTotal_price2 = (Label)di.FindControl("lblTotal_price2");
                DropDownList ddlEconomics1 = (DropDownList)di.FindControl("ddlEconomics1");
                TextBox txtViewEcon1 = (TextBox)di.FindControl("txtViewEcon1");
                if (ddlEconomics1.SelectedValue.Trim().Length > 0)
                {
                    ddlEconomics1.ForeColor = System.Drawing.Color.Black;
                    decimal Cost = Convert.ToDecimal(lblTotal_price2.Text);

                    if (Convert.ToInt32(ddlEconomics1.SelectedValue) == 5)
                    {
                        txtViewEcon1.Enabled = true;

                    }
                    else
                    {
                        txtViewEcon1.Enabled = false;

                        if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 1)
                        {
                            txtViewEcon1.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 0)
                        {
                            txtViewEcon1.Text = "0";
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 2)
                        {
                            txtViewEcon1.Text = Cost.ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 3)
                        {
                            txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 20 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 4)
                        {
                            txtViewEcon1.Text = Convert.ToDecimal(Cost * 2).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 6)
                        {
                            txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 30 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 7)
                        {
                            txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 50 / 100).ToString("0.00");
                        }
                        else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 8)
                        {
                            txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 70 / 100).ToString("0.00");
                        }

                    }
                }

            }
            GridViewRow footerRow = grdSelectedItem2.FooterRow;
            GridViewRow headerRow = grdSelectedItem2.HeaderRow;
            foreach (GridViewRow row in grdSelectedItem2.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader2") as Label;
                subtotal += Double.Parse((row.FindControl("txtViewEcon1") as TextBox).Text);
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            subtotal = 0.0;
        }
        CalculateTotal();
    }
    protected void ddlChangeOrderType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlChangeOrderType.ID, ddlChangeOrderType.GetType().Name, "SelectedIndexChanged"); 
        if (Convert.ToInt32(ddlChangeOrderType.SelectedValue) == 3)
        {
            rdoconfirm.SelectedValue = "2";
            tblReview.Visible = false;
            rdoconfirm.Enabled = false;
        }
        else
        {
            tblReview.Visible = true;
            rdoconfirm.Enabled = true;
        }

    }
    protected void txtTaxPer_TextChanged(object sender, EventArgs e)
    {
        if (ChkIsTax.Checked)
        {
            if (txtTaxPer.Text.Trim() == "")
            {
                lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing Tax Rate.");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtTaxPer.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Invalid Tax Rate.");
                    return;
                }
            }

            txtTaxPer.ToolTip = txtTaxPer.Text;
            CalculateTotal();
        }
    }
    protected void btnbtnDocuSign_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnbtnDocuSign.ID, btnbtnDocuSign.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name,short_notes FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id " +
                    " WHERE chage_order_id=" + Convert.ToInt32(hdnChEstId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_pricing_list.client_id=" + Convert.ToInt32(hdnClientId.Value);
        List<ChangeOrderPricingListModel> rList = _db.ExecuteQuery<ChangeOrderPricingListModel>(strQ, string.Empty).ToList();

        changeorder_estimate cho = new changeorder_estimate();
        cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptchange_order_Docusign.rpt");
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

        //if (_db.Co_PaymentTerms.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
            hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


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
            strUponSignValue = txtUponSignValue.Text;
            dUponSignAmount = Convert.ToDecimal(lblGtotal.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

        }

        string strId = hdnChEstId.Value + " " + hdnCustomerId.Value + "_" + hdnEstimateId.Value + "_" + Convert.ToInt32(hdnClientId.Value);
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
        ht.Add("p_CustomerEstimateId", strId);

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

        //if (lblEmail.Text.Length > 0)
        //{
        //    CreateSendEnvelope(FilePath, lblEmail.Text, lblCustomerName.Text);
        //    lblResult1.Text = csCommonUtility.GetSystemMessage("Change Order Document has been sent to Customer's email for electronic signature by using DocuSign");
        //}

        if (lblEmail.Text.Length > 0)
        {
            try
            {
                if (CreateSendEnvelope(FilePath, lblEmail.Text, lblCustomerName.Text))
                    lblResult1.Text = csCommonUtility.GetSystemMessage("Change Order Document has been sent to Customer's email for electronic signature by using DocuSign");
            }
            catch (Exception ex)
            {
                lblResult1.Text = csCommonUtility.GetSystemErrorMessage("DocuSign Error: " + ex.Message);
            }
        }



    }
    public bool CreateSendEnvelope(string pdfPath, string CustomerEmail, string CustomerName)
    {

        try
        {
            string username = ConfigurationManager.AppSettings["APIUserEmail"];
            string password = ConfigurationManager.AppSettings["Password"];
            string integratorKey = ConfigurationManager.AppSettings["IntegratorsKey"];
            string apiURL = ConfigurationManager.AppSettings["APIUrl"];

            string designerName = "John Smith";
            string designerEmail = "avijit@faztrack.com";


            ApiClient apiClient = new ApiClient(apiURL);

            DocuSign.eSign.Client.Configuration cfi = new DocuSign.eSign.Client.Configuration(apiClient);

            // configure 'X-DocuSign-Authentication' header

            string authHeader = "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + integratorKey + "\"}";

            cfi.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login API call

            DocuSign.eSign.Api.AuthenticationApi authApi = new AuthenticationApi(cfi);

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

                    doc.Name = "Change Order Documents";

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

                    signHere1.AnchorString = "Customer Signature";


                    signHere1.AnchorXOffset = "-10";

                    signHere1.AnchorYOffset = "-22";


                    signerCust.Tabs.SignHereTabs.Add(signHere1);


                    signerCust.Tabs.DateSignedTabs = new List<DateSigned>();
                    DateSigned signHereDate1 = new DateSigned();

                    signHereDate1.DocumentId = "1";

                    signHereDate1.AnchorString = "Customer Signature";
                    signHereDate1.AnchorXOffset = "230";

                    signHereDate1.AnchorYOffset = "-22";

                    signerCust.Tabs.DateSignedTabs.Add(signHereDate1);






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


                    signHereDesigner1.AnchorXOffset = "-10";

                    signHereDesigner1.AnchorYOffset = "-22";

                    signerDesigner.Tabs.SignHereTabs.Add(signHereDesigner1);



                    signerDesigner.Tabs.DateSignedTabs = new List<DateSigned>();
                    DateSigned signHereDesignerDate1 = new DateSigned();

                    signHereDesignerDate1.DocumentId = "1";

                    signHereDesignerDate1.AnchorString = "Design Consultant";
                    signHereDesignerDate1.AnchorXOffset = "230";

                    signHereDesignerDate1.AnchorYOffset = "-22";

                    //DateSigned signHereDesignerDate2 = new DateSigned();

                    //signHereDesignerDate2.DocumentId = "1";

                    //signHereDesignerDate2.AnchorString = "Date :";
                    //signHereDesignerDate2.AnchorXOffset = "50";

                    //signHereDesignerDate2.AnchorYOffset = "-2";

                    signerDesigner.Tabs.DateSignedTabs.Add(signHereDesignerDate1);

                    //signerDesigner.Tabs.DateSignedTabs.Add(signHereDesignerDate2);

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
            throw (ex);
        }
        return false;

    }

    protected void btnCalculate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCalculate.ID, btnCalculate.GetType().Name, "Click"); 
        Calculate();
    }
    protected void lnkResend_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkResend.ID, lnkResend.GetType().Name, "Click"); 
        string sFileName = CreateReportfor_Mail();
        hdnFileName.Value = sFileName;

        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow()", true);


    }

    private void GetTools()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string tools = string.Empty;
            userinfo obj = (userinfo)Session["oUser"];
            user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == obj.user_id);
            int ncid = Convert.ToInt32(hdnCustomerId.Value);
            int neid = Convert.ToInt32(hdnEstimateId.Value);

            customer_estimate cus_est = csCommonUtility.GetCustomerEstimateInfo(ncid, neid);
            bool bIsEstimateActive = Convert.ToBoolean(cus_est.IsEstimateActive);

            bool IsCustomersurveysExist = _db.customersurveys.Any(cl => cl.customerid == ncid && cl.estimate_id == neid);

            if (hdnEmailType.Value == "1")
                hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=" + neid;
            else
                hypMessage.NavigateUrl = "customermessagecenter.aspx?cid=" + ncid + "&eid=" + neid;

            hyp_vendor.NavigateUrl = "Vendor_cost_details.aspx?eid=" + neid + "&cid=" + ncid;

            if (!_db.estimate_payments.Any(est_p => est_p.estimate_id == neid && est_p.customer_id == ncid && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
            {
                hyp_Payment.NavigateUrl = "payment_info.aspx?eid=" + neid + "&cid=" + ncid;

            }
            else
            {
                estimate_payment objEstPay = new estimate_payment();
                objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == neid && pay.customer_id == ncid && pay.client_id == Convert.ToInt32(hdnClientId.Value));
                hyp_Payment.NavigateUrl = "payment_recieved.aspx?cid=" + ncid + "&epid=" + objEstPay.est_payment_id + "&eid=" + neid;

            }

            hyp_jstatus.NavigateUrl = "customer_job_status_info.aspx?eid=" + neid + "&cid=" + ncid;


            if (bIsEstimateActive && cus_est.status_id == 3)
            {
                hyp_Schedule.NavigateUrl = "schedulecalendar.aspx?eid=" + neid + "&cid=" + ncid + "&TypeID=1";
            }



            hyp_Sow.NavigateUrl = "composite_sow.aspx?eid=" + neid + "&cid=" + ncid;

            hyp_survey.NavigateUrl = "Customer_survey.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_survey.Target = "_blank";



            if (IsCustomersurveysExist)
            {
                hyp_survey.Visible = true;
            }
            else
            {
                hyp_survey.Visible = false;
            }

            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=2&eid=" + neid;
            hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=2&eid=" + neid;
            hyp_PreCon.NavigateUrl = "PreconstructionCheckList.aspx?eid=" + neid + "&cid=" + ncid;
            hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + neid + "&nbackId=1&cid=" + ncid;
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=1&eid=" + neid;
            hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + neid + "&nbackid=1&cid=" + ncid;
            hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + neid + "&cid=" + ncid;
            hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + neid + "&cid=" + ncid;
            hypCostLoc.Target = "_blank";

            if (objUser != null)
            {
                tools = (objUser.tools) ?? "";

                if (tools.Contains("Message"))
                    hypMessage.Visible = true;
                else
                    hypMessage.Visible = false;

                if (tools.Contains("ProjectNotes"))
                    hyp_ProjectNotes.Visible = true;
                else
                    hyp_ProjectNotes.Visible = false;

                if (tools.Contains("ActivityLog"))
                    hyp_CallLog.Visible = true;
                else
                    hyp_CallLog.Visible = false;

                if (tools.Contains("SiteReview"))
                    hyp_SiteReview.Visible = true;
                else
                    hyp_SiteReview.Visible = false;

                if (tools.Contains("DocumentManagement"))
                    hyp_DocumentManagement.Visible = true;
                else
                    hyp_DocumentManagement.Visible = false;



                if (tools.Contains("Payment") && cus_est.status_id == 3)
                    hyp_Payment.Visible = true;
                else
                    hyp_Payment.Visible = false;

                if (tools.Contains("JobStatus") && cus_est.status_id == 3)
                    hyp_jstatus.Visible = true;
                else
                    hyp_jstatus.Visible = false;

                if (tools.Contains("Schedule") && cus_est.status_id == 3)
                    hyp_Schedule.Visible = true;
                else
                    hyp_Schedule.Visible = false;

                if (tools.Contains("CompositeSow") && cus_est.status_id == 3)
                    hyp_Sow.Visible = true;
                else
                    hyp_Sow.Visible = false;

                if (tools.Contains("PreConCheckList") && cus_est.status_id == 3)
                    hyp_PreCon.Visible = true;
                else
                    hyp_PreCon.Visible = false;


                if (tools.Contains("Vendor") && cus_est.estimate_id != 0)
                    hyp_vendor.Visible = true;
                else
                    hyp_vendor.Visible = false;

                if (tools.Contains("AllowanceReport") && cus_est.estimate_id != 0)
                    hyp_Allowance.Visible = true;
                else
                    hyp_Allowance.Visible = false;

                if (tools.Contains("Selection") && cus_est.estimate_id != 0)
                    hyp_Section_Selection.Visible = true;
                else
                    hyp_Section_Selection.Visible = false;

                if (tools.Contains("ProjectSummary") && cus_est.estimate_id != 0)
                    hypCostLoc.Visible = true;
                else
                    hypCostLoc.Visible = false;

                if (tools.Contains("MaterialTracking") && cus_est.estimate_id != 0)
                    hyp_MaterialTracking.Visible = true;
                else
                    hyp_MaterialTracking.Visible = false;
            }
        }
        catch (Exception ex)
        {
        }
    }
}


