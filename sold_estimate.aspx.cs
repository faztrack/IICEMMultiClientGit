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
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class sold_estimate : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    private double subtotalCost = 0.0;
    private double grandtotalCost = 0.0;
    private double subtotal_diectCost = 0.0;
    private double grandtotal_directCost = 0.0;
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo obj = (userinfo)Session["oUser"];

                hdnEmailType.Value = obj.EmailIntegrationType.ToString();

                int nRoleId = obj.role_id;
                if (nRoleId == 4 || nRoleId == 1)
                {
                    chkPMDisplay.Visible = true;
                    lblPm.Visible = true;
                }
                else
                {
                    chkPMDisplay.Visible = false;
                    lblPm.Visible = false;

                }

            }
            if (Page.User.IsInRole("admin036") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSuperintendent();
            btnUpdateSuperintendent.Visible = false;
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);
            
            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                hdnLastName.Value = cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                hdnClientId.Value = cust.client_id.ToString();

                if (cust.SuperintendentId != null && cust.SuperintendentId != 0)
                {
                    string strSuperintendent = "";
                    ListItem dditem = ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString());
                    if (dditem != null)
                        this.ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString()).Selected = true;
                    else
                    {
                        user_info uinfo = _db.user_infos.Single(u => u.user_id == cust.SuperintendentId);
                        if (uinfo != null)
                        {
                            strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                            ddlSuperintendent.Items.Insert(0, new ListItem(strSuperintendent, cust.SuperintendentId.ToString()));
                        }
                    }
                }

                int nSalesPersonId = Convert.ToInt32(cust.sales_person_id);

                if (_db.sales_persons.Any(c => c.sales_person_id == nSalesPersonId && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(nSalesPersonId));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                estimate_payment est_pay = new estimate_payment();

                est_pay = _db.estimate_payments.Where(ep => ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).FirstOrDefault();
                if (est_pay != null)
                {
                    if (est_pay.tax_rate != null)
                    {
                        lblTax.Text = est_pay.tax_rate.ToString();
                    }
                }
                if (lblTax.Text != "")
                {
                    if (Convert.ToDecimal(lblTax.Text) == 0)
                    {
                        lblTax.Visible = false;
                        lblTax_label.Visible = false;

                    }
                    else
                    {
                        lblTax.Visible = true;
                        lblTax_label.Visible = true;
                    }
                }
                else
                {
                    lblTax.Visible = false;
                    lblTax_label.Visible = false;
                }


                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblJobNumber.Text = cus_est.job_number;
                  
                    txtAlterJobNumber.Text = cus_est.alter_job_number;
                    
                    lblEstimateName.Text = cus_est.estimate_name;
                    
                    ddlStatus.SelectedValue = cus_est.status_id.ToString();
                    ddlStatus.Enabled = false;
                    lblSaleDate.Visible = true;
                    txtSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                    txtSaleDate.Visible = true;
                    txtSaleDate.ReadOnly = true;
                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";
                    txtComments.ReadOnly = true;

                    if (Convert.ToBoolean(cus_est.IsEstimateActive) == false)
                    {
                        rdbEstimateIsActive.SelectedValue = "0";
                    }
                    else
                    {
                        rdbEstimateIsActive.SelectedValue = "1";

                    }
                    chkCustDisp.Checked = Convert.ToBoolean(cus_est.IsCustDisplay);

                    //if ((cus_est.job_number ?? "").Length > 0)
                    //{
                    //    lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                    //}
                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
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
                Calculate_Total();
                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {
                    rdoSort.Visible = false;
                    tblTotalProjectPrice.Visible = false;
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
                    tblTotalProjectPrice.Visible = true;
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
                if (Session["sSytemMassage"] != null)
                {
                    
                    lblResult1.Text = csCommonUtility.GetSystemMessage(Session["sSytemMassage"].ToString());

                    Session.Add("sSytemMassage", null);
                }
            }



            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSaveAlterJob", "btnPricingPrint", "btnPricingPrintWOPrice",  "lnkUpdateEstimate", "btnTemplateEstimate", "ddlSuperintendent", "ddlSuperintendentTwo", "ddlDesigner", "ddlArchitect", "chkCustDisp", "rdbEstimateIsActive", "btnExpAsSoldList", "pnlUpdateEstimate", "chkPMDisplay", "hypGoogleMap" });
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE role_id = 4 and is_active=1";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSuperintendent.DataSource = mList;
        ddlSuperintendent.DataTextField = "Superintendent_name";
        ddlSuperintendent.DataValueField = "user_id";
        ddlSuperintendent.DataBind();
        ddlSuperintendent.Items.Insert(0, "Select");
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
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " GROUP BY pricing_details.location_id,location.location_name order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
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
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " GROUP BY pricing_details.location_id,location.location_name order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }
    public void Calculate_Total()
    {
        decimal direct = 0;
        decimal retail = 0;
        decimal grandtotal = 0;
        direct = GetDirctTotal();
        retail = GetRetailTotal();
        grandtotal = direct + retail;

        lblDirctTotalCost.Text = direct.ToString("c");
        lblRetailTotalCost.Text = retail.ToString("c");
        lblGrandTotalCost.Text = grandtotal.ToString("c");

    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelecterdItem = (GridView)sender;
            bool IsMandatory = Convert.ToBoolean(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[1]);
            Label lblshNote = (Label)e.Row.FindControl("lblshNote");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");

            if (lblshNote.Text != "" && lblshNote.Text.Length > 50)
            {
                lblshNote.ToolTip = lblshNote.Text;
                lblshNote.Text = lblshNote.Text.Substring(0, 50) + "...";
                lnkOpen.Visible = true;
            }
            else
            {
                lblshNote.ToolTip = lblshNote.Text;
                lnkOpen.Visible = false;
               // e.Row.Cells[4].Text = str;

            }


           // string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
            //if (str != "" && str.Length > 50)
            //{
            //    e.Row.Cells[4].ToolTip = str;
            //    e.Row.Cells[4].Text = str.Substring(0, 50) + "...";
            //}
            //else
            //{
            //    e.Row.Cells[4].ToolTip = str;
            //    e.Row.Cells[4].Text = str;

            //}
            if (IsMandatory)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");

            }
            if (chkCustDisp.Checked)
            {
                grdSelecterdItem.Columns[5].Visible = false;
                grdSelecterdItem.Columns[6].Visible = false;
                grdSelecterdItem.Columns[7].Visible = false;
                grdSelecterdItem.Columns[8].Visible = false;
                grdSelecterdItem.Columns[9].Visible = false;
                grdSelecterdItem.Columns[10].Visible = false;
                grdSelecterdItem.Columns[11].Visible = false;

                grdSelecterdItem.Columns[4].ItemStyle.Width = new Unit(20, UnitType.Percentage);
                grdSelecterdItem.Columns[3].ItemStyle.Width = new Unit(40, UnitType.Percentage);
            }
            if (chkPMDisplay.Checked)
            {
                grdSelecterdItem.Columns[7].Visible = true;
                grdSelecterdItem.Columns[9].Visible = true;
                grdSelecterdItem.Columns[10].Visible = true;
            }
            else
            {
                grdSelecterdItem.Columns[7].Visible = false;
                grdSelecterdItem.Columns[9].Visible = false;
                grdSelecterdItem.Columns[10].Visible = false;

            }

        }

    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes = gRow.Cells[4].Controls[0].FindControl("lblshNote") as Label;
        Label lblshort_notes_r = gRow.Cells[4].Controls[1].FindControl("lblshort_notes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[4].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblshort_notes.Visible = false;
            lblshort_notes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblshort_notes.Visible = true;
            lblshort_notes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";

        }
    }

    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    protected string GetTotalPrice()
    {

        decimal nGrandtotal = Convert.ToDecimal(grandtotal);
        decimal nGrandtotalCost = Convert.ToDecimal(grandtotalCost);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);
        //  string Total = string.Empty;
        //if (chkPMDisplay.Checked)
        //    Total = "Total Ext. Cost: " + nGrandtotalCost.ToString("c")+ "         Total Ext. Price: "+nGrandtotal.ToString("c")+"      Total with Tax: " + grandtotal.ToString("c");
        //else
        //    Total = "Total with Tax: " + grandtotal.ToString("c");

        return "Total with Tax: " + grandtotal.ToString("c");
    }

    protected string GetOverallProjectedProfit()
    {
        decimal nOverallProjectProfit = 0;
        if (grandtotalCost > 0)
            nOverallProjectProfit = (Math.Round(((Convert.ToDecimal(grandtotal) - Convert.ToDecimal(grandtotalCost)) * 100) / Convert.ToDecimal(grandtotalCost), 2));
        else
            nOverallProjectProfit = Math.Round(Convert.ToDecimal(grandtotal));

        return "Overall Projected Profit: " + nOverallProjectProfit.ToString() + "%";

    }
    protected string GetTotalExtCost()
    {   
        decimal nGrandtotalCost = Convert.ToDecimal(grandtotalCost);
        return nGrandtotalCost.ToString("c");
    }
    protected string GetTotalExPrice()
    {   
        decimal nGrandtotal = Convert.ToDecimal(grandtotal);
        return nGrandtotal.ToString("c");
    }


    protected string GetTotalPriceDirect()
    {
        decimal nGrandtotal_direct = Convert.ToDecimal(grandtotal_direct);
        decimal nGrandtotal_directCost = Convert.ToDecimal(grandtotal_directCost);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);
        // string Total = string.Empty;
        //if (chkPMDisplay.Checked)
        //    Total = "Total Ext. Cost: " + nGrandtotal_directCost.ToString("c") + "         Total Ext. Price: " + nGrandtotal_direct.ToString("c") + "      Total with Tax: " + grandtotal_direct.ToString("c");
        //else
        //    Total = "Total with tax " + grandtotal_direct.ToString("c");
        return "Total with tax " + grandtotal_direct.ToString("c");
    }
    protected string GetTotalExtCostDirect()
    {

        decimal nGrandtotalCost = Convert.ToDecimal(grandtotal_directCost);
        // string Total = string.Empty;
        //Total = "Total Ext. Cost: " + nGrandtotalCost.ToString("c");
        return nGrandtotalCost.ToString("c");
    }
    protected string GetTotalExPriceDirect()
    {

        decimal nGrandtotal = Convert.ToDecimal(grandtotal_direct);
        //string Total = string.Empty;
        //Total = "Total Ext. Price: " + nGrandtotal.ToString("c");
        return nGrandtotal.ToString("c");
    }

    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            LinkButton lnkSendEmail1 = (LinkButton)e.Row.FindControl("lnkSendEmail1");
            Label lblMSub = (Label)e.Row.FindControl("lblMSub");
            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail1.Visible = false;

            }
            else
            {
                if (chkPMDisplay.Checked)
                {
                    lnkSendEmail1.Visible = true;
                }
                else
                {
                    lnkSendEmail1.Visible = false;
                }
            }
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            Label lblProjectedProfit = footerRow.FindControl("lblProjectedProfit") as Label;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");

                Label lblSubTotalCost = footerRow.FindControl("lblSubTotalCost") as Label;
                subtotalCost += Double.Parse((row.FindControl("lblTotal_Cost") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                lblSubTotalCost.Text = subtotalCost.ToString("c");



                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                if (chkCustDisp.Checked)
                    lblSubTotalLabel.Text = "Sub Total: " + subtotal.ToString("c");
                else
                    lblSubTotalLabel.Text = "Sub Total:";
            }
            grandtotal += subtotal;
            lblMSub.Text = " (" + subtotal.ToString("c") + ")";

            if (chkPMDisplay.Checked == true)
            {
                lblProjectedProfit.Visible = true;
                lblProjectedProfit.Text = "Projected Profit: " + (Math.Round(((subtotal - subtotalCost) * 100) / subtotalCost, 2)).ToString() + "%";
            }
            else
            {
                lblProjectedProfit.Visible = false;
            }
            subtotal = 0.0;

            grandtotalCost += subtotalCost;
            subtotalCost = 0.0;

        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblGtotalExtCost = (Label)e.Row.FindControl("lblGtotalExtCost");
            Label lblGtotalExtPrice = (Label)e.Row.FindControl("lblGtotalExtPrice");
            Label lblGtotalLabel = (Label)e.Row.FindControl("lblGtotalLabel");
            Label lblOverallProjectedProfit = (Label)e.Row.FindControl("lblOverallProjectedProfit");
            if (chkPMDisplay.Checked == true)
            {
                lblGtotalExtPrice.Visible = true;
                lblGtotalExtCost.Visible = true;
                lblGtotalLabel.Visible = true;
                lblOverallProjectedProfit.Visible = true;

            }
            else
            {
                lblGtotalExtPrice.Visible = false;
                lblGtotalExtCost.Visible = false;
                lblGtotalLabel.Visible = false;
                lblOverallProjectedProfit.Visible = false;

            }

        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (rdoSort.SelectedValue == "1")
        {
            var price_detail = from p in _db.pricing_details
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.customer_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.customer_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value) && p.pricing_type == "A"
                               orderby p.section_level ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.pricing_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
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
                                   tmpCol = string.Empty,
                                   is_mandatory = (bool)p.is_mandatory,
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_update_date,
                                   section_level = (int)p.section_level,
                                   location_id = (int)p.location_id,
                                   customer_id = (int)p.customer_id,
                                   sales_person_id = (int)p.sales_person_id,
                                   client_id = (int)p.client_id,
                                   unit_cost = 0,
                                   total_unit_cost = 0,
                                   total_labor_cost = 0
                               };
            DataTable dt = SessionInfo.LINQToDataTable(price_detail);
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;

                decimal dTPrice = 0;

                string sItemName = dr["item_name"].ToString();

                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);

                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                    }

                    dLabor_rate = 0;

                }
                else
                {
                    if (dRetail_multiplier > 0)
                    {

                        dOrginalCost = (dItemCost / dRetail_multiplier);
                    }
                    else
                    {
                        dOrginalCost = dItemCost;
                    }
                }

                dOrginalTotalCost = dOrginalCost * dQuantity;
                dLaborTotal = dLabor_rate * dQuantity;
                dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                dr["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                dr["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                dr["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total

            }
            DataView dv = dt.DefaultView;
            dv.Sort = "section_level,item_id ASC";
            grd.DataSource = dv; //price_detail.ToList().OrderBy(c => c.section_level).ThenBy(c => c.item_id);
            grd.DataKeyNames = new string[] { "pricing_id", "is_mandatory" };
            grd.DataBind();
        }
        else
        {
            var price_detail = from p in _db.pricing_details
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.customer_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.customer_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                       select cs.section_id).Contains(p.section_level)
                                      && p.section_level == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(hdnClientId.Value) && p.pricing_type == "A"
                               orderby lc.location_name ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.pricing_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
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
                                   tmpCol = string.Empty,
                                   is_mandatory = (bool)p.is_mandatory,
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_update_date,
                                   section_level = (int)p.section_level,
                                   location_id = (int)p.location_id,
                                   customer_id = (int)p.customer_id,
                                   sales_person_id = (int)p.sales_person_id,
                                   client_id = (int)p.client_id,
                                   unit_cost = 0,
                                   total_unit_cost = 0,
                                   total_labor_cost = 0
                               };
            DataTable dt = SessionInfo.LINQToDataTable(price_detail);
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;

                decimal dTPrice = 0;

                string sItemName = dr["item_name"].ToString();

                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                if (nDirectId == 1)
                    dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                else
                    dTPrice = Convert.ToDecimal(dr["total_direct_price"]);

                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                    }

                    dLabor_rate = 0;

                }
                else
                {
                    if (dRetail_multiplier > 0)
                    {

                        dOrginalCost = (dItemCost / dRetail_multiplier);
                    }
                    else
                    {
                        dOrginalCost = dItemCost;
                    }
                }

                dOrginalTotalCost = dOrginalCost * dQuantity;
                dLaborTotal = dLabor_rate * dQuantity;
                dLineItemTotal = dOrginalTotalCost + dLaborTotal;
                dr["unit_cost"] = dOrginalCost; // Orginal Unit Cost
                dr["total_labor_cost"] = dLaborTotal; // Orginal dLabor_rate * dQuantity;
                dr["total_unit_cost"] = dLineItemTotal; // Orginal Cost Total

            }
            DataView dv = dt.DefaultView;

            dv.Sort = "location_name ASC";
            grd.DataSource = dv; // // price_detail.ToList().OrderBy(c => c.location_name);
            grd.DataKeyNames = new string[] { "pricing_id", "is_mandatory" };
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
        Calculate_Total();
    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelectedItem2 = (GridView)sender;
            bool IsMandatory = Convert.ToBoolean(grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[1]);
            string str = e.Row.Cells[4].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 50)
            {
                e.Row.Cells[4].ToolTip = str;
                e.Row.Cells[4].Text = str.Substring(0, 50) + "...";
            }
            else
            {
                e.Row.Cells[4].ToolTip = str;
                e.Row.Cells[4].Text = str;

            }
            if (IsMandatory)
            {
                e.Row.Attributes.CssStyle.Add("color", "Violet");

            }
            if (chkCustDisp.Checked)
            {
                grdSelectedItem2.Columns[5].Visible = false;
                grdSelectedItem2.Columns[6].Visible = false;
                grdSelectedItem2.Columns[7].Visible = false;
                grdSelectedItem2.Columns[8].Visible = false;
                grdSelectedItem2.Columns[9].Visible = false;
                grdSelectedItem2.Columns[10].Visible = false;
                grdSelectedItem2.Columns[11].Visible = false;

                grdSelectedItem2.Columns[4].ItemStyle.Width = new Unit(18, UnitType.Percentage);
                grdSelectedItem2.Columns[3].ItemStyle.Width = new Unit(40, UnitType.Percentage);
            }
            if (chkPMDisplay.Checked)
            {
                grdSelectedItem2.Columns[7].Visible = true;
                grdSelectedItem2.Columns[9].Visible = true;
                grdSelectedItem2.Columns[10].Visible = true;
            }
            else
            {
                grdSelectedItem2.Columns[7].Visible = false;
                grdSelectedItem2.Columns[9].Visible = false;
                grdSelectedItem2.Columns[10].Visible = false;

            }

        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            LinkButton lnkSendEmail2 = (LinkButton)e.Row.FindControl("lnkSendEmail2");
            Label lblMSub2 = (Label)e.Row.FindControl("lblMSub2");
            if (rdoSort.SelectedValue == "1")
            {
                lnkSendEmail2.Visible = false;
            }
            else
            {
                if (chkPMDisplay.Checked)
                {
                    lnkSendEmail2.Visible = true;
                }
                else
                {
                    lnkSendEmail2.Visible = false;
                }

            }

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
                subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal2.Text = subtotal_diect.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader2.Text = "Section";
                }
                else
                {
                    lblHeader2.Text = "Location";
                }
                if (chkCustDisp.Checked)
                    lblSubTotalLabel2.Text = "Sub Total:" + subtotal_diect.ToString("c");
                else
                    lblSubTotalLabel2.Text = "Sub Total:";
            }
            grandtotal_direct += subtotal_diect;
            lblMSub2.Text = " (" + subtotal_diect.ToString("c") + ")";
            subtotal_diect = 0.0;
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblGtotalExtCost = (Label)e.Row.FindControl("lblGtotalExtCost2");
            Label lblGtotalExtPrice = (Label)e.Row.FindControl("lblGtotalExtPrice2");
            Label lblGtotalLabel = (Label)e.Row.FindControl("lblGtotalLabel2");
            if (chkPMDisplay.Checked == true)
            {
                lblGtotalExtPrice.Visible = true;
                lblGtotalExtCost.Visible = true;
                lblGtotalLabel.Visible = true;

            }
            else
            {
                lblGtotalExtPrice.Visible = false;
                lblGtotalExtCost.Visible = false;
                lblGtotalLabel.Visible = false;

            }

        }

    }
    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&TypeID=1");
    }
    protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "SelectedIndexChanged"); 
        if (ddlSuperintendent.SelectedItem.Text == "Select")
        {
            lblResult1.Text = csCommonUtility.GetSystemErrorMessage("Superintendent is a required field");
            btnUpdateSuperintendent.Visible = false;
        }
        else
        {
            btnUpdateSuperintendent.Visible = true;
        }

    }
    protected void btnUpdateSuperintendent_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateSuperintendent.ID, btnUpdateSuperintendent.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        int nSuperintendentId = 0;
        if (ddlSuperintendent.SelectedItem.Text != "Select")
        {
            nSuperintendentId = Convert.ToInt32(ddlSuperintendent.SelectedValue);
        }
        string strCustQ = "UPDATE customers SET  SuperintendentId =" + nSuperintendentId + "  WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strCustQ, string.Empty);
        lblResult1.Text = csCommonUtility.GetSystemMessage("Superintendent updated successfully");

        btnUpdateSuperintendent.Visible = false;
    }
    protected void rdbEstimateIsActive_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdbEstimateIsActive.ID, rdbEstimateIsActive.GetType().Name, "SelectedIndexChanged"); 
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = "UPDATE customer_estimate SET IsEstimateActive = " + rdbEstimateIsActive.SelectedValue +
               " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
               " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
               " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            if (_db.ScheduleCalendars.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).Count() > 0)
            {
                string sSql = "UPDATE ScheduleCalendar SET IsEstimateActive=" + rdbEstimateIsActive.SelectedValue + " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
                _db.ExecuteCommand(sSql, string.Empty);
            }
            if (rdbEstimateIsActive.SelectedValue == "1")
            {
                lblResult1.Text = csCommonUtility.GetSystemMessage("Project activated successfully.");
            }
            else
            {
                lblResult1.Text = csCommonUtility.GetSystemMessage("Project deactivated successfully.");
            }
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }
    }
    protected void chkCustDisp_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCustDisp.ID, chkCustDisp.GetType().Name, "CheckedChanged"); 
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = string.Empty;

            chkPMDisplay.Checked = false;

            if (chkCustDisp.Checked)
            {
                strQ = "UPDATE customer_estimate SET IsCustDisplay = 1 " +
              " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
              " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
              " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ, string.Empty);
                //lblResult1.Text = csCommonUtility.GetSystemMessage("Customized display setting of Project activated successfully.");
            }
            else
            {
                strQ = "UPDATE customer_estimate SET IsCustDisplay = 0 " +
             " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
             " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
             " AND client_id=" + Convert.ToInt32(hdnClientId.Value);
                _db.ExecuteCommand(strQ, string.Empty);
                // lblResult1.Text = csCommonUtility.GetSystemMessage("Customized display setting of Project deactivated successfully.");
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
            Calculate_Total();
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }

    }
    protected void chkPMDisplay_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkPMDisplay.ID, chkPMDisplay.GetType().Name, "CheckedChanged"); 
        try
        {
            chkCustDisp.Checked = false;
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
            Calculate_Total();
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }
    }
    protected void btnExpCostLocList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCostLocList.ID, btnExpCostLocList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value) + "  " +
              " UNION " +
              " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtCostMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sCostList", dtCostMaster);

        DataTable tmpTable = LoadTmpDataTable();
        #region Cost by Location
        decimal GItemTotal = 0;
        decimal GLaborTotal = 0;
        decimal GSubTotalCost = 0;

        DataView dvLoc = new DataView(dtCostMaster);
        DataTable dtResults = dvLoc.ToTable(true, "location_id", "location_name");

        DataTable tblCustList = (DataTable)Session["sCostList"];
        DataRow drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Cost by Location";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Customer Name: " + lblCustomerName.Text;
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Estimate: " + lblEstimateName.Text;
        tmpTable.Rows.Add(drNew);

        foreach (DataRow dr in dtResults.Rows)
        {
            decimal dSubItemTotal = 0;
            decimal dSubLaborTotal = 0;
            decimal dSubTotalCost = 0;
            int nLocationId = Convert.ToInt32(dr["location_id"]);
            string strLocation = dr["location_name"].ToString();

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Location: " + strLocation;
            tmpTable.Rows.Add(drNew);


            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Section Name";
            drNew["Item Name"] = "Item Name";
            drNew["Short Notes"] = "Short Notes";
            drNew["U of M"] = "U of M";
            drNew["Code"] = "Code";
            drNew["Item Cost"] = "Item Cost";
            drNew["Item Total"] = "Item Total";
            drNew["Labor Rate"] = "Labor Rate";
            drNew["Labor Total"] = "Labor Total";
            drNew["Total Cost"] = "Total Cost";
            tmpTable.Rows.Add(drNew);

            bool Iexists = tblCustList.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(nLocationId)).Count() > 0;
            if (Iexists)
            {
                var rows = tblCustList.Select("location_id =" + nLocationId + "");
                foreach (var row in rows)
                {
                    decimal dOrginalCost = 0;
                    decimal dOrginalTotalCost = 0;
                    decimal dLaborTotal = 0;
                    decimal dLineItemTotal = 0;
                    string sItemName = row["item_name"].ToString();

                    decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                    decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                    decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                    decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                    if (dRetail_multiplier > 0)
                    {
                        if (sItemName.IndexOf("Other") > 0)
                        {
                            dOrginalCost = (dItemCost / dRetail_multiplier) / 2;
                        }
                        else
                        {
                            dOrginalCost = (dItemCost / dRetail_multiplier);
                        }
                    }
                    else
                    {
                        if (sItemName.IndexOf("Other") > 0)
                        {
                            dOrginalCost = dItemCost / 2;
                        }
                        else
                        {
                            dOrginalCost = dItemCost;
                        }

                    }
                    dOrginalTotalCost = dOrginalCost * dQuantity;
                    dLaborTotal = dLabor_rate * dQuantity;
                    dLineItemTotal = dOrginalTotalCost + dLaborTotal;

                    dSubItemTotal += dOrginalTotalCost;
                    dSubLaborTotal += dLaborTotal;
                    dSubTotalCost += dLineItemTotal;

                    GItemTotal += dOrginalTotalCost;
                    GLaborTotal += dLaborTotal;
                    GSubTotalCost += dLineItemTotal;

                    drNew = tmpTable.NewRow();
                    drNew["Section Name"] = row["section_name"];
                    drNew["Item Name"] = sItemName;
                    drNew["Short Notes"] = row["short_notes"];
                    drNew["U of M"] = row["measure_unit"];
                    drNew["Code"] = row["quantity"].ToString();
                    drNew["Item Cost"] = dOrginalCost.ToString("0.0000");
                    drNew["Item Total"] = dOrginalTotalCost.ToString("c");
                    drNew["Labor Rate"] = dLabor_rate.ToString("c");
                    drNew["Labor Total"] = dLaborTotal.ToString("c");
                    drNew["Total Cost"] = dLineItemTotal.ToString("c");
                    tmpTable.Rows.Add(drNew);

                }
            }

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            drNew["Item Name"] = "";
            drNew["Short Notes"] = "";
            drNew["U of M"] = "";
            drNew["Code"] = "";
            drNew["Item Cost"] = "Sub Total:";
            drNew["Item Total"] = dSubItemTotal.ToString("c");
            drNew["Labor Rate"] = "";
            drNew["Labor Total"] = dSubLaborTotal.ToString("c");
            drNew["Total Cost"] = dSubTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

        }

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "";
        drNew["Item Name"] = "";
        drNew["Short Notes"] = "";
        drNew["U of M"] = "";
        drNew["Code"] = "";
        drNew["Item Cost"] = "Grand Total:";
        drNew["Item Total"] = GItemTotal.ToString("c");
        drNew["Labor Rate"] = "";
        drNew["Labor Total"] = GLaborTotal.ToString("c");
        drNew["Total Cost"] = GSubTotalCost.ToString("c");
        tmpTable.Rows.Add(drNew);

        #endregion

        Session.Add("CostReport", tmpTable);
        string strUrl = "CostReportCsv.aspx?TypeId=1";
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
    }
    protected void btnExpCostSecList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCostSecList.ID, btnExpCostSecList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value) + "  " +
              " UNION " +
              " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
              " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
              " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(hdnClientId.Value);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        if (CList.Count == 0)
        {
            strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


            CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        }

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtCostMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sCostList", dtCostMaster);

        DataTable tmpTable = LoadTmpDataTable();
        #region Cost by Section
        decimal GItemTotal = 0;
        decimal GLaborTotal = 0;
        decimal GSubTotalCost = 0;
        DataView dvSec = new DataView(dtCostMaster);
        DataTable dtResults = dvSec.ToTable(true, "section_level", "section_name");

        DataTable tblCustList = (DataTable)Session["sCostList"];
        DataRow drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Cost by Section";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Customer Name: " + lblCustomerName.Text;
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "Estimate: " + lblEstimateName.Text;
        tmpTable.Rows.Add(drNew);

        foreach (DataRow dr in dtResults.Rows)
        {
            decimal dSubItemTotal = 0;
            decimal dSubLaborTotal = 0;
            decimal dSubTotalCost = 0;
            int nSection_level = Convert.ToInt32(dr["section_level"]);
            string strSection = dr["section_name"].ToString();

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Section: " + strSection;
            tmpTable.Rows.Add(drNew);


            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "Location Name";
            drNew["Item Name"] = "Item Name";
            drNew["Short Notes"] = "Short Notes";
            drNew["U of M"] = "U of M";
            drNew["Code"] = "Code";
            drNew["Item Cost"] = "Item Cost";
            drNew["Item Total"] = "Item Total";
            drNew["Labor Rate"] = "Labor Rate";
            drNew["Labor Total"] = "Labor Total";
            drNew["Total Cost"] = "Total Cost";
            tmpTable.Rows.Add(drNew);

            bool Iexists = tblCustList.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(nSection_level)).Count() > 0;
            if (Iexists)
            {
                var rows = tblCustList.Select("section_level =" + nSection_level + "");
                foreach (var row in rows)
                {
                    decimal dOrginalCost = 0;
                    decimal dOrginalTotalCost = 0;
                    decimal dLaborTotal = 0;
                    decimal dLineItemTotal = 0;
                    string sItemName = row["item_name"].ToString();

                    decimal dItemCost = Convert.ToDecimal(row["item_cost"]);
                    decimal dRetail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                    decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                    decimal dLabor_rate = Convert.ToDecimal(row["labor_rate"]);
                    if (dRetail_multiplier > 0)
                    {
                        if (sItemName.IndexOf("Other") > 0)
                        {
                            dOrginalCost = (dItemCost / dRetail_multiplier) / 2;
                        }
                        else
                        {
                            dOrginalCost = (dItemCost / dRetail_multiplier);
                        }
                    }
                    else
                    {
                        if (sItemName.IndexOf("Other") > 0)
                        {
                            dOrginalCost = dItemCost / 2;
                        }
                        else
                        {
                            dOrginalCost = dItemCost;
                        }

                    }
                    dOrginalTotalCost = dOrginalCost * dQuantity;
                    dLaborTotal = dLabor_rate * dQuantity;
                    dLineItemTotal = dOrginalTotalCost + dLaborTotal;

                    dSubItemTotal += dOrginalTotalCost;
                    dSubLaborTotal += dLaborTotal;
                    dSubTotalCost += dLineItemTotal;

                    GItemTotal += dOrginalTotalCost;
                    GLaborTotal += dLaborTotal;
                    GSubTotalCost += dLineItemTotal;

                    drNew = tmpTable.NewRow();
                    drNew["Section Name"] = row["location_name"];
                    drNew["Item Name"] = sItemName;
                    drNew["Short Notes"] = row["short_notes"];
                    drNew["U of M"] = row["measure_unit"];
                    drNew["Code"] = row["quantity"].ToString();
                    drNew["Item Cost"] = dOrginalCost.ToString("0.0000");
                    drNew["Item Total"] = dOrginalTotalCost.ToString("c");
                    drNew["Labor Rate"] = dLabor_rate.ToString("c");
                    drNew["Labor Total"] = dLaborTotal.ToString("c");
                    drNew["Total Cost"] = dLineItemTotal.ToString("c");
                    tmpTable.Rows.Add(drNew);

                }
            }
            drNew = tmpTable.NewRow();
            drNew["Section Name"] = "";
            drNew["Item Name"] = "";
            drNew["Short Notes"] = "";
            drNew["U of M"] = "";
            drNew["Code"] = "";
            drNew["Item Cost"] = "Sub Total:";
            drNew["Item Total"] = dSubItemTotal.ToString("c");
            drNew["Labor Rate"] = "";
            drNew["Labor Total"] = dSubLaborTotal.ToString("c");
            drNew["Total Cost"] = dSubTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

        }
        drNew = tmpTable.NewRow();
        drNew["Section Name"] = "";
        drNew["Item Name"] = "";
        drNew["Short Notes"] = "";
        drNew["U of M"] = "";
        drNew["Code"] = "";
        drNew["Item Cost"] = "Grand Total:";
        drNew["Item Total"] = GItemTotal.ToString("c");
        drNew["Labor Rate"] = "";
        drNew["Labor Total"] = GLaborTotal.ToString("c");
        drNew["Total Cost"] = GSubTotalCost.ToString("c");
        tmpTable.Rows.Add(drNew);
        #endregion
        Session.Add("CostReport", tmpTable);
        string strUrl = "CostReportCsv.aspx?TypeId=2";
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
    }

    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("Section Name", typeof(string));
        table.Columns.Add("Item Name", typeof(string));
        table.Columns.Add("Short Notes", typeof(string));
        table.Columns.Add("U of M", typeof(string));
        table.Columns.Add("Code", typeof(string));
        table.Columns.Add("Item Cost", typeof(string));
        table.Columns.Add("Item Total", typeof(string));
        table.Columns.Add("Labor Rate", typeof(string));
        table.Columns.Add("Labor Total", typeof(string));
        table.Columns.Add("Total Cost", typeof(string));


        return table;
    }
    private DataTable LoadAsSoldDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("ITEM ID", typeof(string));
        table.Columns.Add("SL", typeof(string));
        table.Columns.Add("SECTION", typeof(string));
        table.Columns.Add("ITEM NAME", typeof(string));
        table.Columns.Add("SHORT NOTES", typeof(string));
        table.Columns.Add("UOM", typeof(string));
        table.Columns.Add("QTY", typeof(string));
        table.Columns.Add("MATERIAL", typeof(string));
        table.Columns.Add("LABOR", typeof(string));
        table.Columns.Add("EXT. PRICE", typeof(string));
        return table;
    }
    private void GetPaymentInfo(int nEstimateId, int nCustomerId, int nSalesPersonId)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();
        objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId && pay.sales_person_id == nSalesPersonId && pay.client_id == Convert.ToInt32(hdnClientId.Value));

        if (objEstPay != null)
        {
            decimal nTax = Convert.ToDecimal(objEstPay.tax_rate);

            string IncIds = objEstPay.incentive_ids;
            if (IncIds != null)
            {
                string strQ = "SELECT * FROM incentives WHERE incentive_id IN (" + IncIds + ")";
                DataTable dt = csCommonUtility.GetDataTable(strQ);
            }
        }
    }
    protected void btnExpAsSoldList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpAsSoldList.ID, btnExpAsSoldList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.estimate_payments.Where(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            estimate_payment objEstPay = new estimate_payment();
            objEstPay = _db.estimate_payments.SingleOrDefault(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value));

            if (objEstPay != null)
            {
                decimal nTax = Convert.ToDecimal(objEstPay.tax_rate);

                string IncIds = objEstPay.incentive_ids;
                if (IncIds != null)
                {
                    string str = "SELECT * FROM incentives WHERE incentive_id IN (" + IncIds + ")";
                    DataTable dt = csCommonUtility.GetDataTable(str);

                    Session.Add("sIncentivesList", dt);
                }
            }
        }

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                   " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id  " +
                   " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value) + " order by sort_id,section_level asc";

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();

        if (CList.Count == 0)
        {
            return;
        }
        DataTable dtSoldMaster = csCommonUtility.LINQToDataTable(CList);
        Session.Add("sSoldList", dtSoldMaster);

        DataTable tmpTable = LoadAsSoldDataTable();

        if (rdoSort.SelectedValue == "1")
        {
            #region Cost by Location
            decimal GSubsoldTotalCost = 0;
            decimal GSubsoldTotalMaterial = 0;
            decimal GSubsoldTotalLabor = 0;

            DataView dvLocsold = new DataView(dtSoldMaster);
            DataTable dtSoldResults = dvLocsold.ToTable(true, "location_id", "location_name");

            DataTable tblSoldList = (DataTable)Session["sSoldList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Project as Sold  by Location";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Job# " + lblJobNumber.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Customer Name: " + lblCustomerName.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Estimate: " + lblEstimateName.Text;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtSoldResults.Rows)
            {
                decimal dSubTotalCost = 0;
                decimal dSubTotalMaterial = 0;
                decimal dSubTotalLabor = 0;
                int nLocationId = Convert.ToInt32(dr["location_id"]);
                string strLocation = dr["location_name"].ToString();

                drNew = tmpTable.NewRow();
                drNew["ITEM NAME"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "Location: " + strLocation;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "ITEM ID";
                drNew["SL"] = "SL";
                drNew["SECTION"] = "SECTION";
                drNew["ITEM NAME"] = "ITEM NAME";
                drNew["SHORT NOTES"] = "SHORT NOTES";
                drNew["UOM"] = "UOM";
                drNew["QTY"] = "QTY";
                drNew["MATERIAL"] = "MATERIAL";
                drNew["LABOR"] = "LABOR";
                drNew["EXT. PRICE"] = "EXT. PRICE";
                tmpTable.Rows.Add(drNew);

                bool Iexists = tblSoldList.AsEnumerable().Where(c => c.Field<Int32>("location_id").Equals(nLocationId)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblSoldList.Select("location_id =" + nLocationId + "");
                    foreach (var row in rows)
                    {

                        decimal dLineItemTotal = 0;
                        decimal dLineItemMaterial = 0;
                        decimal dLineItemLabor = 0;
                        string sItemName = row["item_name"].ToString();

                        decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                        decimal dlabor_rate = Convert.ToDecimal(row["labor_rate"]);
                        decimal dretail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                        decimal ditem_cost = Convert.ToDecimal(row["item_cost"]);
                        int nLaborID = Convert.ToInt32(row["labor_id"]);
                        dLineItemTotal = Convert.ToDecimal(row["total_retail_price"]);
                        if (nLaborID == 1)
                        {
                            dLineItemLabor = 0;
                            dLineItemMaterial = dLineItemTotal;

                        }
                        else
                        {
                            if (ditem_cost == 0)
                            {
                                dLineItemLabor = dLineItemTotal;
                                dLineItemMaterial = 0;

                            }
                            else
                            {
                                dLineItemLabor = (dlabor_rate * dretail_multiplier) * dQuantity;
                                dLineItemMaterial = dLineItemTotal - dLineItemLabor;

                            }

                        }
                        dSubTotalLabor += dLineItemLabor;
                        dSubTotalMaterial += dLineItemMaterial;
                        dSubTotalCost += dLineItemTotal;

                        GSubsoldTotalLabor += dLineItemLabor;
                        GSubsoldTotalMaterial += dLineItemMaterial;
                        GSubsoldTotalCost += dLineItemTotal;

                        drNew = tmpTable.NewRow();

                        drNew["ITEM ID"] = row["item_id"].ToString();
                        drNew["SL"] = row["section_serial"].ToString();
                        drNew["SECTION"] = row["section_name"];
                        drNew["ITEM NAME"] = sItemName;
                        drNew["SHORT NOTES"] = row["short_notes"];
                        drNew["UOM"] = row["measure_unit"];
                        drNew["QTY"] = row["quantity"].ToString();
                        drNew["MATERIAL"] = dLineItemMaterial.ToString("c");
                        drNew["LABOR"] = dLineItemLabor.ToString("c");
                        drNew["EXT. PRICE"] = dLineItemTotal.ToString("c");
                        tmpTable.Rows.Add(drNew);

                    }
                }

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "Sub Total:";
                drNew["MATERIAL"] = dSubTotalMaterial.ToString("c");
                drNew["LABOR"] = dSubTotalLabor.ToString("c");
                drNew["EXT. PRICE"] = dSubTotalCost.ToString("c");
                tmpTable.Rows.Add(drNew);

            }

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "";
            drNew["SL"] = "";
            drNew["SECTION"] = "";
            drNew["ITEM NAME"] = "";
            drNew["SHORT NOTES"] = "";
            drNew["UOM"] = "";
            if (Session["sIncentivesList"] != null)
            {
                drNew["QTY"] = "Project Total:";
            }
            else
            {
                drNew["QTY"] = "Grand Total:";
            }
            drNew["MATERIAL"] = GSubsoldTotalMaterial.ToString("c");
            drNew["LABOR"] = GSubsoldTotalLabor.ToString("c");
            drNew["EXT. PRICE"] = GSubsoldTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

            if (Session["sIncentivesList"] != null)
            {
                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "PROMOTIONS & INCENTIVES";
                tmpTable.Rows.Add(drNew);

                DataTable dtInc = (DataTable)Session["sIncentivesList"];
                decimal adjustedTotal = 0;
                decimal nTotalIncentive = 0;
                int i = 0;
                foreach (DataRow dr in dtInc.Rows)
                {
                    i++;
                    decimal nIncentive = 0;
                    string strincentive_name = dr["incentive_name"].ToString();
                    decimal IncPer = Convert.ToDecimal(dr["discount"]);
                    decimal IncAmount = Convert.ToDecimal(dr["amount"]);
                    int ntype = Convert.ToInt32(dr["incentive_type"]);
                    if (ntype == 1)
                        nIncentive = Convert.ToDecimal(GSubsoldTotalCost * IncPer / 100);
                    else
                        nIncentive = IncAmount;

                    //nIncentive = Convert.ToDecimal(GSubsoldTotalCost * IncPer / 100); Without Type

                    drNew = tmpTable.NewRow();
                    drNew["ITEM ID"] = "";
                    drNew["SL"] = "";
                    if (dtInc.Rows.Count > 1)
                    {
                        drNew["SECTION"] = "Incentive# " + i;
                    }
                    else
                    {
                        drNew["SECTION"] = "Incentive";
                    }
                    drNew["ITEM NAME"] = strincentive_name;
                    drNew["SHORT NOTES"] = "";
                    drNew["UOM"] = "";
                    drNew["QTY"] = "";
                    drNew["MATERIAL"] = "";
                    drNew["LABOR"] = "";
                    drNew["EXT. PRICE"] = nIncentive.ToString("c");
                    tmpTable.Rows.Add(drNew);

                    nTotalIncentive += nIncentive;

                }

                adjustedTotal = GSubsoldTotalCost - nTotalIncentive;

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "SUMMARY";
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Materials:";
                drNew["EXT. PRICE"] = GSubsoldTotalMaterial.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Labor:";
                drNew["EXT. PRICE"] = GSubsoldTotalLabor.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Project Total w/o Incentives:";
                drNew["EXT. PRICE"] = GSubsoldTotalCost.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Incentives Applied:";
                drNew["EXT. PRICE"] = nTotalIncentive.ToString("c");
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Grand Total:";
                drNew["EXT. PRICE"] = adjustedTotal.ToString("c");
                tmpTable.Rows.Add(drNew);


            }

            #endregion

            Session.Add("SoldReport", tmpTable);
            string strUrl = "ProjectSoldReport.aspx?TypeId=1";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
        }
        else
        {
            #region Cost by Location
            decimal GSubsoldTotalCost = 0;
            decimal GSubsoldTotalMaterial = 0;
            decimal GSubsoldTotalLabor = 0;

            DataView dvsecsold = new DataView(dtSoldMaster);
            dvsecsold.Sort = "section_level, item_id";
            DataTable dtSoldResults = dvsecsold.ToTable(true, "section_level", "section_name");

            DataTable tblSoldList = (DataTable)Session["sSoldList"];
            DataRow drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Project as Sold  by Section";
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Job# " + lblJobNumber.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Customer Name: " + lblCustomerName.Text;
            tmpTable.Rows.Add(drNew);

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "Estimate: " + lblEstimateName.Text;
            tmpTable.Rows.Add(drNew);

            foreach (DataRow dr in dtSoldResults.Rows)
            {
                decimal dSubTotalCost = 0;
                decimal dSubTotalMaterial = 0;
                decimal dSubTotalLabor = 0;

                int nSection_level = Convert.ToInt32(dr["section_level"]);
                string strSection = dr["section_name"].ToString();

                drNew = tmpTable.NewRow();
                drNew["ITEM NAME"] = "";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "Section: " + strSection;
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "ITEM ID";
                drNew["SL"] = "SL";
                drNew["SECTION"] = "LOCATION";
                drNew["ITEM NAME"] = "ITEM NAME";
                drNew["SHORT NOTES"] = "SHORT NOTES";
                drNew["UOM"] = "UOM";
                drNew["QTY"] = "QTY";
                drNew["MATERIAL"] = "MATERIAL";
                drNew["LABOR"] = "LABOR";
                drNew["EXT. PRICE"] = "EXT. PRICE";
                tmpTable.Rows.Add(drNew);

                bool Iexists = tblSoldList.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(nSection_level)).Count() > 0;
                if (Iexists)
                {
                    var rows = tblSoldList.Select("section_level =" + nSection_level + "");
                    foreach (var row in rows)
                    {

                        decimal dLineItemTotal = 0;
                        decimal dLineItemMaterial = 0;
                        decimal dLineItemLabor = 0;

                        string sItemName = row["item_name"].ToString();

                        decimal dQuantity = Convert.ToDecimal(row["quantity"]);
                        decimal dlabor_rate = Convert.ToDecimal(row["labor_rate"]);
                        decimal dretail_multiplier = Convert.ToDecimal(row["retail_multiplier"]);
                        decimal ditem_cost = Convert.ToDecimal(row["item_cost"]);
                        int nLaborID = Convert.ToInt32(row["labor_id"]);
                        dLineItemTotal = Convert.ToDecimal(row["total_retail_price"]);
                        if (nLaborID == 1)
                        {
                            dLineItemLabor = 0;
                            dLineItemMaterial = dLineItemTotal;

                        }
                        else
                        {
                            if (ditem_cost == 0)
                            {
                                dLineItemLabor = dLineItemTotal;
                                dLineItemMaterial = 0;

                            }
                            else
                            {
                                dLineItemLabor = (dlabor_rate * dretail_multiplier) * dQuantity;
                                dLineItemMaterial = dLineItemTotal - dLineItemLabor;

                            }

                        }
                        dSubTotalLabor += dLineItemLabor;
                        dSubTotalMaterial += dLineItemMaterial;
                        dSubTotalCost += dLineItemTotal;

                        GSubsoldTotalLabor += dLineItemLabor;
                        GSubsoldTotalMaterial += dLineItemMaterial;
                        GSubsoldTotalCost += dLineItemTotal;

                        drNew = tmpTable.NewRow();

                        drNew["ITEM ID"] = row["item_id"].ToString();
                        drNew["SL"] = row["section_serial"].ToString();
                        drNew["SECTION"] = row["location_name"];
                        drNew["ITEM NAME"] = sItemName;
                        drNew["SHORT NOTES"] = row["short_notes"];
                        drNew["UOM"] = row["measure_unit"];
                        drNew["QTY"] = row["quantity"].ToString();
                        drNew["MATERIAL"] = dLineItemMaterial.ToString("c");
                        drNew["LABOR"] = dLineItemLabor.ToString("c");
                        drNew["EXT. PRICE"] = dLineItemTotal.ToString("c");
                        tmpTable.Rows.Add(drNew);

                    }
                }

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "Sub Total:";
                drNew["MATERIAL"] = dSubTotalMaterial.ToString("c");
                drNew["LABOR"] = dSubTotalLabor.ToString("c");
                drNew["EXT. PRICE"] = dSubTotalCost.ToString("c");
                tmpTable.Rows.Add(drNew);

            }

            drNew = tmpTable.NewRow();
            drNew["ITEM ID"] = "";
            drNew["SL"] = "";
            drNew["SECTION"] = "";
            drNew["ITEM NAME"] = "";
            drNew["SHORT NOTES"] = "";
            drNew["UOM"] = "";
            if (Session["sIncentivesList"] != null)
            {
                drNew["QTY"] = "Project Total:";
            }
            else
            {
                drNew["QTY"] = "Grand Total:";
            }
            drNew["MATERIAL"] = GSubsoldTotalMaterial.ToString("c");
            drNew["LABOR"] = GSubsoldTotalLabor.ToString("c");
            drNew["EXT. PRICE"] = GSubsoldTotalCost.ToString("c");
            tmpTable.Rows.Add(drNew);

            if (Session["sIncentivesList"] != null)
            {
                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "PROMOTIONS & INCENTIVES";
                tmpTable.Rows.Add(drNew);

                DataTable dtInc = (DataTable)Session["sIncentivesList"];
                decimal adjustedTotal = 0;
                decimal nTotalIncentive = 0;
                int i = 0;
                foreach (DataRow dr in dtInc.Rows)
                {
                    i++;
                    decimal nIncentive = 0;
                    string strincentive_name = dr["incentive_name"].ToString();
                    decimal IncPer = Convert.ToDecimal(dr["discount"]);
                    decimal IncAmount = Convert.ToDecimal(dr["amount"]);
                    int ntype = Convert.ToInt32(dr["incentive_type"]);
                    if (ntype == 1)
                        nIncentive = Convert.ToDecimal(GSubsoldTotalCost * IncPer / 100);
                    else
                        nIncentive = IncAmount;

                    // nIncentive = Convert.ToDecimal(GSubsoldTotalCost * IncPer / 100); //Without Type

                    drNew = tmpTable.NewRow();
                    drNew["ITEM ID"] = "";
                    drNew["SL"] = "";
                    if (dtInc.Rows.Count > 1)
                    {
                        drNew["SECTION"] = "Incentive# " + i;
                    }
                    else
                    {
                        drNew["SECTION"] = "Incentive";
                    }
                    drNew["ITEM NAME"] = strincentive_name;
                    drNew["SHORT NOTES"] = "";
                    drNew["UOM"] = "";
                    drNew["QTY"] = "";
                    drNew["MATERIAL"] = "";
                    drNew["LABOR"] = "";
                    drNew["EXT. PRICE"] = nIncentive.ToString("c");
                    tmpTable.Rows.Add(drNew);

                    nTotalIncentive += nIncentive;

                }

                adjustedTotal = GSubsoldTotalCost - nTotalIncentive;

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "SUMMARY";
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Materials:";
                drNew["EXT. PRICE"] = GSubsoldTotalMaterial.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Labor:";
                drNew["EXT. PRICE"] = GSubsoldTotalLabor.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Project Total w/o Incentives:";
                drNew["EXT. PRICE"] = GSubsoldTotalCost.ToString("c");
                tmpTable.Rows.Add(drNew);

                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Total Incentives Applied:";
                drNew["EXT. PRICE"] = nTotalIncentive.ToString("c");
                tmpTable.Rows.Add(drNew);


                drNew = tmpTable.NewRow();
                drNew["ITEM ID"] = "";
                drNew["SL"] = "";
                drNew["SECTION"] = "";
                drNew["ITEM NAME"] = "";
                drNew["SHORT NOTES"] = "";
                drNew["UOM"] = "";
                drNew["QTY"] = "";
                drNew["MATERIAL"] = "";
                drNew["LABOR"] = "Grand Total:";
                drNew["EXT. PRICE"] = adjustedTotal.ToString("c");
                tmpTable.Rows.Add(drNew);


            }

            #endregion

            Session.Add("SoldReport", tmpTable);
            string strUrl = "ProjectSoldReport.aspx?TypeId=2";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + strUrl + "','_blank')", true);
        }



    }

    protected void grdGrouping_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "sEmail")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
         
            string secName = "";
            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;
            string sSubject = "Request for Bid - " + secName+" (" + hdnLastName.Value + ": " + lblJobNumber.Text + ")";
            hdnSecName.Value = secName;
            Session.Add("sSubject", sSubject);
            string sFileName = CreateSectionReportForEMail(nSecId);
            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));

            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }

    }


    protected void grdGroupingDirect_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "sEmail2")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable dt = new DataTable();
            int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));

            int index = Convert.ToInt32(e.CommandArgument);
            int nSecId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
           
            string secName = "";
            sectioninfo si = new sectioninfo();
            si = _db.sectioninfos.FirstOrDefault(com => com.section_id == nSecId);
            if (si != null)
                secName = si.section_name;
            string sSubject = "Request for Bid - " + secName + " (" + hdnLastName.Value + ": " + lblJobNumber.Text + ")";
            hdnSecName.Value = secName;
            Session.Add("sSubject", sSubject);
            string sFileName = CreateSectionReportForEMail(nSecId);
            if (Session["sPricingMailBody"] != null)
            {
                List<PricingDetailModel> CList = (List<PricingDetailModel>)Session["sPricingMailBody"];
                dt = SessionInfo.LINQToDataTable(CList);
            }
            else
            {
                string strQ = string.Empty;

                strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                            " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


                List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
                dt = SessionInfo.LINQToDataTable(CList);
            }
            Session.Add("sBody", CreateHtml(dt, nCid, secName));
            ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow('" + sFileName + "')", true);
        }

    }
    string CreateHtml(DataTable dt, int ncid, string secName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        // Customer Address
        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == ncid);
        string strCustomer = cust.last_name1;
        string strAddress = cust.city + ", " + cust.state + " " + cust.zip_code;
        string address = cust.city + ",+" + cust.state + ",+" + cust.zip_code;
        string strPO = "";
        string strEmailSignature = "";
        string strCompanyName = "";

        company_profile com = new company_profile();
        if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        {
            com = _db.company_profiles.Single(cp => cp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            strCompanyName = com.company_name;
        }

        userinfo obj = new userinfo();
        if ((userinfo)Session["oUser"] != null)
        {
            obj = (userinfo)Session["oUser"];
        }
        if (obj.user_id > 0)
        {
            user_info objuser = _db.user_infos.Where(u => u.user_id == obj.user_id).SingleOrDefault();

            if (objuser != null)
            {
                strEmailSignature = objuser.EmailSignature ?? "";
            }
        }
        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
        {
            customer_estimate cus_est = new customer_estimate();
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            //strPO = cus_est.job_number;
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strPO = cus_est.job_number;
            else
                strPO = cus_est.alter_job_number;
        }

        string url = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

        DataView dvFinal = dt.DefaultView;
        //  dvFinal.Sort = "location_name,week_id";
        string strHTML = "Dear Vendor:<br/><br/>";

        strHTML += strCompanyName + " is requesting a bid for the following (see PDF attachment). Please reply to this email with the amount and any comment as soon as possible. Please attach any document if necessary. <br/>  Thank you for your participation.<br/><br/>";

        strHTML += strEmailSignature;

        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='center'> <tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Last Name: " + strCustomer + "</p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'><a style='color:#000;' target='_blank' href='" + url + "'>Job Location: " + strAddress + "</a></p> </td></tr>";
        strHTML += "<tr><td align='left' valign='top'><p style='color:#000; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>Job Number: " + strPO + "</p> </td></tr>";
        strHTML += " <tr style='background-color:#333333; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'><td align='left' valign='top'><table width='100%' border='0' cellspacing='1' cellpadding='5' > <tr style='background-color:#333333; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='6%'>SL</td><td  width='12%' >Section</td><td width='12%'>Location</td><td width='40%'>Item Name</td><td width='5%'>UoM</td><td align='right'width='5%'>Code</td><td width='20%'>Short Notes</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string strColor = "";

            if (i % 2 == 0)
                strColor = "background-color:#f0eae8; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#333333; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";

            strHTML += "<tr style='" + strColor + "'><td>" + dr["section_serial"].ToString() + "</td><td>" + dr["section_name"].ToString() + "</td><td>" + dr["location_name"].ToString() + "</td><td>" + dr["item_name"].ToString() + "</td><td>" + dr["measure_unit"].ToString() + "</td><td align='right'>" + Convert.ToInt32(dr["quantity"]).ToString() + "</td><td>" + dr["short_notes"].ToString() + "</td></tr>";

        }
        strHTML += "</table> </td></tr>";
        strHTML += "</table><br/><br/><br/>";



        return strHTML;
    }

    private string CreateSectionReportForEMail(int nSecId)
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


        string strQ = string.Empty;

        //strQ = " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
        //      " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
        //      " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + nEstId + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + nEstId + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 1 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  " +
        //      " UNION " +
        //      " SELECT co_pricing_list_id as pricing_id, co_pricing_master.client_id, customer_id, estimate_id, co_pricing_master.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt,prev_total_price AS total_direct_price, prev_total_price AS total_retail_price, is_direct, 'A' AS pricing_type, short_notes,location_name " +
        //      " FROM co_pricing_master  INNER JOIN location ON co_pricing_master.location_id=location.location_id AND co_pricing_master.client_id=location.client_id " +
        //      " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + nEstId + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections  WHERE changeorder_sections.estimate_id =" + nEstId + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND item_status_id = 2 AND co_pricing_master.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        //List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        //if (CList.Count == 0)
        //{
        //    strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
        //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
        //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);


        //    CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        //}
        strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND section_level=" + nSecId + " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();


        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptEmailReportBySection.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        //string strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName2 = "";
        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
            strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        else
            strCustomerName2 = cus_est.alter_job_number;
        string strCustomerName = objCust.last_name1; // objCust.first_name1 + " " + objCust.last_name1;
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
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_LeadTime", "");
        ht.Add("p_Contractdate", "");
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);
        ht.Add("p_date", "");
        ht.Add("p_customername2", strCustomerName2);
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

            return exportReportForSection(rptFile, CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        }
        catch
        {

        }


        return "";
    }
    protected string exportReportForSection(CrystalDecisions.CrystalReports.Engine.ReportDocument selectedReport, CrystalDecisions.Shared.ExportFormatType eft)
    {

        string strFile = "";
        try
        {
            string contentType = "";
            //strFile = "SECTION_" + DateTime.Now.Ticks;
            strFile = hdnLastName.Value + "-" + hdnSecName.Value + "_" + DateTime.Now.Ticks;

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

    protected void btnPricingPrint_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPricingPrint.ID, btnPricingPrint.GetType().Name, "Click"); 
        int nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeId"));
        int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
        hdnCustomerId.Value = nCid.ToString();

        Session.Add("CustomerId", hdnCustomerId.Value);

        int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
        hdnEstimateId.Value = nEstId.ToString();

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        string strQ = string.Empty;

        strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(hdnClientId.Value);


        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();

        string strPsumm = string.Empty;
        if (rdoSort.SelectedValue == "1")
        {
            strPsumm = " SELECT  t1.location_id as MainID,lc.location_name  AS MainName,t3.ProjectCost, max(t1.sort_id) as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + t2.section_name  " +
                    " FROM pricing_details t2  " +
                    " WHERE t2.location_id = t1.location_id AND t2.estimate_id = " + nEstId + " AND t2.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  " +
                    "FOR XML PATH (''))  " +
                    "  ,1,2,'') AS SummaryName  " +
                    " FROM pricing_details t1   " +
                    " INNER JOIN location lc ON lc.location_id = t1.location_id  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,location_id from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id= " + nEstId + " and  " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id = " + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id = " + Convert.ToInt32(hdnClientId.Value) + " )  " +
                    " GROUP BY location_id ) AS t3 on t3.location_id = t1.location_id  " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " and t1.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id = " + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + ")" +
                    " GROUP BY t1.location_id,lc.location_name,t3.ProjectCost; ";
        }
        else
        {
            strPsumm = " SELECT  t1.section_level as MainID,t1.section_name AS MainName,t3.ProjectCost,1 as SortId, " +
                    " STUFF(( " +
                    " SELECT DISTINCT ', ' + lc.location_name  " +
                    " FROM pricing_details t2 " +
                    " INNER JOIN location lc ON lc.location_id = t2.location_id " +
                    " WHERE t2.section_level = t1.section_level AND t2.estimate_id =" + nEstId + " AND t2.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " " +
                    " FOR XML PATH ('')) " +
                    " ,1,2,'') AS SummaryName " +
                    " FROM pricing_details t1  " +
                    " INNER JOIN (SELECT SUM(total_retail_price) AS ProjectCost,section_level from pricing_details  WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  estimate_id=" + nEstId + " and " +
                    " pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + ")" +
                    " GROUP BY section_level ) AS t3 on t3.section_level = t1.section_level " +
                    " WHERE t1.estimate_id = " + nEstId + " AND t1.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and t1.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " )" +
                    " GROUP BY t1.section_level,t1.section_name,t3.ProjectCost;";


        }

        List<PMainSummary> PSummList = _db.ExecuteQuery<PMainSummary>(strPsumm, string.Empty).ToList();

        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == nEstId);

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = string.Empty;

        if (rdoSort.SelectedValue == "1")
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportByLocation.rpt");
        else if (rdoSort.SelectedValue == "2")
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportBySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\rptPricingReportByLocation.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);

        ReportDocument subReport = rptFile.OpenSubreport("rptSummaryPricing.rpt");
        subReport.SetDataSource(PSummList);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        //string strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName2 = "";
        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
            strCustomerName2 = cus_est.job_number;//objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        else
            strCustomerName2 = cus_est.alter_job_number;
        string strCustomerName = objCust.last_name1; // objCust.first_name1 + " " + objCust.last_name1;
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
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_LeadTime", "");
        ht.Add("p_Contractdate", "");
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);
        ht.Add("p_date", "");
        ht.Add("p_customername2", strCustomerName2);
        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);


        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }

    protected void btnTemplateEstimate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnTemplateEstimate.ID, btnTemplateEstimate.GetType().Name, "Click"); 
        lblResult1.Text = "";
        try
        {
            string est = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {

                List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
                List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)).ToList();
                List<pricing_detail> Pd_List = _db.pricing_details.Where(pd => pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A").ToList();
                estimate_payment objes = _db.estimate_payments.SingleOrDefault(cs => cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value));

                int nEstId = 0;
                var result = (from mest in _db.model_estimates
                              where mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(hdnClientId.Value)
                              select mest.model_estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
                nEstId = nEstId + 1;
                hdnModelEstimateId.Value = nEstId.ToString();
                model_estimate me = new model_estimate();
                me.client_id = Convert.ToInt32(hdnClientId.Value);
                me.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                me.status_id = 1;
                me.estimate_comments = "";
                me.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                est = "Estimate Template" + " " + hdnModelEstimateId.Value;
                me.model_estimate_name = est;
                me.create_date = DateTime.Now;
                me.last_udated_date = DateTime.Now;
                me.IsPublic = false;
                _db.model_estimates.InsertOnSubmit(me);
                _db.SubmitChanges();
                foreach (customer_location objcl in Cust_LocList)
                {
                    model_estimate_location mod_loc = new model_estimate_location();
                    mod_loc.client_id = objcl.client_id;
                    mod_loc.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    mod_loc.location_id = objcl.location_id;
                    mod_loc.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    _db.model_estimate_locations.InsertOnSubmit(mod_loc);
                }
                foreach (customer_section objcs in Cust_SecList)
                {
                    model_estimate_section mod_sec = new model_estimate_section();
                    mod_sec.client_id = objcs.client_id;
                    mod_sec.section_id = objcs.section_id;
                    mod_sec.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    mod_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    _db.model_estimate_sections.InsertOnSubmit(mod_sec);
                }
                foreach (pricing_detail objPd in Pd_List)
                {
                    model_estimate_pricing pd = new model_estimate_pricing();
                    pd.client_id = objPd.client_id; ;
                    pd.model_estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    pd.location_id = objPd.location_id; ;
                    pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    pd.section_level = objPd.section_level;
                    pd.item_id = objPd.item_id;
                    pd.section_name = objPd.section_name;
                    pd.item_name = objPd.item_name;
                    pd.measure_unit = objPd.measure_unit;
                    pd.minimum_qty = objPd.minimum_qty;
                    pd.quantity = objPd.quantity;
                    pd.retail_multiplier = objPd.retail_multiplier;
                    pd.labor_id = objPd.labor_id;
                    pd.is_direct = objPd.is_direct;
                    pd.item_cost = objPd.item_cost;
                    pd.total_direct_price = objPd.total_direct_price;
                    pd.total_retail_price = objPd.total_retail_price;
                    pd.labor_rate = objPd.labor_rate;
                    pd.section_serial = objPd.section_serial;
                    pd.item_cnt = objPd.item_cnt;
                    pd.short_notes = objPd.short_notes;
                    pd.pricing_type = objPd.pricing_type;
                    pd.create_date = DateTime.Now;
                    pd.last_updated_date = DateTime.Now;
                    pd.sort_id = objPd.sort_id;
                    _db.model_estimate_pricings.InsertOnSubmit(pd);
                }

                _db.SubmitChanges();



                // Estimate Payment 
                if (objes != null)
                {
                    model_estimate_payment mod_es = new model_estimate_payment();
                    mod_es.est_payment_id = objes.est_payment_id;
                    mod_es.client_id = objes.client_id;
                    mod_es.estimate_id = Convert.ToInt32(hdnModelEstimateId.Value);
                    mod_es.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    mod_es.project_subtotal =objes.project_subtotal;
                    mod_es.tax_rate = objes.tax_rate;
                    mod_es.tax_amount = objes.tax_amount;
                    mod_es.total_with_tax =objes.total_with_tax;
                    mod_es.adjusted_price = objes.adjusted_price;
                    mod_es.adjusted_tax_amount = objes.adjusted_tax_amount;
                    mod_es.adjusted_tax_rate = objes.adjusted_tax_rate;
                    mod_es.new_total_with_tax = objes.new_total_with_tax;
                    mod_es.is_incentives = objes.is_incentives;
                    mod_es.total_incentives = objes.total_incentives;
                    mod_es.incentive_ids = objes.incentive_ids;
                    mod_es.deposit_percent = objes.deposit_percent;
                    mod_es.deposit_amount = objes.deposit_amount;
                    mod_es.countertop_percent = objes.countertop_percent;
                    mod_es.countertop_amount = objes.countertop_amount;
                    mod_es.start_job_percent = objes.start_job_percent;
                    mod_es.start_job_amount = objes.start_job_amount;
                    mod_es.due_completion_percent = objes.due_completion_percent;
                    mod_es.due_completion_amount = objes.due_completion_amount;
                    mod_es.final_measure_percent = objes.final_measure_percent;
                    mod_es.final_measure_amount = objes.final_measure_amount;
                    mod_es.deliver_caninet_percent = objes.deliver_caninet_percent;
                    mod_es.deliver_cabinet_amount = objes.deliver_cabinet_amount;
                    mod_es.substantial_percent = objes.substantial_percent;
                    mod_es.substantial_amount = objes.substantial_amount;
                    mod_es.other_value = objes.other_value;
                    mod_es.other_percent = objes.other_percent;
                    mod_es.other_amount = objes.other_amount;
                    mod_es.based_on_percent = objes.based_on_percent;
                    mod_es.based_on_dollar = objes.based_on_dollar;
                    mod_es.create_date = objes.create_date;
                    mod_es.updated_date = objes.updated_date;
                  
                    mod_es.deposit_date = objes.deposit_date;
                    mod_es.countertop_date = objes.countertop_date;
                    mod_es.startof_job_date = objes.startof_job_date;
                    mod_es.due_completion_date = objes.due_completion_date;
                    mod_es.measure_date = objes.measure_date;
                    mod_es.delivery_date = objes.deposit_date;
                    mod_es.substantial_date = objes.substantial_date;
                    mod_es.other_date = objes.other_date;
                    mod_es.lead_time = objes.lead_time;
                    mod_es.contract_date = objes.contract_date;
                    mod_es.deposit_value = objes.deposit_value;
                    mod_es.countertop_value = objes.countertop_value;
                    mod_es.start_job_value = objes.start_job_value;
                    mod_es.due_completion_value = objes.due_completion_value;
                    mod_es.final_measure_value = objes.final_measure_value;
                    mod_es.deliver_caninet_value = objes.deliver_caninet_value;
                    mod_es.substantial_value = objes.substantial_value;
                    mod_es.special_note = objes.special_note;
                    mod_es.start_date = objes.start_date;
                    mod_es.completion_date = objes.completion_date;
                    mod_es.is_KithenSheet = objes.is_KithenSheet;
                    mod_es.is_BathSheet = objes.is_BathSheet;
                    mod_es.is_ShowerSheet = objes.is_ShowerSheet;
                    mod_es.is_TubSheet = objes.is_TubSheet;
                    mod_es.drywall_percent = objes.drywall_percent;
                    mod_es.drywall_amount = objes.drywall_amount;
                    mod_es.drywall_date = objes.drywall_date;
                    mod_es.drywall_value = objes.drywall_value;
                    mod_es.flooring_percent = objes.flooring_percent;
                    mod_es.flooring_amount = objes.flooring_amount;
                    mod_es.flooring_date = objes.flooring_date;
                    mod_es.flooring_value = objes.flooring_value;
                    _db.model_estimate_payments.InsertOnSubmit(mod_es);
                    _db.SubmitChanges();
                }

            }
            Session.Add("sSytemMassage", "Estimate has been saved as a Template (" + est + ") successfully.");
            Response.Redirect("sold_estimate.aspx?eid=" + Convert.ToInt32(hdnEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
        }
        catch (Exception ex)
        {
            lblResult1.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
       
        

    }

    protected void btnSaveAlterJob_Click(object sender, EventArgs e)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            if (txtAlterJobNumber.Text == "")
            {
                lblJobMSG.Text = csCommonUtility.GetSystemErrorMessage("Missing alternate job number");
                return;
            }
            customer_estimate cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            if (cus_est.alter_job_number != null)
            {
                if (txtAlterJobNumber.Text.ToLower().Trim() != cus_est.alter_job_number.ToLower().Trim())
                {
                    if (_db.customer_estimates.Where(sp => sp.alter_job_number.ToLower().Trim() == txtAlterJobNumber.Text.ToLower().Trim()).SingleOrDefault() != null)
                    {
                        lblJobMSG.Text = csCommonUtility.GetSystemErrorMessage("Alternate job number is used another estimate. Please try another.");
                        return;
                    }
                }
            }
            else
            {
                if (_db.customer_estimates.Where(sp => sp.alter_job_number.ToLower().Trim() == txtAlterJobNumber.Text.ToLower().Trim()).SingleOrDefault() != null)
                {
                    lblJobMSG.Text = csCommonUtility.GetSystemErrorMessage("Alternate job number is used another estimate. Please try another.");
                    return;
                }
            }


            string strQ = "update customer_estimate set alter_job_number='" + txtAlterJobNumber.Text.Trim() + "'WHERE  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND client_id=" + Convert.ToInt32(hdnClientId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            lblJobMSG.Text = csCommonUtility.GetSystemMessage("Alternate job number has been saved successfully.");
        }
        catch (Exception ex)
        {
            lblJobMSG.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        if (txtNewEstimateName.Text.Trim() == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("New Estimate Name is required field");

            modUpdateEstimate.Show();
            return;
        }

        string strNewName = txtNewEstimateName.Text.Trim();
        DataClassesDataContext _db = new DataClassesDataContext();
        customer_estimate cus_est = new customer_estimate();
        int ncid = Convert.ToInt32(hdnCustomerId.Value);
        int nestId = Convert.ToInt32(hdnEstimateId.Value);
        if (Convert.ToInt32(hdnCustomerId.Value) > 0 && Convert.ToInt32(hdnEstimateId.Value) > 0)
            cus_est = _db.customer_estimates.Single(ce => ce.customer_id == ncid && ce.estimate_id == nestId && ce.client_id == Convert.ToInt32(hdnClientId.Value));

        string oldName = cus_est.estimate_name;
        cus_est.estimate_name = strNewName;


        _db.SubmitChanges();


        // Rename estimate folder name
        //try
        //{
        //    string sPath = Server.MapPath("~/UploadedFiles//") + hdnCustomerId.Value + "//ESTIMATES & COs//";

        //    System.IO.Directory.Move(sPath + "//" + oldName, sPath + "//" + strNewName);
        //}
        //catch (Exception ex)
        //{
        //    string sError = ex.Message;
        //}
        lblMessage.Text = "";
        txtNewEstimateName.Text = "";
        modUpdateEstimate.Hide();

        Response.Redirect("sold_estimate.aspx?eid=" + nestId + "&cid=" + ncid);
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        // KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnClose.ID, btnClose.GetType().Name, "Close Click");
        lblMessage.Text = "";
        txtNewEstimateName.Text = "";
        modUpdateEstimate.Hide();
    }
}
