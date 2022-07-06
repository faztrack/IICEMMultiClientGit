using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Drawing;
using System.Net.Mail;
using System.Data;
using System.Text.RegularExpressions;

public partial class customerchangeorder : System.Web.UI.Page
{
    private double subtotal = 0.0;

    private double subtotal_diect = 0.0;
    private double grandtotal = 0.0;

    private double tTotal = 0.0;
    private double Tax = 0.0;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            try
            {

                KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                if (Session["oCustomerUser"] == null)
                {
                    Response.Redirect("customerlogin.aspx");
                }
                BindStates();
                int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
                hdnCustomerId.Value = nCid.ToString();

                Session.Add("CustomerId", hdnCustomerId.Value);

                int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
                hdnEstimateId.Value = nEstid.ToString();

                int nChEstid = Convert.ToInt32(Request.QueryString.Get("coid"));
                hdnCOId.Value = nChEstid.ToString();

                int nCOEstId = Convert.ToInt32(Request.QueryString.Get("coestid"));


                DataClassesDataContext _db = new DataClassesDataContext();

                string strQ = "UPDATE changeorder_estimate SET lastviewed ='" + DateTime.Now + "' WHERE change_order_estimate_id=" + nCOEstId;
                _db.ExecuteCommand(strQ, string.Empty);
                _db.SubmitChanges();
                company_profile com = new company_profile();
                if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
                {
                    com = _db.company_profiles.Single(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value));
                    lblComProfilelEmail.Text = com.email;
                    hdnCCEmail.Value = com.ChangeOrdersEmail ?? "";
                }
                // Get Customer Info
                if (Convert.ToInt32(hdnCustomerId.Value) > 0)
                {
                    customer objCust = new customer();
                    objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1;
                    lblCustEmail.Text = objCust.email;
                    lblLastName.Text = objCust.last_name1;

                    txtCardHolderName.Text = objCust.first_name1 + " " + objCust.last_name1;
                    txtAddress.Text = objCust.address;
                    txtCity.Text = objCust.city;
                    ddlState.SelectedValue = objCust.state;
                    txtZip.Text = objCust.zip_code;

                    hdnClientId.Value = objCust.client_id.ToString();

                    sales_person sp = new sales_person();
                    sp = _db.sales_persons.SingleOrDefault(s => s.sales_person_id == Convert.ToInt32(objCust.sales_person_id) && s.is_active == true);
                    if (sp != null)
                    {
                        lblSalesPerson.Text = sp.first_name + " " + sp.last_name;
                        lblSalesPersonEmail.Text = sp.email;
                    }

                    user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId && u.is_active == true);
                    if (uinfo != null)
                    {
                        hdnSuperandentEmail.Value = uinfo.email;
                    }


                }
                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                }
                GetCardLists();

                if (grdCardList.Rows.Count > 0)
                {
                    pnlExistCard.Visible = true;
                    pnlNewCard.Visible = false;
                    chkNewCard.Visible = true;
                    chkNewCard.Checked = false;
                }
                else
                {
                    pnlExistCard.Visible = false;
                    pnlNewCard.Visible = true;
                    chkNewCard.Checked = true;
                    chkNewCard.Visible = false;
                }
                // Get Change Order Info
                if (Convert.ToInt32(hdnCOId.Value) > 0)
                {
                    changeorder_estimate cho = new changeorder_estimate();
                    cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnCOId.Value));
                    hdnChangeOrderView.Value = cho.IsChangeOrderQtyViewByCust.ToString();
                    lblChangeOrderName.Text = cho.changeorder_name;

                    lbltax.Text = cho.tax.ToString();
                    // CO Payment Term

                    string strUponSignValue = string.Empty;
                    string strUponCompletionValue = string.Empty;
                    string strBalanceDueValue = string.Empty;
                    string strOtherValue = string.Empty;


                    decimal dUponSignAmount = 0;
                    decimal dUponCompletionAmount = 0;
                    decimal dBalanceDueAmount = 0;
                    decimal dOtherAmount = 0;
                    int nTermId = 0;

                    if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnCOId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)))
                    {
                        Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnCOId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value));
                        //hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


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


                        string strTerms = string.Empty;

                        if (dOtherAmount != 0)
                        {
                            nTermId = 4100 + Convert.ToInt32(hdnCOId.Value);
                            strTerms += strOtherValue + ": " + dOtherAmount.ToString("c") + "</br>";

                            lblamount.Text = dOtherAmount.ToString("c");
                            txtAmount.Text = dOtherAmount.ToString("c");
                            lblPayterm.Text = strOtherValue;
                            lblamount1.Text = dOtherAmount.ToString("c");
                            txtAmount1.Text = dOtherAmount.ToString("c");
                            lblPayTerm1.Text = strOtherValue;

                            lblAmountC.Text = dOtherAmount.ToString("c");
                            lblPayTermC.Text = strOtherValue;

                        }
                        if (dBalanceDueAmount != 0)
                        {
                            nTermId = 3100 + Convert.ToInt32(hdnCOId.Value);
                            strTerms += strBalanceDueValue + ": " + dBalanceDueAmount.ToString("c") + "</br>";

                            lblamount.Text = dBalanceDueAmount.ToString("c");
                            txtAmount.Text = dBalanceDueAmount.ToString("c");
                            lblPayterm.Text = strBalanceDueValue;
                            lblamount1.Text = dBalanceDueAmount.ToString("c");
                            txtAmount1.Text = dBalanceDueAmount.ToString("c");
                            lblPayTerm1.Text = strBalanceDueValue;

                            lblAmountC.Text = dBalanceDueAmount.ToString("c");
                            lblPayTermC.Text = strBalanceDueValue;

                        }
                        if (dUponCompletionAmount != 0)
                        {
                            nTermId = 2100 + Convert.ToInt32(hdnCOId.Value);
                            strTerms += strUponCompletionValue + ": " + dUponCompletionAmount.ToString("c") + "</br>";

                            lblamount.Text = dUponCompletionAmount.ToString("c");
                            txtAmount.Text = dUponCompletionAmount.ToString("c");
                            lblPayterm.Text = strUponCompletionValue;
                            lblamount1.Text = dUponCompletionAmount.ToString("c");
                            txtAmount1.Text = dUponCompletionAmount.ToString("c");
                            lblPayTerm1.Text = strUponCompletionValue;

                            lblAmountC.Text = dUponCompletionAmount.ToString("c");
                            lblPayTermC.Text = strUponCompletionValue;
                        }
                        if (dUponSignAmount != 0)
                        {
                            nTermId = 1100 + Convert.ToInt32(hdnCOId.Value);
                            strTerms += strUponSignValue + ": " + dUponSignAmount.ToString("c") + "</br>";
                            lblamount.Text = dUponSignAmount.ToString("c");
                            txtAmount.Text = dUponSignAmount.ToString("c");
                            lblPayterm.Text = strUponSignValue;
                            lblamount1.Text = dUponSignAmount.ToString("c");
                            txtAmount1.Text = dUponSignAmount.ToString("c");
                            lblPayTerm1.Text = strUponSignValue;

                            lblAmountC.Text = dUponSignAmount.ToString("c");
                            lblPayTermC.Text = strUponSignValue;
                        }



                        lblterms.Text = strTerms;
                    }
                    else
                    {
                        nTermId = 100 + Convert.ToInt32(hdnCOId.Value);
                        lblterms.Text = cho.payment_terms;

                        lblamount.Text = grandtotal.ToString("c");
                        txtAmount.Text = grandtotal.ToString("c");
                        lblPayterm.Text = lblChangeOrderName.Text;
                        lblamount1.Text = grandtotal.ToString("c");
                        txtAmount1.Text = grandtotal.ToString("c");
                        lblPayTerm1.Text = lblChangeOrderName.Text;

                        lblAmountC.Text = grandtotal.ToString("c");
                        lblPayTermC.Text = lblChangeOrderName.Text;

                    }

                    hdnPayTermId.Value = nTermId.ToString(); // Add terms Id in hidden field
                    int nStatusId = Convert.ToInt32(cho.change_order_status_id);
                    hdnCOStatusId.Value = nStatusId.ToString();

                    if (nStatusId == 1)
                        lblCoStatus.Text = "Draft";
                    else if (nStatusId == 2)
                        lblCoStatus.Text = "Pending";
                    else if (nStatusId == 3)
                        lblCoStatus.Text = "Executed";
                    else if (nStatusId == 4)
                        lblCoStatus.Text = "Declined";
                }

                // Change Order Status
                customerchangeorderstatus objccos = new customerchangeorderstatus();
                // Data not exist
                if (_db.customerchangeorderstatus.Where(c => c.customerid == nCid && c.estimateid == nEstid && c.changeorderid == nChEstid).SingleOrDefault() == null)
                {
                    objccos.customerid = nCid;
                    objccos.estimateid = nEstid;
                    objccos.changeorderid = nChEstid;
                    objccos.status = 1; // Pending
                    //objccos.client_id = Convert.ToInt32(hdnClientId.Value);

                    _db.customerchangeorderstatus.InsertOnSubmit(objccos);
                    _db.SubmitChanges();

                    // rdo Pending will be Selected

                    hdnID.Value = objccos.id.ToString();


                }
                else
                {
                    // Data exist
                    objccos = _db.customerchangeorderstatus.Single(c => c.customerid == nCid && c.estimateid == nEstid && c.changeorderid == nChEstid);

                    hdnID.Value = objccos.id.ToString();

                    rdoStatus.SelectedValue = objccos.status.ToString();

                    if (objccos.status == 1)
                    {
                        pnlAccept.Visible = false;
                        pnlReject.Visible = false;
                        pnlAcceptCreditedCO.Visible = false;
                    }
                    else if (objccos.status == 2)
                    {
                        rdoStatus.Enabled = false;
                        pnlAccept.Visible = false;
                        pnlReject.Visible = false;
                        pnlAcceptCreditedCO.Visible = false;
                        btnAcceptDOWN.Visible = false;

                    }
                    else if (objccos.status == 4)
                    {
                        rdoStatus.Enabled = false;
                        tblNewPayment.Visible = false;
                        pnlAccept.Visible = false;
                        pnlReject.Visible = false;
                        pnlAcceptCreditedCO.Visible = false;
                        pnlAcceptA.Visible = false;
                        txtReject.Text = "";
                    }
                    else if (objccos.status == 5)
                    {
                        rdoStatus.Enabled = false;
                        pnlAcceptA.Visible = false;
                        pnlAccept.Visible = false;
                        pnlReject.Visible = false;
                        pnlAcceptCreditedCO.Visible = false;

                        txtReject.Text = "";
                    }
                    else
                    {
                        rdoStatus.Enabled = false;
                        pnlAccept.Visible = false;
                        pnlReject.Visible = true;
                        txtReject.Text = objccos.causeforreject;
                        txtReject.Enabled = false;
                        btnReject.Visible = false;

                    }

                }



                if (Convert.ToInt32(hdnCOStatusId.Value) > 2)
                {
                    BindSelectedItemGrid_Direct_executed();
                    BindSelectedItemGrid_executed();
                }
                else
                {
                    BindSelectedItemGrid();
                    BindSelectedItemGrid_Direct();
                }

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

                CalculateTotal();
            }
            catch(Exception ex)
            {
                lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }

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

    public void BindSelectedItemGrid()
    {
        try
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
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    public void BindSelectedItemGrid_Direct()
    {
        try
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
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    public void BindSelectedItemGrid_executed()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            if (rdoSort.SelectedValue == "2")
            {
                strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            }
            else
            {
                strQ = "select DISTINCT change_order_pricing_list.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list  INNER JOIN location on location.location_id = change_order_pricing_list.location_id where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            }
            List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
            grdGrouping.DataSource = mList;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    public void BindSelectedItemGrid_Direct_executed()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            if (rdoSort.SelectedValue == "2")
            {
                strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            }
            else
            {
                strQ = "select DISTINCT change_order_pricing_list.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list  INNER JOIN location on location.location_id = change_order_pricing_list.location_id where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            }
            List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
            grdGroupingDirect.DataSource = mList;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected string GetTotalPrice()
    {
        string str = "Payement Terms: </br>";
        return str + lblterms.Text;
    }
    protected string GetTax()
    {
        return "Tax: " + Tax.ToString("c");
    }
    protected string GetGrandTotalPrice()
    {
        return " Grand Total: " + grandtotal.ToString("c");
    }

    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {

                int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);

                GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
                int nDirectId = 1;
                if (Convert.ToInt32(hdnCOStatusId.Value) > 2)
                {
                    GetData_ReadOnly(colId, gv, nDirectId);
                }
                else
                {
                    GetData(colId, gv, nDirectId);
                }
                GridViewRow footerRow = gv.FooterRow;
                GridViewRow headerRow = gv.HeaderRow;
                foreach (GridViewRow row in gv.Rows)
                {
                    Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                    Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                    Label lblSubTotalLabel11 = footerRow.FindControl("lblSubTotalLabel11") as Label;
                    Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                    subtotal += Double.Parse((row.FindControl("lblViewEcon") as Label).Text.Replace("$", ""));
                    labelTotal.Text = subtotal.ToString("c");
                    if (rdoSort.SelectedValue == "1")
                    {
                        lblHeader.Text = "Section";
                    }
                    else
                    {
                        lblHeader.Text = "Location";
                    }
                    if (Convert.ToBoolean(hdnChangeOrderView.Value) == false)
                        lblSubTotalLabel.Text = "Sub Total:";
                    else
                        lblSubTotalLabel11.Text = "Sub Total:";
                }

                double taxRate = Convert.ToDouble(lbltax.Text);
                Tax += subtotal * (taxRate / 100);

                tTotal += subtotal;

                grandtotal = tTotal + Tax;
                subtotal = 0.0;
            }
            catch (Exception ex)
            {
                lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }
        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            if (rdoSort.SelectedValue == "1")
            {

                var price_detail = from p in _db.co_pricing_masters
                                   join lc in _db.locations on p.location_id equals lc.location_id
                                   where (from clc in _db.changeorder_locations
                                          where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) 
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
                    co_obj1.chage_order_id = Convert.ToInt32(hdnCOId.Value);
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
                    if (_db.change_order_pricing_lists.Where(pd => pd.chage_order_id == Convert.ToInt32(hdnCOId.Value) && pd.co_pricing_list_id == objcl1.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value)).ToList().Count > 0)
                    {
                        change_order_pricing_list copl = new change_order_pricing_list();
                        copl = _db.change_order_pricing_lists.Single(pd => pd.chage_order_id == Convert.ToInt32(hdnCOId.Value) && pd.co_pricing_list_id == objcl1.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value));
                        co_obj1.EconomicsId = (int)copl.EconomicsId;
                        co_obj1.EconomicsCost = (decimal)copl.EconomicsCost;
                    }
                    else
                    {
                        co_obj1.EconomicsId = 0;
                        co_obj1.EconomicsCost = 0;
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
                                          where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) 
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
                    co_obj.chage_order_id = Convert.ToInt32(hdnCOId.Value);
                    co_obj.location_id = objcl.location_id;
                    co_obj.sales_person_id = objcl.sales_person_id;
                    co_obj.section_level = objcl.section_level;
                    co_obj.item_id = objcl.item_id;
                    co_obj.section_name = objcl.section_name;
                    co_obj.item_name = objcl.item_name;
                    co_obj.is_direct = objcl.is_direct;
                    co_obj.item_status_id = objcl.item_status_id;
                    co_obj.section_serial = objcl.section_serial;
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
                    if (_db.change_order_pricing_lists.Where(pd => pd.chage_order_id == Convert.ToInt32(hdnCOId.Value) && pd.co_pricing_list_id == objcl.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value)).ToList().Count > 0)
                    {
                        change_order_pricing_list copl = new change_order_pricing_list();
                        copl = _db.change_order_pricing_lists.Single(pd => pd.chage_order_id == Convert.ToInt32(hdnCOId.Value) && pd.co_pricing_list_id == objcl.co_pricing_list_id && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value));
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
                grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
                grd.DataBind();

            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged");
            if (Convert.ToInt32(hdnCOStatusId.Value) > 2)
            {
                BindSelectedItemGrid_Direct_executed();
                BindSelectedItemGrid_executed();
            }
            else
            {
                BindSelectedItemGrid();
                BindSelectedItemGrid_Direct();
            }

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
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            GridView gv1 = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToBoolean(hdnChangeOrderView.Value) == false)
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
                    //LinkButton btn = (LinkButton)e.Row.Cells[8].Controls[0];
                    //btn.Text = "Undo";
                    //btn.CommandName = "Undo";



                }
                else if (nItemStatusId == 3)
                {
                    e.Row.Attributes.CssStyle.Add("color", "green");
                    //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
                }


            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }


    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {


            GridView gv2 = (GridView)sender;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Convert.ToBoolean(hdnChangeOrderView.Value) == false)
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

                }
                else if (nItemStatusId == 3)
                {
                    e.Row.Attributes.CssStyle.Add("color", "green");
                    e.Row.Attributes.CssStyle.Add("font-weight", "bold");
                }


            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {


                int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);

                int nDirectId = 2;

                GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;

                if (Convert.ToInt32(hdnCOStatusId.Value) > 2)
                {
                    GetData_ReadOnly(colId, gv, nDirectId);
                }
                else
                {
                    GetData(colId, gv, nDirectId);
                }
                GridViewRow footerRow = gv.FooterRow;
                GridViewRow headerRow = gv.HeaderRow;
                foreach (GridViewRow row in gv.Rows)
                {

                    Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                    Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                    Label lblSubTotalLabel21 = footerRow.FindControl("lblSubTotalLabel21") as Label;
                    Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                    subtotal_diect += Double.Parse((row.FindControl("lblViewEcon1") as Label).Text.Replace("$", ""));
                    labelTotal2.Text = subtotal_diect.ToString("c");
                    if (rdoSort.SelectedValue == "1")
                    {
                        lblHeader2.Text = "Section";
                    }
                    else
                    {
                        lblHeader2.Text = "Location";
                    }
                    if (Convert.ToBoolean(hdnChangeOrderView.Value) == false)
                        lblSubTotalLabel2.Text = "Sub Total:";
                    else
                        lblSubTotalLabel21.Text = "Sub Total:";
                }
                subtotal_diect = 0.0;

            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }



    private void CalculateTotal()
    {
        try
        {
            double totalPrice = 0;
            foreach (GridViewRow dimaster1 in grdGrouping.Rows)
            {
                GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                foreach (GridViewRow di in grdSelectedItem1.Rows)
                {
                    string strNewPrice = ((Label)di.FindControl("lblViewEcon")).Text.Replace("$", "").Replace("(", "-").Replace(")", "");
                    double price = 0;

                    if (strNewPrice.Length > 0)
                    {
                        price = Convert.ToDouble(strNewPrice);
                        totalPrice += price;
                    }


                }
            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected void rdoStatus_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoStatus.ID, rdoStatus.GetType().Name, "SelectedIndexChanged");
            lblAcceptMessage.Text = "";
            lblRejectMessage.Text = "";
            lblCreditedResult.Text = "";

            if (rdoStatus.SelectedValue == "1")
            {
                pnlAccept.Visible = false;
                pnlReject.Visible = false;
                pnlAcceptA.Visible = false;
                pnlAcceptCreditedCO.Visible = false;

                txtReject.Text = "";
            }
            else if (rdoStatus.SelectedValue == "2")
            {
                tblNewPayment.Visible = true;
                pnlAccept.Visible = true;
                pnlReject.Visible = false;
                pnlAcceptA.Visible = false;
                pnlAcceptCreditedCO.Visible = false;

                txtReject.Text = "";
            }
            else if (rdoStatus.SelectedValue == "4")
            {
                tblNewPayment.Visible = false;
                pnlAccept.Visible = false;
                pnlReject.Visible = false;
                pnlAcceptCreditedCO.Visible = false;
                pnlAcceptA.Visible = true;
                txtReject.Text = "";
            }
            else if (rdoStatus.SelectedValue == "5")
            {
                pnlAcceptA.Visible = false;
                pnlAccept.Visible = false;
                pnlReject.Visible = false;

                pnlAcceptCreditedCO.Visible = true;

                txtReject.Text = "";
            }
            else
            {
                pnlAcceptCreditedCO.Visible = false;
                pnlAcceptA.Visible = false;
                pnlAccept.Visible = false;
                pnlReject.Visible = true;
            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnReject.ID, btnReject.GetType().Name, "Click");
            lblAcceptMessage.Text = "";
            lblRejectMessage.Text = "";

            if (txtReject.Text.Trim() == "")
            {
                lblRejectMessage.Text = csCommonUtility.GetSystemErrorMessage("Please input reason for reject.");
                return;
            }

            DataClassesDataContext _db = new DataClassesDataContext();

            customerchangeorderstatus objccos = new customerchangeorderstatus();
            objccos = _db.customerchangeorderstatus.Single(c => c.id == Convert.ToInt32(hdnID.Value));

            string strQ = "UPDATE customerchangeorderstatus SET status = 3, causeforreject ='" + txtReject.Text + "' , rejectdate = '" + DateTime.Now + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimateid =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorderid =" + Convert.ToInt32(hdnCOId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            _db.SubmitChanges();

            rdoStatus.Enabled = false;
            txtReject.Enabled = false;
            btnReject.Visible = false;

            lblRejectMessage.Text = csCommonUtility.GetSystemMessage("Change order is rejected.");
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected void btnAcceptDOWN_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAcceptDOWN.ID, btnAcceptDOWN.GetType().Name, "Click");
            lblAcceptMessage.Text = "";

            DataClassesDataContext _db = new DataClassesDataContext();

            customerchangeorderstatus objccos = new customerchangeorderstatus();
            objccos = _db.customerchangeorderstatus.Single(c => c.id == Convert.ToInt32(hdnID.Value));

            string strQ = "UPDATE customerchangeorderstatus SET status = 2, accepteddate = '" + DateTime.Now + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimateid =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorderid =" + Convert.ToInt32(hdnCOId.Value);

            string strQ1 = "";
            string MasterAdd_strQ = "";
            string MasterDel_strQ = "";
            bool IsClose = false;
            if (Convert.ToInt32(rdoStatus.SelectedValue) == 2)
            {
                IsClose = true;

                strQ1 = "UPDATE changeorder_estimate SET change_order_status_id = 3,execute_date='" + DateTime.Today + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Now + "' WHERE chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id = 1 WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ1, string.Empty);
                _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
                _db.ExecuteCommand(MasterDel_strQ, string.Empty);

            }
            else if (Convert.ToInt32(rdoStatus.SelectedValue) == 4)
            {
                IsClose = true;
                strQ = "UPDATE customerchangeorderstatus SET status = 4, accepteddate = '" + DateTime.Now + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimateid =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorderid =" + Convert.ToInt32(hdnCOId.Value);

                strQ1 = "UPDATE changeorder_estimate SET change_order_status_id = 3,execute_date='" + DateTime.Today + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Now + "' WHERE chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id = 1 WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ1, string.Empty);
                _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
                _db.ExecuteCommand(MasterDel_strQ, string.Empty);

            }
            else if (Convert.ToInt32(rdoStatus.SelectedValue) == 3)
            {
                IsClose = true;
                strQ1 = "UPDATE changeorder_estimate SET change_order_status_id = 4,execute_date='" + DateTime.Today + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Today + "' WHERE chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id=1,total_retail_price=prev_total_price WHERE item_status_id = 2 AND is_direct=1 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                string MasterAdd_direct_strQ = "UPDATE co_pricing_master SET item_status_id = 1,total_direct_price=prev_total_price WHERE item_status_id = 2 AND is_direct=2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ1, string.Empty);
                _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
                _db.ExecuteCommand(MasterAdd_direct_strQ, string.Empty);
                _db.ExecuteCommand(MasterDel_strQ, string.Empty);
            }


            _db.ExecuteCommand(strQ, string.Empty);
            _db.SubmitChanges();

            rdoStatus.Enabled = false;

            btnAcceptDOWN.Visible = false;


            string strToEmail = string.Empty;
            if (lblSalesPersonEmail.Text.Length > 4)
                strToEmail = lblSalesPersonEmail.Text;
            if (hdnSuperandentEmail.Value.Length > 4)
            {
                if (strToEmail.Length > 4)
                    strToEmail += ", " + hdnSuperandentEmail.Value;
                else
                    strToEmail = hdnSuperandentEmail.Value;
            }
            string strChangeOrdersEmail = hdnCCEmail.Value;
            string strCCEmail = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");


            if (strToEmail.Length > 4)
            {
                string[] strIds = strToEmail.Split(',');
                foreach (string strId in strIds)
                {
                    Match match1 = regex.Match(strId.Trim());
                    if (!match1.Success)
                    {
                        strToEmail += lblComProfilelEmail.Text + ",";

                    }
                }
            }
            else
            {
                strToEmail = lblComProfilelEmail.Text;

            }

            if (strChangeOrdersEmail.Length > 4)
            {
                string[] strCCIds = strChangeOrdersEmail.Split(',');
                foreach (string strCCId in strCCIds)
                {
                    Match match1 = regex.Match(strCCId.Trim());
                    if (match1.Success)
                    {
                        strCCEmail += strCCId + ",";

                    }
                }
            }
            strCCEmail = strCCEmail.TrimEnd(',');
            strToEmail = strToEmail.TrimEnd(',');

            Match match = regex.Match(strToEmail);
           
            if (strToEmail.Length > 4)
            {
                EmailAPI email = new EmailAPI();
                customeruserinfo obj = new customeruserinfo();
                string strUser = "";
                string strFrom = "alyons@azinteriorinnovations.com";
                int ProtocolType = 1;

                if ((customeruserinfo)Session["oCustomerUser"] != null)
                {
                    obj = (customeruserinfo)Session["oCustomerUser"];
                    strUser = obj.customerusername;
                }

                email.From = strFrom;

                email.To = strToEmail.Trim();

                if (strCCEmail.Length > 4)
                    email.CC = strCCEmail;

                email.BCC = "iisupport@faztrack.com, tislam@faztrack.com";

                email.Subject = lblLastName.Text + "- Change Order Accepted on " + DateTime.Now.ToShortDateString();

                email.Body = CreateHtml();

                email.UserName = strUser;

                email.IsSaveEmailInDB = true;

                email.ProtocolType = ProtocolType;

                email.SendEmail();
            }




            lblAcceptMessage.Text = csCommonUtility.GetSystemMessage("Change order accepted successfully.");
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    private void GetData_ReadOnly(int colId, GridView grd, int nDirectId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            if (rdoSort.SelectedValue == "1")
            {
                List<change_order_pricing_list> CO_pricing_list = _db.change_order_pricing_lists.Where(cl => cl.location_id == colId && cl.is_direct == nDirectId && cl.chage_order_id == Convert.ToInt32(hdnCOId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
                grd.DataSource = CO_pricing_list;
                grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
                grd.DataBind();
            }
            else
            {
                List<change_order_pricing_list> CO_pricing_list = _db.change_order_pricing_lists.Where(cl => cl.section_level == colId && cl.is_direct == nDirectId && cl.chage_order_id == Convert.ToInt32(hdnCOId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
                grd.DataSource = CO_pricing_list;
                grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
                grd.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }
    string CreateHtml()
    {
        string TABLE = "";
        string strName = "";
        if (rdoSort.SelectedValue == "1")
        {
            strName = "Section";
        }
        else
        {
            strName = "Location";
        }

        TABLE = "<table cellpadding='0' cellspacing='2' width='100%' style='margin: 0 auto;font-family:Arial, Helvetica, sans-serif;'>" +
                "<tr style='color:#444;text-decoration: none; font-weight:bold; font-size:16px; background-color:#eee; height:30px;'>" +
                "<td style='height:40px;' align='center' width='34%'>Customer: " + lblCustomerName.Text + " approved the following change order (" + lblChangeOrderName.Text + ")</td></tr>" +
                "<tr><td align='center' width='10px'></td></tr>" +
            // Invoice Items
                "<tr><td align='center' width='98%'>" +
                    "<table style='border:1px solid #c1c1c1;' cellpadding='5' cellspacing='0' width='100%'>" +
                        "<tr style='color: #021bcf; font-size: 14px; font-weight: bold; text-align:center; background-color:#f1f1f1; height:32px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Item Id</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>SL</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + strName + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Item Name</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Short Notes</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Ext.Price</td></tr>";
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            Label Label1 = (Label)dimaster1.FindControl("Label1");

            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 16px; font-weight: bold;text-align:center; background-color:#fff; height:40px;'><td colspan='6' style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + Label1.Text + "</td></tr>";

            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                Label lblItemName = (Label)di.FindControl("lblItemName");
                Label lblShort = (Label)di.FindControl("lblShort");
                Label lblViewEcon = (Label)di.FindControl("lblViewEcon");


                int nItemStatusId = Convert.ToInt32(grdSelectedItem1.DataKeys[di.RowIndex].Values[1]);
                if (nItemStatusId == 2) //BABU
                {
                    TABLE += "<tr style='border: 1px solid #c1c1c1; color: #c80000; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[0].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[1].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + lblItemName.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[3].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + lblShort.Text + "&nbsp;" + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblViewEcon.Text + "</td></tr>";
                }
                else if (nItemStatusId == 3)
                {
                    TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal; text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[0].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[1].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + lblItemName.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[3].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + lblShort.Text + "&nbsp;" + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblViewEcon.Text + "</td></tr>";
                }

            }
        }
        if (grdGroupingDirect.Rows.Count > 0)
        {
            foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
            {
                Label Label2 = (Label)dimaster2.FindControl("Label2");

                TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 16px; font-weight: bold;text-align:center; background-color:#fff; height:40px;'><td colspan='6' style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + Label2.Text + "</td></tr>";

                GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
                foreach (GridViewRow di in grdSelectedItem2.Rows)
                {
                    Label lblItemName2 = (Label)di.FindControl("lblItemName2");
                    Label lblShort1 = (Label)di.FindControl("lblShort1");
                    Label lblViewEcon1 = (Label)di.FindControl("lblViewEcon1");
                    int nItemStatusId = Convert.ToInt32(grdSelectedItem2.DataKeys[di.RowIndex].Values[1]);
                    if (nItemStatusId == 2) //BABU
                    {
                        TABLE += "<tr style='border: 1px solid #c1c1c1; color: #c80000; font-size: 14px; font-weight: normal; text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[0].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[1].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + lblItemName2.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[3].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + lblShort1.Text + "&nbsp;" + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblViewEcon1.Text + "</td></tr>";
                    }
                    else if (nItemStatusId == 3)
                    {
                        TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal; text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[0].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[1].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + lblItemName2.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[3].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + lblShort1.Text + "&nbsp;" + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblViewEcon1.Text + "</td></tr>";
                    }

                }
            }
        }
        TABLE += "</table>" +
        "</td></tr>" +
        "<tr><td align='center' width='10px'></td></tr>" +
            // Footer Items
            "<tr><td align='center' width='98%'>" +
                "<table style='background-color: #F5F5F5;border: 1px solid #DDDDDD; box-shadow: 0 0 2px #EEEEEE; margin: 0 auto; font-size:14px; color: #555;' cellpadding='5' cellspacing='0' width='100%'>" +
            "<tr><td align='center' width='10px'></td></tr>" +
            "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Payment Term: </b><br/>" + lblterms.Text + "</td></tr>" +
    "</table>";

        return TABLE;
    }


    private void GetCardLists()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        PaymentProfile objPP = new PaymentProfile();

        try
        {
            var item = from pp in _db.PaymentProfiles
                       where pp.CustomerId == Convert.ToInt32(hdnCustomerId.Value)
                       select new csCreditCard
                       {
                           PaymentProfileId = (int)pp.PaymentProfileId,
                           CustomerId = (int)pp.CustomerId,
                           AuthorisedPaymentId = pp.AuthorisedPaymentId,
                           AuthorisedCustomerId = (int)pp.AuthorisedCustomerId,
                           CardNumber = pp.CardNumber,
                           NameOnCard = pp.NameOnCard,
                           CardType = pp.CardType,
                           ExpirationDate = (DateTime)pp.ExpirationDate,
                           CreateDate = (DateTime)pp.CreateDate,
                           LastUpdatedDate = (DateTime)pp.LastUpdatedDate,
                           LastUpdatedBy = (string)pp.LastUpdatedBy
                       };

            if (item.Count() > 0)
            {
                grdCardList.DataSource = item;
                grdCardList.DataKeyNames = new string[] { "PaymentProfileId", "CustomerId", "CardNumber", "CardType" };
                grdCardList.DataBind();

                DataTable dt = csCommonUtility.LINQToDataTable(item);
                Session.Add("sCardList", dt);
            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);// "Change order accepted successfully.";
        }
    }
    protected void grdCardList_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCardList.ID, grdCardList.GetType().Name, "Sorting");
            lblAcceptMessage.Text = "";
            lblReason.Text = "";
            DataTable dtCardList = (DataTable)Session["sCardList"];

            string strShort = e.SortExpression + " " + hdnOrder.Value;

            DataView dv = dtCardList.DefaultView;
            dv.Sort = strShort;
            Session["sCardList"] = dv.ToTable();

            if (hdnOrder.Value == "ASC")
                hdnOrder.Value = "DESC";
            else
                hdnOrder.Value = "ASC";
            GetCardLists();
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdCardList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                int cPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[0]);
                int nCustomerId = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[1]);
                int nCardNumber = Convert.ToInt32(grdCardList.DataKeys[e.Row.RowIndex].Values[2]);
                string strCardType = grdCardList.DataKeys[e.Row.RowIndex].Values[3].ToString();

                System.Web.UI.WebControls.Image imggrdCardType = (System.Web.UI.WebControls.Image)e.Row.FindControl("imggrdCardType");
                imggrdCardType.ImageUrl = "~/Images/" + strCardType.ToUpper() + ".png";
                imggrdCardType.AlternateText = strCardType;

                Label lblgrdCreditCard = (Label)e.Row.FindControl("lblgrdCreditCard");
                lblgrdCreditCard.Text = strCardType + " ending in " + nCardNumber;

                Label lblgrdExpirationDatee = (Label)e.Row.FindControl("lblgrdExpirationDatee");
                if (DateTime.Now > Convert.ToDateTime(lblgrdExpirationDatee.Text))
                {
                    lblgrdExpirationDatee.Text = "Expired <br/>" + Convert.ToDateTime(lblgrdExpirationDatee.Text).ToString("MM/yyyy");
                    lblgrdExpirationDatee.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblgrdExpirationDatee.Text = Convert.ToDateTime(lblgrdExpirationDatee.Text).ToString("MM/yyyy");
                    lblgrdExpirationDatee.ForeColor = System.Drawing.Color.Black;
                }

            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void chkNewCard_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkNewCard.ID, chkNewCard.GetType().Name, "CheckedChanged");
            if (chkNewCard.Checked)
            {
                pnlNewCard.Visible = true;
                if (grdCardList.Rows.Count > 0)
                {
                    foreach (GridViewRow di in grdCardList.Rows)
                    {
                        RadioButton rdoSelect = (RadioButton)di.FindControl("rdoSelect");
                        if (rdoSelect.Checked)
                        {
                            rdoSelect.Checked = false;
                        }

                    }


                }
            }
            else
            {
                pnlNewCard.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }
    protected void btnFinalizePayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnFinalizePayment.ID, btnFinalizePayment.GetType().Name, "Click"); 
        PaymentProfile payPro = new PaymentProfile();
        AuthorizeAPI api = new AuthorizeAPI();
        lblReason.Text = "";
        lblAcceptMessage.Text = "";
        bool cCheck = false;
        bool bFound = false;
        decimal nAmount = 0;
        string nTermId = hdnPayTermId.Value;//100 + Convert.ToInt32(hdnCOId.Value);
        string strPayTermId = nTermId.ToString();
        string strPayTermDesc = lblChangeOrderName.Text;
        string sTranId = "";
        if (txtCardHolderName.Text == "")
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is a required field.");
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is a required field.");
            txtCardHolderName.Focus();
            return;
        }
        if (txtCardHolderName.Text.Trim().Length > 20)
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is allowed maximum 20 characters.");
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Card holder name is allowed maximum 20 characters.");
            txtCardHolderName.Focus();
            return;
        }
        if (txtAmount.Text.Trim().Length == 0)
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Amount is a required field.");
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Amount is a required field.");// "Change order accepted successfully.";
            return;
        }
        else
        {

            try
            {
                nAmount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Amount must be number.");
                lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Amount must be number."); // "Change order accepted successfully.";
                return;

            }
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        valCheckOut.Visible = true;

        foreach (GridViewRow item in grdCardList.Rows)
        {
            RadioButton rdo = (RadioButton)item.FindControl("rdoSelect");
            if (rdo.Checked)
            {
                bFound = true;
                int nPaymentProfileId = Convert.ToInt32(grdCardList.DataKeys[item.RowIndex].Values[0]);
                int nCustomerId = Convert.ToInt32(grdCardList.DataKeys[item.RowIndex].Values[1]);

                payPro = _db.PaymentProfiles.SingleOrDefault(p => p.PaymentProfileId == nPaymentProfileId && p.CustomerId == nCustomerId);

                if (payPro != null)
                {
                    try
                    {

                        sTranId = api.ChargeCustomerProfile(payPro.AuthorisedCustomerId.ToString(), payPro.AuthorisedPaymentId.ToString(), nAmount);
                        if (sTranId.Length > 0)
                        {
                            cCheck = true;

                        }
                    }
                    catch (Exception ex)
                    {
                        lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                        lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

                        return;

                    }
                }
            }
        }


        if (bFound == false)
        {
            if (!_checkCreditCardInformation())
                return;

            if (valCheckOut.IsValid == true)
            {
                try
                {
                    customer objcust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    string sOrderNo = DateTime.Now.ToString("yyyyMMddHHmmssff"); //CommonUtility.GetOrderNumber();


                    decimal total_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));



                    sTranId = api.ChargeCreditCard(txtCreditCardNumber.Text.Trim(), ddlMonth.SelectedItem.Text.Trim() + ddlYear.SelectedValue.Trim(), txtCVV.Text, total_amount, txtCardHolderName.Text.Trim(), objcust.email, txtAddress.Text, txtCity.Text, ddlState.SelectedItem.Text, txtZip.Text);


                    if (sTranId.Length > 0)
                    {
                        cCheck = true;

                    }



                }
                catch (Exception ex)
                {
                    lblReason.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                    lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                    return;

                }
            }
            else
            {
                //valSummary.Visible = true;
                valCheckOut.Text = "";
            }

        }


        if (cCheck)
        {
            try
            {
                // Insert Customer

                string strOrderNote = string.Empty;

                string strServiceName = string.Empty;

                string strCCN = txtCreditCardNumber.Text.Trim().ToString();

                string BillAddress = string.Empty;
                string BillCity = string.Empty;
                string BillState = string.Empty;
                string BillZip = string.Empty;

                string strCardType = string.Empty;

                if (strCCN.Length > 3)
                {
                    strCCN = strCCN.Substring((strCCN.Length - 4), 4);
                    strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
                    BillAddress = txtAddress.Text;
                    BillCity = txtCity.Text;
                    BillState = ddlState.SelectedValue;
                    BillZip = txtZip.Text;

                }
                else
                {
                    strCCN = payPro.CardNumber.ToString();
                    BillAddress = payPro.BillAddress;
                    BillCity = payPro.BillCity;
                    BillState = payPro.BillState;
                    BillZip = payPro.BillZip;
                    strCardType = payPro.CardType;
                }
                New_partial_payment pay_cost = new New_partial_payment();

                // pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);

                pay_cost.pay_term_ids = strPayTermId;
                pay_cost.pay_term_desc = strPayTermDesc;
                pay_cost.client_id = Convert.ToInt32(hdnClientId.Value);
                pay_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                pay_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                pay_cost.pay_type_id = 3;
                pay_cost.reference = strCCN;
                pay_cost.pay_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")); ;
                pay_cost.pay_date = DateTime.Now;
                pay_cost.create_date = DateTime.Now;
                pay_cost.TransactionId = sTranId;
                pay_cost.CreditCardType = strCardType;
                pay_cost.CreditCardNum = strCCN;
                _db.New_partial_payments.InsertOnSubmit(pay_cost);
                _db.SubmitChanges();
                int payId = pay_cost.payment_id;
                hdnPayId.Value = payId.ToString();
                // Send Email to New Customer
                //Email.SendMailtoCustomer( Convert.ToInt32(hdnCustomerId.Value));



                // Insert Payment
                Payments_card_Info objPay = new Payments_card_Info();

                objPay.PaymentMethodID = 0; //rdCreditCardType.SelectedIndex + 1;
                objPay.payment_id = payId;
                objPay.Date = DateTime.Now;
                objPay.CreditCardNum = strCCN;

                if (chkNewCard.Checked)
                {
                    objPay.CardHoldersName = txtCardHolderName.Text;

                    objPay.CreditCardExpDate = Convert.ToDateTime(ddlMonth.SelectedValue + "/1/" + ddlYear.SelectedItem.Text);
                }
                else
                {
                    objPay.CardHoldersName = payPro.NameOnCard;

                    objPay.CreditCardExpDate = payPro.ExpirationDate;

                }

                objPay.CreditCardAuthNum = "";
                objPay.Amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                objPay.Notes = "";
                objPay.Message = "";
                objPay.Approval = "";
                objPay.IsEmailSent = false;
                objPay.TransactionId = sTranId;
                objPay.BillAddress = BillAddress;
                objPay.BillCity = BillCity;
                objPay.client_id = Convert.ToInt32(hdnClientId.Value);
                objPay.BillState = BillState;
                objPay.BillZip = BillZip;
                _db.Payments_card_Infos.InsertOnSubmit(objPay);
                _db.SubmitChanges();

                //Save card info after transaction

                if (_db.PaymentProfiles.Where(pp => pp.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && pp.NameOnCard == objPay.CardHoldersName && pp.CardNumber == objPay.CreditCardNum && pp.CardType == strCardType).Count() == 0)
                {
                    if (chkSaveCardInfo.Checked)
                    {
                        SaveCustomerProfile(Convert.ToInt32(hdnPayId.Value));

                    }
                }
               SendEmailToCustomer(Convert.ToInt32(hdnCustomerId.Value), Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")), strPayTermDesc, strCardType, strCCN);


                GetCardLists();

                if (grdCardList.Rows.Count > 0)
                {
                    pnlExistCard.Visible = true;
                    pnlNewCard.Visible = false;
                    chkNewCard.Visible = true;
                    chkNewCard.Checked = false;
                }
                else
                {
                    pnlExistCard.Visible = false;
                    pnlNewCard.Visible = true;
                    chkNewCard.Checked = true;
                    chkNewCard.Visible = false;
                }
                txtAmount.Text = "";
                lblPayterm.Text = "";
                txtCreditCardNumber.Text = "";
                txtCVV.Text = "";
                ddlMonth.SelectedIndex = 0;
                ddlYear.SelectedIndex = 0;
                tblNewPayment.Visible = false;
                btnAcceptDOWN_Click(sender, e);
                lblReason.Text = csCommonUtility.GetSystemMessage("This change oder has been excuted with successful payment");
                lblAcceptMessage.Text = csCommonUtility.GetSystemMessage("This change oder has been excuted with successful payment");
            }
            catch (Exception ex)
            {
                Session.RemoveAll();
                Session.Clear();
                lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
                lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }

        }
        else
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage("Transaction fail...." + Environment.NewLine + lblReason.Text + "");
            lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage("Transaction fail...." + Environment.NewLine + lblReason.Text + "");

        }



    }

    private bool _checkCreditCardInformation()
    {
        lblReason.Text = "";
        if (txtCreditCardNumber.Text == "")
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Credit Card Number is a required field");

            return false;
        }
     

        if (ddlMonth.SelectedIndex == 0)
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry month");
            return false;
        }
        if (ddlYear.SelectedIndex == 0)
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("Select expiry year");
            return false;
        }
        if (txtCVV.Text == "")
        {
            lblReason.Text = csCommonUtility.GetSystemRequiredMessage("CVV is a required field");
            return false;
        }
        return true;
    }

    private void SaveCustomerProfile(int nPaymentId)
    {

        try
        {
            AuthorizeAPI api = new AuthorizeAPI();
            DataClassesDataContext _db = new DataClassesDataContext();

            PaymentProfile objPP = new PaymentProfile();


            Payments_card_Info cardInfo = _db.Payments_card_Infos.Single(p => p.payment_id == nPaymentId);

            int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            string nCardNumber = cardInfo.CreditCardNum;
            string strNameOnCard = cardInfo.CardHoldersName;
            string strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
            string strMonth = ddlMonth.SelectedItem.Text.Trim();
            string strYear = ddlYear.SelectedItem.Text.Trim();

            if (_db.PaymentProfiles.Where(pp => pp.CustomerId == nCustomerId && pp.NameOnCard == strNameOnCard && pp.CardNumber == nCardNumber && pp.CardType == strCardType).Count() > 0)
            {
                return;
            }

            objPP.CustomerId = nCustomerId;
            objPP.AuthorisedPaymentId = "0";
            objPP.AuthorisedCustomerId = 0;
            objPP.CardNumber = nCardNumber;
            objPP.NameOnCard = strNameOnCard;
            objPP.CardType = strCardType;
            objPP.ExpirationDate = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), 1);
            objPP.CreateDate = DateTime.Now;
            objPP.LastUpdatedDate = DateTime.Now;
            objPP.LastUpdatedBy = "";// hdnCustomerLastName.Value.ToString();

            objPP.BillAddress = txtAddress.Text;
            objPP.BillCity = txtCity.Text;
            objPP.BillState = ddlState.SelectedValue;
            objPP.BillZip = txtZip.Text;
            objPP.client_id = Convert.ToInt32(hdnClientId.Value);

            List<string> sArray = api.CreateCustomerProfileFromTransaction(cardInfo.TransactionId);

            if (sArray.Count > 1)
            {
                objPP.AuthorisedCustomerId = Convert.ToInt32(sArray[0]);
                objPP.AuthorisedPaymentId = sArray[1];
                _db.PaymentProfiles.InsertOnSubmit(objPP);
                _db.SubmitChanges();
            }
            txtCreditCardNumber.Text = "";
            ddlYear.SelectedIndex = -1;
            ddlMonth.SelectedIndex = -1;

        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    string body(string fullName, string amount, string strTerms, string strPMName, string strCCNum, string companyName, string website)
    {




        string body = @"<div marginwidth='0' marginheight='0' style='font:14px/20px 'Helvetica',Arial,sans-serif;margin:0;padding:75px 0 0 0;text-align:center;background-color:#330f02'>
    <center>
       <table border='0' cellpadding='20' cellspacing='0' height='100%' width='100%' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624bodyTable' style='background-color:#330f02'>
          <tbody>
             <tr>
                <td align='center' valign='top'>
                   <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;border-radius:6px;background-color:none' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624templateContainer'>
                      <tbody>
                         <tr>
                            <td align='center' valign='top'>
                               <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px'>
                                  <tbody>
                                     <tr>
                                        <td style='margin: 0 auto; text-align: center'>
                                           <h1 style='font-size:24px;line-height:100%;margin-bottom:30px;margin-top:0;padding:0'>
                                             
                                                <img src='https://ii.faztrack.com/assets/II_EMAIL_LOGO.png' alt='' border='0' width='50%'  ></a>
                                           </h1>
                                        </td>
                                     </tr>
                                  </tbody>
                               </table>
                            </td>
                         </tr>
                         <tr>
                            <td align='center' valign='top'>
                               <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width:600px;border-radius:6px;background-color:#d6dfdb' id='m_1588011772617908220m_3406656465879084808m_-7536552561093049624templateBody'>
                                  <tbody>
                                     <tr>
                                        <td align='left' valign='top' style='line-height:110%;font-family:Verdana;font-size:14px;color:#333333;padding:20px'>
                                           <div>Dear " + fullName + ",</div> " + Environment.NewLine + Environment.NewLine +
                                           "<p style='line-height:20px'>Your payment of <b>" + amount + "</b> for <b>" + strTerms + "</b> using your <b>" + strPMName + "</b> ending in <b>" + strCCNum + "</b> was processed.</p>" + Environment.NewLine + Environment.NewLine +
                                           "<p>Please click <a target='_blank' href='" + csCommonUtility.GetProjectUrl() + "/customerlogin.aspx'> here </a> to view your current project status.</p>" + Environment.NewLine +
                                           "<p>Should you have any question please do not hesitate to contact us.</p>" + Environment.NewLine + Environment.NewLine;


        body += @"<p>Sincerely,</p>" + Environment.NewLine +
                "<p>" + companyName + "</p>" + Environment.NewLine +
                "<p>" + website + "</p>";

        body += @"</td>
                                         </tr>
                                      </tbody>
                                   </table>
                                </td>
                             </tr>
                             <tr>
                                <td align='center' valign='top'>
                                   <table border='0' cellpadding='20' cellspacing='0' width='100%' style='max-width:600px'>
                                      <tbody>
                                         <tr>
                                            <td align='center' valign='top'>
                                            </td>
                                         </tr>
                                      </tbody>
                                   </table>
                                </td>
                             </tr>
                          </tbody>
                       </table>
                    </td>
                 </tr>
              </tbody>
           </table>
        </center>
    
     </div>";

        return body;


    }


    private void SendEmailToCustomer(int nCustomerId, decimal amount, string strTerms, string strPMName, string strCCNum)
    {
        // Email To Group shohid

        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = new customer();
        objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
        customeruserinfo objcu = new customeruserinfo();
        objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(c => c.client_id == Convert.ToInt32(hdnClientId.Value));

        string strTable = "";
        strTable = body(objCust.first_name1 + " " + objCust.last_name1, Convert.ToDecimal(amount).ToString("c"), strTerms, strPMName, strCCNum, oCom.company_name, oCom.website);

       

        string strToEmail = objCust.email;
        string FromEmail = oCom.email;

        string strChangeOrdersEmail = hdnCCEmail.Value;
        string strCCEmail = "";

        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        if (strChangeOrdersEmail.Length > 4)
        {
            string[] strCCIds = strChangeOrdersEmail.Split(',');
            foreach (string strCCId in strCCIds)
            {
                Match match1 = regex.Match(strCCId.Trim());
                if (match1.Success)
                {
                    strCCEmail += strCCId + ",";

                }
            }
        }
        strCCEmail = strCCEmail.TrimEnd(',');

       

        Match match = regex.Match(strToEmail);
        
        if (strToEmail.Length > 4)
        {
            EmailAPI email = new EmailAPI();
            customeruserinfo obj = new customeruserinfo();
            string strUser = "";
            string strFrom = "alyons@azinteriorinnovations.com";
            int ProtocolType = 1;

            if ((customeruserinfo)Session["oCustomerUser"] != null)
            {
                obj = (customeruserinfo)Session["oCustomerUser"];
                strUser = obj.customerusername;
            }

            email.From = strFrom;

            email.To = strToEmail.Trim();
            if (strCCEmail.Length > 4)
                email.CC = strCCEmail;
            email.BCC = "iisupport@faztrack.com, tislam@faztrack.com";
            // email.BCC = strCCEmail;

            email.Subject = "Your payment with Arizona's Interior Innovations";

            email.Body = strTable;

            email.UserName = strUser;

            email.IsSaveEmailInDB = true;

            email.ProtocolType = ProtocolType;

            email.SendEmail();
        }

           

    }
    protected void txtCreditCardNumber_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtCreditCardNumber.ID, txtCreditCardNumber.GetType().Name, "TextChanged"); 
        lblReason.Text = "";
        if (txtCreditCardNumber.Text.Trim().Length > 5)
        {
            AuthorizeAPI api = new AuthorizeAPI();
            string strCardType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString();
            imgCardType.ImageUrl = "~/Images/" + strCardType.ToUpper() + ".png";
            imgCardType.AlternateText = strCardType;

           
            chkSaveCardInfo.Visible = true;
        }
        else
        {
            chkSaveCardInfo.Visible = false;
        }
    }
    protected void txtAmount_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtAmount.ID, txtAmount.GetType().Name, "TextChanged"); 
        string str = txtAmount.Text.Replace("$", "");
        lblamount.Text = "$" + str;
    }
    protected void valCheckOut_ServerValidate(object source, ServerValidateEventArgs args)
    {
        valCheckOut.ErrorMessage = "";

        args.IsValid = true;

        if (args.IsValid)
        {
            args.IsValid = false;

            //credit card validation
            AuthorizeAPI api = new AuthorizeAPI();
            string cType = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString(); // credit card type
            //string cType = rdCreditCardType.SelectedValue;	// credit card type
            string cNumber = this.txtCreditCardNumber.Text;
            int cLength = cNumber.Length;	// credit card number length

            switch (cType)
            {
                case "VISA":	//visa
                    if (cNumber.StartsWith("4") && (cLength == 13 || cLength == 16))
                        args.IsValid = true;
                    break;

                case "MasterCard":	//master card
                    if (cNumber.StartsWith("5") && cLength == 16)
                        args.IsValid = true;
                    break;

                case "Amex":
                    if ((cNumber.StartsWith("34") || cNumber.StartsWith("37")) && cLength == 15)
                        args.IsValid = true;
                    break;
            

                case "Discover":	//discover
                    if (cNumber.StartsWith("6011") && cLength == 16)
                        args.IsValid = true;
                    break;

                case "DinersClub":	//diner's club
                    if ((cNumber.StartsWith("30") || cNumber.StartsWith("36") || cNumber.StartsWith("38")) &&
                        (cLength == 14 || cLength == 10))
                    {
                        args.IsValid = true;
                    }
                    break;

                case "CB":	//carte blanc
                    if ((cNumber.StartsWith("30") || cNumber.StartsWith("38")) && cLength == 10)
                        args.IsValid = true;
                    break;

                case "TP":	//uatp
                    if (cNumber.StartsWith("1") && cLength == 15)
                        args.IsValid = true;
                    break;

                case "JCB":
                    if ((cNumber.StartsWith("3088") || cNumber.StartsWith("3096") || cNumber.StartsWith("3112") ||
                        cNumber.StartsWith("3158") || cNumber.StartsWith("3337") || cNumber.StartsWith("3528")) && cLength == 16)
                    {
                        args.IsValid = true;
                    }
                    break;

                default:
                    break;
            }


            if (args.IsValid == false) valCheckOut.ErrorMessage = "Invalid credit card number for selected card type.";
        }

        // check security digit ONLY when credit card number is valid
        if (args.IsValid)
        {
            args.IsValid = ValidateSecurityDigits();
            if (args.IsValid == false) valCheckOut.ErrorMessage += "<br><li>Invalid security digits.</li>";
        }
        if (ddlMonth.SelectedValue == "0")
        {
            args.IsValid = false;
            valCheckOut.ErrorMessage += "<br><li>Exp. Month is a required field.</li>";
        }
        if ((ddlYear.SelectedIndex == 0) || (Convert.ToInt32(ddlYear.SelectedItem.Text.ToString()) < System.DateTime.Now.Year))
        {
            args.IsValid = false;
            valCheckOut.ErrorMessage += "<br><li>Exp. Year is a required field.</li>";
        }
    }
    private bool ValidateSecurityDigits()
    {
        // default to true, since user might accidentally turn on the security digits box, when the selected credit card type
        // does not require security digits
        bool isValid = true;

        //string ctype = rdCreditCardType.SelectedValue;
        AuthorizeAPI api = new AuthorizeAPI();
        string ctype = api.GetCardType(txtCreditCardNumber.Text.Trim()).ToString(); // credit card type
        int securityDigitLength = txtCVV.Text.Length;

        switch (ctype.ToUpper())
        {
            case "VI":
            case "MC":
            case "DS":
                if (securityDigitLength != 3)
                    isValid = false;
                break;

            case "AX":
                if (securityDigitLength != 4)
                    isValid = false;
                break;
            default:
                isValid = true;
                break;
        }
        return isValid;
    }
    protected void btnEcheckPayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnEcheckPayment.ID, btnEcheckPayment.GetType().Name, "Click"); 
        PaymentProfile payPro = new PaymentProfile();

        lblAcceptMessage.Text = "";
        lblReason0.Text = "";
        bool cCheck = false;
        decimal nAmount = 0;

        string nTermId = hdnPayTermId.Value;//100 + Convert.ToInt32(hdnCOId.Value);
        string strPayTermId = nTermId.ToString();
        string strPayTermDesc = lblChangeOrderName.Text;
        string sTranId = "";


        if (txtAmount1.Text.Trim().Length == 0)
        {
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Amount is a required field.");

            return;
        }
        else
        {

            try
            {
                nAmount = Convert.ToDecimal(txtAmount1.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
                lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Amount must be number.");
                return;

            }
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        //  if (chkPaymentECheck.Checked == true)
        if (rdoStatus.SelectedItem.Value == "4")
        {
            if (!_checkECheckInformation())
                return;


            try
            {
                customer objcust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                string sOrderNo = DateTime.Now.ToString("yyyyMMddHHmmssff"); //CommonUtility.GetOrderNumber();


                decimal total_amount = Convert.ToDecimal(txtAmount1.Text.Replace("$", string.Empty));

                AuthorizeAPI api = new AuthorizeAPI();

                sTranId = api.ChargeECheck(txtBank_acct_num.Text.Trim(), txtBank_aba_code.Text.Trim(), txtBank_acct_name.Text.Trim(), txtbank_name.Text.Trim(), nAmount);
                if (sTranId.Length > 0)
                {
                    cCheck = true;

                }


            }
            catch (Exception ex)
            {
                lblAcceptMessage.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                lblReason0.Text = csCommonUtility.GetSystemErrorMessage("The transaction is unsuccessful. Reason: " + ex.Message);
                return;

            }
        }


        if (cCheck)
        {
            try
            {

                New_partial_payment pay_cost = new New_partial_payment();

                // pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);

                pay_cost.pay_term_ids = strPayTermId;
                pay_cost.pay_term_desc = strPayTermDesc;
                pay_cost.client_id = 1;
                pay_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                pay_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                pay_cost.pay_type_id = 2;
                pay_cost.reference = "Check";
                pay_cost.pay_amount = Convert.ToDecimal(txtAmount1.Text.Replace("$", "").Replace("(", "-").Replace(")", "")); ;
                pay_cost.pay_date = DateTime.Now;
                pay_cost.create_date = DateTime.Now;
                pay_cost.TransactionId = sTranId;
                pay_cost.CreditCardType = "";
                pay_cost.CreditCardNum = "";
                _db.New_partial_payments.InsertOnSubmit(pay_cost);
                _db.SubmitChanges();
                int payId = pay_cost.payment_id;
                hdnPayId.Value = payId.ToString();


                // Insert Payment
                PaymentECheckInfo objEPay = new PaymentECheckInfo();

                objEPay.payment_id = payId;
                objEPay.Date = DateTime.Now;
                objEPay.AccountNumber = txtBank_acct_num.Text.Trim();
                objEPay.RoutingNumber = txtBank_aba_code.Text.Trim();
                objEPay.Amount = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                objEPay.AccountType = "Checking";
                objEPay.EcheckType = "WEB";
                objEPay.NameOnAccount = txtBank_acct_name.Text.Trim();
                objEPay.BankName = txtbank_name.Text.Trim();

                _db.PaymentECheckInfos.InsertOnSubmit(objEPay);
                _db.SubmitChanges();
                string strAccountNum = txtBank_acct_num.Text.Trim().Substring((txtBank_acct_num.Text.Trim().Length - 3), 3);

                SendEcheckEmailToCustomer(Convert.ToInt32(hdnCustomerId.Value), Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")), strPayTermDesc, strAccountNum, txtbank_name.Text.Trim());

                txtAmount1.Text = "";
                lblPayterm.Text = "";
                txtBank_acct_num.Text = "";
                txtBank_aba_code.Text = "";
                txtBank_acct_name.Text = "";
                txtbank_name.Text = "";

                pnlECheckPayment.Visible = false;
                btnAcceptDOWN_Click(sender, e);
                lblReason0.Text = csCommonUtility.GetSystemMessage("This change oder has been excuted with successful payment");
                lblAcceptMessage.Text = csCommonUtility.GetSystemMessage("This change oder has been excuted with successful payment");
            }
            catch (Exception ex)
            {
                Session.RemoveAll();
                Session.Clear();
                lblReason0.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }

        }
        else
        {
            lblReason0.Text = csCommonUtility.GetSystemErrorMessage("Transaction fail...." + Environment.NewLine + lblReason0.Text + "");

        }



    }
    private bool _checkECheckInformation()
    {
        lblAcceptMessage.Text = "";

        if (txtBank_acct_name.Text == "")
        {
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Bank Account Name is a required field");
            return false;
        }
        if (txtBank_acct_num.Text == "")
        {
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Bank account number is a required field");
            return false;
        }
        if (txtbank_name.Text == "")
        {
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Bank Name is a required field");
            return false;
        }
        if (txtBank_aba_code.Text == "")
        {
            lblAcceptMessage.Text = csCommonUtility.GetSystemRequiredMessage("Bank Routing Number is a required field");

            return false;
        }




        return true;
    }

    private void SendEcheckEmailToCustomer(int nCustomerId, decimal amount, string strTerms, string strAccountNum, string strBankNamae)
    {
        // Email To Group shohid

        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = new customer();
        objCust = _db.customers.Single(c => c.customer_id == nCustomerId);
        customeruserinfo objcu = new customeruserinfo();
        objcu = _db.customeruserinfos.Single(cu => cu.customerid == nCustomerId);

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(c => c.client_id == Convert.ToInt32(hdnClientId.Value));

        string strTable = "<table align='center' width='704px' border='0'>" + Environment.NewLine +
                "<tr><td align='left'>Dear " + objCust.first_name1 + " " + objCust.last_name1 + ",</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Your payment of  <b>" + Convert.ToDecimal(amount).ToString("c") + "</b> for <b>" + strTerms + "</b> using your bank account of <b>" + strBankNamae + "</b> bank, account number ending in <b>" + strAccountNum + "</b> was processed.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Please click <a target='_blank' href='https://ii.faztrack.com/customerlogin.aspx'> here </a> to view your current project status.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Should you have any question please do not hesitate to contact us.</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr><tr><td align='left'></td></tr>" + Environment.NewLine +
                "<tr><td align='left'>Sincerely,</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>" + oCom.company_name + "</td></tr>" + Environment.NewLine +
                "<tr><td align='left'>" + oCom.website + "</td></tr>" + Environment.NewLine +
                "<tr><td align='left'></td></tr></table>";

        string strToEmail = objCust.email;
        string FromEmail = oCom.email;

        string strChangeOrdersEmail = hdnCCEmail.Value;
        string strCCEmail = "";

        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        if (strChangeOrdersEmail.Length > 4)
        {
            string[] strCCIds = strChangeOrdersEmail.Split(',');
            foreach (string strCCId in strCCIds)
            {
                Match match1 = regex.Match(strCCId.Trim());
                if (match1.Success)
                {
                    strCCEmail += strCCId + ",";

                }
            }
        }
        strCCEmail = strCCEmail.TrimEnd(',');

        Match match = regex.Match(strToEmail);
        if (!match.Success)
            strToEmail = "";
        if (strToEmail.Length > 4)
        {
            EmailAPI email = new EmailAPI();
            customeruserinfo obj = new customeruserinfo();
            string strUser = "";
            string strFrom = "alyons@azinteriorinnovations.com";
            int ProtocolType = 1;

            if ((customeruserinfo)Session["oCustomerUser"] != null)
            {
                obj = (customeruserinfo)Session["oCustomerUser"];
                strUser = obj.customerusername;
            }

            email.From = strFrom;

            email.To = strToEmail.Trim();
            if (strCCEmail.Length > 4)
                email.CC = strCCEmail;

            email.BCC = "iisupport@faztrack.com, tislam@faztrack.com";
            // email.BCC = strCCEmail;

            email.Subject = "Your payment with Arizona's Interior Innovations";

            email.Body = strTable;

            email.UserName = strUser;

            email.IsSaveEmailInDB = true;

            email.ProtocolType = ProtocolType;

            email.SendEmail();
        }

            //Previous Email

        //Match match = regex.Match(strToEmail);
        //if (!match.Success)
        //    strToEmail = "";

        //if (strToEmail.Length > 4)
        //{
        //    MailMessage msg = new MailMessage();
        //    if (oCom.email.Length > 4)
        //        msg.From = new MailAddress(oCom.email);
        //    else
        //        msg.From = new MailAddress("info@interiorinnovations.biz");

        //    msg.To.Add(strToEmail.Trim());
        //    if (strCCEmail.Length > 4)
        //        msg.Bcc.Add(strCCEmail);
        //    msg.Bcc.Add("tislam007@gmail.com");
        //    msg.Subject = "Your payment with Interior Innovations";
        //    msg.IsBodyHtml = true;
        //    msg.Body = strTable;
        //    msg.Priority = MailPriority.High;

        //    try
        //    {
        //        csCommonUtility.SendByLocalhost(msg);
        //        //SmtpClient smtp = new SmtpClient();
        //        //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
        //        //smtp.Send(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblReason0.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        //        return;
        //    }

        //}

    }
    protected void btnAcceptCreditedCO_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAcceptCreditedCO.ID, btnAcceptCreditedCO.GetType().Name, "Click");
            lblCreditedResult.Text = "";

            if (Convert.ToDecimal(lblAmountC.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) <= 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();

                customerchangeorderstatus objccos = new customerchangeorderstatus();
                objccos = _db.customerchangeorderstatus.Single(c => c.id == Convert.ToInt32(hdnID.Value));

                string strQ = "UPDATE customerchangeorderstatus SET status = 5, accepteddate = '" + DateTime.Now + "' WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimateid =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorderid =" + Convert.ToInt32(hdnCOId.Value);

                string strQ1 = "";
                string MasterAdd_strQ = "";
                string MasterDel_strQ = "";
                bool IsClose = false;
                if (Convert.ToInt32(rdoStatus.SelectedValue) == 5)
                {
                    IsClose = true;

                    strQ1 = "UPDATE changeorder_estimate SET change_order_status_id = 3,execute_date='" + DateTime.Today + "',is_close='" + IsClose + "', last_updated_date='" + DateTime.Now + "' WHERE chage_order_id =" + Convert.ToInt32(hdnCOId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                    MasterAdd_strQ = "UPDATE co_pricing_master SET item_status_id = 1 WHERE item_status_id = 3 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                    MasterDel_strQ = "Delete co_pricing_master WHERE item_status_id = 2 AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                    _db.ExecuteCommand(strQ1, string.Empty);
                    _db.ExecuteCommand(MasterAdd_strQ, string.Empty);
                    _db.ExecuteCommand(MasterDel_strQ, string.Empty);

                }


                _db.ExecuteCommand(strQ, string.Empty);
                _db.SubmitChanges();

                rdoStatus.Enabled = false;

                btnAcceptCreditedCO.Visible = false;
                string strToEmail = string.Empty;
                if (lblSalesPersonEmail.Text.Length > 4)
                    strToEmail = lblSalesPersonEmail.Text;
                if (hdnSuperandentEmail.Value.Length > 4)
                {
                    if (strToEmail.Length > 4)
                        strToEmail += ", " + hdnSuperandentEmail.Value;
                    else
                        strToEmail = hdnSuperandentEmail.Value;
                }
                string strChangeOrdersEmail = hdnCCEmail.Value;
                string strCCEmail = "";

                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");


                if (strToEmail.Length > 4)
                {
                    string[] strIds = strToEmail.Split(',');
                    foreach (string strId in strIds)
                    {
                        Match match1 = regex.Match(strId.Trim());
                        if (!match1.Success)
                        {
                            strToEmail+=lblComProfilelEmail.Text+",";

                        }
                    }
                }
                else
                {
                    strToEmail = lblComProfilelEmail.Text;

                }

                if (strChangeOrdersEmail.Length > 4)
                {
                    string[] strCCIds = strChangeOrdersEmail.Split(',');
                    foreach (string strCCId in strCCIds)
                    {
                        Match match1 = regex.Match(strCCId.Trim());
                        if (match1.Success)
                        {
                            strCCEmail += strCCId + ",";

                        }
                    }
                }
                strCCEmail = strCCEmail.TrimEnd(',');


                strToEmail = strToEmail.TrimEnd(',');

                Match match = regex.Match(strToEmail);  // shohid

                if (strToEmail.Length > 4)
                {
                    EmailAPI email = new EmailAPI();
                    customeruserinfo obj = new customeruserinfo();
                    string strUser = "";
                    string strFrom = "alyons@azinteriorinnovations.com";
                    int ProtocolType = 1;

                    if ((customeruserinfo)Session["oCustomerUser"] != null)
                    {
                        obj = (customeruserinfo)Session["oCustomerUser"];
                        strUser = obj.customerusername;
                    }

                    email.From = strFrom;

                    email.To = strToEmail.Trim();

                    if (strCCEmail.Length > 4)
                        email.CC = strCCEmail;

                    email.BCC = "iisupport@faztrack.com, tislam@faztrack.com";

                    email.Subject = lblLastName.Text + "- Change Order Accepted on " + DateTime.Now.ToShortDateString();

                    email.Body = CreateHtml();

                    email.UserName = strUser;

                    email.IsSaveEmailInDB = true;

                    email.ProtocolType = ProtocolType;

                    email.SendEmail();

                    //Previous Email

                    //MailMessage msg = new MailMessage();
                    //msg.From = new MailAddress("info@interiorinnovations.biz", "InteriorInnovations (system msg: do not reply)");
                    //msg.To.Add(strToEmail);
                    //if (strCCEmail.Length > 4)
                    //    msg.CC.Add(strCCEmail);
                    //msg.Bcc.Add("avijit019@gmail.com, tislam007@gmail.com");
                    //msg.Subject = lblLastName.Text + "- Change Order Accepted on " + DateTime.Now.ToShortDateString();
                    //msg.IsBodyHtml = true;
                    //msg.Body = CreateHtml();
                    //msg.Priority = MailPriority.High;

                    //try
                    //{
                    //    csCommonUtility.SendByLocalhost(msg);
                    //    //SmtpClient smtp = new SmtpClient();
                    //    //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                    //    //smtp.Send(msg);
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw (ex);
                    //}


                    lblCreditedResult.Text = csCommonUtility.GetSystemMessage("Change order accepted successfully.");
                }
                
            }
            else
            {
                lblCreditedResult.Text = csCommonUtility.GetSystemRequiredMessage("This accept option only for Credited Change order, Please accept by another option.");

            }
        }
        catch (Exception ex)
        {
            lblReason.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
}
