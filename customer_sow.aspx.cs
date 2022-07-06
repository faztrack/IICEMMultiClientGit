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

public partial class customer_sow : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oCustomerUser"] == null)
            {
                Response.Redirect("customerlogin.aspx");
            }
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();
            //GetEstimate(nCid);
            Session.Add("CustomerId", hdnCustomerId.Value);
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;

                hdnClientId.Value = cust.client_id.ToString();

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


                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;

                    string strStatus = "";
                    if (cus_est.status_id == 1)
                    {
                        strStatus = "Pending";
                    }
                    else if (cus_est.status_id == 2)
                    {
                        strStatus = "Sit";
                    }
                    else if (cus_est.status_id == 3)
                    {
                        strStatus = "Sold";
                    }
                    lblStatus.Text = strStatus;

                    lblSaleDate.Visible = true;
                    lblSaleDateValue.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                    lblSaleDateValue.Visible = true;

                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";
                    txtComments.ReadOnly = true;

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
                Calculate();
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
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)                        
                            grdGroupingDirect.FooterRow.Visible = true;
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

            GetEstimate(nCid);
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerdashboard.aspx");
    }


    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (rdoSort.SelectedValue == "2")
        {
            string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
            if ((dtsec == null) || (dtsec.Rows.Count == 0))
            {
                dtsec = LoadMasterTable();
            }
            string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =  " + Convert.ToInt32(hdnClientId.Value) +
                            " order by section_level asc";
            DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
            DataRow drNew = null;
            if (dtsec2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtsec2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    if (!ContainDataRowInDataTable(dtsec, dr))
                    {
                        drNew = dtsec.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtsec.Rows.Add(drNew);
                    }
                }
            }
            grdGrouping.DataSource = dtsec;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();
        }
        else
        {
            string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
            if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
            {
                dtLoc = LoadMasterTable();
            }
            string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                            " INNER JOIN location on location.location_id = cop.location_id " +
                            " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                            " AND is_direct=1 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + " order by colName asc ";
            DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
            DataRow drNew = null;
            if (dtLoc2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtLoc2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    if (!ContainDataRowInDataTable(dtLoc, dr))
                    {
                        drNew = dtLoc.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtLoc.Rows.Add(drNew);
                    }
                }
            }
            grdGrouping.DataSource = dtLoc;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();
        }


    }
    bool ContainDataRowInDataTable(DataTable T, DataRow R)
    {
        foreach (DataRow item in T.Rows)
        {
            for (int i = 0; i < item.ItemArray.Length; i++)
            {
                if (Enumerable.SequenceEqual(item.ItemArray, R.ItemArray))
                    return true;
            }
        }
        return false;
    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (rdoSort.SelectedValue == "2")
        {
            string strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
            DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
            if ((dtsec == null) || (dtsec.Rows.Count == 0))
            {
                dtsec = LoadMasterTable();
            }
            string strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =  " + Convert.ToInt32(hdnClientId.Value) +
                            " order by section_level asc";
            DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
            DataRow drNew = null;
            if (dtsec2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtsec2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    if (!ContainDataRowInDataTable(dtsec, dr))
                    {
                        drNew = dtsec.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtsec.Rows.Add(drNew);
                    }
                }
            }
            grdGroupingDirect.DataSource = dtsec;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
        else
        {
            string strQ3 = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " order by colName asc";
            DataTable dtLoc = csCommonUtility.GetDataTable(strQ3);
            if ((dtLoc == null) || (dtLoc.Rows.Count == 0))
            {
                dtLoc = LoadMasterTable();
            }
            string strQ4 = " select DISTINCT cop.location_id AS colId,'LOCATION: '+ location.location_name as colName from change_order_pricing_list cop " +
                            " INNER JOIN location on location.location_id = cop.location_id " +
                            " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 " +
                            " AND is_direct = 2 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + " order by colName asc ";
            DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
            DataRow drNew = null;
            if (dtLoc2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtLoc2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    if (!ContainDataRowInDataTable(dtLoc, dr))
                    {
                        drNew = dtLoc.NewRow();
                        drNew["colId"] = ncolId;
                        drNew["colName"] = strColName;
                        dtLoc.Rows.Add(drNew);
                    }
                }
            }
            grdGroupingDirect.DataSource = dtLoc;
            grdGroupingDirect.DataKeyNames = new string[] { "colId" };
            grdGroupingDirect.DataBind();
        }
    }
    private void Calculate()
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes
            //lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes

        }
        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == Convert.ToInt32(hdnClientId.Value)
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecievedAmount.Text = payAmount.ToString("c");

        decimal TotalCOAmount = 0;
        var COitem = from co in _db.changeorder_estimates
                     where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                     orderby co.changeorder_name ascending
                     select co;
        foreach (changeorder_estimate cho in COitem)
        {
            int ncoeid = cho.chage_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            CoTaxRate = Convert.ToDecimal(cho.tax);
            decimal dEconCost = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int cn = Coresult.Count();
            if (Coresult != null && cn > 0)
                dEconCost = Coresult.Sum();

            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;

            }
            else
            {
                CoPrice = dEconCost;
            }
            TotalCOAmount += CoPrice;

        }
        lblTotalCOAmount.Text = TotalCOAmount.ToString("c");

        decimal TotalCostAmount = 0;
        TotalCostAmount = totalwithtax + TotalCOAmount;
        lblTotalAmount.Text = TotalCostAmount.ToString("c");

        decimal TotalBalanceAmount = 0;
        TotalBalanceAmount = TotalCostAmount - payAmount;
        lblTotalBalanceAmount.Text = TotalBalanceAmount.ToString("c");

    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblshort_notes = (Label)e.Row.Cells[5].FindControl("lblshort_notes");
            string str = lblshort_notes.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                lblshort_notes.ToolTip = str;
                lblshort_notes.Text = str.Substring(0, 25) + "...";
            }
            else
            {
                lblshort_notes.ToolTip = str;
                lblshort_notes.Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[0]);
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Text = "Item Deleted" + e.Row.Cells[5].Text;

            }
            else if (nItemStatusId == 3)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    Label lblT_price1 = (Label)e.Row.Cells[4].FindControl("lblT_price1");
                    lblT_price1.Text = "0.00";
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Cells[5].Text = "Item Added" + e.Row.Cells[5].Text;
            }
            else
            {
                e.Row.Cells[5].Text = "No Change";
            }




        }

    }
    private DataTable LoadMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("colId", typeof(int));
        table.Columns.Add("colName", typeof(string));
        return table;
    }

    protected string GetTotalPrice()
    {
        decimal nGrandtotal = Convert.ToDecimal(grandtotal);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);

        return "Total with Tax: " + grandtotal.ToString("c");
    }
    protected string GetTotalPriceDirect()
    {
        decimal nGrandtotal_direct = Convert.ToDecimal(grandtotal_direct);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);

        return "Total with Tax: " + grandtotal_direct.ToString("c");
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            if (Session["Item_list"] != null)
            {
                DataTable dtItemdata = (DataTable)Session["Item_list"];
                DataView dv = dtItemdata.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date ASC";


                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
                gv.DataBind();
            }
            else
            {
                int nDirectId = 1;
                GetData(colId, gv, nDirectId);
            }
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;

            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("lblT_price1") as Label).Text);
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
            grandtotal += subtotal;
            subtotal = 0.0;



        }

    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (rdoSort.SelectedValue == "1")
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                    " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                    " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = " + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " order by section_level";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                           " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                           " FROM change_order_pricing_list " +
                           " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                           " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {
                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

                drNew = dt.NewRow();
                drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
                drNew["item_id"] = dr["item_id"];
                drNew["labor_id"] = dr["labor_id"];
                drNew["section_serial"] = dr["section_serial"];
                drNew["location_name"] = dr["location_name"];
                drNew["section_name"] = dr["section_name"];
                drNew["item_name"] = dr["item_name"];
                drNew["measure_unit"] = dr["measure_unit"];
                drNew["item_cost"] = dr["item_cost"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["minimum_qty"] = dr["minimum_qty"];
                drNew["quantity"] = dr["quantity"];
                drNew["retail_multiplier"] = dr["retail_multiplier"];
                drNew["labor_rate"] = dr["labor_rate"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                dt.Rows.Add(drNew);

            }
            if (nDirectId == 1)
                Session.Add("Item_list", dt);
            else
                Session.Add("Item_list_Direct", dt);
            DataView dv = dt.DefaultView;
            if (rdoSort.SelectedValue == "1")
            {
                dv.RowFilter = "location_id =" + colId;
            }
            else
            {
                dv.RowFilter = "section_level =" + colId;
            }
            dv.Sort = "last_update_date asc";

            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
            grd.DataBind();
        }
        else
        {

            string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                    " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo  " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = " + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " order by location.location_name";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                           " location.location_name AS section_name,section_name AS location_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                           " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                           " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                           " FROM change_order_pricing_list " +
                           " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                           " INNER JOIN changeorder_estimate  on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by location.location_name";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {
                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

                drNew = dt.NewRow();
                drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
                drNew["item_id"] = dr["item_id"];
                drNew["labor_id"] = dr["labor_id"];
                drNew["section_serial"] = dr["section_serial"];
                drNew["location_name"] = dr["location_name"];
                drNew["section_name"] = dr["section_name"];
                drNew["item_name"] = dr["item_name"];
                drNew["measure_unit"] = dr["measure_unit"];
                drNew["item_cost"] = dr["item_cost"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["minimum_qty"] = dr["minimum_qty"];
                drNew["quantity"] = dr["quantity"];
                drNew["retail_multiplier"] = dr["retail_multiplier"];
                drNew["labor_rate"] = dr["labor_rate"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                dt.Rows.Add(drNew);

            }
            if (nDirectId == 1)
                Session.Add("Item_list", dt);
            else
                Session.Add("Item_list_Direct", dt);
            DataView dv = dt.DefaultView;
            if (rdoSort.SelectedValue == "1")
            {
                dv.RowFilter = "location_id =" + colId;
            }
            else
            {
                dv.RowFilter = "section_level =" + colId;
            }

            dv.Sort = "last_update_date asc";
            grd.DataSource = dv;
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
            grd.DataBind();
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("Item_list");
        Session.Remove("Item_list_Direct");
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

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv2 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblshort_notes2 = (Label)e.Row.Cells[4].FindControl("lblshort_notes2");
            string str = lblshort_notes2.Text.Replace("&nbsp;", "");

            
            if (str != "" && str.Length > 25)
            {
                lblshort_notes2.ToolTip = str;
                lblshort_notes2.Text = str.Substring(0, 25) + "...";
            }
            else
            {
                lblshort_notes2.ToolTip = str;
                lblshort_notes2.Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[0]);
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Text = "Item Deleted" + e.Row.Cells[5].Text;
                Label lblT_price2 = (Label)e.Row.Cells[4].FindControl("lblT_price2");
                lblT_price2.Text = "0.00";

            }
            else if (nItemStatusId == 3)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_direct_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    Label lblT_price2 = (Label)e.Row.Cells[4].FindControl("lblT_price2");
                    lblT_price2.Text = "0.00";
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Cells[5].Text = "Item Added" + e.Row.Cells[5].Text;
            }
            else
            {
                e.Row.Cells[5].Text = "No Change";
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
            if (Session["Item_list_Direct"] != null)
            {
                DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
                DataView dv = dtItemDirect.DefaultView;
                if (rdoSort.SelectedValue == "1")
                {
                    dv.RowFilter = "location_id =" + colId;
                }
                else
                {
                    dv.RowFilter = "section_level =" + colId;
                }
                dv.Sort = "last_update_date DESC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id" };
                gv.DataBind();
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
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                subtotal_diect += Double.Parse((row.FindControl("lblT_price2") as Label).Text);

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
            grandtotal_direct += subtotal_diect;
            subtotal_diect = 0.0;

            
        }
    }


    protected void btnPrintByLoc_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintByLoc.ID, btnPrintByLoc.GetType().Name, "Click"); 
        GetData(1);
    }
    private void GetData(int nTypeId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
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
        }

        string strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                    " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                   " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date, " +
                   " is_direct,section_level,location.location_id,'' as tmpCo " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = "+Convert.ToInt32(hdnClientId.Value)+" ) " +
                   "order by section_level";
        DataTable dtP = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                       " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                       " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                       " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                       " FROM change_order_pricing_list " +
                       " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                       " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                       " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        DataTable dtcol = csCommonUtility.GetDataTable(strQ);
        foreach (DataRow dr in dtcol.Rows)
        {
            string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

            drNew = dtP.NewRow();
            drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
            drNew["item_id"] = dr["item_id"];
            drNew["labor_id"] = dr["labor_id"];
            drNew["section_serial"] = dr["section_serial"];
            drNew["location_name"] = dr["location_name"];
            drNew["section_name"] = dr["section_name"];
            drNew["item_name"] = dr["item_name"];
            drNew["measure_unit"] = dr["measure_unit"];
            drNew["item_cost"] = dr["item_cost"];
            drNew["total_retail_price"] = dr["total_retail_price"];
            drNew["total_direct_price"] = dr["total_direct_price"];
            drNew["minimum_qty"] = dr["minimum_qty"];
            drNew["quantity"] = dr["quantity"];
            drNew["retail_multiplier"] = dr["retail_multiplier"];
            drNew["labor_rate"] = dr["labor_rate"];
            drNew["short_notes"] = dr["short_notes"];
            drNew["item_status_id"] = dr["item_status_id"];
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["tmpCo"] = strTmp;
            dtP.Rows.Add(drNew);

        }

        DataView dv = dtP.DefaultView;
        dv.Sort = "last_update_date asc";
        userinfo obj = (userinfo)Session["oUser"];
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
        string strJob = string.Empty;
        //if (cus_est.job_number != null)
        //    strJob = cus_est.job_number;
        if (cus_est.job_number != null)
        {
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strJob = cus_est.job_number;
            else
                strJob = cus_est.alter_job_number;
        }
        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (nTypeId == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\CustCompositeSOWbyLocation.rpt");
        else if (nTypeId == 2)
            strReportPath = Server.MapPath(@"Reports\rpt\CustCompositeSOWbySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\CustCompositeSOWbySection.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dv);

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

        int IsQty = 0;
        int IsUom = 0;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
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
        ht.Add("p_customername2", strCustomerName2);
       
        ht.Add("p_Role", 1);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsUom", IsUom);
        ht.Add("p_Job", strJob);
        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);


    }

    protected void btnPrintBySec_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintBySec.ID, btnPrintBySec.GetType().Name, "Click"); 
        GetData(2);
    }

    private void GetEstimate(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select * from customer_estimate where customer_id=" + nCustId + " and client_id= " + Convert.ToInt32(hdnClientId.Value) + " and status_id = 3 order by estimate_id desc ";
        IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);




        if (clist != null)
        {
            ddlEstimate.DataSource = clist;
            ddlEstimate.DataTextField = "estimate_name";
            ddlEstimate.DataValueField = "estimate_id";
            ddlEstimate.DataBind();

            if (Request.QueryString.Get("eid") != null)
            {
                ddlEstimate.SelectedValue = Request.QueryString.Get("eid").ToString();
            }

            lblCurrentEstimate.Visible = false;
            ddlEstimate.Visible = true;
        }
        else
        {
            lblCurrentEstimate.Text = clist.FirstOrDefault().estimate_name;
            lblCurrentEstimate.Visible = true;
            ddlEstimate.Visible = false;

        }


    }

    protected void ddlEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlEstimate.ID, ddlEstimate.GetType().Name, "SelectedIndexChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();


        int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
        int nEstimateId = Convert.ToInt32(ddlEstimate.SelectedItem.Value);

        Response.Redirect("customer_sow.aspx?eid=" + nEstimateId + "&cid=" + nCustomerId);

    }
}
