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
public partial class change_order_worksheet_readonly : System.Web.UI.Page
{
    private double subtotal = 0.0;
    //private double grandtotal = 0.0;
    string strDetails = "";

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
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            int nChEstid = Convert.ToInt32(Request.QueryString.Get("coestid"));
            hdnChEstId.Value = nChEstid.ToString();
            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            if (com != null)
            {
                hdnChangeOrderView.Value = com.ChangeQtyView.ToString();
            }
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
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
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
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
                    ddlStatus.SelectedValue = cho.change_order_status_id.ToString();
                    ddlChangeOrderType.SelectedValue = cho.change_order_type_id.ToString();
                    txtComments.Text = cho.comments;
                    if (cho.payment_terms != "")
                        ddlTerms.SelectedItem.Text = cho.payment_terms.ToString();
                    if (cho.payment_terms == "Other")
                    {
                        txtOtherTerms.Text = cho.other_terms.ToString();
                        //lblOther.Visible = true;
                        //txtOtherTerms.Visible = true;
                    }
                    rdoList.SelectedValue = cho.is_total.ToString();
                    ChkIsTax.Checked = Convert.ToBoolean(cho.is_tax);
                    lblTax.Text = cho.tax.ToString();
                    lblTax.ToolTip = cho.tax.ToString();
                    if (cho.total_payment_due != "")
                        ddlTotalHeader.SelectedItem.Text = cho.total_payment_due.ToString();
                    txtChangeOrderDate.Text = cho.changeorder_date.ToString();
                    txtNotes1.Text = cho.notes1.ToString();
                    if (cho.change_order_status_id == 3)
                    {
                        lblMessagefinal.Text = csCommonUtility.GetSystemMessage("This Change Order has executed on " + Convert.ToDateTime(cho.last_updated_date).ToShortDateString());

                    }
                    else
                    {
                        lblMessagefinal.Text = csCommonUtility.GetSystemErrorMessage("This Change Order has declined on " + Convert.ToDateTime(cho.last_updated_date).ToShortDateString());

                    }
                }

                BindSelectedItemGrid_executed();
                BindSelectedItemGrid_Direct_executed();

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
            CalculateTotal();

            // CO Payment Term
            // if (_db.Co_PaymentTerms.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(hdnChEstId.Value) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
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
        }
        else
        {
            //EnableDesable();
            CalculateTotal();
        }
    }

    //private void EnableDesable()
    //{
    //    foreach (GridViewRow dimaster1 in grdGrouping.Rows)
    //    {
    //        GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
    //        foreach (GridViewRow di in grdSelectedItem1.Rows)
    //        {
    //            int nItemStatusId = Convert.ToInt32(grdSelectedItem1.DataKeys[di.RowIndex].Values[1]);
    //            Label lblTotal_price = (Label)di.FindControl("lblTotal_price");
    //            DropDownList ddlEconomics = (DropDownList)di.FindControl("ddlEconomics");
    //            TextBox txtViewEcon = (TextBox)di.FindControl("txtViewEcon");
    //            if (ddlEconomics.SelectedValue.Trim().Length > 0)
    //            {
    //                decimal Cost = Convert.ToDecimal(lblTotal_price.Text);

    //                if (Convert.ToInt32(ddlEconomics.SelectedValue) == 5)
    //                {
    //                    txtViewEcon.Enabled = true;

    //                }
    //                else
    //                {
    //                    txtViewEcon.Enabled = false;

    //                    if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 1)
    //                    {
    //                        txtViewEcon.Text = "0";
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 0)
    //                    {
    //                        txtViewEcon.Text = "0";
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 2)
    //                    {
    //                        if (nItemStatusId == 2)
    //                            txtViewEcon.Text = (Cost * -1).ToString("0.00");
    //                        else
    //                            txtViewEcon.Text = Cost.ToString("0.00");

    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 3)
    //                    {
    //                        txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 20 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 4)
    //                    {
    //                        txtViewEcon.Text = Convert.ToDecimal(Cost * 2).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 6)
    //                    {
    //                        txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 30 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 7)
    //                    {
    //                        txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 50 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics.SelectedValue) == 8)
    //                    {
    //                        txtViewEcon.Text = Convert.ToDecimal(Cost + Cost * 70 / 100).ToString("0.00");
    //                    }

    //                }
    //            }

    //        }
    //        GridViewRow footerRow = grdSelectedItem1.FooterRow;
    //        GridViewRow headerRow = grdSelectedItem1.HeaderRow;
    //        foreach (GridViewRow row in grdSelectedItem1.Rows)
    //        {
    //            Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
    //            Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
    //            Label lblHeader = headerRow.FindControl("lblHeader") as Label;
    //            subtotal += Double.Parse((row.FindControl("txtViewEcon") as TextBox).Text);
    //            labelTotal.Text = subtotal.ToString("c");
    //            if (rdoSort.SelectedValue == "1")
    //            {
    //                lblHeader.Text = "Section";
    //            }
    //            else
    //            {
    //                lblHeader.Text = "Location";
    //            }
    //            lblSubTotalLabel.Text = "Sub Total:";
    //        }
    //        subtotal = 0.0;
    //    }
    //    foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
    //    {
    //        GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
    //        foreach (GridViewRow di in grdSelectedItem2.Rows)
    //        {
    //            Label lblTotal_price2 = (Label)di.FindControl("lblTotal_price2");
    //            DropDownList ddlEconomics1 = (DropDownList)di.FindControl("ddlEconomics1");
    //            TextBox txtViewEcon1 = (TextBox)di.FindControl("txtViewEcon1");
    //            if (ddlEconomics1.SelectedValue.Trim().Length > 0)
    //            {
    //                decimal Cost = Convert.ToDecimal(lblTotal_price2.Text);

    //                if (Convert.ToInt32(ddlEconomics1.SelectedValue) == 5)
    //                {
    //                    txtViewEcon1.Enabled = true;

    //                }
    //                else
    //                {
    //                    txtViewEcon1.Enabled = false;

    //                    if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 1)
    //                    {
    //                        txtViewEcon1.Text = "0";
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 0)
    //                    {
    //                        txtViewEcon1.Text = "0";
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 2)
    //                    {
    //                        txtViewEcon1.Text = Cost.ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 3)
    //                    {
    //                        txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 20 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 4)
    //                    {
    //                        txtViewEcon1.Text = Convert.ToDecimal(Cost * 2).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 6)
    //                    {
    //                        txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 30 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 7)
    //                    {
    //                        txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 50 / 100).ToString("0.00");
    //                    }
    //                    else if (Convert.ToDecimal(ddlEconomics1.SelectedValue) == 8)
    //                    {
    //                        txtViewEcon1.Text = Convert.ToDecimal(Cost + Cost * 70 / 100).ToString("0.00");
    //                    }

    //                }
    //            }

    //        }
    //        GridViewRow footerRow = grdSelectedItem2.FooterRow;
    //        GridViewRow headerRow = grdSelectedItem2.HeaderRow;
    //        foreach (GridViewRow row in grdSelectedItem2.Rows)
    //        {
    //            Label labelTotal = footerRow.FindControl("lblSubTotal2") as Label;
    //            Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel2") as Label;
    //            Label lblHeader = headerRow.FindControl("lblHeader2") as Label;
    //            subtotal += Double.Parse((row.FindControl("txtViewEcon1") as TextBox).Text);
    //            labelTotal.Text = subtotal.ToString("c");
    //            if (rdoSort.SelectedValue == "1")
    //            {
    //                lblHeader.Text = "Section";
    //            }
    //            else
    //            {
    //                lblHeader.Text = "Location";
    //            }
    //            lblSubTotalLabel.Text = "Sub Total:";
    //        }
    //        subtotal = 0.0;
    //    }

    //}

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
        Response.Redirect("customerlist.aspx");
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



    public void BindSelectedItemGrid_executed()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT change_order_pricing_list.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list  INNER JOIN location on location.location_id = change_order_pricing_list.location_id where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        grdGrouping.DataSource = mList;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

    }
    public void BindSelectedItemGrid_Direct_executed()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT change_order_pricing_list.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list  INNER JOIN location on location.location_id = change_order_pricing_list.location_id where change_order_pricing_list.chage_order_id =" + Convert.ToInt32(hdnChEstId.Value) + " AND change_order_pricing_list.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND change_order_pricing_list.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND change_order_pricing_list.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        lblResult1.Text = "";
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int locationId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[2]);
            string section_name = gv1.DataKeys[e.Row.RowIndex].Values[3].ToString();
            Label lblItemName = (Label)e.Row.FindControl("lblItemName");
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
            if (rdoSort.SelectedValue == "1")
            {
                lblItemName.Text = section_name;
            }
            else
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                location loc = new location();
                loc = _db.locations.Single(c => c.location_id == locationId);
                lblItemName.Text = loc.location_name;
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

            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }


        }

    }


    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }

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
            List<change_order_pricing_list> CO_pricing_list = _db.change_order_pricing_lists.Where(cl => cl.location_id == colId && cl.is_direct == nDirectId && cl.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
            grd.DataSource = CO_pricing_list;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "location_id", "section_name" };
            grd.DataBind();
        }
        else
        {
            List<change_order_pricing_list> CO_pricing_list = _db.change_order_pricing_lists.Where(cl => cl.section_level == colId && cl.is_direct == nDirectId && cl.chage_order_id == Convert.ToInt32(hdnChEstId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
            grd.DataSource = CO_pricing_list;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "location_id", "section_name" };
            grd.DataBind();
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged"); 
        BindSelectedItemGrid_executed();
        BindSelectedItemGrid_Direct_executed();
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
            lblTax.Text = "0";

        }
        else
        {
            lblTax.Text = lblTax.ToolTip;
        }
        if (Convert.ToDecimal(lblTax.Text) == 0)
        {
            lblRate1.Visible = false;
            lblTax.Visible = false;
            lblPer.Visible = false;
            lblTotal1.Visible = false;
            lblGtotal.Visible = false;
            ChkIsTax.Visible = false;

        }
        else
        {
            lblRate1.Visible = true;
            lblTax.Visible = true;
            lblPer.Visible = true;
            lblTotal1.Visible = true;
            lblGtotal.Visible = true;
            ChkIsTax.Visible = true;
        }

        decimal taxRate = Convert.ToDecimal(lblTax.Text);
        tax = totalPrice * (taxRate / 100);

        lblGtotal.Text = Convert.ToDecimal(totalPrice + tax).ToString("c");



    }

    protected void btnCOReport_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCOReport.ID, btnCOReport.GetType().Name, "Click"); 
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
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
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

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }
    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
    protected void btnGotoChangeOrderList_Click(object sender, EventArgs e)
    {
        Response.Redirect("changeorderlist.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value);
    }
    protected void btnViewHTML_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewHTML.ID, btnViewHTML.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = " SELECT  co_pricing_list_id, customer_id, estimate_id, chage_order_id, change_order_pricing_list.location_id, sales_person_id, section_level, section_serial, item_id, section_name, item_name, total_direct_price, total_retail_price, is_direct, item_status_id, EconomicsId, EconomicsCost, create_date, last_update_date,location_name FROM change_order_pricing_list  " +
                    " INNER JOIN location ON change_order_pricing_list.location_id=location.location_id AND change_order_pricing_list.client_id=location.client_id " +
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
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
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

    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Submit Button");

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
            lblChangeOrderName.Text = strNewName;
            lblExistingChangeOrderName.Text = strNewName;
        }
        catch (Exception ex)
        {
            lblResult1.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

        // Response.Redirect("changeorder_pricing.aspx?eid=" + nestId + "&cid=" + ncid + "&coestid=" + coId);
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Close Button");

        lblMessage.Text = "";
        txtNewChangeOrderName.Text = "";
        modUpdateCoEstimate.Hide();
    }

}


